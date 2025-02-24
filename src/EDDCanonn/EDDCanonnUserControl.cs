/*
 * Copyright © 2022-2022 EDDiscovery development team
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this
 * file except in compliance with the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software distributed under
 * the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
 * ANY KIND, either express or implied. See the License for the specific language
 * governing permissions and limitations under the License.
 */

using System;
using static EDDDLLInterfaces.EDDDLLIF;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using QuickJSON;
using System.Linq;
using EDDCanonn.Base;
using KdTree;
using KdTree.Math;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using System.Text.RegularExpressions;
using System.Net;

namespace EDDCanonn
{
    partial class EDDCanonnUserControl : UserControl, IEDDPanelExtension
    {

        private readonly ActionDataHandler dataHandler;
        private EDDPanelCallbacks PanelCallBack;
        private EDDCallBacks DLLCallBack;


        public EDDCanonnUserControl()
        {
            InitializeComponent();
            dataHandler = new ActionDataHandler();
            AutoScaleMode = AutoScaleMode.Inherit;
        }

        #region start

        private void StartUp() //Triggered by panel Initialize.
        {
            NotifyMainFields("Start Up...");
            try
            {
                if (CanonnHelper.InstanceCount > 0)
                {
                    CanonnHelper.InstanceCount++;
                    //Abort if a Canonn Panel already exists.
                    Abort("Only one Canonn Panel instance can be active. " +
                    "Use the main instance or close all other instances and restart EDD."); 
                    return;
                }
                else
                    CanonnHelper.InstanceCount++;

                linkLabelEDDCanonn.Text = "EDDCanonn v" + EDDCanonnEDDClass.V;

                InitializeNews();
                InitializeWhitelist();
                InitializePatrols();

                if (DLLCallBack.RequestHistory(1, false, out JournalEntry je) == true) //We check here if EDD has already loaded the history.
                {
                    DLLCallBack.RequestSpanshDump(RequestTag.OnStart, this, "", 0, true, false, null);
                    return;
                }
                else
                {
                    dataHandler.StartTaskAsync(
                    (token) =>
                    {
                        while (!historyset) //We wait until the history has been loaded.
                        {
                            token.ThrowIfCancellationRequested(); //We abort here if a cancellation was requested.
                            Task.Delay(250, token).Wait();
                        }
                        SafeInvoke(() =>
                        {
                            DLLCallBack.RequestSpanshDump(RequestTag.OnStart, this, "", 0, true, false, null);
                        });
                        return;
                    },
                    ex =>
                    {
                        string error = $"EDDCanonn: Unexpected error in \"Wait for History\" Thread: {ex.Message}";
                        Console.Error.WriteLine(error);
                        CanonnLogging.Instance.LogToFile(error);
                    },
                        "Startup : History-Listener"
                    );
                }
            }
            catch (Exception ex)
            {
                string error = $"EDDCanonn: Error during Startup: {ex.Message}";
                Console.Error.WriteLine(error);
                CanonnLogging.Instance.LogToFile(error);
            }
        }
        #endregion

        #region Patrol

        private Patrols patrols;
        private void InitializePatrols()
        {
            try
            {
                patrols = new Patrols();

                ExtComboBoxPatrol.Items.Add("All"); //The 'all' KdTree-Dictionary has already been set in “Patrols.cs”.

                ExtComboBoxRange.Items.AddRange(CanonnHelper.PatrolRanges.Select(x => x.ToString() + " LY").ToArray());

                ConcurrentBag<Task> _tasks = new ConcurrentBag<Task>(); //We have to make sure that all workers are finished before we update the patrols.

                //Start a 'head worker' to avoid blocking the UI thread.
                dataHandler.StartTaskAsync(
                (token) =>
                {
                    //Fetch information about available patrols.
                    List<Dictionary<string, string>> records = CanonnHelper.ParseTsv(dataHandler.FetchData(CanonnHelper.PatrolUrl).response);
                    foreach (Dictionary<string, string> record in records)
                    {
                        token.ThrowIfCancellationRequested(); //We abort here if a cancellation was requested.
                        try
                        {
                            string description = record.TryGetValue("Description", out string descriptionValue)
                            ? descriptionValue : "uncategorized";
                            bool enabled = record.TryGetValue("Enabled", out string enabledValue)
                            && enabledValue == "Y";
                            string type = record.TryGetValue("Type", out string typeValue)
                            ? typeValue : string.Empty;
                            string url = record.TryGetValue("Url", out string urlValue)
                            ? urlValue : string.Empty;

                            if (!enabled)
                                continue;
                            if (description.Equals("Landscape Signals"))
                                continue;

                            if (type.Equals("tsv")) //Worker for tsv files.
                            {
                                _tasks.Add(dataHandler.StartTaskAsync(
                                    (subToken) =>
                                    {
                                        CreateFromTSV(url, description, subToken);
                                    },
                                    ex =>
                                    {
                                        string error = $"EDDCanonn: Error Initialize Patrols -> {description}: {ex.Message}";
                                        Console.Error.WriteLine(error);
                                        CanonnLogging.Instance.LogToFile(error);
                                    },
                                        "InitializePatrol -> SubThread: " + description
                                    ));
                            }
                            else if (type.Equals("json")) //Worker for json files.
                            {
                                _tasks.Add(dataHandler.StartTaskAsync(
                                    (subToken) =>
                                    {
                                        CreateFromJson(url, description, subToken);
                                    },
                                    ex =>
                                    {
                                        string error = $"EDDCanonn: Error Initialize Patrols -> {description}: {ex.Message}";
                                        Console.Error.WriteLine(error);
                                        CanonnLogging.Instance.LogToFile(error);
                                    },
                                        "InitializePatrol -> SubThread: " + description
                                    ));
                            }
                        }
                        catch (Exception ex)
                        {
                            string error = $"EDDCanonn: Error processing record category: {ex.Message}";
                            Console.Error.WriteLine(error);
                            CanonnLogging.Instance.LogToFile(error);
                        }
                    }
                },
                ex =>
                {
                    string error = $"EDDCanonn: Error In Patrols HeadThread: {ex.Message}";
                    Console.Error.WriteLine(error);
                    CanonnLogging.Instance.LogToFile(error);
                },
                    "InitializePatrol -> HeadThread",
                    new Action(
                    () => //We are still in the 'HeadThread' here. We forward this action as 'ContinueWith' -> ExecuteSynchronously.
                    {
                        Task.WaitAll(_tasks.ToArray()); //The 'HeadThread' must wait until its workers are all finished.
                        SafeInvoke(() =>
                        {
                            ExtComboBoxPatrol.Enabled = true; ExtComboBoxPatrol.SelectedIndex = 0;
                            ExtComboBoxRange.Enabled = true; ExtComboBoxRange.SelectedIndex = 2;
                        });
                        UpdatePatrols();
                    })
                );
            }
            catch (Exception ex)
            {
                string error = $"EDDCanonn: Error in InitializePatrols: {ex.Message}";
                Console.Error.WriteLine(error);
                CanonnLogging.Instance.LogToFile(error);
            }
        }

