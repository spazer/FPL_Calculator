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
        /// <summary>
        /// Base liquipedia link
        /// </summary>
        const string WIKIBASE = "http://wiki.teamliquid.net/starcraft2/index.php?title=";

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

        #region ProgressUpdate
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
        #endregion ProgressUpdate

        #region BruteTrading
        private void buttonBruteForceTrading_Click(object sender, EventArgs e)
        {
            // Start the background worker if it is not already active
            if (!backgroundWorkerBruteForce.IsBusy && !backgroundWorkerBruteTrading.IsBusy)
            {
                toolStripProgressBar.Visible = true;
                backgroundWorkerBruteTrading.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Responds when the background worker is activated
        /// </summary>
        /// <param name="sender">Object calling this function</param>
        /// <param name="e">Event arguments</param>
        private void backgroundWorkerBruteTrading_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            // Get HTML from Liquipedia
            backgroundWorkerBruteTrading.ReportProgress(0, "Retrieving Liquipedia page");
            WebFetch page = new WebFetch(textBoxURL.Text + "&action=edit", errorwriter);

            if (page.pageContent != string.Empty)
            {
                page.ReduceToWikiMarkup();
                if (page.pageContent.IndexOf("#REDIRECT") != -1)
                {
                    int start = page.pageContent.IndexOf("[[") + 2;
                    int end = page.pageContent.IndexOf("]]");
                    page = new WebFetch(WIKIBASE + page.pageContent.Substring(start, end - start) + "&action=edit", errorwriter);
                    page.ReduceToWikiMarkup();
                }

                // Limit the number of weeks we consider
                int maxWeeks;
                bool result = int.TryParse(textBoxWeeks.Text, out maxWeeks);
                if (!result)
                {
                    maxWeeks = 0;
                }

                // Read in data from the webpage
                backgroundWorkerBruteTrading.ReportProgress(0, "Parsing webpage");
                statistics.ParseMarkup(page.pageContent, maxWeeks);

                // Print all the data we've retrieved
                statistics.PrintPlayerStats();
                statistics.PrintTeamStats();

                // Limit the number of players we consider
                int maxPlayers;
                result = int.TryParse(textBoxPlayersPerWeek.Text, out maxPlayers);
                if (!result)
                {
                    maxPlayers = 0;
                }

                // Brute force the solution
                backgroundWorkerBruteTrading.ReportProgress(0, "Brute forcing solution");
                statistics.BestTeamGenerationWithTrades(maxPlayers);
                backgroundWorkerBruteTrading.ReportProgress(0, "Solution complete");
            }
            else
            {
                backgroundWorkerBruteTrading.ReportProgress(0, "Couldn't retrieve Liquipedia page");
            }
        }

        /// <summary>
        /// Responds when the background worker sends a progress update
        /// </summary>
        /// <param name="sender">Object calling this function</param>
        /// <param name="e">Event arguments</param>
        private void backgroundWorkerBruteTrading_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string message = e.UserState as string;
            toolStripStatusLabel.Text = message;
        }

        /// <summary>
        /// Responds when the background worker finishes its tasks
        /// </summary>
        /// <param name="sender">Object calling this function</param>
        /// <param name="e">Event arguments</param>
        private void backgroundWorkerBruteTrading_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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
        #endregion BruteTrading

        #region BruteForce
        /// <summary>
        /// Responds when buttonBruteForce is clicked
        /// </summary>
        /// <param name="sender">Object calling this function</param>
        /// <param name="e">Event arguments</param>
        private void buttonBruteForce_Click(object sender, EventArgs e)
        {
            // Start the background worker if it is not already active
            if (!backgroundWorkerBruteForce.IsBusy && !backgroundWorkerBruteTrading.IsBusy)
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

            if (page.pageContent != string.Empty)
            {
                page.ReduceToWikiMarkup();
                if (page.pageContent.IndexOf("#REDIRECT") != -1)
                {
                    int start = page.pageContent.IndexOf("[[") + 2;
                    int end = page.pageContent.IndexOf("]]");
                    page = new WebFetch(WIKIBASE + page.pageContent.Substring(start, end - start) + "&action=edit", errorwriter);
                    page.ReduceToWikiMarkup();
                }

                // Limit the number of weeks we consider
                int maxWeeks;
                bool result = int.TryParse(textBoxWeeks.Text, out maxWeeks);
                if (!result)
                {
                    maxWeeks = 0;
                }

                // Read in data from the webpage
                backgroundWorkerBruteForce.ReportProgress(0, "Parsing webpage");
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
        #endregion BruteForce

        #region Algorithmic
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

            if (page.pageContent != string.Empty)
            {
                page.ReduceToWikiMarkup();
                if (page.pageContent.IndexOf("#REDIRECT") != -1)
                {
                    int start = page.pageContent.IndexOf("[[") + 2;
                    int end = page.pageContent.IndexOf("]]");
                    page = new WebFetch(WIKIBASE + page.pageContent.Substring(start, end - start) + "&action=edit", errorwriter);
                    page.ReduceToWikiMarkup();
                }
                
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
        #endregion Algorithmic

        #region BruteTradeAnti
        private void buttonBruteTradeAnti_Click(object sender, EventArgs e)
        {
            // Start the background worker if it is not already active
            if (!backgroundWorkerBruteForce.IsBusy && !backgroundWorkerBruteTradeAnti.IsBusy)
            {
                toolStripProgressBar.Visible = true;
                backgroundWorkerBruteTradeAnti.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Responds when the background worker is activated
        /// </summary>
        /// <param name="sender">Object calling this function</param>
        /// <param name="e">Event arguments</param>
        private void backgroundWorkerBruteTradeAnti_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            // Get HTML from Liquipedia
            backgroundWorkerBruteTradeAnti.ReportProgress(0, "Retrieving Liquipedia page");
            WebFetch page = new WebFetch(textBoxURL.Text + "&action=edit", errorwriter);

            if (page.pageContent != string.Empty)
            {
                page.ReduceToWikiMarkup();
                if (page.pageContent.IndexOf("#REDIRECT") != -1)
                {
                    int start = page.pageContent.IndexOf("[[") + 2;
                    int end = page.pageContent.IndexOf("]]");
                    page = new WebFetch(WIKIBASE + page.pageContent.Substring(start, end - start) + "&action=edit", errorwriter);
                    page.ReduceToWikiMarkup();
                }

                // Limit the number of weeks we consider
                int maxWeeks;
                bool result = int.TryParse(textBoxWeeks.Text, out maxWeeks);
                if (!result)
                {
                    maxWeeks = 0;
                }

                // Read in data from the webpage
                backgroundWorkerBruteTradeAnti.ReportProgress(0, "Parsing webpage");
                statistics.ParseMarkup(page.pageContent, maxWeeks);

                // Print all the data we've retrieved
                statistics.PrintPlayerStats();
                statistics.PrintTeamStats();

                // Limit the number of players we consider
                int maxPlayers;
                result = int.TryParse(textBoxPlayersPerWeek.Text, out maxPlayers);
                if (!result)
                {
                    maxPlayers = 0;
                }

                // Brute force the solution
                backgroundWorkerBruteTradeAnti.ReportProgress(0, "Brute forcing anti");
                statistics.BestAntiGenerationWithTrades(maxPlayers);
                backgroundWorkerBruteTradeAnti.ReportProgress(0, "Solution complete");
            }
            else
            {
                backgroundWorkerBruteTradeAnti.ReportProgress(0, "Couldn't retrieve Liquipedia page");
            }
        }

        /// <summary>
        /// Responds when the background worker sends a progress update
        /// </summary>
        /// <param name="sender">Object calling this function</param>
        /// <param name="e">Event arguments</param>
        private void backgroundWorkerBruteTradeAnti_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string message = e.UserState as string;
            toolStripStatusLabel.Text = message;
        }

        /// <summary>
        /// Responds when the background worker finishes its tasks
        /// </summary>
        /// <param name="sender">Object calling this function</param>
        /// <param name="e">Event arguments</param>
        private void backgroundWorkerBruteTradeAnti_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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
        #endregion BruteTradeAnti
    }
}
