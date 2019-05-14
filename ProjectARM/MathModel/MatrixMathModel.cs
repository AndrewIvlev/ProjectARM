using System;
using System.Collections;

namespace ProjectARM
{
    //Матричное описание модели манипулятора
    public class MatrixMathModel : MathModel
    {
        private Matrix D;
        private ArrayList T;
        private ArrayList dT;
        private BlockMatrix[] S;
        private BlockMatrix[] dS;

        public MatrixMathModel() { }

        public MatrixMathModel(MathModel model) : base(model)
        {
            D = new Matrix(3, n - 1);
            S = new BlockMatrix[n];
            dS = new BlockMatrix[n];
            for (int i = 0; i < n; i++)
            {
                S[i] = new BlockMatrix();
                dS[i] = new BlockMatrix();
            }
            S[0] = null;
            dS[0] = null;
        }

        public MatrixMathModel(int n) : base(n)
        {
            D = new Matrix(3, n - 1);
            S = new BlockMatrix[n];
            dS = new BlockMatrix[n];
            for (int i = 0; i < n; i++)
            {
                S[i] = new BlockMatrix();
                dS[i] = new BlockMatrix();
            }
            S[0] = null;
            dS[0] = null;
        }

        public MatrixMathModel(int n, unit[] units) : base(n, units)
        {
            D = new Matrix(3, n - 1);
            S = new BlockMatrix[n];
            dS = new BlockMatrix[n];
            for (int i = 0; i < n; i++)
            {
                S[i] = new BlockMatrix();
                dS[i] = new BlockMatrix();
            }
            S[0] = null;
            dS[0] = null;
        }

        public Vector3D F(int i) => (T[i] as BlockMatrix)?.GetLastColumn();
        
        public override void LagrangeMethodToThePoint(Vector3D p)
        {
            DefaultA();
            CalcS();
            CalcT();

            var F = this.F(n - 1);
            var d = new Vector3D(
                p.X - F.X,
                p.Y - F.Y,
                p.Z - F.Z
            );

            CalcdS();
            CalcdT();
            CalcD();
            var C = CalcC();
            var detC = Det3D(C);
            var Cx = ConcatAsColumn(C, d, 0);
            var Cy = ConcatAsColumn(C, d, 1);
            var Cz = ConcatAsColumn(C, d, 2);

            var μ = new Vector3D(
                Det3D(Cx) / detC,
                Det3D(Cy) / detC,
                Det3D(Cz) / detC
            );

            for (var i = 0; i < n - 1; i++)
            {
                var dF = GetdF(i);
                q[i] += (μ.X * dF.X + μ.Y * dF.Y + μ.Z * dF.Z) / (2 * A[i, i]);
            }
        }

        public Vector3D SolutionVerification(Matrix A, Vector3D b, Vector3D X)
        {
            throw new NotImplementedException();
        }

        public override double GetPointError(Vector3D p) => NormaVectora(new Vector3D(p.X - F(n).X, p.Y - F(n).Y, p.Z - F(n).Z));

        public double NormaVectora(Vector3D p) => Math.Sqrt(Math.Pow(p.X, 2) + Math.Pow(p.Y, 2));

        // Составляем матрицы S и B для каждого звена по их типу
        private void CalcS()
        {

            for (var i = 1; i < n; i++)
            {
                var unit = units[i];
                switch (unit.type)
                {
                    case 'R':
                        S[i] = new BlockMatrix();
                        S[i][0, 0] = Math.Cos(q[i - 1]);
                        S[i][0, 1] = - Math.Sin(q[i - 1]);
                        S[i][1, 0] = Math.Sin(q[i - 1]);
                        S[i][1, 1] = Math.Cos(q[i - 1]);
                        break;
                    case 'P':
                        S[i] = new BlockMatrix();
                        S[i][2, 3] = q[i - 1];
                        break;
                    default:
                        throw new Exception("Unexpected unit type");
                }
            }
        }

        private void CalcdS()
        {

            for (var i = 1; i < n; i++)
            {
                var unit = units[i];
                switch (unit.type)
                {
                    case 'R':
                        dS[i] = new BlockMatrix();
                        dS[i][0, 0] = -Math.Sin(q[i - 1]);
                        dS[i][0, 1] = -Math.Cos(q[i - 1]);
                        dS[i][1, 0] = Math.Cos(q[i - 1]);
                        dS[i][1, 1] = -Math.Sin(q[i - 1]);
                        dS[i][2, 2] = 0;
                        break;
                    case 'P':
                        dS[i] = new BlockMatrix();
                        dS[i][0, 0] = 0;
                        dS[i][1, 1] = 0;
                        dS[i][2, 2] = 0;
                        dS[i][2, 3] = 1;
                        break;
                    default:
                        throw new Exception("Unexpected unit type");
                }
            }
        }

        private void CalcT()
        {
            T = new ArrayList();
            var tmp = new BlockMatrix();

            T.Add(tmp = units[0].B);

            for (var i = 1; i < n; i++)
                T.Add(tmp *= S[i] * units[i].B);
        }

        private BlockMatrix CalcdF(int i)
        {
            var dF = units[0].B;

            for (var k = 1; k < n; k++)
                dF *= k == i ? dS[k] * units[k].B : S[k] * units[k].B;

            return dF;
        }

        private void CalcdT()
        {
            dT = new ArrayList();
            for (var i = 1; i < n; i++)
                dT.Add(CalcdF(i));
        }

        //That function return vector ( dFxqi, dFyqi, dFzqi )
        private Vector3D GetdF(int i) => (dT[i] as BlockMatrix)?.GetLastColumn();

        /// <summary>
        /// D is Matrix of gradients Fx, Fy and Fz
        /// ( Fxq1 Fxq2 ... Fxqn )
        /// ( Fyq1 Fyq2 ... Fyqn )
        /// ( Fzq1 Fzq2 ... Fzqn )
        /// </summary>
        private void CalcD()
        {
            for (var i = 0; i < n - 1; i++)
            {
                var b = GetdF(i);
                D[0, i] = b.X;
                D[1, i] = b.Y;
                D[2, i] = b.Z;
            }
        }

        // Вычисляем матрицу коэффициентов
        private Matrix CalcC()
        {
            var C = new Matrix(3);

            for (var i = 0; i < n - 1; i++)
            {
                C[0, 0] += D[0, i] * D[0, i];
                C[0, 1] += D[0, i] * D[1, i];
                C[0, 2] += D[0, i] * D[2, i];
                C[1, 1] += D[1, i] * D[1, i];
                C[1, 2] += D[1, i] * D[2, i];
                C[2, 2] += D[2, i] * D[2, i];
            }
            C[1, 0] = C[0, 1];
            C[2, 0] = C[0, 2];
            C[2, 1] = C[1, 2];

            return C;
        }

        private double Det3D(Matrix M) =>
            M[0, 0] * (M[1, 1] * M[2, 2] - M[2, 1] * M[1, 2])
            - M[0, 1] * (M[1, 0] * M[2, 2] - M[1, 2] * M[2, 0])
            + M[0, 2] * (M[1, 0] * M[2, 2] - M[2, 0] * M[1, 1]);

        private Matrix ConcatAsColumn(Matrix A, Vector3D v, int j)
        {
            if (A.rows != 3)
                throw new ArgumentOutOfRangeException();

            var res = new Matrix(A)
            {
                [0, j] = v.X,
                [1, j] = v.Y,
                [2, j] = v.Z
            };
            
            return res;
        }
    }
}
