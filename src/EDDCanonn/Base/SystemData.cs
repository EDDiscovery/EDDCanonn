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
using System.Collections.Generic;
using System.Linq;
using QuickJSON;

namespace EDDCanonnPanel.Base
{
    public class SystemData
    {
       
        public SystemData() { }
        public SystemData(SystemData data)
        { //deep copy
            Name = data.Name;
            X = data.X;
            Y = data.Y;
            Z = data.Z;

            SystemAddress = data.SystemAddress;
            BodyCount = data.BodyCount;

            if (data.Bodys != null)
            {
                Bodys = new SortedDictionary<int, Body>();
                foreach (KeyValuePair<int,Body> kvp in data.Bodys)
                {
                    Bodys[kvp.Key] = new Body(kvp.Value);
                }
            }

            FSSSignalList = data.FSSSignalList?.Select(j => new JObject(j)).ToList() ?? new List<JObject>();
            CodexEntryList = data.CodexEntryList?.Select(j => new JObject(j)).ToList() ?? new List<JObject>();
        }

        // System
        public string Name { get; set; } = null;
        public double X { get; set; } = 0.0;
        public double Y { get; set; } = 0.0;
        public double Z { get; set; } = 0.0;
        public long SystemAddress { get; set; } = 0;
        public SortedDictionary<int, Body> Bodys { get; set; } = null;

        //Global
        public int BodyCount { get; set; } = -1;
        public List<JObject> FSSSignalList { get; set; } = null;
        public List<JObject> CodexEntryList { get; set; } = null;

        public Body GetBodyByName(string bodyName)
        {
            return Bodys?.Values.FirstOrDefault(body =>
                !string.IsNullOrEmpty(body.BodyName) &&
                body.BodyName.Equals(bodyName, StringComparison.OrdinalIgnoreCase));
        }

        public int CountBodysFilteredByNodeType(string[] include)
        {
            if (Bodys == null)
                return 0;

            return (include == null || include.Length == 0)
                ? Bodys.Count
                : Bodys.Values.Count(body =>
                    body.NodeType != null && include.Any(phrase =>
                        body.NodeType.Equals(phrase, StringComparison.OrdinalIgnoreCase)));
        }

        public override string ToString()
        {
            string result = $"System Name: {Name}\n" +
                            $"Coordinates: ({X}, {Y}, {Z})\n" +
                            $"System Address: {SystemAddress}\n" +
                            $"FSS Total Bodies: {BodyCount}\n\n";

            result += "FSS Signals:\n";
            if (FSSSignalList != null && FSSSignalList.Count > 0)
            {
                foreach (var signal in FSSSignalList)
                {
                    result += $"  {signal.ToString()}\n";
                }
            }
            else
            {
                result += "  None\n";
            }

            result += "\nCodex Entries:\n";
            if (CodexEntryList != null && CodexEntryList.Count > 0)
            {
                foreach (var entry in CodexEntryList)
                {
                    result += $"  {entry.ToString()}\n";
                }
            }
            else
            {
                result += "  None\n";
            }

            result += "\nBodies:\n";
            if (Bodys != null && Bodys.Count > 0)
            {
                foreach (var bodyEntry in Bodys)
                {
                    var body = bodyEntry.Value;
                    result += $"    ID: {body.BodyID}\n" +
                              $"    Body Name: {body.BodyName}\n" +
                              $"    IsMapped: {body.IsMapped}\n" +
                              $"    NodeType: {body.NodeType}\n";

                    if (body.ScanData != null)
                    {
                        result += "    Scan Data:\n" +
                                  $"      Body ID: {body.ScanData.BodyID}\n";

                        result += AddListToString("Rings", body.ScanData.Rings);
                        result += AddListToString("Signals", body.ScanData.Signals);
                        result += AddListToString("Organics", body.ScanData.Organics);
                        result += AddListToString("Surface Features", body.ScanData.SurfaceFeatures);
                        result += AddListToString("Genuses", body.ScanData.Genuses);
                    }
                }
            }
            else
            {
                result += "  None\n";
            }

            return result;
        }

        private string AddListToString(string name, List<JObject> list)
        {
            string result = $"      {name}:\n";
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    result += $"        {item.ToString()}\n";
                }
            }
            else
            {
                result += "        None\n";
            }
            return result;
        }

    }

    public class Body
    {
        public Body() { }
        public Body(Body body) {
            BodyID = body.BodyID;
            BodyName = body.BodyName;
            NodeType = body.NodeType;
            IsMapped = body.IsMapped;
            if (body.ScanData != null) 
                ScanData = new ScanData(body.ScanData);
        }

        public int BodyID { get; set; } = 0;
        public string BodyName { get; set; } = null;
        public string NodeType { get; set; } = null;
        public bool IsMapped { get; set; } = false;
        public ScanData ScanData { get; set; } = null;
    }

    public class ScanData
    {

        public ScanData() { }

        public ScanData(ScanData scanData)
        { //deep copy
            BodyID = scanData.BodyID;

            Rings = scanData.Rings?.Select(j => new JObject(j)).ToList() ?? new List<JObject>();
            Signals = scanData.Signals?.Select(j => new JObject(j)).ToList() ?? new List<JObject>();
            Organics = scanData.Organics?.Select(j => new JObject(j)).ToList() ?? new List<JObject>();
            SurfaceFeatures = scanData.SurfaceFeatures?.Select(j => new JObject(j)).ToList() ?? new List<JObject>();
            Genuses = scanData.Genuses?.Select(j => new JObject(j)).ToList() ?? new List<JObject>();
        }

        public int BodyID { get; set; } = 0;
        public List<JObject> Rings { get; set; } = null;
        public List<JObject> Signals { get; set; } = null;
        public List<JObject> Organics { get; set; } = null;
        public List<JObject> SurfaceFeatures { get; set; } = null;
        public List<JObject> Genuses { get; set; } = null;
    }

}
