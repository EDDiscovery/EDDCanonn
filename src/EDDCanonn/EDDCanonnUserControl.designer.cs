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

using System.Windows.Forms;

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

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.textBoxSystem = new System.Windows.Forms.TextBox();
            this.labelSysName = new System.Windows.Forms.Label();
            this.DebugLog = new System.Windows.Forms.TextBox();
            this.labelSystemCount = new System.Windows.Forms.Label();
            this.textBoxBodyCount = new System.Windows.Forms.TextBox();
            this.tableLayoutSystem = new System.Windows.Forms.TableLayoutPanel();
            this.gridData = new System.Windows.Forms.TabControl();
            this.tabAuto = new System.Windows.Forms.TabPage();
            this.dataGridViewAuto = new System.Windows.Forms.DataGridView();
            this.tabData = new System.Windows.Forms.TabPage();
            this.dataGridViewData = new System.Windows.Forms.DataGridView();
            this.tabBio = new System.Windows.Forms.TabPage();
            this.dataGridViewBio = new System.Windows.Forms.DataGridView();
            this.tabGeo = new System.Windows.Forms.TabPage();
            this.dataGridViewGeo = new System.Windows.Forms.DataGridView();
            this.tabRingSurvey = new System.Windows.Forms.TabPage();
            this.tabGmo = new System.Windows.Forms.TabPage();
            this.dataGridView4 = new System.Windows.Forms.DataGridView();
            this.tabDebug = new System.Windows.Forms.TabPage();
            this.LogSystem = new System.Windows.Forms.Button();
            this.ClearDebugLog = new System.Windows.Forms.Button();
            this.TestWhitelist = new System.Windows.Forms.Button();
            this.LogWhitelist = new System.Windows.Forms.Button();
            this.dataGridPatrol = new System.Windows.Forms.DataGridView();
            this.Patrol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Instructions = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Distance = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Sysname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PatrolUrl = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.CopySystem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenUrl = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPatrols = new System.Windows.Forms.TableLayoutPanel();
            this.menuStripRange = new System.Windows.Forms.MenuStrip();
            this.toolStripRange = new System.Windows.Forms.ToolStripComboBox();
            this.menuStripPatrol = new System.Windows.Forms.MenuStrip();
            this.toolStripPatrol = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripComboBox2 = new System.Windows.Forms.ToolStripComboBox();
            this.buttonCreatePatrol = new System.Windows.Forms.Button();
            this.tableLayoutNews = new System.Windows.Forms.TableLayoutPanel();
            this.labelNews = new System.Windows.Forms.Label();
            this.buttonNextNews = new System.Windows.Forms.Button();
            this.textBoxNews = new System.Windows.Forms.TextBox();
            this.buttonPrevNews = new System.Windows.Forms.Button();
            this.ColumnData0 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnData1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnData2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnData3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnData4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnBio0 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnBio1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnBio2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnBio3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnBio4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnGeo0 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnGeo1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnGeo2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnGeo3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnGeo4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutSystem.SuspendLayout();
            this.gridData.SuspendLayout();
            this.tabAuto.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAuto)).BeginInit();
            this.tabData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewData)).BeginInit();
            this.tabBio.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewBio)).BeginInit();
            this.tabGeo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewGeo)).BeginInit();
            this.tabGmo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView4)).BeginInit();
            this.tabDebug.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridPatrol)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.tableLayoutPatrols.SuspendLayout();
            this.menuStripRange.SuspendLayout();
            this.menuStripPatrol.SuspendLayout();
            this.tableLayoutNews.SuspendLayout();
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
            // gridData
            // 
            this.gridData.Controls.Add(this.tabAuto);
            this.gridData.Controls.Add(this.tabData);
            this.gridData.Controls.Add(this.tabBio);
            this.gridData.Controls.Add(this.tabGeo);
            this.gridData.Controls.Add(this.tabRingSurvey);
            this.gridData.Controls.Add(this.tabGmo);
            this.gridData.Controls.Add(this.tabDebug);
            this.gridData.Location = new System.Drawing.Point(20, 37);
            this.gridData.Multiline = true;
            this.gridData.Name = "gridData";
            this.gridData.SelectedIndex = 0;
            this.gridData.Size = new System.Drawing.Size(364, 159);
            this.gridData.TabIndex = 11;
            // 
            // tabAuto
            // 
            this.tabAuto.Controls.Add(this.dataGridViewAuto);
            this.tabAuto.Location = new System.Drawing.Point(4, 22);
            this.tabAuto.Name = "tabAuto";
            this.tabAuto.Padding = new System.Windows.Forms.Padding(3);
            this.tabAuto.Size = new System.Drawing.Size(356, 133);
            this.tabAuto.TabIndex = 0;
            this.tabAuto.Text = "Automatic";
            this.tabAuto.UseVisualStyleBackColor = true;
            // 
            // dataGridViewAuto
            // 
            this.dataGridViewAuto.AllowUserToAddRows = false;
            this.dataGridViewAuto.AllowUserToDeleteRows = false;
            this.dataGridViewAuto.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewAuto.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewAuto.Name = "dataGridViewAuto";
            this.dataGridViewAuto.ReadOnly = true;
            this.dataGridViewAuto.Size = new System.Drawing.Size(356, 133);
            this.dataGridViewAuto.TabIndex = 15;
            // 
            // tabData
            // 
            this.tabData.Controls.Add(this.dataGridViewData);
            this.tabData.Location = new System.Drawing.Point(4, 22);
            this.tabData.Name = "tabData";
            this.tabData.Padding = new System.Windows.Forms.Padding(3);
            this.tabData.Size = new System.Drawing.Size(356, 133);
            this.tabData.TabIndex = 1;
            this.tabData.Text = "Missing Data";
            this.tabData.UseVisualStyleBackColor = true;
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
            this.dataGridViewData.Size = new System.Drawing.Size(356, 133);
            this.dataGridViewData.TabIndex = 13;
            // 
            // tabBio
            // 
            this.tabBio.Controls.Add(this.dataGridViewBio);
            this.tabBio.Location = new System.Drawing.Point(4, 22);
            this.tabBio.Name = "tabBio";
            this.tabBio.Size = new System.Drawing.Size(356, 133);
            this.tabBio.TabIndex = 7;
            this.tabBio.Text = "Bio";
            this.tabBio.UseVisualStyleBackColor = true;
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
            this.dataGridViewBio.Size = new System.Drawing.Size(356, 133);
            this.dataGridViewBio.TabIndex = 14;
            // 
            // tabGeo
            // 
            this.tabGeo.Controls.Add(this.dataGridViewGeo);
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
            this.dataGridViewGeo.Size = new System.Drawing.Size(356, 133);
            this.dataGridViewGeo.TabIndex = 14;
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
            this.tabGmo.Controls.Add(this.dataGridView4);
            this.tabGmo.Location = new System.Drawing.Point(4, 22);
            this.tabGmo.Name = "tabGmo";
            this.tabGmo.Padding = new System.Windows.Forms.Padding(3);
            this.tabGmo.Size = new System.Drawing.Size(356, 133);
            this.tabGmo.TabIndex = 6;
            this.tabGmo.Text = "GMO";
            this.tabGmo.UseVisualStyleBackColor = true;
            // 
            // dataGridView4
            // 
            this.dataGridView4.AllowUserToAddRows = false;
            this.dataGridView4.AllowUserToDeleteRows = false;
            this.dataGridView4.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView4.Location = new System.Drawing.Point(0, 0);
            this.dataGridView4.Name = "dataGridView4";
            this.dataGridView4.ReadOnly = true;
            this.dataGridView4.Size = new System.Drawing.Size(356, 133);
            this.dataGridView4.TabIndex = 16;
            // 
            // tabDebug
            // 
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
            // LogSystem
            // 
            this.LogSystem.Location = new System.Drawing.Point(168, 0);
            this.LogSystem.Name = "LogSystem";
            this.LogSystem.Size = new System.Drawing.Size(75, 23);
            this.LogSystem.TabIndex = 9;
            this.LogSystem.Text = "LSystem";
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
            this.TestWhitelist.Location = new System.Drawing.Point(87, 0);
            this.TestWhitelist.Name = "TestWhitelist";
            this.TestWhitelist.Size = new System.Drawing.Size(75, 23);
            this.TestWhitelist.TabIndex = 7;
            this.TestWhitelist.Text = "TWhitelist";
            this.TestWhitelist.UseVisualStyleBackColor = true;
            this.TestWhitelist.Click += new System.EventHandler(this.TestWhitelist_Click);
            // 
            // LogWhitelist
            // 
            this.LogWhitelist.Location = new System.Drawing.Point(6, 0);
            this.LogWhitelist.Name = "LogWhitelist";
            this.LogWhitelist.Size = new System.Drawing.Size(75, 23);
            this.LogWhitelist.TabIndex = 6;
            this.LogWhitelist.Text = "LWhitelist";
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
            this.dataGridPatrol.Location = new System.Drawing.Point(20, 313);
            this.dataGridPatrol.Name = "dataGridPatrol";
            this.dataGridPatrol.ReadOnly = true;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridPatrol.RowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridPatrol.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridPatrol.ShowCellErrors = false;
            this.dataGridPatrol.ShowCellToolTips = false;
            this.dataGridPatrol.ShowEditingIcon = false;
            this.dataGridPatrol.ShowRowErrors = false;
            this.dataGridPatrol.Size = new System.Drawing.Size(364, 179);
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
            // tableLayoutPatrols
            // 
            this.tableLayoutPatrols.ColumnCount = 3;
            this.tableLayoutPatrols.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 46.85315F));
            this.tableLayoutPatrols.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 53.14685F));
            this.tableLayoutPatrols.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 77F));
            this.tableLayoutPatrols.Controls.Add(this.menuStripRange, 1, 0);
            this.tableLayoutPatrols.Controls.Add(this.menuStripPatrol, 0, 0);
            this.tableLayoutPatrols.Controls.Add(this.buttonCreatePatrol, 2, 0);
            this.tableLayoutPatrols.Location = new System.Drawing.Point(20, 270);
            this.tableLayoutPatrols.Name = "tableLayoutPatrols";
            this.tableLayoutPatrols.RowCount = 1;
            this.tableLayoutPatrols.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPatrols.Size = new System.Drawing.Size(364, 37);
            this.tableLayoutPatrols.TabIndex = 13;
            // 
            // menuStripRange
            // 
            this.menuStripRange.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.menuStripRange.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStripRange.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripRange});
            this.menuStripRange.Location = new System.Drawing.Point(134, 5);
            this.menuStripRange.Name = "menuStripRange";
            this.menuStripRange.Size = new System.Drawing.Size(152, 27);
            this.menuStripRange.TabIndex = 0;
            this.menuStripRange.Text = "";
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
            this.menuStripPatrol.Text = "";
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
            // EDDCanonnUserControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoScroll = true;
            this.Controls.Add(this.tableLayoutNews);
            this.Controls.Add(this.tableLayoutPatrols);
            this.Controls.Add(this.dataGridPatrol);
            this.Controls.Add(this.gridData);
            this.Controls.Add(this.tableLayoutSystem);
            this.Enabled = false;
            this.Name = "EDDCanonnUserControl";
            this.Size = new System.Drawing.Size(407, 512);
            this.tableLayoutSystem.ResumeLayout(false);
            this.tableLayoutSystem.PerformLayout();
            this.gridData.ResumeLayout(false);
            this.tabAuto.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewAuto)).EndInit();
            this.tabData.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewData)).EndInit();
            this.tabBio.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewBio)).EndInit();
            this.tabGeo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewGeo)).EndInit();
            this.tabGmo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView4)).EndInit();
            this.tabDebug.ResumeLayout(false);
            this.tabDebug.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridPatrol)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.tableLayoutPatrols.ResumeLayout(false);
            this.tableLayoutPatrols.PerformLayout();
            this.menuStripRange.ResumeLayout(false);
            this.menuStripRange.PerformLayout();
            this.menuStripPatrol.ResumeLayout(false);
            this.menuStripPatrol.PerformLayout();
            this.tableLayoutNews.ResumeLayout(false);
            this.tableLayoutNews.PerformLayout();
            this.ResumeLayout(false);

        }
        private TextBox textBoxSystem;
        private Label labelSysName;
        private TextBox DebugLog;
        private Label labelSystemCount;
        private TextBox textBoxBodyCount;
        private TableLayoutPanel tableLayoutSystem;
        private TabControl gridData;
        private TabPage tabAuto;
        private TabPage tabData;
        private TabPage tabRingSurvey;
        private TabPage tabDebug;
        private Button TestWhitelist;
        private Button LogWhitelist;
        private Button ClearDebugLog;
        private DataGridView dataGridPatrol;
        private TableLayoutPanel tableLayoutPatrols;
        private MenuStrip menuStripPatrol;
        private ToolStripComboBox toolStripPatrol;
        private ToolStripComboBox toolStripComboBox2;
        private TableLayoutPanel tableLayoutNews;
        private Button buttonPrevNews;
        private Button buttonNextNews;
        private Label labelNews;
        private TextBox textBoxNews;
        private DataGridView dataGridViewData;
        private Button LogSystem;
        private DataGridView dataGridViewAuto;
        private TabPage tabGmo;
        private Button buttonCreatePatrol;
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
        private DataGridView dataGridView4;
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
    }
}
