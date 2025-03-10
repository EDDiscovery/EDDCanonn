﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EDDCanonnPanel.Base;
using EDDCanonnPanel.Utility;
using QuickJSON;

namespace EDDCanonnPanel.Emitter
{
    public class CanonnEmitter

    {
        private ActionDataHandler dataHandler;

        public CanonnEmitter()
        {
            Debug.WriteLine("EDDCanonnEmitter Made instance");

            CanonnEDDClass.OnNewJournalEntry += ProcessJournalEntry;
            CanonnEDDClass.OnTermination += Closing;
            CanonnEDDClass.OnUIEvent += ProcessUIEvent;

            dataHandler = new ActionDataHandler();
            InitializeWhitelist();
        }

        private readonly object _canonnPushLock = new object();
        private void ProcessJournalEntry(EDDDLLInterfaces.EDDDLLIF.JournalEntry entry)
        {
            if (entry.json == null || Whitelist == null || string.IsNullOrEmpty(entry.eventid))
                return;

            JObject o = entry.json.JSONParseObject();
            if (o == null)
                return;

            string eventId = entry.eventid;

            if (IsEventValid(eventId, o)) // Send event to canonn if valid.
            {
                dataHandler.StartTaskAsync( //wip
                (subToken) =>
                {
                    lock (_canonnPushLock)
                    {
                        string payload = Payload.BuildPayload(entry, GetStatusJson()).ToString(true, "  ");
                        string buildMsg = Environment.NewLine + $"Build payload for: {eventId} => " + Environment.NewLine + "{payload}" + Environment.NewLine;
                        CanonnLogging.Instance.LogToFile(buildMsg);

                        (bool success, string response) = dataHandler.PushData(LinkUtil.CanonnPostUrl, payload);

                        string statusMsg = Environment.NewLine + $"Status: =>" + Environment.NewLine + "{payload}" + Environment.NewLine;
                        CanonnLogging.Instance.LogToFile(statusMsg);
                    }
                },
                ex =>
                {
                    string error = $"EDDCanonn: Unexpected error in Canonn-Push: {ex.Message}";
                    CanonnLogging.Instance.LogToFile(error);
                },
                    "Emitter - Canonn-Push"
                );
            }
        }

        private readonly object _lockStatusJson = new object();
        private JObject _statusJson;
        private void ProcessUIEvent(string jsonui)
        {
            JObject o = jsonui.JSONParse().Object();
            if (o == null)
                return;

            string type = o["EventTypeStr"].Str();
            if (string.IsNullOrEmpty(type))
                return;

            lock (_lockStatusJson)
                _statusJson = o;
        }

        private JObject GetStatusJson() //Return a deep copy.
        {
            lock (_lockStatusJson)
                return new JObject(_statusJson);
        }

        public void Closing()
        {
            dataHandler?.Closing();
        }

        #region WhiteList

        //this generates a data structure like this:

        // Event Type: CodexEntry
        // Event Type: ApproachSettlement
        // Event Type: undefined
        //   Data Block:
        //     USSType: $USS_Type_AXShips;
        //   Data Block:
        //     BodyType: HyperbolicOrbiter
        //   Data Block:
        //     NearestDestination_Localised: Nonhuman Signature
        //   Data Block:
        //     NearestDestination: $POIScene_Wreckage_UA;
        // Event Type: FSSSignalDiscovered
        //   Data Block:
        //     SignalName: $Fixed_Event_Life_Belt;
        //   Data Block:
        //     SignalName: $Fixed_Event_Life_Cloud;
        //   Data Block:
        //     SignalName: $Fixed_Event_Life_Ring;
        //   Data Block:
        //     IsStation: True
        // Event Type: BuySuit
        // Event Type: Docked
        //   Data Block:
        //     StationType: FleetCarrier
        //   Data Block:
        //     StationName: Hutton Orbital
        // Event Type: CarrierJump
        //   Data Block:
        //     StationType: FleetCarrier
        // Event Type: Commander
        // Event Type: FSSBodySignals
        // Event Type: Interdicted
        //   Data Block:
        //     Faction: 
        //     IsPlayer: False
        //   Data Block:
        //     IsPlayer: False
        //     IsThargoid: True
        // Event Type: Promotion
        // Event Type: SellOrganicData
        // Event Type: SAASignalsFound
        // Event Type: ScanOrganic
        // Event Type: MaterialCollected
        //   Data Block:
        //     Name: tg_shipflightdata
        //   Data Block:
        //     Name: unknownshipsignature

