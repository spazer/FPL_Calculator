using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPL_Calculator
{
    class Team
    {
        private int arraySize;
        private int gamesPlayed;
        private int totalGames;
        private string name;
        private int[] points;
        private double[] adjustedTradeValue;
        private int cost;

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

        public int TotalPoints
        {
            get
            {
                int total = 0;

                for (int i = 0; i < arraySize; i++)
                {
                    total += points[i];
                }

                return total;
            }
        }
        #endregion Properties

        #region Constructor
        public Team(string inputName, int inputCost, int inputArraySize, int inputTotalGames)
        {
            name = inputName;
            cost = inputCost;
            arraySize = inputArraySize;
            totalGames = inputTotalGames;

            gamesPlayed = 0;

            points = new int[inputArraySize];
            for (int i = 0; i < inputArraySize; i++)
            {
                points[i] = 0;
            }

            adjustedTradeValue = new double[inputArraySize];
            adjustedTradeValue[0] = inputCost;
            for (int i = 1; i < inputArraySize; i++)
            {
                adjustedTradeValue[i] = 0;
            }
        }
        #endregion Constructor

        #region PointAllocation
        public void WinPoints(int week, int wins, int losses)
        {
            switch (wins - losses)
            {
                case -4:
                    points[week - 1] += -2;
                    break;
                case -3:
                    points[week - 1] += -1;
                    break;
                case -2:
                    points[week - 1] += 0;
                    break;
                case -1:
                    points[week - 1] += 1;
                    break;
                case 1: 
                    points[week - 1] += 2;
                    break;
                case 2:
                    points[week - 1] += 4;
                    break;
                case 3:
                    points[week - 1] += 6;
                    break;
                case 4:
                    points[week - 1] += 8;
                    break;
            }

            gamesPlayed++;
            adjustedTradeValue[week] = (double)this.Cost * (double)(totalGames - gamesPlayed) / (double)totalGames +
                                       (double)points[week - 1] * 1 / 7;
        }
        #endregion PointAllocation

        public int Points(int week)
        {
            return points[week];
        }

        public double AdjustedTradeValue(int week)
        {
            return adjustedTradeValue[week + 1];
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
