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
            InitializePatrols();
        }

        #region Patrol
        private Patrols patrols = new Patrols();
        private void InitializePatrols()
        {
            try
            {
                dataHandler.StartTaskAsync(
                () =>
                {                              
                    List<Dictionary<string, string>> records = CanonnHelper.ParseTsv(dataHandler.FetchData(CanonnHelper.PatrolUrl));
                    List<Task> _tasks = new List<Task>();

                    foreach (Dictionary<string, string> record in records)
                    {
                        try
                        {
                            string description = record.TryGetValue("Description", out string descriptionValue) 
                                ? descriptionValue 
                                : "uncategorized";

                            bool enabled = record.TryGetValue("Enabled", out string enabledValue) 
                                && enabledValue == "Y";

                            string type = record.TryGetValue("Type", out string typeValue) 
                                ? typeValue 
                                : string.Empty;

                            string url = record.TryGetValue("Url", out string urlValue) 
                                ? urlValue 
                                : string.Empty;

                            if (!enabled)
                                continue;                       

                            if (type.Equals("tsv"))
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
                                    "InitializePatrol: " + description
                                    ));
                            }
                            else if (type.Equals("json"))
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
                                    "InitializePatrol: " + description
                                    ));
                            }

                        }
                        catch (Exception ex)
                        {
                            Console.Error.WriteLine($"EDDCanonn: Error processing patrol record: {ex.Message}");
                        }
                    }

                    Task.WaitAll(_tasks.ToArray()); // wip
                },
                ex =>
                {
                    Console.Error.WriteLine($"EDDCanonn: Error Initialize Patrols HeadThread: {ex.Message}");
                },
                "InitializePatrol - HeadThread"
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
                KdTree<float, Patrol> kdT = new KdTree<float, Patrol>(3, new FloatMath());
                List<Dictionary<string, string>> records = CanonnHelper.ParseTsv(dataHandler.FetchData(url));
                foreach (Dictionary<string, string> record in records)
                {
                    try
                    {
                        string patrolType = record.TryGetValue("Patrol", out string patrolValue) ? patrolValue :
                                            record.TryGetValue("Type", out string typeValue) ? typeValue : string.Empty;
                        long id64 = long.TryParse(record.TryGetValue("Id64", out string id64Value) ? id64Value : string.Empty, out long parsedId64) ? parsedId64 : -1;
                        float x = float.TryParse(record.TryGetValue("X", out string xValue) ? xValue : string.Empty, out float parsedX) ? parsedX : 0.0f;
                        float y = float.TryParse(record.TryGetValue("Y", out string yValue) ? yValue : string.Empty, out float parsedY) ? parsedY : 0.0f;
                        float z = float.TryParse(record.TryGetValue("Z", out string zValue) ? zValue : string.Empty, out float parsedZ) ? parsedZ : 0.0f;
                        string instructions = record.TryGetValue("Instructions", out string instructionsValue) ? instructionsValue : "none";
                        string urlp = record.TryGetValue("Url", out string urlValue) ? urlValue : string.Empty;
                        Patrol patrol = new Patrol(x, y, z, instructions, urlp, id64, patrolType);
                        kdT.Add(new float[] { patrol.X, patrol.Y, patrol.Z }, patrol);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"EDDCanonn: Error processing patrol record: {ex.Message}");
                    }
                }
                patrols.Add(category, kdT);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"EDDCanonn: Error in CreateFromTSV for category {category}: {ex.Message}");
            }
        }


        private void CreateFromJson(string url, string category)
        {

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

            string bodyName = rawEvent["BodyName"].Value?.ToString();
            if (!string.IsNullOrEmpty(bodyName))
                gameState["bodyName"] = bodyName;
            else if (!string.IsNullOrEmpty(je.bodyname) && je.bodyname != "Unknown")
                gameState["bodyName"] = je.bodyname;

            string stationName = rawEvent["StationName"].Value?.ToString();
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
                    systemData.X = eventData["StarPos"][0]?.ToObject<double>() ?? 0.0;
                    systemData.Y = eventData["StarPos"][1]?.ToObject<double>() ?? 0.0;
                    systemData.Z = eventData["StarPos"][2]?.ToObject<double>() ?? 0.0;
                    systemData.HasCoordinate = true;
                }
            }
        }

        private void fetchScanData(JObject eventData, Body body) //Call this method only via a lock. Otherwise it could get bad.
        {
            if (body.ScanData == null)
            {
                body.ScanData = new ScanData
                {
                    //Primitives
                    BodyID = body.BodyID,
                    IsPlanet = eventData.Contains("PlanetClass"),
                    ScanType = eventData["ScanType"].Value?.ToString(),

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
                body.ScanData.ScanType = eventData["ScanType"].Value?.ToString();

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

                if (eventData["Organics"] is JArray organics)
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

        private void ProcessScan(JObject eventData)
        {
            if (systemData == null)
                systemData = new SystemData(); //Enforces encapsulation and creates a new SystemData instance internally, disregarding any parameters.

            lock (_lockSystemData)
            {
                if (systemData.Bodys == null)
                {
                    systemData.Bodys = new Dictionary<int, Body>();
                }

                if (eventData["BodyName"]?.Value?.ToString()?.Contains("Belt") == true)
                    return;

                int bodyId = eventData["BodyID"]?.ToObject<int>() ?? -1;

                if (bodyId == -1)
                    return;

                Body body;

                if (!systemData.Bodys.ContainsKey(bodyId))
                {
                    body = new Body
                    {
                        BodyID = bodyId,
                        BodyName = eventData["BodyName"].Value?.ToString(),
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

        private void ProcessFSSBodySignals(JObject eventData)//wip
        {
            lock (_lockSystemData)
            {

            }
        }

        private void ProcessFSSSignalDiscovered(JObject eventData)//wip
        {
            lock (_lockSystemData)
            {

            }
        }

        private void ProcessSAASignalsFound(JObject eventData)//wip
        {
            lock (_lockSystemData)
            {

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

                if (starNode == null || starNode.Count == 0)
                {
                    continue;
                }

                //  if (!(starNode["NodeType"].Value?.ToString() == "star" || starNode["NodeType"].Value?.ToString() == "body")){
                //       continue;
                //   }

                if (starNode["NodeType"].Value?.ToString() == "belt")
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
                        BodyID = scanDataNode["BodyID"]?.ToObject<int>() ?? 0,
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

        public void DataResult(object requesttag, string data)
        {
            try
            {
                if (requesttag == null || data == null)
                    throw new ArgumentNullException("requesttag or data cannot be null.");

                JObject o = data.JSONParse().Object();

                if (!(requesttag is RequestTag || requesttag is JObject))
                {
                    return;
                }
                else
                {
                    dataHandler.StartTaskAsync(
                    () =>
                    {
                        if (requesttag is RequestTag rt)
                        {
                            if (rt.Equals(RequestTag.OnStart))
                            {
                                lock (_lockSystemData)
                                    ProcessCallbackSystem(o);
                                draw();
                            }
                        }
                        else
                        {
                            if (requesttag is JObject jb)
                            {
                                if (jb["event"].Value?.ToString() == "Location")
                                {
                                    lock (_lockSystemData)
                                    {
                                        ProcessCallbackSystem(o);
                                        if (systemData == null) //If the game previously crashed during the jump and no system data was previously saved by EDD.
                                            ProcessNewSystem(jb);
                                    }
                                    draw();
                                }
                                else if (jb["event"].Value?.ToString() == "FSDJump")
                                {
                                    lock (_lockSystemData)
                                    {
                                        ProcessCallbackSystem(o);
                                        if (systemData == null)
                                            ProcessNewSystem(jb);
                                    }
                                    draw();
                                }
                                else
                                    return;
                            }
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
                    {

                    }

                    if (je.eventid.Equals("FSDJump")) //Prepare data for the next system.
                    {
                        resetSystemData();
                        Invoke((MethodInvoker)delegate
                        {
                            DLLCallBack.RequestScanData(je.json.JSONParseObject(), this, je.systemname, true);
                        });
                    }
                    else if (je.eventid.Equals("Location")) //Prepare data for the next system. Include ‘Location’ event as requestTag in case of a crash. 
                    {
                        resetSystemData();
                        Invoke((MethodInvoker)delegate
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
            DLLCallBack.RequestScanData(RequestTag.OnStart, this, "", true);
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
            DebugLog.AppendText(systemData?.ToString() + "\n");
        }


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
    }
}
