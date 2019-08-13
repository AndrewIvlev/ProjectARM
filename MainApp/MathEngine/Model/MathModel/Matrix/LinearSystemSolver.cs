using System;
using System.Windows.Media.Media3D;

namespace ManipulationSystemLibrary
{
    public static class LinearSystemSolver
    {
        public static Point3D CramersRule(double[,] a, Point3D b)
        {
            var x = new Point3D(0, 0, 0);
            var det = a[0, 0] * a[1, 1] - a[0, 1] * a[1, 0];
            if (Math.Abs(det) > 0)
            {
                var detX1 = b.X * a[1, 1] - a[0, 1] * b.Y;
                x.X = detX1 / det;
                var detX2 = a[0, 0] * b.Y - b.X * a[1, 0];
                x.Y = detX2 / det;
            }
            else return new Point3D(0, 0, 0);
            return x;
        }
    }
}
