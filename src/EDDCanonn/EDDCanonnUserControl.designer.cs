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
            // extScrollBarRing
            // 
            extScrollBarRing.BackColor = form;
            extScrollBarRing.SliderColor = GridSliderBack;
            extScrollBarRing.BorderColor = GridBorderLines;
            extScrollBarRing.BackColor = form;
            extScrollBarRing.SliderColor = GridSliderBack;
            extScrollBarRing.BorderColor = extScrollBarRing.ThumbBorderColor =
            extScrollBarRing.ArrowBorderColor = GridBorderLines;
            extScrollBarRing.ArrowButtonColor = extScrollBarRing.ThumbButtonColor = c1;
            extScrollBarRing.MouseOverButtonColor = c1.Multiply(mouseoverscaling);
            extScrollBarRing.MousePressedButtonColor = c1.Multiply(mouseselectedscaling);
            extScrollBarRing.ForeColor = GridScrollArrow;
            extScrollBarRing.FlatStyle = fs;

            ExtComboBoxRange.ForeColor = ButtonTextColor;
            ExtComboBoxRange.BackColor = ExtComboBoxRange.DropDownBackgroundColor = ButtonBackColor;
            ExtComboBoxRange.BorderColor = ButtonBorderColor;
            ExtComboBoxRange.MouseOverBackgroundColor = ButtonBackColor.Multiply(mouseoverscaling);
            ExtComboBoxRange.ScrollBarButtonColor = TextBlockScrollButton;
            ExtComboBoxRange.ScrollBarColor = TextBlockSliderBack;
            ExtComboBoxRange.FlatStyle = fs;
            ExtComboBoxRange.Repaint();

            ExtComboBoxPatrol.ForeColor = ButtonTextColor;
            ExtComboBoxPatrol.BackColor = ExtComboBoxPatrol.DropDownBackgroundColor = ButtonBackColor;
            ExtComboBoxPatrol.BorderColor = ButtonBorderColor;
            ExtComboBoxPatrol.MouseOverBackgroundColor = ButtonBackColor.Multiply(mouseoverscaling);
            ExtComboBoxPatrol.ScrollBarButtonColor = TextBlockScrollButton;
            ExtComboBoxPatrol.ScrollBarColor = TextBlockSliderBack;
            ExtComboBoxPatrol.FlatStyle = fs;
            ExtComboBoxPatrol.Repaint();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            ExtendedControls.TabStyleSquare tabStyleSquare1 = new ExtendedControls.TabStyleSquare();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EDDCanonnUserControl));
            this.textBoxSystem = new System.Windows.Forms.TextBox();
            this.labelSysName = new System.Windows.Forms.Label();
            this.labelSystemCount = new System.Windows.Forms.Label();
            this.textBoxBodyCount = new System.Windows.Forms.TextBox();
            this.tableLayoutSystem = new System.Windows.Forms.TableLayoutPanel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.CopySystem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenUrl = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPatrol = new System.Windows.Forms.TableLayoutPanel();
            this.ExtComboBoxPatrol = new ExtendedControls.ExtComboBox();
            this.ExtComboBoxRange = new ExtendedControls.ExtComboBox();
            this.buttonCreatePatrol = new ExtendedControls.ExtButton();
            this.tableLayoutNews = new System.Windows.Forms.TableLayoutPanel();
            this.labelNews = new System.Windows.Forms.Label();
            this.buttonNextNews = new ExtendedControls.ExtButton();
            this.textBoxNews = new System.Windows.Forms.TextBox();
            this.buttonPrevNews = new ExtendedControls.ExtButton();
            this.extPanelDataGridViewScrollPatrol = new ExtendedControls.ExtPanelDataGridViewScroll();
            this.extScrollBarPatrol = new ExtendedControls.ExtScrollBar();
            this.dataGridPatrol = new System.Windows.Forms.DataGridView();
            this.Patrol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Instructions = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Distance = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Sysname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PatrolUrl = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.extTabControlData = new ExtendedControls.ExtTabControl();
            this.tabData = new System.Windows.Forms.TabPage();
            this.extPanelDataGridViewScrollData = new ExtendedControls.ExtPanelDataGridViewScroll();
            this.extScrollBarData = new ExtendedControls.ExtScrollBar();
            this.dataGridViewData = new System.Windows.Forms.DataGridView();
            this.ColumnData0 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnData1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnData2 = new System.Windows.Forms.DataGridViewImageColumn();
            this.tabRing = new System.Windows.Forms.TabPage();
            this.extPanelDataGridViewScrollRing = new ExtendedControls.ExtPanelDataGridViewScroll();
            this.extScrollBarRing = new ExtendedControls.ExtScrollBar();
            this.dataGridViewRing = new System.Windows.Forms.DataGridView();
            this.ColumnRing0 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnRing1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnRing2 = new System.Windows.Forms.DataGridViewImageColumn();
            this.tabBio = new System.Windows.Forms.TabPage();
            this.extPanelDataGridViewScrollBio = new ExtendedControls.ExtPanelDataGridViewScroll();
            this.extScrollBarBio = new ExtendedControls.ExtScrollBar();
            this.dataGridViewBio = new System.Windows.Forms.DataGridView();
            this.ColumnBio0 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnBio1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnBio2 = new System.Windows.Forms.DataGridViewImageColumn();
            this.tabSurvey = new System.Windows.Forms.TabPage();
            this.labelSurvey = new System.Windows.Forms.Label();
            this.tabGmo = new System.Windows.Forms.TabPage();
            this.tabPageAbout = new System.Windows.Forms.TabPage();
            this.tableLayoutAbout = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBoxEDD = new System.Windows.Forms.PictureBox();
            this.pictureBoxCanonn = new System.Windows.Forms.PictureBox();
            this.labelDev = new System.Windows.Forms.Label();
            this.labelTest = new System.Windows.Forms.Label();
            this.labelSpecial = new System.Windows.Forms.Label();
            this.linkLabelEDDCanonn = new System.Windows.Forms.LinkLabel();
            this.tabDebug = new System.Windows.Forms.TabPage();
            this.CallSystem = new ExtendedControls.ExtButton();
            this.LogSystem = new ExtendedControls.ExtButton();
            this.ClearDebugLog = new ExtendedControls.ExtButton();
            this.TestWhitelist = new ExtendedControls.ExtButton();
            this.LogWhitelist = new ExtendedControls.ExtButton();
            this.DebugLog = new System.Windows.Forms.TextBox();
            this.labelGMO = new System.Windows.Forms.Label();
            this.tableLayoutSystem.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.tableLayoutPatrol.SuspendLayout();
            this.tableLayoutNews.SuspendLayout();
            this.extPanelDataGridViewScrollPatrol.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridPatrol)).BeginInit();
            this.extTabControlData.SuspendLayout();
            this.tabData.SuspendLayout();
            this.extPanelDataGridViewScrollData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewData)).BeginInit();
            this.tabRing.SuspendLayout();
            this.extPanelDataGridViewScrollRing.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRing)).BeginInit();
            this.tabBio.SuspendLayout();
            this.extPanelDataGridViewScrollBio.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewBio)).BeginInit();
            this.tabSurvey.SuspendLayout();
            this.tabGmo.SuspendLayout();
            this.tabPageAbout.SuspendLayout();
            this.tableLayoutAbout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxEDD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCanonn)).BeginInit();
            this.tabDebug.SuspendLayout();
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
            this.textBoxSystem.Text = "#";
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
            this.tableLayoutPatrol.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 51.3986F));
            this.tableLayoutPatrol.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 48.6014F));
            this.tableLayoutPatrol.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 77F));
            this.tableLayoutPatrol.Controls.Add(this.ExtComboBoxPatrol, 0, 0);
            this.tableLayoutPatrol.Controls.Add(this.ExtComboBoxRange, 1, 0);
            this.tableLayoutPatrol.Controls.Add(this.buttonCreatePatrol, 2, 0);
            this.tableLayoutPatrol.Location = new System.Drawing.Point(20, 270);
            this.tableLayoutPatrol.Name = "tableLayoutPatrol";
            this.tableLayoutPatrol.RowCount = 1;
            this.tableLayoutPatrol.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPatrol.Size = new System.Drawing.Size(364, 37);
            this.tableLayoutPatrol.TabIndex = 13;
            // 
            // ExtComboBoxPatrol
            // 
            this.ExtComboBoxPatrol.BorderColor = System.Drawing.Color.White;
            this.ExtComboBoxPatrol.ButtonColorScaling = 0.5F;
            this.ExtComboBoxPatrol.DataSource = null;
            this.ExtComboBoxPatrol.DisableBackgroundDisabledShadingGradient = false;
            this.ExtComboBoxPatrol.DisplayMember = "";
            this.ExtComboBoxPatrol.DropDownBackgroundColor = System.Drawing.Color.Gray;
            this.ExtComboBoxPatrol.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ExtComboBoxPatrol.Location = new System.Drawing.Point(3, 3);
            this.ExtComboBoxPatrol.MouseOverBackgroundColor = System.Drawing.Color.Silver;
            this.ExtComboBoxPatrol.Name = "ExtComboBoxPatrol";
            this.ExtComboBoxPatrol.ScrollBarButtonColor = System.Drawing.Color.LightGray;
            this.ExtComboBoxPatrol.ScrollBarColor = System.Drawing.Color.LightGray;
            this.ExtComboBoxPatrol.SelectedIndex = -1;
            this.ExtComboBoxPatrol.SelectedItem = null;
            this.ExtComboBoxPatrol.SelectedValue = null;
            this.ExtComboBoxPatrol.Size = new System.Drawing.Size(122, 21);
            this.ExtComboBoxPatrol.TabIndex = 18;
            this.ExtComboBoxPatrol.Text = "Patrols";
            this.ExtComboBoxPatrol.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ExtComboBoxPatrol.ValueMember = "";
            this.ExtComboBoxPatrol.SelectedIndexChanged += new System.EventHandler(this.toolStripPatrol_IndexChanged);
            // 
            // ExtComboBoxRange
            // 
            this.ExtComboBoxRange.BorderColor = System.Drawing.Color.White;
            this.ExtComboBoxRange.ButtonColorScaling = 0.5F;
            this.ExtComboBoxRange.DataSource = null;
            this.ExtComboBoxRange.DisableBackgroundDisabledShadingGradient = false;
            this.ExtComboBoxRange.DisplayMember = "";
            this.ExtComboBoxRange.DropDownBackgroundColor = System.Drawing.Color.Gray;
            this.ExtComboBoxRange.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ExtComboBoxRange.Location = new System.Drawing.Point(150, 3);
            this.ExtComboBoxRange.MouseOverBackgroundColor = System.Drawing.Color.Silver;
            this.ExtComboBoxRange.Name = "ExtComboBoxRange";
            this.ExtComboBoxRange.ScrollBarButtonColor = System.Drawing.Color.LightGray;
            this.ExtComboBoxRange.ScrollBarColor = System.Drawing.Color.LightGray;
            this.ExtComboBoxRange.SelectedIndex = -1;
            this.ExtComboBoxRange.SelectedItem = null;
            this.ExtComboBoxRange.SelectedValue = null;
            this.ExtComboBoxRange.Size = new System.Drawing.Size(122, 21);
            this.ExtComboBoxRange.TabIndex = 17;
            this.ExtComboBoxRange.Text = "Ranges";
            this.ExtComboBoxRange.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ExtComboBoxRange.ValueMember = "";
            this.ExtComboBoxRange.SelectedIndexChanged += new System.EventHandler(this.toolStripPatrol_IndexChanged);
            // 
            // buttonCreatePatrol
            // 
            this.buttonCreatePatrol.Enabled = false;
            this.buttonCreatePatrol.Location = new System.Drawing.Point(289, 3);
            this.buttonCreatePatrol.Name = "buttonCreatePatrol";
            this.buttonCreatePatrol.Size = new System.Drawing.Size(71, 21);
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
            this.textBoxNews.Text = "#";
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
            // extTabControlData
            // 
            this.extTabControlData.AllowDragReorder = false;
            this.extTabControlData.Controls.Add(this.tabData);
            this.extTabControlData.Controls.Add(this.tabRing);
            this.extTabControlData.Controls.Add(this.tabBio);
            this.extTabControlData.Controls.Add(this.tabSurvey);
            this.extTabControlData.Controls.Add(this.tabGmo);
            this.extTabControlData.Controls.Add(this.tabPageAbout);
            this.extTabControlData.Controls.Add(this.tabDebug);
            this.extTabControlData.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.extTabControlData.Location = new System.Drawing.Point(20, 37);
            this.extTabControlData.Multiline = true;
            this.extTabControlData.Name = "extTabControlData";
            this.extTabControlData.SelectedIndex = 0;
            this.extTabControlData.ShowToolTips = true;
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
            // tabData
            // 
            this.tabData.Controls.Add(this.extPanelDataGridViewScrollData);
            this.tabData.Location = new System.Drawing.Point(4, 22);
            this.tabData.Name = "tabData";
            this.tabData.Padding = new System.Windows.Forms.Padding(3);
            this.tabData.Size = new System.Drawing.Size(356, 133);
            this.tabData.TabIndex = 1;
            this.tabData.Text = "Missing Data";
            this.tabData.ToolTipText = "Summary of all missing data.";
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
            this.dataGridViewData.AllowUserToResizeColumns = false;
            this.dataGridViewData.AllowUserToResizeRows = false;
            this.dataGridViewData.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewData.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridViewData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnData0,
            this.ColumnData1,
            this.ColumnData2});
            this.dataGridViewData.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewData.Name = "dataGridViewData";
            this.dataGridViewData.ReadOnly = true;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewData.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewData.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dataGridViewData.ShowCellErrors = false;
            this.dataGridViewData.ShowCellToolTips = false;
            this.dataGridViewData.ShowEditingIcon = false;
            this.dataGridViewData.ShowRowErrors = false;
            this.dataGridViewData.Size = new System.Drawing.Size(339, 134);
            this.dataGridViewData.TabIndex = 12;
            // 
            // ColumnData0
            // 
            this.ColumnData0.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnData0.HeaderText = "----";
            this.ColumnData0.Name = "ColumnData0";
            this.ColumnData0.ReadOnly = true;
            this.ColumnData0.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnData0.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnData0.Width = 25;
            // 
            // ColumnData1
            // 
            this.ColumnData1.HeaderText = "----";
            this.ColumnData1.Name = "ColumnData1";
            this.ColumnData1.ReadOnly = true;
            this.ColumnData1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnData1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ColumnData2
            // 
            this.ColumnData2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnData2.HeaderText = "----";
            this.ColumnData2.Name = "ColumnData2";
            this.ColumnData2.ReadOnly = true;
            this.ColumnData2.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnData2.Width = 25;
            // 
            // tabRing
            // 
            this.tabRing.Controls.Add(this.extPanelDataGridViewScrollRing);
            this.tabRing.Location = new System.Drawing.Point(4, 22);
            this.tabRing.Name = "tabRing";
            this.tabRing.Size = new System.Drawing.Size(356, 133);
            this.tabRing.TabIndex = 8;
            this.tabRing.Text = "Ring";
            this.tabRing.ToolTipText = "Shows missing ringdata.";
            this.tabRing.UseVisualStyleBackColor = true;
            // 
            // extPanelDataGridViewScrollRing
            // 
            this.extPanelDataGridViewScrollRing.Controls.Add(this.extScrollBarRing);
            this.extPanelDataGridViewScrollRing.Controls.Add(this.dataGridViewRing);
            this.extPanelDataGridViewScrollRing.InternalMargin = new System.Windows.Forms.Padding(0);
            this.extPanelDataGridViewScrollRing.Location = new System.Drawing.Point(0, 0);
            this.extPanelDataGridViewScrollRing.Name = "extPanelDataGridViewScrollRing";
            this.extPanelDataGridViewScrollRing.Size = new System.Drawing.Size(358, 134);
            this.extPanelDataGridViewScrollRing.TabIndex = 15;
            this.extPanelDataGridViewScrollRing.VerticalScrollBarDockRight = true;
            // 
            // extScrollBarRing
            // 
            this.extScrollBarRing.AlwaysHideScrollBar = false;
            this.extScrollBarRing.ArrowBorderColor = System.Drawing.Color.LightBlue;
            this.extScrollBarRing.ArrowButtonColor = System.Drawing.Color.LightGray;
            this.extScrollBarRing.ArrowColorScaling = 0.5F;
            this.extScrollBarRing.ArrowDownDrawAngle = 270F;
            this.extScrollBarRing.ArrowUpDrawAngle = 90F;
            this.extScrollBarRing.BorderColor = System.Drawing.Color.White;
            this.extScrollBarRing.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.extScrollBarRing.HideScrollBar = false;
            this.extScrollBarRing.LargeChange = 0;
            this.extScrollBarRing.Location = new System.Drawing.Point(339, 0);
            this.extScrollBarRing.Maximum = -1;
            this.extScrollBarRing.Minimum = 0;
            this.extScrollBarRing.MouseOverButtonColor = System.Drawing.Color.Green;
            this.extScrollBarRing.MousePressedButtonColor = System.Drawing.Color.Red;
            this.extScrollBarRing.Name = "extScrollBarRing";
            this.extScrollBarRing.Size = new System.Drawing.Size(19, 134);
            this.extScrollBarRing.SliderColor = System.Drawing.Color.DarkGray;
            this.extScrollBarRing.SmallChange = 1;
            this.extScrollBarRing.TabIndex = 15;
            this.extScrollBarRing.Text = "extScrollBar1";
            this.extScrollBarRing.ThumbBorderColor = System.Drawing.Color.Yellow;
            this.extScrollBarRing.ThumbButtonColor = System.Drawing.Color.DarkBlue;
            this.extScrollBarRing.ThumbColorScaling = 0.5F;
            this.extScrollBarRing.ThumbDrawAngle = 0F;
            this.extScrollBarRing.Value = -1;
            this.extScrollBarRing.ValueLimited = -1;
            // 
            // dataGridViewRing
            // 
            this.dataGridViewRing.AllowUserToAddRows = false;
            this.dataGridViewRing.AllowUserToDeleteRows = false;
            this.dataGridViewRing.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewRing.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridViewRing.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewRing.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnRing0,
            this.ColumnRing1,
            this.ColumnRing2});
            this.dataGridViewRing.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewRing.Name = "dataGridViewRing";
            this.dataGridViewRing.ReadOnly = true;
            this.dataGridViewRing.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dataGridViewRing.Size = new System.Drawing.Size(339, 134);
            this.dataGridViewRing.TabIndex = 14;
            // 
            // ColumnRing0
            // 
            this.ColumnRing0.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnRing0.HeaderText = "----";
            this.ColumnRing0.Name = "ColumnRing0";
            this.ColumnRing0.ReadOnly = true;
            this.ColumnRing0.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnRing0.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnRing0.Width = 25;
            // 
            // ColumnRing1
            // 
            this.ColumnRing1.HeaderText = "----";
            this.ColumnRing1.Name = "ColumnRing1";
            this.ColumnRing1.ReadOnly = true;
            this.ColumnRing1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnRing1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ColumnRing2
            // 
            this.ColumnRing2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnRing2.HeaderText = "----";
            this.ColumnRing2.Name = "ColumnRing2";
            this.ColumnRing2.ReadOnly = true;
            this.ColumnRing2.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnRing2.Width = 25;
            // 
            // tabBio
            // 
            this.tabBio.Controls.Add(this.extPanelDataGridViewScrollBio);
            this.tabBio.Location = new System.Drawing.Point(4, 22);
            this.tabBio.Name = "tabBio";
            this.tabBio.Size = new System.Drawing.Size(356, 133);
            this.tabBio.TabIndex = 7;
            this.tabBio.Text = "Bio";
            this.tabBio.ToolTipText = "Shows missing biodata.";
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
            this.dataGridViewBio.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewBio.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridViewBio.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewBio.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnBio0,
            this.ColumnBio1,
            this.ColumnBio2});
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
            this.ColumnBio0.HeaderText = "----";
            this.ColumnBio0.Name = "ColumnBio0";
            this.ColumnBio0.ReadOnly = true;
            this.ColumnBio0.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnBio0.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnBio0.Width = 25;
            // 
            // ColumnBio1
            // 
            this.ColumnBio1.HeaderText = "----";
            this.ColumnBio1.Name = "ColumnBio1";
            this.ColumnBio1.ReadOnly = true;
            this.ColumnBio1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnBio1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ColumnBio2
            // 
            this.ColumnBio2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnBio2.HeaderText = "----";
            this.ColumnBio2.Name = "ColumnBio2";
            this.ColumnBio2.ReadOnly = true;
            this.ColumnBio2.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnBio2.Width = 25;
            // 
            // tabSurvey
            // 
            this.tabSurvey.Controls.Add(this.labelSurvey);
            this.tabSurvey.Location = new System.Drawing.Point(4, 22);
            this.tabSurvey.Name = "tabSurvey";
            this.tabSurvey.Padding = new System.Windows.Forms.Padding(3);
            this.tabSurvey.Size = new System.Drawing.Size(356, 133);
            this.tabSurvey.TabIndex = 4;
            this.tabSurvey.Text = "Survey";
            this.tabSurvey.ToolTipText = "Confirm your surveys.";
            this.tabSurvey.UseVisualStyleBackColor = true;
            // 
            // labelSurvey
            // 
            this.labelSurvey.AutoSize = true;
            this.labelSurvey.Location = new System.Drawing.Point(134, 3);
            this.labelSurvey.Name = "labelSurvey";
            this.labelSurvey.Size = new System.Drawing.Size(91, 13);
            this.labelSurvey.TabIndex = 0;
            this.labelSurvey.Text = "Work in Progress.";
            this.labelSurvey.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabGmo
            // 
            this.tabGmo.Controls.Add(this.labelGMO);
            this.tabGmo.Location = new System.Drawing.Point(4, 22);
            this.tabGmo.Name = "tabGmo";
            this.tabGmo.Padding = new System.Windows.Forms.Padding(3);
            this.tabGmo.Size = new System.Drawing.Size(356, 133);
            this.tabGmo.TabIndex = 6;
            this.tabGmo.Text = "GMO";
            this.tabGmo.UseVisualStyleBackColor = true;
            // 
            // tabPageAbout
            // 
            this.tabPageAbout.Controls.Add(this.tableLayoutAbout);
            this.tabPageAbout.Location = new System.Drawing.Point(4, 22);
            this.tabPageAbout.Name = "tabPageAbout";
            this.tabPageAbout.Size = new System.Drawing.Size(356, 133);
            this.tabPageAbout.TabIndex = 9;
            this.tabPageAbout.Text = "About";
            this.tabPageAbout.UseVisualStyleBackColor = true;
            // 
            // tableLayoutAbout
            // 
            this.tableLayoutAbout.ColumnCount = 3;
            this.tableLayoutAbout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutAbout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutAbout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutAbout.Controls.Add(this.pictureBoxEDD, 2, 3);
            this.tableLayoutAbout.Controls.Add(this.pictureBoxCanonn, 2, 0);
            this.tableLayoutAbout.Controls.Add(this.labelDev, 0, 1);
            this.tableLayoutAbout.Controls.Add(this.labelTest, 0, 2);
            this.tableLayoutAbout.Controls.Add(this.labelSpecial, 0, 3);
            this.tableLayoutAbout.Controls.Add(this.linkLabelEDDCanonn, 0, 0);
            this.tableLayoutAbout.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutAbout.Name = "tableLayoutAbout";
            this.tableLayoutAbout.RowCount = 4;
            this.tableLayoutAbout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutAbout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutAbout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutAbout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutAbout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutAbout.Size = new System.Drawing.Size(350, 127);
            this.tableLayoutAbout.TabIndex = 0;
            // 
            // pictureBoxEDD
            // 
            this.pictureBoxEDD.Image = global::EDDCanonn.Properties.Resources.edd;
            this.pictureBoxEDD.Location = new System.Drawing.Point(303, 96);
            this.pictureBoxEDD.Name = "pictureBoxEDD";
            this.pictureBoxEDD.Size = new System.Drawing.Size(44, 28);
            this.pictureBoxEDD.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxEDD.TabIndex = 0;
            this.pictureBoxEDD.TabStop = false;
            this.pictureBoxEDD.Click += new System.EventHandler(this.pictureBoxEDD_Click);
            // 
            // pictureBoxCanonn
            // 
            this.pictureBoxCanonn.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxCanonn.Image")));
            this.pictureBoxCanonn.Location = new System.Drawing.Point(303, 3);
            this.pictureBoxCanonn.Name = "pictureBoxCanonn";
            this.pictureBoxCanonn.Size = new System.Drawing.Size(44, 25);
            this.pictureBoxCanonn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxCanonn.TabIndex = 1;
            this.pictureBoxCanonn.TabStop = false;
            this.pictureBoxCanonn.Click += new System.EventHandler(this.pictureBoxCanonn_Click);
            // 
            // labelDev
            // 
            this.labelDev.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelDev.AutoSize = true;
            this.labelDev.Location = new System.Drawing.Point(3, 40);
            this.labelDev.Name = "labelDev";
            this.labelDev.Size = new System.Drawing.Size(171, 13);
            this.labelDev.TabIndex = 3;
            this.labelDev.Text = "Developed by EDDiscovery Team.";
            // 
            // labelTest
            // 
            this.labelTest.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelTest.AutoSize = true;
            this.labelTest.Location = new System.Drawing.Point(3, 71);
            this.labelTest.Name = "labelTest";
            this.labelTest.Size = new System.Drawing.Size(138, 13);
            this.labelTest.TabIndex = 4;
            this.labelTest.Text = "Tested by: Eahlstan, Jugom";
            this.labelTest.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelSpecial
            // 
            this.labelSpecial.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelSpecial.AutoSize = true;
            this.labelSpecial.Location = new System.Drawing.Point(3, 103);
            this.labelSpecial.Name = "labelSpecial";
            this.labelSpecial.Size = new System.Drawing.Size(239, 13);
            this.labelSpecial.TabIndex = 5;
            this.labelSpecial.Text = "Special thanks to: Robby, LCU No Fool Like One";
            // 
            // linkLabelEDDCanonn
            // 
            this.linkLabelEDDCanonn.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.linkLabelEDDCanonn.AutoSize = true;
            this.linkLabelEDDCanonn.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.linkLabelEDDCanonn.Location = new System.Drawing.Point(3, 9);
            this.linkLabelEDDCanonn.Name = "linkLabelEDDCanonn";
            this.linkLabelEDDCanonn.Size = new System.Drawing.Size(128, 13);
            this.linkLabelEDDCanonn.TabIndex = 6;
            this.linkLabelEDDCanonn.TabStop = true;
            this.linkLabelEDDCanonn.Text = "EDDCanonn v1.0 | 03/25";
            this.linkLabelEDDCanonn.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelEDDCanonn_LinkClicked);
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
            // labelGMO
            // 
            this.labelGMO.AutoSize = true;
            this.labelGMO.Location = new System.Drawing.Point(134, 3);
            this.labelGMO.Name = "labelGMO";
            this.labelGMO.Size = new System.Drawing.Size(91, 13);
            this.labelGMO.TabIndex = 1;
            this.labelGMO.Text = "Work in Progress.";
            this.labelGMO.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
            this.contextMenuStrip1.ResumeLayout(false);
            this.tableLayoutPatrol.ResumeLayout(false);
            this.tableLayoutNews.ResumeLayout(false);
            this.tableLayoutNews.PerformLayout();
            this.extPanelDataGridViewScrollPatrol.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridPatrol)).EndInit();
            this.extTabControlData.ResumeLayout(false);
            this.tabData.ResumeLayout(false);
            this.extPanelDataGridViewScrollData.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewData)).EndInit();
            this.tabRing.ResumeLayout(false);
            this.extPanelDataGridViewScrollRing.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRing)).EndInit();
            this.tabBio.ResumeLayout(false);
            this.extPanelDataGridViewScrollBio.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewBio)).EndInit();
            this.tabSurvey.ResumeLayout(false);
            this.tabSurvey.PerformLayout();
            this.tabGmo.ResumeLayout(false);
            this.tabGmo.PerformLayout();
            this.tabPageAbout.ResumeLayout(false);
            this.tableLayoutAbout.ResumeLayout(false);
            this.tableLayoutAbout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxEDD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCanonn)).EndInit();
            this.tabDebug.ResumeLayout(false);
            this.tabDebug.PerformLayout();
            this.ResumeLayout(false);

        }
        private TextBox textBoxSystem;
        private Label labelSysName;
        private TextBox DebugLog;
        private Label labelSystemCount;
        private TextBox textBoxBodyCount;
        private TableLayoutPanel tableLayoutSystem;
        private ExtendedControls.ExtTabControl extTabControlData;
        private TabPage tabData;
        private TabPage tabSurvey;
        private TabPage tabDebug;
        private ExtendedControls.ExtButton TestWhitelist;
        private ExtendedControls.ExtButton LogWhitelist;
        private ExtendedControls.ExtButton ClearDebugLog;
        private DataGridView dataGridPatrol;
        private TableLayoutPanel tableLayoutPatrol;
        private TableLayoutPanel tableLayoutNews;
        private ExtendedControls.ExtButton buttonPrevNews;
        private ExtendedControls.ExtButton buttonNextNews;
        private Label labelNews;
        private TextBox textBoxNews;
        private DataGridView dataGridViewData;
        private ExtendedControls.ExtButton LogSystem;
        private TabPage tabGmo;
        private ExtendedControls.ExtButton buttonCreatePatrol;
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
        private TabPage tabRing;
        private DataGridView dataGridViewBio;
        private DataGridView dataGridViewRing;
        private ExtendedControls.ExtButton CallSystem;
        private ExtendedControls.ExtPanelDataGridViewScroll extPanelDataGridViewScrollPatrol;
        private ExtendedControls.ExtScrollBar extScrollBarPatrol;
        private ExtPanelDataGridViewScroll extPanelDataGridViewScrollData;
        private ExtScrollBar extScrollBarData;
        private ExtPanelDataGridViewScroll extPanelDataGridViewScrollBio;
        private ExtScrollBar extScrollBarBio;
        private ExtPanelDataGridViewScroll extPanelDataGridViewScrollRing;
        private ExtScrollBar extScrollBarRing;
        private ExtComboBox ExtComboBoxRange;
        private ExtComboBox ExtComboBoxPatrol;
        private DataGridViewTextBoxColumn ColumnData0;
        private DataGridViewTextBoxColumn ColumnData1;
        private DataGridViewImageColumn ColumnData2;
        private Label labelSurvey;
        private DataGridViewTextBoxColumn ColumnBio0;
        private DataGridViewTextBoxColumn ColumnBio1;
        private DataGridViewImageColumn ColumnBio2;
        private DataGridViewTextBoxColumn ColumnRing0;
        private DataGridViewTextBoxColumn ColumnRing1;
        private DataGridViewImageColumn ColumnRing2;
        private TabPage tabPageAbout;
        private TableLayoutPanel tableLayoutAbout;
        private PictureBox pictureBoxEDD;
        private PictureBox pictureBoxCanonn;
        private Label labelDev;
        private Label labelTest;
        private Label labelSpecial;
        private LinkLabel linkLabelEDDCanonn;
        private Label labelGMO;
    }
}
