namespace ArmManipulatorArm.MathModel.Arm
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Windows.Media.Media3D;

    using ArmManipulatorArm.MathModel.Matrix;

    using Newtonsoft.Json;

    public class Arm
    {
        public int N;
        public BlockMatrix RootB;
        public Unit[] Units;

        [JsonIgnore] public double[] A { get; set; }

        [JsonIgnore] public ArrayList T;
        [JsonIgnore] public Matrix D;
        [JsonIgnore] public Matrix C;
        [JsonIgnore] public ArrayList dT;
        [JsonIgnore] public BlockMatrix[] S;
        [JsonIgnore] public BlockMatrix[] dS;

        public Arm(BlockMatrix rootB, Unit[] units)
        {
            this.N = units.Length;
            RootB = rootB;
            Units = new Unit[N];
            for (var i = 0; i < N; i++)
                Units[i] = units[i];

            A = new double[N];
            D = new Matrix(3, N);
            C = new Matrix(3, 3);
            S = new BlockMatrix[N];
            dS = new BlockMatrix[N];
            for (var i = 0; i < N; i++)
            {
                A[i] = (double)1 / this.N;
                S[i] = new BlockMatrix();
                dS[i] = new BlockMatrix();
            }
        }

        public void SetA(double[] A)
        {
            for (var i = 0; i < N; i++)
            {
                if (Math.Abs(A[i]) < 0)
                    throw new Exception("The coefficient must be non-zero.");
                this.A[i] = A[i];
            }
        }

        public void SetQ(double[] newQ)
        {
            for (var i = 0; i < N; i++)
            {
                this.Units[i].Q = newQ[i];
            }
        }

        public void OffsetQ(double[] dQ)
        {
            for (var i = 0; i < N; i++)
            {
                this.Units[i].Q += dQ[i];
            }
        }

        public double[] GetQ()
        {
            var result = new double[this.N];
            for (var i = 0; i < this.N; i++)
            {
                result[i] = this.Units[i].Q;
            }

            return result;
        }

        public double GetUnitLen(int unit) => unit == 0
            ? RootB.ColumnAsVector3D(3).Length
            : Units[unit].B.ColumnAsVector3D(3).Length;

        public void CalcSByUnitsType()
        {
            for (var i = 0; i < this.N; i++)
            {
                switch (this.Units[i].Type)
                {
                    case 'R':
                        this.S[i] = new BlockMatrix();
                        this.S[i][0, 0] = Math.Cos(this.Units[i].Q);
                        this.S[i][0, 1] = -Math.Sin(this.Units[i].Q);
                        this.S[i][1, 0] = Math.Sin(this.Units[i].Q);
                        this.S[i][1, 1] = Math.Cos(this.Units[i].Q);
                        break;
                    case 'P':
                        this.S[i] = new BlockMatrix();
                        this.S[i][2, 3] = this.Units[i].Q;
                        break;
                    default:
                        throw new Exception("Unexpected unit type");
                }
            }
        }

        // TODO: Сделать такой же массив, умножая с правой стороны
        public void CalcT()
        {
            this.T = new ArrayList();

            BlockMatrix tmp;
            this.T.Add(tmp = this.RootB * this.S[0]);
            for (var i = 1; i < this.N; i++)
            {
                this.T.Add(tmp *= this.Units[i - 1].B * this.S[i]);
            }

            this.T.Add(tmp * this.Units[this.N - 1].B);
        }

        public Vector3D F(int i) => ((BlockMatrix)this.T[i]).ColumnAsVector3D(3);
        
        //That function return vector ( dFxqi, dFyqi, dFzqi )
        public Vector3D GetdF(int i) => ((BlockMatrix)dT[i]).ColumnAsVector3D(3);

        public Vector3D GetZAxis(int i) => ((BlockMatrix)T[i]).ColumnAsVector3D(2);

        /// <summary>
        /// Вычисление максимально возможной длины манипулятора,
        /// которая равна сумме длин всех звеньев плюс макисмальные длины звеньев поступательного типа
        /// </summary>
        public double MaxLength() => this.Units.Sum(unit => unit.GetLength());

        public void AllAngleToRadianFromDegree()
        {
            for (var i = 0; i < this.N; i++)
            {
                if (this.Units[i].Type == 'P')
                {
                    this.Units[i].Q = -MathFunctions.DegreeToRadian(this.Units[i].Q);
                }
            }
        }

        public double[] LagrangeMethodToThePoint(Point3D p)
        {
            var dQ = new double[this.N];

            var f = this.F(this.N);
            var d = new Point3D(
                p.X - f.X,
                p.Y - f.Y,
                p.Z - f.Z);

            var C = this.C;
            var detC = Matrix.Det3D(C);
            var Cx = Matrix.ConcatAsColumn(C, d, 0);
            var detCx = Matrix.Det3D(Cx);
            var Cy = Matrix.ConcatAsColumn(C, d, 1);
            var detCy = Matrix.Det3D(Cy);
            var Cz = Matrix.ConcatAsColumn(C, d, 2);
            var detCz = Matrix.Det3D(Cz);

            var μ = new Point3D(
                detCx / detC,
                detCy / detC,
                detCz / detC);

            for (var i = 0; i < this.N; i++)
            {
                var dF = this.GetdF(i);
                dQ[i] = ((μ.X * dF.X) + (μ.Y * dF.Y) + (μ.Z * dF.Z)) / (2 * this.A[i]);
            }

            return dQ;
        }


        public double GetPointError(Point3D p)
        {
            return MathFunctions.NormaVector(
                new Point3D(p.X - this.F(this.N).X, p.Y - this.F(this.N).Y, p.Z - this.F(this.N).Z));
        }

        public static bool operator ==(Arm a, Arm b)
        {
            if (a.N != b.N)
            {
                Console.WriteLine("Number of units is not equal.");
                return false;
            }

            if (a.RootB != b.RootB)
            {
                Console.WriteLine("Root matrices B are not equal.");
                return false;
            }

            for (var i = 0; i < a.N; i++)
            {
                if (a.Units[i] != b.Units[i])
                {
                    Console.WriteLine($"Units #{i} are not equal.");
                    return false;
                }
            }

            return true;
        }

        public static bool operator !=(Arm a, Arm b)
        {
            if (a.N != b.N)
                return true;
            if (a.RootB != b.RootB)
                return true;
            for (var i = 0; i < a.N; i++)
                if (a.Units[i] != b.Units[i])
                    return true;
            return false;
        }

        public void CalcdS()
        {
            for (var i = 0; i < this.N; i++)
            {
                switch (this.Units[i].Type)
                {
                    case 'R':
                        this.dS[i] = new BlockMatrix();
                        this.dS[i][0, 0] = -Math.Sin(this.Units[i].Q);
                        this.dS[i][0, 1] = -Math.Cos(this.Units[i].Q);
                        this.dS[i][1, 0] = Math.Cos(this.Units[i].Q);
                        this.dS[i][1, 1] = -Math.Sin(this.Units[i].Q);
                        this.dS[i][2, 2] = 0;
                        break;
                    case 'P':
                        this.dS[i] = new BlockMatrix();
                        this.dS[i][0, 0] = 0;
                        this.dS[i][1, 1] = 0;
                        this.dS[i][2, 2] = 0;
                        this.dS[i][2, 3] = 1;
                        break;
                    default:
                        throw new Exception("Unexpected unit type");
                }
            }
        }

        // TODO: refactor this method, use already calculated matrix multiplication in T
        // чтобы заново не перемножать одни и те же матрицы
        private BlockMatrix CalcdF(int index)
        {
            var dF = new BlockMatrix();
            dF *= this.RootB;
            for (var i = 0; i < this.N; i++)
            {
                dF *= i == index ? this.dS[i] : this.S[i];
                dF *= this.Units[i].B;
            }

            return dF;
        }

        public void CalcdT()
        {
            this.dT = new ArrayList();
            for (var i = 0; i < this.N; i++)
            {
                this.dT.Add(CalcdF(i));
            }
        }

        /// <summary>
        /// D is Matrix of gradients Fx, Fy and Fz
        /// ( Fxq1 Fxq2 ... Fxqn )
        /// ( Fyq1 Fyq2 ... Fyqn )
        /// ( Fzq1 Fzq2 ... Fzqn )
        /// </summary>
        public void CalcD()
        {
            for (var i = 0; i < this.N; i++)
            {
                var b = this.GetdF(i);
                this.D[0, i] = b.X;
                this.D[1, i] = b.Y;
                this.D[2, i] = b.Z;
            }
        }

        // Вычисляем матрицу коэффициентов
        public void CalcC()
        {
            for (var i = 0; i < this.N; i++)
            {
                this.C[0, 0] += this.D[0, i] * this.D[0, i];
                this.C[0, 1] += this.D[0, i] * this.D[1, i];
                this.C[0, 2] += this.D[0, i] * this.D[2, i];
                this.C[1, 1] += this.D[1, i] * this.D[1, i];
                this.C[1, 2] += this.D[1, i] * this.D[2, i];
                this.C[2, 2] += this.D[2, i] * this.D[2, i];
            }

            this.C[1, 0] = this.C[0, 1];
            this.C[2, 0] = this.C[0, 2];
            this.C[2, 1] = this.C[1, 2];
        }
    }
}
