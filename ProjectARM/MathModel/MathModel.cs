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
        public BlockMatrix B;
    }
    public abstract class MathModel
    {
        public double[] q;
        protected Matrix A;
        public unit[] units;
        public int n;

        public MathModel() { }

        public MathModel(MathModel model)
        {
            n = model.n;
            units = new unit[n];
            q = new double[n - 1];
            A = new Matrix(n - 1, n - 1);
            for (int i = 0; i < n; i++)
                units[i] = model.units[i];
            for (int i = 0; i < n - 1; i++)
                q[i] = model.q[i];
        }

        public MathModel(int n)
        {
            this.n = n;
            units = new unit[n];
            q = new double[n - 1];
            A = new Matrix(n - 1, n - 1);
            for (int i = 0; i < n; i++)
                units[i] = new unit { type = '0', len = 0, angle = 0 };
            for (int i = 0; i < n - 1; i++)
                q[i] = 0;
        }

        public MathModel(int n, unit[] units)
        {
            this.n = n;
            this.units = new unit[n];
            q = new double[n - 1];
            A = new Matrix(n - 1, n - 1);
            for (int i = 0; i < n; i++)
                this.units[i] = units[i];
            for (int i = 0; i < n - 1; i++)
                q[i] = 0;
        }

        public  double MaxL(double[] UnitTypePmaxLen)
        {
            double MaxL = 0;

            for (int i = 0; i < n; i++) //Вычисление максимально возможной длины
                MaxL += units[i].len;               //манипулятора, которая равна сумме длин всех звеньев
            foreach (double d in UnitTypePmaxLen)   //плюс макисмальные длины звеньев типа Р
                MaxL += d;

            return MaxL;
        }

        public void AllAngleToRadianFromDegree()
        {
            for (int i = 0; i < n; i++)
                units[i].angle = - DegreeToRadian(units[i].angle);
        }
        public abstract void LagrangeMethodToThePoint(Vector3D p);

        public abstract double GetPointError(Vector3D p);

        public void DefaultA()
        {
            for (int i = 0; i < n - 1; i++)
                for (int j = 0; j < n - 1; j++)
                    A[i, j] = i == j ? 1 : 0;
        }

        public void SetA(double[] A)
        {
            for (int i = 0; i < n; i++)
            {
                if (A[i] == 0) throw new Exception("The coefficient must be non-zero.");
                this.A[i, i] = A[i];
            }
        }

        public static double DegreeToRadian(double angle) => Math.PI * angle / 180.0;
        public static double RadianToDegree(double angle) => angle * (180.0 / Math.PI);
    }
}
