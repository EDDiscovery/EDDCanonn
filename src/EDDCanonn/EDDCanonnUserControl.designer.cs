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
            this.eventOutput = new System.Windows.Forms.TextBox();
            this.labelSystemCount = new System.Windows.Forms.Label();
            this.textBoxBodyCount = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.debug = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxSystem
            // 
            this.textBoxSystem.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textBoxSystem.Location = new System.Drawing.Point(66, 8);
            this.textBoxSystem.Name = "textBoxSystem";
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
            this.labelSysName.Click += new System.EventHandler(this.label1_Click);
            // 
            // eventOutput
            // 
            this.eventOutput.Location = new System.Drawing.Point(23, 280);
            this.eventOutput.Multiline = true;
            this.eventOutput.Name = "eventOutput";
            this.eventOutput.ReadOnly = true;
            this.eventOutput.Size = new System.Drawing.Size(358, 218);
            this.eventOutput.TabIndex = 5;
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
            this.labelSystemCount.Click += new System.EventHandler(this.label1_Click_1);
            // 
            // textBoxBodyCount
            // 
            this.textBoxBodyCount.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textBoxBodyCount.Location = new System.Drawing.Point(279, 8);
            this.textBoxBodyCount.Name = "textBoxBodyCount";
            this.textBoxBodyCount.Size = new System.Drawing.Size(82, 20);
            this.textBoxBodyCount.TabIndex = 7;
            this.textBoxBodyCount.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
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
            // debug
            // 
            this.debug.AutoSize = true;
            this.debug.Location = new System.Drawing.Point(21, 264);
            this.debug.Name = "debug";
            this.debug.Size = new System.Drawing.Size(39, 13);
            this.debug.TabIndex = 9;
            this.debug.Text = "Debug";
            this.debug.Click += new System.EventHandler(this.debug_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(20, 46);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(364, 100);
            this.tabControl1.TabIndex = 11;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(356, 74);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Missing Data";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(356, 74);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Biology";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(356, 74);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Geology";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(301, 148);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(73, 17);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "Automatic";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // EDDCanonnUserControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoScroll = true;
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.debug);
            this.Controls.Add(this.eventOutput);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "EDDCanonnUserControl";
            this.Size = new System.Drawing.Size(399, 513);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private TextBox textBoxSystem;
        private Label labelSysName;
        private TextBox eventOutput;
        private Label labelSystemCount;
        private TextBox textBoxBodyCount;
        private TableLayoutPanel tableLayoutPanel1;
        private Label debug;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private CheckBox checkBox1;
    }
}
