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

        #region Patrol

        private Patrols patrols = new Patrols();
        private void InitializePatrols() 
        {
            try
            {
                toolStripPatrol.Items.Add("all"); //The 'all' KdTree-Dictionary has already been set in “Patrols.cs”.
                toolStripRange.Items.AddRange(CanonnHelper.PatrolRanges.Cast<object>().ToArray());

                ConcurrentBag<Task> _tasks = new ConcurrentBag<Task>(); //We have to make sure that all workers are finished before we update the patrols.

                //Start a 'head worker' to avoid blocking the UI thread.
                dataHandler.StartTaskAsync(
                () =>
                {
                        //Fetch information about available patrols.
                        List<Dictionary<string, string>> records = CanonnHelper.ParseTsv(dataHandler.FetchData(CanonnHelper.PatrolUrl));
                        foreach (Dictionary<string, string> record in records)
                        {
                            try
                            {
                                string description = record.TryGetValue("Description", out string descriptionValue)? descriptionValue : "uncategorized";
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
                                        () =>
                                        {
                                            CreateFromTSV(url, description);
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
                                        () =>
                                        {
                                            CreateFromJson(url, description);
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
                        Invoke((MethodInvoker)delegate
                        {
                            toolStripPatrol.Enabled = true; toolStripPatrol.SelectedIndex = 0;
                            toolStripRange.Enabled = true; toolStripRange.SelectedIndex = 2;
                            _patrolLock = false;
                        });
                        UpdatePatrol();
                    })
                );
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"EDDCanonn: Error in InitializePatrols: {ex.Message}");
            }
        }

        private void CreateFromTSV(string url, string category)
        {
            try
            {
                KdTree<double, Patrol> kdT = new KdTree<double, Patrol>(3, new DoubleMath());
                List<Dictionary<string, string>> records = CanonnHelper.ParseTsv(dataHandler.FetchData(url));
                foreach (Dictionary<string, string> record in records)
                {
                    try
                    {
                        string patrolType = record.TryGetValue("Patrol", out string patrolValue) ? patrolValue :
                                            record.TryGetValue("Type", out string typeValue) ? typeValue : string.Empty;
                        string system = record.TryGetValue("System", out string systemValue) ? systemValue : string.Empty;

                        double x = CanonnHelper.GetValueOrDefault(new JToken(record["X"]?? null), 0.0);
                        double y = CanonnHelper.GetValueOrDefault(new JToken(record["Y"]?? null), 0.0);
                        double z = CanonnHelper.GetValueOrDefault(new JToken(record["Z"]?? null), 0.0);

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
                BeginInvoke((MethodInvoker)delegate
                {
                    toolStripPatrol.Items.Add(category);
                });  

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"EDDCanonn: Error in CreateFromTSV for category {category}: {ex.Message}");
            }
        }


        private void CreateFromJson(string url, string category)
        {
            try
            {
                KdTree<double, Patrol> kdT = new KdTree<double, Patrol>(3, new DoubleMath());
                JArray jsonRecords = dataHandler.FetchData(url).JSONParse().Array();
                if (jsonRecords == null || jsonRecords.Count == 0)
                    return;

                foreach (JObject record in jsonRecords)
                {
                    try
                    {
                        if (record == null || record.Count == 0)
                            continue;

                        string system = record["system"]?.Value?.ToString() ?? string.Empty;

                        double x = CanonnHelper.GetValueOrDefault(record["x"]?? null, 0.0);
                        double y = CanonnHelper.GetValueOrDefault(record["y"]?? null, 0.0);
                        double z = CanonnHelper.GetValueOrDefault(record["z"]?? null, 0.0);

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
                BeginInvoke((MethodInvoker)delegate
                {
                    toolStripPatrol.Items.Add(category);
                });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"EDDCanonn: Error in CreateFromJson for category {category}: {ex.Message}");
            }
        }

        private bool _patrolLock = true;
        private void UpdatePatrol()
        {
             dataHandler.StartTaskAsync(
                 () =>
                 {
                    try
                    {
                        while (systemData == null)
                        {
                            Task.Delay(1000).Wait();
                        }

                        string type = "";
                        double range = 0.0;
                        SystemData system = deepCopySystemData();

                        Invoke((MethodInvoker)delegate
                        {
                            type = toolStripPatrol.SelectedItem?.ToString() ?? "";
                            range = double.TryParse(toolStripRange.SelectedItem?.ToString(), out double r) ? r : 0.0;
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

                        BeginInvoke((MethodInvoker)delegate
                        {
                            dataGridPatrol.Rows.Clear();
                            dataGridPatrol.Rows.AddRange(result.ToArray());
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"EDDCanonn: Unexpected error in UpdatePatrol Task: {ex.Message}");
                    }
                    finally
                    {
                        _patrolLock = false;
                    }
                },
                ex => Console.Error.WriteLine($"EDDCanonn: Error during UpdatePatrol: {ex.Message}"),
                "UpdatePatrol"
            );
        }


        private void toolStripPatrol_IndexChanged(object sender, EventArgs e)
        {
            if (_patrolLock)
                return;
            _patrolLock = true;
            UpdatePatrol();
        }

        private void toolStripRange_IndexChanged(object sender, EventArgs e)
        {
            if (_patrolLock)
                return;
            _patrolLock = true;
            UpdatePatrol();
        }

        private void CopySystem_Click(object sender, EventArgs e)
        {
            if (dataGridPatrol.SelectedCells.Count == 0) return;
            DataGridViewRow selectedRow = dataGridPatrol.Rows[dataGridPatrol.SelectedCells[0].RowIndex];

            if (dataGridPatrol.Columns["SysName"] == null) return;
            object cellValue = selectedRow.Cells["SysName"].Value;
            if (cellValue == null) return;
            Clipboard.SetText(cellValue.ToString());
        }
        private void OpenUrl_Click(object sender, EventArgs e)
        {
            if (dataGridPatrol.SelectedCells.Count == 0) return;
            DataGridViewRow selectedRow = dataGridPatrol.Rows[dataGridPatrol.SelectedCells[0].RowIndex];

            if (dataGridPatrol.Columns["PatrolUrl"] == null) return;
            object cellValue = selectedRow.Cells["PatrolUrl"].Value;
            if (cellValue == null) return;
            CanonnHelper.OpenUrl(cellValue.ToString());
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

        private void InitializeWhitelist()
        {
            try
            {
                // Fetch the whitelist
                dataHandler.FetchDataAsync(CanonnHelper.WhitelistUrl,
                jsonResponse =>
                {
                    try
                    {
                        JArray whitelistItems = jsonResponse.JSONParse().Array();

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
            for (int e = 0; e < _globalWhitelist.Events.Count; e++)
            {
                if (_globalWhitelist.Events[e].Type.Equals(typeValue, StringComparison.InvariantCultureIgnoreCase))
                {
                    existingEvent = _globalWhitelist.Events[e];
                    break;
                }
            }

            if (existingEvent == null)
            {
                existingEvent = new WhitelistEvent { Type = typeValue };
                _globalWhitelist.Events.Add(existingEvent);
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


        private WhitelistData _globalWhitelist = new WhitelistData();

        private bool IsEventValid(string eventName, string jsonString)
        {
            JObject jsonObject = string.IsNullOrEmpty(jsonString) ? null : jsonString.JSONParse().Object();

            WhitelistEvent eventNode = _globalWhitelist.Events.FirstOrDefault(e =>
                e.Type.Equals(eventName, StringComparison.InvariantCultureIgnoreCase));

            if (eventNode != null && IsDataBlockValid(eventNode, jsonObject))
                return true;

            if (eventNode == null)
            {
                eventNode = _globalWhitelist.Events.FirstOrDefault(e =>
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
            for (int i = 0; i < _globalWhitelist.Events.Count; i++)
            {
                WhitelistEvent we = _globalWhitelist.Events[i];
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
        private void ProcessEvent(JournalEntry je)
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
                    case "FSSSignalDiscovered":
                        ProcessFSSSignalDiscovered(eventData);
                        break;
                    case "CodexEntry":
                        ProcessCodex(eventData);
                        break;
                    default:
                        // Unsupported event
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
                systemData.SystemAddress = eventData["SystemAddress"]?.ToObject<long>() ?? 0;

                if (eventData["StarPos"] != null)
                {
                    systemData.X = CanonnHelper.GetValueOrDefault(eventData["StarPos"][0] ?? null, 0.0);
                    systemData.Y = CanonnHelper.GetValueOrDefault(eventData["StarPos"][1] ?? null, 0.0);
                    systemData.Z = CanonnHelper.GetValueOrDefault(eventData["StarPos"][2] ?? null, 0.0);
                    systemData.HasCoordinate = true;
                }
            }
        }

        private void fetchScanData(JObject eventData, Body body) //Call this method only via a lock. Otherwise it could get bad.
        {
            if (body.BodyName?.Contains("Belt") == true)
                return;

            if (body.ScanData == null)
            {
                body.ScanData = new ScanData
                {
                    //Primitives
                    BodyID = body.BodyID,
                    IsPlanet = eventData.Contains("PlanetClass"),
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
                body.ScanData.IsPlanet = eventData.Contains("PlanetClass");
                body.ScanData.ScanType = eventData["ScanType"]?.Value?.ToString();

                //List<JObject> : AddRange(field.OfType<JObject>().Where(s => !body.ScanData.field.Any(existing => JToken.DeepEquals(existing, s)))) --> Should work?
                if (eventData["Signals"] is JArray signals)
                {
                    if (body.ScanData.Signals == null)
                        body.ScanData.Signals = new List<JObject>();
                    body.ScanData.Signals.AddRange(signals.OfType<JObject>().Where(s => !body.ScanData.Signals.Any(existing => JToken.DeepEquals(existing, s))));
                }

                if (eventData["Rings"] is JArray rings)
                {
                    if (body.ScanData.Rings == null)
                        body.ScanData.Rings = new List<JObject>();
                    body.ScanData.Rings.AddRange(rings.OfType<JObject>().Where(r => !body.ScanData.Rings.Any(existing => JToken.DeepEquals(existing, r))));
                }

                if (eventData["Organics"] is JArray organics) //I think that's unnecessary. We'll leave it anyway.
                {
                    if (body.ScanData.Organics == null)
                        body.ScanData.Organics = new List<JObject>();
                    body.ScanData.Organics.AddRange(organics.OfType<JObject>().Where(o => !body.ScanData.Organics.Any(existing => JToken.DeepEquals(existing, o))));
                }

                if (eventData["Genuses"] is JArray genuses)
                {
                    if (body.ScanData.Genuses == null)
                        body.ScanData.Genuses = new List<JObject>();
                    body.ScanData.Genuses.AddRange(genuses.OfType<JObject>().Where(g => !body.ScanData.Genuses.Any(existing => JToken.DeepEquals(existing, g))));
                }

                if (eventData["SurfaceFeatures"] is JArray surfaceFeatures)
                {
                    if (body.ScanData.SurfaceFeatures == null)
                        body.ScanData.SurfaceFeatures = new List<JObject>();
                    body.ScanData.SurfaceFeatures.AddRange(surfaceFeatures.OfType<JObject>().Where(sf => !body.ScanData.SurfaceFeatures.Any(existing => JToken.DeepEquals(existing, sf))));
                }

            }
        }

        private void ProcessScan(JObject eventData) //We can use this for most scan events.
        {
            if (systemData == null)
                systemData = new SystemData(); //Enforces encapsulation and creates a new SystemData instance internally, disregarding any parameters.

            lock (_lockSystemData)
            {
                if (systemData.Bodys == null)
                {
                    systemData.Bodys = new Dictionary<int, Body>();
                }
      
                int bodyId = CanonnHelper.GetValueOrDefault(eventData["BodyID"]?? null, -1);

                if (bodyId == -1)
                    return;

                Body body;

                if (!systemData.Bodys.ContainsKey(bodyId))
                {
                    body = new Body
                    {
                        BodyID = bodyId,
                        BodyName = eventData["BodyName"]?.Value?.ToString(),
                    };
                    systemData.Bodys[bodyId] = body;
                }
                else
                    body = systemData.Bodys[bodyId];

                fetchScanData(eventData, body);
            }
        }

        private void ProcessFSSDiscoveryScan(JObject eventData)
        {
            if (systemData == null)
                systemData = new SystemData(); //Enforces encapsulation and creates a new SystemData instance internally, disregarding any parameters.

            lock (_lockSystemData)
            {
                systemData.FSSTotalBodies = eventData["BodyCount"]?.ToObject<int>() ?? 0;
                systemData.FSSTotalNonBodies = eventData["NonBodyCount"]?.ToObject<int>() ?? 0;
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
                systemData = new SystemData(); //Enforces encapsulation and creates a new SystemData instance internally, disregarding any parameters.

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
                    };
                    systemData.Bodys[bodyId] = body;
                }
                else
                    body = systemData.Bodys[bodyId];

                if (eventData["Genus"] is JToken genus)
                {
                    JObject newOrganic = new JObject
                    {
                        ["Genus"] = genus
                    };

                    body.ScanData.Organics.Add(newOrganic.OfType<JObject>().FirstOrDefault(sf => !body.ScanData.Organics.Any(existing => JToken.DeepEquals(existing, sf))));
                }
            }
        }

        private void ProcessFSSSignalDiscovered(JObject eventData) //wip
        {
            if (systemData == null)
                systemData = new SystemData(); //Enforces encapsulation and creates a new SystemData instance internally, disregarding any parameters.

            lock (_lockSystemData)
            {
                if (eventData["SignalName"] is JToken signal)
                {
                    JObject newSignal = new JObject
                    {
                        ["SignalName"] = signal
                    };

                    systemData.FSSSignalList.Add(newSignal.OfType<JObject>().FirstOrDefault(sf => !systemData.FSSSignalList.Any(existing => JToken.DeepEquals(existing, sf))));
                }
            }
        }

        private void ProcessCodex(JObject eventData) //wip
        {
            if (systemData == null)
                systemData = new SystemData(); //Enforces encapsulation and creates a new SystemData instance internally, disregarding any parameters.

            lock (_lockSystemData)
            {
                if (eventData["Name"] is JToken codex)
                {
                    JObject newCodex = new JObject
                    {
                        ["Name"] = codex
                    };

                    systemData.CodexEntryList.Add(newCodex.OfType<JObject>().FirstOrDefault(sf => !systemData.CodexEntryList.Any(existing => JToken.DeepEquals(existing, sf))));
                }
            }
        }


        #endregion

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

                    systemData.FSSTotalBodies = root["FSSTotalBodies"]?.ToObject<int>() ?? 0;
                    systemData.FSSTotalNonBodies = root["FSSTotalNonBodies"]?.ToObject<int>() ?? 0;

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
            systemData.X = CanonnHelper.GetValueOrDefault(system["x"]?? null,0.0);
            systemData.Y = CanonnHelper.GetValueOrDefault(system["y"] ?? null,0.0);
            systemData.Z = CanonnHelper.GetValueOrDefault(system["z"] ?? null,0.0);
            systemData.HasCoordinate = system["HasCoordinate"]?.ToObject<bool>() ?? false;
            systemData.SystemAddress = CanonnHelper.GetValueOrDefault(system["SystemAddress"] ?? null, 0l);
        }

        private void ProcessCallbackStarNodes(JObject starNodes, int? parentBodyId = null) //wip
        {
            if (starNodes == null)
                return;

            foreach (KeyValuePair<string, JToken> property in starNodes)
            {
                string nodeKey = property.Key;
                JObject starNode = property.Value as JObject;

                if (starNode == null || starNode.Count == 0)
                {
                    continue;
                }

                if (starNode["NodeType"].Value?.ToString() == "belt")
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
                    NodeType = starNode["NodeType"].Value?.ToString(),
                    BodyName = starNode["BodyDesignator"].Value?.ToString(),
                };


                JObject scanDataNode = starNode["ScanData"] as JObject;
                if (scanDataNode != null)
                {
                    body.ScanData = new ScanData
                    {
                        //Primitives
                        ScanType = scanDataNode["ScanType"].Value?.ToString(),
                        BodyID = CanonnHelper.GetValueOrDefault(scanDataNode["ScanType"]?? null, -1),
                        IsPlanet = scanDataNode["IsPlanet"]?.ToObject<bool>() ?? false,
                        HasRings = scanDataNode["HasRings"]?.ToObject<bool>() ?? false,

                        //List<JObject>    
                        Signals = CanonnHelper.GetJObjectList(scanDataNode, "Signals"),
                        SurfaceFeatures = CanonnHelper.GetJObjectList(scanDataNode, "SurfaceFeatures"),
                        Rings = CanonnHelper.GetJObjectList(scanDataNode, "Rings"),
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

        #region ProcessData
        public enum RequestTag
        {
            OnStart
        }

        public void DataResult(object requestTag, string data)
        {
            if (CanonnHelper.InstanceCount > 1)
                return;

            try
            {
                if (requestTag == null || data == null)
                    return;

                JObject o = data.JSONParse().Object();
                if (!(requestTag is RequestTag || requestTag is JObject))
                    return;

                dataHandler.StartTaskAsync(() =>
                {
                    if (requestTag is RequestTag rt && rt.Equals(RequestTag.OnStart))
                    {
                        lock (_lockSystemData) ProcessCallbackSystem(o);
                        draw();
                    }
                    else if (requestTag is JObject jb)
                    {
                        string evt = jb["event"]?.Value?.ToString();
                        if (evt == "Location" || evt == "FSDJump")
                        {
                            lock (_lockSystemData)
                            {
                                ProcessCallbackSystem(o);
                                if (systemData == null) ProcessNewSystem(jb);
                            }
                            draw();
                        }
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

        #region IEDDPanelExtension
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

            NotifyMainFields("Start Up...");

            try
            {
                if(CanonnHelper.InstanceCount > 0)
                {
                    NotifyMainFields("Aborted");
                    CanonnHelper.InstanceCount++;
                    gridData.SelectedIndex = gridData.TabCount -1;
                    DebugLog.AppendText("Only one Canonn Panel instance can be active.");
                    this.Enabled = false;
                    return;
                }
                else
                CanonnHelper.InstanceCount++;

                InitializeWhitelist();
                InitializePatrols();

                JournalEntry je = new EDDDLLInterfaces.EDDDLLIF.JournalEntry();

                if (DLLCallBack.RequestHistory(1, false, out je) == true)
                {
                    DLLCallBack.RequestScanData(RequestTag.OnStart, this, "", true);
                }
                else
                {
                    dataHandler.StartTaskAsync(
                    () =>
                    {
                        while (!historyset)
                        {
                            Task.Delay(250).Wait();
                        }
                        BeginInvoke((MethodInvoker)delegate
                        {
                            DLLCallBack.RequestScanData(RequestTag.OnStart, this, "", true);
                        });
                        return;
                    },
                    ex =>
                    {
                        Console.Error.WriteLine($"EDDCanonn: Unexpected error in \"Wait for History\" Thread: {ex.Message}");
                    },
                        "InitialiseSystem"
                    );
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"EDDCanonn: Error during Initialise: {ex.Message}");
            }
        }

        public void NewFilteredJournal(JournalEntry je) //wip
        {
            if (CanonnHelper.InstanceCount > 1)
                return;

            try
            {
                dataHandler.StartTaskAsync(
                () =>
                {
                    if (IsEventValid(je.eventid, je.json))
                    {
                        //Payload.BuildPayload(je, getStatusJson());
                    }

                    if (je.eventid.Equals("FSDJump")) //Prepare data for the next system. Include ‘FSDJump’ event as requestTag in case the System is unknown.
                    {
                        resetSystemData();
                        BeginInvoke((MethodInvoker)delegate
                        {
                            DLLCallBack.RequestScanData(je.json.JSONParseObject(), this, je.systemname, true);
                        });
                    }
                    else if (je.eventid.Equals("Location")) //Prepare data for the next system. Include ‘Location’ event as requestTag in case of a crash. 
                    {
                        resetSystemData();
                        BeginInvoke((MethodInvoker)delegate
                        {
                            DLLCallBack.RequestScanData(je.json.JSONParseObject(), this, je.systemname, true);
                        });
                    }
                    else
                        ProcessEvent(je);
                    draw(); //wip
                },
                ex =>
                {
                    Console.Error.WriteLine($"EDDCanonn: Error processing JournalEntry: {ex.Message}");
                },
                    "NewFilteredJournal: " + je.eventid
                );
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"EDDCanonn: Unexpected error in NewFilteredJournal: {ex.Message}");
            }
        }

        private readonly object _lockStatusJson = new object();
        private JObject _statusJson;
        public void NewUIEvent(string jsonui)
        {
            if (CanonnHelper.InstanceCount > 1)
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

        private JObject getStatusJson() //Return a copy.
        {
            lock (_lockStatusJson)
                return new JObject(_statusJson);
        }

        public void NewTarget(Tuple<string, double, double, double> target)
        {
            if (CanonnHelper.InstanceCount > 1)
                return;
            //wip
        }

        public void NewUnfilteredJournal(JournalEntry je)
        {
            if (CanonnHelper.InstanceCount > 1)
                return;
            //wip
        }
        private bool historyset = false;
        public void HistoryChange(int count, string commander, bool beta, bool legacy)
        {
            if (CanonnHelper.InstanceCount > 1)
                return;
            Console.WriteLine("Hallo");
            if (!historyset)
                historyset = true;
        }

        public void InitialDisplay()
        {
            if (CanonnHelper.InstanceCount > 1)
                return;

            //wip
        }

        public void LoadLayout()
        {
            PanelCallBack.LoadGridLayout(dataGridPatrol);
        }

        public void ScreenShotCaptured(string file, Size s)
        {
            if (CanonnHelper.InstanceCount > 1)
                return;

            throw new NotImplementedException();
        }

        public void SetTransparency(bool ison, Color curcol)
        {
            if (CanonnHelper.InstanceCount > 1)
                return;

            this.BackColor = curcol;
        }

        public void ThemeChanged(string themeasjson)
        {
            if (CanonnHelper.InstanceCount > 1)
                return;

            //wip
        }

        public void TransparencyModeChanged(bool on)
        {
            if (CanonnHelper.InstanceCount > 1)
                return;

            //wip
        }

        void IEDDPanelExtension.CursorChanged(JournalEntry je)
        {
            if (CanonnHelper.InstanceCount > 1)
                return;

            //wip
        }

        public void ControlTextVisibleChange(bool on)
        {
            if (CanonnHelper.InstanceCount > 1)
                return;

            throw new NotImplementedException();
        }

        public string HelpKeyOrAddress()
        {
            throw new NotImplementedException();
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

        private void LogSystem_Click(object sender, EventArgs e)//wip
        {
            DebugLog.AppendText(systemData?.ToString() + "\n");

        }
        #endregion

        private void draw()
        {
            SystemData system = deepCopySystemData();

            if (system == null)
                return;

            Invoke((MethodInvoker)delegate
            {
                textBoxSystem.Clear();
                textBoxSystem.AppendText(system.Name);
                textBoxBodyCount.Clear();
                textBoxBodyCount.AppendText(system.Bodys?.Count + " / " + system.FSSTotalBodies);
            });
        }

        private void NotifyMainFields(String arg)
        {
            Invoke((MethodInvoker)delegate
            {
                textBoxSystem.Text = arg;
                toolStripRange.Text = arg;
                toolStripPatrol.Text = arg;
                textBoxNews.Text = arg;
            });
        }
    }
}
