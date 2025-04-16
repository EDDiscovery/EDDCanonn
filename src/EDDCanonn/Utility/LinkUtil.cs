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

using EDDCanonnPanel.Base;
using System.Diagnostics;
using System;

namespace EDDCanonnPanel.Utility
{
    public static class LinkUtil
    {
        //Base URL to open system-specific data on signals.canonn.tech.
        public static readonly string SignalsCanonnTech = "https://signals.canonn.tech/index.html?system=";
        //URL for posting event data to the Canonn backend.
        public static readonly string CanonnPostUrl = "https://us-central1-canonn-api-236217.cloudfunctions.net/postEvent";
        //URL to fetch patrol data from the Canonn spreadsheet.
        public static readonly string PatrolUrl = "https://docs.google.com/spreadsheets/d/e/2PACX-1vQsi1Vbfx4Sk2msNYiqo0PVnW3VHSrvvtIRkjT-JvH_oG9fP67TARWX2jIjehFHKLwh4VXdSh0atk3J/pub?gid=0&single=true&output=tsv";

        //URL to retrieve biological statistics by ID.
        public static readonly string BioStats = "https://us-central1-canonn-api-236217.cloudfunctions.net/query/codex/biostats?id=";
        //URL to retrieve codex hierarchy references.
        public static readonly string CodexRef = "https://us-central1-canonn-api-236217.cloudfunctions.net/query/codex/ref?hierarchy=1";
        //URL to fetch the latest posts from the Canonn news feed.
        public static readonly string CanonnNews = "https://canonn.science/wp-json/wp/v2/posts";

        //GitHub repository for the EDDCanonn plugin.
        public static readonly string EDDCanonnGitHub = "https://github.com/EDDiscovery/EDDCanonn";
        //GitHub organization page for EDDiscovery.
        public static readonly string EDDGitHub = "https://github.com/EDDiscovery";
        //Official Canonn Research Group website.
        public static readonly string CanonnWebPage = "https://canonn.science";

        // Attempts to open a given URL in the default web browser.
        public static void OpenUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return;

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                };

                Process.Start(psi);
            }
            catch (Exception ex)
            {
                string error = $"EDDCanonn: Error opening URL: {ex}";
                CanonnLogging.Instance.Log(error);
            }
        }
    }
}
