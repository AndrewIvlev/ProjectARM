using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectARM
{
    public class DPoint
    {
        public double x;
        public double y;
        public double z;

        public DPoint(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public DPoint ConvertToDPoint(double[] vector)
        {
            DPoint point = null;
            if (vector.Length == 3)
                point = new DPoint(vector[0], vector[1], vector[2]);
            return point;
        }
    }
}
