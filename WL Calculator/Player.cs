using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WL_Calculator
{
    class Player
    {
        private int arraySize;
        private int gamesPlayed;
        private int totalGames;
        private string name;
        private int[] points;
        private double[] adjustedTradeValue;
        private int cost;
        private int streak;
        private string race;
        private string team;
        private double pointMod;
        private bool[] alreadyAppeared;

        #region Properties
        public double PointToCostRatio
        {
            get 
            {
                return (double)TotalPoints / (double)cost;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public int Cost
        {
            get
            {
                return cost;
            }
        }

        public string Team
        {
            get
            {
                return team;
            }
        }

        public string Race
        {
            get
            {
                return race;
            }
        }

        public int TotalPoints
        {
            get
            {
                int total = 0;

                for (int i=0; i<arraySize; i++)
                {
                    total += points[i];
                }

                return total;
            }
        }

        public int Streak
        {
            get 
            { 
                return streak; 
            }
        }
        #endregion Properties

        #region Constructor
        public Player(string inputName, int inputCost, string inputRace, string inputTeam, int inputArraySize, int inputTotalGames, double inputPointMod)
        {
            name = inputName;
            cost = inputCost;
            race = inputRace;
            team = inputTeam;
            arraySize = inputArraySize;
            totalGames = inputTotalGames;
            pointMod = inputPointMod;

            gamesPlayed = 0;

            points = new int[inputArraySize];
            alreadyAppeared = new bool[inputArraySize];
            for (int i = 0; i < inputArraySize; i++)
            {
                points[i] = 0;
                alreadyAppeared[i] = false;
            }

            adjustedTradeValue = new double[inputArraySize];
            adjustedTradeValue[0] = inputCost;
            for (int i = 1; i < inputArraySize; i++)
            {
                adjustedTradeValue[i] = 0;
            }

            streak = 0;
        }
        #endregion Constructor

        #region PointAllocation
        public void AppearancePoints(int week)
        {
            points[week - 1]++;
            alreadyAppeared[week - 1] = true;
            UpdateTradeValue(week - 1);
        }

        public void TeamWinPoints(int week)
        {
            points[week - 1]++;
            gamesPlayed++;
            UpdateTradeValue(week - 1);
        }

        public void TeamLose(int week)
        {
            gamesPlayed++;
            UpdateTradeValue(week - 1);
        }

        public void WinPoints(int week, int gameNumber, int opponentStreak)
        {
            points[week - 1] += 2;

            // Streak break points
            if (opponentStreak == 2)
            {
                points[week - 1] += 1;
            }
            else if (opponentStreak == 3)
            {
                points[week - 1] += 2;
            }

            UpdateTradeValue(week - 1);
        }

        public void LosePoints(int week, int gameNumber)
        {
            points[week - 1] -= 1;

            UpdateTradeValue(week - 1);
        }

        private void UpdateTradeValue(int week)
        {
            adjustedTradeValue[week] = (double)this.Cost * (double)(totalGames - gamesPlayed) / (double)totalGames + 
                                       (double)TotalPoints / pointMod;
        }
        #endregion PointAllocation

        public int Points(int week)
        {
            return points[week];
        }

        public double AdjustedTradeValue(int week)
        {
            return adjustedTradeValue[week];
        }

        public int FuturePoints(int start, int end)
        {
            int result = 0;
            for (int i = start; i <= end; i++)
            {
                result += points[i];
            }
            return result;
        }
    }
}
