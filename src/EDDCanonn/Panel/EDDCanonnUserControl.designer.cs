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

using System.Drawing;
using System.Windows.Forms;
using ExtendedControls;
using QuickJSON;

namespace EDDCanonnPanel
{
    partial class EDDCanonnUserControl
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            ExtendedControls.TabStyleSquare tabStyleSquare1 = new ExtendedControls.TabStyleSquare();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
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
            this.textBoxNews = new System.Windows.Forms.TextBox();
            this.buttonNextNews = new ExtendedControls.ExtButton();
            this.buttonPrevNews = new ExtendedControls.ExtButton();
            this.labelNewsIndex = new System.Windows.Forms.Label();
            this.dataGridViewBioAlt = new System.Windows.Forms.DataGridView();
            this.ColumnBioInfo0 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnBioInfo1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnBioInfo2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clickableBioInfo = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.ColumnData1 = new System.Windows.Forms.DataGridViewImageColumn();
            this.tabRing = new System.Windows.Forms.TabPage();
            this.extPanelDataGridViewScrollRing = new ExtendedControls.ExtPanelDataGridViewScroll();
            this.extScrollBarRing = new ExtendedControls.ExtScrollBar();
            this.dataGridViewRing = new System.Windows.Forms.DataGridView();
            this.ColumnRing0 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnRing1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnRing2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnRing3 = new System.Windows.Forms.DataGridViewImageColumn();
            this.tabBio = new System.Windows.Forms.TabPage();
            this.extPanelDataGridViewScrollBio = new ExtendedControls.ExtPanelDataGridViewScroll();
            this.extScrollBarBio = new ExtendedControls.ExtScrollBar();
            this.dataGridViewBio = new System.Windows.Forms.DataGridView();
            this.ColumnBio0 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnBio1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnBio2 = new System.Windows.Forms.DataGridViewImageColumn();
            this.clickable = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabGeo = new System.Windows.Forms.TabPage();
            this.extPanelDataGridViewScrollGeo = new ExtendedControls.ExtPanelDataGridViewScroll();
            this.extScrollBarGeo = new ExtendedControls.ExtScrollBar();
            this.dataGridViewGeo = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewImageColumn2 = new System.Windows.Forms.DataGridViewImageColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabSurvey = new System.Windows.Forms.TabPage();
            this.labelSurvey = new System.Windows.Forms.Label();
            this.tabGmo = new System.Windows.Forms.TabPage();
            this.extPanelDataGridViewScrollGMO = new ExtendedControls.ExtPanelDataGridViewScroll();
            this.extScrollBarGMO = new ExtendedControls.ExtScrollBar();
            this.dataGridViewGMO = new System.Windows.Forms.DataGridView();
            this.ColumnGMO0 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPageAbout = new System.Windows.Forms.TabPage();
            this.tableLayoutAbout = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBoxCanonn = new System.Windows.Forms.PictureBox();
            this.labelDev = new System.Windows.Forms.Label();
            this.linkLabelEDDCanonn = new System.Windows.Forms.LinkLabel();
            this.labelSpecial = new System.Windows.Forms.Label();
            this.labelTest = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBoxEDD = new System.Windows.Forms.PictureBox();
            this.LSY = new System.Windows.Forms.Button();
            this.tableLayoutSystem.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.tableLayoutPatrol.SuspendLayout();
            this.tableLayoutNews.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewBioAlt)).BeginInit();
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
            this.tabGeo.SuspendLayout();
            this.extPanelDataGridViewScrollGeo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewGeo)).BeginInit();
            this.tabSurvey.SuspendLayout();
            this.tabGmo.SuspendLayout();
            this.extPanelDataGridViewScrollGMO.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewGMO)).BeginInit();
            this.tabPageAbout.SuspendLayout();
            this.tableLayoutAbout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCanonn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxEDD)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxSystem
            // 
            this.textBoxSystem.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textBoxSystem.Cursor = System.Windows.Forms.Cursors.Hand;
            this.textBoxSystem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxSystem.Location = new System.Drawing.Point(200, 4);
            this.textBoxSystem.Name = "textBoxSystem";
            this.textBoxSystem.ReadOnly = true;
            this.textBoxSystem.Size = new System.Drawing.Size(163, 20);
            this.textBoxSystem.TabIndex = 3;
            this.textBoxSystem.Text = "Waiting..";
            this.textBoxSystem.Click += new System.EventHandler(this.textBoxSystem_Click);
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
            this.labelSystemCount.Location = new System.Drawing.Point(731, 1);
            this.labelSystemCount.Name = "labelSystemCount";
            this.labelSystemCount.Size = new System.Drawing.Size(35, 26);
            this.labelSystemCount.TabIndex = 6;
            this.labelSystemCount.Text = "Body Count";
            // 
            // textBoxBodyCount
            // 
            this.textBoxBodyCount.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textBoxBodyCount.Location = new System.Drawing.Point(775, 4);
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
            this.tableLayoutSystem.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutSystem.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutSystem.Name = "tableLayoutSystem";
            this.tableLayoutSystem.RowCount = 1;
            this.tableLayoutSystem.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 27F));
            this.tableLayoutSystem.Size = new System.Drawing.Size(860, 28);
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
            this.CopySystem.Click += new System.EventHandler(this.toolstrip_CopySystem_Click);
            // 
            // OpenUrl
            // 
            this.OpenUrl.Name = "OpenUrl";
            this.OpenUrl.Size = new System.Drawing.Size(143, 22);
            this.OpenUrl.Text = "Open Link";
            this.OpenUrl.Click += new System.EventHandler(this.toolstrip_OpenURL_Click);
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
            this.tableLayoutPatrol.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPatrol.Location = new System.Drawing.Point(0, 348);
            this.tableLayoutPatrol.Name = "tableLayoutPatrol";
            this.tableLayoutPatrol.RowCount = 1;
            this.tableLayoutPatrol.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPatrol.Size = new System.Drawing.Size(860, 28);
            this.tableLayoutPatrol.TabIndex = 13;
            // 
            // ExtComboBoxPatrol
            // 
            this.ExtComboBoxPatrol.BackColor2 = System.Drawing.Color.Red;
            this.ExtComboBoxPatrol.BorderColor = System.Drawing.Color.White;
            this.ExtComboBoxPatrol.BorderSize = 1;
            this.ExtComboBoxPatrol.ControlBackground = System.Drawing.SystemColors.Control;
            this.ExtComboBoxPatrol.DataSource = null;
            this.ExtComboBoxPatrol.DisableBackgroundDisabledShadingGradient = false;
            this.ExtComboBoxPatrol.DisabledScaling = 0.5F;
            this.ExtComboBoxPatrol.DisplayMember = "";
            this.ExtComboBoxPatrol.DropDownMinimumItemWidth = -1;
            this.ExtComboBoxPatrol.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ExtComboBoxPatrol.GradientDirection = 90F;
            this.ExtComboBoxPatrol.Location = new System.Drawing.Point(3, 3);
            this.ExtComboBoxPatrol.MouseOverScalingColor = 1.3F;
            this.ExtComboBoxPatrol.Name = "ExtComboBoxPatrol";
            this.ExtComboBoxPatrol.SelectedIndex = -1;
            this.ExtComboBoxPatrol.SelectedItem = null;
            this.ExtComboBoxPatrol.SelectedValue = null;
            this.ExtComboBoxPatrol.Size = new System.Drawing.Size(122, 21);
            this.ExtComboBoxPatrol.TabIndex = 18;
            this.ExtComboBoxPatrol.Text = "Patrols";
            this.ExtComboBoxPatrol.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ExtComboBoxPatrol.ValueMember = "";
            this.ExtComboBoxPatrol.SelectedIndexChanged += new System.EventHandler(this.extComboBoxPatrolOrRange_IndexChanged);
            // 
            // ExtComboBoxRange
            // 
            this.ExtComboBoxRange.BackColor2 = System.Drawing.Color.Red;
            this.ExtComboBoxRange.BorderColor = System.Drawing.Color.White;
            this.ExtComboBoxRange.BorderSize = 1;
            this.ExtComboBoxRange.ControlBackground = System.Drawing.SystemColors.Control;
            this.ExtComboBoxRange.DataSource = null;
            this.ExtComboBoxRange.DisableBackgroundDisabledShadingGradient = false;
            this.ExtComboBoxRange.DisabledScaling = 0.5F;
            this.ExtComboBoxRange.DisplayMember = "";
            this.ExtComboBoxRange.DropDownMinimumItemWidth = -1;
            this.ExtComboBoxRange.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ExtComboBoxRange.GradientDirection = 90F;
            this.ExtComboBoxRange.Location = new System.Drawing.Point(405, 3);
            this.ExtComboBoxRange.MouseOverScalingColor = 1.3F;
            this.ExtComboBoxRange.Name = "ExtComboBoxRange";
            this.ExtComboBoxRange.SelectedIndex = -1;
            this.ExtComboBoxRange.SelectedItem = null;
            this.ExtComboBoxRange.SelectedValue = null;
            this.ExtComboBoxRange.Size = new System.Drawing.Size(122, 21);
            this.ExtComboBoxRange.TabIndex = 17;
            this.ExtComboBoxRange.Text = "Ranges";
            this.ExtComboBoxRange.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ExtComboBoxRange.ValueMember = "";
            this.ExtComboBoxRange.SelectedIndexChanged += new System.EventHandler(this.extComboBoxPatrolOrRange_IndexChanged);
            // 
            // buttonCreatePatrol
            // 
            this.buttonCreatePatrol.BackColor2 = System.Drawing.Color.White;
            this.buttonCreatePatrol.ButtonDisabledScaling = 0.5F;
            this.buttonCreatePatrol.Enabled = false;
            this.buttonCreatePatrol.GradientDirection = 90F;
            this.buttonCreatePatrol.Location = new System.Drawing.Point(785, 3);
            this.buttonCreatePatrol.MouseOverScaling = 1.3F;
            this.buttonCreatePatrol.MouseSelectedScaling = 1.3F;
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
            this.tableLayoutNews.Controls.Add(this.textBoxNews, 1, 1);
            this.tableLayoutNews.Controls.Add(this.buttonNextNews, 2, 1);
            this.tableLayoutNews.Controls.Add(this.buttonPrevNews, 0, 1);
            this.tableLayoutNews.Controls.Add(this.labelNewsIndex, 2, 0);
            this.tableLayoutNews.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutNews.Location = new System.Drawing.Point(0, 282);
            this.tableLayoutNews.Name = "tableLayoutNews";
            this.tableLayoutNews.RowCount = 2;
            this.tableLayoutNews.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutNews.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutNews.Size = new System.Drawing.Size(860, 66);
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
            // textBoxNews
            // 
            this.textBoxNews.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textBoxNews.Cursor = System.Windows.Forms.Cursors.Hand;
            this.textBoxNews.Location = new System.Drawing.Point(50, 19);
            this.textBoxNews.MaxLength = 3000;
            this.textBoxNews.Multiline = true;
            this.textBoxNews.Name = "textBoxNews";
            this.textBoxNews.ReadOnly = true;
            this.textBoxNews.Size = new System.Drawing.Size(260, 44);
            this.textBoxNews.TabIndex = 4;
            this.textBoxNews.Text = "Waiting..";
            this.textBoxNews.Click += new System.EventHandler(this.textBoxNews_Click);
            // 
            // buttonNextNews
            // 
            this.buttonNextNews.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonNextNews.AutoSize = true;
            this.buttonNextNews.BackColor2 = System.Drawing.Color.White;
            this.buttonNextNews.ButtonDisabledScaling = 0.5F;
            this.buttonNextNews.GradientDirection = 90F;
            this.buttonNextNews.Location = new System.Drawing.Point(814, 29);
            this.buttonNextNews.MouseOverScaling = 1.3F;
            this.buttonNextNews.MouseSelectedScaling = 1.3F;
            this.buttonNextNews.Name = "buttonNextNews";
            this.buttonNextNews.Size = new System.Drawing.Size(43, 23);
            this.buttonNextNews.TabIndex = 1;
            this.buttonNextNews.Text = "Next";
            this.buttonNextNews.UseVisualStyleBackColor = true;
            this.buttonNextNews.Click += new System.EventHandler(this.buttonNextNews_Click);
            // 
            // buttonPrevNews
            // 
            this.buttonPrevNews.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buttonPrevNews.AutoSize = true;
            this.buttonPrevNews.BackColor2 = System.Drawing.Color.White;
            this.buttonPrevNews.ButtonDisabledScaling = 0.5F;
            this.buttonPrevNews.GradientDirection = 90F;
            this.buttonPrevNews.Location = new System.Drawing.Point(3, 29);
            this.buttonPrevNews.MouseOverScaling = 1.3F;
            this.buttonPrevNews.MouseSelectedScaling = 1.3F;
            this.buttonPrevNews.Name = "buttonPrevNews";
            this.buttonPrevNews.Size = new System.Drawing.Size(41, 23);
            this.buttonPrevNews.TabIndex = 0;
            this.buttonPrevNews.Text = "Prev";
            this.buttonPrevNews.UseVisualStyleBackColor = true;
            this.buttonPrevNews.Click += new System.EventHandler(this.buttonPrevNews_Click);
            // 
            // labelNewsIndex
            // 
            this.labelNewsIndex.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelNewsIndex.AutoSize = true;
            this.labelNewsIndex.Location = new System.Drawing.Point(812, 1);
            this.labelNewsIndex.Name = "labelNewsIndex";
            this.labelNewsIndex.Size = new System.Drawing.Size(45, 13);
            this.labelNewsIndex.TabIndex = 16;
            this.labelNewsIndex.Text = "Page: #";
            // 
            // dataGridViewBioAlt
            // 
            this.dataGridViewBioAlt.AllowUserToAddRows = false;
            this.dataGridViewBioAlt.AllowUserToDeleteRows = false;
            this.dataGridViewBioAlt.AllowUserToResizeColumns = false;
            this.dataGridViewBioAlt.AllowUserToResizeRows = false;
            this.dataGridViewBioAlt.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewBioAlt.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridViewBioAlt.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewBioAlt.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnBioInfo0,
            this.ColumnBioInfo1,
            this.ColumnBioInfo2,
            this.clickableBioInfo});
            this.dataGridViewBioAlt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewBioAlt.Location = new System.Drawing.Point(0, 376);
            this.dataGridViewBioAlt.Name = "dataGridViewBioAlt";
            this.dataGridViewBioAlt.ReadOnly = true;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewBioAlt.RowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewBioAlt.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dataGridViewBioAlt.ShowCellErrors = false;
            this.dataGridViewBioAlt.ShowCellToolTips = false;
            this.dataGridViewBioAlt.ShowEditingIcon = false;
            this.dataGridViewBioAlt.ShowRowErrors = false;
            this.dataGridViewBioAlt.Size = new System.Drawing.Size(860, 394);
            this.dataGridViewBioAlt.TabIndex = 17;
            this.dataGridViewBioAlt.Visible = false;
            this.dataGridViewBioAlt.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewBioAlt_CellMouseDoubleClick);
            // 
            // ColumnBioInfo0
            // 
            this.ColumnBioInfo0.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnBioInfo0.HeaderText = "";
            this.ColumnBioInfo0.MinimumWidth = 25;
            this.ColumnBioInfo0.Name = "ColumnBioInfo0";
            this.ColumnBioInfo0.ReadOnly = true;
            this.ColumnBioInfo0.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnBioInfo0.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnBioInfo0.Width = 25;
            // 
            // ColumnBioInfo1
            // 
            this.ColumnBioInfo1.HeaderText = "";
            this.ColumnBioInfo1.MinimumWidth = 25;
            this.ColumnBioInfo1.Name = "ColumnBioInfo1";
            this.ColumnBioInfo1.ReadOnly = true;
            this.ColumnBioInfo1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnBioInfo1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ColumnBioInfo2
            // 
            this.ColumnBioInfo2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnBioInfo2.HeaderText = "";
            this.ColumnBioInfo2.MinimumWidth = 25;
            this.ColumnBioInfo2.Name = "ColumnBioInfo2";
            this.ColumnBioInfo2.ReadOnly = true;
            this.ColumnBioInfo2.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnBioInfo2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnBioInfo2.Width = 25;
            // 
            // clickableBioInfo
            // 
            this.clickableBioInfo.HeaderText = "";
            this.clickableBioInfo.Name = "clickableBioInfo";
            this.clickableBioInfo.ReadOnly = true;
            this.clickableBioInfo.Visible = false;
            // 
            // extPanelDataGridViewScrollPatrol
            // 
            this.extPanelDataGridViewScrollPatrol.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.extPanelDataGridViewScrollPatrol.Controls.Add(this.extScrollBarPatrol);
            this.extPanelDataGridViewScrollPatrol.Controls.Add(this.dataGridPatrol);
            this.extPanelDataGridViewScrollPatrol.Dock = System.Windows.Forms.DockStyle.Fill;
            this.extPanelDataGridViewScrollPatrol.InternalMargin = new System.Windows.Forms.Padding(0);
            this.extPanelDataGridViewScrollPatrol.Location = new System.Drawing.Point(0, 376);
            this.extPanelDataGridViewScrollPatrol.Name = "extPanelDataGridViewScrollPatrol";
            this.extPanelDataGridViewScrollPatrol.ScrollBarWidth = 24;
            this.extPanelDataGridViewScrollPatrol.Size = new System.Drawing.Size(860, 394);
            this.extPanelDataGridViewScrollPatrol.TabIndex = 15;
            this.extPanelDataGridViewScrollPatrol.VerticalScrollBarDockRight = true;
            // 
            // extScrollBarPatrol
            // 
            this.extScrollBarPatrol.AlwaysHideScrollBar = false;
            this.extScrollBarPatrol.ArrowBorderColor = System.Drawing.Color.LightBlue;
            this.extScrollBarPatrol.ArrowButtonColor = System.Drawing.Color.LightGray;
            this.extScrollBarPatrol.ArrowButtonColor2 = System.Drawing.Color.LightGray;
            this.extScrollBarPatrol.ArrowDownDrawAngle = 270F;
            this.extScrollBarPatrol.ArrowUpDrawAngle = 90F;
            this.extScrollBarPatrol.BorderColor = System.Drawing.Color.White;
            this.extScrollBarPatrol.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.extScrollBarPatrol.HideScrollBar = false;
            this.extScrollBarPatrol.LargeChange = 0;
            this.extScrollBarPatrol.Location = new System.Drawing.Point(834, 0);
            this.extScrollBarPatrol.Maximum = -1;
            this.extScrollBarPatrol.Minimum = 0;
            this.extScrollBarPatrol.MouseOverButtonColor = System.Drawing.Color.Green;
            this.extScrollBarPatrol.MouseOverButtonColor2 = System.Drawing.Color.Green;
            this.extScrollBarPatrol.MousePressedButtonColor = System.Drawing.Color.Red;
            this.extScrollBarPatrol.MousePressedButtonColor2 = System.Drawing.Color.Red;
            this.extScrollBarPatrol.Name = "extScrollBarPatrol";
            this.extScrollBarPatrol.Size = new System.Drawing.Size(24, 392);
            this.extScrollBarPatrol.SkinnyStyle = ExtendedControls.ExtScrollBar.ScrollStyle.Normal;
            this.extScrollBarPatrol.SliderColor = System.Drawing.Color.DarkGray;
            this.extScrollBarPatrol.SliderColor2 = System.Drawing.Color.DarkGray;
            this.extScrollBarPatrol.SliderDrawAngle = 90F;
            this.extScrollBarPatrol.SmallChange = 1;
            this.extScrollBarPatrol.TabIndex = 13;
            this.extScrollBarPatrol.Text = "extScrollBarPatrol";
            this.extScrollBarPatrol.ThumbBorderColor = System.Drawing.Color.Yellow;
            this.extScrollBarPatrol.ThumbButtonColor = System.Drawing.Color.DarkBlue;
            this.extScrollBarPatrol.ThumbButtonColor2 = System.Drawing.Color.DarkBlue;
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
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridPatrol.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridPatrol.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dataGridPatrol.ShowCellErrors = false;
            this.dataGridPatrol.ShowCellToolTips = false;
            this.dataGridPatrol.ShowEditingIcon = false;
            this.dataGridPatrol.ShowRowErrors = false;
            this.dataGridPatrol.Size = new System.Drawing.Size(834, 392);
            this.dataGridPatrol.TabIndex = 12;
            this.dataGridPatrol.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGridPatrol_MouseDown);
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
            this.extTabControlData.Controls.Add(this.tabGeo);
            this.extTabControlData.Controls.Add(this.tabSurvey);
            this.extTabControlData.Controls.Add(this.tabGmo);
            this.extTabControlData.Controls.Add(this.tabPageAbout);
            this.extTabControlData.Dock = System.Windows.Forms.DockStyle.Top;
            this.extTabControlData.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.extTabControlData.Location = new System.Drawing.Point(0, 28);
            this.extTabControlData.Margin = new System.Windows.Forms.Padding(0);
            this.extTabControlData.Multiline = true;
            this.extTabControlData.Name = "extTabControlData";
            this.extTabControlData.PaintTransparentColor = System.Drawing.Color.Transparent;
            this.extTabControlData.SelectedIndex = 0;
            this.extTabControlData.ShowToolTips = true;
            this.extTabControlData.Size = new System.Drawing.Size(860, 254);
            this.extTabControlData.TabBackgroundGradientDirection = 0F;
            this.extTabControlData.TabControlBorderColor = System.Drawing.Color.DarkGray;
            this.extTabControlData.TabControlBorderColor2 = System.Drawing.Color.DarkGray;
            this.extTabControlData.TabDisabledScaling = 0.5F;
            this.extTabControlData.TabGradientDirection = 90F;
            this.extTabControlData.TabIndex = 11;
            this.extTabControlData.TabMouseOverColor = System.Drawing.Color.White;
            this.extTabControlData.TabMouseOverColor2 = System.Drawing.Color.White;
            this.extTabControlData.TabNotSelectedBorderColor = System.Drawing.Color.Gray;
            this.extTabControlData.TabNotSelectedColor = System.Drawing.Color.Gray;
            this.extTabControlData.TabNotSelectedColor2 = System.Drawing.Color.Gray;
            this.extTabControlData.TabSelectedColor = System.Drawing.Color.LightGray;
            this.extTabControlData.TabSelectedColor2 = System.Drawing.Color.Gray;
            this.extTabControlData.TabStyle = tabStyleSquare1;
            this.extTabControlData.TextNotSelectedColor = System.Drawing.SystemColors.ControlText;
            this.extTabControlData.TextSelectedColor = System.Drawing.SystemColors.ControlText;
            this.extTabControlData.ThemeColors = new System.Drawing.Color[] {
        System.Drawing.SystemColors.Control,
        System.Drawing.SystemColors.Control,
        System.Drawing.SystemColors.Control,
        System.Drawing.SystemColors.Control};
            this.extTabControlData.ThemeColorSet = 1;
            // 
            // tabData
            // 
            this.tabData.Controls.Add(this.extPanelDataGridViewScrollData);
            this.tabData.Location = new System.Drawing.Point(4, 22);
            this.tabData.Name = "tabData";
            this.tabData.Padding = new System.Windows.Forms.Padding(3);
            this.tabData.Size = new System.Drawing.Size(852, 228);
            this.tabData.TabIndex = 1;
            this.tabData.Text = "Overview";
            this.tabData.ToolTipText = "Summary of all system data.";
            this.tabData.UseVisualStyleBackColor = true;
            // 
            // extPanelDataGridViewScrollData
            // 
            this.extPanelDataGridViewScrollData.Controls.Add(this.extScrollBarData);
            this.extPanelDataGridViewScrollData.Controls.Add(this.dataGridViewData);
            this.extPanelDataGridViewScrollData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.extPanelDataGridViewScrollData.InternalMargin = new System.Windows.Forms.Padding(0);
            this.extPanelDataGridViewScrollData.Location = new System.Drawing.Point(3, 3);
            this.extPanelDataGridViewScrollData.Margin = new System.Windows.Forms.Padding(0);
            this.extPanelDataGridViewScrollData.Name = "extPanelDataGridViewScrollData";
            this.extPanelDataGridViewScrollData.ScrollBarWidth = 24;
            this.extPanelDataGridViewScrollData.Size = new System.Drawing.Size(846, 222);
            this.extPanelDataGridViewScrollData.TabIndex = 14;
            this.extPanelDataGridViewScrollData.VerticalScrollBarDockRight = true;
            // 
            // extScrollBarData
            // 
            this.extScrollBarData.AlwaysHideScrollBar = false;
            this.extScrollBarData.ArrowBorderColor = System.Drawing.Color.LightBlue;
            this.extScrollBarData.ArrowButtonColor = System.Drawing.Color.LightGray;
            this.extScrollBarData.ArrowButtonColor2 = System.Drawing.Color.LightGray;
            this.extScrollBarData.ArrowDownDrawAngle = 270F;
            this.extScrollBarData.ArrowUpDrawAngle = 90F;
            this.extScrollBarData.BorderColor = System.Drawing.Color.White;
            this.extScrollBarData.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.extScrollBarData.HideScrollBar = false;
            this.extScrollBarData.LargeChange = 0;
            this.extScrollBarData.Location = new System.Drawing.Point(822, 0);
            this.extScrollBarData.Maximum = -1;
            this.extScrollBarData.Minimum = 0;
            this.extScrollBarData.MouseOverButtonColor = System.Drawing.Color.Green;
            this.extScrollBarData.MouseOverButtonColor2 = System.Drawing.Color.Green;
            this.extScrollBarData.MousePressedButtonColor = System.Drawing.Color.Red;
            this.extScrollBarData.MousePressedButtonColor2 = System.Drawing.Color.Red;
            this.extScrollBarData.Name = "extScrollBarData";
            this.extScrollBarData.Size = new System.Drawing.Size(24, 222);
            this.extScrollBarData.SkinnyStyle = ExtendedControls.ExtScrollBar.ScrollStyle.Normal;
            this.extScrollBarData.SliderColor = System.Drawing.Color.DarkGray;
            this.extScrollBarData.SliderColor2 = System.Drawing.Color.DarkGray;
            this.extScrollBarData.SliderDrawAngle = 90F;
            this.extScrollBarData.SmallChange = 1;
            this.extScrollBarData.TabIndex = 14;
            this.extScrollBarData.Text = "extScrollBar1";
            this.extScrollBarData.ThumbBorderColor = System.Drawing.Color.Yellow;
            this.extScrollBarData.ThumbButtonColor = System.Drawing.Color.DarkBlue;
            this.extScrollBarData.ThumbButtonColor2 = System.Drawing.Color.DarkBlue;
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
            this.ColumnData1});
            this.dataGridViewData.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewData.Name = "dataGridViewData";
            this.dataGridViewData.ReadOnly = true;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewData.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewData.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dataGridViewData.ShowCellErrors = false;
            this.dataGridViewData.ShowCellToolTips = false;
            this.dataGridViewData.ShowEditingIcon = false;
            this.dataGridViewData.ShowRowErrors = false;
            this.dataGridViewData.Size = new System.Drawing.Size(822, 222);
            this.dataGridViewData.TabIndex = 12;
            // 
            // ColumnData0
            // 
            this.ColumnData0.HeaderText = "";
            this.ColumnData0.Name = "ColumnData0";
            this.ColumnData0.ReadOnly = true;
            this.ColumnData0.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnData0.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ColumnData1
            // 
            this.ColumnData1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnData1.HeaderText = "";
            this.ColumnData1.MinimumWidth = 25;
            this.ColumnData1.Name = "ColumnData1";
            this.ColumnData1.ReadOnly = true;
            this.ColumnData1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnData1.Width = 25;
            // 
            // tabRing
            // 
            this.tabRing.Controls.Add(this.extPanelDataGridViewScrollRing);
            this.tabRing.Location = new System.Drawing.Point(4, 22);
            this.tabRing.Name = "tabRing";
            this.tabRing.Padding = new System.Windows.Forms.Padding(3);
            this.tabRing.Size = new System.Drawing.Size(852, 228);
            this.tabRing.TabIndex = 8;
            this.tabRing.Text = "Ring";
            this.tabRing.ToolTipText = "Shows missing ringdata.";
            this.tabRing.UseVisualStyleBackColor = true;
            // 
            // extPanelDataGridViewScrollRing
            // 
            this.extPanelDataGridViewScrollRing.Controls.Add(this.extScrollBarRing);
            this.extPanelDataGridViewScrollRing.Controls.Add(this.dataGridViewRing);
            this.extPanelDataGridViewScrollRing.Dock = System.Windows.Forms.DockStyle.Fill;
            this.extPanelDataGridViewScrollRing.InternalMargin = new System.Windows.Forms.Padding(0);
            this.extPanelDataGridViewScrollRing.Location = new System.Drawing.Point(3, 3);
            this.extPanelDataGridViewScrollRing.Name = "extPanelDataGridViewScrollRing";
            this.extPanelDataGridViewScrollRing.ScrollBarWidth = 24;
            this.extPanelDataGridViewScrollRing.Size = new System.Drawing.Size(846, 222);
            this.extPanelDataGridViewScrollRing.TabIndex = 15;
            this.extPanelDataGridViewScrollRing.VerticalScrollBarDockRight = true;
            // 
            // extScrollBarRing
            // 
            this.extScrollBarRing.AlwaysHideScrollBar = false;
            this.extScrollBarRing.ArrowBorderColor = System.Drawing.Color.LightBlue;
            this.extScrollBarRing.ArrowButtonColor = System.Drawing.Color.LightGray;
            this.extScrollBarRing.ArrowButtonColor2 = System.Drawing.Color.LightGray;
            this.extScrollBarRing.ArrowDownDrawAngle = 270F;
            this.extScrollBarRing.ArrowUpDrawAngle = 90F;
            this.extScrollBarRing.BorderColor = System.Drawing.Color.White;
            this.extScrollBarRing.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.extScrollBarRing.HideScrollBar = false;
            this.extScrollBarRing.LargeChange = 0;
            this.extScrollBarRing.Location = new System.Drawing.Point(822, 0);
            this.extScrollBarRing.Maximum = -1;
            this.extScrollBarRing.Minimum = 0;
            this.extScrollBarRing.MouseOverButtonColor = System.Drawing.Color.Green;
            this.extScrollBarRing.MouseOverButtonColor2 = System.Drawing.Color.Green;
            this.extScrollBarRing.MousePressedButtonColor = System.Drawing.Color.Red;
            this.extScrollBarRing.MousePressedButtonColor2 = System.Drawing.Color.Red;
            this.extScrollBarRing.Name = "extScrollBarRing";
            this.extScrollBarRing.Size = new System.Drawing.Size(24, 222);
            this.extScrollBarRing.SkinnyStyle = ExtendedControls.ExtScrollBar.ScrollStyle.Normal;
            this.extScrollBarRing.SliderColor = System.Drawing.Color.DarkGray;
            this.extScrollBarRing.SliderColor2 = System.Drawing.Color.DarkGray;
            this.extScrollBarRing.SliderDrawAngle = 90F;
            this.extScrollBarRing.SmallChange = 1;
            this.extScrollBarRing.TabIndex = 15;
            this.extScrollBarRing.Text = "extScrollBar1";
            this.extScrollBarRing.ThumbBorderColor = System.Drawing.Color.Yellow;
            this.extScrollBarRing.ThumbButtonColor = System.Drawing.Color.DarkBlue;
            this.extScrollBarRing.ThumbButtonColor2 = System.Drawing.Color.DarkBlue;
            this.extScrollBarRing.ThumbDrawAngle = 0F;
            this.extScrollBarRing.Value = -1;
            this.extScrollBarRing.ValueLimited = -1;
            // 
            // dataGridViewRing
            // 
            this.dataGridViewRing.AllowUserToAddRows = false;
            this.dataGridViewRing.AllowUserToDeleteRows = false;
            this.dataGridViewRing.AllowUserToResizeColumns = false;
            this.dataGridViewRing.AllowUserToResizeRows = false;
            this.dataGridViewRing.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewRing.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridViewRing.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewRing.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnRing0,
            this.ColumnRing1,
            this.ColumnRing2,
            this.ColumnRing3});
            this.dataGridViewRing.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewRing.Name = "dataGridViewRing";
            this.dataGridViewRing.ReadOnly = true;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewRing.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridViewRing.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dataGridViewRing.ShowCellErrors = false;
            this.dataGridViewRing.ShowCellToolTips = false;
            this.dataGridViewRing.ShowEditingIcon = false;
            this.dataGridViewRing.ShowRowErrors = false;
            this.dataGridViewRing.Size = new System.Drawing.Size(822, 222);
            this.dataGridViewRing.TabIndex = 14;
            // 
            // ColumnRing0
            // 
            this.ColumnRing0.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnRing0.HeaderText = "";
            this.ColumnRing0.MinimumWidth = 25;
            this.ColumnRing0.Name = "ColumnRing0";
            this.ColumnRing0.ReadOnly = true;
            this.ColumnRing0.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnRing0.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnRing0.Width = 25;
            // 
            // ColumnRing1
            // 
            this.ColumnRing1.HeaderText = "";
            this.ColumnRing1.MinimumWidth = 25;
            this.ColumnRing1.Name = "ColumnRing1";
            this.ColumnRing1.ReadOnly = true;
            this.ColumnRing1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnRing1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ColumnRing2
            // 
            this.ColumnRing2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnRing2.HeaderText = "";
            this.ColumnRing2.MinimumWidth = 25;
            this.ColumnRing2.Name = "ColumnRing2";
            this.ColumnRing2.ReadOnly = true;
            this.ColumnRing2.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnRing2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnRing2.Width = 25;
            // 
            // ColumnRing3
            // 
            this.ColumnRing3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnRing3.HeaderText = "";
            this.ColumnRing3.MinimumWidth = 25;
            this.ColumnRing3.Name = "ColumnRing3";
            this.ColumnRing3.ReadOnly = true;
            this.ColumnRing3.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnRing3.Width = 25;
            // 
            // tabBio
            // 
            this.tabBio.Controls.Add(this.extPanelDataGridViewScrollBio);
            this.tabBio.Location = new System.Drawing.Point(4, 22);
            this.tabBio.Name = "tabBio";
            this.tabBio.Padding = new System.Windows.Forms.Padding(3);
            this.tabBio.Size = new System.Drawing.Size(852, 228);
            this.tabBio.TabIndex = 7;
            this.tabBio.Text = "Biological";
            this.tabBio.ToolTipText = "Shows biodata for this system.";
            this.tabBio.UseVisualStyleBackColor = true;
            // 
            // extPanelDataGridViewScrollBio
            // 
            this.extPanelDataGridViewScrollBio.Controls.Add(this.extScrollBarBio);
            this.extPanelDataGridViewScrollBio.Controls.Add(this.dataGridViewBio);
            this.extPanelDataGridViewScrollBio.Dock = System.Windows.Forms.DockStyle.Fill;
            this.extPanelDataGridViewScrollBio.InternalMargin = new System.Windows.Forms.Padding(0);
            this.extPanelDataGridViewScrollBio.Location = new System.Drawing.Point(3, 3);
            this.extPanelDataGridViewScrollBio.Name = "extPanelDataGridViewScrollBio";
            this.extPanelDataGridViewScrollBio.ScrollBarWidth = 24;
            this.extPanelDataGridViewScrollBio.Size = new System.Drawing.Size(846, 222);
            this.extPanelDataGridViewScrollBio.TabIndex = 15;
            this.extPanelDataGridViewScrollBio.VerticalScrollBarDockRight = true;
            // 
            // extScrollBarBio
            // 
            this.extScrollBarBio.AlwaysHideScrollBar = false;
            this.extScrollBarBio.ArrowBorderColor = System.Drawing.Color.LightBlue;
            this.extScrollBarBio.ArrowButtonColor = System.Drawing.Color.LightGray;
            this.extScrollBarBio.ArrowButtonColor2 = System.Drawing.Color.LightGray;
            this.extScrollBarBio.ArrowDownDrawAngle = 270F;
            this.extScrollBarBio.ArrowUpDrawAngle = 90F;
            this.extScrollBarBio.BorderColor = System.Drawing.Color.White;
            this.extScrollBarBio.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.extScrollBarBio.HideScrollBar = false;
            this.extScrollBarBio.LargeChange = 0;
            this.extScrollBarBio.Location = new System.Drawing.Point(822, 0);
            this.extScrollBarBio.Maximum = -1;
            this.extScrollBarBio.Minimum = 0;
            this.extScrollBarBio.MouseOverButtonColor = System.Drawing.Color.Green;
            this.extScrollBarBio.MouseOverButtonColor2 = System.Drawing.Color.Green;
            this.extScrollBarBio.MousePressedButtonColor = System.Drawing.Color.Red;
            this.extScrollBarBio.MousePressedButtonColor2 = System.Drawing.Color.Red;
            this.extScrollBarBio.Name = "extScrollBarBio";
            this.extScrollBarBio.Size = new System.Drawing.Size(24, 222);
            this.extScrollBarBio.SkinnyStyle = ExtendedControls.ExtScrollBar.ScrollStyle.Normal;
            this.extScrollBarBio.SliderColor = System.Drawing.Color.DarkGray;
            this.extScrollBarBio.SliderColor2 = System.Drawing.Color.DarkGray;
            this.extScrollBarBio.SliderDrawAngle = 90F;
            this.extScrollBarBio.SmallChange = 1;
            this.extScrollBarBio.TabIndex = 15;
            this.extScrollBarBio.Text = "extScrollBar1";
            this.extScrollBarBio.ThumbBorderColor = System.Drawing.Color.Yellow;
            this.extScrollBarBio.ThumbButtonColor = System.Drawing.Color.DarkBlue;
            this.extScrollBarBio.ThumbButtonColor2 = System.Drawing.Color.DarkBlue;
            this.extScrollBarBio.ThumbDrawAngle = 0F;
            this.extScrollBarBio.Value = -1;
            this.extScrollBarBio.ValueLimited = -1;
            // 
            // dataGridViewBio
            // 
            this.dataGridViewBio.AllowUserToAddRows = false;
            this.dataGridViewBio.AllowUserToDeleteRows = false;
            this.dataGridViewBio.AllowUserToResizeColumns = false;
            this.dataGridViewBio.AllowUserToResizeRows = false;
            this.dataGridViewBio.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewBio.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridViewBio.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewBio.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnBio0,
            this.ColumnBio1,
            this.ColumnBio2,
            this.clickable});
            this.dataGridViewBio.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewBio.Name = "dataGridViewBio";
            this.dataGridViewBio.ReadOnly = true;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewBio.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridViewBio.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dataGridViewBio.ShowCellErrors = false;
            this.dataGridViewBio.ShowCellToolTips = false;
            this.dataGridViewBio.ShowEditingIcon = false;
            this.dataGridViewBio.ShowRowErrors = false;
            this.dataGridViewBio.Size = new System.Drawing.Size(822, 222);
            this.dataGridViewBio.TabIndex = 14;
            this.dataGridViewBio.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewBio_CellMouseDoubleClick);
            // 
            // ColumnBio0
            // 
            this.ColumnBio0.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnBio0.HeaderText = "";
            this.ColumnBio0.MinimumWidth = 25;
            this.ColumnBio0.Name = "ColumnBio0";
            this.ColumnBio0.ReadOnly = true;
            this.ColumnBio0.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnBio0.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnBio0.Width = 25;
            // 
            // ColumnBio1
            // 
            this.ColumnBio1.HeaderText = "";
            this.ColumnBio1.MinimumWidth = 25;
            this.ColumnBio1.Name = "ColumnBio1";
            this.ColumnBio1.ReadOnly = true;
            this.ColumnBio1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnBio1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ColumnBio2
            // 
            this.ColumnBio2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ColumnBio2.HeaderText = "";
            this.ColumnBio2.MinimumWidth = 25;
            this.ColumnBio2.Name = "ColumnBio2";
            this.ColumnBio2.ReadOnly = true;
            this.ColumnBio2.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnBio2.Width = 25;
            // 
            // clickable
            // 
            this.clickable.HeaderText = "";
            this.clickable.Name = "clickable";
            this.clickable.ReadOnly = true;
            this.clickable.Visible = false;
            // 
            // tabGeo
            // 
            this.tabGeo.Controls.Add(this.extPanelDataGridViewScrollGeo);
            this.tabGeo.Location = new System.Drawing.Point(4, 22);
            this.tabGeo.Name = "tabGeo";
            this.tabGeo.Padding = new System.Windows.Forms.Padding(3);
            this.tabGeo.Size = new System.Drawing.Size(852, 228);
            this.tabGeo.TabIndex = 10;
            this.tabGeo.Text = "Geological";
            this.tabGeo.UseVisualStyleBackColor = true;
            // 
            // extPanelDataGridViewScrollGeo
            // 
            this.extPanelDataGridViewScrollGeo.Controls.Add(this.extScrollBarGeo);
            this.extPanelDataGridViewScrollGeo.Controls.Add(this.dataGridViewGeo);
            this.extPanelDataGridViewScrollGeo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.extPanelDataGridViewScrollGeo.InternalMargin = new System.Windows.Forms.Padding(0);
            this.extPanelDataGridViewScrollGeo.Location = new System.Drawing.Point(3, 3);
            this.extPanelDataGridViewScrollGeo.Name = "extPanelDataGridViewScrollGeo";
            this.extPanelDataGridViewScrollGeo.ScrollBarWidth = 24;
            this.extPanelDataGridViewScrollGeo.Size = new System.Drawing.Size(846, 222);
            this.extPanelDataGridViewScrollGeo.TabIndex = 16;
            this.extPanelDataGridViewScrollGeo.VerticalScrollBarDockRight = true;
            // 
            // extScrollBarGeo
            // 
            this.extScrollBarGeo.AlwaysHideScrollBar = false;
            this.extScrollBarGeo.ArrowBorderColor = System.Drawing.Color.LightBlue;
            this.extScrollBarGeo.ArrowButtonColor = System.Drawing.Color.LightGray;
            this.extScrollBarGeo.ArrowButtonColor2 = System.Drawing.Color.LightGray;
            this.extScrollBarGeo.ArrowDownDrawAngle = 270F;
            this.extScrollBarGeo.ArrowUpDrawAngle = 90F;
            this.extScrollBarGeo.BorderColor = System.Drawing.Color.White;
            this.extScrollBarGeo.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.extScrollBarGeo.HideScrollBar = false;
            this.extScrollBarGeo.LargeChange = 0;
            this.extScrollBarGeo.Location = new System.Drawing.Point(822, 0);
            this.extScrollBarGeo.Maximum = -1;
            this.extScrollBarGeo.Minimum = 0;
            this.extScrollBarGeo.MouseOverButtonColor = System.Drawing.Color.Green;
            this.extScrollBarGeo.MouseOverButtonColor2 = System.Drawing.Color.Green;
            this.extScrollBarGeo.MousePressedButtonColor = System.Drawing.Color.Red;
            this.extScrollBarGeo.MousePressedButtonColor2 = System.Drawing.Color.Red;
            this.extScrollBarGeo.Name = "extScrollBarGeo";
            this.extScrollBarGeo.Size = new System.Drawing.Size(24, 222);
            this.extScrollBarGeo.SkinnyStyle = ExtendedControls.ExtScrollBar.ScrollStyle.Normal;
            this.extScrollBarGeo.SliderColor = System.Drawing.Color.DarkGray;
            this.extScrollBarGeo.SliderColor2 = System.Drawing.Color.DarkGray;
            this.extScrollBarGeo.SliderDrawAngle = 90F;
            this.extScrollBarGeo.SmallChange = 1;
            this.extScrollBarGeo.TabIndex = 15;
            this.extScrollBarGeo.Text = "extScrollBar1";
            this.extScrollBarGeo.ThumbBorderColor = System.Drawing.Color.Yellow;
            this.extScrollBarGeo.ThumbButtonColor = System.Drawing.Color.DarkBlue;
            this.extScrollBarGeo.ThumbButtonColor2 = System.Drawing.Color.DarkBlue;
            this.extScrollBarGeo.ThumbDrawAngle = 0F;
            this.extScrollBarGeo.Value = -1;
            this.extScrollBarGeo.ValueLimited = -1;
            // 
            // dataGridViewGeo
            // 
            this.dataGridViewGeo.AllowUserToAddRows = false;
            this.dataGridViewGeo.AllowUserToDeleteRows = false;
            this.dataGridViewGeo.AllowUserToResizeColumns = false;
            this.dataGridViewGeo.AllowUserToResizeRows = false;
            this.dataGridViewGeo.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewGeo.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridViewGeo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewGeo.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4,
            this.dataGridViewImageColumn2,
            this.dataGridViewTextBoxColumn5});
            this.dataGridViewGeo.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewGeo.Name = "dataGridViewGeo";
            this.dataGridViewGeo.ReadOnly = true;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewGeo.RowsDefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridViewGeo.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dataGridViewGeo.ShowCellErrors = false;
            this.dataGridViewGeo.ShowCellToolTips = false;
            this.dataGridViewGeo.ShowEditingIcon = false;
            this.dataGridViewGeo.ShowRowErrors = false;
            this.dataGridViewGeo.Size = new System.Drawing.Size(822, 222);
            this.dataGridViewGeo.TabIndex = 14;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.dataGridViewTextBoxColumn3.HeaderText = "";
            this.dataGridViewTextBoxColumn3.MinimumWidth = 25;
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewTextBoxColumn3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn3.Width = 25;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "";
            this.dataGridViewTextBoxColumn4.MinimumWidth = 25;
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewTextBoxColumn4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewImageColumn2
            // 
            this.dataGridViewImageColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.dataGridViewImageColumn2.HeaderText = "";
            this.dataGridViewImageColumn2.MinimumWidth = 25;
            this.dataGridViewImageColumn2.Name = "dataGridViewImageColumn2";
            this.dataGridViewImageColumn2.ReadOnly = true;
            this.dataGridViewImageColumn2.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewImageColumn2.Width = 25;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.HeaderText = "";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            this.dataGridViewTextBoxColumn5.Visible = false;
            // 
            // tabSurvey
            // 
            this.tabSurvey.Controls.Add(this.labelSurvey);
            this.tabSurvey.Location = new System.Drawing.Point(4, 22);
            this.tabSurvey.Name = "tabSurvey";
            this.tabSurvey.Padding = new System.Windows.Forms.Padding(3);
            this.tabSurvey.Size = new System.Drawing.Size(852, 228);
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
            this.tabGmo.Controls.Add(this.extPanelDataGridViewScrollGMO);
            this.tabGmo.Location = new System.Drawing.Point(4, 22);
            this.tabGmo.Name = "tabGmo";
            this.tabGmo.Padding = new System.Windows.Forms.Padding(3);
            this.tabGmo.Size = new System.Drawing.Size(852, 228);
            this.tabGmo.TabIndex = 6;
            this.tabGmo.Text = "GMO";
            this.tabGmo.UseVisualStyleBackColor = true;
            // 
            // extPanelDataGridViewScrollGMO
            // 
            this.extPanelDataGridViewScrollGMO.Controls.Add(this.extScrollBarGMO);
            this.extPanelDataGridViewScrollGMO.Controls.Add(this.dataGridViewGMO);
            this.extPanelDataGridViewScrollGMO.Dock = System.Windows.Forms.DockStyle.Fill;
            this.extPanelDataGridViewScrollGMO.InternalMargin = new System.Windows.Forms.Padding(0);
            this.extPanelDataGridViewScrollGMO.Location = new System.Drawing.Point(3, 3);
            this.extPanelDataGridViewScrollGMO.Name = "extPanelDataGridViewScrollGMO";
            this.extPanelDataGridViewScrollGMO.ScrollBarWidth = 24;
            this.extPanelDataGridViewScrollGMO.Size = new System.Drawing.Size(846, 222);
            this.extPanelDataGridViewScrollGMO.TabIndex = 16;
            this.extPanelDataGridViewScrollGMO.VerticalScrollBarDockRight = true;
            // 
            // extScrollBarGMO
            // 
            this.extScrollBarGMO.AlwaysHideScrollBar = false;
            this.extScrollBarGMO.ArrowBorderColor = System.Drawing.Color.LightBlue;
            this.extScrollBarGMO.ArrowButtonColor = System.Drawing.Color.LightGray;
            this.extScrollBarGMO.ArrowButtonColor2 = System.Drawing.Color.LightGray;
            this.extScrollBarGMO.ArrowDownDrawAngle = 270F;
            this.extScrollBarGMO.ArrowUpDrawAngle = 90F;
            this.extScrollBarGMO.BorderColor = System.Drawing.Color.White;
            this.extScrollBarGMO.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.extScrollBarGMO.HideScrollBar = false;
            this.extScrollBarGMO.LargeChange = 0;
            this.extScrollBarGMO.Location = new System.Drawing.Point(822, 0);
            this.extScrollBarGMO.Maximum = -1;
            this.extScrollBarGMO.Minimum = 0;
            this.extScrollBarGMO.MouseOverButtonColor = System.Drawing.Color.Green;
            this.extScrollBarGMO.MouseOverButtonColor2 = System.Drawing.Color.Green;
            this.extScrollBarGMO.MousePressedButtonColor = System.Drawing.Color.Red;
            this.extScrollBarGMO.MousePressedButtonColor2 = System.Drawing.Color.Red;
            this.extScrollBarGMO.Name = "extScrollBarGMO";
            this.extScrollBarGMO.Size = new System.Drawing.Size(24, 222);
            this.extScrollBarGMO.SkinnyStyle = ExtendedControls.ExtScrollBar.ScrollStyle.Normal;
            this.extScrollBarGMO.SliderColor = System.Drawing.Color.DarkGray;
            this.extScrollBarGMO.SliderColor2 = System.Drawing.Color.DarkGray;
            this.extScrollBarGMO.SliderDrawAngle = 90F;
            this.extScrollBarGMO.SmallChange = 1;
            this.extScrollBarGMO.TabIndex = 15;
            this.extScrollBarGMO.Text = "extScrollBar1";
            this.extScrollBarGMO.ThumbBorderColor = System.Drawing.Color.Yellow;
            this.extScrollBarGMO.ThumbButtonColor = System.Drawing.Color.DarkBlue;
            this.extScrollBarGMO.ThumbButtonColor2 = System.Drawing.Color.DarkBlue;
            this.extScrollBarGMO.ThumbDrawAngle = 0F;
            this.extScrollBarGMO.Value = -1;
            this.extScrollBarGMO.ValueLimited = -1;
            // 
            // dataGridViewGMO
            // 
            this.dataGridViewGMO.AllowUserToAddRows = false;
            this.dataGridViewGMO.AllowUserToDeleteRows = false;
            this.dataGridViewGMO.AllowUserToResizeColumns = false;
            this.dataGridViewGMO.AllowUserToResizeRows = false;
            this.dataGridViewGMO.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewGMO.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridViewGMO.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewGMO.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnGMO0});
            this.dataGridViewGMO.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewGMO.Name = "dataGridViewGMO";
            this.dataGridViewGMO.ReadOnly = true;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewGMO.RowsDefaultCellStyle = dataGridViewCellStyle7;
            this.dataGridViewGMO.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dataGridViewGMO.ShowCellErrors = false;
            this.dataGridViewGMO.ShowCellToolTips = false;
            this.dataGridViewGMO.ShowEditingIcon = false;
            this.dataGridViewGMO.ShowRowErrors = false;
            this.dataGridViewGMO.Size = new System.Drawing.Size(822, 222);
            this.dataGridViewGMO.TabIndex = 14;
            // 
            // ColumnGMO0
            // 
            this.ColumnGMO0.HeaderText = "";
            this.ColumnGMO0.MinimumWidth = 296;
            this.ColumnGMO0.Name = "ColumnGMO0";
            this.ColumnGMO0.ReadOnly = true;
            this.ColumnGMO0.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnGMO0.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // tabPageAbout
            // 
            this.tabPageAbout.Controls.Add(this.tableLayoutAbout);
            this.tabPageAbout.Location = new System.Drawing.Point(4, 22);
            this.tabPageAbout.Name = "tabPageAbout";
            this.tabPageAbout.Size = new System.Drawing.Size(852, 228);
            this.tabPageAbout.TabIndex = 9;
            this.tabPageAbout.Text = "About";
            this.tabPageAbout.UseVisualStyleBackColor = true;
            // 
            // tableLayoutAbout
            // 
            this.tableLayoutAbout.ColumnCount = 3;
            this.tableLayoutAbout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutAbout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.tableLayoutAbout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutAbout.Controls.Add(this.pictureBoxCanonn, 2, 0);
            this.tableLayoutAbout.Controls.Add(this.labelDev, 0, 1);
            this.tableLayoutAbout.Controls.Add(this.linkLabelEDDCanonn, 0, 0);
            this.tableLayoutAbout.Controls.Add(this.labelSpecial, 0, 4);
            this.tableLayoutAbout.Controls.Add(this.labelTest, 0, 3);
            this.tableLayoutAbout.Controls.Add(this.label1, 0, 2);
            this.tableLayoutAbout.Controls.Add(this.pictureBoxEDD, 1, 0);
            this.tableLayoutAbout.Controls.Add(this.LSY, 2, 4);
            this.tableLayoutAbout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutAbout.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutAbout.Name = "tableLayoutAbout";
            this.tableLayoutAbout.RowCount = 5;
            this.tableLayoutAbout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.21622F));
            this.tableLayoutAbout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15.76577F));
            this.tableLayoutAbout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.86487F));
            this.tableLayoutAbout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 38.28829F));
            this.tableLayoutAbout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.41441F));
            this.tableLayoutAbout.Size = new System.Drawing.Size(852, 228);
            this.tableLayoutAbout.TabIndex = 0;
            // 
            // pictureBoxCanonn
            // 
            this.pictureBoxCanonn.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxCanonn.Image")));
            this.pictureBoxCanonn.Location = new System.Drawing.Point(805, 3);
            this.pictureBoxCanonn.Name = "pictureBoxCanonn";
            this.pictureBoxCanonn.Size = new System.Drawing.Size(44, 30);
            this.pictureBoxCanonn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxCanonn.TabIndex = 1;
            this.pictureBoxCanonn.TabStop = false;
            this.pictureBoxCanonn.Click += new System.EventHandler(this.pictureBoxCanonn_Click);
            // 
            // labelDev
            // 
            this.labelDev.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelDev.AutoSize = true;
            this.labelDev.Location = new System.Drawing.Point(3, 42);
            this.labelDev.Name = "labelDev";
            this.labelDev.Size = new System.Drawing.Size(171, 26);
            this.labelDev.TabIndex = 3;
            this.labelDev.Text = "Developed by EDDiscovery Team.\r\n######################";
            // 
            // linkLabelEDDCanonn
            // 
            this.linkLabelEDDCanonn.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.linkLabelEDDCanonn.AutoSize = true;
            this.linkLabelEDDCanonn.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.linkLabelEDDCanonn.Location = new System.Drawing.Point(3, 12);
            this.linkLabelEDDCanonn.Name = "linkLabelEDDCanonn";
            this.linkLabelEDDCanonn.Size = new System.Drawing.Size(76, 13);
            this.linkLabelEDDCanonn.TabIndex = 6;
            this.linkLabelEDDCanonn.TabStop = true;
            this.linkLabelEDDCanonn.Text = "EDDCanonn v";
            this.linkLabelEDDCanonn.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelEDDCanonn_LinkClicked);
            // 
            // labelSpecial
            // 
            this.labelSpecial.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelSpecial.AutoSize = true;
            this.labelSpecial.Location = new System.Drawing.Point(3, 204);
            this.labelSpecial.Name = "labelSpecial";
            this.labelSpecial.Size = new System.Drawing.Size(239, 13);
            this.labelSpecial.TabIndex = 5;
            this.labelSpecial.Text = "Special thanks to: Robby, LCU No Fool Like One";
            // 
            // labelTest
            // 
            this.labelTest.AutoSize = true;
            this.labelTest.Location = new System.Drawing.Point(3, 107);
            this.labelTest.Name = "labelTest";
            this.labelTest.Size = new System.Drawing.Size(288, 13);
            this.labelTest.TabIndex = 4;
            this.labelTest.Text = "Eahlstan, Jugom, NYZCHF, MentalFS, JDM12983, Recluso";
            this.labelTest.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 77);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 26);
            this.label1.TabIndex = 7;
            this.label1.Text = "Tested by:\r\n#######";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pictureBoxEDD
            // 
            this.pictureBoxEDD.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pictureBoxEDD.Image = global::EDDCanonnPanel.Properties.Resources.edd;
            this.pictureBoxEDD.Location = new System.Drawing.Point(757, 4);
            this.pictureBoxEDD.Name = "pictureBoxEDD";
            this.pictureBoxEDD.Size = new System.Drawing.Size(42, 30);
            this.pictureBoxEDD.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxEDD.TabIndex = 0;
            this.pictureBoxEDD.TabStop = false;
            this.pictureBoxEDD.Click += new System.EventHandler(this.pictureBoxEDD_Click);
            // 
            // LSY
            // 
            this.LSY.Location = new System.Drawing.Point(805, 197);
            this.LSY.Name = "LSY";
            this.LSY.Size = new System.Drawing.Size(44, 27);
            this.LSY.TabIndex = 8;
            this.LSY.Text = "LSY";
            this.LSY.UseVisualStyleBackColor = true;
            this.LSY.Click += new System.EventHandler(this.LSY_Click);
            // 
            // EDDCanonnUserControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoScroll = true;
            this.Controls.Add(this.dataGridViewBioAlt);
            this.Controls.Add(this.extPanelDataGridViewScrollPatrol);
            this.Controls.Add(this.tableLayoutPatrol);
            this.Controls.Add(this.tableLayoutNews);
            this.Controls.Add(this.extTabControlData);
            this.Controls.Add(this.tableLayoutSystem);
            this.Enabled = false;
            this.Name = "EDDCanonnUserControl";
            this.Size = new System.Drawing.Size(860, 770);
            this.tableLayoutSystem.ResumeLayout(false);
            this.tableLayoutSystem.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.tableLayoutPatrol.ResumeLayout(false);
            this.tableLayoutNews.ResumeLayout(false);
            this.tableLayoutNews.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewBioAlt)).EndInit();
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
            this.tabGeo.ResumeLayout(false);
            this.extPanelDataGridViewScrollGeo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewGeo)).EndInit();
            this.tabSurvey.ResumeLayout(false);
            this.tabSurvey.PerformLayout();
            this.tabGmo.ResumeLayout(false);
            this.extPanelDataGridViewScrollGMO.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewGMO)).EndInit();
            this.tabPageAbout.ResumeLayout(false);
            this.tableLayoutAbout.ResumeLayout(false);
            this.tableLayoutAbout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCanonn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxEDD)).EndInit();
            this.ResumeLayout(false);

        }
        private TextBox textBoxSystem;
        private Label labelSysName;
        private Label labelSystemCount;
        private TextBox textBoxBodyCount;
        private TableLayoutPanel tableLayoutSystem;
        private DataGridView dataGridPatrol;
        private TableLayoutPanel tableLayoutPatrol;
        private TableLayoutPanel tableLayoutNews;
        private ExtendedControls.ExtButton buttonPrevNews;
        private ExtendedControls.ExtButton buttonNextNews;
        private Label labelNews;
        private TextBox textBoxNews;
        private ExtendedControls.ExtButton buttonCreatePatrol;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem CopySystem;
        private ToolStripMenuItem OpenUrl;
        private DataGridViewTextBoxColumn Patrol;
        private DataGridViewTextBoxColumn Instructions;
        private DataGridViewTextBoxColumn Distance;
        private DataGridViewTextBoxColumn Sysname;
        private DataGridViewTextBoxColumn PatrolUrl;
        private ExtendedControls.ExtPanelDataGridViewScroll extPanelDataGridViewScrollPatrol;
        private ExtendedControls.ExtScrollBar extScrollBarPatrol;
        private ExtComboBox ExtComboBoxRange;
        private ExtComboBox ExtComboBoxPatrol;
        private Label labelNewsIndex;
        private TabPage tabPageAbout;
        private TableLayoutPanel tableLayoutAbout;
        private PictureBox pictureBoxCanonn;
        private Label labelDev;
        private LinkLabel linkLabelEDDCanonn;
        private Label labelSpecial;
        private Label labelTest;
        private Label label1;
        private PictureBox pictureBoxEDD;
        private Button LSY;
        private TabPage tabGmo;
        private ExtPanelDataGridViewScroll extPanelDataGridViewScrollGMO;
        private ExtScrollBar extScrollBarGMO;
        private DataGridView dataGridViewGMO;
        private TabPage tabSurvey;
        private Label labelSurvey;
        private TabPage tabBio;
        private ExtPanelDataGridViewScroll extPanelDataGridViewScrollBio;
        private ExtScrollBar extScrollBarBio;
        private DataGridView dataGridViewBio;
        private TabPage tabRing;
        private ExtPanelDataGridViewScroll extPanelDataGridViewScrollRing;
        private ExtScrollBar extScrollBarRing;
        private DataGridView dataGridViewRing;
        private TabPage tabData;
        private ExtPanelDataGridViewScroll extPanelDataGridViewScrollData;
        private ExtScrollBar extScrollBarData;
        private DataGridView dataGridViewData;
        private ExtTabControl extTabControlData;
        private DataGridView dataGridViewBioAlt;
        private TabPage tabGeo;
        private ExtPanelDataGridViewScroll extPanelDataGridViewScrollGeo;
        private ExtScrollBar extScrollBarGeo;
        private DataGridView dataGridViewGeo;
        private DataGridViewTextBoxColumn ColumnBioInfo0;
        private DataGridViewTextBoxColumn ColumnBioInfo1;
        private DataGridViewTextBoxColumn ColumnBioInfo2;
        private DataGridViewTextBoxColumn clickableBioInfo;
        private DataGridViewTextBoxColumn ColumnData0;
        private DataGridViewImageColumn ColumnData1;
        private DataGridViewTextBoxColumn ColumnRing0;
        private DataGridViewTextBoxColumn ColumnRing1;
        private DataGridViewTextBoxColumn ColumnRing2;
        private DataGridViewImageColumn ColumnRing3;
        private DataGridViewTextBoxColumn ColumnBio0;
        private DataGridViewTextBoxColumn ColumnBio1;
        private DataGridViewImageColumn ColumnBio2;
        private DataGridViewTextBoxColumn clickable;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private DataGridViewImageColumn dataGridViewImageColumn2;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private DataGridViewTextBoxColumn ColumnGMO0;
    }
}