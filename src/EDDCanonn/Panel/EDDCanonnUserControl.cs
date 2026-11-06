/******************************************************************************
 * 
 * Copyright 2022-2026 EDDiscovery development team
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
using static EDDDLLInterfaces.EDDDLLIF;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using QuickJSON;
using System.Linq;
using EDDCanonnPanel.Base;
using KdTree;
using KdTree.Math;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using System.Text.RegularExpressions;
using System.Net;
using EDDCanonnPanel.Utility;
using System.ComponentModel;
using System.Reflection;
using System.Globalization;
using ExtendedControls;


namespace EDDCanonnPanel
{
    partial class EDDCanonnUserControl : UserControl, IEDDPanelExtension
    {

        private readonly ActionDataHandler dataHandler;
        private EDDPanelCallbacks PanelCallBack;
        private EDDCallBacks DLLCallBack;
        private bool isAbort = false;

        public EDDCanonnUserControl()
        {
            InitializeComponent();
            dataHandler = new ActionDataHandler();
            AutoScaleMode = AutoScaleMode.Inherit;
        }

        #region EDD Panel Interface

        public void Initialise(EDDPanelCallbacks callbacks, int displayid, string themeasjson, string configuration)
        {
            DLLCallBack = CanonnEDDClass.DLLCallBack;
            PanelCallBack = callbacks;
            ThemeChanged(themeasjson);

            UpdateMainFieldsWithMessage("Start Up...");
            try
            {
                if (CanonnUtil.InstanceCount > 0)
                {
                    CanonnUtil.InstanceCount++;
                    //Abort if a Canonn Panel already exists.
                    UpdateMainFieldsWithMessage("Multiple Canonn Panels");
                    isAbort = true;
                    return;
                }
                else
                    CanonnUtil.InstanceCount++;

                linkLabelEDDCanonn.Text = "EDDCanonn v" + CanonnUtil.V;

                InitializeNews();
                InitializeCodex();
                InitializePatrols();

                // callback to see if history is ready, if it does, do the spansh request on the current system

                if (DLLCallBack.RequestHistory(1, false, out JournalEntry je) == true) 
                {
                    CanonnUtil.UserName = je.cmdrname;
                    DLLCallBack.RequestSpanshDump(RequestTag.OnStart, this, "", 0, true, false, null);
                    return;
                }
                else
                {
                    // else set up a wait task until we see the history is set via the 

                    dataHandler.StartTaskAsync( (token) =>
                    {
                        while (!historyset) //We wait until the history has been loaded.
                        {
                            token.ThrowIfCancellationRequested(); //We abort here if a cancellation was requested.
                            Task.Delay(250, token).Wait();
                        }
                        SafeInvoke(() =>
                        {
                            DLLCallBack.RequestSpanshDump(RequestTag.OnStart, this, "", 0, true, false, null);
                        });
                        return;
                    },
                    ex =>
                    {
                        string error = $"EDDCanonn: Unexpected error in \"Wait for History\" Thread: {ex}";
                        CanonnLogging.Instance.Log(error);
                    },
                        "Startup : History-Listener"
                    );
                }
            }
            catch (Exception ex)
            {
                string error = $"EDDCanonn: Error during Startup: {ex}";
                CanonnLogging.Instance.Log(error);
            }
        }

        public bool SupportTransparency => false;
        public bool DefaultTransparent => false;

        public void SetTransparency(bool ison, Color curcol)
        {
            this.BackColor = curcol;
        }

        public void TransparencyModeChanged(bool on)
        {
        }

        public void LoadLayout()
        {
            PanelCallBack.LoadGridLayout(dataGridPatrol);
            PanelCallBack.LoadGridLayout(dataGridViewData);
            PanelCallBack.LoadGridLayout(dataGridViewBio);
            PanelCallBack.LoadGridLayout(dataGridViewRing);
            PanelCallBack.LoadGridLayout(dataGridViewGMO);
        }

        public void InitialDisplay()
        {
        }

        void IEDDPanelExtension.CursorChanged(JournalEntry je)
        {
        }

        public void Closing()
        {
            CanonnUtil.InstanceCount--;
            dataHandler.Closing();
        }

        public bool AllowClose()
        {
            return true;
        }

        public string HelpKeyOrAddress()
        {
            return LinkUtil.EDDCanonnGitHub;
        }

        public void ControlTextVisibleChange(bool on)
        {
        }

        #endregion

        #region EDD Event Interface

        private bool historyset = false;
        public void HistoryChange(int count, string commander, bool beta, bool legacy)
        {
            if (isAbort)
                return;
            if (!historyset)
            {
                historyset = true;
                CanonnUtil.UserName = commander;
            }
        }

        //If false, jornals are ignored. Only released after the 'OnStart' data result.
        private bool _journalLock = false;
        //If false, events are held back.
        private bool _eventLock = false;
        public void NewUnfilteredJournal(JournalEntry je)
        {
            if (isAbort || !_journalLock)
                return;

            dataHandler.StartTaskAsync(
            (token) =>
            {
                JObject o = je.json.JSONParseObject();
                string eventId = je.eventid;

                if (eventId.Equals("FSDJump") || eventId.Equals("Location"))
                {
                    SafeBeginInvoke(() =>
                    {
                        _eventLock = false; // As long as we make a callback, we hold back the events (canonn events excluded).
                        ResetSystemData();
                        DLLCallBack.RequestSpanshDump(o, this, je.systemname, je.systemaddress, true, false, null);
                    });

                    // The callback may take some time. That's why we give the user some feedback in an early stage.
                    SafeBeginInvoke(() =>
                    {
                        textBoxSystem.Text = je.systemname;
                        textBoxBodyCount.Text = "Fetch...";
                    });

                    return;
                }

                while (!_eventLock) // Wait until a 'DataResult' has been completed.
                {
                    token.ThrowIfCancellationRequested();
                    Task.Delay(250, token).Wait();
                }

                ProcessEvent(je);
            },
            ex =>
            {
                string error = $"EDDCanonn: Error processing JournalEntry: {ex}";
                CanonnLogging.Instance.Log(error);
            },
            "NewUnfilteredJournal: " + je.eventid + " in system: " + je.systemname);
        }

        public void NewFilteredJournal(JournalEntry je)
        {
        }

        public void NewUIEvent(string jsonui)
        {
        }

        public void NewTarget(Tuple<string, double, double, double> target)
        {
        }

        #endregion

        #region EDD Panel Interface Other callbacks

        public void ThemeChanged(string themeasjson)
        {
            JObject theme = JObject.Parse(themeasjson);
            Theme thm = Theme.FromJSON(theme);
            thm.ApplyStd(this);
        }

        public void ScreenShotCaptured(string file, Size s)
        {
        }

        #endregion


        #region UC interaction with CanonnEDDClass for DLL

        public enum RequestTag
        {
            OnStart,
            ReFetch
        };


        // callback from EDD with data.. Passed by CannonEDDClass back to us
        public void DataResultCallBackFromDLLRequest(object requestTag, string data)
        {
            try
            {
                if (requestTag == null || data == null)
                    return; //No tag or data, nothing to process.

                JObject o = data.JSONParse().Object();
                if (!(requestTag is RequestTag || requestTag is JObject))
                    return; //Invalid tag format.

                CanonnLogging.Instance.Log(Environment.NewLine + "=== DataResult ===" + Environment.NewLine + o.ToString(true, "  ") ?? "none" + Environment.NewLine);

                dataHandler.StartTaskAsync((token) =>
                {
                    if (requestTag is RequestTag rt)
                    {
                        if (rt.Equals(RequestTag.OnStart) || rt.Equals(RequestTag.ReFetch)) //Triggered by panel creation.
                        {
                            lock (_lockSystemData)
                                ProcessSpanshDump(o);
                            SafeInvoke(() => this.Enabled = true);
                            _eventLock = _journalLock = true; //Plugin startup complete.
                            if (systemData == null)
                                lock (_lockSystemData)
                                    systemData = new SystemData();
                        }
                        else if (false)
                        {
                            //...
                        }
                    }
                    else if (requestTag is JObject jb && new[] { "Location", "FSDJump" }.Contains(jb["event"].StrNull()))
                    {
                        lock (_lockSystemData)
                        {
                            ProcessSpanshDump(o);
                            if (systemData == null)
                                ProcessNewSystem(jb);
                        }
                        _eventLock = true; //Process collected events.
                    }

                    UpdateUI(true);
                    UpdatePatrols();
                },
                ex =>
                {
                    string error = $"EDDCanonn: Error processing Systemdata: {ex}";
                    CanonnLogging.Instance.Log(error);
                },
                    "DataResult"
                );
            }
            catch (Exception ex)
            {
                CanonnLogging.Instance.Log($"EDDCanonn: Unexpected error in DataResult: {ex}");
            }
        }


        #endregion

        #region UpdateUI


        // Update due to Data callback from spansh etc request
        // Update due to new system from events

        private void UpdateUI(bool jumped = false) //Updates all UI events. Except for the patrols.
        {
            dataHandler.StartTaskAsync(
                (token) =>
                {
                    try
                    {
                        while (systemData == null) //We have to wait until the system data has been loaded.
                        {
                            token.ThrowIfCancellationRequested(); //We abort here if a cancellation was requested.
                            Task.Delay(250, token).Wait();
                        }

                        SystemData system = DeepCopySystemData();

                        if (system == null) //Ensure system data is valid before updating UI.
                            return;
                        UpdateSystemBarFields(system, jumped);
                        UpdateTabControl(system, jumped);
                        UpdateTabControlAlternateGrids(system);
                    }
                    catch (Exception ex)
                    {
                        string error = $"EDDCanonn: Error during UpdateUI: {ex}";
                        CanonnLogging.Instance.Log(error);
                    }
                },
                ex =>
                {
                    string error = $"EDDCanonn: Error during UpdateUI execution: {ex}";
                    CanonnLogging.Instance.Log(error);
                },
                "UpdateUI"
            );
        }

        #endregion

        #region Invokers

        private void SafeBeginInvoke(Action action)
        {
            if (this.IsHandleCreated && !this.IsDisposed)
            {
                try
                {
                    BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            action();
                        }
                        catch (Exception ex)
                        {
                            string error = $"EDDCanonn: UI update failed in invoked action (SafeBeginInvoke): {ex}";
                            CanonnLogging.Instance.Log(error);
                        }
                    }));
                }
                catch (Exception ex)
                {
                    string error = $"EDDCanonn: UI update failed in SafeBeginInvoke: {ex}";
                    CanonnLogging.Instance.Log(error);
                }
            }
        }

        private void SafeInvoke(Action action)
        {
            if (this.IsHandleCreated && !this.IsDisposed)
            {
                try
                {
                    Invoke(new Action(() =>
                    {
                        try
                        {
                            action();
                        }
                        catch (Exception ex)
                        {
                            string error = $"EDDCanonn: UI update failed in invoked action (SafeInvoke): {ex}";
                            CanonnLogging.Instance.Log(error);
                        }
                    }));
                }
                catch (Exception ex)
                {
                    string error = $"EDDCanonn: UI update failed in SafeInvoke: {ex}";
                    CanonnLogging.Instance.Log(error);
                }
            }
        }

        #endregion


        #region Notify - list of items by dictionary added to the Data grid in tab control

        private enum NType
        {
            [Description("This system is not yet known to spansh.")]
            systemNotKnown,
            [Description("Spansh data may be incorrect. FSS all bodies.")]
            spanshIncorrect,
            [Description("A GMO is available for this system.")]
            gmoAvailable,
            [Description("D-Scan is required.")]
            dscanRequired
        }
        private Dictionary<NType, DataGridViewRow> notifications;

        private string NTypeDescription(NType type)
        {
            return
                typeof(NType)
                        .GetField(type.ToString())
                        ?.GetCustomAttribute<DescriptionAttribute>()
                        ?.Description;
        }

        // this DGV gets added to Tab Data ..
        private Dictionary<NType, DataGridViewRow> DataGridNotifications
        {
            get
            {
                lock (_lockSystemData)
                {
                    if (notifications == null)
                        notifications = new Dictionary<NType, DataGridViewRow>();
                    return notifications;
                }
            }
        }

        #endregion

        #region UI Helper 

        private void UpdateMainFieldsWithMessage(String arg)
            {
            SafeBeginInvoke(() =>
            {
                textBoxSystem.Text = arg;
                ExtComboBoxRange.Text = arg;
                ExtComboBoxPatrol.Text = arg;
                textBoxNews.Text = arg;
            });
            }
        #endregion


        private void LSY_Click(object sender, EventArgs e)
        {
            SystemData system = DeepCopySystemData();

            CanonnLogging.Instance.Log(Environment.NewLine + "=== System ToString ===" + Environment.NewLine + system.ToString() ?? "none" + Environment.NewLine);
        }

    }
}
