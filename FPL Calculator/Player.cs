using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPL_Calculator
{
    class Player
    {
        private int arraySize;

        private string name;
        private int[] points;
        private double[] adjustedTradeValue;
        private int cost;
        private int streak;
        private string race;
        private string team;

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

        public double[] AdjustedTradeValue
        {
            get
            {
                return adjustedTradeValue;
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

        public int Points(int week)
        {
            return points[week - 1];
        }

        #region Constructor
        public Player(string inputName, int inputCost, string inputRace, string inputTeam, int inputArraySize)
        {
            name = inputName;
            cost = inputCost;
            race = inputRace;
            team = inputTeam;
            arraySize = inputArraySize;

            points = new int[inputArraySize];
            for (int i = 0; i < inputArraySize; i++)
            {
                points[i] = 0;
            }

            adjustedTradeValue = new double[inputArraySize];
            for (int i = 0; i < inputArraySize; i++)
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
        }

        public void TeamWinPoints(int week)
        {
            points[week - 1]++;
        }

        public void WinPoints(int week, int gameNumber, int opponentStreak)
        {
            // Ace match
            if (gameNumber == 7)
            {
                points[week - 1] += 4;
            }
            else
            {
                points[week - 1] += 2;
            }

            // Streak points
            streak++;
            if (streak == 3)
            {
                points[week - 1] += 1;
            }
            else if (streak == 6)
            {
                points[week - 1] += 2;
            }
            else if (streak == 9)
            {
                points[week - 1] += 3;
            }

            // Streak break points
            if (opponentStreak >= 3)
            {
                points[week - 1] += 1;
            }
        }

        public void LosePoints(int week, int gameNumber)
        {
            // Ace match
            if (gameNumber == 7)
            {
                points[week - 1] -= 2;
            }
            else
            {
                points[week - 1] -= 1;
            }

            // Reset streak
            streak = 0;
        }
        #endregion PointAllocation
    }
}
