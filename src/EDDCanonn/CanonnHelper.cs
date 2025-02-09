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
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using QuickJSON;
namespace EDDCanonn
{
    //Class for outsourced help functions.
    public static class CanonnHelper
    {
        //Count for active canonn plugin instances.
        public static int InstanceCount = 0;
        //Array for the patrol ranges.
        public static readonly int[] PatrolRanges = { 6, 24, 120, 720, 5040 };
        //Default fallback for galactic coords.
        public static readonly double PositionFallback = -99999.99;

        //Canonn urls.
        public static readonly string WhitelistUrl = "https://us-central1-canonn-api-236217.cloudfunctions.net/postEventWhitelist";
        public static readonly string EventPushUrl = "https://us-central1-canonn-api-236217.cloudfunctions.net/postEvent";
        public static readonly string PatrolUrl = "https://docs.google.com/spreadsheets/d/e/2PACX-1vQsi1Vbfx4Sk2msNYiqo0PVnW3VHSrvvtIRkjT-JvH_oG9fP67TARWX2jIjehFHKLwh4VXdSh0atk3J/pub?gid=0&single=true&output=tsv";

        //Attempts to convert a JToken to a specified value type, returning a fallback value. It is for numbers!
        public static T GetValueOrDefault<T>(JToken obj, T fallback) where T : struct
        {
            if (obj == null)
            {
                return fallback;
            }

            try
            {
                Type targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

                if (obj.IsString)
                {
                    string value = obj.Value.ToString();
                    if (string.IsNullOrWhiteSpace(value))
                        return fallback;

                    if (targetType == typeof(int) && int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out int intResult))
                        return (T)(object)intResult;

                    if (targetType == typeof(double) && double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double doubleResult))
                        return (T)(object)doubleResult;

                    if (targetType == typeof(float) && float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out float floatResult))
                        return (T)(object)floatResult;

                    if (targetType == typeof(bool) && bool.TryParse(value, out bool boolResult))
                        return (T)(object)boolResult;

                    if (targetType == typeof(DateTime) && DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateResult))
                        return (T)(object)dateResult;

                    return fallback;
                }

                return obj.ToObject<T>();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"EDDCanonn: Error parsing value: {ex.Message}");
                return fallback;
            }
        }

        //Checks if a given JObject is unique within a list and returns it if it is not already present and does not contain an excluded value.
        public static JObject GetUniqueEntry(JObject eventData, List<JObject> existingList, string exclude = null)
        {
            if (eventData == null || existingList == null)
                return null;

            if (exclude != null && eventData.ToString().Contains(exclude))
                return null;

            if (existingList.Any(existing => JToken.DeepEquals(existing, eventData)))
                return null;

            return eventData;
        }

        //The same as above, but for JArrays.
        public static List<JObject> GetUniqueEntries(JObject eventData, string key, List<JObject> existingList, string exclude = null)
        {
            if (eventData[key] is JArray array)
            {
                List<JObject> result = existingList ?? new List<JObject>();
                result.AddRange(array.OfType<JObject>()
                                        .Where(item => (exclude == null || !item.ToString().Contains(exclude)) &&
                                                    !result.Any(existing => JToken.DeepEquals(existing, item))));
                return result;
            }
            return existingList ?? new List<JObject>();
        }

        //The same as 'GetUniqueEntries', but without duplicate check.
        public static List<JObject> GetJObjectList(JObject source, string key, string exclude = null)
        {
            if (source[key] is JArray array)
            {
                return array.OfType<JObject>()
                            .Where(obj => exclude == null || !obj.ToString().Contains(exclude))
                            .ToList();
            }
            return null;
        }

        //Parses TSV content into a list of dictionaries.
        public static List<Dictionary<string, string>> ParseTsv(string tsvContent)
        {
            if (string.IsNullOrWhiteSpace(tsvContent))
                throw new ArgumentException($"EDDCanonn: The TSV content is empty or null.");

            List<Dictionary<string, string>> records = new List<Dictionary<string, string>>();

            try
            {
                string[] lines = tsvContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length == 0)
                    throw new ArgumentException($"EDDCanonn: The TSV content does not contain any valid lines.");

                string[] headers = lines[0].Split('\t');
                for (int i = 1; i < lines.Length; i++)
                {
                    string[] fields = lines[i].Split('\t');

                    if (fields.Length != headers.Length)
                        throw new FormatException($"EDDCanonn: Mismatch between header count and field count in line {i + 1}.");

                    Dictionary<string, string> record = new Dictionary<string, string>();
                    for (int j = 0; j < headers.Length; j++)
                    {
                        record[headers[j]] = fields[j];
                    }
                    records.Add(record);
                }
            }
            catch (FormatException fe)
            {
                Console.WriteLine($"EDDCanonn: TSV parsing error: {fe.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EDDCanonn: Unexpected error while parsing TSV: {ex.Message}");
                throw;
            }

            return records;
        }

        //Attempts to open a given URL in the default web browser.
        public static void OpenUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return;

            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url.Replace("&", "^&")}")
                    {
                        CreateNoWindow = true
                    });
                }
                else
                {
                    // Linux/macOS support (if needed)
                    Process.Start("xdg-open", url);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EDDCanonn: Error opening URL: {ex.Message}");
            }
        }

    }
}
