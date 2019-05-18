using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace ManipulationSystemLibrary
{
    public static class LinearSystemSolver
    {
        public static Vector3D CramerMethod(double[,] A, Vector3D b)
        {
            Vector3D X = new Vector3D(0, 0, 0);
            double det = A[0, 0] * A[1, 1] - A[0, 1] * A[1, 0];
            if (det != 0)
            {
                double detx1 = b.X * A[1, 1] - A[0, 1] * b.Y;
                X.X = detx1 / det;
                double detx2 = A[0, 0] * b.Y - b.X * A[1, 0];
                X.Y = detx2 / det;
            }
            else return new Vector3D(0, 0, 0);
            return X;
        }
    }
}
