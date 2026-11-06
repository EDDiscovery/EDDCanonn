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
using TestHarness.Properties;

namespace CHash
{
    public partial class TestHarness : Form
    {
        EDDDLLManager DLLManager = new EDDDLLManager();
        string csharpappdata = @"..\..\..\EDDCanonn\bin\debug";

        string csconfig = "csdef";
        string winconfig = "winconfig";

        public TestHarness()
        {
            InitializeComponent();

            EDDDLLAssemblyFinder.AssemblyFindPaths.Add(csharpappdata);
            AppDomain.CurrentDomain.AssemblyResolve += EDDDLLAssemblyFinder.AssemblyResolve;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            buttonStart_Click(null, null);
        }

    
        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (DLLManager.Count == 0)
            {
                callbacks.ver = 3;
                callbacks.RequestHistory = DLLRequestHistory;
                callbacks.RunAction = DLLRunAction;
                callbacks.GetShipLoadout = DLLGetShipLoadout;
                callbacks.AddPanel = DLLAddPanel;
                callbacks.RequestSpanshDump = DLLRequestSpanshDump;
                callbacks.GetGMOs = DLLGetGMOs;

                string[] options = new string[] { EDDDLLInterfaces.EDDDLLIF.FLAG_HOSTNAME + "EDDiscovery",
                                              EDDDLLInterfaces.EDDDLLIF.FLAG_JOURNALVERSION + EDDDLLInterfaces.EDDDLLIF.JournalVersion,
                                              EDDDLLInterfaces.EDDDLLIF.FLAG_CALLBACKVERSION + EDDDLLInterfaces.EDDDLLIF.DLLCallBackVersion,
                                            };

                string eddcannonpath = Path.GetFullPath(Path.Combine(csharpappdata, "EDDCanonnPanel.dll"));

                DLLManager.SetDLLPermission(eddcannonpath, true);

                var r2 = DLLManager.Load(new string[] { csharpappdata }, new bool[] { false }, "20.0.0.99", options, callbacks, (name) => csconfig, (name, set) => { csconfig = set; });
                richTextBox1.Text += "CSDLL Loaded: " + r2.Item1 + Environment.NewLine;
                richTextBox1.Text += "CSDLL Failed: " + r2.Item2 + Environment.NewLine;
                richTextBox1.Text += "CSDLL Not Allowed: " + r2.Item3 + Environment.NewLine;

                buttonStart.Enabled = false;
            }
            else
                richTextBox1.Text += "Already loaded" + Environment.NewLine;
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            DLLManager.UnLoad();
            richTextBox1.Text += "DLL UnLoad" + Environment.NewLine;
        }



        private void buttonNJE_Click(object sender, EventArgs e)
        {
            var nje = MakeJE();
            DLLManager.NewJournalEntry(nje, false);
            richTextBox1.Text += "NJE" + Environment.NewLine;
        }
 
        private void buttonUnfilteredJE_Click(object sender, EventArgs e)
        {
            var nje = MakeJE();
            DLLManager.NewUnfilteredJournalEntry(nje, false);
            richTextBox1.Text += "Unfiltered NJE" + Environment.NewLine;

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

            DLLManager.Refresh("Jameson", nje);
            richTextBox1.Text += "Refresh" + Environment.NewLine;
        }

        private void buttonConfigcs_Click(object sender, EventArgs e)
        {
            Config("CSharpDLL", ref csconfig);
        }
        private void buttonConfigwin_Click(object sender, EventArgs e)
        {
            Config("Win64DLL", ref winconfig);

        }

        private void Config(string s, ref string istr)
        {
            var caller = DLLManager.FindCaller(s);
            if (caller != null)
            {
                if (caller.HasConfig())
                {
                    string res = caller.Config(istr, true);
                    richTextBox1.AppendText("Config ret:" + res + Environment.NewLine);
                    istr = res;
                }
                else
                    richTextBox1.AppendText("No config" + Environment.NewLine);
            }

        }

