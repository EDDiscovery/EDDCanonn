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
using System.Linq;
using EDDCanonnPanel.Base;
using QuickJSON;

namespace EDDCanonnPanel.Utility
{
    public static class DataUtil
    {
        //Array for the patrol ranges.
        public static readonly int[] PatrolRanges = { 6, 24, 120, 720, 5040 };
        //Default fallback for galactic coords.
        public static readonly double PositionFallback = -99999.99;

        private static JArray _genusLocalised;
        public static String GenusLocalised(string genus)
        {
            if (_genusLocalised == null)
                _genusLocalised = genusLocalisedJson.JSONParseArray();

            if (_genusLocalised.Count == 0) return genus;

            return _genusLocalised.FirstOrDefault(obj => obj is JObject o && o["genus"]?.Value?.ToString() == genus)?["genus_localised"]?.Value?.ToString() ?? genus;
        }

        private static readonly string genusLocalisedJson = @"
        [
            { ""genus"": ""$Codex_Ent_Fonticulus_Genus_Name;"", ""genus_localised"": ""Fonticulua""         },
            { ""genus"": ""$Codex_Ent_Tubus_Genus_Name;"",      ""genus_localised"": ""Tubus""              },
            { ""genus"": ""$Codex_Ent_Tube_Name;"",             ""genus_localised"": ""Tubers""             },
            { ""genus"": ""$Codex_Ent_Tussocks_Genus_Name;"",   ""genus_localised"": ""Tussock""            },
            { ""genus"": ""$Codex_Ent_Seed_Name;"",             ""genus_localised"": ""Roseum Brain Tree""  },
            { ""genus"": ""$Codex_Ent_Osseus_Genus_Name;"",     ""genus_localised"": ""Osseus""             },
            { ""genus"": ""$Codex_Ent_Shrubs_Genus_Name;"",     ""genus_localised"": ""Frutexa""            },
            { ""genus"": ""$Codex_Ent_Stratum_Genus_Name;"",    ""genus_localised"": ""Stratum""            },
            { ""genus"": ""$Codex_Ent_Fumerolas_Genus_Name;"",  ""genus_localised"": ""Fumerola""           },
            { ""genus"": ""$Codex_Ent_Conchas_Genus_Name;"",    ""genus_localised"": ""Concha""             },
            { ""genus"": ""$Codex_Ent_Electricae_Genus_Name;"", ""genus_localised"": ""Electricae""         },
            { ""genus"": ""$Codex_Ent_Ground_Struct_Ice_Name;"",""genus_localised"": ""Crystalline Shards"" },
            { ""genus"": ""$Codex_Ent_Sphere_Name;"",           ""genus_localised"": ""Luteolum Anemone""   },
            { ""genus"": ""$Codex_Ent_Cactoid_Genus_Name;"",    ""genus_localised"": ""Cactoida""           },
            { ""genus"": ""$Codex_Ent_Fungoids_Genus_Name;"",   ""genus_localised"": ""Fungoida""           },
            { ""genus"": ""$Codex_Ent_Aleoids_Genus_Name;"",    ""genus_localised"": ""Aleoida""            },
            { ""genus"": ""$Codex_Ent_Recepta_Genus_Name;"",    ""genus_localised"": ""Recepta""            },
            { ""genus"": ""$Codex_Ent_Bacterial_Genus_Name;"",  ""genus_localised"": ""Bacterium""          },
            { ""genus"": ""$Codex_Ent_Cone_Name;"",             ""genus_localised"": ""Bark Mounds""        },
            { ""genus"": ""$Codex_Ent_Vents_Name;"",            ""genus_localised"": ""Amphora Plant""      },
            { ""genus"": ""$Codex_Ent_Brancae_Name;"",          ""genus_localised"": ""Brain Trees""        },
            { ""genus"": ""$Codex_Ent_Barnacles_Name;"",        ""genus_localised"": ""Thargoid Barnacles"" },
            { ""genus"": ""$Codex_Ent_Thargoid_Coral_Name;"",   ""genus_localised"": ""Coral Structures""   },
            { ""genus"": ""$Codex_Ent_Thargoid_Tower_Name;"",   ""genus_localised"": ""Thargoid Spires""    }
        ]";


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
                string error = $"EDDCanonn: TSV parsing error: {fe.Message}";
                CanonnLogging.Instance.LogToFile(error);
                throw;
            }
            catch (Exception ex)
            {
                string error = $"EDDCanonn: Unexpected error while parsing TSV: {ex.Message}";
                CanonnLogging.Instance.LogToFile(error);
                throw;
            }

            return records;
        }
    }
}
