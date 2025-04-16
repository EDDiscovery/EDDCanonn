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
using System.IO;
using System.Linq;
using System.Reflection;
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

        //Resolves a biological type or genus name to its corresponding genus or type,
        //depending on the input match found in the _biologyGenuses list.
        public static String BiologyGenuses(string input)
        {
            if (_biologyGenuses == null || _biologyGenuses.Count == 0) return input;

            return _biologyGenuses
                .OfType<JObject>()
                .FirstOrDefault(o => o["Type"].StrNull() == input || o["Genus"].StrNull() == input)
                is JObject match
                ? (match["Type"].StrNull() == input ? match["Genus"].StrNull() ?? input : match["Type"].StrNull() ?? input)
                : input;
        }

        private static JArray _biologyGenuses =
        @"
        [
            { ""Type"": ""Fonticulus"",         ""Genus"": ""$Codex_Ent_Fonticulus_Genus_Name;""    },
            { ""Type"": ""Tubus"",              ""Genus"": ""$Codex_Ent_Tubus_Genus_Name;""         },
            { ""Type"": ""Tussocks"",           ""Genus"": ""$Codex_Ent_Tussocks_Genus_Name;""      },
            { ""Type"": ""Osseus"",             ""Genus"": ""$Codex_Ent_Osseus_Genus_Name;""        },
            { ""Type"": ""Shrubs"",             ""Genus"": ""$Codex_Ent_Shrubs_Genus_Name;""        },
            { ""Type"": ""Stratum"",            ""Genus"": ""$Codex_Ent_Stratum_Genus_Name;""       },
            { ""Type"": ""Fumerolas"",          ""Genus"": ""$Codex_Ent_Fumerolas_Genus_Name;""     },
            { ""Type"": ""Conchas"",            ""Genus"": ""$Codex_Ent_Conchas_Genus_Name;""       },
            { ""Type"": ""Electricae"",         ""Genus"": ""$Codex_Ent_Electricae_Genus_Name;""    },
            { ""Type"": ""Cactoid"",            ""Genus"": ""$Codex_Ent_Cactoid_Genus_Name;""       },
            { ""Type"": ""Fungoids"",           ""Genus"": ""$Codex_Ent_Fungoids_Genus_Name;""      },
            { ""Type"": ""Aleoids"",            ""Genus"": ""$Codex_Ent_Aleoids_Genus_Name;""       },
            { ""Type"": ""Recepta"",            ""Genus"": ""$Codex_Ent_Recepta_Genus_Name;""       },
            { ""Type"": ""Bacterial"",          ""Genus"": ""$Codex_Ent_Bacterial_Genus_Name;""     },
            { ""Type"": ""Clypeus"",            ""Genus"": ""$Codex_Ent_Clypeus_Genus_Name;""       },
            { ""Type"": ""Shards"",             ""Genus"": ""$Codex_Ent_Ground_Struct_Ice_Name;""   },

            { ""Type"": ""Tubers"",             ""Genus"": ""$Codex_Ent_Tube_Name;""                },
            { ""Type"": ""Brain Tree"",         ""Genus"": ""$Codex_Ent_Brancae_Name;""             },
            { ""Type"": ""Anemone"",            ""Genus"": ""$Codex_Ent_Sphere_Name;""              },
            { ""Type"": ""Bark Mounds"",        ""Genus"": ""$Codex_Ent_Cone_Name;""                },
            { ""Type"": ""Amphora Plant"",      ""Genus"": ""$Codex_Ent_Vents_Name;""               }]".JSONParseArray() ?? null; 

        public static string ReadEmbeddedTextFile(string resourceName)
        {
            if (string.IsNullOrWhiteSpace(resourceName))
                throw new ArgumentException("EDDCanonn: The resource name is empty or null.");

            try
            {
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                        throw new FileNotFoundException($"EDDCanonn: Resource '{resourceName}' not found in assembly.");

                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string content = reader.ReadToEnd();

                        if (string.IsNullOrWhiteSpace(content))
                            throw new ArgumentException($"EDDCanonn: The file '{resourceName}' is empty or contains only whitespace.");

                        return content;
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                string error = $"EDDCanonn: File not found: {ex}";
                CanonnLogging.Instance.Log(error);
                throw;
            }
            catch (ArgumentException ex)
            {
                string error = $"EDDCanonn: Invalid file content: {ex}";
                CanonnLogging.Instance.Log(error);
                throw;
            }
            catch (Exception ex)
            {
                string error = $"EDDCanonn: Unexpected error while reading resource '{resourceName}': {ex}";
                CanonnLogging.Instance.Log(error);
                throw;
            }
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
                string error = $"EDDCanonn: TSV parsing error: {fe.Message}";
                CanonnLogging.Instance.Log(error);
                throw;
            }
            catch (Exception ex)
            {
                string error = $"EDDCanonn: Unexpected error while parsing TSV: {ex}";
                CanonnLogging.Instance.Log(error);
                throw;
            }

            return records;
        }
    }
}
