using System;
using System.Windows.Media.Media3D;

namespace ArmManipulatorArm.MathModel
{
    public static class MathFunctions
    {
        public static double DegreeToRadian(double angle) => Math.PI * angle / 180.0;

        public static double RadianToDegree(double angle) => angle * (180.0 / Math.PI);

        public static double NormaVector(Vector3D p) => Math.Sqrt(Math.Pow(p.X, 2) + Math.Pow(p.Y, 2) + Math.Pow(p.Z, 2));
    }
}
