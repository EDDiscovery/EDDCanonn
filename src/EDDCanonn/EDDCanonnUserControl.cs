using System;
using static EDDDLLInterfaces.EDDDLLIF;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using QuickJSON;
using System.Linq;
using EDDCanonn.Base;
using System.Collections;

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
            Init();
        }

        private void Init()
        {
            dataHandler = new ActionDataHandler();
            InitializeWhitelist();
        }

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

        #region CanonnPayload
        public JObject BuildPayload(JournalEntry je)//wip
        {
            JObject gameState = new JObject();
            JObject rawEvent = je.json.JSONParse().Object();
            JObject status = getStatusJson();

            string clientVersion = "EDDCanonnClient v0.1";
            string platform = "PC";

            JObject payload = new JObject();

            payload["gameState"] = gameState;

            gameState["systemName"] = je.systemname;
            gameState["systemAddress"] = je.systemaddress;
            gameState["systemCoordinates"] = JArray.FromObject(new double[] { je.x, je.y, je.z });

            string bodyName = rawEvent["BodyName"]?.ToString();
            if (!string.IsNullOrEmpty(bodyName))
                gameState["bodyName"] = bodyName;
            else if (!string.IsNullOrEmpty(je.bodyname) && je.bodyname != "Unknown")
                gameState["bodyName"] = je.bodyname;

            string stationName = rawEvent["StationName"]?.ToString();
            if (!string.IsNullOrEmpty(stationName))
                gameState["station"] = stationName;
            else if (!string.IsNullOrEmpty(je.stationname) && je.stationname != "Unknown")
                gameState["station"] = je.stationname;

            if (status != null && status.Contains("Pos") && status["Pos"]["ValidPosition"]?.ToObject<bool>() == true)
            {
                if (status["Pos"]["Latitude"] != null)
                    gameState["latitude"] = status["Pos"]["Latitude"].ToObject<double>();

                if (status["Pos"]["Longitude"] != null)
                    gameState["longitude"] = status["Pos"]["Longitude"].ToObject<double>();
            }

            if (status != null)
            {
                if (status.Contains("Temperature") && status["Temperature"] != null && status["Temperature"].ToObject<double>() >= 0)
                    gameState["temperature"] = status["Temperature"].ToObject<double>();

                if (status.Contains("Gravity") && status["Gravity"] != null && status["Gravity"].ToObject<double>() >= 0)
                    gameState["gravity"] = status["Gravity"].ToObject<double>();
            }

            gameState["clientVersion"] = clientVersion;
            gameState["isBeta"] = je.beta;
            gameState["platform"] = platform;
            gameState["odyssey"] = je.odyssey;

            payload["rawEvent"] = rawEvent;
            payload["eventType"] = je.eventid;
            payload["cmdrName"] = je.cmdrname;

            return payload;
        }
        #endregion

        //This only affects the data structure for visual feedback.
        #region SystemData
        private readonly object _lockSystemData = new object();
        private SystemData _systemData; //Do not use this.
        private SystemData systemData //Encapsulation Enforcement | Is that what it's called?
        {
            get
            {
                if (_systemData == null)
                {
                    lock (_lockSystemData)
                    {
                        if (_systemData == null)
                        {
                            _systemData = new SystemData();
                        }
                    }
                }
                return _systemData;
            }
        }
        #endregion

        //This only affects the data structure for visual feedback.
        #region ProcessEvents
        private void ProcessEvent(JournalEntry je)
        {
            JObject eventData = je.json.JSONParse().Object();

            switch (je.eventid)
            {
                case "FSDJump":
                    ProcessFSDJump(eventData);
                    break;
                case "Scan":
                    ProcessScan(eventData);
                    break;
                case "FSSBodySignals":
                    ProcessFSSBodySignals(eventData);
                    break;
                case "FSSDiscoveryScan":
                    ProcessFSSDiscoveryScan(eventData);
                    break;
                case "FSSSignalDiscovered":
                    ProcessFSSSignalDiscovered(eventData);
                    break;
                case "SAASignalsFound":
                    ProcessSAASignalsFound(eventData);
                    break;
                default:
                    // Unsupported event
                    break;
            }
        }

        private void ProcessFSDJump(JObject eventData)
        {
            lock (_lockSystemData)
            {

            }
        }

        private void ProcessScan(JObject eventData)
        {
            lock (_lockSystemData)
            {

            }
        }

        private void ProcessFSSBodySignals(JObject eventData)
        {
            lock (_lockSystemData)
            {

            }
        }

        private void ProcessFSSDiscoveryScan(JObject eventData)
        {
            lock (_lockSystemData)
            {

            }
        }

        private void ProcessFSSSignalDiscovered(JObject eventData)
        {
            lock (_lockSystemData)
            {

            }
        }

        private void ProcessSAASignalsFound(JObject eventData)
        {
            lock (_lockSystemData)
            {

            }
        }

        private void ProcessRefresh(JournalEntry je)
        {
            lock (_lockSystemData)
                _systemData = null;
            DLLCallBack.RequestScanData(RequestTag.System, this, je.systemname, true);
        }
        #endregion

        #region ProcessCallbackSystem
        public void ProcessCallbackSystem(JObject root)
        {
            lock (_lockSystemData)
            {
                try
                {
                    if (root == null)
                        return;

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

                        if (root["FSSSignalList"] != null && root["FSSSignalList"] is JArray signals)
                        {
                            if (systemData.FSSSignalList == null)
                                systemData.FSSSignalList = new List<JObject>();
                            systemData.FSSSignalList.AddRange(signals.OfType<JObject>());
                        }

                        if (root["CodexEntryList"] != null && root["CodexEntryList"] is JArray codexEntries)
                        {
                            if (systemData.CodexEntryList == null)
                                systemData.CodexEntryList = new List<JObject>();
                            systemData.CodexEntryList.AddRange(codexEntries.OfType<JObject>());
                        }

                    systemData.FSSTotalBodies = root["FSSTotalBodies"]?.ToObject<int>() ?? 0;
                    systemData.FSSTotalNonBodies = root["FSSTotalNonBodies"]?.ToObject<int>() ?? 0;

                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message);
                }

            }
        }

        private void ProcessCallbackSystemData(JObject system)
        {
            // Extract and populate main system details
            systemData.Name = system["Name"]?.ToString();
            systemData.X = system["X"]?.ToObject<double>() ?? 0.0;
            systemData.Y = system["Y"]?.ToObject<double>() ?? 0.0;
            systemData.Z = system["Z"]?.ToObject<double>() ?? 0.0;
            systemData.HasCoordinate = system["HasCoordinate"]?.ToObject<bool>() ?? false;
            systemData.SystemAddress = system["SystemAddress"]?.ToObject<long>() ?? 0;
        }

        private void ProcessCallbackStarNodes(JObject starNodes, int? parentBodyId = null) //wip
        {
            if (starNodes == null)
                return;

            foreach (KeyValuePair<string, JToken> property in starNodes)
            {
                string nodeKey = property.Key;
                JObject starNode = property.Value as JObject;

                if (starNode == null)
                {
                    continue;
                }

                // Extract BodyID; skip processing if invalid
                int bodyId = starNode["BodyID"] != null ? starNode["BodyID"].ToObject<int>() : -1;
                if (bodyId == -1)
                {
                    continue;
                }

                if (systemData.Bodys == null)
                {
                    systemData.Bodys = new Dictionary<string, Body>();
                }


                if (systemData.Bodys.ContainsKey(bodyId.ToString()))
                {
                    continue;
                }

                Body body = new Body
                {
                    BodyID = bodyId,
                    NodeType = starNode["NodeType"]?.ToString(),
                    BodyDesignator = starNode["BodyDesignator"]?.ToString(),
                    OwnName = starNode["OwnName"]?.ToString(),
                    SystemBodyName = starNode["SystemBodyName"]?.ToString(),
                    Level = starNode["Level"] != null ? starNode["Level"].ToObject<int>() : 0
                };

                JObject scanDataNode = starNode["ScanData"] as JObject;
                if (scanDataNode != null)
                {
                    body.ScanData = new ScanData //wip
                    {
                        IsPlanet = scanDataNode["IsPlanet"] != null ? scanDataNode["IsPlanet"].ToObject<bool>() : false,
                        BodyDesignation = scanDataNode["BodyDesignation"]?.ToString(),
                        BodyDesignationOrName = scanDataNode["BodyDesignationOrName"]?.ToString(),
                        ScanType = scanDataNode["ScanType"]?.ToString(),
                        BodyName = scanDataNode["BodyName"]?.ToString(),
                        BodyID = scanDataNode["BodyID"] != null ? scanDataNode["BodyID"].ToObject<int>() : 0,
                        HasRings = scanDataNode["HasRings"] != null ? scanDataNode["HasRings"].ToObject<bool>() : false,
                        SurfaceFeatures = scanDataNode["SurfaceFeatures"] is JArray surfaceFeatures ? surfaceFeatures.OfType<JObject>().ToList() : new List<JObject>(),
                        Rings = scanDataNode["Rings"] is JArray rings ? rings.OfType<JObject>().ToList() : new List<JObject>(),
                        Signals = scanDataNode["Signals"] is JArray signals ? signals.OfType<JObject>().ToList() : new List<JObject>(),
                        Organics = scanDataNode["Organics"] is JArray organics ? organics.OfType<JObject>().ToList() : new List<JObject>(),
                        Genuses = scanDataNode["Genuses"] is JArray genuses ? genuses.OfType<JObject>().ToList() : new List<JObject>()
                    };
                }

                systemData.Bodys[bodyId.ToString()] = body;

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
            System
        }

        public void DataResult(object requesttag, string data)
        {
            try
            {
                if (requesttag == null || data == null)
                    throw new ArgumentNullException("requesttag or data cannot be null.");
                if (!requesttag.GetType().IsEnum)
                    throw new ArgumentException("requesttag must be of type enum.");

                JObject o = data.JSONParse().Object();

                if (!(requesttag is RequestTag))
                    return;
                else
                {
                    dataHandler.StartTaskAsync(
                    () =>
                    {
                        if (requesttag.Equals(RequestTag.System))
                        {
                            ProcessCallbackSystem(o);
                            DebugLog.Invoke((MethodInvoker)delegate //wip
                            {
                                DebugLog.AppendText(systemData.ToString() + Environment.NewLine);
                            });
                        }
                    },
                    ex =>
                    {
                        Console.Error.WriteLine($"EDDCanonn: Error processing Systemdata: {ex.Message}");
                    },
                    "DataResult"
                    );
                }
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
            dataHandler.Closing();
        }

        public void Initialise(EDDPanelCallbacks callbacks, int displayid, string themeasjson, string configuration)
        {
            DLLCallBack = EDDCanonnEDDClass.DLLCallBack;
            PanelCallBack = callbacks;
        }

        public void NewFilteredJournal(JournalEntry je) //wip
        {
            try
            {
                dataHandler.StartTaskAsync(
                () =>
                {
                    if (IsEventValid(je.eventid, je.json))
                        DebugLog.Invoke((MethodInvoker)delegate
                        {
                            DebugLog.AppendText(BuildPayload(je) + Environment.NewLine);
                            DebugLog.AppendText("" + Environment.NewLine);
                        });

                    if (je.eventid.Equals("StartJump") || je.eventid.Equals("Location"))
                        ProcessRefresh(je);
                    else
                        ProcessEvent(je);
                },
                ex =>
                {
                    Console.Error.WriteLine($"EDDCanonn: Error processing JournalEntry: {ex.Message}");
                },
                "NewFilteredJournal"
                );
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"EDDCanonn: Unexpected error in NewFilteredJournal: {ex.Message}");
            }
        }

        private readonly object _lockStatusJson = new object();
        private JObject statusJson;
        public void NewUIEvent(string jsonui)
        {
            JObject o = jsonui.JSONParse().Object();
            if (o == null)
                return;

            string type = o["EventTypeStr"].Str();
            if (string.IsNullOrEmpty(type))
                return;

            lock (_lockStatusJson)
                statusJson = o;
        }

        private JObject getStatusJson()
        {
            lock (_lockStatusJson)
                return new JObject(statusJson);
        }

        public void NewTarget(Tuple<string, double, double, double> target)
        {
            //wip
        }

        public void NewUnfilteredJournal(JournalEntry je)
        {
            //wip
        }

        public void HistoryChange(int count, string commander, bool beta, bool legacy)
        {
            //wip
        }

        public void InitialDisplay()
        {
            //wip
        }

        public void LoadLayout()
        {
            //wip
        }

        public void ScreenShotCaptured(string file, Size s)
        {
            throw new NotImplementedException();
        }

        public void SetTransparency(bool ison, Color curcol)
        {
            this.BackColor = curcol;
        }

        public void ThemeChanged(string themeasjson)
        {
            //wip
        }

        public void TransparencyModeChanged(bool on)
        {
            //wip
        }

        void IEDDPanelExtension.CursorChanged(JournalEntry je)
        {
            //wip
        }

        public void ControlTextVisibleChange(bool on)
        {
            throw new NotImplementedException();
        }

        public string HelpKeyOrAddress()
        {
            throw new NotImplementedException();
        }


        #endregion

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

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

        private void EDDCanonnUserControl_Load(object sender, EventArgs e)
        {

        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStripComboBox3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_2(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)//wip
        {
            DLLCallBack.RequestScanData(RequestTag.System, this, "Bloomee IR-W f1-344", true);
        }
    }
}
