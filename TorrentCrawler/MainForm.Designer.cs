namespace TorrentCrawler
{
    partial class MainForm
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
            this.titleLabel = new System.Windows.Forms.Label();
            this.AnalyzeArchiveButton = new System.Windows.Forms.Button();
            this.AnalyzeNewestButton = new System.Windows.Forms.Button();
            this.TestButton = new System.Windows.Forms.Button();
            this.ArchiveProgressBar = new System.Windows.Forms.ProgressBar();
            this.TorrentsNotRespondingLabel = new System.Windows.Forms.Label();
            this.BlocksCompletedLabel = new System.Windows.Forms.Label();
            this.StatsLabel = new System.Windows.Forms.Label();
            this.TorrentsUpdatedLabel = new System.Windows.Forms.Label();
            this.TorrentsInsertedLabel = new System.Windows.Forms.Label();
            this.TotalTorrentsLabel = new System.Windows.Forms.Label();
            this.TimeLabel = new System.Windows.Forms.Label();
            this.StartingBlockInput = new System.Windows.Forms.NumericUpDown();
            this.StartingBlockLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.StartingBlockInput)).BeginInit();
            this.SuspendLayout();
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Palatino Linotype", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLabel.Location = new System.Drawing.Point(140, 44);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(507, 87);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "Torrent Crawler";
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AnalyzeArchiveButton
            // 
            this.AnalyzeArchiveButton.Location = new System.Drawing.Point(427, 170);
            this.AnalyzeArchiveButton.Name = "AnalyzeArchiveButton";
            this.AnalyzeArchiveButton.Size = new System.Drawing.Size(220, 68);
            this.AnalyzeArchiveButton.TabIndex = 1;
            this.AnalyzeArchiveButton.Text = "Analyze the KickAss Archive";
            this.AnalyzeArchiveButton.UseVisualStyleBackColor = true;
            this.AnalyzeArchiveButton.Click += new System.EventHandler(this.analyzeKickAssArchive_Click);
            // 
            // AnalyzeNewestButton
            // 
            this.AnalyzeNewestButton.Location = new System.Drawing.Point(295, 272);
            this.AnalyzeNewestButton.Name = "AnalyzeNewestButton";
            this.AnalyzeNewestButton.Size = new System.Drawing.Size(222, 68);
            this.AnalyzeNewestButton.TabIndex = 2;
            this.AnalyzeNewestButton.Text = "Analyze Newest";
            this.AnalyzeNewestButton.UseVisualStyleBackColor = true;
            this.AnalyzeNewestButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.AnalyzeNewestButton_MouseClick);
            // 
            // TestButton
            // 
            this.TestButton.Location = new System.Drawing.Point(312, 363);
            this.TestButton.Name = "TestButton";
            this.TestButton.Size = new System.Drawing.Size(176, 49);
            this.TestButton.TabIndex = 3;
            this.TestButton.Text = "Test Button";
            this.TestButton.UseVisualStyleBackColor = true;
            this.TestButton.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TestButton_MouseClick);
            // 
            // ArchiveProgressBar
            // 
            this.ArchiveProgressBar.Location = new System.Drawing.Point(106, 435);
            this.ArchiveProgressBar.Maximum = 1498;
            this.ArchiveProgressBar.Name = "ArchiveProgressBar";
            this.ArchiveProgressBar.Size = new System.Drawing.Size(626, 49);
            this.ArchiveProgressBar.Step = 1;
            this.ArchiveProgressBar.TabIndex = 4;
            // 
            // TorrentsNotRespondingLabel
            // 
            this.TorrentsNotRespondingLabel.AutoSize = true;
            this.TorrentsNotRespondingLabel.Location = new System.Drawing.Point(794, 272);
            this.TorrentsNotRespondingLabel.Name = "TorrentsNotRespondingLabel";
            this.TorrentsNotRespondingLabel.Size = new System.Drawing.Size(148, 13);
            this.TorrentsNotRespondingLabel.TabIndex = 5;
            this.TorrentsNotRespondingLabel.Text = "Torrents that didn\'t respond: 0";
            // 
            // BlocksCompletedLabel
            // 
            this.BlocksCompletedLabel.AutoSize = true;
            this.BlocksCompletedLabel.Location = new System.Drawing.Point(794, 198);
            this.BlocksCompletedLabel.Name = "BlocksCompletedLabel";
            this.BlocksCompletedLabel.Size = new System.Drawing.Size(128, 13);
            this.BlocksCompletedLabel.TabIndex = 6;
            this.BlocksCompletedLabel.Text = "Blocks completed until: -1";
            // 
            // StatsLabel
            // 
            this.StatsLabel.AutoSize = true;
            this.StatsLabel.Font = new System.Drawing.Font("Palatino Linotype", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StatsLabel.Location = new System.Drawing.Point(789, 130);
            this.StatsLabel.Name = "StatsLabel";
            this.StatsLabel.Size = new System.Drawing.Size(252, 47);
            this.StatsLabel.TabIndex = 7;
            this.StatsLabel.Text = "Stats this Run:";
            // 
            // TorrentsUpdatedLabel
            // 
            this.TorrentsUpdatedLabel.AutoSize = true;
            this.TorrentsUpdatedLabel.Location = new System.Drawing.Point(794, 250);
            this.TorrentsUpdatedLabel.Name = "TorrentsUpdatedLabel";
            this.TorrentsUpdatedLabel.Size = new System.Drawing.Size(100, 13);
            this.TorrentsUpdatedLabel.TabIndex = 8;
            this.TorrentsUpdatedLabel.Text = "Torrents updated: 0";
            // 
            // TorrentsInsertedLabel
            // 
            this.TorrentsInsertedLabel.AutoSize = true;
            this.TorrentsInsertedLabel.Location = new System.Drawing.Point(794, 232);
            this.TorrentsInsertedLabel.Name = "TorrentsInsertedLabel";
            this.TorrentsInsertedLabel.Size = new System.Drawing.Size(172, 13);
            this.TorrentsInsertedLabel.TabIndex = 9;
            this.TorrentsInsertedLabel.Text = "Torrents inserted for the first time: 0";
            // 
            // TotalTorrentsLabel
            // 
            this.TotalTorrentsLabel.AutoSize = true;
            this.TotalTorrentsLabel.Location = new System.Drawing.Point(794, 308);
            this.TotalTorrentsLabel.Name = "TotalTorrentsLabel";
            this.TotalTorrentsLabel.Size = new System.Drawing.Size(134, 13);
            this.TotalTorrentsLabel.TabIndex = 10;
            this.TotalTorrentsLabel.Text = "Total inspected Torrents: 0";
            // 
            // TimeLabel
            // 
            this.TimeLabel.AutoSize = true;
            this.TimeLabel.Location = new System.Drawing.Point(804, 363);
            this.TimeLabel.Name = "TimeLabel";
            this.TimeLabel.Size = new System.Drawing.Size(85, 13);
            this.TimeLabel.TabIndex = 11;
            this.TimeLabel.Text = "Time Running: 0";
            // 
            // StartingBlockInput
            // 
            this.StartingBlockInput.Location = new System.Drawing.Point(260, 184);
            this.StartingBlockInput.Maximum = new decimal(new int[] {
            1498,
            0,
            0,
            0});
            this.StartingBlockInput.Name = "StartingBlockInput";
            this.StartingBlockInput.Size = new System.Drawing.Size(120, 20);
            this.StartingBlockInput.TabIndex = 12;
            // 
            // StartingBlockLabel
            // 
            this.StartingBlockLabel.AutoSize = true;
            this.StartingBlockLabel.Location = new System.Drawing.Point(152, 186);
            this.StartingBlockLabel.Name = "StartingBlockLabel";
            this.StartingBlockLabel.Size = new System.Drawing.Size(85, 13);
            this.StartingBlockLabel.TabIndex = 13;
            this.StartingBlockLabel.Text = "Start from Block:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Info;
            this.ClientSize = new System.Drawing.Size(1145, 529);
            this.Controls.Add(this.StartingBlockLabel);
            this.Controls.Add(this.StartingBlockInput);
            this.Controls.Add(this.TimeLabel);
            this.Controls.Add(this.TotalTorrentsLabel);
            this.Controls.Add(this.TorrentsInsertedLabel);
            this.Controls.Add(this.TorrentsUpdatedLabel);
            this.Controls.Add(this.StatsLabel);
            this.Controls.Add(this.BlocksCompletedLabel);
            this.Controls.Add(this.TorrentsNotRespondingLabel);
            this.Controls.Add(this.ArchiveProgressBar);
            this.Controls.Add(this.TestButton);
            this.Controls.Add(this.AnalyzeNewestButton);
            this.Controls.Add(this.AnalyzeArchiveButton);
            this.Controls.Add(this.titleLabel);
            this.Name = "MainForm";
            this.Text = "MainWindow";
            ((System.ComponentModel.ISupportInitialize)(this.StartingBlockInput)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Button AnalyzeArchiveButton;
        private System.Windows.Forms.Button AnalyzeNewestButton;
        private System.Windows.Forms.Button TestButton;
        private System.Windows.Forms.ProgressBar ArchiveProgressBar;
        private System.Windows.Forms.Label TorrentsNotRespondingLabel;
        private System.Windows.Forms.Label BlocksCompletedLabel;
        private System.Windows.Forms.Label StatsLabel;
        private System.Windows.Forms.Label TorrentsUpdatedLabel;
        private System.Windows.Forms.Label TorrentsInsertedLabel;
        private System.Windows.Forms.Label TotalTorrentsLabel;
        private System.Windows.Forms.Label TimeLabel;
        private System.Windows.Forms.NumericUpDown StartingBlockInput;
        private System.Windows.Forms.Label StartingBlockLabel;
    }
}