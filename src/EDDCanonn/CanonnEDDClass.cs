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
using System.Diagnostics;
using System.Reflection;

namespace EDDCanonnPanel
{
    public class CanonnEDDClass
    {
        public static readonly Version V = Assembly.GetExecutingAssembly().GetName().Version;
        public static EDDDLLInterfaces.EDDDLLIF.EDDCallBacks DLLCallBack;

        public CanonnEDDClass()
        {
            Debug.WriteLine("EDDCanonn Made DLL instance");
        }

        public string EDDInitialise(string vstr, string dllfolder, EDDDLLInterfaces.EDDDLLIF.EDDCallBacks cb)
        {
            
            DLLCallBack = cb;
            Debug.WriteLine("EDDCanonn Init func " + vstr + " " + dllfolder);


            if (cb.ver >= 3 && cb.AddPanel != null)
            {
                string uniqueName = "EDDCanonn";
                cb.AddPanel(uniqueName, typeof(EDDCanonnUserControl), "Canonn", uniqueName, "Canonn", Properties.Resources.canonn);
            }
            else
            {
                Debug.WriteLine("Panel registration failed: Incompatible version or AddPanel is null");
            }

            return V.ToString(); 

        }

        public void EDDDataResult(object requesttag, object usertag, string data)
        {
            EDDCanonnPanel.EDDCanonnUserControl uc = usertag as EDDCanonnPanel.EDDCanonnUserControl;
            uc.DataResult(requesttag, data);
        }

        public void EDDTerminate()
        {
            Debug.WriteLine("EDDCanonn Unload");
        }
    }
}
