using EDDCanonnPanel.Base;
using EDDCanonnPanel.Utility;
using QuickJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDCanonnPanel
{
    partial class EDDCanonnUserControl 
    {
        /*********************************************************************************************************
         * CRITICAL NOTICE:
         * ProcessSpanshDump and the underlying methods should also only be called via the '_lockSystemData' lock.
         * DO NOT IGNORE THIS.
         *********************************************************************************************************/

        // From request to EDD for spansh data, process return data
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
                {
                    DataGridNotifications[NType.spanshIncorrect] = CanonnUtil.CreateDataGridViewRow(dataGridViewData, new object[] {
                            NTypeDescription(NType.spanshIncorrect),
                            Properties.Resources.spansh });
                }
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

        private void ProcessCanonnBiostats(long i64) //WIP: Bio + Geo
        {
            JObject o = dataHandler.FetchData(LinkUtil.BioStats + i64).response.JSONParse().Object();
            if (o?["system"]?["bodies"] == null || o["system"]["bodies"].Count == 0)
                return;

            CanonnLogging.Instance.Log(Environment.NewLine + "=== Canonn DataResult ===" + Environment.NewLine + o.ToString(true, "  ") ?? "none" + Environment.NewLine);

            foreach (JObject node in o["system"]["bodies"])
            {
                if (node == null || node.Count == 0)
                    continue;

                int bodyId = node["bodyId"].Int(-1);
                if (bodyId == -1)
                    continue;

                Base.Body body = systemData?.Bodys?[bodyId];
                if (body?.ScanData == null)
                    continue;

                List<JObject> biology = CanonnUtil.GetJObjectList(node["signals"] as JObject, "biology", "Bio");
                if (biology?.Count > 0)
                {
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

                List<JObject> geology = CanonnUtil.GetJObjectList(node["signals"] as JObject, "geology", "Geo");
                if (geology?.Count > 0)
                {
                    body.ScanData.Geologic = geology
                        .Select(g =>
                        {
                            string variantLocalised = g["Geo"].StrNull();
                            CodexEntry entry = CodexDatabase?.GetByLocalisedName(variantLocalised);

                            return new JObject
                            {
                                ["Variant"] = entry?.Name,
                                ["Variant_Localised"] = variantLocalised
                            };
                        })
                        .ToList();
                }
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


    }
}
