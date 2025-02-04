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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.textBoxSystem = new System.Windows.Forms.TextBox();
            this.labelSysName = new System.Windows.Forms.Label();
            this.DebugLog = new System.Windows.Forms.TextBox();
            this.labelSystemCount = new System.Windows.Forms.Label();
            this.textBoxBodyCount = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.gridData = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.gridAuto = new System.Windows.Forms.DataGridView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.LogSystem = new System.Windows.Forms.Button();
            this.ClearDebugLog = new System.Windows.Forms.Button();
            this.TestWhitelist = new System.Windows.Forms.Button();
            this.LogWhitelist = new System.Windows.Forms.Button();
            this.dataGridPatrol = new System.Windows.Forms.DataGridView();
            this.Patrol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Instructions = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Distance = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.menuStrip2 = new System.Windows.Forms.MenuStrip();
            this.toolStripRange = new System.Windows.Forms.ToolStripComboBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripPatrol = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripComboBox2 = new System.Windows.Forms.ToolStripComboBox();
            this.create = new System.Windows.Forms.Button();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.gridData.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridAuto)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridPatrol)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.menuStrip2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
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
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27.15517F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 72.84483F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 87F));
            this.tableLayoutPanel1.Controls.Add(this.textBoxBodyCount, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelSystemCount, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBoxSystem, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelSysName, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(20, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 27F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(364, 28);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // gridData
            // 
            this.gridData.Controls.Add(this.tabPage1);
            this.gridData.Controls.Add(this.tabPage2);
            this.gridData.Controls.Add(this.tabPage3);
            this.gridData.Controls.Add(this.tabPage4);
            this.gridData.Controls.Add(this.tabPage5);
            this.gridData.Location = new System.Drawing.Point(20, 37);
            this.gridData.Multiline = true;
            this.gridData.Name = "gridData";
            this.gridData.SelectedIndex = 0;
            this.gridData.Size = new System.Drawing.Size(364, 159);
            this.gridData.TabIndex = 11;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.gridAuto);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(356, 133);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Automatic";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // gridAuto
            // 
            this.gridAuto.AllowUserToAddRows = false;
            this.gridAuto.AllowUserToDeleteRows = false;
            this.gridAuto.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridAuto.Location = new System.Drawing.Point(0, 0);
            this.gridAuto.Name = "gridAuto";
            this.gridAuto.ReadOnly = true;
            this.gridAuto.Size = new System.Drawing.Size(356, 133);
            this.gridAuto.TabIndex = 15;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dataGridView2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(356, 133);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Missing Data";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dataGridView2
            // 
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.AllowUserToDeleteRows = false;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Location = new System.Drawing.Point(0, 0);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.ReadOnly = true;
            this.dataGridView2.Size = new System.Drawing.Size(356, 133);
            this.dataGridView2.TabIndex = 13;
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(356, 133);
            this.tabPage3.TabIndex = 4;
            this.tabPage3.Text = "RingSurvey";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(356, 133);
            this.tabPage4.TabIndex = 6;
            this.tabPage4.Text = "GMO";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.LogSystem);
            this.tabPage5.Controls.Add(this.ClearDebugLog);
            this.tabPage5.Controls.Add(this.TestWhitelist);
            this.tabPage5.Controls.Add(this.LogWhitelist);
            this.tabPage5.Controls.Add(this.DebugLog);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(356, 133);
            this.tabPage5.TabIndex = 5;
            this.tabPage5.Text = "Debug";
            this.tabPage5.UseVisualStyleBackColor = true;
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
            this.Distance});
            this.dataGridPatrol.ContextMenuStrip = this.contextMenuStrip1;
            this.dataGridPatrol.Location = new System.Drawing.Point(20, 313);
            this.dataGridPatrol.Name = "dataGridPatrol";
            this.dataGridPatrol.ReadOnly = true;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridPatrol.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridPatrol.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridPatrol.ShowCellErrors = false;
            this.dataGridPatrol.ShowCellToolTips = false;
            this.dataGridPatrol.ShowEditingIcon = false;
            this.dataGridPatrol.ShowRowErrors = false;
            this.dataGridPatrol.Size = new System.Drawing.Size(364, 179);
            this.dataGridPatrol.TabIndex = 12;
            this.dataGridPatrol.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridPatrol_CellContentClick);
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
            this.Instructions.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
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
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(144, 48);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(143, 22);
            this.toolStripMenuItem1.Text = "Copy System";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(143, 22);
            this.toolStripMenuItem2.Text = "Open Link";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 46.85315F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 53.14685F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 77F));
            this.tableLayoutPanel2.Controls.Add(this.menuStrip2, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.menuStrip1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.create, 2, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(20, 270);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(364, 37);
            this.tableLayoutPanel2.TabIndex = 13;
            // 
            // menuStrip2
            // 
            this.menuStrip2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.menuStrip2.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripRange});
            this.menuStrip2.Location = new System.Drawing.Point(134, 5);
            this.menuStrip2.Name = "menuStrip2";
            this.menuStrip2.Size = new System.Drawing.Size(131, 27);
            this.menuStrip2.TabIndex = 1;
            this.menuStrip2.Text = "menuStrip2";
            // 
            // toolStripRange
            // 
            this.toolStripRange.Enabled = false;
            this.toolStripRange.Name = "toolStripRange";
            this.toolStripRange.Size = new System.Drawing.Size(121, 23);
            this.toolStripRange.Text = "Range";
            this.toolStripRange.SelectedIndexChanged += new System.EventHandler(this.toolStripRange_IndexChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripPatrol,
            this.toolStripComboBox2});
            this.menuStrip1.Location = new System.Drawing.Point(0, 5);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(134, 27);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripPatrol
            // 
            this.toolStripPatrol.Enabled = false;
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
            // create
            // 
            this.create.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.create.Enabled = false;
            this.create.Location = new System.Drawing.Point(289, 7);
            this.create.Name = "create";
            this.create.Size = new System.Drawing.Size(72, 23);
            this.create.TabIndex = 16;
            this.create.Text = "Create";
            this.create.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(this.label1, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.button2, 2, 1);
            this.tableLayoutPanel3.Controls.Add(this.textBox1, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.button1, 0, 1);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(21, 198);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(364, 66);
            this.tableLayoutPanel3.TabIndex = 14;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(50, 1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Canonn-News";
            // 
            // button2
            // 
            this.button2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.button2.AutoSize = true;
            this.button2.Location = new System.Drawing.Point(318, 29);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(43, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "Next";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textBox1.Location = new System.Drawing.Point(50, 19);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(262, 44);
            this.textBox1.TabIndex = 4;
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.button1.AutoSize = true;
            this.button1.Location = new System.Drawing.Point(3, 29);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(41, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Prev";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // EDDCanonnUserControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoScroll = true;
            this.Controls.Add(this.tableLayoutPanel3);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.dataGridPatrol);
            this.Controls.Add(this.gridData);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "EDDCanonnUserControl";
            this.Size = new System.Drawing.Size(407, 512);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.gridData.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridAuto)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridPatrol)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.menuStrip2.ResumeLayout(false);
            this.menuStrip2.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);

        }
        private TextBox textBoxSystem;
        private Label labelSysName;
        private TextBox DebugLog;
        private Label labelSystemCount;
        private TextBox textBoxBodyCount;
        private TableLayoutPanel tableLayoutPanel1;
        private TabControl gridData;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private TabPage tabPage5;
        private Button TestWhitelist;
        private Button LogWhitelist;
        private Button ClearDebugLog;
        private DataGridView dataGridPatrol;
        private TableLayoutPanel tableLayoutPanel2;
        private MenuStrip menuStrip1;
        private ToolStripComboBox toolStripPatrol;
        private ToolStripComboBox toolStripComboBox2;
        private TableLayoutPanel tableLayoutPanel3;
        private Button button1;
        private Button button2;
        private Label label1;
        private TextBox textBox1;
        private DataGridView dataGridView2;
        private Button LogSystem;
        private DataGridView gridAuto;
        private TabPage tabPage4;
        private Button create;
        private MenuStrip menuStrip2;
        private ToolStripComboBox toolStripRange;
        private ContextMenuStrip contextMenuStrip1;
        private DataGridViewTextBoxColumn Patrol;
        private DataGridViewTextBoxColumn Instructions;
        private DataGridViewTextBoxColumn Distance;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItem2;
    }
}
