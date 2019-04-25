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
        public ArrayList dT;
        public BlockMatrix[] B;
        public BlockMatrix[] S;
        public BlockMatrix[] dS;

        public MatrixMathModel() { }

        public MatrixMathModel(MathModel model) : base(model)
        {
            B = new BlockMatrix[n];
            S = new BlockMatrix[n];
            dS = new BlockMatrix[n];
            for (int i = 0; i < n; i++)
            {
                B[i] = new BlockMatrix();
                S[i] = new BlockMatrix();
                dS[i] = new BlockMatrix();
            }
            S[0] = null;
            dS[0] = null;
            calcBSq();
        }

        public MatrixMathModel(int n) : base(n)
        {
            B = new BlockMatrix[n];
            S = new BlockMatrix[n];
            dS = new BlockMatrix[n];
            for (int i = 0; i < n; i++)
            {
                B[i] = new BlockMatrix();
                S[i] = new BlockMatrix();
                dS[i] = new BlockMatrix();
            }
            S[0] = null;
            dS[0] = null;
            calcBSq();
        }

        public MatrixMathModel(int n, unit[] units) : base(n, units)
        {
            B = new BlockMatrix[n];
            S = new BlockMatrix[n];
            dS = new BlockMatrix[n];
            for (int i = 0; i < n; i++)
            {
                B[i] = new BlockMatrix();
                S[i] = new BlockMatrix();
                dS[i] = new BlockMatrix();
            }
            S[0] = null;
            dS[0] = null;
            calcBSq();
        }

        public override void LagrangeMethodToThePoint(DPoint p)
        {
            calcT();
            var F = this.F(n - 1);
            calcdS();
            calcdT();
            var D = calcD();

            DPoint d = new DPoint(p.x - F.x, p.y - F.y, p.z - F.z);
            //DPoint μ = CramerMethod(A, d);

            //for (int i = 0; i < 4; i++)
            //q[i] += MagicFunc(μ, q, a[i], dFxpodqi[i], dFypodqi[i]);
        }

        public DPoint SolutionVerification(double[,] A, DPoint b, DPoint X)
        {
            throw new NotImplementedException();
        }

        public override double GetPointError(DPoint p)
        {
            throw new NotImplementedException();
        }

        public double GetPointError(double[] q, DPoint p) => NormaVectora(new DPoint(p.x - F(n).x, p.y - F(n).y, p.z - F(n).z));

        public double NormaVectora(DPoint p) => Math.Sqrt(Math.Pow(p.x, 2) + Math.Pow(p.y, 2));

        private DPoint CramerMethod(double[,] A, DPoint b)
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
        
        // Составляем матрицы S и B для каждого звена по их типу
        private void calcBSq()
        {
            int i = 0;
            foreach (var unit in units)
            {
                //var i = Array.IndexOf(units, unit);
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

        private void calcT()
        {
            T = new ArrayList();
            var tmp = new BlockMatrix();

            T.Add(tmp = B[0]);

            for (int i = 1; i < n; i++)
                T.Add(tmp *= S[i] * B[i]);
        }

        private DPoint GetT(int i)
        {
            var T = this.T[i] as BlockMatrix;
            return T.GetLastColumn();
        }
        // Maybe concat this two(up/down) methods?
        private DPoint F(int i)
        {
            return GetT(i);
        }

        private DPoint getb(int i)
        {
            var dT = this.dT[i] as BlockMatrix;
            return dT.GetLastColumn();
        }

        private BlockMatrix getdF(int i)
        {
            var dF = new BlockMatrix();
            dF = B[0];

            for (int k = 1; k < n; k++)
                dF *= k == i ? dS[i] * B[i] : S[i] * B[i];

            return dF;
        }

        private void calcdT()
        {
            dT = new ArrayList();
            for (int i = 1; i < n; i++)
                dT.Add(getdF(i));
        }

        private void calcdS()
        {
            var i = 0;
            foreach( var unit in units)
            {
                //var i = Array.IndexOf(units, unit);
                switch (unit.type)
                {
                    case 'R':
                        dS[i] = new BlockMatrix();
                        dS[i].SetByIJ(0, 0, -Math.Sin(q[i - 1]));
                        dS[i].SetByIJ(0, 1, -Math.Cos(q[i - 1]));
                        dS[i].SetByIJ(1, 0, Math.Cos(q[i - 1]));
                        dS[i].SetByIJ(1, 1, -Math.Sin(q[i - 1]));
                        dS[i].SetByIJ(2, 2, 0);
                        break;
                    case 'P':
                        dS[i] = new BlockMatrix();
                        dS[i].SetByIJ(0, 0, 0);
                        dS[i].SetByIJ(1, 1, 0);
                        dS[i].SetByIJ(2, 2, 0);
                        dS[i].SetByIJ(2, 3, 1);
                        break;
                }
                i++;
            }
        }

        private double[,] calcD()
        {
            var D = new double[3, n];

            for (int i = 0; i < n; i++)
            {
                var b = getb(i);
                D[0, i] = b.x;
                D[1, i] = b.y;
                D[2, i] = b.z;
            }

            return D;
        }
    }
}