        private WhitelistData Whitelist;
        private void InitializeWhitelist()
        {
            Whitelist = new WhitelistData();
            // Fetch the whitelist
            dataHandler.StartTaskAsync(
            (token) =>
            {
                try
                {
                    JArray whitelistItems = dataHandler.FetchData(LinkUtil.CanonnPostUrl + "Whitelist").response.JSONParseArray();
                    if (whitelistItems == null || whitelistItems.Count == 0)
                        throw new Exception("EDDCanonn: Whitelist is null");


                    for (int i = 0; i < whitelistItems.Count; i++)
                    {
                        JObject itemObject = whitelistItems[i].Object();

                        AddToWhitelistItem(itemObject);
                    }

                    CanonnLogging.Instance.LogToFile("=== Whitelist Entries ===" + Environment.NewLine +
                        WhiteListTest.PrintWhitelist(Whitelist) + Environment.NewLine);

                }
                catch (Exception ex)
                {
                    string error = $"EDDCanonn: Error processing whitelist: {ex.Message}";
                    CanonnLogging.Instance.LogToFile(error);
                    throw;
                }
            },
            ex =>
            {
                Whitelist = null;
                string error = $"EDDCanonn: Error in InitializeWhitelist Task: {ex.Message}";
                CanonnLogging.Instance.LogToFile(error);
            },
            "Emitter - InitializeWhitelist"
            );
        }

        private void AddToWhitelistItem(JObject itemObject)
        {
            string definitionRaw = itemObject["definition"].Str();
            if (string.IsNullOrEmpty(definitionRaw))
                return;

            JObject definitionObject = definitionRaw.JSONParse().Object();

            // Default key to identify the type. Choose the most common one.
            string typeKey = "event";
            string typeValue = definitionObject[typeKey].Str();
            // Everything that does not contain the default key is treated as undefined.
            if (string.IsNullOrEmpty(typeValue))
                typeValue = "undefined";

            WhitelistEvent existingEvent = null;
            for (int e = 0; e < Whitelist.Events.Count; e++)
            {
                if (Whitelist.Events[e].Type.Equals(typeValue, StringComparison.InvariantCultureIgnoreCase))
                {
                    existingEvent = Whitelist.Events[e];
                    break;
                }
            }

            if (existingEvent == null)
            {
                existingEvent = new WhitelistEvent { Type = typeValue };
                Whitelist.Events.Add(existingEvent);
            }

            Dictionary<string, object> dataBlock = new Dictionary<string, object>();
            List<string> keys = new List<string>(definitionObject.PropertyNames());

            for (int kk = 0; kk < keys.Count; kk++)
            {
                string key = keys[kk];
                if (!key.Equals(typeKey, StringComparison.InvariantCultureIgnoreCase))
                {
                    object val = definitionObject[key].Value;
                    dataBlock[key] = val;
                }
            }
            if (dataBlock.Count > 0)
                existingEvent.DataBlocks.Add(dataBlock);
        }

        private bool IsEventValid(string eventName, JObject jsonObject)
        {
            WhitelistEvent eventNode = Whitelist.Events.FirstOrDefault(e =>
                e.Type.Equals(eventName, StringComparison.InvariantCultureIgnoreCase));

            if (eventNode != null && IsDataBlockValid(eventNode, jsonObject))
                return true;

            if (eventNode == null)
            {
                eventNode = Whitelist.Events.FirstOrDefault(e =>
                e.Type.Equals("undefined", StringComparison.InvariantCultureIgnoreCase));

                if (eventNode != null && IsDataBlockValid(eventNode, jsonObject))
                    return true;
            }

            return false;
        }

        private bool IsDataBlockValid(WhitelistEvent eventNode, JObject jsonObject)
        {
            if (eventNode.DataBlocks.Count == 0)
                return true;

            foreach (var dataBlock in eventNode.DataBlocks)
            {
                bool allKeyValuePairsMatch = true;

                foreach (var key in dataBlock.Keys)
                {
                    if (!jsonObject.Contains(key))
                    {
                        allKeyValuePairsMatch = false;
                        break;
                    }

                    if (!jsonObject[key].ToString().Trim('"').Equals(dataBlock[key].ToString(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        allKeyValuePairsMatch = false;
                        break;
                    }
                }

                if (allKeyValuePairsMatch)
                    return true;
            }

            return false;
        }
        #endregion
    }
}
