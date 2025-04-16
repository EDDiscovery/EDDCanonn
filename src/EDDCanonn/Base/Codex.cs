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

namespace EDDCanonnPanel.Base
{
    class CodexEntry
    {
        public CodexEntry(string codexType, string codexSubType, string localisedName, int entryId, string name, string category, string subCategory, string platform, int reward)
        {
            CodexType = codexType;
            CodexSubType = codexSubType;
            LocalisedName = localisedName;
            EntryId = entryId;
            Name = name;
            Category = category;
            SubCategory = subCategory;
            Platform = platform;
            Reward = reward;
        }

        public string CodexType { get; set; }
        public string CodexSubType { get; set; }
        public string LocalisedName { get; set; } 
        public int EntryId { get; set; }  
        public string Name { get; set; }  
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string Platform { get; set; }
        public int Reward { get; set; }
    }
    class CodexDatabase
    {
        private readonly Dictionary<int, CodexEntry> entriesById = new Dictionary<int, CodexEntry>();
        private readonly Dictionary<string, CodexEntry> entriesByLocalised = new Dictionary<string, CodexEntry>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, CodexEntry> entriesByName = new Dictionary<string, CodexEntry>(StringComparer.OrdinalIgnoreCase);

        public void Add(CodexEntry entry)
        {
            entriesById[entry.EntryId] = entry;

            if (!string.IsNullOrEmpty(entry.LocalisedName))
                entriesByLocalised[entry.LocalisedName] = entry;

            if (!string.IsNullOrEmpty(entry.Name))
                entriesByName[entry.Name] = entry;
        }

        public CodexEntry GetByEntryId(int id)
        {
            return entriesById.TryGetValue(id, out var entry) ? entry : null;
        }

        public CodexEntry GetByLocalisedName(string name)
        {
            return entriesByLocalised.TryGetValue(name, out var entry) ? entry : null;
        }

        public CodexEntry GetByName(string codexName)
        {
            return entriesByName.TryGetValue(codexName, out var entry) ? entry : null;
        }
    }
}
