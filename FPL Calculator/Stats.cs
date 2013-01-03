using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace FPL_Calculator
{
    partial class Stats
    {
        #region Constants
        /// <summary>
        /// Determines the size of the player/team point arrays (a.k.a number of weeks)
        /// </summary>
        const int ARRAYSIZE = 10;

        /// <summary>
        /// Base liquipedia link
        /// </summary>
        const string WIKIBASE = "http://wiki.teamliquid.net/starcraft2/index.php?title=";

        /// <summary>
        /// FPL stats page
        /// </summary>
        const string FPLSTATS = "http://www.teamliquid.net/fantasy/proleague/Stats.php?r=12";
        #endregion Constants

        #region Delegates
        /// <summary>
        /// Delegate for progress updates
        /// </summary>
        /// <param name="progress">Progress from 0-100</param>
        public delegate void progressEventHandler(int progress);
        #endregion Delegates

        #region Events
        /// <summary>
        /// Event for progress updates
        /// </summary>
        public event progressEventHandler ProgressUpdate;
        #endregion Events

        #region Structs
        /// <summary>
        /// Contains data for a single solution for the algorithm based solutions
        /// </summary>
        struct chosenTeam
        {
            /// <summary>
            /// Players on the main team
            /// </summary>
            public string[] players;

            /// <summary>
            /// Chosen progaming team
            /// </summary>
            public string team;
        }
        #endregion Structs

        #region Variables
        /// <summary>
        /// Contains all teams in a list
        /// </summary>
        private Dictionary<string, Team> teamList = new Dictionary<string, Team>();

        /// <summary>
        /// Contains all players in a list
        /// </summary>
        private Dictionary<string, Player> playerList = new Dictionary<string, Player>();

        /// <summary>
        /// Writes messages to a logfile
        /// </summary>
        private ErrorWriter errorwriter;

        /// <summary>
        /// Searches for alternate player names
        /// </summary>
        private AlternateNames altSearcher;

        /// <summary>
        /// Contains players that should not be drafted in the first week
        /// </summary>
        private List<string> doNotDraftList = new List<string>();
        #endregion Variables

        #region Constructor
        public Stats(ErrorWriter ewriter) 
        {
            errorwriter = ewriter;

            int index = 0;
            int tempindex = 0;
            int tableEnd = 0;
            
            // Initialize AlternateNames
            altSearcher = new AlternateNames(errorwriter);

            // Read Do Not Draft list
            try
            {
                StreamReader reader = new StreamReader(".//DoNotDraft.txt");
                doNotDraftList.Add((reader.ReadLine()).ToUpper());
            }
            catch (FileNotFoundException e)
            {
                errorwriter.Write("Could not find DoNotDraft.txt ( " + e.Message + ")");
            }
            catch (Exception e)
            {
                errorwriter.Write("Error making the do not draft list ( " + e.Message + ")");
            }
            

            // Read source for player and team lists
            WebFetch fetch = new WebFetch(FPLSTATS, errorwriter);
            string source = fetch.pageContent;

            // Locate start of team table
            index = source.IndexOf("<table class='highlight'>");

            // Locate end of team table
            tableEnd = source.IndexOf("</table>", index);

            // Get info for all teams
            index = source.IndexOf("http://www.teamliquid.net/tlpd/sc2-korean", index);
            do
            {
                try
                {
                    // Get team name
                    tempindex = source.IndexOf(">", index) + 1;
                    index = source.IndexOf("<", tempindex);
                    string name = source.Substring(tempindex, index - tempindex);

                    // Skip 15 columns
                    for (int i = 0; i < 15; i++)
                    {
                        index = source.IndexOf("<td", index) + 3;
                    }

                    // Read team cost
                    tempindex = source.IndexOf("<td", index) + 3;
                    tempindex = source.IndexOf(">", tempindex) + 1;
                    index = source.IndexOf("<", tempindex);
                    int cost = Convert.ToInt32(source.Substring(tempindex, index - tempindex));

                    teamList.Add(name.ToUpper(), new Team(name, cost, ARRAYSIZE));
                }
                catch (Exception e)
                {
                    errorwriter.Write("Error reading team list (" + e.Message + ")");
                }
                finally
                {
                    index = source.IndexOf("http://www.teamliquid.net/tlpd/sc2-korean", index);
                }
            } while (index < tableEnd && index != -1);

            // Output log entry
            errorwriter.Write("Info for " + teamList.Count + " teams read");

            // Locate start of player table
            index = source.IndexOf("<table class='highlight'>", tableEnd);

            // Locate end of player table
            tableEnd = source.IndexOf("</table>", index);

            // Get info for all players
            index = source.IndexOf("<img src='/tlpd/images/", index);
            do
            {
                try
                {
                    // Get race
                    index += "<img src='/tlpd/images/".Length;
                    string race = source.Substring(index, 1).ToUpper();

                    // Get name
                    index = source.IndexOf("http://www.teamliquid.net/tlpd/sc2-korean", index);
                    tempindex = source.IndexOf(">", index) + 1;
                    index = source.IndexOf("<", tempindex);
                    string name = source.Substring(tempindex, index - tempindex);

                    // Get team
                    index = source.IndexOf("http://www.teamliquid.net/tlpd/sc2-korean", index);
                    tempindex = source.IndexOf(">", index) + 1;
                    index = source.IndexOf("<", tempindex);
                    string team = FullTeamName(source.Substring(tempindex, index - tempindex));

                    // Skip 12 columns
                    for (int i = 0; i < 12; i++)
                    {
                        index = source.IndexOf("<td", index) + 3;
                    }

                    // Read player cost
                    tempindex = source.IndexOf("<td", index) + 4;
                    tempindex = source.IndexOf(">", tempindex) + 1;
                    index = source.IndexOf("<", tempindex);
                    int cost = Convert.ToInt32(source.Substring(tempindex, index - tempindex));

                    playerList.Add(name.ToUpper(), new Player(name, cost, race, team, ARRAYSIZE));
                }
                catch (Exception e)
                {
                    errorwriter.Write("Error reading player list (" + e.Message + ")");
                }
                finally
                {
                    index = source.IndexOf("<img src='/tlpd/images/", index);
                }
            } while (index < tableEnd && index != -1);

            // Output log entry
            errorwriter.Write("Info for " + playerList.Count + " players read");
        }
        #endregion Constructor

        #region DataParser
        /// <summary>
        /// Parses liquipedia match info for win/loss data
        /// </summary>
        /// <param name="input">Liquipedia markup</param>
        /// <param name="maxWeeks">Maximum weeks to consider</param>
        public void ParseMarkup(string input, int maxWeeks)
        {
            int index = 0;
            int tempindex = 0;
            int week = 0;
            int match = 0;

            if (maxWeeks <= 0 || maxWeeks > ARRAYSIZE)
            {
                maxWeeks = ARRAYSIZE;
            }

            // Loop through markup looking for match data
            index = input.IndexOf("HiddenSort|Round", index);
            while (index != -1)
            {
                try
                {
                    // Get the week number
                    index = input.IndexOf("W", index) + 1;
                    week = Convert.ToInt32((input.Substring(index, 1)));
                    if (week > maxWeeks)
                    {
                        index = input.IndexOf("HiddenSort|Round", index);
                        continue;
                    }

                    // Get the match number
                    index = input.IndexOf("- Match ", index) + "- Match ".Length;
                    match = Convert.ToInt32((input.Substring(index, 1)));

                    string boxData;

                    // If "TeamMatch" isn't found within 20 characters, 
                    // there is a good chance that the infobox is stored on a different page
                    // We need to retrieve that box
                    if (input.IndexOf("TeamMatch", index) - index > 20 || input.IndexOf("TeamMatch", index) < 0)
                    {
                        index = input.IndexOf("{{:", index);
                        tempindex = input.IndexOf("\n", index);
                        string url = input.Substring(index, tempindex - index);
                        url = url.Replace("{{:", string.Empty);
                        url = url.Replace("}}", string.Empty);
                        WebFetch boxFetch = new WebFetch(WIKIBASE + url + "&action=edit", errorwriter);
                        boxFetch.ReduceToWikiMarkup();
                        boxData = boxFetch.pageContent;
                    }
                    else
                    {
                        index = input.IndexOf("TeamMatch", index);
                        boxData = input.Substring(index, input.IndexOf("}}", index) - index);
                    }

                    boxParser(boxData, week);
                    
                    index = input.IndexOf("HiddenSort|Round", index);
                }
                catch (Exception e)
                {
                    errorwriter.Write("Error during markup parsing (" + e.Message + ")");
                }
            }

            errorwriter.Write("Parse complete");
        }

        private void boxParser(string input, int week)
        {
            int index = 0;
            int tempindex = 0;
            string team1;
            string team2;

            // Get the first team
            index = input.IndexOf("team1=", index);
            if (index == -1)
            {
                return;
            }
            else
            {
                index += "team1=".Length;
            }

            tempindex = input.IndexOf("\n", index);
            team1 = FullTeamName(input.Substring(index, tempindex - index));

            // Get the second team
            index = input.IndexOf("team2=", index);
            if (index == -1)
            {
                return;
            }
            else
            {
                index += "team2=".Length;
            }

            tempindex = input.IndexOf("\n", index);
            team2 = FullTeamName(input.Substring(index, tempindex - index));

            // Get the winning team
            index = input.IndexOf("teamwin=", index) + "teamwin=".Length;
            string teamwin = input.Substring(index, 1);

            if (!(teamwin == "1" || teamwin == "2"))
            {
                errorwriter.Write("Week " + week + " (" + team1 + "/" + team2 + ") not completed");
                return;
            }

            // Give team win points to players
            if (teamwin == "1")
            {
                teamwin = team1;
            }
            else
            {
                teamwin = team2;
            }

            for (int i = 0; i < playerList.Count; i++)
            {
                if (playerList.ElementAt(i).Value.Team == teamwin)
                {
                    playerList.ElementAt(i).Value.TeamWinPoints(week);
                }
            }

            int team1wins = 0;
            int team2wins = 0;
            bool teamWinsAllocated = false;

            // Go through all the games
            for (int game = 1; game <= 7; game++)
            {
                string gameString;
                if (game < 7)
                {
                    gameString = "m" + game.ToString();
                }
                else
                {
                    gameString = "ace";
                }

                // Get player 1
                index = input.IndexOf(gameString + "p1=", index) + gameString.Length + 3;
                tempindex = input.IndexOf("|", index);
                string player1 = input.Substring(index, tempindex - index).ToUpper().Trim();
                
                // Since there are two HerOs
                if (player1 == "HERO" && team1 == "CJ Entus")
                {
                    player1 = "HERO[JOIN]";
                }

                if (!playerList.ContainsKey(player1))
                {
                    // Search for alternates, continue loop if none is found
                    errorwriter.Write("Can't find " + player1 + ", searching alts");
                    player1 = altSearcher.FindPlayerAlt(player1, playerList.Keys);
                    if (player1 == string.Empty)
                    {
                        continue;
                    }
                }

                // Get player 2
                index = input.IndexOf(gameString + "p2=", index) + gameString.Length + 3;
                tempindex = input.IndexOf("|", index);
                string player2 = input.Substring(index, tempindex - index).ToUpper().Trim();

                // Since there are two HerOs
                if (player2 == "HERO" && team2 == "CJ Entus")
                {
                    player2 = "HERO[JOIN]";
                }

                if (!playerList.ContainsKey(player2))
                {
                    // Search for alternates, continue loop if none is found
                    errorwriter.Write("Can't find " + player2 + ", searching alts");
                    player2 = altSearcher.FindPlayerAlt(player2, playerList.Keys);
                    if (player2 == string.Empty)
                    {
                        continue;
                    }
                }

                // Get winner
                index = input.IndexOf(gameString + "win=", index) + gameString.Length + 4;
                string winner = input.Substring(index, 1).ToUpper();
                if (!(winner == "1" || winner == "2" || winner == "S"))  // S is in case of "skip"
                {
                    errorwriter.Write("Week " + week + " game " + game + " (" + player1 + "/" + player2 + ") not completed");
                    continue;
                }

                // Give appearance points
                playerList[player1].AppearancePoints(week);
                playerList[player2].AppearancePoints(week);

                // Allocate wins and losses
                if (winner == "1")
                {
                    playerList[player1].WinPoints(week, game, playerList[player2].Streak);
                    playerList[player2].LosePoints(week, game);
                    team1wins++;
                }
                else if (winner == "2")
                {
                    playerList[player2].WinPoints(week, game, playerList[player1].Streak);
                    playerList[player1].LosePoints(week, game);
                    team2wins++;
                }

                // Allocate points to teams
                if ((team1wins == 4 || team2wins == 4) && !teamWinsAllocated)
                {
                    teamList[team1.ToUpper()].WinPoints(week, team1wins, team2wins);
                    teamList[team2.ToUpper()].WinPoints(week, team2wins, team1wins);
                    teamWinsAllocated = true;
                }

                errorwriter.Write("Added " + team1 + "@" + team2 + " (" + player1 + "/" + player2 + ")");
            }   
        }
        #endregion DataParser

        #region StatPrinting
        public void PrintTeamStats()
        {
            StreamWriter output = new StreamWriter(".//TeamStats.txt");

            // Write headings
            output.Write("Team");
            for (int i = 0; i < ARRAYSIZE; i++)
            {
                output.Write("\tW" + (i + 1).ToString());
            }

            output.Write("\tCost\tTotal\n");

            // Write team values
            for (int i = 0; i < teamList.Count; i++)
            {
                // Write name
                output.Write(teamList.ElementAt(i).Value.Name);

                // Write weekly point gains
                for (int j = 1; j <= ARRAYSIZE; j++)
                {
                    output.Write("\t" + teamList.ElementAt(i).Value.Points(j).ToString());
                }

                // Write cost and total points
                output.Write("\t" + teamList.ElementAt(i).Value.Cost + "\t" + teamList.ElementAt(i).Value.TotalPoints + "\n");
            }

            output.Close();
        }

        public void PrintPlayerStats()
        {
            StreamWriter output = new StreamWriter(".//PlayerStats.txt");

            // Write headings
            output.Write("Player");
            for (int i = 0; i < ARRAYSIZE; i++)
            {
                output.Write("\tW" + (i + 1).ToString());
            }

            output.Write("\tCost\tTotal\n");

            // Write player values
            for (int i = 0; i < playerList.Count; i++)
            {
                // Write name
                output.Write(playerList.ElementAt(i).Value.Name);

                // Write weekly point gains
                for (int j = 1; j <= ARRAYSIZE; j++)
                {
                    output.Write("\t" + playerList.ElementAt(i).Value.Points(j).ToString());
                }

                // Write cost and total points
                output.Write("\t" + playerList.ElementAt(i).Value.Cost + "\t" + playerList.ElementAt(i).Value.TotalPoints + "\n");
            }

            output.Close();
        }
        #endregion StatPrinting

        #region TeamNameStandardization
        private string FullTeamName(string nickname)
        {
            nickname = nickname.ToUpper();
            if (nickname == "CJ" || nickname == "CJENTUS")
            {
                return "CJ Entus";
            }
            else if (nickname == "WJS" || nickname == "WOONGJINSTARS")
            {
                return "Woongjin Stars";
            }
            else if (nickname == "STX" || nickname == "STXSOUL")
            {
                return "STX Soul";
            }
            else if (nickname == "KT" || nickname == "KTROLSTER")
            {
                return "KT Rolster";
            }
            else if (nickname == "EGL" || nickname == "EG-LIQUID")
            {
                return "EG.TL";
            }
            else if (nickname == "T8" || nickname == "TEAM8")
            {
                return "Team 8";
            }
            else if (nickname == "SAM" || nickname == "SAMSUNGKHAN")
            {
                return "Samsung Khan";
            }
            else if (nickname == "SKT" || nickname == "SKTELECOMT1")
            {
                return "SK Telecom T1";
            }
            else
            {
                return "Unknown";
            }
        }
        #endregion TeamNameStandardization

        #region BruteForceMainNoTrades
        /// <summary>
        /// Tries all possibilities for main teams, picks the best one
        /// </summary>
        /// <param name="limit">Number of players to consider</param>
        public void BruteForceMainNoTrades(int limit)
        {
            errorwriter.Write("Starting brute force solution with " + limit + " players");

            string selectedTeam = string.Empty;
            string[] selectedPlayers = new string[6];
            int totalPoints = 0;

            // Sort players by point total
            SortedList<string, int> playerPointComparison = new SortedList<string, int>();
            for (int i = 0; i < playerList.Count; i++)
            {
                playerPointComparison.Add(playerList.Keys.ElementAt(i), playerList.ElementAt(i).Value.TotalPoints);
            }

            var playerPointSorted = playerPointComparison.OrderByDescending(l => l.Value);

            if (limit > playerPointSorted.Count())
            {
                limit = playerPointSorted.Count();
            }

            string[] players = new string[6];

            // Loop for team
            for (int i0 = 0; i0 < teamList.Count; i0++)
            {
                string team = teamList.ElementAt(i0).Key;

                ProgressUpdate(i0 * 100 / teamList.Count());

                // Nest loops for all six players
                for (int i1 = 0; i1 < limit - 5; i1++)
                {
                    players[0] = playerPointSorted.ElementAt(i1).Key;
                    if (DoNotDraftFailed(players[0], true))
                    {
                        continue;
                    }

                    for (int i2 = i1+1; i2 < limit - 4; i2++)
                    {
                        players[1] = playerPointSorted.ElementAt(i2).Key;
                        if (DoNotDraftFailed(players[1], true))
                        {
                            continue;
                        }

                        for (int i3 = i2+1; i3 < limit - 3; i3++)
                        {
                            players[2] = playerPointSorted.ElementAt(i3).Key;
                            if (DoNotDraftFailed(players[2], true))
                            {
                                continue;
                            }

                            for (int i4 = i3 + 1; i4 < limit - 2; i4++)
                            {
                                players[3] = playerPointSorted.ElementAt(i4).Key;
                                if (DoNotDraftFailed(players[3], true))
                                {
                                    continue;
                                }

                                for (int i5 = i4+1; i5 < limit - 1; i5++)
                                {
                                    players[4] = playerPointSorted.ElementAt(i5).Key;
                                    if (DoNotDraftFailed(players[4], true))
                                    {
                                        continue;
                                    }

                                    for (int i6 = i5+1; i6 < limit; i6++)
                                    {
                                        players[5] = playerPointSorted.ElementAt(i6).Key;

                                        if (DoNotDraftFailed(players[5], true))
                                        {
                                            continue;
                                        }

                                        if (CheckTeamRequirements(players, team, true))
                                        {
                                            if (MainPointTotal(players, team) > totalPoints)
                                            {
                                                for (int x = 0; x < 6; x++)
                                                {
                                                    selectedPlayers[x] = players[x];
                                                    selectedTeam = team;
                                                    totalPoints = MainPointTotal(players, team);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Print best solution
            StreamWriter writer = new StreamWriter(".//MainTeamNoTrades (Brute Force).txt");
            writer.Write("Name\tCost\tPoints\n");

            for (int i = 0; i < 6; i++)
            {
                writer.WriteLine(playerList[selectedPlayers[i]].Name + "\t" + playerList[selectedPlayers[i]].Cost + "\t" + playerList[selectedPlayers[i]].TotalPoints);
            }

            writer.WriteLine(teamList[selectedTeam].Name + "\t" + teamList[selectedTeam].Cost + "\t" + teamList[selectedTeam].TotalPoints);

            writer.Close();
            errorwriter.Write("Brute force complete");
        }

        private bool CheckTeamRequirements(string[] players, string team, bool firstDraft)
        {
            // Race requirement
            bool tflag = false;
            bool pflag = false;
            bool zflag = false;
            for (int i = 0; i < players.Count(); i++)
            {
                switch (playerList[players[i]].Race.ToLower())
                {
                    case "t": 
                        tflag = true; 
                        break;
                    case "p": 
                        pflag = true; 
                        break;
                    case "z": 
                        zflag = true; 
                        break;
                }
            }

            if (!(tflag && pflag && zflag))
            {
                return false;
            }

            // Cost requirement
            if (MainCostRequirementCheck(players, team) > 0)
            {
                return false;
            }

            return true;
        }
        #endregion BruteForceMainNoTrades

        #region BestTeamGenerationNoTrades
        /// <summary>
        /// Determines the highest scoring team without trades through algorithm use
        /// </summary>
        public void BestMainWithoutTrades()
        {
            string selectedTeam;
            string[] selectedPlayers = new string[6];
            Dictionary<chosenTeam, int> solutions = new Dictionary<chosenTeam, int>();
            
            // Sort players by point total
            SortedList<string, int> playerPointComparison = new SortedList<string, int>();
            for (int i = 0; i < playerList.Count; i++)
            {
                playerPointComparison.Add(playerList.Keys.ElementAt(i), playerList.ElementAt(i).Value.TotalPoints);
            }

            var playerPointSorted = playerPointComparison.OrderByDescending(l => l.Value);

            // Iterate through the list with each team since there's only eight of them
            errorwriter.Write("Calculating best team with no trades");
            for (int i = 0; i < teamList.Count; i++)
            {
                selectedTeam = teamList.ElementAt(i).Key;
                errorwriter.Write("Using " + selectedTeam);

                // Take the highest scoring players
                for (int j = 0; j < 6; j++)
                {
                    selectedPlayers[j] = playerPointSorted.ElementAt(j).Key;
                }

                // The next player in playerPointSorted
                int index = 5;

                // The cost of the team needs to be reduced to 30 or less
                while (MainCostRequirementCheck(selectedPlayers, selectedTeam) > 0)
                {   
                    // Find the player with the lowest point to cost ratio
                    int lowestCostBenefitPlayer = 0;
                    for (int j = 1; j < 6; j++)
                    {
                        if (playerList[selectedPlayers[j]].PointToCostRatio < playerList[selectedPlayers[lowestCostBenefitPlayer]].PointToCostRatio)
                        {
                            lowestCostBenefitPlayer = j;
                        }
                    }

                    // Replace the lowest cost benefit player with the next player in the list, but only if their cost is lower
                    while (true)
                    {
                        index++;

                        if (index >= playerList.Count)
                        {
                            errorwriter.Write("Error during best team determination: index became greater than playerList.Count");
                            break;
                        }

                        string targetPlayer = playerPointSorted.ElementAt(index).Key;
                        if (playerList[targetPlayer].Cost < playerList[selectedPlayers[lowestCostBenefitPlayer]].Cost)
                        {
                            errorwriter.Write("Swapping " + selectedPlayers[lowestCostBenefitPlayer] + " with " + targetPlayer + " (Reducing cost)");
                            selectedPlayers[lowestCostBenefitPlayer] = targetPlayer;
                            break;
                        }
                    }
                }

                // Replace players with lower cost equivalents if available
                for (int j = 0; j < 6; j++)
                {
                    for (int k = 0; k < playerPointSorted.Count(); k++)
                    {
                        if (playerPointSorted.ElementAt(k).Value == playerList[selectedPlayers[j]].TotalPoints)
                        {
                            // Note that replacing a player with the same player is impossible since their costs will be the same
                            if (playerList[playerPointSorted.ElementAt(k).Key].Cost < playerList[selectedPlayers[j]].Cost)
                            {
                                bool duplicate = false;

                                // Avoid duplicates
                                for (int l = 0; l < 6; l++)
                                {
                                    if (playerPointSorted.ElementAt(k).Key == selectedPlayers[l])
                                    {
                                        duplicate = true;
                                    }
                                }

                                if (!duplicate)
                                {
                                    errorwriter.Write("Swapping " + selectedPlayers[j] + " with " + playerPointSorted.ElementAt(k).Key + " (Minimizing cost)");
                                    selectedPlayers[j] = playerPointSorted.ElementAt(k).Key;
                                }
                            }
                        }
                        else if (playerPointSorted.ElementAt(k).Value < playerList[selectedPlayers[j]].TotalPoints)
                        {
                            break;
                        }
                    }
                }

                // Calculate remaining cost we have to work with
                int leftover = -MainCostRequirementCheck(selectedPlayers, selectedTeam);
                
                // If the leftover isn't zero, we can potentially put in higher cost, higher point gain players 
                if (leftover != 0)
                {
                    // Find the point value of the lowest scoring player
                    int lowestPoints = playerList[selectedPlayers[0]].TotalPoints;
                    for (int j = 1; j < 6; j++)
                    {
                        if (playerList[selectedPlayers[j]].TotalPoints < lowestPoints)
                        {
                            lowestPoints = playerList[selectedPlayers[j]].TotalPoints;
                        }
                    }

                    for (int j = 0; j < playerPointSorted.Count(); j++)
                    {
                        // No use looking at players with the same points as the lowest selected player
                        if (playerPointSorted.ElementAt(j).Value == lowestPoints)
                        {
                            break;
                        }

                        bool used = false;

                        // Make sure the target player is not already selected
                        string targetPlayer = playerPointSorted.ElementAt(j).Key;
                        for (int k = 0; k < 6; k++)
                        {
                            if (targetPlayer == selectedPlayers[k])
                            {
                                used = true;
                            }
                        }

                        // Keep iterating if player is already used
                        if (used)
                        {
                            continue;
                        }
                        else
                        {
                            int difference = 0;
                            int pointGain = 0;
                            int playerTochange = -1;
                            Dictionary<string, int> possibleChanges = new Dictionary<string,int>();

                            // Check that the player can be switched in without violating limits
                            for (int k = 0; k < 6; k++)
                            {
                                if (playerList[targetPlayer].TotalPoints > playerList[selectedPlayers[k]].TotalPoints)
                                {
                                    difference = playerList[targetPlayer].Cost - playerList[selectedPlayers[k]].Cost;
                                    if (difference <= leftover)
                                    {
                                        if (pointGain < playerList[targetPlayer].TotalPoints - playerList[selectedPlayers[k]].TotalPoints)
                                        {
                                            playerTochange = k;
                                            pointGain = playerList[targetPlayer].TotalPoints - playerList[selectedPlayers[k]].TotalPoints;
                                        }
                                    }
                                }
                            }

                            // Swap the players
                            if (playerTochange != -1)
                            {
                                errorwriter.Write("Swapping " + selectedPlayers[playerTochange] + " with " + targetPlayer + " (Increasing point gain)");
                                selectedPlayers[playerTochange] = targetPlayer;
                                leftover = -MainCostRequirementCheck(selectedPlayers, selectedTeam);
                                if (leftover == 0)
                                {
                                    break;
                                }
                            }
                        }
                    }       
                }

                // Store solution
                chosenTeam newTeam = new chosenTeam();
                newTeam.players = new string[6];
                for (int j = 0; j < 6; j++)
                {
                    newTeam.players[j] = selectedPlayers[j];
                }
                newTeam.team = selectedTeam;
                solutions.Add(newTeam, MainPointTotal(selectedPlayers, selectedTeam));

                int totalPoints = teamList[selectedTeam].TotalPoints;
                int totalCost = teamList[selectedTeam].Cost;
                for (int j = 0; j < 6; j++)
                {
                    totalCost += playerList[selectedPlayers[j]].Cost;
                    totalPoints += playerList[selectedPlayers[j]].TotalPoints;
                }

                errorwriter.Write("Total cost of " + totalCost +
                    ", and " + totalPoints + " points total.");
            }

            var orderedsolution = solutions.OrderByDescending(l => l.Value);

            // Print best solution
            StreamWriter writer = new StreamWriter(".//MainTeamNoTrades.txt");
            writer.Write("Name\tCost\tPoints\n");
            string[] players = new string[6];
            string team = orderedsolution.ElementAt(0).Key.team;

            for (int i = 0; i < 6; i++)
            {
                players[i] = orderedsolution.ElementAt(0).Key.players.ElementAt(i);
            }

            for (int i = 0; i< 6; i++)
            {
                writer.WriteLine(playerList[players[i]].Name + "\t" + playerList[players[i]].Cost + "\t" + playerList[players[i]].TotalPoints);
            }

            writer.WriteLine(teamList[team].Name + "\t" + teamList[team].Cost + "\t" + teamList[team].TotalPoints);
            writer.Write("\n\nNote that this function currently doesn't check to see if this team meets the race requirements");

            writer.Close();
        }
        #endregion BestTeamGenerationNoTrades

        #region RequirementChecks
        /// <summary>
        /// Determines the difference between the selected team and the cost limit
        /// </summary>
        /// <param name="teamPlayers">Selected team as an array of strings</param>
        /// <param name="team">Selected progaming team</param>
        /// <returns>Cost difference between the selected team and the point limit (30)</returns>
        private int MainCostRequirementCheck(string[] teamPlayers, string team)
        {
            int total = 0;
            for (int i = 0; i < teamPlayers.Count(); i++)
            {
                total += playerList[teamPlayers[i].ToUpper()].Cost;
            }

            total += teamList[team.ToUpper()].Cost;

            return total - 30;
        }

        /// <summary>
        /// Checks to ensure you cannot draft players on the do not draft list
        /// </summary>
        /// <param name="players">Player to check</param>
        /// <param name="firstDraft">True if this is the first draft</param>
        /// <returns>False if requirement is not met. True otherwise</returns>
        private bool DoNotDraftFailed(string player, bool firstDraft)
        {
            // Do not draft players on the do not draft list
            if (firstDraft)
            {
                for (int i = 0; i < doNotDraftList.Count; i++)
                {
                    if (player == doNotDraftList[i])
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checks to see that the anti point requirements are met
        /// </summary>
        /// <param name="anti">String containing antiteam player names</param>
        /// <returns>Difference between team cost and requried cost. -ve means insufficient cost</returns>
        private int AntiCostRequirementCheck(string[] anti)
        {
            int total = 0;
            for (int i = 0; i < anti.Count(); i++)
            {
                total += playerList[anti[i].ToUpper()].Cost;
            }

            return total - 13;
        }
        #endregion RequirementChecks

        /// <summary>
        /// Calculates the total points of the main team
        /// </summary>
        /// <param name="teamPlayers">Selected players as an array of strings</param>
        /// <param name="team">Selected progaming team</param>
        /// <returns>Point total</returns>
        private int MainPointTotal(string[] teamPlayers, string team)
        {
            int total = 0;
            for (int i = 0; i < teamPlayers.Count(); i++)
            {
                total += playerList[teamPlayers[i].ToUpper()].TotalPoints;
            }

            total += teamList[team.ToUpper()].TotalPoints;

            return total;
        }
    }
}
