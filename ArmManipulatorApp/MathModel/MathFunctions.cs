using System;
using System.Windows.Media.Media3D;

namespace ArmManipulatorArm.MathModel
{
    public static class MathFunctions
    {
        public static double DegreeToRadian(double angle) => Math.PI * angle / 180.0;

        public static double RadianToDegree(double angle) => angle * (180.0 / Math.PI);

        public static double NormaVector(Vector3D p) => Math.Sqrt(Math.Pow(p.X, 2) + Math.Pow(p.Y, 2) + Math.Pow(p.Z, 2));

        public static void Normalize(Vector3D p)
        {
            var norma = NormaVector(p);
            p.X = p.X / norma;
            p.Y = p.Y / norma;
            p.Z = p.Z / norma;
        }

        public static bool SegmentContains(double left, double right, double num) => (num >= left && num <= right);

        public static double Projection(double left, double right, double num)
        {
            if (left > right) throw new Exception("Invalid arguments: left side is bigger then right side.");

            var max = left;
            if (num > max)
                max = num;
            if (max < right)
                return max;
            else
                return right;
        }
    }
}
