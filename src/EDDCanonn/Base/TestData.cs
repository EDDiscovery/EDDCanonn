using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDCanonn.Base
{
    public static class TestData
    {
        public static readonly List<(string EventName, string JsonPayload, bool ExpectedResult)> WhiteListTestCases = new List<(string EventName, string JsonPayload, bool ExpectedResult)>
{
    (
        "SAASignalsFound",
        "{ \"timestamp\": \"2025-01-16T14:26:05Z\", \"event\": \"SAAScanComplete\", " +
        "\"BodyName\": \"Stuemeae KM-W c1-6019 B 3\", \"SystemAddress\": 1654574041540370, " +
        "\"BodyID\": 19, \"ProbesUsed\": 4, \"EfficiencyTarget\": 6 }",
        true
    ),
    (
        "Interdicted",
        "{ \"timestamp\": \"2024-12-15T17:17:42Z\", \"event\": \"Interdicted\", " +
        "\"Submitted\": false, \"IsPlayer\": false, \"IsThargoid\": true }",
        true
    ),
    (
        "Interdicted",
        "{ \"timestamp\": \"2024-12-15T17:17:42Z\", \"event\": \"Interdicted\", " +
        "\"Submitted\": false, \"IsPlayer\": false, \"IsThargoid\": false }",
        false
    ),
    (
        "Interdicted",
        "{ \"timestamp\": \"2024-12-15T17:17:42Z\", \"event\": \"Interdicted\", " +
        "\"Submitted\": false, \"IsPlayer\": true, \"IsThargoid\": false }",
        false
    ),
    (
        "MaterialCollected",
        "{ \"timestamp\": \"2024-12-14T13:11:04Z\", \"event\": \"MaterialCollected\", " +
        "\"Category\": \"Encoded\", \"Name\": \"tg_shipflightdata\", " +
        "\"Name_Localised\": \"Massive Energy Surge Analytics\", \"Count\": 1 }",
        true
    ),
    (
        "FSSSignalDiscovered",
        "{ \"timestamp\": \"2024-12-18T10:32:51Z\", \"event\": \"FSSSignalDiscovered\", " +
        "\"SystemAddress\": 2106421430643, \"SignalName\": \"$Fixed_Event_Life_Belt;\", " +
        "\"SignalName_Localised\": \"Resource Extraction Site [High]\", " +
        "\"SignalType\": \"ResourceExtraction\" }",
        true
    ),
    (
        "FSSSignalDiscovered",
        "{ \"timestamp\": \"2024-12-18T10:32:51Z\", \"event\": \"FSSSignalDiscovered\", " +
        "\"SystemAddress\": 2106421430643, \"SignalName\": \"$MULTIPLAYER_SCENARIO78_TITLE;\", " +
        "\"SignalName_Localised\": \"Resource Extraction Site [High]\", " +
        "\"SignalType\": \"ResourceExtraction\" }",
        false
    ),
    (
        "Promotion",
        "{ \"timestamp\": \"2024-12-18T15:08:51Z\", \"event\": \"Promotion\", " +
        "\"Federation\": 7 }",
        true
    ),
    (
        "Any",
        "{ \"timestamp\": \"2025-01-16T14:26:05Z\", \"event\": \"SAAScanComplete\", " +
        "\"BodyType\": \"HyperbolicOrbiter\", \"SystemAddress\": 1654574041540370, " +
        "\"BodyID\": 19, \"ProbesUsed\": 4, \"EfficiencyTarget\": 6 }",
        true
    ),
    (
        "Any",
        "{ \"timestamp\": \"2025-01-16T14:26:05Z\", \"event\": \"SAAScanComplete\", " +
        "\"BodyName\": \"Stuemeae KM-W c1-6019 B 3\", \"SystemAddress\": 1654574041540370, " +
        "\"BodyID\": 19, \"ProbesUsed\": 4, \"EfficiencyTarget\": 6 }",
        false
    )
};
    }
}
