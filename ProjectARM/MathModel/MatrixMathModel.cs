using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectARM
{
    public class MatrixMathModel : MathModel
    {
        public double[] q;
        delegate double function(double[] q);

        public MatrixMathModel(int _N)
        {
            N = _N;
            type = new char[N];
            len = new double[N];
            angle = new double[N];
            a = new double[N];
            for (int i = 0; i < N; i++)
                a[i] = 1;
        }
        public MatrixMathModel(char[] _type, double[] _len, double[] _angle)
        {
            N = _type.Length;
            type = new char[N];
            len = new double[N];
            angle = new double[N];
            a = new double[N];

            for (int i = 0; i < N; i++)
            {
                a[i] = 1;
                type[i] = _type[i];
                len[i] = _len[i];
                angle[i] = _angle[i];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double[][,] getMatrixDescription()
        {
            // Матрица T, определяющая правила пересчёта координат из системы хвата в систему нулевого звена;
            // представляет собой произведение матриц B и S, для наглядности храним в виде массива матриц, где
            // последовательно представлены матрицы B0, S1, B1, S2, B2, S3, ..., Sn, Bn
            double[][,] T = new double[N][,];
            
            for(int i = 0; i < N; i++)
            {
                T[i] = new double[4, 4];
            }

            return T;

        }

        public override double MaxL(double[] UnitTypePmaxLen)
        {
            double MaxL = 0;
            for (int i = 0; i < N; i++)             //Вычисление максимально возможной длины
                MaxL += len[i];                     //манипулятора, которая равна сумме длин всех звеньев
            foreach (double d in UnitTypePmaxLen)   //плюс макисмальные длины звеньев типа Р
                MaxL += d;
            return MaxL;
        }

        public static double DegreeToRadian(double angle) => Math.PI * angle / 180.0;
        public static double RadianToDegree(double angle) => angle * (180.0 / Math.PI);
       

        public Dpoint CramerMethod(double[,] A, Dpoint b)
        {
            Dpoint X = new Dpoint(0, 0);
            double det = A[0, 0] * A[1, 1] - A[0, 1] * A[1, 0];
            if (det != 0)
            {
                double detx1 = b.x * A[1, 1] - A[0, 1] * b.y;
                X.x = detx1 / det;
                double detx2 = A[0, 0] * b.y - b.x * A[1, 0];
                X.y = detx2 / det;
            }
            else return new Dpoint(0, 0);
            return X;
        }
        public Dpoint SolutionVerification(double[,] A, Dpoint b, Dpoint X)
        {
            Dpoint error;
            error.x = b.x - A[0, 0] * X.x - A[0, 1] * X.y;
            error.y = b.y - A[1, 0] * X.x - A[1, 1] * X.y;
            return error;
        }
        //public double GetPointError(double[] q, Dpoint p) => NormaVectora(new Dpoint((p.x - Fx(q)), (p.y - Fy(q))));
        public double NormaVectora(Dpoint p) => Math.Sqrt(Math.Pow(p.x, 2) + Math.Pow(p.y, 2));


        public override double[] LagrangeMethodToThePoint(Dpoint p) {
            double[] q = new double[N];
            return q;
        }

        public override double GetPointError(Dpoint p)
        {
            throw new NotImplementedException();
        }
    }
}
