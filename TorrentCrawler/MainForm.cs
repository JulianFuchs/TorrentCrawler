using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace TorrentCrawler
{
    public partial class MainForm : Form
    {
        private bool anyButtonClicked;
        private enum CrawlerFunction {analyzeArchive, analyzeNewest, testingFunctions};
        DebugForm debugForm;
        public MainForm()
        {
            InitializeComponent();
            anyButtonClicked = false;
            debugForm = new DebugForm();
            Thread newThread = new Thread(() => Application.Run(debugForm));
            newThread.Start();

            Program.mainForm = this;
        }

        private void analyzeKickAssArchive_Click(object sender, EventArgs e)
        {
            if (!anyButtonClicked)
            {
                anyButtonClicked = true;
                Program.debug("Starting Crawler");

                Thread newThread = new Thread(new ParameterizedThreadStart(startAnalyzing));

                newThread.Start(CrawlerFunction.analyzeArchive);
            }
        }

        private void AnalyzeNewestButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (!anyButtonClicked)
            {
                anyButtonClicked = true;
                Program.debug("Starting Crawler");

                Thread newThread = new Thread(new ParameterizedThreadStart(startAnalyzing));

                newThread.Start(CrawlerFunction.analyzeNewest);
            }
        }

        private void startAnalyzing(Object input)
        {
            CrawlerFunction crawlerFunction = (CrawlerFunction)input;
            Crawler crawler = new Crawler();
            Thread newThread = new Thread(crawler.startAnalyzingArchive); // messy

            switch (crawlerFunction)
            { 
                case CrawlerFunction.analyzeArchive:
                    newThread = new Thread(crawler.startAnalyzingArchive);
                    break;
                case CrawlerFunction.analyzeNewest:
                    newThread = new Thread(crawler.startAnalyzingNewset);
                    break;
                case CrawlerFunction.testingFunctions:
                    newThread = new Thread(crawler.startTestingFunctions);
                    break;
            }
            
            newThread.Start();

            newThread.Join();
            anyButtonClicked = false;
        }

        private void TestButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (!anyButtonClicked)
            {
                anyButtonClicked = true;
                Program.debug("Testing Crawler Functions");

                Thread newThread = new Thread(new ParameterizedThreadStart(startAnalyzing));

                newThread.Start(CrawlerFunction.testingFunctions);
            }
        }

        public class ArchiveProgressBarInitializationObject
        {
            public int maximum;
            public int current;

            public ArchiveProgressBarInitializationObject(int maximum, int current)
            {
                this.maximum = maximum;
                this.current = current;
            }
        }

        public void initializeArchiveProgressBar(Object o)
        {
            ArchiveProgressBarInitializationObject init = (ArchiveProgressBarInitializationObject)o;
            ArchiveProgressBar.Maximum = init.maximum;
            ArchiveProgressBar.Value = init.current;
        }

        public void stepArchiveProgressBar()
        {
            ArchiveProgressBar.PerformStep();
        }

        public void setBlocksCompleted(int count)
        {
            BlocksCompletedLabel.Text = "Blocks completed until: " + count;
        }

        public void setInsertedFirstTime(int count)
        {
            TorrentsInsertedLabel.Text = "Torrents inserted for the first time: " + count;
        }

        public void setUpdated(int count)
        {
            TorrentsUpdatedLabel.Text = "Torrents updated: " + count;
        }

        public void setNotResponding(int count)
        {
            TorrentsNotRespondingLabel.Text = "Torrents that didn't respond: " + count;
        }

        public void setTotalTorrents(int count)
        {
            TotalTorrentsLabel.Text = "Total inspected Torrents: " + count;
        }

        public void setTimeRunning(TimeSpan time)
        {
            int seconds = time.Seconds;
            int minutes = time.Minutes;
            int hours = time.Hours;
            int days = time.Days;
            TimeLabel.Text = "Time Running: " + days + " days; " + hours + " hours; " + minutes + " minutes; and " + seconds + " seconds";
        }

        public int startingBlockGetInput()
        {
            return Decimal.ToInt32(StartingBlockInput.Value);
        }
    }
}
