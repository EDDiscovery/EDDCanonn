﻿/******************************************************************************
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
using System.Diagnostics;
using System.IO;
using EDDCanonnPanel.Emitter;
using QuickJSON;

namespace EDDCanonnPanel
{
    public class CanonnEDDClass
    {
        public static EDDDLLInterfaces.EDDDLLIF.EDDCallBacks DLLCallBack;
        public static string DLLPath;

        public CanonnEDDClass()
        {
            Debug.WriteLine("EDDCanonnPanel Made DLL instance");
        }

        public string EDDInitialise(string vstr, string dllfolder, EDDDLLInterfaces.EDDDLLIF.EDDCallBacks cb)
        {
            DLLCallBack = cb;
            Debug.WriteLine("EDDCanonnPanel Init func " + vstr + " " + dllfolder);

            DLLPath = Path.GetDirectoryName(dllfolder);

            if (cb.ver >= 3 && cb.AddPanel != null)
            {
                string uniqueName = "EDDCanonnPanel";
                cb.AddPanel(uniqueName, typeof(EDDCanonnUserControl), "Canonn", uniqueName, "Canonn", Properties.Resources.canonn);
            }
            else
            {
                Debug.WriteLine("Panel registration failed: Incompatible version or AddPanel is null");
            }

            new CanonnEmitter();
            return CanonnUtil.V.ToString(); 
        }

        public void EDDDataResult(object requesttag, object usertag, string data)
        {
            EDDCanonnPanel.EDDCanonnUserControl uc = usertag as EDDCanonnPanel.EDDCanonnUserControl;
            uc.DataResult(requesttag, data);
        }

        public static event Action<EDDDLLInterfaces.EDDDLLIF.JournalEntry> OnNewJournalEntry;
        public static event Action OnTermination;

        public void EDDNewJournalEntry(EDDDLLInterfaces.EDDDLLIF.JournalEntry entry)
        {
            OnNewJournalEntry?.Invoke(entry);
        }

        private static readonly object _lockStatusJson = new object();
        private static JObject _statusJson;

        public void EDDNewUIEvent(string json)
        {
            JObject o = json.JSONParse().Object();
            if (o == null || string.IsNullOrEmpty(o["EventTypeStr"].Str()))
                return;

            lock (_lockStatusJson)
                _statusJson = o;
        }

        public static JObject GetStatusJson() //Return a deep copy.
        {
            lock (_lockStatusJson)
                return new JObject(_statusJson);
        }

        public void EDDTerminate()
        {
            OnTermination?.Invoke();
            Debug.WriteLine("EDDCanonnPanel Unload");
        }
    }
}
