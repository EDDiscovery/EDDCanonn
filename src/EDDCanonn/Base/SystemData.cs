
using System.Collections.Generic;
using QuickJSON;

namespace EDDCanonn.Base
{
    public class SystemData
    {
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
        public int BodyID { get; set; }
        public string NodeType { get; set; }
        public string BodyName { get; set; }
        public ScanData ScanData { get; set; }
    }

    public class ScanData
    {
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
