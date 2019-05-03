using System;
using System.Collections;

namespace ProjectARM
{
    //Матричное описание модели манипулятора
    public class MatrixMathModel : MathModel
    {
        private ArrayList T;
        private ArrayList dT;
        private BlockMatrix[] B;
        private BlockMatrix[] S;
        private BlockMatrix[] dS;

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
            CalcBSq();
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
            CalcBSq();
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
            CalcBSq();
        }

        public Vector3D F(int i) => (T[i] as BlockMatrix)?.GetLastColumn();

        //public override void LagrangeMethodToThePoint(Vector3D p)
        //{
        //    DefaultA();

        //    CalcT();
        //    var F = this.F(n - 1);

        //    CalcdS();
        //    CalcdT();
        //    var D = CalcD();

        //    var transpD = Matrix.Transpose(D);
        //    var inverseA = Matrix.Inverse(A);

        //    var d = new Matrix(3, 1)
        //    {
        //        [0, 0] = p.X - F.X,
        //        [1, 0] = p.Y - F.Y,
        //        [2, 0] = p.Z - F.Z
        //    };
        //    //There we need to solve linear equations system (D|d) and get dq - solution of this system
        //    var inverseAtranspD = inverseA * transpD;
        //    var dq = inverseAtranspD * Matrix.GeneralInverse(D * inverseAtranspD) * d; //TODO: Use generalized inverse in this case

        //    for (int i = 0; i < n - 1; i++)
        //        q[i] += dq[i, 0];
        //}

        public override void LagrangeMethodToThePoint(Vector3D p)
        {
            DefaultA();

            CalcT();
            var F = this.F(n - 1);

            CalcdS();
            CalcdT();
            var D = CalcD();

            //var transpD = Matrix.Transpose(D);

            var d = new Vector3D
            (
                p.X - F.X,
                p.Y - F.Y,
                p.Z - F.Z
            );

            var C = CalcC();
            var detC = Det3D(C);
            var Cx = new Matrix(C);
            var Cy = new Matrix(C);
            var Cz = new Matrix(C);

            var μ = new Vector3D(
                Det3D(Cx) / detC,
                Det3D(Cy) / detC,
                Det3D(Cz) / detC
            );

            for (int i = 0; i < n - 1; i++)
                q[i] += μFunction(μ, i);
        }

        public Vector3D SolutionVerification(Matrix A, Vector3D b, Vector3D X)
        {
            throw new NotImplementedException();
        }

        public override double GetPointError(Vector3D p)
        {
            throw new NotImplementedException();
        }

        public double GetPointError(double[] q, Vector3D p) => NormaVectora(new Vector3D(p.X - F(n).X, p.Y - F(n).Y, p.Z - F(n).Z));

        public double NormaVectora(Vector3D p) => Math.Sqrt(Math.Pow(p.X, 2) + Math.Pow(p.Y, 2));

        private double μFunction(Vector3D μ, int i) => (μ.X * GetB(i).X + μ.Y * GetB(i).Y + μ.Z * GetB(i).Z) / (2 * A[i, i]);

        // Составляем матрицы S и B для каждого звена по их типу
        private void CalcBSq()
        {
            if (units[0].type == 'S')
            {
                var unit = units[0];
                B[0] = new BlockMatrix();
                B[0][0, 0] = Math.Cos(unit.angle);
                B[0][0, 1] = -Math.Sin(unit.angle);
                B[0][1, 0] = Math.Sin(unit.angle);
                B[0][1, 1] = Math.Cos(unit.angle);
                B[0][2, 3] = unit.len;  
            }

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
                B[i] = new BlockMatrix();
                B[i][2, 3] = unit.len;
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

            T.Add(tmp = B[0]);

            for (var i = 1; i < n; i++)
                T.Add(tmp *= S[i] * B[i]);
        }

        private BlockMatrix GetdF(int i)
        {
            var dF = B[0];

            for (var k = 1; k < n; k++)
                dF *= k == i ? dS[k] * B[k] : S[k] * B[k];

            return dF;
        }

        private void CalcdT()
        {
            dT = new ArrayList();
            for (var i = 1; i < n; i++)
                dT.Add(GetdF(i));
        }

        //That function return vector ( dFxqi, dFyqi, dFzqi )
        private Vector3D GetB(int i) => (dT[i] as BlockMatrix)?.GetLastColumn();

        private Matrix CalcD()
        {
            var D = new Matrix(3, n - 1);

            for (var i = 0; i < n - 1; i++)
            {
                var b = GetB(i);
                D[0, i] = b.X;
                D[1, i] = b.Y;
                D[2, i] = b.Z;
            }

            return D;
        }

        private Matrix CalcC()
        {
            var C = new Matrix();

            return C;
        }

        private double Det3D(Matrix M)
        {
            double det = 0;

            return det;
        }
    }
}
