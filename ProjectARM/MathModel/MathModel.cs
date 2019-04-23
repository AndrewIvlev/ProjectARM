using System;

namespace ProjectARM
{
    public struct unit
    {
        /// <summary>
        ///  S - Static
        ///  R - Revolute
        ///  C - Cylindrical
        ///  P - Prismatic
        ///  G - Gripper
        /// </summary>
        public char type;
        public double len;
        public double angle;
    }
    public abstract class MathModel
    {
        public double[] q;
        public double[] a;
        public unit[] units;
        public int n;

        public MathModel(int n)
        {
            this.n = n;
            units = new unit[n];
            q = new double[n - 1];
            a = new double[n - 1];
            for (int i = 0; i < n; i++)
                units[i] = new unit { type = '0', len = 0, angle = 0 };
            for (int i = 0; i < n - 1; i++)
            {
                q[i] = 0;
                a[i] = 1;
            }
        }

        public MathModel(int n, unit[] units)
        {
            this.n = n;
            this.units = new unit[n];
            q = new double[n - 1];
            a = new double[n - 1];
            for (int i = 0; i < n; i++)
                this.units[i] = units[i];
            for (int i = 0; i < n - 1; i++)
            {
                q[i] = 0;
                a[i] = 1;
            }
        }

        public  double MaxL(double[] UnitTypePmaxLen)
        {
            double MaxL = 0;
            for (int i = 0; i < n; i++) //Вычисление максимально возможной длины
                MaxL += units[i].len;                     //манипулятора, которая равна сумме длин всех звеньев
            foreach (double d in UnitTypePmaxLen)   //плюс макисмальные длины звеньев типа Р
                MaxL += d;
            return MaxL;
        }

        public abstract void LagrangeMethodToThePoint(DPoint p);

        public abstract double GetPointError(DPoint p);

        public void SetA(double[] A)
        {
            for (int i = 0; i < n; i++)
            {
                if (A[i] == 0) throw new Exception("The coefficient must be non-zero.");
                a[i] = A[i];
            }
        }

        public static double DegreeToRadian(double angle) => Math.PI * angle / 180.0;
        public static double RadianToDegree(double angle) => angle * (180.0 / Math.PI);
    }
}
