using System;

namespace ProjectARM
{
    public abstract class MathModel
    {
        /// <summary>
        ///  S - Static
        ///  R - Revolute
        ///  P - Prismatic
        ///  G - Gripper
        /// </summary>
        public static char[] type;
        public static double[] len;
        public static double[] angle;
        public static double[] a;
        public static int N;

        public abstract double MaxL(double[] UnitTypePmaxLen);
        public abstract void LagrangeMethodToThePoint(Dpoint p);
        public abstract double GetPointError(Dpoint p);

        public static void SetA(double[] A)
        {
            for (int i = 0; i < N; i++)
            {
                if (A[i] == 0) throw new Exception("The coefficient must be non-zero.");
                a[i] = A[i];
            }
        }

        public static double DegreeToRadian(double angle) => Math.PI * angle / 180.0;
        public static double RadianToDegree(double angle) => angle * (180.0 / Math.PI);
    }
}
