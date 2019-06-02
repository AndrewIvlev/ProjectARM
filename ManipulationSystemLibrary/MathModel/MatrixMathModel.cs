using System;
using System.Collections;
using System.Windows.Media.Media3D;

namespace ManipulationSystemLibrary
{
    //Матричное описание модели манипулятора
    public class MatrixMathModel : MathModel
    {
        private Matrix D;
        public ArrayList T;
        private ArrayList dT;
        private BlockMatrix[] S;
        private BlockMatrix[] dS;

        public MatrixMathModel() { }

        public MatrixMathModel(MathModel model) : base(model)
        {
            D = new Matrix(3, n);
            S = new BlockMatrix[n];
            dS = new BlockMatrix[n];
            for (int i = 0; i < n; i++)
            {
                S[i] = new BlockMatrix();
                dS[i] = new BlockMatrix();
            }
        }

        public MatrixMathModel(int n) : base(n)
        {
            D = new Matrix(3, n);
            S = new BlockMatrix[n];
            dS = new BlockMatrix[n];
            for (int i = 0; i < n; i++)
            {
                S[i] = new BlockMatrix();
                dS[i] = new BlockMatrix();
            }
        }

        public MatrixMathModel(int n, Unit[] units) : base(n, units)
        {
            D = new Matrix(3, n);
            S = new BlockMatrix[n];
            dS = new BlockMatrix[n];
            for (int i = 0; i < n; i++)
            {
                S[i] = new BlockMatrix();
                dS[i] = new BlockMatrix();
            }
        }

        public override void SetQ(double[] newQ)
        {
            base.SetQ(newQ);
            CalculationMetaData();
        }

        public void CalculationMetaData()
        {
            CalcS();
            CalcT();
        }
        
        public Vector3D F(int i) => (T[i] as BlockMatrix).ColumnAsVector3D(3);

        public Vector3D GetZAxis(int i) => (T[i] as BlockMatrix).ColumnAsVector3D(2);

        public override void LagrangeMethodToThePoint(Point3D p)
        {
            var f = this.F(n);
            var d = new Point3D(
                p.X - f.X,
                p.Y - f.Y,
                p.Z - f.Z
            );

            CalcdS();
            CalcdT();
            CalcD();

            var C = CalcC();
            var detC = Det3D(C);

            var Cx = ConcatAsColumn(C, d, 0);
            var detCx = Det3D(Cx);
            var Cy = ConcatAsColumn(C, d, 1);
            var detCy = Det3D(Cy);
            var Cz = ConcatAsColumn(C, d, 2);
            var detCz = Det3D(Cz);

            var μ = new Point3D(
                detCx / detC,
                detCy / detC,
                detCz / detC
            );

            for (var i = 0; i < n; i++)
            {
                var dF = GetdF(i);
                q[i] += (μ.X * dF.X + μ.Y * dF.Y + μ.Z * dF.Z) / (2 * A[i, i]);
            }
        }

        public Point3D SolutionVerification(Matrix a, Point3D b, Point3D x)
        {
            throw new NotImplementedException();
        }

        public override double GetPointError(Point3D p) => NormaVectora(new Point3D(p.X - F(n).X, p.Y - F(n).Y, p.Z - F(n).Z));
                
        private void CalcS()
        {
            for (var i = 0; i < n; i++)
            {
                switch (units[i].type)
                {
                    case 'R':
                        S[i] = new BlockMatrix();
                        S[i][0, 0] = Math.Cos(q[i]);
                        S[i][0, 1] = - Math.Sin(q[i]);
                        S[i][1, 0] = Math.Sin(q[i]);
                        S[i][1, 1] = Math.Cos(q[i]);
                        break;
                    case 'P':
                        S[i] = new BlockMatrix();
                        S[i][2, 3] = q[i];
                        break;
                    default:
                        throw new Exception("Unexpected unit type");
                }
            }
        }

        private void CalcdS()
        {
            for (var i = 0; i < n; i++)
            {
                switch (units[i].type)
                {
                    case 'R':
                        dS[i] = new BlockMatrix();
                        dS[i][0, 0] = -Math.Sin(q[i]);
                        dS[i][0, 1] = -Math.Cos(q[i]);
                        dS[i][1, 0] = Math.Cos(q[i]);
                        dS[i][1, 1] = -Math.Sin(q[i]);
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

        // TODO: Сделать такой же массив умножая с правой стороны
        private void CalcT()
        {
            T = new ArrayList();
            var tmp = new BlockMatrix();

            T.Add(tmp = rootB * S[0]);
            for (var i = 1; i < n; i++)
                T.Add(tmp *= units[i - 1].B * S[i]);

            T.Add(tmp *= units[n - 1].B);
        }

        // TODO: refactor this method, use already calculated matrix 
        // multiplication in T чтобы заново не перемножать одни и те же матрицы
        private BlockMatrix CalcdF(int index)
        {
            var dF = new BlockMatrix();
            dF *= rootB;
            for (var i = 1; i < n; i++)
            {
                dF *= i == index ? dS[i] : S[i];
                dF *= units[i].B;
            }

            return dF;
        }

        private void CalcdT()
        {
            dT = new ArrayList();
            for (var i = 0; i < n; i++)
                dT.Add(CalcdF(i));
        }

        //That function return vector ( dFxqi, dFyqi, dFzqi )
        private Vector3D GetdF(int i) => (dT[i] as BlockMatrix).ColumnAsVector3D(3);

        /// <summary>
        /// D is Matrix of gradients Fx, Fy and Fz
        /// ( Fxq1 Fxq2 ... Fxqn )
        /// ( Fyq1 Fyq2 ... Fyqn )
        /// ( Fzq1 Fzq2 ... Fzqn )
        /// </summary>
        private void CalcD()
        {
            for (var i = 0; i < n; i++)
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

            for (var i = 0; i < n; i++)
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

        private static Matrix ConcatAsColumn(Matrix A, Point3D v, int j)
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
