
using System;
using System.Collections.Generic;
using System.Linq;
using QuickJSON;
namespace EDDCanonn
{
    public static class CanonnHelper
    {
        public static readonly string WhitelistUrl = "https://us-central1-canonn-api-236217.cloudfunctions.net/postEventWhitelist";
        public static readonly string EventPushUrl = "https://us-central1-canonn-api-236217.cloudfunctions.net/postEvent";
        public static readonly string PatrolUrl = "https://docs.google.com/spreadsheets/d/e/2PACX-1vQsi1Vbfx4Sk2msNYiqo0PVnW3VHSrvvtIRkjT-JvH_oG9fP67TARWX2jIjehFHKLwh4VXdSh0atk3J/pub?gid=0&single=true&output=tsv";
   
        public static List<JObject> GetJObjectList(JObject source, string key)
        {
            if (source[key] is JArray array)
                return array.OfType<JObject>().ToList();
            return null;
        }

        public static List<Dictionary<string, string>> ParseTsv(string tsvContent)
        {
            List<Dictionary<string, string>> records = new List<Dictionary<string, string>>();
            string[] lines = tsvContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length == 0)
                throw new ArgumentException("The TSV content is empty.");

            string[] headers = lines[0].Split('\t');

            for (int i = 1; i < lines.Length; i++)
            {
                string[] fields = lines[i].Split('\t');
                if (fields.Length != headers.Length)
                    throw new ArgumentException($"The number of fields in line {i + 1} does not match the number of header fields.");

                Dictionary<string, string> record = new Dictionary<string, string>();
                for (int j = 0; j < headers.Length; j++)
                {
                    record.Add(headers[j], fields[j]);
                }
                records.Add(record);
            }

            return records;
        }
    }
}
