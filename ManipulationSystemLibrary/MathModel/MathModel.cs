using System;
using System.Windows.Media.Media3D;

namespace ManipulationSystemLibrary
{
    public struct Unit
    {
        /// <summary>
        ///  R - Revolute
        ///  P - Prismatic
        /// </summary>
        public char type;
        public BlockMatrix B;
    }

    public abstract class MathModel
    {
        public double[] q;
        protected Matrix A;
        public Unit[] units;
        public BlockMatrix rootB;
        public int n;

        public MathModel() { }

        public MathModel(MathModel model)
        {
            n = model.n;
            rootB = model.rootB;
            units = new Unit[n];
            q = new double[n];
            A = new Matrix(n, n);
            for (int i = 0; i < n; i++)
            {
                units[i] = model.units[i];
                q[i] = model.q[i];
            }
        }

        public MathModel(int n)
        {
            this.n = n;
            rootB = new BlockMatrix();
            units = new Unit[n];
            q = new double[n];
            A = new Matrix(n, n);
            for (int i = 0; i < n; i++)
            {
                units[i] = new Unit { type = 'S'};
                q[i] = 0;
            }
        }

        public MathModel(int n, Unit[] units)
        {
            this.n = n;
            rootB = new BlockMatrix();
            this.units = new Unit[n];
            q = new double[n];
            A = new Matrix(n, n);
            for (int i = 0; i < n; i++)
            {
                this.units[i] = units[i];
                q[i] = 0;
            }
        }

        public virtual void SetQ(double[] newQ)
        {
            for (int i = 0; i < n; i++)
                q[i] = newQ[i];
        }
        
        public double GetUnitLen(int unit) => unit == 0 ? 
            rootB.ColumnAsVector3D(3).Length :
            units[unit].B.ColumnAsVector3D(3).Length;

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
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
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
        public double NormaVectora(Point3D p) => Math.Sqrt(Math.Pow(p.X, 2) + Math.Pow(p.Y, 2) + Math.Pow(p.Z, 2));
    }
}
