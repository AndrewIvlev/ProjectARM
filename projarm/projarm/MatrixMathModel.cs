using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projarm
{
    class MatrixMathModel
    {
        public char type; //S - Static, R - Revolute, P - Prismatic, G - Gripper
        public static double[] len;
        public static double[] angle;
        private static double[] a;
    }
}
