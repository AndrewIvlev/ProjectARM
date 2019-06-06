using System.Collections.Generic;

namespace ManipulationSystemLibrary
{
    public class LagrangeInterpolate
    {
        private List<double> allX = new List<double>();
        private List<double> allY = new List<double>();

        public void Add(double x, double y)
        {
            allX.Add(x);
            allY.Add(y);
        }
        public int GetCount() => allX.Count;
        
        public double InterpolateX(double x)
        {
            double y = 0;
            for (var i = 0; i <= allX.Count - 1; i++)
            {
                double numerator = 1;
                double denominator = 1;
                for (var c = 0; c <= allX.Count - 1; c++)
                {
                    if (c != i)
                    {
                        numerator *= (x - allX[c]);
                        denominator *= (allX[i] - allX[c]);

                    }
                }
                y += allY[i] * (numerator / denominator);
            }
            return y;
        }

    }
}