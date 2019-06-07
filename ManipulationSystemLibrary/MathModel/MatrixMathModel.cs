using System;
using System.Collections;
using System.Windows.Media.Media3D;
using ManipulationSystemLibrary.Matrix;

namespace ManipulationSystemLibrary.MathModel
{
    public class MatrixMathModel : MathModel
    {
        private Matrix.Matrix D;
        public ArrayList T;
        private ArrayList dT;
        private BlockMatrix[] S;
        private BlockMatrix[] dS;

        public MatrixMathModel(int n) : base(n)
        {
            D = new Matrix.Matrix(3, n);
            S = new BlockMatrix[n];
            dS = new BlockMatrix[n];
            for (var i = 0; i < n; i++)
            {
                S[i] = new BlockMatrix();
                dS[i] = new BlockMatrix();
            }
        }

        public MatrixMathModel(MathModel model)
        {
            D = new Matrix.Matrix(3, model.N);
            S = new BlockMatrix[model.N];
            dS = new BlockMatrix[model.N];
            for (var i = 0; i < model.N; i++)
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

        public override void CalculationMetaData()
        {
            CalcSByUnitsType();
            CalcT();
        }
        
        public Vector3D F(int i) => ((BlockMatrix) T[i]).ColumnAsVector3D(3);

        public Vector3D GetZAxis(int i) => ((BlockMatrix) T[i]).ColumnAsVector3D(2);

        public override void LagrangeMethodToThePoint(Point3D p)
        {
            var f = F(N);
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

            for (var i = 0; i < N; i++)
            {
                var dF = GetdF(i);
                Units[i].Q += (μ.X * dF.X + μ.Y * dF.Y + μ.Z * dF.Z) / (2 * A[i, i]);
            }
        }

        public Point3D SolutionVerification(Matrix.Matrix a, Point3D b, Point3D x)
        {
            throw new NotImplementedException();
        }

        public override double GetPointError(Point3D p) => NormaVector(new Point3D(p.X - F(N).X, p.Y - F(N).Y, p.Z - F(N).Z));
                
        private void CalcSByUnitsType()
        {
            for (var i = 0; i < N; i++)
            {
                switch (Units[i].Type)
                {
                    case 'R':
                        S[i] = new BlockMatrix();
                        S[i][0, 0] =  Math.Cos(Units[i].Q);
                        S[i][0, 1] = -Math.Sin(Units[i].Q);
                        S[i][1, 0] =  Math.Sin(Units[i].Q);
                        S[i][1, 1] =  Math.Cos(Units[i].Q);
                        break;
                    case 'P':
                        S[i] = new BlockMatrix();
                        S[i][2, 3] = Units[i].Q;
                        break;
                    default:
                        throw new Exception("Unexpected unit type");
                }
            }
        }

        private void CalcdS()
        {
            for (var i = 0; i < N; i++)
            {
                switch (Units[i].Type)
                {
                    case 'R':
                        dS[i] = new BlockMatrix();
                        dS[i][0, 0] = -Math.Sin(Units[i].Q);
                        dS[i][0, 1] = -Math.Cos(Units[i].Q);
                        dS[i][1, 0] =  Math.Cos(Units[i].Q);
                        dS[i][1, 1] = -Math.Sin(Units[i].Q);
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

        // TODO: Сделать такой же массив, умножая с правой стороны
        private void CalcT()
        {
            T = new ArrayList();
            var tmp = new BlockMatrix();

            T.Add(tmp = RootB * S[0]);
            for (var i = 1; i < N; i++)
                T.Add(tmp *= Units[i - 1].B * S[i]);

            T.Add(tmp *= Units[N - 1].B);
        }

        // TODO: refactor this method, use already calculated matrix 
        // multiplication in T чтобы заново не перемножать одни и те же матрицы
        private BlockMatrix CalcdF(int index)
        {
            var dF = new BlockMatrix();
            dF *= RootB;
            for (var i = 1; i < N; i++)
            {
                dF *= i == index ? dS[i] : S[i];
                dF *= Units[i].B;
            }

            return dF;
        }

        private void CalcdT()
        {
            dT = new ArrayList();
            for (var i = 0; i < N; i++)
                dT.Add(CalcdF(i));
        }

        //That function return vector ( dFxqi, dFyqi, dFzqi )
        private Vector3D GetdF(int i) => ((BlockMatrix) dT[i]).ColumnAsVector3D(3);

        /// <summary>
        /// D is Matrix of gradients Fx, Fy and Fz
        /// ( Fxq1 Fxq2 ... Fxqn )
        /// ( Fyq1 Fyq2 ... Fyqn )
        /// ( Fzq1 Fzq2 ... Fzqn )
        /// </summary>
        private void CalcD()
        {
            for (var i = 0; i < N; i++)
            {
                var b = GetdF(i);
                D[0, i] = b.X;
                D[1, i] = b.Y;
                D[2, i] = b.Z;
            }
        }

        // Вычисляем матрицу коэффициентов
        private Matrix.Matrix CalcC()
        {
            var C = new Matrix.Matrix(3);

            for (var i = 0; i < N; i++)
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

        private static double Det3D(Matrix.Matrix m) =>
            m[0, 0] * (m[1, 1] * m[2, 2] - m[2, 1] * m[1, 2])
            - m[0, 1] * (m[1, 0] * m[2, 2] - m[1, 2] * m[2, 0])
            + m[0, 2] * (m[1, 0] * m[2, 2] - m[2, 0] * m[1, 1]);

        /// <summary>
        /// Replacing column in matrix with vector
        /// </summary>
        /// <param name="a">Matrix</param>
        /// <param name="v">Column</param>
        /// <param name="i">Index</param>
        /// <returns></returns>
        private static Matrix.Matrix ConcatAsColumn(Matrix.Matrix a, Point3D v, int i)
        {
            if (a.Rows != 3)
                throw new ArgumentOutOfRangeException();

            var res = new Matrix.Matrix(a)
            {
                [0, i] = v.X,
                [1, i] = v.Y,
                [2, i] = v.Z
            };
            
            return res;
        }
    }
}
