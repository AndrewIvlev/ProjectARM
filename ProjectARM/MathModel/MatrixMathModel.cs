using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectARM
{
    public class MatrixMathModel : MathModel
    {
        delegate double function(double[] q);
        public BlockMatrix[] T;

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
        

        public override double MaxL(double[] UnitTypePmaxLen)
        {
            double MaxL = 0;
            for (int i = 0; i < N; i++)             //Вычисление максимально возможной длины
                MaxL += len[i];                     //манипулятора, которая равна сумме длин всех звеньев
            foreach (double d in UnitTypePmaxLen)   //плюс макисмальные длины звеньев типа Р
                MaxL += d;
            return MaxL;
        }

        public DPoint CramerMethod(double[,] A, DPoint b)
        {
            DPoint X = new DPoint(0, 0, 0);
            double det = A[0, 0] * A[1, 1] - A[0, 1] * A[1, 0];
            if (det != 0)
            {
                double detx1 = b.x * A[1, 1] - A[0, 1] * b.y;
                X.x = detx1 / det;
                double detx2 = A[0, 0] * b.y - b.x * A[1, 0];
                X.y = detx2 / det;
            }
            else return new DPoint(0, 0, 0);
            return X;
        }
        public DPoint SolutionVerification(double[,] A, DPoint b, DPoint X)
        {
            throw new NotImplementedException();
        }
        //public double GetPointError(double[] q, DPoint p) => NormaVectora(new DPoint((p.x - Fx(q)), (p.y - Fy(q))));
        public double NormaVectora(DPoint p) => Math.Sqrt(Math.Pow(p.x, 2) + Math.Pow(p.y, 2));


        public override double[] LagrangeMethodToThePoint(DPoint p) {
            
            DPoint b = new DPoint(p.x - F(q).x, p.y - F(q).y, p.z - F(q).z);
            DPoint μ = CramerMethod(A, b);
            
            for (int i = 0; i < 4; i++)
                q[i] += MagicFunc(μ, q, a[i], dFxpodqi[i], dFypodqi[i]);

            return q;
        }

        public DPoint F(double[] q)
        {
            return GetT(N) * new double[4]{ 0, 0, 0, 1};
        }
        
        public double[] GetT(int N)
        {
        }

        public void calcT(double q)
        {
            T = new BlockMatrix[N];
            BlockMatrix B = new BlockMatrix();
            BlockMatrix S = new BlockMatrix();

            for (int i = 0; i < N; i++)
            {
                T[i] = new BlockMatrix();
                switch (type[i])
                {
                    if (type[i] == 'R' || type[i] == 'C')
                    {
                             = new double[3, 4] {
                            { Math.Cos(q), -Math.Sin(q), 0, 0 },
                            { Math.Sin(q), Math.Cos(q), 0, 0 },
                            {0, 0, 1, 0 }
                        };
                    }
                    else
                    {
                        if (type[i] == 'P')
                        {
                                 = new double[3, 4] {
                                { 1, 0, 0, 0 },
                                { 0, 1, 0, 0 },
                                {0, 0, 1, q }
                            };
                        }
                    }
                }
            }
        }

        public override double GetPointError(DPoint p)
        {
            throw new NotImplementedException();
        }
    }
}
