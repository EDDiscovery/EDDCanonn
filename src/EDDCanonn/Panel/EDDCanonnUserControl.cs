/******************************************************************************
 * 
 * Copyright © 2022-2022 EDDiscovery development team
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at:
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 ******************************************************************************/

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
using System.ComponentModel;
using System.Reflection;


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
                InitializeCodex();
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
                        string error = $"EDDCanonn: Unexpected error in \"Wait for History\" Thread: {ex}";
                        CanonnLogging.Instance.Log(error);
                    },
                        "Startup : History-Listener"
                    );
                    }
                }
            catch (Exception ex)
                {
                string error = $"EDDCanonn: Error during Startup: {ex}";
                CanonnLogging.Instance.Log(error);
                Abort(error);
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
                                        string error = $"EDDCanonn: Error Initialize Patrols -> {description}: {ex}";
                                        CanonnLogging.Instance.Log(error);
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
                                        string error = $"EDDCanonn: Error Initialize Patrols -> {description}: {ex}";
                                        CanonnLogging.Instance.Log(error);
                                    },
                                        "InitializePatrol -> SubThread: " + description
                                    ));
                                }
                            }
                        catch (Exception ex)
                            {
                            string error = $"EDDCanonn: Error processing record category: {ex}";
                            CanonnLogging.Instance.Log(error);
                            }
                        }
                },
                ex =>
                {
                    string error = $"EDDCanonn: Error In Patrols HeadThread: {ex}";
                    CanonnLogging.Instance.Log(error);
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
                        });
                        patrolToolStripLock = false;
                    })
                );
                }
            catch (Exception ex)
                {
                string error = $"EDDCanonn: Error in InitializePatrols: {ex}";
                CanonnLogging.Instance.Log(error);
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

                        double x = new JToken(record["X"]).Double(DataUtil.PositionFallback);
                        double y = new JToken(record["Y"]).Double(DataUtil.PositionFallback);
                        double z = new JToken(record["Z"]).Double(DataUtil.PositionFallback);

                        string instructions = record.TryGetValue("Instructions", out string instructionsValue) ? instructionsValue : "none";
                        string urlp = record.TryGetValue("Url", out string urlValue) ? urlValue : string.Empty;

                        //Construct a new Patrol object and insert it into the KdTree.
                        Patrol patrol = new Patrol(patrolType, category, system, x, y, z, instructions, urlp);
                        kdT.Add(new double[] { patrol.x, patrol.y, patrol.z }, patrol);
                        }
                    catch (Exception ex)
                        {
                        string error = $"EDDCanonn: Error processing record: {ex}";
                        CanonnLogging.Instance.Log(error);
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
                string error = $"EDDCanonn: Error in CreateFromTSV for category {category}: {ex}";
                CanonnLogging.Instance.Log(error);
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

                        string system = record["system"].Str();

                        double x = record["x"].Double(DataUtil.PositionFallback);
                        double y = record["y"].Double(DataUtil.PositionFallback);
                        double z = record["z"].Double(DataUtil.PositionFallback);

                        string instructions = record["instructions"].Str("none");
                        string urlp = record["url"].Str();

                        //Construct a new Patrol object and insert it into the KdTree.
                        Patrol patrol = new Patrol(category, system, x, y, z, instructions, urlp);
                        kdT.Add(new double[] { patrol.x, patrol.y, patrol.z }, patrol);
                        }
                    catch (Exception ex)
                        {
                        string error = $"EDDCanonn: Error processing JSON record: {ex}";
                        CanonnLogging.Instance.Log(error);
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
                string error = $"EDDCanonn: Error in CreateFromJson for category {category}: {ex}";
                CanonnLogging.Instance.Log(error);
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

                        if (string.IsNullOrEmpty(type) || range == -1)
                            return;

                        List<(string category, Patrol patrol, double distance)> patrolList = patrols.FindPatrolsInRange(type, system.X, system.Y, system.Z, range);

                        List<DataGridViewRow> result = new List<DataGridViewRow>();
                        foreach (var (category, patrol, distance) in patrolList)
                            result.Add(CanonnUtil.CreateDataGridViewRow(dataGridPatrol, new object[] { patrol.category, patrol.instructions, distance.ToString(), patrol.system, patrol.url }));

                        UpdateDataGridPatrol(result); // Update UI with new patrol list.
                        }
                },
            ex =>
            {
                string error = $"EDDCanonn: Error during UpdatePatrols Task: {ex}";
                CanonnLogging.Instance.Log(error);
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
            if (patrolToolStripLock)
                return;
            else
                patrolToolStripLock = true;
            UpdatePatrols();
            }

        private void CopySystem_Click(object sender, EventArgs e)
            {
            if (dataGridPatrol.SelectedCells.Count == 0)
                return;

            int rowIndex = dataGridPatrol.SelectedCells[0].RowIndex;
            if (rowIndex < 0 || rowIndex >= dataGridPatrol.Rows.Count)
                return;

            DataGridViewRow selectedRow = dataGridPatrol.Rows[rowIndex];
            if (dataGridPatrol.Columns["SysName"] == null || selectedRow.Cells["SysName"] == null)
                return;

            object cellValue = selectedRow.Cells["SysName"].Value;
            if (cellValue == null)
                return;

            try
                {
                Clipboard.SetText(cellValue.ToString());
                }
            catch (Exception ex)
                {
                string error = $"EDDCanonn: Clipboard error:: {ex}";
                CanonnLogging.Instance.Log(error);
                }
            }

        private void OpenUrl_Click(object sender, EventArgs e)
            {
            if (dataGridPatrol.SelectedCells.Count == 0)
                return;

            int rowIndex = dataGridPatrol.SelectedCells[0].RowIndex;
            if (rowIndex < 0 || rowIndex >= dataGridPatrol.Rows.Count)
                return;

            DataGridViewRow selectedRow = dataGridPatrol.Rows[rowIndex];
            if (dataGridPatrol.Columns["PatrolUrl"] == null || selectedRow.Cells["PatrolUrl"] == null)
                return;

            object cellValue = selectedRow.Cells["PatrolUrl"].Value;
            if (cellValue == null)
                return;

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

        #region Codex
        private CodexDatabase CodexDatabase;
        private void InitializeCodex()
            {
            CodexDatabase = new CodexDatabase();

            dataHandler.StartTaskAsync(
            (token) =>
            {
                JObject o = dataHandler.FetchData(LinkUtil.CodexRef).response?.JSONParse().Object()
                    ?? DataUtil.ReadEmbeddedTextFile("EDDCanonnPanel.Resources.codex_ref.json")?.JSONParse().Object();

                if (o == null)
                    return;

                o.PropertyNames()
                    .SelectMany(type => o[type] is JObject typeObj
                        ? typeObj.PropertyNames()
                            .SelectMany(subType => typeObj[subType] is JObject subTypeObj
                                ? subTypeObj.PropertyNames()
                                    .Select(entryKey => subTypeObj[entryKey] is JObject obj ? new CodexEntry(
                                        type,
                                        subType,
                                        entryKey ?? CanonnUtil.GenerateId().ToString(),
                                        obj["entryid"].Int(CanonnUtil.GenerateId()),
                                        obj["name"].Str(CanonnUtil.GenerateId().ToString()),
                                        obj["category"].StrNull(),
                                        obj["sub_category"].StrNull(),
                                        obj["platform"].StrNull(),
                                        obj["reward"].Int(-1)
                                    ) : null)
                                : Enumerable.Empty<CodexEntry>())
                        : Enumerable.Empty<CodexEntry>())
                    .Where(entry => entry != null)
                    .ToList()
                    .ForEach(CodexDatabase.Add);
            },
            ex =>
            {
                string error = $"EDDCanonn: Unexpected error in InitializeCodex: {ex}";
                CanonnLogging.Instance.Log(error);
            },
                "InitializeCodex"
            );
            }
        #endregion

        #region SystemData

        private readonly object _lockSystemData = new object();
        private SystemData _systemData; //Do not use this. Otherwise it could get bad.
        private void ResetSystemData()
            {
            lock (_lockSystemData)
                {
                    SafeInvoke(() =>
                    {
                        _systemData = null;
                        extTabControlData.SelectedIndex = 0;
                        CanonnUtil.DisposeDataGridViewRowList(_dataGridNotifications.Values.ToList());
                        _dataGridNotifications.Clear();
                    });
                }
            }

        private SystemData DeepCopySystemData()
            {
            lock (_lockSystemData)
                {
                if (_systemData != null)
                    return new SystemData(_systemData);
                else
                    return null;
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
                        lock (_lockSystemData)
                            ProcessScan(eventData);
                        break;
                    case "ScanOrganic":
                        lock (_lockSystemData)
                            ProcessOrganic(eventData);
                        break;
                    case "FSSDiscoveryScan":
                        lock (_lockSystemData)
                            ProcessFSSDiscoveryScan(eventData);
                        break;
                    case "FSSAllBodiesFound":
                        lock (_lockSystemData)
                            ProcessFSSAllBodiesFound(eventData);
                        break;
                    case "SAAScanComplete":
                        lock (_lockSystemData)
                            ProcessSAAScan(eventData);
                        break;
                    case "CodexEntry":
                        lock (_lockSystemData)
                            ProcessCodex(eventData);
                        break;
                    default:
                        matched = false;
                        break;
                    }

                if (matched) //We only update the UI when changes are made.
                    {
                    if (systemData?.Name != je.systemname && _eventLock)
                        {
                        /************************************************************************************
                        * There is a possibility that the plugin does not start from the correct system,
                        * especially if it was launched during an FSD. To correct such cases,
                        * a check is performed for each valid event.                         
                        ************************************************************************************/
                        lock (_lockSystemData)
                            {
                            _eventLock = false;
                            SafeBeginInvoke(() =>
                            {
                                CanonnLogging.Instance.Log($"EDDCanonn: system name does not match. Fetch again: {systemData?.Name} <--Plugin | Journal--> {je.systemname}");
                                ResetSystemData();
                                DLLCallBack.RequestSpanshDump(RequestTag.ReFetch, this, je.systemname, je.systemaddress, true, false, null);
                            });
                            return;
                            }
                        }

                    string mg = $"EDDCanonn: Processed event for visual feedback: " + je.eventid;
                    CanonnLogging.Instance.Log(mg);
                    UpdateUI();
                    }

                }
            catch (Exception ex)
                {
                string error = $"EDDCanonn: Error processing event for visual feedback: {je.eventid} : {ex}";
                CanonnLogging.Instance.Log(error);
                }
            }

        /************************************************************************************
         * CRITICAL NOTICE:
         * Always call event-related methods exclusively within the '_lockSystemData' lock.
         * DO NOT IGNORE THIS.
         ************************************************************************************/

        private void ProcessNewSystem(JObject eventData)
            {
            try
                {
                if (systemData == null)
                    systemData = new SystemData(); //Enforces encapsulation and creates a new SystemData instance internally, disregarding any parameters.

                systemData.Name = eventData["StarSystem"].StrNull() ?? eventData["System"].Str("none");
                systemData.SystemAddress = eventData["SystemAddress"].Long(-1);
                if (eventData["StarPos"] != null)
                    {
                    systemData.X = eventData["StarPos"][0].Double(DataUtil.PositionFallback);
                    systemData.Y = eventData["StarPos"][1].Double(DataUtil.PositionFallback);
                    systemData.Z = eventData["StarPos"][2].Double(DataUtil.PositionFallback);
                    }
                }
            catch (Exception ex) //If we end up here. We have a problem.
                {
                ResetSystemData(); //In any case, we set the system to null.
                string error = $"EDDCanonn: Error processing NewSystem: {ex}";
                CanonnLogging.Instance.Log(error);
                }
            }

        private void FetchScanData(JObject eventData, Base.Body body)
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
                body.ScanData.Genuses = CanonnUtil.GetUniqueEntries(eventData, "Genuses", body.ScanData.Genuses);
                }
            }

        private void IdentifyNodeType(Base.Body body, JObject eventData)
            {
            if (body.NodeType != null)
                return;
            if (eventData.Contains("PlanetClass") && !eventData["PlanetClass"].IsNull)
                body.NodeType = "Planet";
            else if (eventData.Contains("StarType") && !eventData["StarType"].IsNull)
                body.NodeType = "Star";
            else if (body.BodyName?.Contains("Belt") == true)
                body.NodeType = "Belt";
            else if (body.BodyName?.Contains("Ring") == true)
                body.NodeType = "Ring";
            }

        private Base.Body ProcessScan(JObject eventData)
            {
            if (systemData == null)
                ProcessNewSystem(eventData);
            if (systemData == null)
                return null;

            if (systemData.Bodys == null)
                {
                systemData.Bodys = new SortedDictionary<int, Base.Body>();
                }

            int bodyId = eventData["BodyID"].Int(eventData["Body"].Int(-1));
            if (bodyId == -1)
                return null;

            string bodyName = eventData["BodyName"].Str("none");

            Base.Body body;

            if (!systemData.Bodys.ContainsKey(bodyId))
                {
                body = new Base.Body
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

        private void ProcessFSSDiscoveryScan(JObject eventData)
            {
            if (systemData == null)
                systemData = new SystemData(); //Enforces encapsulation.

            systemData.BodyCount = eventData["BodyCount"].Int(-1);
            DataGridNotifications.Remove(NType.dscanRequired);
            }

        private void ProcessFSSAllBodiesFound(JObject eventData)
            {
            DataGridNotifications.Remove(NType.spanshIncorrect);
            DataGridNotifications.Remove(NType.systemNotKnown);
            }

        private void ProcessSAAScan(JObject eventData)
            {
            int bodyId = eventData["BodyID"].Int(-1);
            if (bodyId == -1)
                return;

            Base.Body body;
            if (!systemData?.Bodys?.ContainsKey(bodyId) ?? true) //In the event that the SAA scan comes before the scan event.
                body = ProcessScan(eventData);
            else
                body = systemData.Bodys[bodyId];

            if (body == null)
                return;

            body.IsMapped = true;
            }

        private void ProcessOrganic(JObject eventData)
            {
            int bodyId = eventData["Body"].Int(-1);
            if (bodyId == -1)
                return;

            Base.Body body;
            if (!systemData?.Bodys?.ContainsKey(bodyId) ?? true)
                body = ProcessScan(eventData);
            else
                body = systemData.Bodys[bodyId];

            if (body == null)
                return;

            if (eventData["Genus"] == null)
                return;

            string variant = eventData["Variant"].Value?.ToString();
            string genus = eventData["Genus"].Value?.ToString();
            string variant_localised = CodexDatabase.GetByName(variant).LocalisedName;

            JObject o = new JObject
                {
                ["Organics"] = new JArray
                    {
                        new JObject
                        {
                            ["Variant"] = variant,                          //$Codex_Ent_Tussocks_09_F_Name;
                            ["Variant_Localised"] = variant_localised,      //Tussock Propagito - Yellow
                            ["Genus"] = genus                               //$Codex_Ent_Tussocks_Genus_Name;
                        }
                    }
                };

            body.ScanData.Organics = CanonnUtil.GetUniqueEntries(o, "Organics", body.ScanData.Organics);

            if (eventData["ScanType"].StrNull() == "Analyse")
                return;

            if (body.ScanData.SystemPois == null)
                body.ScanData.SystemPois = new List<SystemPoi>();

            SystemPoi poi = new SystemPoi(
                body.BodyName?.Replace(systemData?.Name, "")?.TrimStart(),
                variant_localised,
                -1,
                "Biology",
                -1,
                "-",
                "-",
                true
            );

            body.ScanData.SystemPois.Add(poi);

            }

        private void ProcessCodex(JObject eventData)
            {
            string category = eventData["SubCategory"].StrNull();
            if (category == null)
                return;

            if (category.Equals("$Codex_SubCategory_Organic_Structures;"))
                {
                int bodyId = eventData["BodyID"].Int(-1);
                if (bodyId == -1)
                    return;

                Base.Body body;
                if (!systemData?.Bodys?.ContainsKey(bodyId) ?? true)
                    body = ProcessScan(eventData);
                else
                    body = systemData.Bodys[bodyId];

                if (body == null)
                    return;

                string variant = eventData["Name"].StrNull();
                CodexEntry entry = CodexDatabase.GetByName(variant);

                JObject o = new JObject
                    {
                    ["Organics"] = new JArray
                        {
                            new JObject
                            {
                                ["Variant"] = variant,                                  //$Codex_Ent_Tussocks_09_F_Name;  
                                ["Variant_Localised"] = entry?.LocalisedName,           //Tussock Propagito - Yellow
                                ["Genus"] = DataUtil.BiologyGenuses(entry.CodexSubType) //$Codex_Ent_Tussocks_Genus_Name;                                                                                      
                            }
                        }
                    };

                body.ScanData.Organics = CanonnUtil.GetUniqueEntries(o, "Organics", body.ScanData.Organics);

                if (body.ScanData.SystemPois == null)
                    body.ScanData.SystemPois = new List<SystemPoi>();

                SystemPoi poi = new SystemPoi(
                    body.BodyName?.Replace(systemData?.Name, "")?.TrimStart(),
                    entry?.LocalisedName,
                    -1,
                    "Biology",
                    -1,
                    "-",
                    "-",
                    true
                );

                body.ScanData.SystemPois.Add(poi);

                }
            if (category.Equals("$Codex_SubCategory_Geology_and_Anomalies;"))
                {
                int bodyId = eventData["BodyID"].Int(-1);
                if (bodyId == -1)
                    return;

                Base.Body body;
                if (!systemData?.Bodys?.ContainsKey(bodyId) ?? true)
                    body = ProcessScan(eventData);
                else
                    body = systemData.Bodys[bodyId];

                if (body == null)
                    return;

                string variant = eventData["Name"].StrNull();
                CodexEntry entry = CodexDatabase.GetByName(variant);

                JObject o = new JObject
                    {
                    ["Geologic"] = new JArray
                        {
                            new JObject
                            {
                                ["Variant"] = variant,                              
                                ["Variant_Localised"] = entry?.LocalisedName,                                                                                              
                            }
                        }
                    };

                body.ScanData.Geologic = CanonnUtil.GetUniqueEntries(o, "Geologic", body.ScanData.Geologic);
                }
            else if (false)
                {
                //...
                }
            }

        #endregion

        #region ProcessSpanshDump

        /*********************************************************************************************************
         * CRITICAL NOTICE:
         * ProcessSpanshDump and the underlying methods should also only be called via the '_lockSystemData' lock.
         * DO NOT IGNORE THIS.
         *********************************************************************************************************/

        public void ProcessSpanshDump(JObject root)
            {
            if (root == null || root.Count == 0) // If this is true after a ‘Location’ / 'Jump' event, the system is later initialised via that event.
                {
                DataGridNotifications[NType.systemNotKnown] = CanonnUtil.CreateDataGridViewRow(dataGridViewData, new object[] {
                    NTypeDescription(NType.systemNotKnown),
                    Properties.Resources.spansh });
                return;
                }

            if (systemData == null)
                systemData = new SystemData(); //Enforces encapsulation and creates a new SystemData instance internally, disregarding any parameters.

            try
                {
                JObject systemDataNode = root["system"]?.Object() ?? null;
                if (systemDataNode != null)
                    ProcessSpanshSystemData(systemDataNode);
                else
                    throw new Exception($"EDDCanonn: systemDataNode is null");

                JArray bodyNode = systemDataNode?["bodies"]?.Array() ?? null;
                if (bodyNode != null)
                    ProcessSpanshBodyNode(bodyNode);

                if (systemData.SystemAddress != -1)
                    ProcessCanonnBiostats(systemData.SystemAddress);

                ProcessCanonnSystemPoi(systemData.Name);

                if (systemData.BodyCount == -1 && systemData.Bodys?.Count > 0)
                    DataGridNotifications[NType.spanshIncorrect] = CanonnUtil.CreateDataGridViewRow(dataGridViewData, new object[] {
                            NTypeDescription(NType.spanshIncorrect),
                            Properties.Resources.spansh });
                }
            catch (Exception ex)
                {
                ResetSystemData(); //If something goes wrong here, we assume that no data is available. If this happens after a ‘Location’ / 'Jump' event, the system is initialised via that event.
                string error = $"EDDCanonn: Error processing CallbackSystem: {ex}";
                CanonnLogging.Instance.Log(error);
                }
            }

        private void ProcessSpanshSystemData(JObject system)
            {
            // Extract and populate main system details
            systemData.Name = system["name"].Str("none");

            systemData.X = system["coords"]["x"].Double(DataUtil.PositionFallback);
            systemData.Y = system["coords"]["y"].Double(DataUtil.PositionFallback);
            systemData.Z = system["coords"]["z"].Double(DataUtil.PositionFallback);

            systemData.SystemAddress = system["id64"].Long(-1);
            systemData.BodyCount = system["bodyCount"].Int(-1);
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
                int bodyId = node["bodyId"].Int(-1);
                if (bodyId == -1)
                    {
                    continue;
                    }

                if (systemData.Bodys == null)
                    {
                    systemData.Bodys = new SortedDictionary<int, Base.Body>();
                    }

                if (systemData.Bodys.ContainsKey(bodyId))
                    {
                    continue;
                    }

                Base.Body body = new Base.Body
                    {
                    //Primitives
                    BodyID = bodyId,
                    BodyName = node["name"].StrNull(),
                    NodeType = node["type"].StrNull(),
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

            CanonnLogging.Instance.Log(Environment.NewLine + "=== Canonn DataResult ===" + Environment.NewLine + o.ToString(true, "  ") ?? "none" + Environment.NewLine);

            foreach (JObject node in o["system"]["bodies"])
                {
                if (node == null || node.Count == 0)
                    {
                    continue;
                    }

                int bodyId = node["bodyId"].Int(-1);
                if (bodyId == -1)
                    {
                    continue;
                    }

                Base.Body body = systemData?.Bodys?[bodyId] ?? null;
                if (body == null)
                    {
                    continue;
                    }

                if (body.ScanData == null)
                    {
                    continue;
                    }

                List<JObject> biology = CanonnUtil.GetJObjectList(node["signals"] as JObject, "biology", "Bio");

                if (biology == null || biology.Count == 0)
                    {
                    continue;
                    }

                body.ScanData.Organics = biology
                    .Select(b =>
                    {
                        string variantLocalised = b["Bio"].StrNull();
                        CodexEntry entry = CodexDatabase?.GetByLocalisedName(variantLocalised);

                        return new JObject
                            {
                            ["Variant"] = entry?.Name,
                            ["Variant_Localised"] = variantLocalised,
                            ["Genus"] = DataUtil.BiologyGenuses(entry?.CodexSubType)
                            };
                    })
                    .ToList();
                }
            }

        private void ProcessCanonnSystemPoi(string systemName)
            {
            try
                {
                JObject o = dataHandler.FetchData(LinkUtil.GetCanonnSystemPoi(systemName, CanonnUtil.UserName)).response?.JSONParse().Object();
                if (o == null || o["codex"] == null)
                    return;

                foreach (JObject entry in o["codex"])
                    {
                    Base.Body body = systemData?.GetBodyByName($"{systemName} {entry["body"].StrNull()}");
                    if (body == null)
                        continue;

                    if (body.ScanData == null)
                        continue;

                    if (body.ScanData.SystemPois == null)
                        body.ScanData.SystemPois = new List<SystemPoi>();

                    SystemPoi poi = new SystemPoi(
                        entry["body"].StrNull(),
                        entry["english_name"].StrNull(),
                        entry["entryid"].Int(-1),
                        entry["hud_category"].StrNull(),
                        entry["index_id"].Int(-1),
                        entry["latitude"].Str("-"),
                        entry["longitude"].Str("-"),
                        entry["scanned"].Str("false").Equals("true", StringComparison.OrdinalIgnoreCase)
                    );

                    body.ScanData.SystemPois.Add(poi);
                    }
                }
            catch (Exception ex)
                {
                string error = $"EDDCanonn: Unexpected error in ProcessCanonnSystemPoi: {ex}";
                CanonnLogging.Instance.Log(error);
                }
            }

        #endregion

        #region ProcessData

        public enum RequestTag
            {
            OnStart,
            ReFetch
            }

        public void DataResult(object requestTag, string data)
            {
            if (isAbort)
                return;

            try
                {
                if (requestTag == null || data == null)
                    return; //No tag or data, nothing to process.

                JObject o = data.JSONParse().Object();
                if (!(requestTag is RequestTag || requestTag is JObject))
                    return; //Invalid tag format.

                CanonnLogging.Instance.Log(Environment.NewLine + "=== DataResult ===" + Environment.NewLine + o.ToString(true, "  ") ?? "none" + Environment.NewLine);

                dataHandler.StartTaskAsync((token) =>
                {
                    if (requestTag is RequestTag rt)
                        {
                        if (rt.Equals(RequestTag.OnStart) || rt.Equals(RequestTag.ReFetch)) //Triggered by panel creation.
                            {
                            lock (_lockSystemData)
                                ProcessSpanshDump(o);
                            SafeInvoke(() => this.Enabled = true);
                            _eventLock = _journalLock = true; //Plugin startup complete.
                            if (systemData == null)
                                lock (_lockSystemData)
                                    systemData = new SystemData();
                            }
                        else if (false)
                            {
                            //...
                            }
                        }
                    else if (requestTag is JObject jb && new[] { "Location", "FSDJump" }.Contains(jb["event"].StrNull()))
                        {
                        lock (_lockSystemData)
                            {
                            ProcessSpanshDump(o);
                            if (systemData == null)
                                ProcessNewSystem(jb);
                            }
                        _eventLock = true; //Process collected events.
                        }

                    UpdateUI(true);
                    UpdatePatrols();
                },
                ex =>
                {
                    string error = $"EDDCanonn: Error processing Systemdata: {ex}";
                    CanonnLogging.Instance.Log(error);
                },
                    "DataResult"
                );
                }
            catch (Exception ex)
                {
                CanonnLogging.Instance.Log($"EDDCanonn: Unexpected error in DataResult: {ex}");
                }
            }
        #endregion

        #region ProcessUpperGridViews

        private (List<DataGridViewRow> missing, List<DataGridViewRow> existing) CollectBioData(SystemData system)
            {
            if (system?.Bodys?.Values == null)
                return (null, null);

            List<DataGridViewRow> missingRows = new List<DataGridViewRow>
            {
                CanonnUtil.CreateDataGridViewRow(dataGridViewBio, new object[] { "Missing Bio Data:", null, new Bitmap(1, 1), "false" })
            };

            List<DataGridViewRow> existingRows = new List<DataGridViewRow>
            {
                CanonnUtil.CreateDataGridViewRow(dataGridViewBio, new object[] { "Existing Bio Data:", null, new Bitmap(1, 1), "false" })
            };

            //Filter only bodies that have biological signals.
            IEnumerable<Base.Body> validBodies = system.Bodys.Values
                .Where(body => body?.ScanData?.Signals?.Any() == true);

            foreach (Base.Body body in validBodies)
                {
                //Find the first biological signal entry.
                JObject biologicalSignal = CanonnUtil.FindFirstMatchingJObject(body.ScanData.Signals, "Type", "$SAA_SignalType_Biological;") ??
                                           CanonnUtil.FindFirstMatchingJObject(body.ScanData.Signals, null, "$SAA_SignalType_Biological;");

                if (biologicalSignal == null)
                    continue;

                int bioCount = biologicalSignal["Count"].Int(-1);
                bioCount = bioCount == -1
                    ? biologicalSignal["$SAA_SignalType_Biological;"].Int(-1)
                    : bioCount;

                //If no genus data is available, add an entry with unknown.
                if (body.ScanData.Genuses?.Any() != true)
                    {
                    missingRows.Add(CanonnUtil.CreateDataGridViewRow(dataGridViewBio, new object[]
                    {
                        body.BodyName?.Replace(system.Name, "") ?? "none",
                        $"{bioCount} unknown species",
                        Properties.Resources.biology,
                        "false"
                    }));
                    continue;
                    }

                //Find samples that have not been collected yet.
                List<JObject> missingSamples = body.ScanData.Genuses
                    .Where(genus => !CanonnUtil.ContainsKeyValuePair(body.ScanData.Organics, "Genus", genus["Genus"].StrNull()))
                    .ToList();

                if (missingSamples.Count < body.ScanData.Genuses.Count && body.ScanData.SystemPois?.Count > 0)
                    {
                    existingRows.Add(CanonnUtil.CreateDataGridViewRow(dataGridViewBio, new object[]
                    {
                        body.BodyName?.Replace(system.Name, "")?.TrimStart(),
                        "Double-click for an overview.",
                        Properties.Resources.biology,
                        "true"
                    }));
                    }

                if (missingSamples.Any())
                    {
                    missingRows.AddRange(missingSamples.Select((genus, index) =>
                    CanonnUtil.CreateDataGridViewRow(dataGridViewBio, new object[]
                    {
                        index == 0 ? body.BodyName?.Replace(system.Name, "")?.TrimStart() : null, //Only show body name for the first missing sample.
                        "Data required: " + genus["Genus"] != "Brain Tree" ? DataUtil.BiologyGenuses(genus["Genus"].StrNull()) : "Brain Tree",
                        Properties.Resources.biology,
                        "false"
                    })));
                    }
                }

            new[] { missingRows, existingRows }
                .Where(list => list.Count <= 1)
                .ToList()
                .ForEach(CanonnUtil.DisposeDataGridViewRowList);

            return (missingRows, existingRows);
            }

        private List<DataGridViewRow> CollectGeoData(SystemData system)
            {
            if (system?.Bodys?.Values == null)
                return null;

            List<DataGridViewRow> missingRows = new List<DataGridViewRow>
    {
        CanonnUtil.CreateDataGridViewRow(dataGridViewGeo, new object[] { "Missing Geo Data:", null, new Bitmap(1, 1), "false" })
    };

            IEnumerable<Base.Body> validBodies = system.Bodys.Values
                .Where(body => body?.ScanData?.Signals?.Any() == true);

            foreach (Base.Body body in validBodies)
                {
                JObject geologicalSignal = CanonnUtil.FindFirstMatchingJObject(body.ScanData.Signals, "Type", "$SAA_SignalType_Geological;") ??
                                           CanonnUtil.FindFirstMatchingJObject(body.ScanData.Signals, null, "$SAA_SignalType_Geological;");

                if (geologicalSignal == null)
                    continue;

                int geoCount = geologicalSignal["Count"].Int(-1);
                geoCount = geoCount == -1
                    ? geologicalSignal["$SAA_SignalType_Geological;"].Int(-1)
                    : geoCount;

                if (geoCount <= 0)
                    continue;

                int foundCount = body.ScanData.Geologic?.Count ?? 0;
                int missingCount = geoCount - foundCount;

                if (missingCount <= 0)
                    continue;

                missingRows.Add(CanonnUtil.CreateDataGridViewRow(dataGridViewGeo, new object[]
                {
            body.BodyName?.Replace(system.Name, "")?.TrimStart(),
            $"{missingCount} missing geo signals",
            Properties.Resources.geology,
            "false"
                }));
                }

            if (missingRows.Count <= 1)
                {
                CanonnUtil.DisposeDataGridViewRowList(missingRows);
                return null;
                }

            return missingRows;
            }


        private List<DataGridViewRow> CollectBioInfoData(SystemData system)
            {      
            List<DataGridViewRow> rows = new List<DataGridViewRow>
                {
                    CanonnUtil.CreateDataGridViewRow(dataGridViewBioInfo, new object[]
                    {
                        "Double-click for return.",
                        null,
                        dataGridViewBioInfo?.AccessibleDescription,
                        "true"
                    })
                };

            if (system?.Bodys?.Values == null || dataGridViewBioInfo?.AccessibleDescription == null)
                return rows;

            Base.Body body = systemData?.GetBodyByName($"{system.Name} {dataGridViewBioInfo.AccessibleDescription}");
            if (body?.ScanData?.SystemPois == null)
                return rows;

            List<IGrouping<string, SystemPoi>> groupedPois = body.ScanData.SystemPois
                .Where(poi => poi.HudCategory == "Biology")
                .GroupBy(poi => poi.Name)
                .ToList();

            foreach (IGrouping<string, SystemPoi> group in groupedPois)
                {
                string name = group.Key;
                int total = group.Count();
                int scanned = group.Count(poi => poi.Scanned);

                rows.Add(CanonnUtil.CreateDataGridViewRow(dataGridViewBioInfo, new object[]
                {
                    name,
                    scanned + " self",
                    total + " total",
                    "false"
                }));

                foreach (SystemPoi poi in group)
                    {
                    rows.Add(CanonnUtil.CreateDataGridViewRow(dataGridViewBioInfo, new object[]
                    {
                            poi.Lat,
                            poi.Lon,
                            null,
                            "false"
                    }));
                    }
                }

            return rows;
            }

        private List<DataGridViewRow> CollectRingData(SystemData system)
            {
            if (system?.Bodys?.Values == null)
                return null;

            List<DataGridViewRow> rows = new List<DataGridViewRow>
            {
                CanonnUtil.CreateDataGridViewRow(dataGridViewRing, new object[] { "Missing Rings:", null, null, new Bitmap(1, 1) })
            };

            Regex ringRegex = new Regex(@"([A-Z])\s+Ring$", RegexOptions.IgnoreCase); //Pattern to extract short names.

            //Filter only bodies that have ring data.
            IEnumerable<Base.Body> validBodies = system.Bodys.Values
                .Where(body => body?.ScanData?.Rings != null);

            foreach (Base.Body body in validBodies)
                {
                //Find missing rings that are not mapped or do not contain an id64.
                List<JObject> missingRings = body.ScanData.Rings
                    .Where(ring =>
                    {
                        string ringName = ring["name"].Str(ring["Name"].StrNull()) ?? "none";
                        return ringName?.Contains("Ring") == true
                            && !(system.GetBodyByName(ringName)?.IsMapped == true || ring.Contains("id64"));
                    })
                    .ToList();

                if (!missingRings.Any())
                    continue;

                rows.AddRange(missingRings.Select((ring, index) =>
                {
                    string ringName = ring["name"].Str(ring["Name"].StrNull()) ?? "none";
                    //Extract inner and outer radius values, converting from meters to light-seconds.
                    double[] values = new[] { "innerRadius", "InnerRad", "outerRadius", "OuterRad" }
                        .Select(k => ring[k].Double(0.0) / 299792458)
                        .Select(v => Math.Round(v, 2))
                        .ToArray();

                    string result = $"{(values[0] == 0.0 ? values[1] : values[0])} ls - {(values[2] == 0.0 ? values[3] : values[2])} ls";

                    Match matchRing = ringRegex.Match(ringName);

                    return CanonnUtil.CreateDataGridViewRow(dataGridViewRing, new object[]
                    {
                        index == 0 ? body.BodyName?.Replace(system.Name, "") : null, //Only show the body name for the first ring.
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
            JArray gmos = DLLCallBack.GetGMOs("systemname=" + system.Name)?.JSONParseArray() ?? null;
            if (gmos == null || gmos.Count == 0)
                return null;

            List<DataGridViewRow> rows = new List<DataGridViewRow>();

            string combinedDescriptions = "Tags: " + string.Join(", ", gmos[0]?["GalMapTypes"]?.Array()
                ?.Select(galMapType => galMapType?["Description"].StrNull())
                .Where(desc => !string.IsNullOrWhiteSpace(desc)) ?? Enumerable.Empty<string>());
            rows.Add(CanonnUtil.CreateDataGridViewRow(dataGridViewGMO, new object[] { combinedDescriptions }));

            rows.Add(CanonnUtil.CreateDataGridViewRow(dataGridViewGMO, new object[] { null }));

            rows.Add(CanonnUtil.CreateDataGridViewRow(dataGridViewGMO, new object[] { gmos[0]?["DescriptiveNames"]?[0].StrNull() + ":" }));
            rows.Add(CanonnUtil.CreateDataGridViewRow(dataGridViewGMO, new object[] { gmos[0]?["Description"].StrNull() }));

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
                        if (News == null || News.Count == 0)
                            return;
                        NewsIndexChanged(0);
                        }
                    catch (Exception ex)
                        {
                        string error = $"EDDCanonn: Error in InitializeNews: {ex}";
                        CanonnLogging.Instance.Log(error);
                        }
                },
                ex =>
                {
                    string error = $"EDDCanonn: Unexpected error in InitializeNews Task: {ex}";
                    CanonnLogging.Instance.Log(error);
                },
                "InitializeNews"
            );
            }

        private void NewsIndexChanged(int n)
            {
            if (News == null || News.Count == 0)
                return;
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

                    string decoded = News[NewsIndex]?["excerpt"]?["rendered"].Str("none");

                    try
                        {
                        decoded = Regex.Replace(WebUtility.HtmlDecode(System.Text.RegularExpressions.Regex.Unescape(decoded)), "<.*?>", "");
                        }
                    catch (Exception ex)
                        {
                        string error = $"EDDCanonn: Error processing news text: {ex}";
                        CanonnLogging.Instance.Log(error);
                        }

                    textBoxNews.AppendText(decoded);
                    labelNewsIndex.Text = "Page: " + (NewsIndex + 1);
                    }
                catch (Exception ex)
                    {
                    string error = $"EDDCanonn: Unexpected error in NewsIndexChanged: {ex}";
                    CanonnLogging.Instance.Log(error);
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
                        UpdateOptionalUpperGridViews(system);
                        }
                    catch (Exception ex)
                        {
                        string error = $"EDDCanonn: Error during UpdateUI: {ex}";
                        CanonnLogging.Instance.Log(error);
                        }
                },
                ex =>
                {
                    string error = $"EDDCanonn: Error during UpdateUI execution: {ex}";
                    CanonnLogging.Instance.Log(error);
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
                    CanonnLogging.Instance.Log(error);
                    }
            });
            }

        private enum NType
            {
            [Description("This system is not yet known to spansh.")]
            systemNotKnown,
            [Description("Spansh data may be incorrect. FSS all bodies.")]
            spanshIncorrect,
            [Description("A GMO is available for this system.")]
            gmoAvailable,
            [Description("D-Scan is required.")]
            dscanRequired
            }

        private string NTypeDescription(NType type)
            {
            return
                typeof(NType)
                        .GetField(type.ToString())
                        ?.GetCustomAttribute<DescriptionAttribute>()
                        ?.Description;
            }

        private Dictionary<NType, DataGridViewRow> _dataGridNotifications;
        private Dictionary<NType, DataGridViewRow> DataGridNotifications
            {
            get
                {
                lock (_lockSystemData)
                    {
                    if (_dataGridNotifications == null)
                        _dataGridNotifications = new Dictionary<NType, DataGridViewRow>();
                    return _dataGridNotifications;
                    }
                }
            }

        private void UpdateUpperGridViews(SystemData system, bool jumped)
            {
            if (system == null)
                return;

            SafeBeginInvoke(() =>
            {
                try
                    {
                    //Clear all grid views before populating them with new data.
                    dataGridViewData.Rows.Clear();
                    dataGridViewRing.Rows.Clear();
                    dataGridViewGeo.Rows.Clear();   
                    dataGridViewBio.Rows.Clear();

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

                    //Collect missing geo data and update the corresponding grid.
                    List<DataGridViewRow> geoRows = CollectGeoData(system);
                    dataGridViewGeo.Rows.AddRange(geoRows?.ToArray() ??
                        new[] { CanonnUtil.CreateDataGridViewRow(dataGridViewGeo, new object[]
                        { "No missing geo data for this system yet.", null, null, new Bitmap(1, 1) }) });

                    if (geoRows?.Count > 0)
                        {
                        dataGridViewData.Rows.Add(CanonnUtil.CreateDataGridViewRow(dataGridViewData, new object[]
                        { "There are geological data that haven't been scanned yet.", Properties.Resources.geology }));
                        }

                    // Collect missing and existing bio data
                    ValueTuple<List<DataGridViewRow>, List<DataGridViewRow>> bioResult = CollectBioData(system);
                    List<DataGridViewRow> missingRows = bioResult.Item1;
                    List<DataGridViewRow> existingRows = bioResult.Item2;

                    // Merge both lists (null-safe)
                    List<DataGridViewRow> combinedRows = (missingRows ?? Enumerable.Empty<DataGridViewRow>())
                        .Concat(existingRows ?? Enumerable.Empty<DataGridViewRow>())
                        .ToList();

                    dataGridViewBio.Rows.AddRange(combinedRows.Count > 0
                        ? combinedRows.ToArray()
                        : new[]
                        {
                            CanonnUtil.CreateDataGridViewRow(dataGridViewBio, new object[]
                            { "No bio data for this system yet.", null, new Bitmap(1, 1) })
                        });

                    if (combinedRows.Count > 0)
                        {
                        dataGridViewData.Rows.Add(CanonnUtil.CreateDataGridViewRow(dataGridViewData, new object[]
                        { "There is bio data available for this system.", Properties.Resources.biology }));
                        }


                    //If no jump occurred, no need to check.
                    if (jumped)
                        {
                        if (system.BodyCount == -1)
                            {
                            DataGridNotifications[NType.dscanRequired] = CanonnUtil.CreateDataGridViewRow(dataGridViewData, new object[] {
                                NTypeDescription(NType.dscanRequired),
                                Properties.Resources.tourist });
                            }

                        dataGridViewGMO.Rows.Clear();
                        //Collect GMO data and update the corresponding grid.
                        List<DataGridViewRow> gmoRows = CollectGMO(system);
                        dataGridViewGMO.Rows.AddRange(gmoRows?.ToArray() ??
                            new[] { CanonnUtil.CreateDataGridViewRow(dataGridViewGMO, new object[]
                            { "No GMO was found for this system." }) });

                        if (gmoRows?.Count > 0)
                            {
                            DataGridNotifications[NType.gmoAvailable] = CanonnUtil.CreateDataGridViewRow(dataGridViewData, new object[] {
                                NTypeDescription(NType.gmoAvailable),
                                Properties.Resources.tourist });
                            }
                        }

                    dataGridViewData.Rows.AddRange(CanonnUtil.CloneDataGridViewRowList(DataGridNotifications.Values.ToList()).ToArray());
                    }
                catch (Exception ex)
                    {
                    string error = $"EDDCanonn: Error in UpdateUpperGridViews: {ex}";
                    CanonnLogging.Instance.Log(error);
                    }
            });
            }

        private bool OptionalUpperGridLock = false;
        private void UpdateOptionalUpperGridViews(SystemData system)
            {
            if (system == null)
                return;
            if (OptionalUpperGridLock)
                return;
            OptionalUpperGridLock = true;
            SafeBeginInvoke(() =>
            {
                dataGridViewBioInfo.Rows.Clear();

                List<DataGridViewRow> bioInfoRows = CollectBioInfoData(system);

                if (bioInfoRows != null)
                    dataGridViewBioInfo.Rows.AddRange(bioInfoRows.ToArray());
            });

            OptionalUpperGridLock = false;
            }

        private void dataGridViewBio_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
            {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            DataGridViewRow row = dataGridViewBio.Rows[e.RowIndex];
            DataGridViewCell clickableCell = row.Cells["clickable"];
            if (clickableCell?.Value?.ToString()?.Equals("true", StringComparison.OrdinalIgnoreCase) != true)
                return;

            DataGridViewCell nameCell = row.Cells["ColumnBio0"];
            string name = nameCell?.Value?.ToString() ?? "Bio Info";

            dataGridViewBioInfo.AccessibleDescription = name;
            CanonnUtil.SwapDGVs(dataGridViewBioInfo, dataGridViewBio, extPanelDataGridViewScrollBio, this);

            UpdateOptionalUpperGridViews(DeepCopySystemData());
            }

        private void dataGridViewBioInfo_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
            {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            DataGridViewRow row = dataGridViewBioInfo.Rows[e.RowIndex];
            DataGridViewCell clickableCell = row.Cells["clickableBioInfo"];
            if (clickableCell?.Value?.ToString()?.Equals("true", StringComparison.OrdinalIgnoreCase) != true)
                return;

            CanonnUtil.SwapDGVs(dataGridViewBio, dataGridViewBioInfo, extPanelDataGridViewScrollBio, this);
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
            LinkUtil.OpenUrl(News[NewsIndex]?["link"].StrNull() ?? LinkUtil.CanonnWebPage);
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
                            CanonnLogging.Instance.Log(error);
                            }
                    }));
                    }
                catch (Exception ex)
                    {
                    string error = $"EDDCanonn: UI update failed in SafeBeginInvoke: {ex}";
                    CanonnLogging.Instance.Log(error);
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
                            CanonnLogging.Instance.Log(error);
                            }
                    }));
                    }
                catch (Exception ex)
                    {
                    string error = $"EDDCanonn: UI update failed in SafeInvoke: {ex}";
                    CanonnLogging.Instance.Log(error);
                    }
                }
            }
        #endregion

        #region IEDDPanelExtension

        //If false, jornals are ignored. Only released after the 'OnStart' data result.
        private bool _journalLock = false;
        //If false, events are held back.
        private bool _eventLock = false;
        public void NewUnfilteredJournal(JournalEntry je)
            {
            if (isAbort || !_journalLock)
                return;

            dataHandler.StartTaskAsync(
            (token) =>
            {
                JObject o = je.json.JSONParseObject();
                string eventId = je.eventid;

                if (eventId.Equals("FSDJump") || eventId.Equals("Location"))
                    {
                    SafeBeginInvoke(() =>
                    {
                        _eventLock = false; // As long as we make a callback, we hold back the events (canonn events excluded).
                        ResetSystemData();
                        DLLCallBack.RequestSpanshDump(o, this, je.systemname, je.systemaddress, true, false, null);
                    });

                    // The callback may take some time. That's why we give the user some feedback in an early stage.
                    NotifyField(textBoxSystem, je.systemname);
                    NotifyField(textBoxBodyCount, "Fetch...");

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
                string error = $"EDDCanonn: Error processing JournalEntry: {ex}";
                CanonnLogging.Instance.Log(error);
            },
            "NewUnfilteredJournal: " + je.eventid + " in system: " + je.systemname);
            }

        private bool historyset = false;
        public void HistoryChange(int count, string commander, bool beta, bool legacy)
            {
            if (isAbort)
                return;
            if (!historyset)
                {
                historyset = true;
                CanonnUtil.UserName = commander;
                }
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

        Color FromJson(JToken color)
            {
            return System.Drawing.ColorTranslator.FromHtml(color.Str("Yellow"));
            }

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
            extTabControlData.SelectedIndex = 0;

            dataGridViewData.Rows.Add(CanonnUtil.CreateDataGridViewRow(dataGridViewData, new object[]
                        { msg, Properties.Resources.tourist }));

            dataHandler.Closing();

            this.Enabled = false;
            _eventLock = false;

            patrols = null;
            News = null;
            }

        private void EDDCanonnUserControl_Resize(object sender, EventArgs e)
            {
            this.Refresh();
            }

        private void LSY_Click(object sender, EventArgs e)
            {
            SystemData system = DeepCopySystemData();

            CanonnLogging.Instance.Log(Environment.NewLine + "=== System ToString ===" + Environment.NewLine + system.ToString() ?? "none" + Environment.NewLine);
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
            CanonnLogging.Instance.Log(error);
            e.ThrowException = false;
            }
        #endregion
        }
    }
