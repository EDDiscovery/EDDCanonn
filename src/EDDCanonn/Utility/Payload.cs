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

using QuickJSON;
using static EDDDLLInterfaces.EDDDLLIF;

namespace EDDCanonnPanel.Base
{
    public class Payload
    {
        private const string Platform = "PC";

        public static JObject BuildPayload(JournalEntry je, JObject statusJson)
        {
            if (statusJson == null) statusJson = new JObject();
            JObject rawEvent = je.json.JSONParse().Object();
            JObject payload = new JObject

            {
                ["gameState"] = ExtractSystemInfo(je, rawEvent, statusJson),
                ["rawEvent"] = rawEvent,
                ["eventType"] = je.eventid,
                ["cmdrName"] = je.cmdrname
            };

            return payload;
        }

        private static JObject ExtractSystemInfo(JournalEntry je, JObject rawEvent, JObject statusJson)
        {
            JObject gameState = new JObject
            {
                ["systemName"] = je.systemname,
                ["systemAddress"] = je.systemaddress,
                ["systemCoordinates"] = JArray.FromObject(new double[] { je.x, je.y, je.z }),
                ["clientVersion"] = "EDDCanonnClientV" + CanonnUtil.V.ToString(),
                ["isBeta"] = je.beta,
                ["platform"] = Platform,
                ["odyssey"] = je.odyssey
            };

            string bodyName = rawEvent["BodyName"].StrNull() != null && rawEvent["BodyName"].StrNull() != "null" ? rawEvent["BodyName"].StrNull()
                : je.bodyname != null && je.bodyname != "Unknown" ? je.bodyname : null;
            if (!string.IsNullOrEmpty(bodyName))
                gameState["bodyName"] = bodyName;

            string stationName = rawEvent["StationName"].StrNull() != null && rawEvent["StationName"].StrNull() != "null" ? rawEvent["StationName"].StrNull()
                : je.stationname != null && je.stationname != "Unknown" ? je.stationname : null;
            if(!string.IsNullOrEmpty(stationName))
                gameState["station"] = stationName;

            ExtractPositionData(gameState, statusJson);
            ExtractAdditionalStatusData(gameState, statusJson);

            return gameState;
        }

        private static void ExtractPositionData(JObject gameState, JObject statusJson)
        {
            if (statusJson.Contains("Pos") && statusJson["Pos"]?["ValidPosition"]?.ToObject<bool>() == true)
            {
                if (statusJson["Pos"]["Latitude"] != null)
                    gameState["latitude"] = statusJson["Pos"]["Latitude"].ToObject<double>();

                if (statusJson["Pos"]["Longitude"] != null)
                    gameState["longitude"] = statusJson["Pos"]["Longitude"].ToObject<double>();
            }
        }

        private static void ExtractAdditionalStatusData(JObject gameState, JObject statusJson)
        {
            if (statusJson.Contains("Temperature") &&
                statusJson["Temperature"] != null &&
                statusJson["Temperature"].ToObject<double>() >= 0)
            {
                gameState["temperature"] = statusJson["Temperature"].ToObject<double>();
            }

            if (statusJson.Contains("Gravity") &&
                statusJson["Gravity"] != null &&
                statusJson["Gravity"].ToObject<double>() >= 0)
            {
                gameState["gravity"] = statusJson["Gravity"].ToObject<double>();
            }
        }
    }
}
