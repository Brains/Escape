using System;

namespace Fluid.NumericalMethods
{
    public delegate double Function(double x, double y);

    public delegate double FunctionOne(double x);

    //------------------------------------------------------------------
    public class Simpson
    {
        private double[,] result;

        public Simpson (FunctionOne f, double a, double b, int pointsNum)
        {
            double I = 0;
            int j = 0;
            double h = 0.015f;

            result = new double[2, pointsNum + 1];

            for (int i = 0; i < pointsNum; i++)
            {
                double fi = a + h;

                // Search loop
                do
                {
                    if (j % 2 == 0)
                        I = I + 2 * f (fi);
                    else
                        I = I + 4 * f (fi);
                    
                    fi = fi + h;
                    j = j + 1;
                }
                while (fi < b);

                // Calculations
                I = I + f (a) + f (b);
                I = I * (h / 3);

                // Save results
                result[0, i] = I;
                result[1, i] = h;

                I = 0;
                h += 0.001f;
            }
        }

        //------------------------------------------------------------------
        public double[,] GetSolution()
        {
            return result;
        }
    }
}