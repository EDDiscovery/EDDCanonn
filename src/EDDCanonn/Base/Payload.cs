﻿/*
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

using QuickJSON;
using static EDDDLLInterfaces.EDDDLLIF;

namespace EDDCanonn.Base
{
    public class Payload
    {
        public static JObject BuildPayload(JournalEntry je, JObject statusJson)
        {
            //Root objects
            JObject gameState = new JObject();
            JObject rawEvent = je.json.JSONParse().Object();
            JObject status = statusJson;

            //Constants and other fields
            string clientVersion = "EDDCanonnClient v0.1";
            string platform = "PC";

            //Prepare payload
            JObject payload = new JObject
            {
                ["gameState"] = gameState
            };

            //System information
            gameState["systemName"] = je.systemname;
            gameState["systemAddress"] = je.systemaddress;
            gameState["systemCoordinates"] = JArray.FromObject(new double[] { je.x, je.y, je.z });

            //Body name
            string bodyName = rawEvent["BodyName"]?.Value?.ToString();
            if (!string.IsNullOrEmpty(bodyName))
            {
                gameState["bodyName"] = bodyName;
            }
            else if (!string.IsNullOrEmpty(je.bodyname) && je.bodyname != "Unknown")
            {
                gameState["bodyName"] = je.bodyname;
            }

            //Station name
            string stationName = rawEvent["StationName"]?.Value?.ToString();
            if (!string.IsNullOrEmpty(stationName))
            {
                gameState["station"] = stationName;
            }
            else if (!string.IsNullOrEmpty(je.stationname) && je.stationname != "Unknown")
            {
                gameState["station"] = je.stationname;
            }

            //Data from status
            if (status != null &&
                status.Contains("Pos") &&
                status["Pos"]?["ValidPosition"]?.ToObject<bool>() == true)
            {
                if (status["Pos"]["Latitude"] != null)
                {
                    gameState["latitude"] = status["Pos"]["Latitude"].ToObject<double>();
                }
                if (status["Pos"]["Longitude"] != null)
                {
                    gameState["longitude"] = status["Pos"]["Longitude"].ToObject<double>();
                }
            }

            //More data from status
            if (status != null)
            {
                if (status.Contains("Temperature") &&
                    status["Temperature"] != null &&
                    status["Temperature"].ToObject<double>() >= 0)
                {
                    gameState["temperature"] = status["Temperature"].ToObject<double>();
                }

                if (status.Contains("Gravity") &&
                    status["Gravity"] != null &&
                    status["Gravity"].ToObject<double>() >= 0)
                {
                    gameState["gravity"] = status["Gravity"].ToObject<double>();
                }
            }

            //Additionals
            gameState["clientVersion"] = clientVersion;
            gameState["isBeta"] = je.beta;
            gameState["platform"] = platform;
            gameState["odyssey"] = je.odyssey;

            //Finalize
            payload["rawEvent"] = rawEvent;
            payload["eventType"] = je.eventid;
            payload["cmdrName"] = je.cmdrname;

            return payload;
        }

    }
}
