namespace ArmManipulatorApp.MathModel.Trajectory
{
    using System.Collections.Generic;
    using System.Windows.Media.Media3D;

    public class LagrangeInterpolate
    {
        public List<Point3D> DataPoints = new List<Point3D>();
        
        public int GetCount() => DataPoints.Count;
        
        public double Interpolate(double x, double y)
        {
            double z = 0;
            var n = this.DataPoints.Count;
            for (var c = 0; c < n; c++)
            {
                double numerator = 1;
                double denominator = 1;
                for (var i = 0; i < n; i++)
                {
                    if (i != c)
                    {
                        for (var j = 0; j < n; j++)
                        {
                            numerator *= (x - this.DataPoints[i].X) * (y - this.DataPoints[j].Y);
                            denominator *= (this.DataPoints[n].X - this.DataPoints[j].X) * (this.DataPoints[n].Y - this.DataPoints[j].Y);
                        }
                    }
                }

                z += this.DataPoints[c].Z * (numerator / denominator);
            }

            return z;
        }
    }
}