        Type paneltype = null;
        string panelid = null;
        public void DLLAddPanel(string id, Type paneltype, string wintitle, string refname, string description, System.Drawing.Image img)
        {
            System.Diagnostics.Debug.WriteLine($"DLL wanted to add panel {id} {paneltype} {wintitle} {refname} {description}");
            this.paneltype = paneltype;
            this.panelid = id;
        }
        private void EDDCannon_Click(object sender, EventArgs e)
        {
            if (paneltype != null)
            {
                var pe = (EDDDLLInterfaces.EDDDLLIF.IEDDPanelExtension)Activator.CreateInstance(paneltype, null);
                Control pp = (Control)pe;
                pp.Dock = DockStyle.Fill;
                panelCanonn.Controls.Add(pp);

                string theme = @"{""Name"":""Elite Verdana Small Skinny Scroll"",""form"":""Black"",""button_back"":""#404040"",""button_back2"":""#262626"",""ButtonBackGradientDirection"":90.0,
""button_border"":""#606060"",""button_text"":""Orange"",""ImageButtonsHaveBorders"":false,""ImageButtonsHaveBackground"":false,""combobox_back"":""#404040"",
""combobox_back2"":""#262626"",""ComboBoxBackAndDropDownGradientDirection"":90.0,""combobox_border"":""#606060"",""combobox_text"":""Orange"",""combobox_sliderback"":""#202020"",
""combobox_sliderback2"":""#131313"",""ComboBoxDropDownSliderGradientDirection"":90.0,""combobox_scrollarrowback"":""Orange"",""combobox_scrollarrowback2"":""#996300"",
""combobox_scrollbuttonback"":""DarkOrange"",""combobox_scrollbuttonback2"":""#995400"",""ComboBoxDropDownScrollButtonGradientDirection"":90.0,
""combobox_scrollarrow"":""Orange"",""list_back"":""#404040"",""list_back2"":""#262626"",""ListBoxBackGradientDirection"":90.0,""list_border"":""#606060"",
""list_text"":""Orange"",""listbox_sliderback"":""#202020"",""listbox_sliderback2"":""#131313"",""ListBoxSliderGradientDirection"":90.0,""listbox_scrollarrowback"":""DarkOrange"",
""listbox_scrollarrowback2"":""#995400"",""listbox_scrollbuttonback"":""DarkOrange"",""listbox_scrollbuttonback2"":""#995400"",""ListBoxScrollButtonGradientDirection"":90.0,
""listbox_scrollarrow"":""Orange"",""grid_borderback"":""#B07300"",""grid_bordertext"":""Black"",""grid_cellbackground"":""#202020"",""grid_altcellbackground"":""#202020"",
""grid_celltext"":""Orange"",""grid_altcelltext"":""Orange"",""grid_borderlines"":""DarkOrange"",""grid_sliderback"":""#202020"",""grid_sliderback2"":""#131313"",
""GridSliderGradientDirection"":90.0,""grid_scrollarrowback"":""DarkOrange"",""grid_scrollarrowback2"":""#995400"",""grid_scrollbutton"":""DarkOrange"",
""grid_scrollbuttonback2"":""#995400"",""GridScrollButtonGradientDirection"":90.0,""grid_highlightback"":""#0A5A0A"",""grid_scrollarrow"":""Orange"",
""textbox_back"":""#202020"",""textbox_fore"":""Orange"",""textbox_highlight"":""Red"",""textbox_success"":""Green"",""textbox_border"":""#404040"",""textbox_border2"":""#262626"",
""textbox_sliderback"":""#202020"",""textbox_sliderback2"":""#131313"",""TextBlockSliderGradientDirection"":90.0,""textbox_scrollarrowback"":""DarkOrange"",
""textbox_scrollarrowback2"":""#995400"",""textbox_scrollbutton"":""DarkOrange"",""textbox_scrollbuttonback2"":""#995400"",""TextBlockScrollButtonGradientDirection"":90.0,
""textbox_scrollarrow"":""Orange"",""textbox_dropdownback"":""#404040"",""textbox_dropdownback2"":""#404040"",""TextBlockDropDownBackGradientDirection"":90.0,
""checkbox"":""Orange"",""checkbox_back"":""Orange"",""checkbox_back2"":""#996300"",""CheckBoxBackGradientDirection"":225.0,""checkbox_tick"":""#412121"",
""checkbox_buttontick"":""#404040"",""checkbox_buttontick2"":""#262626"",""CheckBoxButtonGradientDirection"":90.0,""checkbox_border"":""#606060"",""CheckBoxTickSize"":0.75,
""GroupBack"":[""Black"",""Black"",""Black"",""Black""],""GroupBoxGradientDirection"":90.0,""group_text"":""Orange"",""group_borderlines"":""#824700"",""group_borderlines2"":""#4E2B00"",
""TabStripBack"":[""Black"",""Black"",""Black"",""Black""],""TabStripGradientDirection"":0.0,""tabstrip_text"":""Orange"",""tabstrip_selected"":""#404040"",
""TabControlBack"":[""Black"",""Black"",""Black"",""Black""],""TabControlBackGradientDirection"":0.0,""tabcontrol_text"":""Orange"",""tabcontrol_buttonback"":""#404040"",
""tabcontrol_buttonback2"":""#262626"",""TabControlTabGradientDirection"":90.0,""tabcontrol_borderlines"":""DarkOrange"",""tabcontrol_borderlines2"":""#995400"",
""tabcontrol_pageback"":""Black"",""menu_back"":""Black"",""menu_fore"":""Orange"",""menu_dropdownfore"":""Yellow"",""menu_dropdownback"":""DarkOrange"",
""toolstrip_back"":""Black"",""toolstrip_border"":""DarkOrange"",""label"":""Orange"",""s_panel"":""Orange"",""Panel1"":[""Black"",""Black"",""Black"",""Black""]
,""Panel2"":[""#202020"",""#4C0000"",""#660000"",""#202020""],""Panel3"":[""#202020"",""#4C3200"",""#664200"",""#202020""],""Panel4"":[""#202020"",""#4C4C00"",
""#666600"",""#202020""],""Panel5"":[""Black"",""Black"",""Black"",""Black""],""Panel6"":[""Black"",""Black"",""Black"",""Black""],""Panel7"":[""Black"",""Black"",
""Black"",""Black""],""Panel8"":[""Black"",""Black"",""Black"",""Black""],""PanelGradientDirection"":[0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0],""transparentcolorkey"":""Green"",
""chart1"":""Green"",""chart2"":""Red"",""chart3"":""Blue"",""chart4"":""Orange"",""chart5"":""Purple"",""chart6"":""Aqua"",""chart7"":""Violet"",""chart8"":""Brown"",
""windowsframe"":false,""formopacity"":100.0,""fontname"":""Verdana"",""fontsize"":8.0,""FontStyle"":""Regular"",""buttonstyle"":""Gradient"",""textboxborderstyle"":""Colour"",
""SkinnyScrollBars"":true,""SkinnyScrollBarsHaveButtons"":false,""DialogFontScaling"":0.800000011920929,""MouseOverScaling"":1.2999999523162842,
""MouseSelectedScaling"":1.6000000238418579,""DisabledScaling"":0.5}";

                var callbacks = new EDDDLLInterfaces.EDDDLLIF.EDDPanelCallbacks();
                callbacks.ver = 1;
                pe.Initialise(callbacks, 0, theme, "");

            }
        }
    }
}
