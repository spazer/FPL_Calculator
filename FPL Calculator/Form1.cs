using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FPL_Calculator
{
    public partial class Form1 : Form
    {
        private delegate void progressUpdateDelegate(int progress);
        
        ErrorWriter errorwriter = new ErrorWriter();
        Stats statistics;

        public Form1()
        {
            InitializeComponent();

            statistics = new Stats(errorwriter);
            statistics.ProgressUpdate += new Stats.progressEventHandler(statistics_ProgressUpdate);
        }

        void statistics_ProgressUpdate(int progress)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new progressUpdateDelegate(statistics_ProgressUpdate), progress);
                return;
            }
            else
            {
                if (toolStripProgressBar.Visible == true)
                {
                    toolStripProgressBar.Value = progress;
                }
            }
        }

        private void buttonBruteForce_Click(object sender, EventArgs e)
        {
            if (!backgroundWorkerBruteForce.IsBusy)
            {
                toolStripProgressBar.Visible = true;
                backgroundWorkerBruteForce.RunWorkerAsync();
            }
        }

        private void backgroundWorkerBruteForce_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            
            backgroundWorkerBruteForce.ReportProgress(0, "Retrieving Liquipedia page");
            WebFetch page = new WebFetch(textBoxURL.Text + "&action=edit", errorwriter);

            //StreamReader reader = new StreamReader(".//example.txt");
            //WebFetch page = new WebFetch(reader);

            if (page.pageContent != string.Empty)
            {
                page.ReduceToWikiMarkup();
                
                int maxWeeks;
                bool result = int.TryParse(textBoxWeeks.Text, out maxWeeks);
                if (!result)
                {
                    maxWeeks = 0;
                }

                backgroundWorkerBruteForce.ReportProgress(0, "Parsing webpage");
                statistics.ParseMarkup(page.pageContent, maxWeeks);
                statistics.PrintPlayerStats();
                statistics.PrintTeamStats();

                int maxPlayers;
                result = int.TryParse(textBoxNumPlayers.Text, out maxPlayers);
                if (!result)
                {
                    maxPlayers = 0;
                }

                backgroundWorkerBruteForce.ReportProgress(0, "Brute forcing solution");
                statistics.BruteForceMainNoTrades(maxPlayers);
                backgroundWorkerBruteForce.ReportProgress(0, "Solution complete");
            }
            else
            {
                backgroundWorkerBruteForce.ReportProgress(0, "Couldn't retrieve Liquipedia page");
            }
        }

        private void backgroundWorkerBruteForce_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string message = e.UserState as string;
            toolStripStatusLabel.Text = message;
        }

        private void backgroundWorkerBruteForce_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            toolStripProgressBar.Value = 0;
            toolStripProgressBar.Visible = false;

            if (e.Cancelled == true)
            {
                toolStripStatusLabel.Text = "Operation canceled";
            }
            else if (e.Error != null)
            {
                toolStripStatusLabel.Text = "Error: " + e.Error.Message;
            }
        }

        private void buttonParse_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel.Text = "Retrieving Liquipedia page";
            WebFetch page = new WebFetch(textBoxURL.Text + "&action=edit", errorwriter);

            //StreamReader reader = new StreamReader(".//example.txt");
            //WebFetch page = new WebFetch(reader);

            if (page.pageContent != string.Empty)
            {
                page.ReduceToWikiMarkup();
                Stats statistics = new Stats(errorwriter);

                int maxWeeks;
                bool result = int.TryParse(textBoxWeeks.Text, out maxWeeks);
                if (!result)
                {
                    maxWeeks = 0;
                }

                toolStripStatusLabel.Text = "Parsing webpage";
                statistics.ParseMarkup(page.pageContent, maxWeeks);
                statistics.PrintPlayerStats();
                statistics.PrintTeamStats();

                toolStripStatusLabel.Text = "Calculating best main team";
                statistics.BestMainWithoutTrades();

                toolStripStatusLabel.Text = "Tasks complete";
            }
            else
            {
                toolStripStatusLabel.Text = "Error retrieving Liquipedia page";
                MessageBox.Show("Could not retrieve page. Check your connection.");
            }
        }

        private long factorial(long n)
        {
            long result = 1;
            for (int i = 2; i <= n; i++)
            {
                result *= i;
            }

            return result;
        }
    }
}
