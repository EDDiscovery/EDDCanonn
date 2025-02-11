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

using System.Drawing;
using System.Windows.Forms;
using ExtendedControls;
using QuickJSON;

namespace EDDCanonn
{
    partial class EDDCanonnUserControl
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.RichTextBox richTextBox1;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void setTheme(string themeasjson)
        {
            JObject theme = themeasjson.JSONParse().Object();

            Color form = FromJson(theme["form"]);

            Color ButtonTextColor = FromJson(theme["button_text"]);
            Color ButtonBackColor = FromJson(theme["button_back"]);
            Color ButtonBorderColor = FromJson(theme["button_border"]);

            Color TextBackColor = FromJson(theme["textbox_back"]);
            Color TextBlockColor = FromJson(theme["textbox_fore"]);
            Color TextBlockBorderColor = FromJson(theme["textbox_border"]);

            Color TabcontrolBorder = FromJson(theme["tabcontrol_borderlines"]);
            Color GridScrollButton = FromJson(theme["grid_scrollbutton"]);
            Color GridBorderLines = FromJson(theme["grid_borderlines"]);
            Color GridSliderBack = FromJson(theme["grid_sliderback"]);
            Color GridScrollArrow = FromJson(theme["grid_scrollarrow"]);

            Color TextBlockScrollButton = FromJson(theme["textbox_scrollbutton"]);
            Color TextBlockSliderBack = FromJson(theme["textbox_sliderback"]);

            Color CheckBox = FromJson(theme["checkbox"]);
            Color CheckBoxTick = FromJson(theme["checkbox_tick"]);
            Color c1 = GridScrollButton;
            string buttonstyle = theme["buttonstyle"].Str();

            const float mouseoverscaling = 1.3F;
            const float mouseselectedscaling = 1.5F;

            this.BackColor = form;

            FlatStyle fs = buttonstyle.Equals("System") ? FlatStyle.System : FlatStyle.Popup;

            // 
            // extTabControlData
            // 
            extTabControlData.FlatStyle = fs;
            extTabControlData.TabStyle = new TabStyleAngled();
            extTabControlData.TabControlBorderColor = TabcontrolBorder;
            extTabControlData.TabControlBorderBrightColor = TabcontrolBorder;
            extTabControlData.TabNotSelectedBorderColor = TabcontrolBorder;
            extTabControlData.TabNotSelectedColor = ButtonBackColor;
            extTabControlData.TabSelectedColor = ButtonBackColor.Multiply(mouseselectedscaling);
            extTabControlData.TabMouseOverColor = ButtonBackColor.Multiply(mouseoverscaling);
            extTabControlData.TextSelectedColor = ButtonTextColor;
            extTabControlData.TextNotSelectedColor = ButtonTextColor.Multiply(0.8F);
            extTabControlData.SetStyle(fs, new TabStyleAngled());
            // 
            // TestWhitelist
            // 
            TestWhitelist.ForeColor = ButtonTextColor;
            TestWhitelist.ButtonColorScaling = TestWhitelist.ButtonDisabledScaling = 0.5F;
            TestWhitelist.FlatAppearance.BorderColor = (TestWhitelist.Image != null) ? form : ButtonBorderColor;
            TestWhitelist.FlatAppearance.BorderSize = 1;
            TestWhitelist.FlatAppearance.MouseOverBackColor = ButtonBackColor.Multiply(mouseoverscaling);
            TestWhitelist.FlatAppearance.MouseDownBackColor = ButtonBackColor.Multiply(mouseselectedscaling);
            TestWhitelist.FlatStyle = fs;
            // 
            // LogWhitelist
            // 
            LogWhitelist.ForeColor = ButtonTextColor;
            LogWhitelist.ButtonColorScaling = LogWhitelist.ButtonDisabledScaling = 0.5F;
            LogWhitelist.FlatAppearance.BorderColor = (LogWhitelist.Image != null) ? form : ButtonBorderColor;
            LogWhitelist.FlatAppearance.BorderSize = 1;
            LogWhitelist.FlatAppearance.MouseOverBackColor = ButtonBackColor.Multiply(mouseoverscaling);
            LogWhitelist.FlatAppearance.MouseDownBackColor = ButtonBackColor.Multiply(mouseselectedscaling);
            LogWhitelist.FlatStyle = fs;
            // 
            // ClearDebugLog
            // 
            ClearDebugLog.ForeColor = ButtonTextColor;
            ClearDebugLog.ButtonColorScaling = ClearDebugLog.ButtonDisabledScaling = 0.5F;
            ClearDebugLog.FlatAppearance.BorderColor = (ClearDebugLog.Image != null) ? form : ButtonBorderColor;
            ClearDebugLog.FlatAppearance.BorderSize = 1;
            ClearDebugLog.FlatAppearance.MouseOverBackColor = ButtonBackColor.Multiply(mouseoverscaling);
            ClearDebugLog.FlatAppearance.MouseDownBackColor = ButtonBackColor.Multiply(mouseselectedscaling);
            ClearDebugLog.FlatStyle = fs;
            // 
            // buttonPrevNews
            // 
            buttonPrevNews.ForeColor = ButtonTextColor;
            buttonPrevNews.ButtonColorScaling = buttonPrevNews.ButtonDisabledScaling = 0.5F;
            buttonPrevNews.FlatAppearance.BorderColor = (buttonPrevNews.Image != null) ? form : ButtonBorderColor;
            buttonPrevNews.FlatAppearance.BorderSize = 1;
            buttonPrevNews.FlatAppearance.MouseOverBackColor = ButtonBackColor.Multiply(mouseoverscaling);
            buttonPrevNews.FlatAppearance.MouseDownBackColor = ButtonBackColor.Multiply(mouseselectedscaling);
            buttonPrevNews.FlatStyle = fs;
            // 
            // buttonNextNews
            // 
            buttonNextNews.ForeColor = ButtonTextColor;
            buttonNextNews.ButtonColorScaling = buttonNextNews.ButtonDisabledScaling = 0.5F;
            buttonNextNews.FlatAppearance.BorderColor = (buttonNextNews.Image != null) ? form : ButtonBorderColor;
            buttonNextNews.FlatAppearance.BorderSize = 1;
            buttonNextNews.FlatAppearance.MouseOverBackColor = ButtonBackColor.Multiply(mouseoverscaling);
            buttonNextNews.FlatAppearance.MouseDownBackColor = ButtonBackColor.Multiply(mouseselectedscaling);
            buttonNextNews.FlatStyle = fs;
            // 
            // LogSystem
            // 
            LogSystem.ForeColor = ButtonTextColor;
            LogSystem.ButtonColorScaling = LogSystem.ButtonDisabledScaling = 0.5F;
            LogSystem.FlatAppearance.BorderColor = (LogSystem.Image != null) ? form : ButtonBorderColor;
            LogSystem.FlatAppearance.BorderSize = 1;
            LogSystem.FlatAppearance.MouseOverBackColor = ButtonBackColor.Multiply(mouseoverscaling);
            LogSystem.FlatAppearance.MouseDownBackColor = ButtonBackColor.Multiply(mouseselectedscaling);
            LogSystem.FlatStyle = fs;
            // 
            // buttonCreatePatrol
            // 
            buttonCreatePatrol.ForeColor = ButtonTextColor;
            buttonCreatePatrol.ButtonColorScaling = buttonCreatePatrol.ButtonDisabledScaling = 0.5F;
            buttonCreatePatrol.FlatAppearance.BorderColor = (buttonCreatePatrol.Image != null) ? form : ButtonBorderColor;
            buttonCreatePatrol.FlatAppearance.BorderSize = 1;
            buttonCreatePatrol.FlatAppearance.MouseOverBackColor = ButtonBackColor.Multiply(mouseoverscaling);
            buttonCreatePatrol.FlatAppearance.MouseDownBackColor = ButtonBackColor.Multiply(mouseselectedscaling);
            buttonCreatePatrol.FlatStyle = fs;
            // 
            // CallSystem
            // 
            CallSystem.ForeColor = ButtonTextColor;
            CallSystem.ButtonColorScaling = CallSystem.ButtonDisabledScaling = 0.5F;
            CallSystem.FlatAppearance.BorderColor = (CallSystem.Image != null) ? form : ButtonBorderColor;
            CallSystem.FlatAppearance.BorderSize = 1;
            CallSystem.FlatAppearance.MouseOverBackColor = ButtonBackColor.Multiply(mouseoverscaling);
            CallSystem.FlatAppearance.MouseDownBackColor = ButtonBackColor.Multiply(mouseselectedscaling);
            CallSystem.FlatStyle = fs;
            // 
            // extScrollBarPatrol
            // 
            extScrollBarPatrol.BackColor = form;
            extScrollBarPatrol.SliderColor = GridSliderBack;
            extScrollBarPatrol.BorderColor = GridBorderLines;
            extScrollBarPatrol.BackColor = form;
            extScrollBarPatrol.SliderColor = GridSliderBack;
            extScrollBarPatrol.BorderColor = extScrollBarPatrol.ThumbBorderColor =
            extScrollBarPatrol.ArrowBorderColor = GridBorderLines;
            extScrollBarPatrol.ArrowButtonColor = extScrollBarPatrol.ThumbButtonColor = c1;
            extScrollBarPatrol.MouseOverButtonColor = c1.Multiply(mouseoverscaling);
            extScrollBarPatrol.MousePressedButtonColor = c1.Multiply(mouseselectedscaling);
            extScrollBarPatrol.ForeColor = GridScrollArrow;
            extScrollBarPatrol.FlatStyle = fs;
            // 
            // extScrollBarAuto
            // 
            extScrollBarAuto.BackColor = form;
            extScrollBarAuto.SliderColor = GridSliderBack;
            extScrollBarAuto.BorderColor = GridBorderLines;
            extScrollBarAuto.BackColor = form;
            extScrollBarAuto.SliderColor = GridSliderBack;
            extScrollBarAuto.BorderColor = extScrollBarAuto.ThumbBorderColor =
            extScrollBarAuto.ArrowBorderColor = GridBorderLines;
            extScrollBarAuto.ArrowButtonColor = extScrollBarAuto.ThumbButtonColor = c1;
            extScrollBarAuto.MouseOverButtonColor = c1.Multiply(mouseoverscaling);
            extScrollBarAuto.MousePressedButtonColor = c1.Multiply(mouseselectedscaling);
            extScrollBarAuto.ForeColor = GridScrollArrow;
            extScrollBarAuto.FlatStyle = fs;
            // 
            // extScrollBarData
            // 
            extScrollBarData.BackColor = form;
            extScrollBarData.SliderColor = GridSliderBack;
            extScrollBarData.BorderColor = GridBorderLines;
            extScrollBarData.BackColor = form;
            extScrollBarData.SliderColor = GridSliderBack;
            extScrollBarData.BorderColor = extScrollBarData.ThumbBorderColor =
            extScrollBarData.ArrowBorderColor = GridBorderLines;
            extScrollBarData.ArrowButtonColor = extScrollBarData.ThumbButtonColor = c1;
            extScrollBarData.MouseOverButtonColor = c1.Multiply(mouseoverscaling);
            extScrollBarData.MousePressedButtonColor = c1.Multiply(mouseselectedscaling);
            extScrollBarData.ForeColor = GridScrollArrow;
            extScrollBarData.FlatStyle = fs;
            // 
            // extScrollBarBio
            // 
            extScrollBarBio.BackColor = form;
            extScrollBarBio.SliderColor = GridSliderBack;
            extScrollBarBio.BorderColor = GridBorderLines;
            extScrollBarBio.BackColor = form;
            extScrollBarBio.SliderColor = GridSliderBack;
            extScrollBarBio.BorderColor = extScrollBarBio.ThumbBorderColor =
            extScrollBarBio.ArrowBorderColor = GridBorderLines;
            extScrollBarBio.ArrowButtonColor = extScrollBarBio.ThumbButtonColor = c1;
            extScrollBarBio.MouseOverButtonColor = c1.Multiply(mouseoverscaling);
            extScrollBarBio.MousePressedButtonColor = c1.Multiply(mouseselectedscaling);
            extScrollBarBio.ForeColor = GridScrollArrow;
            extScrollBarBio.FlatStyle = fs;
            // 
            // extScrollBarGeo
            // 
            extScrollBarGeo.BackColor = form;
            extScrollBarGeo.SliderColor = GridSliderBack;
            extScrollBarGeo.BorderColor = GridBorderLines;
            extScrollBarGeo.BackColor = form;
            extScrollBarGeo.SliderColor = GridSliderBack;
            extScrollBarGeo.BorderColor = extScrollBarGeo.ThumbBorderColor =
            extScrollBarGeo.ArrowBorderColor = GridBorderLines;
            extScrollBarGeo.ArrowButtonColor = extScrollBarGeo.ThumbButtonColor = c1;
            extScrollBarGeo.MouseOverButtonColor = c1.Multiply(mouseoverscaling);
            extScrollBarGeo.MousePressedButtonColor = c1.Multiply(mouseselectedscaling);
            extScrollBarGeo.ForeColor = GridScrollArrow;
            extScrollBarGeo.FlatStyle = fs;
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            ExtendedControls.TabStyleSquare tabStyleSquare1 = new ExtendedControls.TabStyleSquare();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.textBoxSystem = new System.Windows.Forms.TextBox();
            this.labelSysName = new System.Windows.Forms.Label();
            this.DebugLog = new System.Windows.Forms.TextBox();
            this.labelSystemCount = new System.Windows.Forms.Label();
            this.textBoxBodyCount = new System.Windows.Forms.TextBox();
            this.tableLayoutSystem = new System.Windows.Forms.TableLayoutPanel();
            this.extTabControlData = new ExtendedControls.ExtTabControl();
            this.tabAuto = new System.Windows.Forms.TabPage();
            this.extPanelDataGridViewScrollAuto = new ExtendedControls.ExtPanelDataGridViewScroll();
            this.extScrollBarAuto = new ExtendedControls.ExtScrollBar();
            this.dataGridViewAuto = new System.Windows.Forms.DataGridView();
            this.tabData = new System.Windows.Forms.TabPage();
            this.extPanelDataGridViewScrollData = new ExtendedControls.ExtPanelDataGridViewScroll();
            this.extScrollBarData = new ExtendedControls.ExtScrollBar();
            this.dataGridViewData = new System.Windows.Forms.DataGridView();
            this.ColumnData0 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnData1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnData2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnData3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnData4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabBio = new System.Windows.Forms.TabPage();
            this.extPanelDataGridViewScrollBio = new ExtendedControls.ExtPanelDataGridViewScroll();
            this.extScrollBarBio = new ExtendedControls.ExtScrollBar();
            this.dataGridViewBio = new System.Windows.Forms.DataGridView();
            this.ColumnBio0 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnBio1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnBio2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnBio3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnBio4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabGeo = new System.Windows.Forms.TabPage();
            this.dataGridViewGeo = new System.Windows.Forms.DataGridView();
            this.ColumnGeo0 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnGeo1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnGeo2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnGeo3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnGeo4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabRingSurvey = new System.Windows.Forms.TabPage();
            this.tabGmo = new System.Windows.Forms.TabPage();
            this.tabDebug = new System.Windows.Forms.TabPage();
            this.CallSystem = new ExtendedControls.ExtButton();
            this.LogSystem = new ExtendedControls.ExtButton();
            this.ClearDebugLog = new ExtendedControls.ExtButton();
            this.TestWhitelist = new ExtendedControls.ExtButton();
            this.LogWhitelist = new ExtendedControls.ExtButton();
            this.dataGridPatrol = new System.Windows.Forms.DataGridView();
            this.Patrol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Instructions = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Distance = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Sysname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PatrolUrl = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.CopySystem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenUrl = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPatrol = new System.Windows.Forms.TableLayoutPanel();
            this.menuStripRange = new System.Windows.Forms.MenuStrip();
            this.toolStripRange = new System.Windows.Forms.ToolStripComboBox();
            this.menuStripPatrol = new System.Windows.Forms.MenuStrip();
            this.toolStripPatrol = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripComboBox2 = new System.Windows.Forms.ToolStripComboBox();
            this.buttonCreatePatrol = new ExtendedControls.ExtButton();
            this.tableLayoutNews = new System.Windows.Forms.TableLayoutPanel();
            this.labelNews = new System.Windows.Forms.Label();
            this.buttonNextNews = new ExtendedControls.ExtButton();
            this.textBoxNews = new System.Windows.Forms.TextBox();
            this.buttonPrevNews = new ExtendedControls.ExtButton();
            this.extPanelDataGridViewScrollPatrol = new ExtendedControls.ExtPanelDataGridViewScroll();
            this.extScrollBarPatrol = new ExtendedControls.ExtScrollBar();
            this.extPanelDataGridViewScrollGeo = new ExtendedControls.ExtPanelDataGridViewScroll();
            this.extScrollBarGeo = new ExtendedControls.ExtScrollBar();
            this.tableLayoutSystem.SuspendLayout();
            this.extTabControlData.SuspendLayout();
            this.tabAuto.SuspendLayout();
            this.extPanelDataGridViewScrollAuto.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAuto)).BeginInit();
            this.tabData.SuspendLayout();
            this.extPanelDataGridViewScrollData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewData)).BeginInit();
            this.tabBio.SuspendLayout();
            this.extPanelDataGridViewScrollBio.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewBio)).BeginInit();
            this.tabGeo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewGeo)).BeginInit();
            this.tabDebug.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridPatrol)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.tableLayoutPatrol.SuspendLayout();
            this.menuStripRange.SuspendLayout();
            this.menuStripPatrol.SuspendLayout();
            this.tableLayoutNews.SuspendLayout();
            this.extPanelDataGridViewScrollPatrol.SuspendLayout();
            this.extPanelDataGridViewScrollGeo.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxSystem
            // 
            this.textBoxSystem.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textBoxSystem.Location = new System.Drawing.Point(66, 4);
            this.textBoxSystem.Name = "textBoxSystem";
            this.textBoxSystem.ReadOnly = true;
            this.textBoxSystem.Size = new System.Drawing.Size(163, 20);
            this.textBoxSystem.TabIndex = 3;
            // 
            // labelSysName
            // 
            this.labelSysName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelSysName.AutoSize = true;
            this.labelSysName.Location = new System.Drawing.Point(3, 7);
            this.labelSysName.Name = "labelSysName";
            this.labelSysName.Size = new System.Drawing.Size(41, 13);
            this.labelSysName.TabIndex = 4;
            this.labelSysName.Text = "System";
            // 
            // DebugLog
            // 
            this.DebugLog.Location = new System.Drawing.Point(6, 23);
            this.DebugLog.Multiline = true;
            this.DebugLog.Name = "DebugLog";
            this.DebugLog.ReadOnly = true;
            this.DebugLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.DebugLog.Size = new System.Drawing.Size(344, 101);
            this.DebugLog.TabIndex = 5;
            // 
            // labelSystemCount
            // 
            this.labelSystemCount.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelSystemCount.AutoSize = true;
            this.labelSystemCount.Location = new System.Drawing.Point(235, 1);
            this.labelSystemCount.Name = "labelSystemCount";
            this.labelSystemCount.Size = new System.Drawing.Size(35, 26);
            this.labelSystemCount.TabIndex = 6;
            this.labelSystemCount.Text = "Body Count";
            // 
            // textBoxBodyCount
            // 
            this.textBoxBodyCount.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textBoxBodyCount.Location = new System.Drawing.Point(279, 4);
            this.textBoxBodyCount.Name = "textBoxBodyCount";
            this.textBoxBodyCount.ReadOnly = true;
            this.textBoxBodyCount.Size = new System.Drawing.Size(82, 20);
            this.textBoxBodyCount.TabIndex = 7;
            this.textBoxBodyCount.Text = "#/#";
            // 
            // tableLayoutSystem
            // 
            this.tableLayoutSystem.ColumnCount = 4;
            this.tableLayoutSystem.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27.15517F));
            this.tableLayoutSystem.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 72.84483F));
            this.tableLayoutSystem.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.tableLayoutSystem.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 87F));
            this.tableLayoutSystem.Controls.Add(this.textBoxBodyCount, 3, 0);
            this.tableLayoutSystem.Controls.Add(this.labelSystemCount, 2, 0);
            this.tableLayoutSystem.Controls.Add(this.textBoxSystem, 1, 0);
            this.tableLayoutSystem.Controls.Add(this.labelSysName, 0, 0);
            this.tableLayoutSystem.Location = new System.Drawing.Point(20, 3);
            this.tableLayoutSystem.Name = "tableLayoutSystem";
            this.tableLayoutSystem.RowCount = 1;
            this.tableLayoutSystem.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 27F));
            this.tableLayoutSystem.Size = new System.Drawing.Size(364, 28);
            this.tableLayoutSystem.TabIndex = 8;
            // 
            // extTabControlData
            // 
            this.extTabControlData.AllowDragReorder = false;
            this.extTabControlData.Controls.Add(this.tabAuto);
            this.extTabControlData.Controls.Add(this.tabData);
            this.extTabControlData.Controls.Add(this.tabBio);
            this.extTabControlData.Controls.Add(this.tabGeo);
            this.extTabControlData.Controls.Add(this.tabRingSurvey);
            this.extTabControlData.Controls.Add(this.tabGmo);
            this.extTabControlData.Controls.Add(this.tabDebug);
            this.extTabControlData.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.extTabControlData.Location = new System.Drawing.Point(20, 37);
            this.extTabControlData.Multiline = true;
            this.extTabControlData.Name = "extTabControlData";
            this.extTabControlData.SelectedIndex = 0;
            this.extTabControlData.Size = new System.Drawing.Size(364, 159);
            this.extTabControlData.TabBackgroundColor = System.Drawing.Color.Transparent;
            this.extTabControlData.TabColorScaling = 0.5F;
            this.extTabControlData.TabControlBorderBrightColor = System.Drawing.Color.LightGray;
            this.extTabControlData.TabControlBorderColor = System.Drawing.Color.DarkGray;
            this.extTabControlData.TabDisabledScaling = 0.5F;
            this.extTabControlData.TabIndex = 11;
            this.extTabControlData.TabMouseOverColor = System.Drawing.Color.White;
            this.extTabControlData.TabNotSelectedBorderColor = System.Drawing.Color.Gray;
            this.extTabControlData.TabNotSelectedColor = System.Drawing.Color.Gray;
            this.extTabControlData.TabSelectedColor = System.Drawing.Color.LightGray;
            this.extTabControlData.TabStyle = tabStyleSquare1;
            this.extTabControlData.TextNotSelectedColor = System.Drawing.SystemColors.ControlText;
            this.extTabControlData.TextSelectedColor = System.Drawing.SystemColors.ControlText;
            // 
            // tabAuto
            // 
            this.tabAuto.Controls.Add(this.extPanelDataGridViewScrollAuto);
            this.tabAuto.Location = new System.Drawing.Point(4, 22);
            this.tabAuto.Name = "tabAuto";
            this.tabAuto.Padding = new System.Windows.Forms.Padding(3);
            this.tabAuto.Size = new System.Drawing.Size(356, 133);
            this.tabAuto.TabIndex = 0;
            this.tabAuto.Text = "Automatic";
            this.tabAuto.UseVisualStyleBackColor = true;
            // 
            // extPanelDataGridViewScrollAuto
            // 
            this.extPanelDataGridViewScrollAuto.Controls.Add(this.extScrollBarAuto);
            this.extPanelDataGridViewScrollAuto.Controls.Add(this.dataGridViewAuto);
            this.extPanelDataGridViewScrollAuto.InternalMargin = new System.Windows.Forms.Padding(0);
            this.extPanelDataGridViewScrollAuto.Location = new System.Drawing.Point(0, 0);
            this.extPanelDataGridViewScrollAuto.Name = "extPanelDataGridViewScrollAuto";
            this.extPanelDataGridViewScrollAuto.Size = new System.Drawing.Size(358, 134);
            this.extPanelDataGridViewScrollAuto.TabIndex = 16;
            this.extPanelDataGridViewScrollAuto.VerticalScrollBarDockRight = true;
            // 
            // extScrollBarAuto
            // 
            this.extScrollBarAuto.AlwaysHideScrollBar = false;
            this.extScrollBarAuto.ArrowBorderColor = System.Drawing.Color.LightBlue;
            this.extScrollBarAuto.ArrowButtonColor = System.Drawing.Color.LightGray;
            this.extScrollBarAuto.ArrowColorScaling = 0.5F;
            this.extScrollBarAuto.ArrowDownDrawAngle = 270F;
            this.extScrollBarAuto.ArrowUpDrawAngle = 90F;
            this.extScrollBarAuto.BorderColor = System.Drawing.Color.White;
            this.extScrollBarAuto.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.extScrollBarAuto.HideScrollBar = false;
            this.extScrollBarAuto.LargeChange = 0;
            this.extScrollBarAuto.Location = new System.Drawing.Point(339, 0);
            this.extScrollBarAuto.Maximum = -1;
            this.extScrollBarAuto.Minimum = 0;
            this.extScrollBarAuto.MouseOverButtonColor = System.Drawing.Color.Green;
            this.extScrollBarAuto.MousePressedButtonColor = System.Drawing.Color.Red;
            this.extScrollBarAuto.Name = "extScrollBarAuto";
            this.extScrollBarAuto.Size = new System.Drawing.Size(19, 134);
            this.extScrollBarAuto.SliderColor = System.Drawing.Color.DarkGray;
            this.extScrollBarAuto.SmallChange = 1;
            this.extScrollBarAuto.TabIndex = 16;
            this.extScrollBarAuto.Text = "extScrollBar1";
            this.extScrollBarAuto.ThumbBorderColor = System.Drawing.Color.Yellow;
            this.extScrollBarAuto.ThumbButtonColor = System.Drawing.Color.DarkBlue;
            this.extScrollBarAuto.ThumbColorScaling = 0.5F;
            this.extScrollBarAuto.ThumbDrawAngle = 0F;
            this.extScrollBarAuto.Value = -1;
            this.extScrollBarAuto.ValueLimited = -1;
            // 
            // dataGridViewAuto
            // 
            this.dataGridViewAuto.AllowUserToAddRows = false;
            this.dataGridViewAuto.AllowUserToDeleteRows = false;
            this.dataGridViewAuto.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewAuto.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewAuto.Name = "dataGridViewAuto";
            this.dataGridViewAuto.ReadOnly = true;
            this.dataGridViewAuto.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dataGridViewAuto.Size = new System.Drawing.Size(339, 134);
            this.dataGridViewAuto.TabIndex = 15;
            // 
            // tabData
            // 
            this.tabData.Controls.Add(this.extPanelDataGridViewScrollData);
            this.tabData.Location = new System.Drawing.Point(4, 22);
            this.tabData.Name = "tabData";
            this.tabData.Padding = new System.Windows.Forms.Padding(3);
            this.tabData.Size = new System.Drawing.Size(356, 133);
            this.tabData.TabIndex = 1;
            this.tabData.Text = "Missing Data";
            this.tabData.UseVisualStyleBackColor = true;
            // 
            // extPanelDataGridViewScrollData
            // 
            this.extPanelDataGridViewScrollData.Controls.Add(this.extScrollBarData);
            this.extPanelDataGridViewScrollData.Controls.Add(this.dataGridViewData);
            this.extPanelDataGridViewScrollData.InternalMargin = new System.Windows.Forms.Padding(0);
            this.extPanelDataGridViewScrollData.Location = new System.Drawing.Point(0, 0);
            this.extPanelDataGridViewScrollData.Name = "extPanelDataGridViewScrollData";
            this.extPanelDataGridViewScrollData.Size = new System.Drawing.Size(358, 134);
            this.extPanelDataGridViewScrollData.TabIndex = 14;
            this.extPanelDataGridViewScrollData.VerticalScrollBarDockRight = true;
            // 
            // extScrollBarData
            // 
            this.extScrollBarData.AlwaysHideScrollBar = false;
            this.extScrollBarData.ArrowBorderColor = System.Drawing.Color.LightBlue;
            this.extScrollBarData.ArrowButtonColor = System.Drawing.Color.LightGray;
            this.extScrollBarData.ArrowColorScaling = 0.5F;
            this.extScrollBarData.ArrowDownDrawAngle = 270F;
            this.extScrollBarData.ArrowUpDrawAngle = 90F;
            this.extScrollBarData.BorderColor = System.Drawing.Color.White;
            this.extScrollBarData.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.extScrollBarData.HideScrollBar = false;
            this.extScrollBarData.LargeChange = 0;
            this.extScrollBarData.Location = new System.Drawing.Point(339, 0);
            this.extScrollBarData.Maximum = -1;
            this.extScrollBarData.Minimum = 0;
            this.extScrollBarData.MouseOverButtonColor = System.Drawing.Color.Green;
            this.extScrollBarData.MousePressedButtonColor = System.Drawing.Color.Red;
            this.extScrollBarData.Name = "extScrollBarData";
            this.extScrollBarData.Size = new System.Drawing.Size(19, 134);
            this.extScrollBarData.SliderColor = System.Drawing.Color.DarkGray;
            this.extScrollBarData.SmallChange = 1;
            this.extScrollBarData.TabIndex = 14;
            this.extScrollBarData.Text = "extScrollBar1";
            this.extScrollBarData.ThumbBorderColor = System.Drawing.Color.Yellow;
            this.extScrollBarData.ThumbButtonColor = System.Drawing.Color.DarkBlue;
            this.extScrollBarData.ThumbColorScaling = 0.5F;
            this.extScrollBarData.ThumbDrawAngle = 0F;
            this.extScrollBarData.Value = -1;
            this.extScrollBarData.ValueLimited = -1;
            // 
            // dataGridViewData
            // 
            this.dataGridViewData.AllowUserToAddRows = false;
            this.dataGridViewData.AllowUserToDeleteRows = false;
            this.dataGridViewData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnData0,
            this.ColumnData1,
            this.ColumnData2,
            this.ColumnData3,
            this.ColumnData4});
            this.dataGridViewData.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewData.Name = "dataGridViewData";
            this.dataGridViewData.ReadOnly = true;
            this.dataGridViewData.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dataGridViewData.Size = new System.Drawing.Size(339, 134);
            this.dataGridViewData.TabIndex = 13;
            // 
            // ColumnData0
            // 
            this.ColumnData0.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnData0.HeaderText = "";
            this.ColumnData0.Name = "ColumnData0";
            this.ColumnData0.ReadOnly = true;
            this.ColumnData0.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnData0.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnData0.Width = 5;
            // 
            // ColumnData1
            // 
            this.ColumnData1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnData1.HeaderText = "";
            this.ColumnData1.Name = "ColumnData1";
            this.ColumnData1.ReadOnly = true;
            this.ColumnData1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnData1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnData1.Width = 5;
            // 
            // ColumnData2
            // 
            this.ColumnData2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnData2.HeaderText = "";
            this.ColumnData2.Name = "ColumnData2";
            this.ColumnData2.ReadOnly = true;
            this.ColumnData2.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnData2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnData2.Width = 5;
            // 
            // ColumnData3
            // 
            this.ColumnData3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnData3.HeaderText = "";
            this.ColumnData3.Name = "ColumnData3";
            this.ColumnData3.ReadOnly = true;
            this.ColumnData3.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnData3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnData3.Width = 5;
            // 
            // ColumnData4
            // 
            this.ColumnData4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnData4.HeaderText = "";
            this.ColumnData4.Name = "ColumnData4";
            this.ColumnData4.ReadOnly = true;
            this.ColumnData4.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnData4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnData4.Width = 5;
            // 
            // tabBio
            // 
            this.tabBio.Controls.Add(this.extPanelDataGridViewScrollBio);
            this.tabBio.Location = new System.Drawing.Point(4, 22);
            this.tabBio.Name = "tabBio";
            this.tabBio.Size = new System.Drawing.Size(356, 133);
            this.tabBio.TabIndex = 7;
            this.tabBio.Text = "Bio";
            this.tabBio.UseVisualStyleBackColor = true;
            // 
            // extPanelDataGridViewScrollBio
            // 
            this.extPanelDataGridViewScrollBio.Controls.Add(this.extScrollBarBio);
            this.extPanelDataGridViewScrollBio.Controls.Add(this.dataGridViewBio);
            this.extPanelDataGridViewScrollBio.InternalMargin = new System.Windows.Forms.Padding(0);
            this.extPanelDataGridViewScrollBio.Location = new System.Drawing.Point(0, 0);
            this.extPanelDataGridViewScrollBio.Name = "extPanelDataGridViewScrollBio";
            this.extPanelDataGridViewScrollBio.Size = new System.Drawing.Size(358, 134);
            this.extPanelDataGridViewScrollBio.TabIndex = 15;
            this.extPanelDataGridViewScrollBio.VerticalScrollBarDockRight = true;
            // 
            // extScrollBarBio
            // 
            this.extScrollBarBio.AlwaysHideScrollBar = false;
            this.extScrollBarBio.ArrowBorderColor = System.Drawing.Color.LightBlue;
            this.extScrollBarBio.ArrowButtonColor = System.Drawing.Color.LightGray;
            this.extScrollBarBio.ArrowColorScaling = 0.5F;
            this.extScrollBarBio.ArrowDownDrawAngle = 270F;
            this.extScrollBarBio.ArrowUpDrawAngle = 90F;
            this.extScrollBarBio.BorderColor = System.Drawing.Color.White;
            this.extScrollBarBio.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.extScrollBarBio.HideScrollBar = false;
            this.extScrollBarBio.LargeChange = 0;
            this.extScrollBarBio.Location = new System.Drawing.Point(339, 0);
            this.extScrollBarBio.Maximum = -1;
            this.extScrollBarBio.Minimum = 0;
            this.extScrollBarBio.MouseOverButtonColor = System.Drawing.Color.Green;
            this.extScrollBarBio.MousePressedButtonColor = System.Drawing.Color.Red;
            this.extScrollBarBio.Name = "extScrollBarBio";
            this.extScrollBarBio.Size = new System.Drawing.Size(19, 134);
            this.extScrollBarBio.SliderColor = System.Drawing.Color.DarkGray;
            this.extScrollBarBio.SmallChange = 1;
            this.extScrollBarBio.TabIndex = 15;
            this.extScrollBarBio.Text = "extScrollBar1";
            this.extScrollBarBio.ThumbBorderColor = System.Drawing.Color.Yellow;
            this.extScrollBarBio.ThumbButtonColor = System.Drawing.Color.DarkBlue;
            this.extScrollBarBio.ThumbColorScaling = 0.5F;
            this.extScrollBarBio.ThumbDrawAngle = 0F;
            this.extScrollBarBio.Value = -1;
            this.extScrollBarBio.ValueLimited = -1;
            // 
            // dataGridViewBio
            // 
            this.dataGridViewBio.AllowUserToAddRows = false;
            this.dataGridViewBio.AllowUserToDeleteRows = false;
            this.dataGridViewBio.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewBio.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnBio0,
            this.ColumnBio1,
            this.ColumnBio2,
            this.ColumnBio3,
            this.ColumnBio4});
            this.dataGridViewBio.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewBio.Name = "dataGridViewBio";
            this.dataGridViewBio.ReadOnly = true;
            this.dataGridViewBio.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dataGridViewBio.Size = new System.Drawing.Size(339, 134);
            this.dataGridViewBio.TabIndex = 14;
            // 
            // ColumnBio0
            // 
            this.ColumnBio0.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnBio0.HeaderText = "";
            this.ColumnBio0.Name = "ColumnBio0";
            this.ColumnBio0.ReadOnly = true;
            this.ColumnBio0.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnBio0.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnBio0.Width = 5;
            // 
            // ColumnBio1
            // 
            this.ColumnBio1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnBio1.HeaderText = "";
            this.ColumnBio1.Name = "ColumnBio1";
            this.ColumnBio1.ReadOnly = true;
            this.ColumnBio1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnBio1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnBio1.Width = 5;
            // 
            // ColumnBio2
            // 
            this.ColumnBio2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnBio2.HeaderText = "";
            this.ColumnBio2.Name = "ColumnBio2";
            this.ColumnBio2.ReadOnly = true;
            this.ColumnBio2.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnBio2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnBio2.Width = 5;
            // 
            // ColumnBio3
            // 
            this.ColumnBio3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnBio3.HeaderText = "";
            this.ColumnBio3.Name = "ColumnBio3";
            this.ColumnBio3.ReadOnly = true;
            this.ColumnBio3.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnBio3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnBio3.Width = 5;
            // 
            // ColumnBio4
            // 
            this.ColumnBio4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnBio4.HeaderText = "";
            this.ColumnBio4.Name = "ColumnBio4";
            this.ColumnBio4.ReadOnly = true;
            this.ColumnBio4.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnBio4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnBio4.Width = 5;
            // 
            // tabGeo
            // 
            this.tabGeo.Controls.Add(this.extPanelDataGridViewScrollGeo);
            this.tabGeo.Location = new System.Drawing.Point(4, 22);
            this.tabGeo.Name = "tabGeo";
            this.tabGeo.Size = new System.Drawing.Size(356, 133);
            this.tabGeo.TabIndex = 8;
            this.tabGeo.Text = "Geo";
            this.tabGeo.UseVisualStyleBackColor = true;
            // 
            // dataGridViewGeo
            // 
            this.dataGridViewGeo.AllowUserToAddRows = false;
            this.dataGridViewGeo.AllowUserToDeleteRows = false;
            this.dataGridViewGeo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewGeo.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnGeo0,
            this.ColumnGeo1,
            this.ColumnGeo2,
            this.ColumnGeo3,
            this.ColumnGeo4});
            this.dataGridViewGeo.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewGeo.Name = "dataGridViewGeo";
            this.dataGridViewGeo.ReadOnly = true;
            this.dataGridViewGeo.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dataGridViewGeo.Size = new System.Drawing.Size(339, 134);
            this.dataGridViewGeo.TabIndex = 14;
            // 
            // ColumnGeo0
            // 
            this.ColumnGeo0.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnGeo0.HeaderText = "";
            this.ColumnGeo0.Name = "ColumnGeo0";
            this.ColumnGeo0.ReadOnly = true;
            this.ColumnGeo0.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnGeo0.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnGeo0.Width = 5;
            // 
            // ColumnGeo1
            // 
            this.ColumnGeo1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnGeo1.HeaderText = "";
            this.ColumnGeo1.Name = "ColumnGeo1";
            this.ColumnGeo1.ReadOnly = true;
            this.ColumnGeo1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnGeo1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnGeo1.Width = 5;
            // 
            // ColumnGeo2
            // 
            this.ColumnGeo2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnGeo2.HeaderText = "";
            this.ColumnGeo2.Name = "ColumnGeo2";
            this.ColumnGeo2.ReadOnly = true;
            this.ColumnGeo2.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnGeo2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnGeo2.Width = 5;
            // 
            // ColumnGeo3
            // 
            this.ColumnGeo3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnGeo3.HeaderText = "";
            this.ColumnGeo3.Name = "ColumnGeo3";
            this.ColumnGeo3.ReadOnly = true;
            this.ColumnGeo3.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnGeo3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnGeo3.Width = 5;
            // 
            // ColumnGeo4
            // 
            this.ColumnGeo4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnGeo4.HeaderText = "";
            this.ColumnGeo4.Name = "ColumnGeo4";
            this.ColumnGeo4.ReadOnly = true;
            this.ColumnGeo4.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnGeo4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnGeo4.Width = 5;
            // 
            // tabRingSurvey
            // 
            this.tabRingSurvey.Location = new System.Drawing.Point(4, 22);
            this.tabRingSurvey.Name = "tabRingSurvey";
            this.tabRingSurvey.Padding = new System.Windows.Forms.Padding(3);
            this.tabRingSurvey.Size = new System.Drawing.Size(356, 133);
            this.tabRingSurvey.TabIndex = 4;
            this.tabRingSurvey.Text = "Ring";
            this.tabRingSurvey.UseVisualStyleBackColor = true;
            // 
            // tabGmo
            // 
            this.tabGmo.Location = new System.Drawing.Point(4, 22);
            this.tabGmo.Name = "tabGmo";
            this.tabGmo.Padding = new System.Windows.Forms.Padding(3);
            this.tabGmo.Size = new System.Drawing.Size(356, 133);
            this.tabGmo.TabIndex = 6;
            this.tabGmo.Text = "GMO";
            this.tabGmo.UseVisualStyleBackColor = true;
            // 
            // tabDebug
            // 
            this.tabDebug.Controls.Add(this.CallSystem);
            this.tabDebug.Controls.Add(this.LogSystem);
            this.tabDebug.Controls.Add(this.ClearDebugLog);
            this.tabDebug.Controls.Add(this.TestWhitelist);
            this.tabDebug.Controls.Add(this.LogWhitelist);
            this.tabDebug.Controls.Add(this.DebugLog);
            this.tabDebug.Location = new System.Drawing.Point(4, 22);
            this.tabDebug.Name = "tabDebug";
            this.tabDebug.Padding = new System.Windows.Forms.Padding(3);
            this.tabDebug.Size = new System.Drawing.Size(356, 133);
            this.tabDebug.TabIndex = 5;
            this.tabDebug.Text = "Debug";
            this.tabDebug.UseVisualStyleBackColor = true;
            // 
            // CallSystem
            // 
            this.CallSystem.Location = new System.Drawing.Point(129, 0);
            this.CallSystem.Name = "CallSystem";
            this.CallSystem.Size = new System.Drawing.Size(35, 22);
            this.CallSystem.TabIndex = 10;
            this.CallSystem.Text = "CS";
            this.CallSystem.UseVisualStyleBackColor = true;
            this.CallSystem.Click += new System.EventHandler(this.CallSystem_Click);
            // 
            // LogSystem
            // 
            this.LogSystem.Location = new System.Drawing.Point(88, 0);
            this.LogSystem.Name = "LogSystem";
            this.LogSystem.Size = new System.Drawing.Size(35, 22);
            this.LogSystem.TabIndex = 9;
            this.LogSystem.Text = "LSY";
            this.LogSystem.UseVisualStyleBackColor = true;
            this.LogSystem.Click += new System.EventHandler(this.LogSystem_Click);
            // 
            // ClearDebugLog
            // 
            this.ClearDebugLog.Location = new System.Drawing.Point(275, 0);
            this.ClearDebugLog.Name = "ClearDebugLog";
            this.ClearDebugLog.Size = new System.Drawing.Size(75, 23);
            this.ClearDebugLog.TabIndex = 8;
            this.ClearDebugLog.Text = "Clear";
            this.ClearDebugLog.UseVisualStyleBackColor = true;
            this.ClearDebugLog.Click += new System.EventHandler(this.ClearDebugLog_Click);
            // 
            // TestWhitelist
            // 
            this.TestWhitelist.Location = new System.Drawing.Point(47, 0);
            this.TestWhitelist.Name = "TestWhitelist";
            this.TestWhitelist.Size = new System.Drawing.Size(35, 22);
            this.TestWhitelist.TabIndex = 7;
            this.TestWhitelist.Text = "TW";
            this.TestWhitelist.UseVisualStyleBackColor = true;
            this.TestWhitelist.Click += new System.EventHandler(this.TestWhitelist_Click);
            // 
            // LogWhitelist
            // 
            this.LogWhitelist.Location = new System.Drawing.Point(6, 0);
            this.LogWhitelist.Name = "LogWhitelist";
            this.LogWhitelist.Size = new System.Drawing.Size(35, 22);
            this.LogWhitelist.TabIndex = 6;
            this.LogWhitelist.Text = "LW";
            this.LogWhitelist.UseVisualStyleBackColor = true;
            this.LogWhitelist.Click += new System.EventHandler(this.LogWhitelist_Click);
            // 
            // dataGridPatrol
            // 
            this.dataGridPatrol.AllowUserToAddRows = false;
            this.dataGridPatrol.AllowUserToDeleteRows = false;
            this.dataGridPatrol.AllowUserToResizeColumns = false;
            this.dataGridPatrol.AllowUserToResizeRows = false;
            this.dataGridPatrol.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridPatrol.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridPatrol.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridPatrol.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Patrol,
            this.Instructions,
            this.Distance,
            this.Sysname,
            this.PatrolUrl});
            this.dataGridPatrol.ContextMenuStrip = this.contextMenuStrip1;
            this.dataGridPatrol.Location = new System.Drawing.Point(0, 0);
            this.dataGridPatrol.Name = "dataGridPatrol";
            this.dataGridPatrol.ReadOnly = true;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridPatrol.RowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridPatrol.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dataGridPatrol.ShowCellErrors = false;
            this.dataGridPatrol.ShowCellToolTips = false;
            this.dataGridPatrol.ShowEditingIcon = false;
            this.dataGridPatrol.ShowRowErrors = false;
            this.dataGridPatrol.Size = new System.Drawing.Size(346, 179);
            this.dataGridPatrol.TabIndex = 12;
            this.dataGridPatrol.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DataGridPatrol_MouseDown);
            // 
            // Patrol
            // 
            this.Patrol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.Patrol.HeaderText = "Patrol";
            this.Patrol.Name = "Patrol";
            this.Patrol.ReadOnly = true;
            this.Patrol.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Patrol.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Patrol.Width = 40;
            // 
            // Instructions
            // 
            this.Instructions.HeaderText = "Instructions";
            this.Instructions.Name = "Instructions";
            this.Instructions.ReadOnly = true;
            this.Instructions.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Instructions.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Distance
            // 
            this.Distance.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.Distance.HeaderText = "Distance";
            this.Distance.Name = "Distance";
            this.Distance.ReadOnly = true;
            this.Distance.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Distance.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Distance.Width = 55;
            // 
            // Sysname
            // 
            this.Sysname.HeaderText = "Sysname";
            this.Sysname.Name = "Sysname";
            this.Sysname.ReadOnly = true;
            this.Sysname.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Sysname.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Sysname.Visible = false;
            // 
            // PatrolUrl
            // 
            this.PatrolUrl.HeaderText = "PatrolUrl";
            this.PatrolUrl.Name = "PatrolUrl";
            this.PatrolUrl.ReadOnly = true;
            this.PatrolUrl.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.PatrolUrl.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.PatrolUrl.Visible = false;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CopySystem,
            this.OpenUrl});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(144, 48);
            // 
            // CopySystem
            // 
            this.CopySystem.Name = "CopySystem";
            this.CopySystem.Size = new System.Drawing.Size(143, 22);
            this.CopySystem.Text = "Copy System";
            this.CopySystem.Click += new System.EventHandler(this.CopySystem_Click);
            // 
            // OpenUrl
            // 
            this.OpenUrl.Name = "OpenUrl";
            this.OpenUrl.Size = new System.Drawing.Size(143, 22);
            this.OpenUrl.Text = "Open Link";
            this.OpenUrl.Click += new System.EventHandler(this.OpenUrl_Click);
            // 
            // tableLayoutPatrol
            // 
            this.tableLayoutPatrol.ColumnCount = 3;
            this.tableLayoutPatrol.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 46.85315F));
            this.tableLayoutPatrol.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 53.14685F));
            this.tableLayoutPatrol.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 77F));
            this.tableLayoutPatrol.Controls.Add(this.menuStripRange, 1, 0);
            this.tableLayoutPatrol.Controls.Add(this.menuStripPatrol, 0, 0);
            this.tableLayoutPatrol.Controls.Add(this.buttonCreatePatrol, 2, 0);
            this.tableLayoutPatrol.Location = new System.Drawing.Point(20, 270);
            this.tableLayoutPatrol.Name = "tableLayoutPatrol";
            this.tableLayoutPatrol.RowCount = 1;
            this.tableLayoutPatrol.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPatrol.Size = new System.Drawing.Size(364, 37);
            this.tableLayoutPatrol.TabIndex = 13;
            // 
            // menuStripRange
            // 
            this.menuStripRange.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.menuStripRange.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStripRange.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripRange});
            this.menuStripRange.Location = new System.Drawing.Point(134, 5);
            this.menuStripRange.Name = "menuStripRange";
            this.menuStripRange.Size = new System.Drawing.Size(131, 27);
            this.menuStripRange.TabIndex = 0;
            // 
            // toolStripRange
            // 
            this.toolStripRange.Name = "toolStripRange";
            this.toolStripRange.Size = new System.Drawing.Size(121, 23);
            this.toolStripRange.Text = "Range";
            this.toolStripRange.SelectedIndexChanged += new System.EventHandler(this.toolStripPatrol_IndexChanged);
            // 
            // menuStripPatrol
            // 
            this.menuStripPatrol.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.menuStripPatrol.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStripPatrol.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripPatrol,
            this.toolStripComboBox2});
            this.menuStripPatrol.Location = new System.Drawing.Point(0, 5);
            this.menuStripPatrol.Name = "menuStripPatrol";
            this.menuStripPatrol.Size = new System.Drawing.Size(134, 27);
            this.menuStripPatrol.TabIndex = 0;
            // 
            // toolStripPatrol
            // 
            this.toolStripPatrol.Name = "toolStripPatrol";
            this.toolStripPatrol.Size = new System.Drawing.Size(121, 23);
            this.toolStripPatrol.Text = "Category";
            this.toolStripPatrol.SelectedIndexChanged += new System.EventHandler(this.toolStripPatrol_IndexChanged);
            // 
            // toolStripComboBox2
            // 
            this.toolStripComboBox2.Name = "toolStripComboBox2";
            this.toolStripComboBox2.Size = new System.Drawing.Size(121, 23);
            this.toolStripComboBox2.Text = "EDSM";
            // 
            // buttonCreatePatrol
            // 
            this.buttonCreatePatrol.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buttonCreatePatrol.Enabled = false;
            this.buttonCreatePatrol.Location = new System.Drawing.Point(289, 7);
            this.buttonCreatePatrol.Name = "buttonCreatePatrol";
            this.buttonCreatePatrol.Size = new System.Drawing.Size(72, 23);
            this.buttonCreatePatrol.TabIndex = 16;
            this.buttonCreatePatrol.Text = "Create";
            this.buttonCreatePatrol.UseVisualStyleBackColor = true;
            // 
            // tableLayoutNews
            // 
            this.tableLayoutNews.ColumnCount = 3;
            this.tableLayoutNews.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutNews.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutNews.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutNews.Controls.Add(this.labelNews, 1, 0);
            this.tableLayoutNews.Controls.Add(this.buttonNextNews, 2, 1);
            this.tableLayoutNews.Controls.Add(this.textBoxNews, 1, 1);
            this.tableLayoutNews.Controls.Add(this.buttonPrevNews, 0, 1);
            this.tableLayoutNews.Location = new System.Drawing.Point(21, 198);
            this.tableLayoutNews.Name = "tableLayoutNews";
            this.tableLayoutNews.RowCount = 2;
            this.tableLayoutNews.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutNews.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutNews.Size = new System.Drawing.Size(364, 66);
            this.tableLayoutNews.TabIndex = 14;
            // 
            // labelNews
            // 
            this.labelNews.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelNews.AutoSize = true;
            this.labelNews.Location = new System.Drawing.Point(50, 1);
            this.labelNews.Name = "labelNews";
            this.labelNews.Size = new System.Drawing.Size(74, 13);
            this.labelNews.TabIndex = 15;
            this.labelNews.Text = "Canonn-News";
            // 
            // buttonNextNews
            // 
            this.buttonNextNews.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonNextNews.AutoSize = true;
            this.buttonNextNews.Location = new System.Drawing.Point(318, 29);
            this.buttonNextNews.Name = "buttonNextNews";
            this.buttonNextNews.Size = new System.Drawing.Size(43, 23);
            this.buttonNextNews.TabIndex = 1;
            this.buttonNextNews.Text = "Next";
            this.buttonNextNews.UseVisualStyleBackColor = true;
            // 
            // textBoxNews
            // 
            this.textBoxNews.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textBoxNews.Location = new System.Drawing.Point(50, 19);
            this.textBoxNews.Multiline = true;
            this.textBoxNews.Name = "textBoxNews";
            this.textBoxNews.ReadOnly = true;
            this.textBoxNews.Size = new System.Drawing.Size(262, 44);
            this.textBoxNews.TabIndex = 4;
            this.textBoxNews.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // buttonPrevNews
            // 
            this.buttonPrevNews.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buttonPrevNews.AutoSize = true;
            this.buttonPrevNews.Location = new System.Drawing.Point(3, 29);
            this.buttonPrevNews.Name = "buttonPrevNews";
            this.buttonPrevNews.Size = new System.Drawing.Size(41, 23);
            this.buttonPrevNews.TabIndex = 0;
            this.buttonPrevNews.Text = "Prev";
            this.buttonPrevNews.UseVisualStyleBackColor = true;
            // 
            // extPanelDataGridViewScrollPatrol
            // 
            this.extPanelDataGridViewScrollPatrol.Controls.Add(this.extScrollBarPatrol);
            this.extPanelDataGridViewScrollPatrol.Controls.Add(this.dataGridPatrol);
            this.extPanelDataGridViewScrollPatrol.InternalMargin = new System.Windows.Forms.Padding(0);
            this.extPanelDataGridViewScrollPatrol.Location = new System.Drawing.Point(20, 313);
            this.extPanelDataGridViewScrollPatrol.Name = "extPanelDataGridViewScrollPatrol";
            this.extPanelDataGridViewScrollPatrol.Size = new System.Drawing.Size(365, 179);
            this.extPanelDataGridViewScrollPatrol.TabIndex = 15;
            this.extPanelDataGridViewScrollPatrol.VerticalScrollBarDockRight = true;
            // 
            // extScrollBarPatrol
            // 
            this.extScrollBarPatrol.AlwaysHideScrollBar = false;
            this.extScrollBarPatrol.ArrowBorderColor = System.Drawing.Color.LightBlue;
            this.extScrollBarPatrol.ArrowButtonColor = System.Drawing.Color.LightGray;
            this.extScrollBarPatrol.ArrowColorScaling = 0.5F;
            this.extScrollBarPatrol.ArrowDownDrawAngle = 270F;
            this.extScrollBarPatrol.ArrowUpDrawAngle = 90F;
            this.extScrollBarPatrol.BorderColor = System.Drawing.Color.White;
            this.extScrollBarPatrol.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.extScrollBarPatrol.HideScrollBar = false;
            this.extScrollBarPatrol.LargeChange = 0;
            this.extScrollBarPatrol.Location = new System.Drawing.Point(346, 0);
            this.extScrollBarPatrol.Maximum = -1;
            this.extScrollBarPatrol.Minimum = 0;
            this.extScrollBarPatrol.MouseOverButtonColor = System.Drawing.Color.Green;
            this.extScrollBarPatrol.MousePressedButtonColor = System.Drawing.Color.Red;
            this.extScrollBarPatrol.Name = "extScrollBarPatrol";
            this.extScrollBarPatrol.Size = new System.Drawing.Size(19, 179);
            this.extScrollBarPatrol.SliderColor = System.Drawing.Color.DarkGray;
            this.extScrollBarPatrol.SmallChange = 1;
            this.extScrollBarPatrol.TabIndex = 13;
            this.extScrollBarPatrol.Text = "extScrollBarPatrol";
            this.extScrollBarPatrol.ThumbBorderColor = System.Drawing.Color.Yellow;
            this.extScrollBarPatrol.ThumbButtonColor = System.Drawing.Color.DarkBlue;
            this.extScrollBarPatrol.ThumbColorScaling = 0.5F;
            this.extScrollBarPatrol.ThumbDrawAngle = 0F;
            this.extScrollBarPatrol.Value = -1;
            this.extScrollBarPatrol.ValueLimited = -1;
            // 
            // extPanelDataGridViewScrollGeo
            // 
            this.extPanelDataGridViewScrollGeo.Controls.Add(this.extScrollBarGeo);
            this.extPanelDataGridViewScrollGeo.Controls.Add(this.dataGridViewGeo);
            this.extPanelDataGridViewScrollGeo.InternalMargin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.extPanelDataGridViewScrollGeo.Location = new System.Drawing.Point(0, 0);
            this.extPanelDataGridViewScrollGeo.Name = "extPanelDataGridViewScrollGeo";
            this.extPanelDataGridViewScrollGeo.Size = new System.Drawing.Size(358, 134);
            this.extPanelDataGridViewScrollGeo.TabIndex = 15;
            this.extPanelDataGridViewScrollGeo.VerticalScrollBarDockRight = true;
            // 
            // extScrollBarGeo
            // 
            this.extScrollBarGeo.AlwaysHideScrollBar = false;
            this.extScrollBarGeo.ArrowBorderColor = System.Drawing.Color.LightBlue;
            this.extScrollBarGeo.ArrowButtonColor = System.Drawing.Color.LightGray;
            this.extScrollBarGeo.ArrowColorScaling = 0.5F;
            this.extScrollBarGeo.ArrowDownDrawAngle = 270F;
            this.extScrollBarGeo.ArrowUpDrawAngle = 90F;
            this.extScrollBarGeo.BorderColor = System.Drawing.Color.White;
            this.extScrollBarGeo.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.extScrollBarGeo.HideScrollBar = false;
            this.extScrollBarGeo.LargeChange = 0;
            this.extScrollBarGeo.Location = new System.Drawing.Point(339, 0);
            this.extScrollBarGeo.Maximum = -1;
            this.extScrollBarGeo.Minimum = 0;
            this.extScrollBarGeo.MouseOverButtonColor = System.Drawing.Color.Green;
            this.extScrollBarGeo.MousePressedButtonColor = System.Drawing.Color.Red;
            this.extScrollBarGeo.Name = "extScrollBarGeo";
            this.extScrollBarGeo.Size = new System.Drawing.Size(19, 134);
            this.extScrollBarGeo.SliderColor = System.Drawing.Color.DarkGray;
            this.extScrollBarGeo.SmallChange = 1;
            this.extScrollBarGeo.TabIndex = 15;
            this.extScrollBarGeo.Text = "extScrollBar1";
            this.extScrollBarGeo.ThumbBorderColor = System.Drawing.Color.Yellow;
            this.extScrollBarGeo.ThumbButtonColor = System.Drawing.Color.DarkBlue;
            this.extScrollBarGeo.ThumbColorScaling = 0.5F;
            this.extScrollBarGeo.ThumbDrawAngle = 0F;
            this.extScrollBarGeo.Value = -1;
            this.extScrollBarGeo.ValueLimited = -1;
            // 
            // EDDCanonnUserControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoScroll = true;
            this.Controls.Add(this.extPanelDataGridViewScrollPatrol);
            this.Controls.Add(this.tableLayoutNews);
            this.Controls.Add(this.tableLayoutPatrol);
            this.Controls.Add(this.extTabControlData);
            this.Controls.Add(this.tableLayoutSystem);
            this.Enabled = false;
            this.Name = "EDDCanonnUserControl";
            this.Size = new System.Drawing.Size(407, 512);
            this.tableLayoutSystem.ResumeLayout(false);
            this.tableLayoutSystem.PerformLayout();
            this.extTabControlData.ResumeLayout(false);
            this.tabAuto.ResumeLayout(false);
            this.extPanelDataGridViewScrollAuto.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAuto)).EndInit();
            this.tabData.ResumeLayout(false);
            this.extPanelDataGridViewScrollData.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewData)).EndInit();
            this.tabBio.ResumeLayout(false);
            this.extPanelDataGridViewScrollBio.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewBio)).EndInit();
            this.tabGeo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewGeo)).EndInit();
            this.tabDebug.ResumeLayout(false);
            this.tabDebug.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridPatrol)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.tableLayoutPatrol.ResumeLayout(false);
            this.tableLayoutPatrol.PerformLayout();
            this.menuStripRange.ResumeLayout(false);
            this.menuStripRange.PerformLayout();
            this.menuStripPatrol.ResumeLayout(false);
            this.menuStripPatrol.PerformLayout();
            this.tableLayoutNews.ResumeLayout(false);
            this.tableLayoutNews.PerformLayout();
            this.extPanelDataGridViewScrollPatrol.ResumeLayout(false);
            this.extPanelDataGridViewScrollGeo.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        private TextBox textBoxSystem;
        private Label labelSysName;
        private TextBox DebugLog;
        private Label labelSystemCount;
        private TextBox textBoxBodyCount;
        private TableLayoutPanel tableLayoutSystem;
        private ExtendedControls.ExtTabControl extTabControlData;
        private TabPage tabAuto;
        private TabPage tabData;
        private TabPage tabRingSurvey;
        private TabPage tabDebug;
        private ExtendedControls.ExtButton TestWhitelist;
        private ExtendedControls.ExtButton LogWhitelist;
        private ExtendedControls.ExtButton ClearDebugLog;
        private DataGridView dataGridPatrol;
        private TableLayoutPanel tableLayoutPatrol;
        private MenuStrip menuStripPatrol;
        private ToolStripComboBox toolStripPatrol;
        private ToolStripComboBox toolStripComboBox2;
        private TableLayoutPanel tableLayoutNews;
        private ExtendedControls.ExtButton buttonPrevNews;
        private ExtendedControls.ExtButton buttonNextNews;
        private Label labelNews;
        private TextBox textBoxNews;
        private DataGridView dataGridViewData;
        private ExtendedControls.ExtButton LogSystem;
        private DataGridView dataGridViewAuto;
        private TabPage tabGmo;
        private ExtendedControls.ExtButton buttonCreatePatrol;
        private MenuStrip menuStripRange;
        private ToolStripComboBox toolStripRange;
        private ContextMenuStrip contextMenuStrip1;
        private DataGridViewTextBoxColumn Inst;
        private ToolStripMenuItem CopySystem;
        private ToolStripMenuItem OpenUrl;
        private DataGridViewTextBoxColumn Patrol;
        private DataGridViewTextBoxColumn Instructions;
        private DataGridViewTextBoxColumn Distance;
        private DataGridViewTextBoxColumn Sysname;
        private DataGridViewTextBoxColumn PatrolUrl;
        private TabPage tabBio;
        private TabPage tabGeo;
        private DataGridView dataGridViewBio;
        private DataGridView dataGridViewGeo;
        private DataGridViewTextBoxColumn ColumnData0;
        private DataGridViewTextBoxColumn ColumnData1;
        private DataGridViewTextBoxColumn ColumnData2;
        private DataGridViewTextBoxColumn ColumnData3;
        private DataGridViewTextBoxColumn ColumnData4;
        private DataGridViewTextBoxColumn ColumnBio0;
        private DataGridViewTextBoxColumn ColumnBio1;
        private DataGridViewTextBoxColumn ColumnBio2;
        private DataGridViewTextBoxColumn ColumnBio3;
        private DataGridViewTextBoxColumn ColumnBio4;
        private DataGridViewTextBoxColumn ColumnGeo0;
        private DataGridViewTextBoxColumn ColumnGeo1;
        private DataGridViewTextBoxColumn ColumnGeo2;
        private DataGridViewTextBoxColumn ColumnGeo3;
        private DataGridViewTextBoxColumn ColumnGeo4;
        private ExtendedControls.ExtButton CallSystem;
        private ExtendedControls.ExtPanelDataGridViewScroll extPanelDataGridViewScrollPatrol;
        private ExtendedControls.ExtScrollBar extScrollBarPatrol;
        private ExtendedControls.ExtPanelDataGridViewScroll extPanelDataGridViewScrollAuto;
        private ExtendedControls.ExtScrollBar extScrollBarAuto;
        private ExtPanelDataGridViewScroll extPanelDataGridViewScrollData;
        private ExtScrollBar extScrollBarData;
        private ExtPanelDataGridViewScroll extPanelDataGridViewScrollBio;
        private ExtScrollBar extScrollBarBio;
        private ExtPanelDataGridViewScroll extPanelDataGridViewScrollGeo;
        private ExtScrollBar extScrollBarGeo;
    }
}
