using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectARM
{
    //Матричное описание модели манипулятора
    public class MatrixMathModel : MathModel
    {
        //public BlockMatrix[] T;

        public MatrixMathModel(int n) : base(n) { }
        public MatrixMathModel(int n, unit[] units) : base(n, units) { }

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


        public override void LagrangeMethodToThePoint(DPoint p)
        {
            DPoint b = new DPoint(p.x - F(q).x, p.y - F(q).y, p.z - F(q).z);
            //DPoint μ = CramerMethod(A, b);
            
            //for (int i = 0; i < 4; i++)
                //q[i] += MagicFunc(μ, q, a[i], dFxpodqi[i], dFypodqi[i]);
        }

        public DPoint F(double[] q)
        {
            return GetT(n) * new double[4] { 0, 0, 0, 1 };
        }

        public double[] GetT(int i)
        {
            return new double[i];
        }

        public void calcT(double q)
        {
            //T = new BlockMatrix[n];
            BlockMatrix B = new BlockMatrix();
            BlockMatrix S = new BlockMatrix();

            for (int i = 0; i < n; i++)
            {
                //[i] = new BlockMatrix();
                //switch (type[i])
                //{
                //    if (type[i] == 'R' || type[i] == 'C')
                //    {
                //             = new double[3, 4] {
                //            { Math.Cos(q), -Math.Sin(q), 0, 0 },
                //            { Math.Sin(q), Math.Cos(q), 0, 0 },
                //            {0, 0, 1, 0 }
                //        };
                //    }
                //    else
                //    {
                //        if (type[i] == 'P')
                //        {
                //                 = new double[3, 4] {
                //                { 1, 0, 0, 0 },
                //                { 0, 1, 0, 0 },
                //                {0, 0, 1, q }
                //            };
                //        }
                //    }
                //}
            }
        }

        public override double GetPointError(DPoint p)
        {
            throw new NotImplementedException();
        }
    }
}
