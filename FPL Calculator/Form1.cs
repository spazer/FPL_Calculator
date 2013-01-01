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
        #region Delegates
        /// <summary>
        /// Delegate for progress updates
        /// </summary>
        /// <param name="progress">Progress from 0-100</param>
        private delegate void progressUpdateDelegate(int progress);
        #endregion Delegates

        #region Variables
        /// <summary>
        /// Writes messages to a logfile
        /// </summary>
        ErrorWriter errorwriter = new ErrorWriter();

        /// <summary>
        /// Contains data for caclulating the best team
        /// </summary>
        Stats statistics;
        #endregion Variables

        #region Constructor
        public Form1()
        {
            InitializeComponent();

            statistics = new Stats(errorwriter);
            statistics.ProgressUpdate += new Stats.progressEventHandler(statistics_ProgressUpdate);
        }
        #endregion Constructor

        /// <summary>
        /// Handles progress update events from the Statistics class
        /// </summary>
        /// <param name="progress">Progress from 0-100</param>
        void statistics_ProgressUpdate(int progress)
        {
            // To make this thread safe
            if (InvokeRequired)
            {
                BeginInvoke(new progressUpdateDelegate(statistics_ProgressUpdate), progress);
                return;
            }
            else
            {
                // Update progress if the progress bar is visible
                if (toolStripProgressBar.Visible == true)
                {
                    toolStripProgressBar.Value = progress;
                }
            }
        }

        /// <summary>
        /// Responds when buttonBruteForce is clicked
        /// </summary>
        /// <param name="sender">Object calling this function</param>
        /// <param name="e">Event arguments</param>
        private void buttonBruteForce_Click(object sender, EventArgs e)
        {
            // Start the background worker if it is not already active
            if (!backgroundWorkerBruteForce.IsBusy)
            {
                toolStripProgressBar.Visible = true;
                backgroundWorkerBruteForce.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Responds when the background worker is activated
        /// </summary>
        /// <param name="sender">Object calling this function</param>
        /// <param name="e">Event arguments</param>
        private void backgroundWorkerBruteForce_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            
            // Get HTML from Liquipedia
            backgroundWorkerBruteForce.ReportProgress(0, "Retrieving Liquipedia page");
            WebFetch page = new WebFetch(textBoxURL.Text + "&action=edit", errorwriter);

            // Reads HTML from a file - for debugging only
            //StreamReader reader = new StreamReader(".//example.txt");
            //WebFetch page = new WebFetch(reader);

            if (page.pageContent != string.Empty)
            {
                page.ReduceToWikiMarkup();

                // Limit the number of weeks we consider
                int maxWeeks;
                bool result = int.TryParse(textBoxWeeks.Text, out maxWeeks);
                if (!result)
                {
                    maxWeeks = 0;
                }

                // Read in data from the webpage
                toolStripStatusLabel.Text = "Parsing webpage";
                statistics.ParseMarkup(page.pageContent, maxWeeks);

                // Print all the data we've retrieved
                statistics.PrintPlayerStats();
                statistics.PrintTeamStats();

                // Limit the number of players we consider
                int maxPlayers;
                result = int.TryParse(textBoxNumPlayers.Text, out maxPlayers);
                if (!result)
                {
                    maxPlayers = 0;
                }

                // Brute force the solution
                backgroundWorkerBruteForce.ReportProgress(0, "Brute forcing solution");
                statistics.BruteForceMainNoTrades(maxPlayers);
                backgroundWorkerBruteForce.ReportProgress(0, "Solution complete");
            }
            else
            {
                backgroundWorkerBruteForce.ReportProgress(0, "Couldn't retrieve Liquipedia page");
            }
        }

        /// <summary>
        /// Responds when the background worker sends a progress update
        /// </summary>
        /// <param name="sender">Object calling this function</param>
        /// <param name="e">Event arguments</param>
        private void backgroundWorkerBruteForce_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string message = e.UserState as string;
            toolStripStatusLabel.Text = message;
        }

        /// <summary>
        /// Responds when the background worker finishes its tasks
        /// </summary>
        /// <param name="sender">Object calling this function</param>
        /// <param name="e">Event arguments</param>
        private void backgroundWorkerBruteForce_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Update the status bar
            toolStripProgressBar.Value = 0;
            toolStripProgressBar.Visible = false;

            if (e.Cancelled == true)
            {
                toolStripStatusLabel.Text = "Operation canceled";
            }
            else if (e.Error != null)
            {
                toolStripStatusLabel.Text = "Error: " + e.Error.Message;
                errorwriter.Write(e.Error.Message);
            }
        }

        /// <summary>
        /// Responds when buttonParse is clicked
        /// </summary>
        /// <param name="sender">Object calling this function</param>
        /// <param name="e">Event arguments</param>
        private void buttonParse_Click(object sender, EventArgs e)
        {
            // Get HTML from Liquipedia
            toolStripStatusLabel.Text = "Retrieving Liquipedia page";
            WebFetch page = new WebFetch(textBoxURL.Text + "&action=edit", errorwriter);

            // Reads HTML from a file - for debugging only
            // StreamReader reader = new StreamReader(".//example.txt");
            // WebFetch page = new WebFetch(reader);

            if (page.pageContent != string.Empty)
            {
                page.ReduceToWikiMarkup();
                Stats statistics = new Stats(errorwriter);

                // Limit the number of weeks we consider
                int maxWeeks;
                bool result = int.TryParse(textBoxWeeks.Text, out maxWeeks);
                if (!result)
                {
                    maxWeeks = 0;
                }

                // Read in data from the webpage
                toolStripStatusLabel.Text = "Parsing webpage";
                statistics.ParseMarkup(page.pageContent, maxWeeks);

                // Print all the data we've retrieved
                statistics.PrintPlayerStats();
                statistics.PrintTeamStats();

                // Calculate the best team
                toolStripStatusLabel.Text = "Calculating best main team";
                statistics.BestMainWithoutTrades();

                toolStripStatusLabel.Text = "Solution complete";
            }
            else
            {
                toolStripStatusLabel.Text = "Error retrieving Liquipedia page";
                MessageBox.Show("Could not retrieve page. Check your connection.");
            }
        }
    }
}
