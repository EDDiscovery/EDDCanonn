using EDDCanonnPanel.Base;
using EDDCanonnPanel.Utility;
using KdTree;
using KdTree.Math;
using QuickJSON;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static EDDDLLInterfaces.EDDDLLIF;

namespace EDDCanonnPanel
{
    partial class EDDCanonnUserControl 
    {
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

            return body;
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
                CanonnEDDClass.GetStatusJson()["Pos"]["Latitude"].Double(DataUtil.PositionFallback).ToString(),
                CanonnEDDClass.GetStatusJson()["Pos"]["Longitude"].Double(DataUtil.PositionFallback).ToString(),
                true
            );

            body.ScanData.SystemPois.Add(poi);

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

        private void ProcessCodex(JObject eventData)
        {
            string category = eventData["SubCategory"].StrNull();
            if (category == null)
                return;

            if (category.Equals("$Codex_SubCategory_Organic_Structures;")) //wip
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
                    eventData["Latitude"].Double(DataUtil.PositionFallback).ToString(),
                    eventData["Longitude"].Double(DataUtil.PositionFallback).ToString(),
                    true
                );

                body.ScanData.SystemPois.Add(poi);

            }
            if (category.Equals("$Codex_SubCategory_Geology_and_Anomalies;")) //wip
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


    }
}
