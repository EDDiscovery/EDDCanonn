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
using EDDCanonnPanel.Base;
using KdTree;
using KdTree.Math;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using System.Text.RegularExpressions;
using System.Net;
using EDDCanonnPanel.Utility;


namespace EDDCanonnPanel
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
                if (CanonnUtil.InstanceCount > 0)
                {
                    CanonnUtil.InstanceCount++;
                    //Abort if a Canonn Panel already exists.
                    Abort("Only one Canonn Panel instance can be active. " +
                    "Use the main instance or close all other instances and restart EDD."); 
                    return;
                }
                else
                    CanonnUtil.InstanceCount++;

                linkLabelEDDCanonn.Text = "EDDCanonn v" + CanonnUtil.V;

                InitializeNews();
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
                        CanonnLogging.Instance.LogToFile(error);
                    },
                        "Startup : History-Listener"
                    );
                }
            }
            catch (Exception ex)
            {
                string error = $"EDDCanonn: Error during Startup: {ex.Message}";
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

                ExtComboBoxRange.Items.AddRange(DataUtil.PatrolRanges.Select(x => x.ToString() + " LY").ToArray());

                ConcurrentBag<Task> _tasks = new ConcurrentBag<Task>(); //We have to make sure that all workers are finished before we update the patrols.

                //Start a 'head worker' to avoid blocking the UI thread.
                dataHandler.StartTaskAsync(
                (token) =>
                {
                    //Fetch information about available patrols.
                    List<Dictionary<string, string>> records = DataUtil.ParseTsv(dataHandler.FetchData(LinkUtil.PatrolUrl).response);
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
                                        CanonnLogging.Instance.LogToFile(error);
                                    },
                                        "InitializePatrol -> SubThread: " + description
                                    ));
                            }
                        }
                        catch (Exception ex)
                        {
                            string error = $"EDDCanonn: Error processing record category: {ex.Message}";
                            CanonnLogging.Instance.LogToFile(error);
                        }
                    }
                },
                ex =>
                {
                    string error = $"EDDCanonn: Error In Patrols HeadThread: {ex.Message}";
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
                        patrolToolStripLock = false;
                    })
                );
            }
            catch (Exception ex)
            {
                string error = $"EDDCanonn: Error in InitializePatrols: {ex.Message}";
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
                List<Dictionary<string, string>> records = DataUtil.ParseTsv(dataHandler.FetchData(url).response);

                foreach (Dictionary<string, string> record in records)
                {
                    token.ThrowIfCancellationRequested(); //We abort here if a cancellation was requested.

                    try
                    {
                        string patrolType = record.TryGetValue("Patrol", out string patrolValue) ? patrolValue :
                            record.TryGetValue("Type", out string typeValue) ? typeValue : string.Empty;

                        string system = record.TryGetValue("System", out string systemValue) ? systemValue : string.Empty;

                        double x = CanonnUtil.GetValueOrDefault(new JToken(record["X"] ?? null),
                            DataUtil.PositionFallback);
                        double y = CanonnUtil.GetValueOrDefault(new JToken(record["Y"] ?? null),
                            DataUtil.PositionFallback);
                        double z = CanonnUtil.GetValueOrDefault(new JToken(record["Z"] ?? null),
                            DataUtil.PositionFallback);

                        string instructions = record.TryGetValue("Instructions", out string instructionsValue) ? instructionsValue : "none";
                        string urlp = record.TryGetValue("Url", out string urlValue) ? urlValue : string.Empty;

                        //Construct a new Patrol object and insert it into the KdTree.
                        Patrol patrol = new Patrol(patrolType, category, system, x, y, z, instructions, urlp);
                        kdT.Add(new double[] { patrol.x, patrol.y, patrol.z }, patrol);
                    }
                    catch (Exception ex)
                    {
                        string error = $"EDDCanonn: Error processing record: {ex.Message}";
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

                        double x = CanonnUtil.GetValueOrDefault(record["x"] ?? null,
                            DataUtil.PositionFallback);
                        double y = CanonnUtil.GetValueOrDefault(record["y"] ?? null,
                            DataUtil.PositionFallback);
                        double z = CanonnUtil.GetValueOrDefault(record["z"] ?? null,
                            DataUtil.PositionFallback);

                        string instructions = record["instructions"]?.Value?.ToString() ?? "none";
                        string urlp = record["url"]?.Value?.ToString() ?? string.Empty;

                        //Construct a new Patrol object and insert it into the KdTree.
                        Patrol patrol = new Patrol(category, system, x, y, z, instructions, urlp);
                        kdT.Add(new double[] { patrol.x, patrol.y, patrol.z }, patrol);
                    }
                    catch (Exception ex)
                    {
                        string error = $"EDDCanonn: Error processing JSON record: {ex.Message}";
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
                CanonnLogging.Instance.LogToFile(error);
            }
        }

        private readonly object _patrolLock = new object();
        private bool patrolToolStripLock = true;
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
                            range = ExtComboBoxRange.HasChildren && DataUtil.PatrolRanges.Length > 0
                                ? (ExtComboBoxRange.SelectedIndex >= 0 && ExtComboBoxRange.SelectedIndex < DataUtil.PatrolRanges.Length
                                    ? DataUtil.PatrolRanges[ExtComboBoxRange.SelectedIndex]
                                    : DataUtil.PatrolRanges[0])
                                : -1;
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
                CanonnLogging.Instance.LogToFile(error);
            },
                "UpdatePatrols",
                new Action(
                    () => //We are still in the 'UpdatePatrolsThread' here. We forward this action as 'ContinueWith' -> ExecuteSynchronously.
                    {
                        SafeInvoke(() =>
                        {
                            patrolToolStripLock = false;
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
            if(patrolToolStripLock)
                return;
            else patrolToolStripLock = true;
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

            LinkUtil.OpenUrl(cellValue.ToString());
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

        //This only affects the data structure for visual feedback.
        #region SystemData

        private readonly object _lockSystemData = new object();
        private SystemData _systemData; //Do not use this. Otherwise it could get bad.
        private void ResetSystemData() //In the event of a jump/location or if web data is not available.
        {
            lock (_lockSystemData)
            {
                _systemData = null;
                CanonnUtil.DisposeDataGridViewRowList(_dataGridNotifications);
            }
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
                    CanonnLogging.Instance.LogToFile(mg);
                    UpdateUI();
                }

            }
            catch (Exception ex)
            {
                string error = $"EDDCanonn: Error processing event for visual feedback: {je.eventid} : {ex.Message}";
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
                        systemData.X = CanonnUtil.GetValueOrDefault(eventData["StarPos"][0] ?? null, DataUtil.PositionFallback);
                        systemData.Y = CanonnUtil.GetValueOrDefault(eventData["StarPos"][1] ?? null, DataUtil.PositionFallback);
                        systemData.Z = CanonnUtil.GetValueOrDefault(eventData["StarPos"][2] ?? null, DataUtil.PositionFallback);
                    }
                }
            }
            catch (Exception ex) //If we end up here. We have a problem.
            {
                ResetSystemData(); //In any case, we set the system to null.
                string error = $"EDDCanonn: Error processing NewSystem: {ex.Message}";
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
                    Signals = CanonnUtil.GetJObjectList(eventData, "Signals"),
                    Rings = CanonnUtil.GetJObjectList(eventData, "Rings"),
                    Organics = CanonnUtil.GetJObjectList(eventData, "Organics"),
                    Genuses = CanonnUtil.GetJObjectList(eventData, "Genuses"),
                };
            }
            else
            {
                //Primitives
                body.ScanData.BodyID = body.BodyID;

                //List<JObject>  
                body.ScanData.Signals = CanonnUtil.GetUniqueEntries(eventData, "Signals", body.ScanData.Signals);
                body.ScanData.Rings = CanonnUtil.GetUniqueEntries(eventData, "Rings", body.ScanData.Rings);
                body.ScanData.Organics = CanonnUtil.GetUniqueEntries(eventData, "Organics", body.ScanData.Organics);
                body.ScanData.Genuses = CanonnUtil.GetUniqueEntries(eventData, "Genuses", body.ScanData.Genuses);
                body.ScanData.SurfaceFeatures = CanonnUtil.GetUniqueEntries(eventData, "SurfaceFeatures", body.ScanData.SurfaceFeatures);
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
                    systemData.Bodys = new SortedDictionary<int, Body>();
                }

                int bodyId = CanonnUtil.GetValueOrDefault(eventData["BodyID"] ?? null, -1);
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
                    //We just set the name again. In case the node was initialized without a name.
                    body.BodyName = bodyName;
                    //Same here.
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
                systemData.BodyCount = CanonnUtil.GetValueOrDefault(eventData["BodyCount"] ?? null, -1);
            }
        }

        private void ProcessSAAScan(JObject eventData)
        {
            if (systemData == null)
                systemData = new SystemData(); //Enforces encapsulation.

            lock (_lockSystemData)
            {
                int bodyId = CanonnUtil.GetValueOrDefault(eventData["BodyID"] ?? null, -1);
                if (bodyId == -1) return;

                Body body;
                if (!systemData.Bodys?.ContainsKey(bodyId) ?? false) //In the event that the SAA scan comes before the scan event.
                    body = ProcessScan(eventData); //We are entering a second lock here. --> Not Nice. 
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
                    systemData.Bodys = new SortedDictionary<int, Body>();
                }

                int bodyId = CanonnUtil.GetValueOrDefault(eventData["Body"] ?? null, -1);

                if (bodyId == -1)
                    return;

                Body body;

                if (!systemData.Bodys.ContainsKey(bodyId)) //We have to take consider here if someone freshly installs EDD and then directly scans an organic on a planet he has already been on.
                {
                    body = new Body
                    {
                        BodyID = bodyId,
                        BodyName = "none", //No node name in scan organic.
                        NodeType = "planet"
                    };
                    systemData.Bodys[bodyId] = body;
                }
                else
                    body = systemData.Bodys[bodyId];

                if (eventData["Genus"] == null)
                    return;


                JObject o = new JObject
                {
                    ["Organics"] = new JArray { new JObject { ["Genus"] = eventData["Genus"].Value?.ToString() } }
                };

                body.ScanData.Organics = CanonnUtil.GetUniqueEntries(o, "Organics", body.ScanData.Organics);
            }
        }
        #endregion

        //This only affects the data structure for visual feedback.
        #region ProcessSpanshDump

        public void ProcessSpanshDump(JObject root)
        {
            if (root == null || root.Count == 0) // If this is true after a ‘Location’ / 'Jump' event, the system is later initialised via that event.
            {
                DataGridNotifications.Add(CanonnUtil.CreateDataGridViewRow(dataGridViewData, new object[] { "This system is not yet known to spansh.", Properties.Resources.spansh }));
                return;
            }

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

                    if(systemData.SystemAddress != -1)
                        ProcessCanonnBiostats(systemData.SystemAddress);

                    if(systemData.BodyCount == -1 && systemData.Bodys?.Count > 0)
                        DataGridNotifications.Add(CanonnUtil.CreateDataGridViewRow(dataGridViewData, new object[] { "Spansh data may be incorrect. FSS all bodies.", Properties.Resources.spansh }));

                }
                catch (Exception ex)
                {
                    ResetSystemData(); //If something goes wrong here, we assume that no data is available. If this happens after a ‘Location’ / 'Jump' event, the system is initialised via that event.
                    string error = $"EDDCanonn: Error processing CallbackSystem for visual feedback: {ex.Message}";
                    CanonnLogging.Instance.LogToFile(error);
                }
            }
        }

        private void ProcessSpanshSystemData(JObject system)
        {
            // Extract and populate main system details
            systemData.Name = system["name"].Value?.ToString();

            systemData.X = CanonnUtil.GetValueOrDefault(system["coords"]?["x"] ?? null, DataUtil.PositionFallback);
            systemData.Y = CanonnUtil.GetValueOrDefault(system["coords"]?["y"] ?? null, DataUtil.PositionFallback);
            systemData.Z = CanonnUtil.GetValueOrDefault(system["coords"]?["z"] ?? null, DataUtil.PositionFallback);

            systemData.SystemAddress = CanonnUtil.GetValueOrDefault(system["id64"] ?? null, -1l);
            systemData.BodyCount = CanonnUtil.GetValueOrDefault(system["bodyCount"] ?? null, -1);
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
                int bodyId = CanonnUtil.GetValueOrDefault(node["bodyId"] ?? null, -1);
                if (bodyId == -1)
                {
                    continue;
                }

                if (systemData.Bodys == null)
                {
                    systemData.Bodys = new SortedDictionary<int, Body>();
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
                    Signals = CanonnUtil.GetJObjectList(node["signals"] as JObject, "signals"),
                    Genuses = CanonnUtil.GetJObjectList(node["signals"] as JObject, "genuses", "Genus"),
                    Rings = CanonnUtil.GetJObjectList(node, "rings"),
                };

                systemData.Bodys[bodyId] = body;
            }
        }

        private void ProcessCanonnBiostats(long i64)
        {
            JObject o = dataHandler.FetchData(LinkUtil.BioStats + i64).response.JSONParse().Object();
            if (o?["system"]?["bodies"] == null || o["system"]["bodies"].Count == 0)
                return;

            CanonnLogging.Instance.LogToFile(Environment.NewLine + "=== Canonn DataResult ===" + Environment.NewLine + o.ToString(true, "  ") ?? "none" + Environment.NewLine);

            foreach (JObject node in o["system"]["bodies"])
            {
                if (node == null || node.Count == 0)
                {
                    continue;
                }

                int bodyId = CanonnUtil.GetValueOrDefault(node["bodyId"] ?? null, -1);
                if (bodyId == -1)
                {
                    continue;
                }

                Body body = systemData?.Bodys?[bodyId] ?? null;
                if (body == null)
                {
                    continue;
                }

                if (body.ScanData == null)
                {
                    continue;
                }

                List<JObject> genuses = CanonnUtil.GetJObjectList(node["signals"] as JObject, "genuses", "Genus");
                List<JObject> biology = CanonnUtil.GetJObjectList(node["signals"] as JObject, "biology", "Bio");

                if (genuses == null || biology == null) 
                { 
                    continue; 
                }

                if(biology.Count == 0 || genuses.Count == 0)
                {
                    continue;
                }

                body.ScanData.Organics = genuses
                    .Where(genus =>
                    {
                        string genusLocalised = DataUtil.GenusLocalised(genus["Genus"]?.Value?.ToString());
                        return biology.Any(bio => bio["Bio"]?.Value?.ToString()?.Contains(genusLocalised) == true);
                    })
                    .ToList();
            }
        }

        #endregion

        //This only affects the data structure for visual feedback.
        #region ProcessData

        public enum RequestTag
        {
            OnStart
        }

        public void DataResult(object requestTag, string data)
        {
            if (isAbort) return;

            try
            {
                if (requestTag == null || data == null)
                    return; //No tag or data, nothing to process.

                JObject o = data.JSONParse().Object();
                if (!(requestTag is RequestTag || requestTag is JObject))
                    return; //Invalid tag format.

                CanonnLogging.Instance.LogToFile(Environment.NewLine + "=== DataResult ===" + Environment.NewLine + o.ToString(true, "  ") ?? "none" + Environment.NewLine);

                dataHandler.StartTaskAsync((token) =>
                {
                    if (requestTag is RequestTag rt)
                    {
                        if (rt.Equals(RequestTag.OnStart)) //Triggered by panel creation.
                        {
                            lock (_lockSystemData) ProcessSpanshDump(o); //I don't think the lock is necessary. But safety first.
                            SafeInvoke(() => this.Enabled = true);
                            _eventLock = _journalLock = true; //Plugin startup complete.
                            if (systemData == null) lock(_lockSystemData) systemData = new SystemData();                   
                        }
                        else if (false)
                        {

                        }
                    }
                    else if (requestTag is JObject jb && new[] { "Location", "FSDJump" }.Contains(jb["event"]?.Value?.ToString()))
                    {
                        lock (_lockSystemData) //I still have no idea. See above.
                        {
                            ProcessSpanshDump(o);
                            if (systemData == null) ProcessNewSystem(jb);
                        }
                        _eventLock = true; //Process collected events.
                    }

                    UpdateUI(true);
                    UpdatePatrols();
                },
                ex =>
                {
                    string error = $"EDDCanonn: Error processing Systemdata: {ex.Message}";
                    CanonnLogging.Instance.LogToFile(error);
                },
                    "DataResult"
                );
            }
            catch (Exception ex)
            {
                CanonnLogging.Instance.LogToFile($"EDDCanonn: Unexpected error in DataResult: {ex.Message}");
            }
        }
        #endregion

        #region ProcessUpperGridViews

        private List<DataGridViewRow> CollectBioData(SystemData system)
        {
            if (system?.Bodys?.Values == null) return null;

            List<DataGridViewRow> rows = new List<DataGridViewRow>
            {
                CanonnUtil.CreateDataGridViewRow(dataGridViewBio, new object[] { "Missing Bio Data:", null, new Bitmap(1, 1) })
            };

            Regex nameRegex = new Regex(@"^[A-Za-z\s-]+\d+-\d+\s+(.+)$", RegexOptions.IgnoreCase);


            //Filter only bodies that have biological signals.
            IEnumerable<Body> validBodies = system.Bodys.Values
                .Where(body => body?.ScanData?.Signals?.Any() == true);

            foreach (Body body in validBodies)
            {
                //Find the first biological signal entry.
                JObject biologicalSignal = CanonnUtil.FindFirstMatchingJObject(body.ScanData.Signals, "Type", "$SAA_SignalType_Biological;") ??
                                           CanonnUtil.FindFirstMatchingJObject(body.ScanData.Signals, null, "$SAA_SignalType_Biological;");

                if (biologicalSignal == null) continue;

                int bioCount = CanonnUtil.GetValueOrDefault(biologicalSignal["Count"], 0);
                bioCount = bioCount == 0
                    ? CanonnUtil.GetValueOrDefault(biologicalSignal["$SAA_SignalType_Biological;"], 0)
                    : bioCount;

                Match match = nameRegex.Match(body.BodyName);

                //If no genus data is available, add an entry with unknown species count.
                if (body.ScanData.Genuses?.Any() != true)
                {
                    rows.Add(CanonnUtil.CreateDataGridViewRow(dataGridViewBio, new object[]
                    {
                        match.Success ? match.Groups[1].Value : body.BodyName,
                        $"{bioCount} unknown species",
                        Properties.Resources.biology
                    }));
                    continue;
                }

                //Find samples that have not been collected yet.
                List<JObject> missingSamples = body.ScanData.Genuses
                    .Where(genus => !CanonnUtil.ContainsKeyValuePair(body.ScanData.Organics, "Genus", genus["Genus"]?.Value?.ToString()))
                    .ToList();

                if (missingSamples.Any())
                {
                    rows.AddRange(missingSamples.Select((genus, index) =>
                    CanonnUtil.CreateDataGridViewRow(dataGridViewBio, new object[]
                    {
                        index == 0 ? match.Success ? match.Groups[1].Value : body.BodyName : null, //Only show body name for the first missing sample.
                        "sample required: " + DataUtil.GenusLocalised(genus["Genus"]?.Value?.ToString()),
                        Properties.Resources.biology
                    })));
                }
            }

            if (rows.Count <= 1)
            {
                CanonnUtil.DisposeDataGridViewRowList(rows);
                return null;
            }

            return rows;
        }

        private List<DataGridViewRow> CollectRingData(SystemData system)
        {
            if (system?.Bodys?.Values == null) return null;

            List<DataGridViewRow> rows = new List<DataGridViewRow>
            {
                CanonnUtil.CreateDataGridViewRow(dataGridViewRing, new object[] { "Missing Rings:", null, null, new Bitmap(1, 1) })
            };

            Regex ringRegex = new Regex(@"([A-Z])\s+Ring$", RegexOptions.IgnoreCase); //Pattern to extract short names.
            Regex nameRegex = new Regex(@"^[A-Za-z\s-]+\d+-\d+\s+(.+)$", RegexOptions.IgnoreCase);

            //Filter only bodies that have ring data.
            IEnumerable<Body> validBodies = system.Bodys.Values
                .Where(body => body?.ScanData?.Rings != null);

            foreach (Body body in validBodies)
            {
                //Find missing rings that are not mapped or do not contain an id64.
                List<JObject> missingRings = body.ScanData.Rings
                    .Where(ring =>
                    {
                        string ringName = ring["name"]?.Value?.ToString() ?? ring["Name"]?.Value?.ToString();
                        return ringName?.Contains("Ring") == true
                            && !(system.GetBodyByName(ringName)?.IsMapped == true || ring.Contains("id64"));
                    })
                    .ToList();

                if (!missingRings.Any()) continue;

                rows.AddRange(missingRings.Select((ring, index) =>
                {
                    string ringName = ring["name"]?.Value?.ToString() ?? ring["Name"]?.Value?.ToString();
                    //Extract inner and outer radius values, converting from meters to light-seconds.
                    double[] values = new[] { "innerRadius", "InnerRad", "outerRadius", "OuterRad" }
                        .Select(k => CanonnUtil.GetValueOrDefault(ring[k], 0.0) / 299792458)
                        .Select(v => Math.Round(v, 2))
                        .ToArray();

                    string result = $"{(values[0] == 0.0 ? values[1] : values[0])} ls - {(values[2] == 0.0 ? values[3] : values[2])} ls";

                    Match matchRing = ringRegex.Match(ringName);
                    Match matchName = nameRegex.Match(body.BodyName);

                    return CanonnUtil.CreateDataGridViewRow(dataGridViewRing, new object[]
                    {
                        index == 0 ? matchName.Success ? matchName.Groups[1].Value : body.BodyName : null, //Only show the body name for the first ring.
                        matchRing.Success ? matchRing.Groups[1].Value + " Ring" : ringName,
                        result,
                        Properties.Resources.ring
                    });
                }));
            }

            if (rows.Count <= 1)
            {
                CanonnUtil.DisposeDataGridViewRowList(rows);
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
            rows.Add(CanonnUtil.CreateDataGridViewRow(dataGridViewGMO, new object[] { combinedDescriptions }));

            rows.Add(CanonnUtil.CreateDataGridViewRow(dataGridViewGMO, new object[] { null }));

            rows.Add(CanonnUtil.CreateDataGridViewRow(dataGridViewGMO, new object[] { gmos[0]?["DescriptiveNames"]?[0]?.Value?.ToString() + ":" }));
            rows.Add(CanonnUtil.CreateDataGridViewRow(dataGridViewGMO, new object[] { gmos[0]?["Description"]?.Value?.ToString() }));

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
                        News = dataHandler.FetchData(LinkUtil.CanonnNews).response.JSONParseArray();
                        if (News == null || News.Count == 0) return;
                        NewsIndexChanged(0);
                    }
                    catch (Exception ex)
                    {
                        string error = $"EDDCanonn: Error in InitializeNews: {ex}";
                        CanonnLogging.Instance.LogToFile(error);
                    }
                },
                ex =>
                {
                    string error = $"EDDCanonn: Unexpected error in InitializeNews Task: {ex}";
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
                        CanonnLogging.Instance.LogToFile(error);
                    }

                    textBoxNews.AppendText(decoded);
                    labelNewsIndex.Text = "Page: " + (NewsIndex + 1);
                }
                catch (Exception ex)
                {
                    string error = $"EDDCanonn: Unexpected error in NewsIndexChanged: {ex.Message}";
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
                        CanonnLogging.Instance.LogToFile(error);
                    }
                },
                ex =>
                {
                    string error = $"EDDCanonn: Error during UpdateUI execution: {ex}";
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
                    textBoxBodyCount.AppendText(system.CountBodysFilteredByNodeType(new string[] { "Planet", "Star" }) + " / " + (system.BodyCount == -1 ? "?" : system.BodyCount.ToString()));
                }
                catch (Exception ex)
                {
                    string error = $"EDDCanonn: Error in UpdateMainFields: {ex}";
                    CanonnLogging.Instance.LogToFile(error);
                }
            });
        }

        private List<DataGridViewRow> _dataGridNotifications;
        private List<DataGridViewRow> DataGridNotifications
        {
            get
            {
                lock (_lockSystemData)
                {
                    if (_dataGridNotifications == null)
                        _dataGridNotifications = new List<DataGridViewRow>();
                    return _dataGridNotifications;
                }
            }
        }
        private void UpdateUpperGridViews(SystemData system, bool jumped)
        {
            if (system == null) return;

            SafeBeginInvoke(() =>
            {
                try
                {
                    //Clear all grid views before populating them with new data.
                    dataGridViewData.Rows.Clear();
                    dataGridViewRing.Rows.Clear();
                    dataGridViewBio.Rows.Clear();
                    dataGridViewGMO.Rows.Clear();                 

                    dataGridViewData.Rows.AddRange(CanonnUtil.CloneDataGridViewRowList(DataGridNotifications).ToArray());

                    if (system.BodyCount == -1)
                    {
                        dataGridViewData.Rows.Add(CanonnUtil.CreateDataGridViewRow(dataGridViewData, new object[]
                        { "D-Scan is required.", Properties.Resources.tourist }));
                    }

                    //Collect missing ring data and update the corresponding grid.
                    List<DataGridViewRow> ringRows = CollectRingData(system);
                    dataGridViewRing.Rows.AddRange(ringRows?.ToArray() ??
                        new[] { CanonnUtil.CreateDataGridViewRow(dataGridViewRing, new object[]
                        { "No missing ring data for this system yet.", null, null, new Bitmap(1, 1) }) });

                    if (ringRows?.Count > 0)
                    {
                        dataGridViewData.Rows.Add(CanonnUtil.CreateDataGridViewRow(dataGridViewData, new object[]
                        { "There are new rings that can be scanned.", Properties.Resources.ring }));
                    }

                    //Collect missing bio data and update the corresponding grid.
                    List<DataGridViewRow> bioRows = CollectBioData(system);
                    dataGridViewBio.Rows.AddRange(bioRows?.ToArray() ??
                        new[] { CanonnUtil.CreateDataGridViewRow(dataGridViewBio, new object[]
                        { "No missing bio data for this system yet.", null, new Bitmap(1, 1) }) });

                    if (bioRows?.Count > 0)
                    {
                        dataGridViewData.Rows.Add(CanonnUtil.CreateDataGridViewRow(dataGridViewData, new object[]
                        { "There is new bio data available for scanning.", Properties.Resources.biology }));
                    }

                    //If no jump occurred, no need to check.
                    if (!jumped) return;

                    //Collect GMO data and update the corresponding grid.
                    List<DataGridViewRow> gmoRows = CollectGMO(system);
                    dataGridViewGMO.Rows.AddRange(gmoRows?.ToArray() ??
                        new[] { CanonnUtil.CreateDataGridViewRow(dataGridViewGMO, new object[]
                        { "No GMO was found for this system." }) });

                    if (gmoRows?.Count > 0)
                    {
                        dataGridViewData.Rows.Add(CanonnUtil.CreateDataGridViewRow(dataGridViewData, new object[]
                        { "A GMO is available for this system.", Properties.Resources.tourist }));
                    }
                }
                catch (Exception ex)
                {
                    string error = $"EDDCanonn: Error in UpdateUpperGridViews: {ex}";
                    CanonnLogging.Instance.LogToFile(error);
                }
            });
        }

        #region Links

        private void linkLabelEDDCanonn_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkUtil.OpenUrl(LinkUtil.EDDCanonnGitHub);
        }

        private void pictureBoxCanonn_Click(object sender, EventArgs e)
        {
            LinkUtil.OpenUrl(LinkUtil.CanonnWebPage);
        }

        private void pictureBoxEDD_Click(object sender, EventArgs e)
        {
            LinkUtil.OpenUrl(LinkUtil.EDDGitHub);
        }

        private void textBoxNews_Click(object sender, EventArgs e)
        {
            LinkUtil.OpenUrl(News[NewsIndex]?["link"]?.Value?.ToString() ?? LinkUtil.CanonnWebPage);
        }

        private void textBoxSystem_Click(object sender, EventArgs e)
        {
            LinkUtil.OpenUrl(LinkUtil.SignalsCanonnTech + systemData?.Name?.Replace(" ", "%20"));
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
                            CanonnLogging.Instance.LogToFile(error);
                        }
                    }));
                }
                catch (Exception ex)
                {
                    string error = $"EDDCanonn: UI update failed in SafeBeginInvoke: {ex}";
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
                            CanonnLogging.Instance.LogToFile(error);
                        }
                    }));
                }
                catch (Exception ex)
                {
                    string error = $"EDDCanonn: UI update failed in SafeInvoke: {ex}";
                    CanonnLogging.Instance.LogToFile(error);
                }
            }
        }
        #endregion

        #region IEDDPanelExtension

        private readonly object _canonnPushLock = new object();
        //If false, jornals are ignored. Only released after the 'OnStart' data result.
        private bool _journalLock = false;
        //If false, events are held back.
        private bool _eventLock = false;
        public void NewUnfilteredJournal(JournalEntry je)
        {
            if (isAbort || !_journalLock)
                return;

            CanonnLogging.Instance.LogToFile("03");

            dataHandler.StartTaskAsync(
            (token) =>
            {
                JObject o = je.json.JSONParseObject();
                string eventId = je.eventid;

                if (eventId.Equals("FSDJump") || eventId.Equals("Location"))
                {
                    _eventLock = false; // As long as we make a callback, we hold back the events (canonn events excluded).
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

                while (!_eventLock) // Wait until a 'DataResult' has been completed.
                {
                    token.ThrowIfCancellationRequested();
                    Task.Delay(250, token).Wait();
                }

                ProcessEvent(je);
            },
            ex =>
            {
                string error = $"EDDCanonn: Error processing JournalEntry: {ex.Message}";
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

        public void NewUIEvent(string jsonui)
        {
            if (isAbort)
                return;
        }

        public bool SupportTransparency => false;

        public bool DefaultTransparent => false;

        public bool AllowClose()
        {
            return true;
        }

        public void Closing()
        {
            CanonnUtil.InstanceCount--;
            dataHandler.Closing();
        }

        public void Initialise(EDDPanelCallbacks callbacks, int displayid, string themeasjson, string configuration)
        {
            DLLCallBack = CanonnEDDClass.DLLCallBack;
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
        }

        public void InitialDisplay()
        {
            if (isAbort)
                return;
        }

        public void LoadLayout()
        {
            PanelCallBack.LoadGridLayout(dataGridPatrol);
            PanelCallBack.LoadGridLayout(dataGridViewData);
            PanelCallBack.LoadGridLayout(dataGridViewBio);
            PanelCallBack.LoadGridLayout(dataGridViewRing);
            PanelCallBack.LoadGridLayout(dataGridViewGMO);
            PanelCallBack.LoadGridLayout(dataGridSignals);
        }

        public void ScreenShotCaptured(string file, Size s)
        {
            if (isAbort)
                return;
        }

        public void SetTransparency(bool ison, Color curcol)
        {
            if (isAbort)
                return;

            this.BackColor = curcol;

        }

        public void TransparencyModeChanged(bool on)
        {
            if (isAbort)
                return;
        }

        void IEDDPanelExtension.CursorChanged(JournalEntry je)
        {
            if (isAbort)
                return;
        }

        public void ControlTextVisibleChange(bool on)
        {
            if (isAbort)
                return;
        }

        public string HelpKeyOrAddress()
        {
            return LinkUtil.EDDCanonnGitHub;
        }
        #endregion

        #region Debug

        private bool isAbort = false;
        private void Abort(string msg = "default")
        {
            isAbort = true;
            _journalLock = false;
            _eventLock = false;

            NotifyMainFields("Aborted");
            extTabControlData.SelectedIndex = extTabControlData.TabCount - 1;

            dataHandler.Closing();

            this.Enabled = false;
            _eventLock = false;

            patrols = null;
            News = null;                
        }

        private void LSY_Click(object sender, EventArgs e)
        {

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

        private void dataGridSignals_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            HandleDataGridViewError("dataGridSignals", e);
        }

        private void HandleDataGridViewError(string gridName, DataGridViewDataErrorEventArgs e)
        {
            string error = $"EDDCanonn: DataGridView error in {gridName} (Row: {e.RowIndex}, Column: {e.ColumnIndex}): {e.Exception}";
            CanonnLogging.Instance.LogToFile(error);
            e.ThrowException = false;
        }
        #endregion
    }
}
