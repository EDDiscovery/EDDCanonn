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

namespace EDDCanonn
{
    partial class EDDCanonnUserControl : UserControl, IEDDPanelExtension
    {
        private ActionDataHandler dataHandler;
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
                    Abort(); //Abort if a Canonn Panel already exists.
                    return;
                }
                else
                    CanonnHelper.InstanceCount++;

                InitializeWhitelist();
                InitializePatrols();

                JournalEntry je = new EDDDLLInterfaces.EDDDLLIF.JournalEntry();

                if (DLLCallBack.RequestHistory(1, false, out je) == true) //We check here if EDD has already loaded the history.
                {
                    DLLCallBack.RequestScanData(RequestTag.OnStart, this, "", true);
                    return;
                }
                else
                {
                    dataHandler.StartTaskAsync(
                    (token) =>
                    {
                        while (!historyset) //We wait until the hostory has been loaded.
                        {
                            token.ThrowIfCancellationRequested(); //We abort here if a cancellation was requested.
                            Task.Delay(250,token).Wait();
                        }
                        SafeInvoke(() =>
                        {
                            DLLCallBack.RequestScanData(RequestTag.OnStart, this, "", true);
                        });
                        return;
                    },
                    ex =>
                    {
                        Console.Error.WriteLine($"EDDCanonn: Unexpected error in \"Wait for History\" Thread: {ex.Message}");
                    },
                        "Startup : History-Listener"
                    );
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"EDDCanonn: Error during Startup: {ex.Message}");
            }
        }

        private bool isAbort = false;
        private void Abort()
        {
            CanonnHelper.InstanceCount++;
            isAbort = true;

            NotifyMainFields("Aborted");
            DebugLog.AppendText("Only one Canonn Panel instance can be active.");
            extTabControlData.SelectedIndex = extTabControlData.TabCount - 1;

            this.Enabled = false;
            dataHandler.CancelAllTasks();
        }

        #endregion

        #region Patrol

        private Patrols patrols;
        private void InitializePatrols()
        {
            try
            {
                patrols = new Patrols();

                ExtComboBoxPatrol.Items.Add("all"); //The 'all' KdTree-Dictionary has already been set in “Patrols.cs”.

                ExtComboBoxRange.Items.AddRange(CanonnHelper.PatrolRanges.Select(x => x.ToString()).ToArray());

                ConcurrentBag<Task> _tasks = new ConcurrentBag<Task>(); //We have to make sure that all workers are finished before we update the patrols.

                //Start a 'head worker' to avoid blocking the UI thread.
                dataHandler.StartTaskAsync(
                (token) =>
                {
                    //Fetch information about available patrols.
                    List<Dictionary<string, string>> records = CanonnHelper.ParseTsv(dataHandler.FetchData(CanonnHelper.PatrolUrl));
                    foreach (Dictionary<string, string> record in records)
                    {
                        token.ThrowIfCancellationRequested(); //We abort here if a cancellation was requested.
                        try
                        {
                            string description = record.TryGetValue("Description", out string descriptionValue) ? descriptionValue : "uncategorized";
                            bool enabled = record.TryGetValue("Enabled", out string enabledValue) && enabledValue == "Y";
                            string type = record.TryGetValue("Type", out string typeValue) ? typeValue : string.Empty;
                            string url = record.TryGetValue("Url", out string urlValue) ? urlValue : string.Empty;

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
                                        Console.Error.WriteLine($"EDDCanonn: Error Initialize Patrols -> {description}: {ex.Message}");
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
                                        Console.Error.WriteLine($"EDDCanonn: Error Initialize Patrols -> {description}: {ex.Message}");
                                    },
                                        "InitializePatrol -> SubThread: " + description
                                    ));
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.Error.WriteLine($"EDDCanonn: Error processing record category: {ex.Message}");
                        }
                    }
                },
                ex =>
                {
                    Console.Error.WriteLine($"EDDCanonn: Error In Patrols HeadThread: {ex.Message}");
                },
                    "InitializePatrol -> HeadThread",
                    new Action(
                    () => //We are still in the 'HeadThread' here. We forward this action as 'ContinueWith' -> ExecuteSynchronously.
                    {
                        Task.WaitAll(_tasks.ToArray()); //The 'HeadThread' must wait until its workers are all finished.
                        SafeInvoke(() =>
                        {
                            ExtComboBoxPatrol.Enabled = true;
                            ExtComboBoxPatrol.SelectedIndex = 0;
                            ExtComboBoxRange.Enabled = true;
                            ExtComboBoxRange.SelectedIndex = 2;
                            _patrolLock = false;
                        });
                        UpdatePatrols();
                    })
                );
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"EDDCanonn: Error in InitializePatrols: {ex.Message}");
            }
        }

        private void CreateFromTSV(string url, string category, CancellationToken token)
        {
            try
            {
                KdTree<double, Patrol> kdT = new KdTree<double, Patrol>(3, new DoubleMath());
                List<Dictionary<string, string>> records = CanonnHelper.ParseTsv(dataHandler.FetchData(url));
                foreach (Dictionary<string, string> record in records)
                {
                    token.ThrowIfCancellationRequested(); //We abort here if a cancellation was requested.
                    try
                    {
                        string patrolType = record.TryGetValue("Patrol", out string patrolValue) ? patrolValue :
                                            record.TryGetValue("Type", out string typeValue) ? typeValue : string.Empty;
                        string system = record.TryGetValue("System", out string systemValue) ? systemValue : string.Empty;

                        double x = CanonnHelper.GetValueOrDefault(new JToken(record["X"] ?? null), CanonnHelper.PositionFallback);
                        double y = CanonnHelper.GetValueOrDefault(new JToken(record["Y"] ?? null), CanonnHelper.PositionFallback);
                        double z = CanonnHelper.GetValueOrDefault(new JToken(record["Z"] ?? null), CanonnHelper.PositionFallback);

                        string instructions = record.TryGetValue("Instructions", out string instructionsValue) ? instructionsValue : "none";
                        string urlp = record.TryGetValue("Url", out string urlValue) ? urlValue : string.Empty;

                        Patrol patrol = new Patrol(patrolType, category, system, x, y, z, instructions, urlp);
                        kdT.Add(new double[] { patrol.x, patrol.y, patrol.z }, patrol);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"EDDCanonn: Error processing record: {ex.Message}");
                    }
                }
                patrols.Add(category, kdT);
                SafeBeginInvoke(() =>
                {
                    ExtComboBoxPatrol.Items.Add(category);
                });

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"EDDCanonn: Error in CreateFromTSV for category {category}: {ex.Message}");
            }
        }

        private void CreateFromJson(string url, string category, CancellationToken token)
        {
            try
            {
                KdTree<double, Patrol> kdT = new KdTree<double, Patrol>(3, new DoubleMath());
                JArray jsonRecords = dataHandler.FetchData(url).JSONParse().Array();
                if (jsonRecords == null || jsonRecords.Count == 0)
                    return;

                foreach (JObject record in jsonRecords)
                {
                    token.ThrowIfCancellationRequested(); //We abort here if a cancellation was requested.
                    try
                    {
                        if (record == null || record.Count == 0)
                            continue;

                        string system = record["system"]?.Value?.ToString() ?? string.Empty;

                        double x = CanonnHelper.GetValueOrDefault(record["x"] ?? null, CanonnHelper.PositionFallback);
                        double y = CanonnHelper.GetValueOrDefault(record["y"] ?? null, CanonnHelper.PositionFallback);
                        double z = CanonnHelper.GetValueOrDefault(record["z"] ?? null, CanonnHelper.PositionFallback);

                        string instructions = record["instructions"]?.Value?.ToString() ?? "none";
                        string urlp = record["url"]?.Value?.ToString() ?? string.Empty;

                        Patrol patrol = new Patrol(category, system, x, y, z, instructions, urlp);
                        kdT.Add(new double[] { patrol.x, patrol.y, patrol.z }, patrol);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"EDDCanonn: Error processing JSON record: {ex.Message}");
                    }
                }

                patrols.Add(category, kdT);
                SafeBeginInvoke(() =>
                {
                    ExtComboBoxPatrol.Items.Add(category);
                });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"EDDCanonn: Error in CreateFromJson for category {category}: {ex.Message}");
            }
        }

        private bool _patrolLock = true;
        private void UpdatePatrols()
        {
            dataHandler.StartTaskAsync(
                (token) =>
                {
                    try
                    {
                        while (systemData == null) //We have to wait until the system data has been loaded.
                        {
                            token.ThrowIfCancellationRequested(); //We abort here if a cancellation was requested.
                            Task.Delay(250,token).Wait();
                        }

                        string type = "";
                        double range = 0.0;
                        SystemData system = deepCopySystemData();

                        SafeInvoke(() =>
                        {
                            type = ExtComboBoxPatrol.SelectedItem?.ToString() ?? "";
                            range = CanonnHelper.GetValueOrDefault(new JToken(ExtComboBoxRange.SelectedItem?.ToString() ?? null), 0.0);
                        });

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

                        UpdateDataGridPatrol(result);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"EDDCanonn: Unexpected error in UpdatePatrols Task: {ex.Message}");
                    }
                    finally
                    {
                        _patrolLock = false;
                    }
                },
               ex => Console.Error.WriteLine($"EDDCanonn: Error during UpdatePatrols: {ex.Message}"),
               "UpdatePatrols"
           );
        }

        private void toolStripPatrol_IndexChanged(object sender, EventArgs e)
        {
            if (_patrolLock)
                return;
            _patrolLock = true;
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
                Console.Error.WriteLine($"EDDCanonn: Clipboard error:: {ex.Message}");
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
            try
            {
                Whitelist = new WhitelistData();
                // Fetch the whitelist
                dataHandler.StartTaskAsync(
                (token) =>
                {
                    try
                    {
                        JArray whitelistItems = dataHandler.FetchData(CanonnHelper.WhitelistUrl).JSONParseArray();

                        if (whitelistItems == null)
                            throw new Exception("EDDCanonn: Whitelist is null"); //wip

                        for (int i = 0; i < whitelistItems.Count; i++)
                        {
                            JObject itemObject = whitelistItems[i].Object();

                            AddToWhitelistItem(itemObject);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"EDDCanonn: Error processing whitelist: {ex.Message}");
                    }
                },
                ex =>
                {
                    Console.Error.WriteLine($"EDDCanonn: Error fetching whitelist: {ex.Message}");
                },
                "InitializeWhitelist"
                );
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"EDDCanonn: Unexpected error in InitializeWhitelist: {ex.Message}");
            }
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

        private void PrintWhitelist()
        {
            for (int i = 0; i < Whitelist.Events.Count; i++)
            {
                WhitelistEvent we = Whitelist.Events[i];
                DebugLog.AppendText("Event Type: " + we.Type + "\r\n");

                for (int j = 0; j < we.DataBlocks.Count; j++)
                {
                    DebugLog.AppendText("  Data Block:\r\n");
                    Dictionary<string, object> db = we.DataBlocks[j];

                    foreach (KeyValuePair<string, object> kvp in db)
                    {
                        DebugLog.AppendText("    " + kvp.Key + ": " + kvp.Value + "\r\n");
                    }
                }
                DebugLog.AppendText("\r\n");
            }
        }

        private void RunWhiteListTestCases()
        {
            foreach (var (eventName, jsonPayload, expectedResult) in TestData.WhiteListTestCases)
                DebugLog.AppendText($"{eventName} - ExpectedResult: {expectedResult} - Result: {IsEventValid(eventName, jsonPayload)}" + Environment.NewLine);
        }
        #endregion

        //This only affects the data structure for visual feedback.
        #region SystemData

        private readonly object _lockSystemData = new object();
        private SystemData _systemData; //Do not use this. Otherwise it could get bad.

        private void resetSystemData() //In the event of a jump/location or if web data is not available.
        {
            lock (_lockSystemData)
                _systemData = null;
        }

        private SystemData deepCopySystemData()
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

        private void ProcessVisualEvent(JournalEntry je)
        {
            try
            {
                JObject eventData = je.json.JSONParse().Object();

                switch (je.eventid)
                {
                    case "Scan":
                        ProcessScan(eventData);
                        break;
                    case "FSSBodySignals":
                        ProcessScan(eventData);
                        break;
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
                        Console.WriteLine($"EDDCanonn: Skip unsupported event: " + je.eventid);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"EDDCanonn: Error processing event for visual feedback: {je.eventid} : {ex.Message}");
            }
        }

        private void ProcessNewSystem(JObject eventData)
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
                    systemData.HasCoordinate = true;
                }
            }
        }

        private void fetchScanData(JObject eventData, Body body) //Call this method only via the '_lockSystemData' lock. Otherwise it could get bad.
        {
            //Even if the event tells us that a body has been mapped, we do not apply this because we cannot know if it is also the case for the databases.
            if (body.ScanData == null)
            {
                body.ScanData = new ScanData
                {
                    //Primitives
                    BodyID = body.BodyID,
                    ScanType = eventData["ScanType"]?.Value?.ToString(),

                    //List<JObject>  
                    Signals = CanonnHelper.GetJObjectList(eventData, "Signals"),
                    SurfaceFeatures = CanonnHelper.GetJObjectList(eventData, "SurfaceFeatures"),
                    Rings = CanonnHelper.GetJObjectList(eventData, "Rings"),
                    Organics = CanonnHelper.GetJObjectList(eventData, "Organics"),
                    Genuses = CanonnHelper.GetJObjectList(eventData, "Genuses"),
                };
            }
            else
            {
                //Primitives
                body.ScanData.BodyID = body.BodyID;
                body.ScanData.ScanType = eventData["ScanType"]?.Value?.ToString();

                //List<JObject>  
                body.ScanData.Signals = CanonnHelper.GetUniqueEntries(eventData, "Signals", body.ScanData.Signals);
                body.ScanData.Rings = CanonnHelper.GetUniqueEntries(eventData, "Rings", body.ScanData.Rings,"Belt");
                body.ScanData.Organics = CanonnHelper.GetUniqueEntries(eventData, "Organics", body.ScanData.Organics);
                body.ScanData.Genuses = CanonnHelper.GetUniqueEntries(eventData, "Genuses", body.ScanData.Genuses);
                body.ScanData.SurfaceFeatures = CanonnHelper.GetUniqueEntries(eventData, "SurfaceFeatures", body.ScanData.SurfaceFeatures);
            }
        }

        private void IdentifyNodeType(Body body, JObject eventData) //Call this method only via the '_lockSystemData' lock. Otherwise it could get bad.
        {
            if (body.NodeType != null) return;
            if (eventData.Contains("PlanetClass") && !eventData["PlanetClass"].IsNull)
                body.NodeType = "body";
            else if (eventData.Contains("StarType") && !eventData["StarType"].IsNull)
                body.NodeType = "star";
            else if (body.BodyName?.Contains("Belt") == true)
                body.NodeType = "belt";
            else if (body.BodyName?.Contains("Ring") == true)
                body.NodeType = "ring";
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
      
                int bodyId = CanonnHelper.GetValueOrDefault(eventData["BodyID"]?? null, -1);
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

                fetchScanData(eventData, body);

                return body;
            }
        }

        private void ProcessFSSDiscoveryScan(JObject eventData)
        {
            if (systemData == null)
                systemData = new SystemData(); //Enforces encapsulation.

            lock (_lockSystemData)
            {
                systemData.FSSTotalBodies = eventData["BodyCount"]?.ToObject<int>() ?? -1;
                systemData.FSSTotalNonBodies = eventData["NonBodyCount"]?.ToObject<int>() ?? -1;
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
        #region ProcessCallbackSystem

        public void ProcessCallbackSystem(JObject root)
        {
            if (root == null || root.Count == 0) // If this is true after a ‘Location’ event, the system is later initialised via that event.
                return;

            if (systemData == null)
                systemData = new SystemData(); //Enforces encapsulation and creates a new SystemData instance internally, disregarding any parameters.

            lock (_lockSystemData)
            {
                try
                {
                    if (root["System"] != null)
                    {
                        JObject systemDataNode = root["System"]?.Object();
                        if (systemDataNode != null)
                            ProcessCallbackSystemData(systemDataNode);
                    }

                    if (root["StarNodes"] != null)
                    {
                        JObject starNodes = root["StarNodes"]?.Object();
                        if (starNodes != null)
                            ProcessCallbackStarNodes(starNodes);
                    }

                    systemData.FSSSignalList = CanonnHelper.GetJObjectList(root, "FSSSignalList");
                    systemData.CodexEntryList = CanonnHelper.GetJObjectList(root, "CodexEntryList");

                    systemData.FSSTotalBodies = root["FSSTotalBodies"]?.ToObject<int>() ?? -1;
                    systemData.FSSTotalNonBodies = root["FSSTotalNonBodies"]?.ToObject<int>() ?? -1;

                }
                catch (Exception ex)
                {
                    resetSystemData(); //If something goes wrong here, we assume that no data is available. If this happens after a ‘Location’ event, the system is initialised via that event.
                    Console.Error.WriteLine($"EDDCanonn: Error processing CallbackSystem for visual feedback: {ex.Message}");
                }

            }
        }

        private void ProcessCallbackSystemData(JObject system)
        {
            // Extract and populate main system details
            systemData.Name = system["Name"].Value?.ToString();
            systemData.X = CanonnHelper.GetValueOrDefault(system["X"] ?? null, CanonnHelper.PositionFallback);
            systemData.Y = CanonnHelper.GetValueOrDefault(system["Y"] ?? null, CanonnHelper.PositionFallback);
            systemData.Z = CanonnHelper.GetValueOrDefault(system["Z"] ?? null, CanonnHelper.PositionFallback);
            systemData.HasCoordinate = system["HasCoordinate"]?.ToObject<bool>() ?? false;
            systemData.SystemAddress = CanonnHelper.GetValueOrDefault(system["SystemAddress"] ?? null, -1l);
        }

        private void ProcessCallbackStarNodes(JObject starNodes, int? parentBodyId = null) //wip
        {
            if (starNodes == null)
                return;

            foreach (KeyValuePair<string, JToken> property in starNodes)
            {
                JObject starNode = property.Value as JObject;

                if (starNode == null || starNode.Count == 0)
                {
                    continue;
                }

                // Extract BodyID; skip processing if invalid
                int bodyId = CanonnHelper.GetValueOrDefault(starNode["BodyID"] ?? null, -1);
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
                    BodyName = starNode["BodyDesignator"].Value?.ToString(),
                    IsMapped = starNode["IsMapped"]?.ToObject<bool>() ?? false,
                    NodeType = starNode["NodeType"].Value?.ToString(),
                
                };


                JObject scanDataNode = starNode["ScanData"] as JObject;
                if (scanDataNode != null)
                {
                    body.ScanData = new ScanData
                    {
                        //Primitives
                        ScanType = scanDataNode["ScanType"].Value?.ToString(),
                        BodyID = bodyId,

                        //List<JObject>    
                        Signals = CanonnHelper.GetJObjectList(scanDataNode, "Signals"),
                        SurfaceFeatures = CanonnHelper.GetJObjectList(scanDataNode, "SurfaceFeatures"),
                        Rings = CanonnHelper.GetJObjectList(scanDataNode, "Rings", "Belt"),
                        Organics = CanonnHelper.GetJObjectList(scanDataNode, "Organics"),
                        Genuses = CanonnHelper.GetJObjectList(scanDataNode, "Genuses"),
                    };
                }

                systemData.Bodys[bodyId] = body;

                // Recursively process child nodes
                JObject children = starNode["Children"] as JObject;
                if (children != null)
                {
                    ProcessCallbackStarNodes(children, bodyId);
                }
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
                            lock (_lockSystemData) ProcessCallbackSystem(o);
                            SafeInvoke(() =>
                            {
                                this.Enabled = true;
                            });
                            activated = true;
                            UpdateMainFields(); // wip
                            UpdateUpperGridViews(); // wip
                        }
                        else if (rt.Equals(RequestTag.Log))
                        {
                            SafeInvoke(() =>
                            {
                                DebugLog.AppendText(data ?? "none" + "\n");
                            });
                        }
                    }
                    else if (requestTag is JObject jb)
                    {
                        string evt = jb["event"]?.Value?.ToString();
                        if (evt == "Location" || evt == "FSDJump") //Triggered by 'jump' or 'location' Event.
                        {
                            lock (_lockSystemData)
                            {
                                ProcessCallbackSystem(o);
                                if (systemData == null) ProcessNewSystem(jb);
                            }
                            activated = true;
                        }
                        UpdateMainFields(); // wip
                        UpdateUpperGridViews(); // wip
                    }
                },
                ex => Console.Error.WriteLine($"EDDCanonn: Error processing Systemdata: {ex.Message}"),
                "DataResult");
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

            List<DataGridViewRow> rows = new List<DataGridViewRow>();
            rows.Add(CanonnHelper.CreateDataGridViewRow(dataGridViewData, new object[] { "Missing Bio Data:", null, new Bitmap(1, 1) }));

            foreach (Body body in system.Bodys.Values)
            {
                if (body?.ScanData?.Signals == null) continue;

                JObject o = CanonnHelper.FindFirstMatchingJObject(body.ScanData.Signals, "Type", "$SAA_SignalType_Biological;"); if (o == null) continue; 

                if (body.ScanData.Genuses == null || body.ScanData.Genuses.Count == 0)
                {
                    rows.Add(CanonnHelper.CreateDataGridViewRow(dataGridViewData, new object[] { body.BodyName, $"{CanonnHelper.GetValueOrDefault(o["Count"] ?? null, 0)} unknown species", Properties.Resources.biology })); continue;
                }

                foreach (JObject genus in body.ScanData.Genuses)
                {
                    string genusName = genus["Genus"]?.Value?.ToString();
                    if (string.IsNullOrWhiteSpace(genusName)) continue;

                    if (body.ScanData.Organics == null || !CanonnHelper.ContainsKeyValuePair(body.ScanData.Organics, "Genus", genusName))
                    {
                        rows.Add(CanonnHelper.CreateDataGridViewRow(dataGridViewData, new object[] { body.BodyName, $"Sample needed: {genus["Genus_Localised"]?.Value?.ToString() ?? genusName}", Properties.Resources.biology }));
                    }
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

            List<DataGridViewRow> rows = new List<DataGridViewRow>();
            rows.Add(CanonnHelper.CreateDataGridViewRow(dataGridViewData, new object[] { "Missing Rings:", null, new Bitmap(1, 1) }));
;
            foreach (Body body in system.Bodys.Values)
            {
                if (body?.ScanData?.Rings == null) continue;

                foreach (JObject ring in body.ScanData.Rings)
                {
                    string ringName = ring["Name"]?.Value?.ToString();
                    if (system.GetBodyByName(ringName)?.IsMapped == true) continue;
                    if (ringName?.Contains("Ring") != true) continue;

                    rows.Add(CanonnHelper.CreateDataGridViewRow(dataGridViewData, new object[] { body.BodyName, ringName, Properties.Resources.ring }));
                }
            }
            if (rows.Count <= 1)
            {
                CanonnHelper.DisposeDataGridViewRowList(rows); return null;
            }
            return rows;
        }
        #endregion

        #region UIUpdate

        private void UpdateMainFields() //wip
        {
            SystemData system = deepCopySystemData();

            if (system == null)
                return;

            SafeBeginInvoke(() =>
            {
                textBoxSystem.Clear();
                textBoxBodyCount.Clear();

                textBoxSystem.AppendText(system.Name ?? "none");
                textBoxBodyCount.AppendText(system.CountBodysFilteredByNodeType(new string[] { "body", "star" }) + " / " + system.FSSTotalBodies);
            });
        }

        private void UpdateDataGridPatrol(List<DataGridViewRow> result)
        {
            SafeBeginInvoke(() =>
            {
                dataGridPatrol.Rows.Clear();
                dataGridPatrol.Rows.AddRange(result.ToArray());
            });
        }

        private void UpdateUpperGridViews()
        {
            SystemData system = deepCopySystemData();
            if (system == null)
                return;

            SafeBeginInvoke(() =>
            {
                dataGridViewData.Rows.Clear();
                dataGridViewRing.Rows.Clear();
                dataGridViewBio.Rows.Clear();

                List<DataGridViewRow> ringRows = CollectRingData(system);
                if (ringRows != null && ringRows.Count > 0)
                {
                    dataGridViewData.Rows.AddRange(CanonnHelper.CloneDataGridViewRowList(ringRows).ToArray());
                    dataGridViewRing.Rows.AddRange(CanonnHelper.CloneDataGridViewRowList(ringRows).ToArray());
                }
                List<DataGridViewRow> bioRows = CollectBioData(system);
                if (bioRows != null && bioRows.Count > 0)
                {
                    dataGridViewData.Rows.AddRange(CanonnHelper.CloneDataGridViewRowList(bioRows).ToArray());
                    dataGridViewBio.Rows.AddRange(CanonnHelper.CloneDataGridViewRowList(bioRows).ToArray());
                }

                CanonnHelper.DisposeDataGridViewRowList(ringRows);
                CanonnHelper.DisposeDataGridViewRowList(bioRows);
            });
        }

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
                BeginInvoke(action);
            }
            else
            {
                Console.WriteLine("EDDCanonn: UI update skipped because the form handle no longer exists.");
            }
        }

        private void SafeInvoke(Action action)
        {
            if (this.IsHandleCreated && !this.IsDisposed)
            {
                Invoke(action);
            }
            else
            {
                Console.WriteLine("EDDCanonn: UI update skipped because the form handle no longer exists.");
            }
        }
        #endregion

        #region IEDDPanelExtension

        private bool activated = false;
        public void NewUnfilteredJournal(JournalEntry je)
        {
            if (isAbort)
                return;
            try
            {
                dataHandler.StartTaskAsync(
                (token) =>
                {
                    JObject o = je.json.JSONParseObject();
                    if (je.eventid.Equals("FSDJump")) //Prepare data for the next system. Include ‘FSDJump’ event as requestTag in case the System is unknown.
                    {
                        activated = false; //As long as we make a callback, we hold back the events (canonn events excluded).
                        resetSystemData();

                        //The callback may take some time. That's why we give the user some feedback in an early stage.
                        NotifyField(textBoxSystem, je.systemname);
                        NotifyField(textBoxBodyCount, "Fetch...");
                        SafeBeginInvoke(() =>
                        {
                            DLLCallBack.RequestScanData(o, this, je.systemname, true);
                        });
                        return;
                    }
                    else if (je.eventid.Equals("Location")) //Prepare data for the next system. Include ‘Location’ event as requestTag in case of a crash. 
                    {
                        activated = false;
                        resetSystemData();

                        NotifyField(textBoxSystem, je.systemname);
                        NotifyField(textBoxBodyCount, "Fetch...");
                        SafeBeginInvoke(() =>
                        {
                            DLLCallBack.RequestScanData(o, this, je.systemname, true);
                        });
                        return;
                    }
                    else
                    {
                        if (IsEventValid(je.eventid, je.json)) //Send event to canonn if valid.
                        {
                            //Payload.BuildPayload(je, getStatusJson()); --> wip
                        }

                        while (!activated) //Wait until a 'DataResult' has been completed.
                        {
                            token.ThrowIfCancellationRequested();
                            Task.Delay(250, token).Wait();
                        }

                        ProcessVisualEvent(je);
                    }
                    UpdateMainFields(); //wip
                    UpdateUpperGridViews(); // wip
                },
                ex =>
                {
                    Console.Error.WriteLine($"EDDCanonn: Error processing JournalEntry: {ex.Message}");
                },
                    "NewUnfilteredJournal: " + je.eventid
                );
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"EDDCanonn: Unexpected error in NewUnfilteredJournal: {ex.Message}");
            }
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

        private JObject getStatusJson() //Return a deep copy.
        {
            lock (_lockStatusJson)
                return new JObject(_statusJson);
        }

        public bool SupportTransparency => true;

        public bool DefaultTransparent => false;

        public bool AllowClose()
        {
            return true;
        }

        public void Closing()
        {
            CanonnHelper.InstanceCount--;
            dataHandler.Closing();
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

        private void LogWhitelist_Click(object sender, EventArgs e)
        {
            PrintWhitelist();
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
            SystemData system = deepCopySystemData();
            DebugLog.AppendText(system?.ToString() ?? "none" + "\n");
        }

        private void CallSystem_Click(object sender, EventArgs e)
        {
            DLLCallBack.RequestScanData(RequestTag.Log, this, "", true);
        }
        #endregion
    }
}
