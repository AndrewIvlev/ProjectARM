using System;
using System.Windows.Media.Media3D;
using ManipulationSystemLibrary.Matrix;

namespace ManipulationSystemLibrary.MathModel
{
    public struct Unit
    {
        /// <summary>
        ///  R - Revolute joint
        ///  P - Prismatic joint
        /// </summary>
        public char Type;

        public BlockMatrix B;
    }

    public abstract class MathModel
    {
        public double[] Q;
        protected Matrix.Matrix A;
        public Unit[] Units;
        public BlockMatrix RootB;
        public int N;

        public MathModel() { }

        public MathModel(MathModel model)
        {
            N = model.N;
            RootB = model.RootB;
            Units = new Unit[N];
            Q = new double[N];
            A = new Matrix.Matrix(N, N);
            for (var i = 0; i < N; i++)
            {
                Units[i] = model.Units[i];
                Q[i] = model.Q[i];
            }
        }

        public MathModel(int n)
        {
            this.N = n;
            RootB = new BlockMatrix();
            Units = new Unit[n];
            Q = new double[n];
            A = new Matrix.Matrix(n, n);
            for (var i = 0; i < n; i++)
            {
                Units[i] = new Unit { Type = 'S'};
                Q[i] = 0;
            }
        }

        public MathModel(int n, Unit[] units)
        {
            this.N = n;
            RootB = new BlockMatrix();
            this.Units = new Unit[n];
            Q = new double[n];
            A = new Matrix.Matrix(n, n);
            for (var i = 0; i < n; i++)
            {
                this.Units[i] = units[i];
                Q[i] = 0;
            }
        }

        public virtual void SetQ(double[] newQ)
        {
            for (var i = 0; i < N; i++)
                Q[i] = newQ[i];
        }
        
        public double GetUnitLen(int unit) => unit == 0 ? 
            RootB.ColumnAsVector3D(3).Length :
            Units[unit].B.ColumnAsVector3D(3).Length;

        //public  double MaxL(double[] UnitTypePmaxLen)
        //{
        //    double MaxL = 0;

        //    for (int i = 0; i < n; i++) //Вычисление максимально возможной длины
        //        MaxL += units[i].len;               //манипулятора, которая равна сумме длин всех звеньев
        //    foreach (double d in UnitTypePmaxLen)   //плюс макисмальные длины звеньев типа Р
        //        MaxL += d;

        //    return MaxL;
        //}

        //public void AllAngleToRadianFromDegree()
        //{
        //    for (int i = 0; i < n; i++)
        //        units[i].angle = - DegreeToRadian(units[i].angle);
        //}

        public abstract void LagrangeMethodToThePoint(Point3D p);

        public abstract double GetPointError(Point3D p);

        public void DefaultA()
        {
            for (var i = 0; i < N; i++)
                for (var j = 0; j < N; j++)
                    A[i, j] = i == j ? 1 : 0;
        }

        public void SetA(double[] A)
        {
            for (var i = 0; i < N; i++)
            {
                if (A[i] == 0) throw new Exception("The coefficient must be non-zero.");
                this.A[i, i] = A[i];
            }
        }

        public static double DegreeToRadian(double angle) => Math.PI * angle / 180.0;
        public static double RadianToDegree(double angle) => angle * (180.0 / Math.PI);
        public double NormaVectora(Point3D p) => Math.Sqrt(Math.Pow(p.X, 2) + Math.Pow(p.Y, 2) + Math.Pow(p.Z, 2));
    }
}
