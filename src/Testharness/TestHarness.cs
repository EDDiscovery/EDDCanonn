using EliteDangerousCore.DLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CHash
{
    public partial class TestHarness : Form
    {
        EDDDLLManager mgr = new EDDDLLManager();
        string csharpappdata = @"..\..\..\appdata";

        string csconfig = "csdef";
        string winconfig = "winconfig";

        public TestHarness()
        {
            InitializeComponent();

            EDDDLLAssemblyFinder.AssemblyFindPaths.Add(csharpappdata);
            AppDomain.CurrentDomain.AssemblyResolve += EDDDLLAssemblyFinder.AssemblyResolve;
        }

        public bool RequestHistory(long index, bool isjid, out EDDDLLInterfaces.EDDDLLIF.JournalEntry f)
        {
            EDDDLLInterfaces.EDDDLLIF.JournalEntry nje = MakeJE();
            nje.indexno = (int)index;

            f = nje;

            richTextBox1.Text += "Request history " + index + " " + isjid + Environment.NewLine;
            return true;
        }

        public bool RunAction(string eventname, string paras)
        {
            richTextBox1.Text += "Run action " + eventname + " " + paras + Environment.NewLine;
            return true;
        }

        public string GetShipLoadout(string shipnameorcurrent)
        {
            return "Ship JSON";
        }
        public void AddPanel(string id, Type paneltype, string wintitle, string refname, string description, System.Drawing.Image img)
        {
            System.Diagnostics.Debug.WriteLine($"DLL wanted to add panel {id} {paneltype} {wintitle} {refname} {description}");
        }

        public EDDDLLInterfaces.EDDDLLIF.EDDCallBacks callbacks = new EDDDLLInterfaces.EDDDLLIF.EDDCallBacks();

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (mgr.Count == 0)
            {
                callbacks.ver = 3;
                callbacks.RequestHistory = RequestHistory;
                callbacks.RunAction = RunAction;
                callbacks.GetShipLoadout = GetShipLoadout;
                callbacks.AddPanel = AddPanel;

                string[] options = new string[] { EDDDLLInterfaces.EDDDLLIF.FLAG_HOSTNAME + "EDDiscovery",
                                              EDDDLLInterfaces.EDDDLLIF.FLAG_JOURNALVERSION + EDDDLLInterfaces.EDDDLLIF.JournalVersion,
                                              EDDDLLInterfaces.EDDDLLIF.FLAG_CALLBACKVERSION + EDDDLLInterfaces.EDDDLLIF.DLLCallBackVersion,
                                            };

                //var r = mgr.Load(@"..\..\..\win64dll\bin\debug", "1.2.3.4", new string[] { "HOSTNAME=TESTHARNESS","JOURNALVERSION=2" }, @"c:\code", callbacks, "All");
                string allow = "All";
                var r = mgr.Load(new string[] { @"..\..\..\x64\debug" }, new bool[] { false }, "1.2.3.4", options , callbacks, ref allow, (name) => winconfig, (name, set) => { winconfig = set; });
                richTextBox1.Text += "Win32 DLL Loaded: " + r.Item1 + Environment.NewLine;
                richTextBox1.Text += "Win32 DLL Failed: " + r.Item2 + Environment.NewLine;
                richTextBox1.Text += "Win32 DLL Not Allowed: " + r.Item3 + Environment.NewLine;

                var r2 = mgr.Load(new string[] { csharpappdata }, new bool[] { false }, "1.2.3.4", options, callbacks, ref allow, (name) => csconfig, (name, set) => { csconfig = set; });
                richTextBox1.Text += "CSDLL Loaded: " + r2.Item1 + Environment.NewLine;
                richTextBox1.Text += "CSDLL Failed: " + r2.Item2 + Environment.NewLine;
                richTextBox1.Text += "CSDLL Not Allowed: " + r2.Item3 + Environment.NewLine;
            }
            else
                richTextBox1.Text += "Already loaded" + Environment.NewLine;
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            mgr.UnLoad();
            richTextBox1.Text += "DLL UnLoad" + Environment.NewLine;
        }

        private void buttonNJE_Click(object sender, EventArgs e)
        {
            var nje = MakeJE();
            mgr.NewJournalEntry(nje, false);
            richTextBox1.Text += "NJE" + Environment.NewLine;
        }

        private EDDDLLInterfaces.EDDDLLIF.JournalEntry MakeJE()
        {
            EDDDLLInterfaces.EDDDLLIF.JournalEntry nje = new EDDDLLInterfaces.EDDDLLIF.JournalEntry() { ver = 7, indexno = 19 };

            //v1
            nje.utctime = DateTime.UtcNow.ToString();
            nje.name = "EventSummary";
            nje.info = "Info";
            nje.detailedinfo = "DI";
            nje.materials = new string[2] { "one", "two" };
            nje.commodities = new string[2] { "c-one", "c-two" };
            nje.currentmissions = new string[2] { "m-one", "m-two" };
            nje.systemname = "Sys Fred";
            nje.x = 100.1;
            nje.y = 200.1;
            nje.z = 300.1;
            nje.travelleddistance = 1234.5;
            nje.travelledseconds = 6789;
            nje.islanded = true;
            nje.isdocked = true;
            nje.whereami = "Body";
            nje.shiptype = "Anaconda";
            nje.gamemode = "Open";
            nje.group = "Fred";
            nje.credits = 0xCCC12345678;
            nje.eventid = "FunEvent";
            nje.jid = 0xAAA12345678;
            nje.totalrecords = 2001;

            //v2
            nje.json = "{\"timestamp\"=\"10-20\"}";
            nje.cmdrname = "Buddy";
            nje.cmdrfid = "F19292";
            nje.shipident = "Y-1929";
            nje.shipname = "Julia";
            nje.hullvalue = 0x12345678;
            nje.rebuy =     0x1234000;
            nje.modulesvalue = 0x1234111;
            nje.stored = true;

            //v3
            nje.travelstate = "Travelling";
            nje.microresources = new string[] { "MR1", "MR2", "MR3" };

            // v4
            nje.horizons = false;
            nje.odyssey = true;
            nje.beta = false;

            //v5
            nje.wanted = false;
            nje.bodyapproached = true;
            nje.bookeddropship = false;
            nje.issrv = true;
            nje.isfighter = false;
            nje.onfoot = true;
            nje.bookedtaxi = false;

            nje.bodyname = "Bodyname";
            nje.bodytype = "Bodytype";
            nje.stationname = "stationname";
            nje.stationtype = "stationtype";
            nje.stationfaction = "stationfaction";
            nje.shiptypefd = "shiptypefd";
            nje.oncrewwithcaptain = "Captain Jack";
            nje.shipid = 0xa1a12345678;
            nje.bodyid = 2020;

            // v6

            nje.gameversion = "gamv1";
            nje.gamebuild = "10.2.3.4";

            // v7
            nje.fsdjumpnextsystemaddress = 1234;
            nje.fsdjumpnextsystemname = "Next sys";
            nje.systemaddress = 1234567;
            nje.marketid = 89012;
            nje.fullbodyid = 991234567;
            nje.loan = 1029292;
            nje.assets = 2902992929292;
            nje.currentboost = 2.3;
            nje.visits = 123;
            nje.multiplayer = true;
            nje.insupercruise = true;

            return nje;
        }

        private void buttonUnfilteredJE_Click(object sender, EventArgs e)
        {
            var nje = MakeJE();
            mgr.NewUnfilteredJournalEntry(nje);
            richTextBox1.Text += "Unfiltered NJE" + Environment.NewLine;

        }

        private void buttonAction_Click(object sender, EventArgs e)
        {
            mgr.ActionCommand("All", "Command", new string[] { "one", "two", "three" });
            richTextBox1.Text += "Action" + Environment.NewLine;
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            EDDDLLInterfaces.EDDDLLIF.JournalEntry nje = new EDDDLLInterfaces.EDDDLLIF.JournalEntry() { ver = 3, indexno = 19 };

            nje.utctime = "01/02/03";
            nje.name = "EventSummary";
            nje.info = "Info";
            nje.detailedinfo = "DI";
            nje.materials = new string[2] { "one", "two" };
            nje.commodities = new string[2] { "c-one", "c-two" };
            nje.currentmissions = new string[2] { "m-one", "m-two" };
            nje.systemname = "Sys Fred";
            nje.x = 100.1;
            nje.y = 200.1;
            nje.z = 300.1;
            nje.travelleddistance = 1234.5;
            nje.travelledseconds = 6789;
            nje.islanded = true;
            nje.isdocked = true;
            nje.whereami = "Body";
            nje.shiptype = "Anaconda";
            nje.gamemode = "Open";
            nje.group = "Fred";
            nje.credits = 123456789;
            nje.totalrecords = 2001;
            nje.jid = 101;
            nje.json = "{\"timestamp\"=\"10-20\"}";
            nje.travelstate = "Travelling";
            nje.microresources = new string[] { "MR1", "MR2" };


            mgr.Refresh("Jameson", nje);
            richTextBox1.Text += "Refresh" + Environment.NewLine;

        }

        private void buttonAJE_Click(object sender, EventArgs e)
        {
            EDDDLLInterfaces.EDDDLLIF.JournalEntry nje = new EDDDLLInterfaces.EDDDLLIF.JournalEntry() { ver = 3, indexno = 19 };

            nje.utctime = DateTime.UtcNow.ToString();
            nje.name = "EventSummary";
            nje.info = "Info";
            nje.detailedinfo = "DI";
            nje.materials = new string[2] { "one", "two" };
            nje.commodities = new string[2] { "c-one", "c-two" };
            nje.currentmissions = new string[2] { "m-one", "m-two" };
            nje.systemname = "Sys Fred";
            nje.x = 100.1;
            nje.y = 200.1;
            nje.z = 300.1;
            nje.travelleddistance = 1234.5;
            nje.travelledseconds = 6789;
            nje.islanded = true;
            nje.isdocked = true;
            nje.whereami = "Body";
            nje.shiptype = "Anaconda";
            nje.gamemode = "Open";
            nje.group = "Fred";
            nje.credits = 123456789;
            nje.eventid = "FunEvent";
            nje.totalrecords = 2001;
            nje.jid = 101;
            nje.json = "{\"timestamp\"=\"10-20\"}";
            nje.cmdrname = "Buddy";
            nje.cmdrfid = "F19292";
            nje.shipident = "Y-1929";
            nje.shipname = "Julia";
            nje.hullvalue = 200000;
            nje.modulesvalue = 20000;
            nje.rebuy = 5000;
            nje.stored = false;
            nje.travelstate = "Travelling";
            nje.microresources = new string[] { "MR1", "MR2" };

            mgr.ActionJournalEntry("All",nje);

            richTextBox1.Text += "Action JE" + Environment.NewLine;

        }

        private void buttonUIEvent_Click(object sender, EventArgs e)
        {
            mgr.NewUIEvent("Test json");
        }

        private void buttonConfigcs_Click(object sender, EventArgs e)
        {
            Config("CSharpDLL", ref csconfig);
        }
        private void buttonConfigwin_Click(object sender, EventArgs e)
        {
            Config("Win64DLL",ref winconfig);

        }

        private void Config(string s, ref string istr)
        {
            var caller = mgr.FindCaller(s);
            if (caller != null)
            {
                if (caller.HasConfig())
                {
                    string res = caller.Config(istr,true);
                    richTextBox1.AppendText("Config ret:" + res + Environment.NewLine);
                    istr = res;
                }
                else
                    richTextBox1.AppendText("No config" + Environment.NewLine);
            }

        }

    }
}
