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
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.button3 = new System.Windows.Forms.Button();
            this.ClearDebugLog = new System.Windows.Forms.Button();
            this.TestWhitelist = new System.Windows.Forms.Button();
            this.LogWhitelist = new System.Windows.Forms.Button();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.menuStrip2 = new System.Windows.Forms.MenuStrip();
            this.toolStripComboBox3 = new System.Windows.Forms.ToolStripComboBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripComboBox2 = new System.Windows.Forms.ToolStripComboBox();
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
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.menuStrip2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxSystem
            // 
            this.textBoxSystem.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textBoxSystem.Location = new System.Drawing.Point(66, 8);
            this.textBoxSystem.Name = "textBoxSystem";
            this.textBoxSystem.ReadOnly = true;
            this.textBoxSystem.Size = new System.Drawing.Size(163, 20);
            this.textBoxSystem.TabIndex = 3;
            // 
            // labelSysName
            // 
            this.labelSysName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelSysName.AutoSize = true;
            this.labelSysName.Location = new System.Drawing.Point(3, 12);
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
            this.labelSystemCount.Location = new System.Drawing.Point(235, 5);
            this.labelSystemCount.Name = "labelSystemCount";
            this.labelSystemCount.Size = new System.Drawing.Size(35, 26);
            this.labelSystemCount.TabIndex = 6;
            this.labelSystemCount.Text = "Body Count";
            // 
            // textBoxBodyCount
            // 
            this.textBoxBodyCount.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textBoxBodyCount.Location = new System.Drawing.Point(279, 8);
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
            this.tableLayoutPanel1.Size = new System.Drawing.Size(364, 37);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // gridData
            // 
            this.gridData.Controls.Add(this.tabPage1);
            this.gridData.Controls.Add(this.tabPage2);
            this.gridData.Controls.Add(this.tabPage3);
            this.gridData.Controls.Add(this.tabPage4);
            this.gridData.Controls.Add(this.tabPage5);
            this.gridData.Location = new System.Drawing.Point(20, 46);
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
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.button3);
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
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(168, 0);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 9;
            this.button3.Text = "Test";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
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
            this.LogWhitelist.Text = "PWhitelist";
            this.LogWhitelist.UseVisualStyleBackColor = true;
            this.LogWhitelist.Click += new System.EventHandler(this.LogWhitelist_Click);
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
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3});
            this.dataGridView1.Location = new System.Drawing.Point(20, 322);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(364, 165);
            this.dataGridView1.TabIndex = 12;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Column1";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Column2";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "Column3";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 46.85315F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 53.14685F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 77F));
            this.tableLayoutPanel2.Controls.Add(this.menuStrip2, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.menuStrip1, 0, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(20, 279);
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
            this.toolStripComboBox3});
            this.menuStrip2.Location = new System.Drawing.Point(134, 5);
            this.menuStrip2.Name = "menuStrip2";
            this.menuStrip2.Size = new System.Drawing.Size(131, 27);
            this.menuStrip2.TabIndex = 1;
            this.menuStrip2.Text = "menuStrip2";
            // 
            // toolStripComboBox3
            // 
            this.toolStripComboBox3.Name = "toolStripComboBox3";
            this.toolStripComboBox3.Size = new System.Drawing.Size(121, 23);
            this.toolStripComboBox3.Text = "Range: 250LY";
            this.toolStripComboBox3.Click += new System.EventHandler(this.toolStripComboBox3_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripComboBox1,
            this.toolStripComboBox2});
            this.menuStrip1.Location = new System.Drawing.Point(0, 5);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(134, 27);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // toolStripComboBox1
            // 
            this.toolStripComboBox1.Name = "toolStripComboBox1";
            this.toolStripComboBox1.Size = new System.Drawing.Size(121, 23);
            this.toolStripComboBox1.Text = "Show All Pois";
            this.toolStripComboBox1.Click += new System.EventHandler(this.toolStripComboBox1_Click);
            // 
            // toolStripComboBox2
            // 
            this.toolStripComboBox2.Name = "toolStripComboBox2";
            this.toolStripComboBox2.Size = new System.Drawing.Size(121, 23);
            this.toolStripComboBox2.Text = "EDSM";
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
            this.tableLayoutPanel3.Location = new System.Drawing.Point(21, 207);
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
            this.label1.Click += new System.EventHandler(this.label1_Click_2);
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
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // EDDCanonnUserControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoScroll = true;
            this.Controls.Add(this.tableLayoutPanel3);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.gridData);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "EDDCanonnUserControl";
            this.Size = new System.Drawing.Size(407, 512);
            this.Load += new System.EventHandler(this.EDDCanonnUserControl_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.gridData.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridAuto)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
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
        private DataGridView dataGridView1;
        private TableLayoutPanel tableLayoutPanel2;
        private MenuStrip menuStrip1;
        private MenuStrip menuStrip2;
        private ToolStripComboBox toolStripComboBox3;
        private ToolStripComboBox toolStripComboBox1;
        private ToolStripComboBox toolStripComboBox2;
        private TableLayoutPanel tableLayoutPanel3;
        private Button button1;
        private Button button2;
        private Label label1;
        private TextBox textBox1;
        private DataGridView dataGridView2;
        private DataGridViewTextBoxColumn Column1;
        private DataGridViewTextBoxColumn Column2;
        private DataGridViewTextBoxColumn Column3;
        private Button button3;
        private DataGridView gridAuto;
        private TabPage tabPage4;
    }
}