        private void CreateFromTSV(string url, string category, CancellationToken token)
        {
            try
            {
                //Initialize a 3D KdTree for patrol locations.
                KdTree<double, Patrol> kdT = new KdTree<double, Patrol>(3, new DoubleMath(), AddDuplicateBehavior.Update);

                //Fetch TSV data and parse it into a list of dictionaries.
                List<Dictionary<string, string>> records = CanonnHelper.ParseTsv(dataHandler.FetchData(url).response);

                foreach (Dictionary<string, string> record in records)
                {
                    token.ThrowIfCancellationRequested(); //We abort here if a cancellation was requested.

                    try
                    {
                        string patrolType = record.TryGetValue("Patrol", out string patrolValue) ? patrolValue :
                            record.TryGetValue("Type", out string typeValue) ? typeValue : string.Empty;

                        string system = record.TryGetValue("System", out string systemValue) ? systemValue : string.Empty;

                        double x = CanonnHelper.GetValueOrDefault(new JToken(record["X"] ?? null),
                            CanonnHelper.PositionFallback);
                        double y = CanonnHelper.GetValueOrDefault(new JToken(record["Y"] ?? null),
                            CanonnHelper.PositionFallback);
                        double z = CanonnHelper.GetValueOrDefault(new JToken(record["Z"] ?? null),
                            CanonnHelper.PositionFallback);

                        string instructions = record.TryGetValue("Instructions", out string instructionsValue) ? instructionsValue : "none";
                        string urlp = record.TryGetValue("Url", out string urlValue) ? urlValue : string.Empty;

                        //Construct a new Patrol object and insert it into the KdTree.
                        Patrol patrol = new Patrol(patrolType, category, system, x, y, z, instructions, urlp);
                        kdT.Add(new double[] { patrol.x, patrol.y, patrol.z }, patrol);
                    }
                    catch (Exception ex)
                    {
                        string error = $"EDDCanonn: Error processing record: {ex.Message}";
                        Console.Error.WriteLine(error);
                        CanonnLogging.Instance.LogToFile(error);
                    }
                }

                //Store the completed KdTree in the patrols dictionary.
                patrols.Add(category, kdT);

                //Update the UI with the new category entry.
                SafeBeginInvoke(() =>
                {
                    ExtComboBoxPatrol.Items.Add(category);
                });

            }
            catch (Exception ex)
            {
                string error = $"EDDCanonn: Error in CreateFromTSV for category {category}: {ex.Message}";
                Console.Error.WriteLine(error);
                CanonnLogging.Instance.LogToFile(error);
            }
        }


        private void CreateFromJson(string url, string category, CancellationToken token)
        {
            try
            {
                //Initialize a 3D KdTree for patrol locations.
                KdTree<double, Patrol> kdT = new KdTree<double, Patrol>(3, new DoubleMath(), AddDuplicateBehavior.Update);
                JArray jsonRecords = dataHandler.FetchData(url).response.JSONParse().Array();
                if (jsonRecords == null || jsonRecords.Count == 0)
                    return;

                foreach (JObject record in jsonRecords)
                {
                    token.ThrowIfCancellationRequested();//We abort here if a cancellation was requested.
                    try
                    {
                        if (record == null || record.Count == 0)
                            continue;

                        string system = record["system"]?.Value?.ToString() ?? string.Empty;

                        double x = CanonnHelper.GetValueOrDefault(record["x"] ?? null,
                            CanonnHelper.PositionFallback);
                        double y = CanonnHelper.GetValueOrDefault(record["y"] ?? null,
                            CanonnHelper.PositionFallback);
                        double z = CanonnHelper.GetValueOrDefault(record["z"] ?? null,
                            CanonnHelper.PositionFallback);

                        string instructions = record["instructions"]?.Value?.ToString() ?? "none";
                        string urlp = record["url"]?.Value?.ToString() ?? string.Empty;

                        //Construct a new Patrol object and insert it into the KdTree.
                        Patrol patrol = new Patrol(category, system, x, y, z, instructions, urlp);
                        kdT.Add(new double[] { patrol.x, patrol.y, patrol.z }, patrol);
                    }
                    catch (Exception ex)
                    {
                        string error = $"EDDCanonn: Error processing JSON record: {ex.Message}";
                        Console.Error.WriteLine(error);
                        CanonnLogging.Instance.LogToFile(error);
                    }
                }
                //Store the completed KdTree in the patrols dictionary.
                patrols.Add(category, kdT);

                //Update the UI with the new category entry.
                SafeBeginInvoke(() =>
                {
                    ExtComboBoxPatrol.Items.Add(category);
                });
            }
            catch (Exception ex)
            {
                string error = $"EDDCanonn: Error in CreateFromJson for category {category}: {ex.Message}";
                Console.Error.WriteLine(error);
                CanonnLogging.Instance.LogToFile(error);
            }
        }

        private readonly object _patrolLock = new object();
        private bool patrolSwitchLock = true;
        private void UpdatePatrols()
        {
            dataHandler.StartTaskAsync(
                (token) =>
                {
                    while (systemData == null) // We have to wait until the system data has been loaded.
                    {
                        token.ThrowIfCancellationRequested();
                        Task.Delay(250, token).Wait();
                    }

                    lock (_patrolLock)
                    {
                        string type = "";
                        double range = 0.0;
                        SystemData system = DeepCopySystemData();

                        SafeInvoke(() =>
                        {
                            type = ExtComboBoxPatrol.SelectedItem?.ToString() ?? "";
                            range = ExtComboBoxRange.HasChildren ? (ExtComboBoxRange.SelectedIndex <= CanonnHelper.PatrolRanges.Length - 1 ? CanonnHelper.PatrolRanges[ExtComboBoxRange.SelectedIndex] : CanonnHelper.PatrolRanges[0]) : -1;
                        });

                        if(string.IsNullOrEmpty(type) || range == -1)
                            return;

                        List<(string category, Patrol patrol, double distance)> patrolList = patrols.FindPatrolsInRange(type, system.X, system.Y, system.Z, range);

                        List<DataGridViewRow> result = new List<DataGridViewRow>();
                        foreach (var (category, patrol, distance) in patrolList)
                        {
                            var row = new DataGridViewRow();
                            row.Cells.Add(new DataGridViewTextBoxCell { Value = patrol.category });
                            row.Cells.Add(new DataGridViewTextBoxCell { Value = patrol.instructions });
                            row.Cells.Add(new DataGridViewTextBoxCell { Value = distance.ToString() });
                            row.Cells.Add(new DataGridViewTextBoxCell { Value = patrol.system });
                            row.Cells.Add(new DataGridViewTextBoxCell { Value = patrol.url });

                            result.Add(row);
                        }
                        UpdateDataGridPatrol(result); // Update UI with new patrol list.
                    }
                },
            ex =>
            {
                string error = $"EDDCanonn: Error during UpdatePatrols Task: {ex.Message}";
                Console.Error.WriteLine(error);
                CanonnLogging.Instance.LogToFile(error);
            },
                "UpdatePatrols",
                new Action(
                    () => //We are still in the 'UpdatePatrolsThread' here. We forward this action as 'ContinueWith' -> ExecuteSynchronously.
                    {
                        SafeInvoke(() =>
                        {
                            patrolSwitchLock = false;
                        });
                    })
            );
        }

        private void UpdateDataGridPatrol(List<DataGridViewRow> result)
        {
            SafeBeginInvoke(() =>
            {
                dataGridPatrol.Rows.Clear();
                dataGridPatrol.Rows.AddRange(result.ToArray());
            });
        }

        private void toolStripPatrol_IndexChanged(object sender, EventArgs e)
        {
            if(patrolSwitchLock)
                return;
            else patrolSwitchLock = true;
            UpdatePatrols();
        }

