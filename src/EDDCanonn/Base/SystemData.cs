
using System.Collections.Generic;
using System.Linq;
using QuickJSON;

namespace EDDCanonn.Base
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
            HasCoordinate = data.HasCoordinate;
            SystemAddress = data.SystemAddress;

            FSSTotalBodies = data.FSSTotalBodies;
            FSSTotalNonBodies = data.FSSTotalNonBodies;

            if (data.Bodys != null)
            {
                Bodys = new Dictionary<int, Body>();
                foreach (KeyValuePair<int,Body> kvp in data.Bodys)
                {
                    Bodys[kvp.Key] = new Body(kvp.Value);
                }
            }

            FSSSignalList = data.FSSSignalList?.Select(j => new JObject(j)).ToList() ?? new List<JObject>();
            CodexEntryList = data.CodexEntryList?.Select(j => new JObject(j)).ToList() ?? new List<JObject>();
        }

        // System
        public string Name { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public bool HasCoordinate { get; set; }
        public long SystemAddress { get; set; }
        public Dictionary<int,Body> Bodys { get; set; }

        //Global
        public int FSSTotalBodies { get; set; }
        public int FSSTotalNonBodies { get; set; }
        public List<JObject> FSSSignalList { get; set; }
        public List<JObject> CodexEntryList { get; set; }

        public override string ToString()
        {
            string result = $"System Name: {Name}\n" +
                            $"Coordinates: ({X}, {Y}, {Z})\n" +
                            $"Has Coordinate: {HasCoordinate}\n" +
                            $"System Address: {SystemAddress}\n" +
                            $"FSS Total Bodies: {FSSTotalBodies}\n" +
                            $"FSS Total Non-Bodies: {FSSTotalNonBodies}\n\n";

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
                    result += $"  Body ID: {body.BodyID}\n" +
                              $"    Node Type: {body.NodeType}\n" +
                              $"    Body Name: {body.BodyName}\n";

                    if (body.ScanData != null)
                    {
                        result += "    Scan Data:\n" +
                                  $"      Is Planet: {body.ScanData.IsPlanet}\n" +
                                  $"      Scan Type: {body.ScanData.ScanType}\n" +
                                  $"      Body ID: {body.ScanData.BodyID}\n" +
                                  $"      Has Rings: {body.ScanData.HasRings}\n";

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
            NodeType = body.NodeType;
            BodyName = body.BodyName;

            if(body.ScanData != null) 
                ScanData = new ScanData(body.ScanData);
        }

        public int BodyID { get; set; }
        public string NodeType { get; set; }
        public string BodyName { get; set; }
        public ScanData ScanData { get; set; }
    }

    public class ScanData
    {

        public ScanData() { }

        public ScanData(ScanData scanData)
        { //deep copy
            IsPlanet = scanData.IsPlanet;
            ScanType = scanData.ScanType;
            BodyID = scanData.BodyID;
            HasRings = scanData.HasRings;

            Rings = scanData.Rings?.Select(j => new JObject(j)).ToList() ?? new List<JObject>();
            Signals = scanData.Signals?.Select(j => new JObject(j)).ToList() ?? new List<JObject>();
            Organics = scanData.Organics?.Select(j => new JObject(j)).ToList() ?? new List<JObject>();
            SurfaceFeatures = scanData.SurfaceFeatures?.Select(j => new JObject(j)).ToList() ?? new List<JObject>();
            Genuses = scanData.Genuses?.Select(j => new JObject(j)).ToList() ?? new List<JObject>();
        }

        public bool IsPlanet { get; set; }
        public string ScanType { get; set; }
        public int BodyID { get; set; }
        public bool HasRings { get; set; }
        public List<JObject> Rings { get; set; }
        public List<JObject> Signals { get; set; }
        public List<JObject> Organics { get; set; }
        public List<JObject> SurfaceFeatures { get; set; }
        public List<JObject> Genuses { get; set; }
    }

}
