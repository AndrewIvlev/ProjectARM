using System;
using System.Windows.Media.Media3D;
using ManipulationSystemLibrary.Matrix;
using Newtonsoft.Json;

namespace ManipulationSystemLibrary.MathModel
{
    public struct Unit
    {
        /// <summary>
        ///  R - Revolute joint
        ///  P - Prismatic joint
        /// </summary>
        public char Type;

        /// <summary>
        /// Generalized coordinates vector
        /// </summary>
        public double Q;

        /// <summary>
        /// Matrix orientation and position of unit
        /// </summary>
        public BlockMatrix B;
    }

    public class MathModel
    {
        public readonly int N;
        public BlockMatrix RootB;
        public Unit[] Units;

        [JsonIgnore]
        public Matrix.Matrix A { get; set; }

        public MathModel() { }

        public MathModel(int n)
        {
            N = n;
            RootB = new BlockMatrix();
            Units = new Unit[n];
            A = new Matrix.Matrix(n, n);
            for (var i = 0; i < n; i++)
                Units[i] = new Unit { Type = 'S'};
        }

        public MathModel(int n, BlockMatrix rootB, Unit[] units)
        {
            N = n;
            RootB = rootB;
            Units = new Unit[n];
            for (var i = 0; i < n; i++)
                Units[i] = units[i];

            A = new Matrix.Matrix(n, n);
        }

        public virtual void SetQ(double[] newQ)
        {
            for (var i = 0; i < N; i++)
                Units[i].Q = newQ[i];
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

        public virtual void CalculationMetaData(){}

        public virtual void LagrangeMethodToThePoint(Point3D p){}

        public virtual double GetPointError(Point3D p) => -1;

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
                if (Math.Abs(A[i]) < 0) throw new Exception("The coefficient must be non-zero.");
                this.A[i, i] = A[i];
            }
        }

        public static double DegreeToRadian(double angle) => Math.PI * angle / 180.0;
        public static double RadianToDegree(double angle) => angle * (180.0 / Math.PI);
        public double NormaVector(Point3D p) => Math.Sqrt(Math.Pow(p.X, 2) + Math.Pow(p.Y, 2) + Math.Pow(p.Z, 2));
    }
}
