namespace FPL_Calculator
{
    partial class Form1
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
            this.buttonParse = new System.Windows.Forms.Button();
            this.textBoxURL = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxWeeks = new System.Windows.Forms.TextBox();
            this.textBoxNumPlayers = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonBruteForce = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.statusStripStatus = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.backgroundWorkerBruteForce = new System.ComponentModel.BackgroundWorker();
            this.textBoxPlayersPerWeek = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonBruteTradeAnti = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.buttonBruteForceTrading = new System.Windows.Forms.Button();
            this.backgroundWorkerBruteTrading = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerBruteTradeAnti = new System.ComponentModel.BackgroundWorker();
            this.groupBox1.SuspendLayout();
            this.statusStripStatus.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonParse
            // 
            this.buttonParse.Location = new System.Drawing.Point(12, 226);
            this.buttonParse.Name = "buttonParse";
            this.buttonParse.Size = new System.Drawing.Size(75, 23);
            this.buttonParse.TabIndex = 0;
            this.buttonParse.Text = "Parse Page";
            this.buttonParse.UseVisualStyleBackColor = true;
            this.buttonParse.Click += new System.EventHandler(this.buttonParse_Click);
            // 
            // textBoxURL
            // 
            this.textBoxURL.Location = new System.Drawing.Point(12, 25);
            this.textBoxURL.Name = "textBoxURL";
            this.textBoxURL.Size = new System.Drawing.Size(536, 20);
            this.textBoxURL.TabIndex = 1;
            this.textBoxURL.Text = "http://wiki.teamliquid.net/starcraft2/index.php?title=2012%E2%80%932013_Proleague" +
    "/Round_1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(196, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Liquipedia URL for the round goes here:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(148, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Number of weeks to consider:";
            // 
            // textBoxWeeks
            // 
            this.textBoxWeeks.Location = new System.Drawing.Point(12, 64);
            this.textBoxWeeks.Name = "textBoxWeeks";
            this.textBoxWeeks.Size = new System.Drawing.Size(43, 20);
            this.textBoxWeeks.TabIndex = 6;
            // 
            // textBoxNumPlayers
            // 
            this.textBoxNumPlayers.Location = new System.Drawing.Point(9, 30);
            this.textBoxNumPlayers.Name = "textBoxNumPlayers";
            this.textBoxNumPlayers.Size = new System.Drawing.Size(43, 20);
            this.textBoxNumPlayers.TabIndex = 7;
            this.textBoxNumPlayers.Text = "40";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(172, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Brute force solutions using the best";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(58, 37);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "players";
            // 
            // buttonBruteForce
            // 
            this.buttonBruteForce.Location = new System.Drawing.Point(9, 139);
            this.buttonBruteForce.Name = "buttonBruteForce";
            this.buttonBruteForce.Size = new System.Drawing.Size(75, 23);
            this.buttonBruteForce.TabIndex = 0;
            this.buttonBruteForce.Text = "Brute Force";
            this.buttonBruteForce.UseVisualStyleBackColor = true;
            this.buttonBruteForce.Click += new System.EventHandler(this.buttonBruteForce_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.textBoxNumPlayers);
            this.groupBox1.Controls.Add(this.buttonBruteForce);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(183, 81);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(208, 168);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Brute Force Options";
            // 
            // statusStripStatus
            // 
            this.statusStripStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel,
            this.toolStripProgressBar});
            this.statusStripStatus.Location = new System.Drawing.Point(0, 257);
            this.statusStripStatus.Name = "statusStripStatus";
            this.statusStripStatus.Size = new System.Drawing.Size(636, 22);
            this.statusStripStatus.TabIndex = 14;
            this.statusStripStatus.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(26, 17);
            this.toolStripStatusLabel.Text = "Idle";
            // 
            // toolStripProgressBar
            // 
            this.toolStripProgressBar.Name = "toolStripProgressBar";
            this.toolStripProgressBar.Size = new System.Drawing.Size(100, 16);
            this.toolStripProgressBar.Visible = false;
            // 
            // backgroundWorkerBruteForce
            // 
            this.backgroundWorkerBruteForce.WorkerReportsProgress = true;
            this.backgroundWorkerBruteForce.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerBruteForce_DoWork);
            this.backgroundWorkerBruteForce.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorkerBruteForce_ProgressChanged);
            this.backgroundWorkerBruteForce.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerBruteForce_RunWorkerCompleted);
            // 
            // textBoxPlayersPerWeek
            // 
            this.textBoxPlayersPerWeek.Location = new System.Drawing.Point(9, 34);
            this.textBoxPlayersPerWeek.Name = "textBoxPlayersPerWeek";
            this.textBoxPlayersPerWeek.Size = new System.Drawing.Size(43, 20);
            this.textBoxPlayersPerWeek.TabIndex = 10;
            this.textBoxPlayersPerWeek.Text = "20";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(58, 37);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(87, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "players per week";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.buttonBruteTradeAnti);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.buttonBruteForceTrading);
            this.groupBox2.Controls.Add(this.textBoxPlayersPerWeek);
            this.groupBox2.Location = new System.Drawing.Point(397, 81);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(216, 168);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Brute Force With Trading Options";
            // 
            // buttonBruteTradeAnti
            // 
            this.buttonBruteTradeAnti.Location = new System.Drawing.Point(90, 139);
            this.buttonBruteTradeAnti.Name = "buttonBruteTradeAnti";
            this.buttonBruteTradeAnti.Size = new System.Drawing.Size(112, 23);
            this.buttonBruteTradeAnti.TabIndex = 12;
            this.buttonBruteTradeAnti.Text = "Brute Force Anti";
            this.buttonBruteTradeAnti.UseVisualStyleBackColor = true;
            this.buttonBruteTradeAnti.Click += new System.EventHandler(this.buttonBruteTradeAnti_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(202, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Brute force solutions using the best/worst";
            // 
            // buttonBruteForceTrading
            // 
            this.buttonBruteForceTrading.Location = new System.Drawing.Point(9, 139);
            this.buttonBruteForceTrading.Name = "buttonBruteForceTrading";
            this.buttonBruteForceTrading.Size = new System.Drawing.Size(75, 23);
            this.buttonBruteForceTrading.TabIndex = 0;
            this.buttonBruteForceTrading.Text = "Brute Force";
            this.buttonBruteForceTrading.UseVisualStyleBackColor = true;
            this.buttonBruteForceTrading.Click += new System.EventHandler(this.buttonBruteForceTrading_Click);
            // 
            // backgroundWorkerBruteTrading
            // 
            this.backgroundWorkerBruteTrading.WorkerReportsProgress = true;
            this.backgroundWorkerBruteTrading.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerBruteTrading_DoWork);
            this.backgroundWorkerBruteTrading.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorkerBruteTrading_ProgressChanged);
            this.backgroundWorkerBruteTrading.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerBruteTrading_RunWorkerCompleted);
            // 
            // backgroundWorkerBruteTradeAnti
            // 
            this.backgroundWorkerBruteTradeAnti.WorkerReportsProgress = true;
            this.backgroundWorkerBruteTradeAnti.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerBruteTradeAnti_DoWork);
            this.backgroundWorkerBruteTradeAnti.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorkerBruteTradeAnti_ProgressChanged);
            this.backgroundWorkerBruteTradeAnti.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerBruteTradeAnti_RunWorkerCompleted);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(636, 279);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.statusStripStatus);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.textBoxWeeks);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxURL);
            this.Controls.Add(this.buttonParse);
            this.Name = "Form1";
            this.Text = "FPL Calculator";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.statusStripStatus.ResumeLayout(false);
            this.statusStripStatus.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonParse;
        private System.Windows.Forms.TextBox textBoxURL;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxWeeks;
        private System.Windows.Forms.TextBox textBoxNumPlayers;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonBruteForce;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.StatusStrip statusStripStatus;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.ComponentModel.BackgroundWorker backgroundWorkerBruteForce;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
        private System.Windows.Forms.TextBox textBoxPlayersPerWeek;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button buttonBruteForceTrading;
        private System.ComponentModel.BackgroundWorker backgroundWorkerBruteTrading;
        private System.Windows.Forms.Button buttonBruteTradeAnti;
        private System.ComponentModel.BackgroundWorker backgroundWorkerBruteTradeAnti;
    }
}

