using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectARM
{
    //Матричное описание модели манипулятора
    public class MatrixMathModel : MathModel
    {
        public ArrayList T;
        public BlockMatrix[] B;
        public BlockMatrix[] S;

        public MatrixMathModel() { }

        public MatrixMathModel(MathModel model) : base(model)
        {
            B = new BlockMatrix[n];
            S = new BlockMatrix[n];
            for (int i = 0; i < n; i++)
            {
                B[i] = new BlockMatrix();
                S[i] = new BlockMatrix();
            }
            S[0] = null;
            calcBSq();
        }

        public MatrixMathModel(int n) : base(n)
        {
            B = new BlockMatrix[n];
            S = new BlockMatrix[n];
            for (int i = 0; i < n; i++)
            {
                B[i] = new BlockMatrix();
                S[i] = new BlockMatrix();
            }
            S[0] = null;
            calcBSq();
        }

        public MatrixMathModel(int n, unit[] units) : base(n, units)
        {
            B = new BlockMatrix[n];
            S = new BlockMatrix[n];
            for (int i = 0; i < n; i++)
            {
                B[i] = new BlockMatrix();
                S[i] = new BlockMatrix();
            }
            S[0] = null;
            calcBSq();
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

        public double GetPointError(double[] q, DPoint p) => NormaVectora(new DPoint(p.x - F(n).x, p.y - F(n).y, p.z - F(n).z));
        public double NormaVectora(DPoint p) => Math.Sqrt(Math.Pow(p.x, 2) + Math.Pow(p.y, 2));

        // Составляем матрицы S и B для каждого звена по их типу
        private void calcBSq()
        {
            var i = 0;
            foreach (var unit in units)
            {
                switch (unit.type)
                {
                    case 'S':
                        B[i] = new BlockMatrix();
                        B[i].SetByIJ(0, 0, Math.Cos(unit.angle));
                        B[i].SetByIJ(0, 1, -Math.Sin(unit.angle));
                        B[i].SetByIJ(1, 0, Math.Sin(unit.angle));
                        B[i].SetByIJ(1, 1, Math.Cos(unit.angle));
                        B[i].SetByIJ(2, 3, unit.len);
                        break;
                    case 'R':
                        S[i] = new BlockMatrix();
                        S[i].SetByIJ(0, 0, Math.Cos(q[i - 1]));
                        S[i].SetByIJ(0, 1, -Math.Sin(q[i - 1]));
                        S[i].SetByIJ(1, 0, Math.Sin(q[i - 1]));
                        S[i].SetByIJ(1, 1, Math.Cos(q[i - 1]));

                        B[i] = new BlockMatrix();
                        B[i].SetByIJ(2, 3, unit.len);
                        break;
                    case 'P':
                        S[i] = new BlockMatrix();
                        S[i].SetByIJ(2, 3, q[i - 1]);

                        B[i] = new BlockMatrix();
                        break;
                }
                i++;
            }
        }

        public void calcT()
        {
            T = new ArrayList();
            var tmp = new BlockMatrix();

            T.Add(tmp = B[0]);

            for (int i = 1; i < n; i++)
                T.Add(tmp *= S[i] * B[i]);
        }

        public DPoint GetT(int i)
        {
            var T = this.T[i] as BlockMatrix;
            return T.GetLastColumn();
        }

        public override void LagrangeMethodToThePoint(DPoint p)
        {
            calcT();
            DPoint b = new DPoint(p.x - F(n).x, p.y - F(n).y, p.z - F(n).z);
            //DPoint μ = CramerMethod(A, b);
            
            //for (int i = 0; i < 4; i++)
                //q[i] += MagicFunc(μ, q, a[i], dFxpodqi[i], dFypodqi[i]);
        }

        public DPoint F(int i)
        {
            return GetT(i);
        }

        public override double GetPointError(DPoint p)
        {
            throw new NotImplementedException();
        }
    }
}