        private void CopySystem_Click(object sender, EventArgs e)
        {
            if (dataGridPatrol.SelectedCells.Count == 0) return;

            int rowIndex = dataGridPatrol.SelectedCells[0].RowIndex;
            if (rowIndex < 0 || rowIndex >= dataGridPatrol.Rows.Count) return;

            DataGridViewRow selectedRow = dataGridPatrol.Rows[rowIndex];
            if (dataGridPatrol.Columns["SysName"] == null || selectedRow.Cells["SysName"] == null) return;

            object cellValue = selectedRow.Cells["SysName"].Value;
            if (cellValue == null) return;

            try
            {
                Clipboard.SetText(cellValue.ToString());
            }
            catch (Exception ex)
            {
                string error = $"EDDCanonn: Clipboard error:: {ex.Message}";
                Console.Error.WriteLine(error);
                CanonnLogging.Instance.LogToFile(error);
            }
        }

        private void OpenUrl_Click(object sender, EventArgs e)
        {
            if (dataGridPatrol.SelectedCells.Count == 0) return;

            int rowIndex = dataGridPatrol.SelectedCells[0].RowIndex;
            if (rowIndex < 0 || rowIndex >= dataGridPatrol.Rows.Count) return;

            DataGridViewRow selectedRow = dataGridPatrol.Rows[rowIndex];
            if (dataGridPatrol.Columns["PatrolUrl"] == null || selectedRow.Cells["PatrolUrl"] == null) return;

            object cellValue = selectedRow.Cells["PatrolUrl"].Value;
            if (cellValue == null) return;

            CanonnHelper.OpenUrl(cellValue.ToString());
        }


        private void DataGridPatrol_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            DataGridView.HitTestInfo hit = dataGridPatrol.HitTest(e.X, e.Y);
            if (hit.RowIndex < 0 || hit.ColumnIndex < 0)
                return;

            dataGridPatrol.ClearSelection();
            dataGridPatrol.Rows[hit.RowIndex].Cells[hit.ColumnIndex].Selected = true;
        }

        #endregion

        #region WhiteList

        //this generates a data structure like this:

        // Event Type: CodexEntry
        // Event Type: ApproachSettlement
        // Event Type: undefined
        //   Data Block:
        //     USSType: $USS_Type_AXShips;
        //   Data Block:
        //     BodyType: HyperbolicOrbiter
        //   Data Block:
        //     NearestDestination_Localised: Nonhuman Signature
        //   Data Block:
        //     NearestDestination: $POIScene_Wreckage_UA;
        // Event Type: FSSSignalDiscovered
        //   Data Block:
        //     SignalName: $Fixed_Event_Life_Belt;
        //   Data Block:
        //     SignalName: $Fixed_Event_Life_Cloud;
        //   Data Block:
        //     SignalName: $Fixed_Event_Life_Ring;
        //   Data Block:
        //     IsStation: True
        // Event Type: BuySuit
        // Event Type: Docked
        //   Data Block:
        //     StationType: FleetCarrier
        //   Data Block:
        //     StationName: Hutton Orbital
        // Event Type: CarrierJump
        //   Data Block:
        //     StationType: FleetCarrier
        // Event Type: Commander
        // Event Type: FSSBodySignals
        // Event Type: Interdicted
        //   Data Block:
        //     Faction: 
        //     IsPlayer: False
        //   Data Block:
        //     IsPlayer: False
        //     IsThargoid: True
        // Event Type: Promotion
        // Event Type: SellOrganicData
        // Event Type: SAASignalsFound
        // Event Type: ScanOrganic
        // Event Type: MaterialCollected
        //   Data Block:
        //     Name: tg_shipflightdata
        //   Data Block:
        //     Name: unknownshipsignature

        private WhitelistData Whitelist;
        private void InitializeWhitelist()
        {
            Whitelist = new WhitelistData();
            // Fetch the whitelist
            dataHandler.StartTaskAsync(
            (token) =>
            {
                try
                {
                    JArray whitelistItems = dataHandler.FetchData(CanonnHelper.CanonnPostUrl + "Whitelist").response.JSONParseArray();
                    if (whitelistItems == null || whitelistItems.Count == 0)
                        throw new Exception("EDDCanonn: Whitelist is null");


                    for (int i = 0; i < whitelistItems.Count; i++)
                    {
                        JObject itemObject = whitelistItems[i].Object();

                        AddToWhitelistItem(itemObject);
                    }
                }
                catch (Exception ex)
                {
                    string error = $"EDDCanonn: Error processing whitelist: {ex.Message}";
                    Console.Error.WriteLine(error);
                    CanonnLogging.Instance.LogToFile(error);
                    throw;
                }
            },
            ex =>
            {
                string error = $"EDDCanonn: Error in InitializeWhitelist Task: {ex.Message}";
                Console.Error.WriteLine(error);
                CanonnLogging.Instance.LogToFile(error);

                Abort("It was not possible to fetch the event whitelist from Canonn. " +
                "This is why the start of EDDCanonn was canceled.");
            },
            "InitializeWhitelist"
            );
        }

        private void AddToWhitelistItem(JObject itemObject)
        {
            string definitionRaw = itemObject["definition"].Str();
            if (string.IsNullOrEmpty(definitionRaw))
                return;

            JObject definitionObject = definitionRaw.JSONParse().Object();

            // Default key to identify the type. Choose the most common one.
            string typeKey = "event";
            string typeValue = definitionObject[typeKey].Str();
            // Everything that does not contain the default key is treated as undefined.
            if (string.IsNullOrEmpty(typeValue))
                typeValue = "undefined";

            WhitelistEvent existingEvent = null;
            for (int e = 0; e < Whitelist.Events.Count; e++)
            {
                if (Whitelist.Events[e].Type.Equals(typeValue, StringComparison.InvariantCultureIgnoreCase))
                {
                    existingEvent = Whitelist.Events[e];
                    break;
                }
            }

            if (existingEvent == null)
            {
                existingEvent = new WhitelistEvent { Type = typeValue };
                Whitelist.Events.Add(existingEvent);
            }

            Dictionary<string, object> dataBlock = new Dictionary<string, object>();
            List<string> keys = new List<string>(definitionObject.PropertyNames());

            for (int kk = 0; kk < keys.Count; kk++)
            {
                string key = keys[kk];
                if (!key.Equals(typeKey, StringComparison.InvariantCultureIgnoreCase))
                {
                    object val = definitionObject[key].Value;
                    dataBlock[key] = val;
                }
            }
            if (dataBlock.Count > 0)
                existingEvent.DataBlocks.Add(dataBlock);
        }

        private bool IsEventValid(string eventName, string jsonString)
        {
            JObject jsonObject = string.IsNullOrEmpty(jsonString) ? null : jsonString.JSONParse().Object();

            WhitelistEvent eventNode = Whitelist.Events.FirstOrDefault(e =>
                e.Type.Equals(eventName, StringComparison.InvariantCultureIgnoreCase));

            if (eventNode != null && IsDataBlockValid(eventNode, jsonObject))
                return true;

            if (eventNode == null)
            {
                eventNode = Whitelist.Events.FirstOrDefault(e =>
                e.Type.Equals("undefined", StringComparison.InvariantCultureIgnoreCase));

                if (eventNode != null && IsDataBlockValid(eventNode, jsonObject))
                    return true;
            }

            return false;

        }

