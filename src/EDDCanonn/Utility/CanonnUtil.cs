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
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using EDDCanonnPanel.Base;
using QuickJSON;
namespace EDDCanonnPanel
{
    //Class for outsourced help functions.
    public static class CanonnUtil
    {
        public static readonly Version V = Assembly.GetExecutingAssembly().GetName().Version;

        //Count for active canonn plugin instances.
        public static int InstanceCount = 0;
        private static int _currentId = 998;
        public static int GenerateId()
        {
            return Interlocked.Increment(ref _currentId);
        }

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
                string error = $"EDDCanonn: Error parsing value: {ex.Message}";
                CanonnLogging.Instance.LogToFile(error);
                return fallback;
            }
        }

        //Checks if a key value pair exists.
        // If the key is null, it checks whether any object contains the value as a standalone field.
        public static bool ContainsKeyValuePair(List<JObject> existingList, string key, string value)
        {
            if (existingList == null || value == null)
                return false;

            if (key == null)
                return existingList.Any(obj => obj.Contains(value));

            return existingList.Any(obj => obj.Contains(key) && obj[key]?.Value?.ToString() == value);
        }

        //The same as above, but the JObject  is returned.
        public static JObject FindFirstMatchingJObject(List<JObject> existingList, string key, string value)
        {
            if (existingList == null || value == null)
                return null;

            if (key == null)
                return existingList.FirstOrDefault(obj => obj.Contains(value));

            return existingList.FirstOrDefault(obj => obj.Contains(key) &&
                string.Equals(obj[key]?.Value?.ToString(), value, StringComparison.OrdinalIgnoreCase));
        }

        //The same as above, but for JArrays.
        public static List<JObject> GetUniqueEntries(JObject eventData, string key, List<JObject> existingList)
        {
            if (eventData[key] is JArray array)
            {
                List<JObject> result = existingList ?? new List<JObject>();
                result.AddRange(array.OfType<JObject>().Where(item => !result.Any(existing => JToken.DeepEquals(existing, item)
                || (existing[existing.PropertyNames()?[0]]?.Value?.ToString() ?? "none_") == (item[item.PropertyNames()?[0]]?.Value?.ToString() ?? "none")
                || (existing.ToString() ?? "none_").Contains(item[item.PropertyNames()?[0]]?.Value?.ToString() ?? "none" ))));
                return result;
            }
            return existingList ?? new List<JObject>();
        }

        //The same as 'GetUniqueEntries', but without duplicate check.
        public static List<JObject> GetJObjectList(JObject source, string key, string subKey = null)
        {
            if (source == null)
                return null;

            if (source[key] is JArray array)
            {
                return array.Select(token => token is JObject obj ? obj : new JObject { [subKey ?? GenerateId().ToString()] = token }).ToList();
            }
            else if (source[key] is JObject obj)
            {
                return new List<JObject> { obj };
            }
            return null;
        }

        //Returns a filled row for the passed GridView.
        public static DataGridViewRow CreateDataGridViewRow(DataGridView dataGridView, object[] objects, string[] tooltips = null)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(dataGridView, objects);

            if (tooltips != null)
                row.Cells.Cast<DataGridViewCell>().Take(tooltips.Length)
                    .Select((cell, i) => (cell, tooltip: tooltips[i]))
                    .Where(x => !string.IsNullOrWhiteSpace(x.tooltip))
                    .ToList()
                    .ForEach(x => x.cell.ToolTipText = x.tooltip);

            return row;
        }

        //Frees all contents of a rowlist. Makes sense if this rows has not been passed to any controls.
        public static void DisposeDataGridViewRowList(List<DataGridViewRow> rows)
        {
            if (rows == null || rows.Count == 0)
                return;

            foreach (DataGridViewRow row in rows)
            {
                row?.Dispose();
            }

            rows.Clear();
        }

        //Makes a deepclone because a row can only have one parent.
        public static List<DataGridViewRow> CloneDataGridViewRowList(List<DataGridViewRow> rows)
        {
            return rows.Select((DataGridViewRow row) =>
            {
                DataGridViewRow clonedRow = (DataGridViewRow)row.Clone();
                for (int i = 0; i < row.Cells.Count; i++)
                {
                    clonedRow.Cells[i].Value = row.Cells[i].Value;
                }
                return clonedRow;
            }).ToList();
        }
    }
}
