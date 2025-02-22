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

namespace EDDCanonn
{
    public class EDDCanonnEDDClass
    {
        public static readonly string V = "0.0";
        public static bool Outdated = false;
        public static EDDDLLInterfaces.EDDDLLIF.EDDCallBacks DLLCallBack;

        public EDDCanonnEDDClass()
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

            return V; 

        }

        public void EDDDataResult(object requesttag, object usertag, string data)
        {
            EDDCanonn.EDDCanonnUserControl uc = usertag as EDDCanonn.EDDCanonnUserControl;
            uc.DataResult(requesttag, data);
        }

        public void EDDTerminate()
        {
            Debug.WriteLine("EDDCanonn Unload");
        }

        public static int CompareVersions(string v)
        {
            try
            {
                Version ver1 = new Version(V);
                Version ver2 = new Version(v);
                return ver1.CompareTo(ver2);
            }
            catch
            {
                Console.Error.WriteLine($"EDDCanonn: Invalid version format");
                return 0;
            }
        }

    }
}
