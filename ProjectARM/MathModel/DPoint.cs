using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectARM
{
    public class DPoint
    {
        public double X;
        public double Y;
        public double Z;

        public DPoint(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public DPoint ConvertToDPoint(double[] vector) =>
            vector.Length == 3 ? new DPoint(vector[0], vector[1], vector[2]) : null;
    }
}