        private bool IsDataBlockValid(WhitelistEvent eventNode, JObject jsonObject)
        {
            if (eventNode.DataBlocks.Count == 0)
                return true;

            foreach (var dataBlock in eventNode.DataBlocks)
            {
                bool allKeyValuePairsMatch = true;

                foreach (var key in dataBlock.Keys)
                {
                    if (!jsonObject.Contains(key))
                    {
                        allKeyValuePairsMatch = false;
                        break;
                    }

                    if (!jsonObject[key].ToString().Trim('"').Equals(dataBlock[key].ToString(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        allKeyValuePairsMatch = false;
                        break;
                    }
                }

                if (allKeyValuePairsMatch)
                    return true;
            }

            return false;
        }

        private void RunWhiteListTestCases()
        {
            foreach (var (eventName, jsonPayload, expectedResult) in WhiteListTest.WhiteListTestCases)
                DebugLog.AppendText($"{eventName} - ExpectedResult: {expectedResult} - Result: {IsEventValid(eventName, jsonPayload)}" + Environment.NewLine);
        }
        #endregion

        //This only affects the data structure for visual feedback.
        #region SystemData

        private readonly object _lockSystemData = new object();
        private SystemData _systemData; //Do not use this. Otherwise it could get bad.

        private void ResetSystemData() //In the event of a jump/location or if web data is not available.
        {
            lock (_lockSystemData)
                _systemData = null;
        }

        private SystemData DeepCopySystemData()
        {
            lock (_lockSystemData)
            {
                if (_systemData != null)
                    return new SystemData(_systemData);
                else return null;
            }
        }

        private SystemData systemData //Encapsulation Enforcement | Is that what it's called?
        {
            get
            {
                lock (_lockSystemData)
                {
                    if (_systemData == null)
                        return null;

                    return _systemData;
                }
            }
            set
            {
                lock (_lockSystemData)
                {
                    if (_systemData != null)
                        return;

                    _systemData = new SystemData(); //Enforces encapsulation.
                }
            }
        }
        #endregion

        //This only affects the data structure for visual feedback.
        #region ProcessEvents

        private void ProcessEvent(JournalEntry je)
        {
            try
            {
                JObject eventData = je.json.JSONParse().Object();
                bool matched = true;

                switch (je.eventid)
                {
                    case "Scan":
                    case "FSSBodySignals":
                    case "SAASignalsFound":
                        ProcessScan(eventData);
                        break;
                    case "ScanOrganic":
                        ProcessOrganic(eventData);
                        break;
                    case "FSSDiscoveryScan":
                        ProcessFSSDiscoveryScan(eventData);
                        break;
                    case "SAAScanComplete":
                        ProcessSAAScan(eventData);
                        break;
                    default:
                        matched = false;
                        break;
                }
                if (matched) //We only update the UI when changes are made.
                {
                    string mg = $"EDDCanonn: Processed event for visual feedback: " + je.eventid;
                    Console.WriteLine(mg);
                    CanonnLogging.Instance.LogToFile(mg);
                    UpdateUI();
                }

            }
            catch (Exception ex)
            {
                string error = $"EDDCanonn: Error processing event for visual feedback: {je.eventid} : {ex.Message}";
                Console.Error.WriteLine(error);
                CanonnLogging.Instance.LogToFile(error);
            }
        }

        private void ProcessNewSystem(JObject eventData)
        {
            try
            {
                if (systemData == null)
                    systemData = new SystemData(); //Enforces encapsulation and creates a new SystemData instance internally, disregarding any parameters.

                lock (_lockSystemData)
                {
                    systemData.Name = eventData["StarSystem"].Value?.ToString();
                    systemData.SystemAddress = eventData["SystemAddress"]?.ToObject<long>() ?? -1;

                    if (eventData["StarPos"] != null)
                    {
                        systemData.X = CanonnHelper.GetValueOrDefault(eventData["StarPos"][0] ?? null, CanonnHelper.PositionFallback);
                        systemData.Y = CanonnHelper.GetValueOrDefault(eventData["StarPos"][1] ?? null, CanonnHelper.PositionFallback);
                        systemData.Z = CanonnHelper.GetValueOrDefault(eventData["StarPos"][2] ?? null, CanonnHelper.PositionFallback);
                    }
                }
            }
            catch (Exception ex) //If we end up here. We have a problem.
            {
                ResetSystemData(); //In any case, we set the system to null.
                string error = $"EDDCanonn: Error processing NewSystem: {ex.Message}";
                Console.WriteLine(error);
                CanonnLogging.Instance.LogToFile(error);
            }
        }



        private void FetchScanData(JObject eventData, Body body) //Call this method only via the '_lockSystemData' lock. Otherwise it could get bad.
        {
            if (body.ScanData == null)
            {
                body.ScanData = new ScanData
                {
                    //Primitives
                    BodyID = body.BodyID,

                    //List<JObject>  
                    Signals = CanonnHelper.GetJObjectList(eventData, "Signals"),
                    Rings = CanonnHelper.GetJObjectList(eventData, "Rings"),
                    Organics = CanonnHelper.GetJObjectList(eventData, "Organics"),
                    Genuses = CanonnHelper.GetJObjectList(eventData, "Genuses"),
                };
            }
            else
            {
                //Primitives
                body.ScanData.BodyID = body.BodyID;

                //List<JObject>  
                body.ScanData.Signals = CanonnHelper.GetUniqueEntries(eventData, "Signals", body.ScanData.Signals);
                body.ScanData.Rings = CanonnHelper.GetUniqueEntries(eventData, "Rings", body.ScanData.Rings, "Belt");
                body.ScanData.Organics = CanonnHelper.GetUniqueEntries(eventData, "Organics", body.ScanData.Organics);
                body.ScanData.Genuses = CanonnHelper.GetUniqueEntries(eventData, "Genuses", body.ScanData.Genuses);
                body.ScanData.SurfaceFeatures = CanonnHelper.GetUniqueEntries(eventData, "SurfaceFeatures", body.ScanData.SurfaceFeatures);
            }
        }

        private void IdentifyNodeType(Body body, JObject eventData) //Call this method only via the '_lockSystemData' lock. Otherwise it could get bad.
        {
            if (body.NodeType != null) return;
            if (eventData.Contains("PlanetClass") && !eventData["PlanetClass"].IsNull)
                body.NodeType = "Planet";
            else if (eventData.Contains("StarType") && !eventData["StarType"].IsNull)
                body.NodeType = "Star";
            else if (body.BodyName?.Contains("Belt") == true)
                body.NodeType = "Belt";
            else if (body.BodyName?.Contains("Ring") == true)
                body.NodeType = "Ring";
        }

        private Body ProcessScan(JObject eventData) //We can use this for most scan events.
        {
            if (systemData == null)
                systemData = new SystemData(); //Enforces encapsulation.

            lock (_lockSystemData)
            {
                if (systemData.Bodys == null)
                {
                    systemData.Bodys = new Dictionary<int, Body>();
                }

                int bodyId = CanonnHelper.GetValueOrDefault(eventData["BodyID"] ?? null, -1);
                if (bodyId == -1) return null;

                string bodyName = eventData["BodyName"]?.Value?.ToString();

                Body body;

                if (!systemData.Bodys.ContainsKey(bodyId))
                {
                    body = new Body
                    {
                        BodyID = bodyId,
                        BodyName = bodyName,
                    };
                    IdentifyNodeType(body, eventData);
                    systemData.Bodys[bodyId] = body;
                }
                else
                {
                    body = systemData.Bodys[bodyId];
                    //We just set the name again. In case the body was initialized without a name.
                    body.BodyName = bodyName;
                    IdentifyNodeType(body, eventData);
                }

                FetchScanData(eventData, body);

                return body;
            }
        }

        private void ProcessFSSDiscoveryScan(JObject eventData)
        {
            if (systemData == null)
                systemData = new SystemData(); //Enforces encapsulation.

            lock (_lockSystemData)
            {
                systemData.BodyCount = CanonnHelper.GetValueOrDefault(eventData["BodyCount"] ?? null, -1);
            }
        }

        private void ProcessSAAScan(JObject eventData)
        {
            if (systemData == null)
                systemData = new SystemData(); //Enforces encapsulation.

            lock (_lockSystemData)
            {
                int bodyId = CanonnHelper.GetValueOrDefault(eventData["BodyID"] ?? null, -1);
                if (bodyId == -1) return;

                Body body;
                if (!systemData.Bodys?.ContainsKey(bodyId) ?? false) //In the event that the SAA scan comes before the scan event.
                    body = ProcessScan(eventData); //We are entering a second lock here
                else
                    body = systemData.Bodys[bodyId];

                body.IsMapped = true;
            }
        }

        /*
        Sample 'ScanOrganic' event
        {
        "timestamp":"2025-02-06T20:51:14Z",
        "event":"ScanOrganic",
        "Genus":"$Codex_Ent_Tussocks_Genus_Name;",
        "Species":"$Codex_Ent_Tussocks_09_Name;",
        "Body":14
        }
        */

        private void ProcessOrganic(JObject eventData) //The 'ScanOrganic' event is special. We cannot treat it as a 'ProcessScan' (although similar) because the keys are named differently.
        { //wip
            if (systemData == null)
                systemData = new SystemData(); //Enforces encapsulation.

            lock (_lockSystemData)
            {
                if (systemData.Bodys == null)
                {
                    systemData.Bodys = new Dictionary<int, Body>();
                }

                int bodyId = CanonnHelper.GetValueOrDefault(eventData["Body"] ?? null, -1);

                if (bodyId == -1)
                    return;

                Body body;

                if (!systemData.Bodys.ContainsKey(bodyId)) //We have to take consider here if someone freshly installs EDD and then directly scans an organic on a planet he has already been on.
                {
                    body = new Body
                    {
                        BodyID = bodyId,
                        BodyName = "none",
                        NodeType = "planet"
                    };
                    systemData.Bodys[bodyId] = body;
                }
                else
                    body = systemData.Bodys[bodyId];

                if (eventData["Genus"] == null)
                    return;

                if (CanonnHelper.ContainsKeyValuePair(body.ScanData.Organics, "Genus", eventData["Genus"]?.Value?.ToString()))
                    return;

                body.ScanData.Organics.Add(CanonnHelper.GetUniqueEntry(eventData, body.ScanData.Organics));
            }
        }
        #endregion

        //This only affects the data structure for visual feedback.
        #region ProcessSpanshDump

        public void ProcessSpanshDump(JObject root)
        {
            if (root == null || root.Count == 0) // If this is true after a ‘Location’ / 'Jump' event, the system is later initialised via that event.
                return;

            if (systemData == null)
                systemData = new SystemData(); //Enforces encapsulation and creates a new SystemData instance internally, disregarding any parameters.

            lock (_lockSystemData)
            {
                try
                {
                    JObject systemDataNode = root["system"]?.Object() ?? null;
                    if (systemDataNode != null)
                        ProcessSpanshSystemData(systemDataNode);

                    JArray bodyNode = systemDataNode?["bodies"]?.Array() ?? null;
                    if (bodyNode != null)
                        ProcessSpanshBodyNode(bodyNode);

                }
                catch (Exception ex)
                {
                    ResetSystemData(); //If something goes wrong here, we assume that no data is available. If this happens after a ‘Location’ / 'Jump' event, the system is initialised via that event.
                    string error = $"EDDCanonn: Error processing CallbackSystem for visual feedback: {ex.Message}";
                    Console.Error.WriteLine(error);
                    CanonnLogging.Instance.LogToFile(error);
                }
            }
        }

        private void ProcessSpanshSystemData(JObject system)
        {
            // Extract and populate main system details
            systemData.Name = system["name"].Value?.ToString();

            systemData.X = CanonnHelper.GetValueOrDefault(system["coords"]?["x"] ?? null, CanonnHelper.PositionFallback);
            systemData.Y = CanonnHelper.GetValueOrDefault(system["coords"]?["y"] ?? null, CanonnHelper.PositionFallback);
            systemData.Z = CanonnHelper.GetValueOrDefault(system["coords"]?["z"] ?? null, CanonnHelper.PositionFallback);

            systemData.SystemAddress = CanonnHelper.GetValueOrDefault(system["id64"] ?? null, -1l);
            systemData.BodyCount = CanonnHelper.GetValueOrDefault(system["bodyCount"] ?? null, -1);
        }

        private void ProcessSpanshBodyNode(JArray bodyNode) //wip
        {
            if (bodyNode == null)
                return;

            foreach (JObject node in bodyNode)
            {

                if (node == null || node.Count == 0)
                {
                    continue;
                }

                // Extract BodyID; skip processing if invalid
                int bodyId = CanonnHelper.GetValueOrDefault(node["bodyId"] ?? null, -1);
                if (bodyId == -1)
                {
                    continue;
                }

                if (systemData.Bodys == null)
                {
                    systemData.Bodys = new Dictionary<int, Body>();
                }

                if (systemData.Bodys.ContainsKey(bodyId))
                {
                    continue;
                }

                Body body = new Body
                {
                    //Primitives
                    BodyID = bodyId,
                    BodyName = node["name"].Value?.ToString(),
                    NodeType = node["type"].Value?.ToString(),
                };

                body.ScanData = new ScanData
                {
                    //Primitives
                    BodyID = bodyId,
                    //List<JObject>    
                    Signals = CanonnHelper.GetJObjectList(node["signals"] as JObject, "signals"),
                    Genuses = CanonnHelper.GetJObjectList(node["signals"] as JObject, "genuses", "Genus"),
                    Rings = CanonnHelper.GetJObjectList(node, "rings"),
                };

                systemData.Bodys[bodyId] = body;
            }
        }
        #endregion

        //This only affects the data structure for visual feedback.
        #region ProcessData

        public enum RequestTag
        {
            OnStart,
            Log
        }

        public void DataResult(object requestTag, string data)
        {
            if (isAbort)
                return;

            try
            {
                if (requestTag == null || data == null)
                    return;

                JObject o = data.JSONParse().Object();
                if (!(requestTag is RequestTag || requestTag is JObject))
                    return;

                dataHandler.StartTaskAsync(
                (token) =>
                {
                    if (requestTag is RequestTag rt) //Triggered by panel creation.
                    {
                        if (rt.Equals(RequestTag.OnStart))
                        {
                            lock (_lockSystemData) ProcessSpanshDump(o);
                            SafeInvoke(() =>
                            {
                                this.Enabled = true;
                            });
                            activated = true;
                        }
                        else if (rt.Equals(RequestTag.Log))
                        {
                            SafeInvoke(() =>
                            {
                                string mg = "\n" + data ?? "none" + "\n";
                                DebugLog.AppendText(mg);
                                CanonnLogging.Instance.LogToFile(mg);
                            });
                            return;
                        }
                    }
                    else if (requestTag is JObject jb)
                    {
                        string evt = jb["event"]?.Value?.ToString();
                        if (evt == "Location" || evt == "FSDJump") //Triggered by 'jump' or 'location' Event.
                        {
                            lock (_lockSystemData)
                            {
                                ProcessSpanshDump(o);
                                if (systemData == null) ProcessNewSystem(jb);
                            }
                            activated = true;
                        }
                    }
                    UpdateUI(true);
                    UpdatePatrols();
                },
                ex =>
                {
                    string error = $"EDDCanonn: Error processing Systemdata: {ex.Message}";
                    Console.Error.WriteLine(error);
                    CanonnLogging.Instance.LogToFile(error);
                },
                    "DataResult"
                );
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"EDDCanonn: Unexpected error in DataResult: {ex.Message}");
            }
        }

        #endregion

        #region ProcessUpperGridViews

        private List<DataGridViewRow> CollectBioData(SystemData system)
        {
            if (system?.Bodys == null) return null;

            List<DataGridViewRow> rows = new List<DataGridViewRow>
            {
                CanonnHelper.CreateDataGridViewRow(dataGridViewBio, new object[] { "Missing Bio Data:", null, new Bitmap(1, 1) })
            };

            foreach (Body body in system.Bodys.Values)
            {
                if (body?.ScanData?.Signals == null || body.ScanData.Signals.Count == 0) continue;

                JObject o = CanonnHelper.FindFirstMatchingJObject(body.ScanData.Signals, "Type", "$SAA_SignalType_Biological;") ?? CanonnHelper.FindFirstMatchingJObject(body.ScanData.Signals, null, "$SAA_SignalType_Biological;");

                if (o == null) continue;

                if (body.ScanData.Genuses == null || body.ScanData.Genuses.Count == 0)
                {
                    rows.Add(CanonnHelper.CreateDataGridViewRow(dataGridViewBio, new object[] { body.BodyName, (CanonnHelper.GetValueOrDefault(o["Count"], 0) == 0 ? CanonnHelper.GetValueOrDefault(o["$SAA_SignalType_Biological;"], 0)
                        : CanonnHelper.GetValueOrDefault(o["Count"], 0)) + " unknown species", Properties.Resources.biology })); 
                    continue;
                }
            }

            if (rows.Count <= 1)
            {
                CanonnHelper.DisposeDataGridViewRowList(rows); return null;
            }
            return rows;
        }

        private List<DataGridViewRow> CollectRingData(SystemData system)
        {
            if (system?.Bodys == null) return null;

            List<DataGridViewRow> rows = new List<DataGridViewRow>
            {
                CanonnHelper.CreateDataGridViewRow(dataGridViewRing, new object[] { "Missing Rings:", null, null, new Bitmap(1, 1) })
            };

            Regex ringRegex = new Regex(@"([A-Z])\s+Ring$", RegexOptions.IgnoreCase);

            foreach (Body body in system.Bodys.Values)
            {
                if (body?.ScanData?.Rings == null) continue;
                bool first = true;

                foreach (JObject ring in body.ScanData.Rings)
                {
                    string ringName = ring["name"]?.Value?.ToString() ?? ring["Name"]?.Value?.ToString() ?? null;
                    if (ringName?.Contains("Ring") != true) continue;

                    if (system.GetBodyByName(ringName)?.IsMapped == true || ring.Contains("id64")) continue;

                    double[] values = new[] { "innerRadius", "InnerRad", "outerRadius", "OuterRad" }
                        .Select(k => CanonnHelper.GetValueOrDefault(ring[k], 0.0) / 299792458)
                        .Select(v => Math.Round(v, 2))
                        .ToArray();

                    string result = $"{(values[0] == 0.0 ? values[1] : values[0])} ls - {(values[2] == 0.0 ? values[3] : values[2])} ls";

                    Match match = ringRegex.Match(ringName);
                    rows.Add(CanonnHelper.CreateDataGridViewRow(dataGridViewRing, new object[]
                    {
                        first ? body.BodyName : null,
                        match.Success ? match.Groups[1].Value + " Ring" : ringName,
                        result,
                        Properties.Resources.ring
                    }));

                    first = false;
                }
            }

            if (rows.Count <= 1)
            {
                CanonnHelper.DisposeDataGridViewRowList(rows);
                return null;
            }

            return rows;
        }

        private List<DataGridViewRow> CollectGMO(SystemData system)
        {
            JArray gmos = DLLCallBack.GetGMOs("systemname=" + system.Name)?.JSONParseArray() ?? null; if (gmos == null || gmos.Count == 0) return null;

            List<DataGridViewRow> rows = new List<DataGridViewRow>();

            string combinedDescriptions = "Tags: " + string.Join(", ", gmos[0]?["GalMapTypes"]?.Array()
                ?.Select(galMapType => galMapType?["Description"]?.Value?.ToString())
                .Where(desc => !string.IsNullOrWhiteSpace(desc)) ?? Enumerable.Empty<string>());
            rows.Add(CanonnHelper.CreateDataGridViewRow(dataGridViewGMO, new object[] { combinedDescriptions }));

            rows.Add(CanonnHelper.CreateDataGridViewRow(dataGridViewGMO, new object[] { null }));

            rows.Add(CanonnHelper.CreateDataGridViewRow(dataGridViewGMO, new object[] { gmos[0]?["DescriptiveNames"]?[0]?.Value?.ToString() + ":" }));
            rows.Add(CanonnHelper.CreateDataGridViewRow(dataGridViewGMO, new object[] { gmos[0]?["Description"]?.Value?.ToString() }));

            return rows;
        }

        #endregion

        #region CanonnNews

        private JArray News;
        private int NewsIndex = 0;

        private void InitializeNews()
        {
            dataHandler.StartTaskAsync(
                (token) =>
                {
                    try
                    {
                        News = dataHandler.FetchData(CanonnHelper.CanonnNews).response.JSONParseArray();
                        if (News == null || News.Count == 0) return;
                        NewsIndexChanged(0);
                    }
                    catch (Exception ex)
                    {
                        string error = $"EDDCanonn: Error in InitializeNews: {ex}";
                        Console.Error.WriteLine(error);
                        CanonnLogging.Instance.LogToFile(error);
                    }
                },
                ex =>
                {
                    string error = $"EDDCanonn: Unexpected error in InitializeNews Task: {ex}";
                    Console.Error.WriteLine(error);
                    CanonnLogging.Instance.LogToFile(error);
                },
                "InitializeNews"
            );
        }


        private void NewsIndexChanged(int n)
        {
            if (News == null || News.Count == 0) return;
            if (n == 1 && NewsIndex + n >= News.Count)
                NewsIndex = 0;
            else if (n == -1 && NewsIndex + n < 0)
                NewsIndex = News.Count - 1;
            else
                NewsIndex += n;

            SafeBeginInvoke(() =>
            {
                try
                {
                    textBoxNews.Clear();

                    string decoded = News[NewsIndex]?["excerpt"]?["rendered"]?.Value?.ToString() ?? "none";

                    try
                    {
                        decoded = Regex.Replace(WebUtility.HtmlDecode(System.Text.RegularExpressions.Regex.Unescape(decoded)), "<.*?>", "");
                    }
                    catch (Exception ex)
                    {
                        string error = $"EDDCanonn: Error processing news text: {ex.Message}";
                        Console.Error.WriteLine(error);
                        CanonnLogging.Instance.LogToFile(error);
                    }

                    textBoxNews.AppendText(decoded);
                    labelNewsIndex.Text = "Page: " + (NewsIndex + 1);
                }
                catch (Exception ex)
                {
                    string error = $"EDDCanonn: Unexpected error in NewsIndexChanged: {ex.Message}";
                    Console.Error.WriteLine(error);
                    CanonnLogging.Instance.LogToFile(error);
                }
            });
        }

        private void buttonNextNews_Click(object sender, EventArgs e)
        {
            NewsIndexChanged(1);
        }

        private void buttonPrevNews_Click(object sender, EventArgs e)
        {
            NewsIndexChanged(-1);
        }
        #endregion

        #region UpdateUI

        private void UpdateUI(bool jumped = false) //Updates all UI events. Except for the patrols.
        {
            dataHandler.StartTaskAsync(
                (token) =>
                {
                    try
                    {
                        while (systemData == null) //We have to wait until the system data has been loaded.
                        {
                            token.ThrowIfCancellationRequested(); //We abort here if a cancellation was requested.
                            Task.Delay(250, token).Wait();
                        }

                        SystemData system = DeepCopySystemData();

                        if (system == null) //Ensure system data is valid before updating UI.
                            return;
                        UpdateMainFields(system, jumped);
                        UpdateUpperGridViews(system, jumped);                     
                    }
                    catch (Exception ex)
                    {
                        string error = $"EDDCanonn: Error during UpdateUI: {ex}";
                        Console.Error.WriteLine(error);
                        CanonnLogging.Instance.LogToFile(error);
                    }
                },
                ex =>
                {
                    string error = $"EDDCanonn: Error during UpdateUI execution: {ex}";
                    Console.Error.WriteLine(error);
                    CanonnLogging.Instance.LogToFile(error);
                },
                "UpdateUI"
            );
        }

        private void UpdateMainFields(SystemData system, bool jumped) //This should only be called by 'UpdateMainFields'.
        {
            if (system == null) //Safety first.
                return;

            SafeBeginInvoke(() =>
            {
                try
                {
                    textBoxSystem.Clear();
                    textBoxBodyCount.Clear();

                    textBoxSystem.AppendText(system.Name ?? "none");
                    textBoxBodyCount.AppendText(system.CountBodysFilteredByNodeType(new string[] { "Planet", "Star" }) + " / " + system.BodyCount);
                }
                catch (Exception ex)
                {
                    string error = $"EDDCanonn: Error in UpdateMainFields: {ex}";
                    Console.Error.WriteLine(error);
                    CanonnLogging.Instance.LogToFile(error);
                }
            });
        }

        private void UpdateUpperGridViews(SystemData system, bool jumped) //This should only be called by 'UpdateMainFields'.
        {
            if (system == null) //Safety first.
                return;

            SafeBeginInvoke(() =>
            {
                try
                {
                    dataGridViewData.Rows.Clear();
                    dataGridViewRing.Rows.Clear();
                    dataGridViewBio.Rows.Clear();

                    List<DataGridViewRow> ringRows = CollectRingData(system);
                    if (ringRows?.Count > 0)
                    {
                        dataGridViewRing.Rows.AddRange(ringRows.ToArray());
                        dataGridViewData.Rows.Add(CanonnHelper.CreateDataGridViewRow(dataGridViewData, new object[] { "There are new rings that can be scanned.", Properties.Resources.ring }));
                    }

                    List<DataGridViewRow> bioRows = CollectBioData(system);
                    if (bioRows?.Count > 0)
                    {
                        dataGridViewBio.Rows.AddRange(bioRows.ToArray());
                        dataGridViewData.Rows.Add(CanonnHelper.CreateDataGridViewRow(dataGridViewData, new object[] { "There is new bio data available for scanning.", Properties.Resources.biology }));
                    }

                    if (jumped)
                    {
                        dataGridViewGMO.Rows.Clear();
                        List<DataGridViewRow> gmoRows = CollectGMO(system);
                        if (gmoRows?.Count > 0)
                        {
                            dataGridViewGMO.Rows.AddRange(gmoRows.ToArray());
                            dataGridViewData.Rows.Add(CanonnHelper.CreateDataGridViewRow(dataGridViewData, new object[] { "A GMO is available for this system.", Properties.Resources.tourist }));
                        }
                    }

                    dataGridViewData.Rows.Add(CanonnHelper.CreateDataGridViewRow(dataGridViewData, new object[] { "There are new surveys that can be confirmed. [WIP]", Properties.Resources.other }));
                }
                catch (Exception ex)
                {
                    string error = $"EDDCanonn: Error in UpdateUpperGridViews: {ex}";
                    Console.Error.WriteLine(error);
                    CanonnLogging.Instance.LogToFile(error);
                }
            });
        }


        #region Links

        private void linkLabelEDDCanonn_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CanonnHelper.OpenUrl(CanonnHelper.EDDCanonnGitHub);
        }

