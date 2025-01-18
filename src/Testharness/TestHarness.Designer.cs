namespace CHash
{
    partial class TestHarness
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.buttonNJE = new System.Windows.Forms.Button();
            this.buttonAction = new System.Windows.Forms.Button();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.buttonConfigcs = new System.Windows.Forms.Button();
            this.buttonAJE = new System.Windows.Forms.Button();
            this.buttonUIEvent = new System.Windows.Forms.Button();
            this.buttonConfigwin = new System.Windows.Forms.Button();
            this.buttonUnfilteredJE = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(27, 15);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 0;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(128, 15);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(75, 23);
            this.buttonStop.TabIndex = 1;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(12, 127);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(730, 419);
            this.richTextBox1.TabIndex = 2;
            this.richTextBox1.Text = "";
            // 
            // buttonNJE
            // 
            this.buttonNJE.Location = new System.Drawing.Point(27, 57);
            this.buttonNJE.Name = "buttonNJE";
            this.buttonNJE.Size = new System.Drawing.Size(75, 23);
            this.buttonNJE.TabIndex = 1;
            this.buttonNJE.Text = "NJE";
            this.buttonNJE.UseVisualStyleBackColor = true;
            this.buttonNJE.Click += new System.EventHandler(this.buttonNJE_Click);
            // 
            // buttonAction
            // 
            this.buttonAction.Location = new System.Drawing.Point(222, 57);
            this.buttonAction.Name = "buttonAction";
            this.buttonAction.Size = new System.Drawing.Size(75, 23);
            this.buttonAction.TabIndex = 1;
            this.buttonAction.Text = "Action CMD";
            this.buttonAction.UseVisualStyleBackColor = true;
            this.buttonAction.Click += new System.EventHandler(this.buttonAction_Click);
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.Location = new System.Drawing.Point(318, 57);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(75, 23);
            this.buttonRefresh.TabIndex = 1;
            this.buttonRefresh.Text = "Refresh";
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // buttonConfigcs
            // 
            this.buttonConfigcs.Location = new System.Drawing.Point(408, 57);
            this.buttonConfigcs.Name = "buttonConfigcs";
            this.buttonConfigcs.Size = new System.Drawing.Size(75, 23);
            this.buttonConfigcs.TabIndex = 1;
            this.buttonConfigcs.Text = "Config CS";
            this.buttonConfigcs.UseVisualStyleBackColor = true;
            this.buttonConfigcs.Click += new System.EventHandler(this.buttonConfigcs_Click);
            // 
            // buttonAJE
            // 
            this.buttonAJE.Location = new System.Drawing.Point(128, 57);
            this.buttonAJE.Name = "buttonAJE";
            this.buttonAJE.Size = new System.Drawing.Size(75, 23);
            this.buttonAJE.TabIndex = 1;
            this.buttonAJE.Text = "Action JE";
            this.buttonAJE.UseVisualStyleBackColor = true;
            this.buttonAJE.Click += new System.EventHandler(this.buttonAJE_Click);
            // 
            // buttonUIEvent
            // 
            this.buttonUIEvent.Location = new System.Drawing.Point(597, 57);
            this.buttonUIEvent.Name = "buttonUIEvent";
            this.buttonUIEvent.Size = new System.Drawing.Size(75, 23);
            this.buttonUIEvent.TabIndex = 1;
            this.buttonUIEvent.Text = "UIEvent";
            this.buttonUIEvent.UseVisualStyleBackColor = true;
            this.buttonUIEvent.Click += new System.EventHandler(this.buttonUIEvent_Click);
            // 
            // buttonConfigwin
            // 
            this.buttonConfigwin.Location = new System.Drawing.Point(489, 57);
            this.buttonConfigwin.Name = "buttonConfigwin";
            this.buttonConfigwin.Size = new System.Drawing.Size(75, 23);
            this.buttonConfigwin.TabIndex = 1;
            this.buttonConfigwin.Text = "Config Win";
            this.buttonConfigwin.UseVisualStyleBackColor = true;
            this.buttonConfigwin.Click += new System.EventHandler(this.buttonConfigwin_Click);
            // 
            // buttonUnfilteredJE
            // 
            this.buttonUnfilteredJE.Location = new System.Drawing.Point(27, 86);
            this.buttonUnfilteredJE.Name = "buttonUnfilteredJE";
            this.buttonUnfilteredJE.Size = new System.Drawing.Size(75, 23);
            this.buttonUnfilteredJE.TabIndex = 1;
            this.buttonUnfilteredJE.Text = "Unfilt JE";
            this.buttonUnfilteredJE.UseVisualStyleBackColor = true;
            this.buttonUnfilteredJE.Click += new System.EventHandler(this.buttonUnfilteredJE_Click);
            // 
            // TestHarness
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(769, 558);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.buttonUIEvent);
            this.Controls.Add(this.buttonConfigwin);
            this.Controls.Add(this.buttonConfigcs);
            this.Controls.Add(this.buttonAJE);
            this.Controls.Add(this.buttonRefresh);
            this.Controls.Add(this.buttonAction);
            this.Controls.Add(this.buttonUnfilteredJE);
            this.Controls.Add(this.buttonNJE);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonStart);
            this.Name = "TestHarness";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button buttonNJE;
        private System.Windows.Forms.Button buttonAction;
        private System.Windows.Forms.Button buttonRefresh;
        private System.Windows.Forms.Button buttonConfigcs;
        private System.Windows.Forms.Button buttonAJE;
        private System.Windows.Forms.Button buttonUIEvent;
        private System.Windows.Forms.Button buttonConfigwin;
        private System.Windows.Forms.Button buttonUnfilteredJE;
    }
}

