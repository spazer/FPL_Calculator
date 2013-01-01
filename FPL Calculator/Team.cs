using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPL_Calculator
{
    class Team
    {
        private int arraySize;

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

        public double[] AdjustedTradeValue
        {
            get
            {
                return adjustedTradeValue;
            }
        }
        #endregion Properties

        #region Constructor
        public Team(string inputName, int inputCost, int inputArraySize)
        {
            name = inputName;
            cost = inputCost;
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
        }
        #endregion Constructor

        #region PointAllocation
        public void WinPoints(int week, int wins, int losses)
        {
            switch (wins - losses)
            {
                case -4:
                    points[week] += -2;
                    break;
                case -3:
                    points[week] += -1;
                    break;
                case -2:
                    points[week] += 0;
                    break;
                case -1:
                    points[week] += 1;
                    break;
                case 1: 
                    points[week] += 2;
                    break;
                case 2:
                    points[week] += 4;
                    break;
                case 3:
                    points[week] += 6;
                    break;
                case 4:
                    points[week] += 8;
                    break;
            }
        }
        #endregion PointAllocation

        public int Points(int week)
        {
            return points[week - 1];
        }
    }
}