        private void pictureBoxCanonn_Click(object sender, EventArgs e)
        {
            CanonnHelper.OpenUrl(CanonnHelper.CanonnWebPage);
        }

        private void pictureBoxEDD_Click(object sender, EventArgs e)
        {
            CanonnHelper.OpenUrl(CanonnHelper.EDDGitHub);
        }

        private void textBoxNews_Click(object sender, EventArgs e)
        {
            CanonnHelper.OpenUrl(News[NewsIndex]?["link"]?.Value?.ToString() ?? CanonnHelper.CanonnWebPage);
        }

        private void textBoxSystem_Click(object sender, EventArgs e)
        {
            CanonnHelper.OpenUrl(CanonnHelper.SignalsCanonnTech + systemData?.Name?.Replace(" ", "%20"));
        }
        #endregion

        #region Notify

        private void NotifyField(Control control, String arg)
        {
            SafeBeginInvoke(() =>
            {
                control.Text = arg;
            });
        }

        private void NotifyMainFields(String arg)
        {
            SafeBeginInvoke(() =>
            {
                textBoxSystem.Text = arg;
                ExtComboBoxRange.Text = arg;
                ExtComboBoxPatrol.Text = arg;
                textBoxNews.Text = arg;
            });
        }
        #endregion

        private void SafeBeginInvoke(Action action)
        {
            if (this.IsHandleCreated && !this.IsDisposed)
            {
                try
                {
                    BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            action();
                        }
                        catch (Exception ex)
                        {
                            string error = $"EDDCanonn: UI update failed in invoked action (SafeBeginInvoke): {ex}";
                            Console.Error.WriteLine(error);
                            CanonnLogging.Instance.LogToFile(error);
                        }
                    }));
                }
                catch (Exception ex)
                {
                    string error = $"EDDCanonn: UI update failed in SafeBeginInvoke: {ex}";
                    Console.Error.WriteLine(error);
                    CanonnLogging.Instance.LogToFile(error);
                }
            }
        }

        private void SafeInvoke(Action action)
        {
            if (this.IsHandleCreated && !this.IsDisposed)
            {
                try
                {
                    Invoke(new Action(() =>
                    {
                        try
                        {
                            action(); 
                        }
                        catch (Exception ex)
                        {
                            string error = $"EDDCanonn: UI update failed in invoked action (SafeInvoke): {ex}";
                            Console.Error.WriteLine(error);
                            CanonnLogging.Instance.LogToFile(error);
                        }
                    }));
                }
                catch (Exception ex)
                {
                    string error = $"EDDCanonn: UI update failed in SafeInvoke: {ex}";
                    Console.Error.WriteLine(error);
                    CanonnLogging.Instance.LogToFile(error);
                }
            }
        }
        #endregion

        #region IEDDPanelExtension

        private bool activated = false;
        public void NewUnfilteredJournal(JournalEntry je)
        {
            if (isAbort)
                return;

            dataHandler.StartTaskAsync(
            (token) =>
            {
                JObject o = je.json.JSONParseObject();
                string eventId = je.eventid;

                if (eventId.Equals("FSDJump") || eventId.Equals("Location"))
                {
                    activated = false; // As long as we make a callback, we hold back the events (canonn events excluded).
                    ResetSystemData();

                    // The callback may take some time. That's why we give the user some feedback in an early stage.
                    NotifyField(textBoxSystem, je.systemname);
                    NotifyField(textBoxBodyCount, "Fetch...");

                    SafeBeginInvoke(() =>
                    {
                        DLLCallBack.RequestSpanshDump(o, this, je.systemname, je.systemaddress, true, false, null);
                    });

                    return;
                }

                if (IsEventValid(eventId, je.json)) // Send event to canonn if valid.
                {
                    Payload.BuildPayload(je, GetStatusJson());
                }

                while (!activated) // Wait until a 'DataResult' has been completed.
                {
                    token.ThrowIfCancellationRequested();
                    Task.Delay(250, token).Wait();
                }

                ProcessEvent(je);
            },
            ex =>
            {
                string error = $"EDDCanonn: Error processing JournalEntry: {ex.Message}";
                Console.Error.WriteLine(error);
                CanonnLogging.Instance.LogToFile(error);
            },
            "NewUnfilteredJournal: " + je.eventid);
        }

        private bool historyset = false;
        public void HistoryChange(int count, string commander, bool beta, bool legacy)
        {
            if (isAbort)
                return;
            if (!historyset)
                historyset = true;
        }

        private readonly object _lockStatusJson = new object();
        private JObject _statusJson;
        public void NewUIEvent(string jsonui)
        {
            if (isAbort)
                return;

            JObject o = jsonui.JSONParse().Object();
            if (o == null)
                return;

            string type = o["EventTypeStr"].Str();
            if (string.IsNullOrEmpty(type))
                return;

            lock (_lockStatusJson)
                _statusJson = o;
        }

        private JObject GetStatusJson() //Return a deep copy.
        {
            lock (_lockStatusJson)
                return new JObject(_statusJson);
        }

        public bool SupportTransparency => false;

        public bool DefaultTransparent => false;

        public bool AllowClose()
        {
            return true;
        }

        public void Closing()
        {
            CanonnHelper.InstanceCount--;
            dataHandler.Closing();
            if (CanonnHelper.InstanceCount == 0)
            {
                CanonnLogging.Instance.StopLogging();
            }
        }

        public void Initialise(EDDPanelCallbacks callbacks, int displayid, string themeasjson, string configuration)
        {
            DLLCallBack = EDDCanonnEDDClass.DLLCallBack;
            PanelCallBack = callbacks;
            ThemeChanged(themeasjson);
            StartUp();
        }

        Color FromJson(JToken color) { return System.Drawing.ColorTranslator.FromHtml(color.Str("Yellow")); }
        public void ThemeChanged(string themeasjson)
        {
            setTheme(themeasjson);
        }

        public void NewFilteredJournal(JournalEntry je)
        {
            if (isAbort)
                return;
        }

        public void NewTarget(Tuple<string, double, double, double> target)
        {
            if (isAbort)
                return;
            //wip
        }

        public void InitialDisplay()
        {
            if (isAbort)
                return;

            //wip
        }

        public void LoadLayout()
        {
            PanelCallBack.LoadGridLayout(dataGridPatrol);
            PanelCallBack.LoadGridLayout(dataGridViewData);
            PanelCallBack.LoadGridLayout(dataGridViewBio);
            PanelCallBack.LoadGridLayout(dataGridViewRing);
            PanelCallBack.LoadGridLayout(dataGridViewGMO);
            //wip
        }

        public void ScreenShotCaptured(string file, Size s)
        {
            if (isAbort)
                return;

            throw new NotImplementedException();

            //wip
        }

        public void SetTransparency(bool ison, Color curcol)
        {
            if (isAbort)
                return;

            this.BackColor = curcol;

            //wip
        }

        public void TransparencyModeChanged(bool on)
        {
            if (isAbort)
                return;

            //wip
        }

        void IEDDPanelExtension.CursorChanged(JournalEntry je)
        {
            if (isAbort)
                return;

            //wip
        }

        public void ControlTextVisibleChange(bool on)
        {
            if (isAbort)
                return;

            throw new NotImplementedException();

            //wip
        }

        public string HelpKeyOrAddress()
        {
            return CanonnHelper.EDDCanonnGitHub;
        }
        #endregion

        #region Debug

        private bool isAbort = false;
        private void Abort(string msg = "default")
        {
            isAbort = true;

            NotifyMainFields("Aborted");
            DebugLog.AppendText(msg);
            extTabControlData.SelectedIndex = extTabControlData.TabCount - 1;

            dataHandler.Closing();

            this.Enabled = false;
            activated = false;

            Whitelist = null;
            patrols = null;
            News = null;                
        }

        private void LogWhitelist_Click(object sender, EventArgs e)
        {
            DebugLog.AppendText(WhiteListTest.PrintWhitelist(Whitelist));
        }

        private void TestWhitelist_Click(object sender, EventArgs e)
        {
            RunWhiteListTestCases();
        }

        private void ClearDebugLog_Click(object sender, EventArgs e)
        {
            DebugLog.Clear();
        }

        private void LogSystem_Click(object sender, EventArgs e)
        {
            SystemData system = DeepCopySystemData();
            string mg = "\n" + system?.ToString() ?? "none" + "\n";
            DebugLog.AppendText(mg);
            CanonnLogging.Instance.LogToFile(mg);
        }

        private void CallSystem_Click(object sender, EventArgs e)
        {
            DLLCallBack.RequestSpanshDump(RequestTag.Log, this, "", 0, true, false, null);
        }

        private void dataGridViewBio_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            HandleDataGridViewError("dataGridViewBio", e);
        }

        private void dataGridViewRing_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            HandleDataGridViewError("dataGridViewRing", e);
        }

        private void dataGridViewData_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            HandleDataGridViewError("dataGridViewData", e);
        }

        private void dataGridViewGMO_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            HandleDataGridViewError("dataGridViewGMO", e);
        }

        private void dataGridPatrol_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            HandleDataGridViewError("dataGridPatrol", e);
        }

        private void HandleDataGridViewError(string gridName, DataGridViewDataErrorEventArgs e)
        {
            string error = $"EDDCanonn: DataGridView error in {gridName} (Row: {e.RowIndex}, Column: {e.ColumnIndex}): {e.Exception}";
            Console.Error.WriteLine(error);
            CanonnLogging.Instance.LogToFile(error);

            e.ThrowException = false;
        }
        #endregion
    }
}
