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
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using QuickJSON;
namespace EDDCanonnPanel
{
    //Utility class for shared helper functions used in the Canonn plugin.
    public static class CanonnUtil
    {
        //Tracks the version of the currently running plugin assembly.
        public static readonly Version V = Assembly.GetExecutingAssembly().GetName().Version;

        //Tracks the number of active Canonn plugin instances.
        public static int InstanceCount = 0;
        private static int _currentId = 998;
        public static int GenerateId()
        {
            return Interlocked.Increment(ref _currentId);
        }

        //Checks whether a key-value pair exists in the provided list.
        //If key is null, checks if the value appears in any object fields.
        public static bool ContainsKeyValuePair(List<JObject> existingList, string key, string value)
        {
            if (existingList == null || value == null)
                return false;

            if (key == null)
                return existingList.Any(obj => obj.Contains(value));

            return existingList.Any(obj => obj.Contains(key) && obj[key].StrNull() == value);
        }

        //Finds and returns the first JObject that matches the given key-value pair.
        //If key is null, matches based on value presence alone.
        public static JObject FindFirstMatchingJObject(List<JObject> existingList, string key, string value)
        {
            if (existingList == null || value == null)
                return null;

            if (key == null)
                return existingList.FirstOrDefault(obj => obj.Contains(value));

            return existingList.FirstOrDefault(obj => obj.Contains(key) &&
                string.Equals(obj[key].StrNull(), value, StringComparison.OrdinalIgnoreCase));
        }

        //Returns a merged list of unique JObject entries from the source array and existing list.
        //Entries are considered duplicates if they match by structure or key content.
        public static List<JObject> GetUniqueEntries(JObject eventData, string key, List<JObject> existingList)
        {
            if (eventData[key] is JArray array)
            {
                List<JObject> result = existingList ?? new List<JObject>();
                result.AddRange(array.OfType<JObject>().Where(item => !result.Any(existing => JToken.DeepEquals(existing, item)
                || (existing[existing.PropertyNames()?[0]].Str("none_")) == (item[item.PropertyNames()?[0]].Str("none"))
                || (existing.Str("none_")).Contains(item[item.PropertyNames()?[0]].Str("none")))));
                return result;
            }
            return existingList ?? new List<JObject>();
        }

        //Returns all JObject items from a source object under the given key.
        //Optionally wraps primitive values into JObject using subKey if needed.
        public static List<JObject> GetJObjectList(JObject source, string key, string subKey = null)
        {
            if (source == null || source.IsNull)
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

        //Creates and returns a fully initialized DataGridViewRow, including optional tooltips.
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

        //Disposes all rows in the given list and clears it.
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

        //Clones a list of DataGridViewRows to allow reuse, since rows can’t belong to multiple views.
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
