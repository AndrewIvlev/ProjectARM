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
        public Matrix RootB;
        public Unit[] Units;

        [JsonIgnore] public double[] A { get; set; }

        [JsonIgnore] public ArrayList T;
        [JsonIgnore] public Matrix D;
        [JsonIgnore] public Matrix C;
        [JsonIgnore] public ArrayList dT;
        [JsonIgnore] public Matrix[] S;
        [JsonIgnore] public Matrix[] dS;

        public Arm(Matrix rootB, Unit[] units)
        {
            this.N = units.Length;
            this.RootB = rootB;
            this.RootB.Rows = this.RootB.Columns = 4;
            this.Units = new Unit[this.N];
            for (var i = 0; i < this.N; i++)
            {
                this.Units[i] = units[i];
            }

            this.A = new double[this.N];
            this.D = new Matrix(3, this.N);
            this.C = new Matrix(3, 3);
            this.S = new Matrix[this.N];
            this.dS = new Matrix[this.N];
            for (var i = 0; i < this.N; i++)
            {
                this.A[i] = (double)1 / this.N;
                this.S[i] = new Matrix(4, 4);
                this.dS[i] = new Matrix(4, 4);
            }
        }

        public void SetA(double[] A)
        {
            for (var i = 0; i < this.N; i++)
            {
                if (Math.Abs(A[i]) < 0)
                {
                    throw new Exception("The coefficient must be non-zero.");
                }

                this.A[i] = A[i];
            }
        }

        public void SetQ(double[] newQ)
        {
            for (var i = 0; i < this.N; i++)
            {
                this.Units[i].Q = newQ[i];
            }
        }

        public void OffsetQ(double[] dQ)
        {
            for (var i = 0; i < this.N; i++)
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
            ? this.RootB.ColumnAsVector3D(3).Length
            : this.Units[unit].B.ColumnAsVector3D(3).Length;

        public double[] GetLenAsArray()
        {
            var lens = new double[this.N + 1];
            for (var i = 0; i < this.N + 1; i++)
            {
                lens[i] = i == 0 ? this.RootB.ColumnAsVector3D(3).Length : this.Units[i - 1].B.ColumnAsVector3D(3).Length;
            }

            return lens;
        }

        public void Build_S_ForAllUnits_ByUnitsType()
        {
            for (var i = 0; i < this.N; i++)
            {
                switch (this.Units[i].Type)
                {
                    case 'R':
                        this.S[i] = new Matrix(4, 4);
                        this.S[i].ToE();
                        this.S[i][0, 0] = Math.Cos(this.Units[i].Q);
                        this.S[i][0, 1] = -Math.Sin(this.Units[i].Q);
                        this.S[i][1, 0] = Math.Sin(this.Units[i].Q);
                        this.S[i][1, 1] = Math.Cos(this.Units[i].Q);
                        break;
                    case 'P':
                        this.S[i] = new Matrix(4, 4);
                        this.S[i].ToE();
                        this.S[i][2, 3] = this.Units[i].Q;
                        break;
                    default:
                        throw new Exception("Unexpected unit type");
                }
            }
        }

        // TODO: Сделать такой же массив, умножая с правой стороны
        public void Calc_T()
        {
            this.T = new ArrayList();

            Matrix tmp;
            this.T.Add(tmp = this.RootB * this.S[0]);
            for (var i = 1; i < this.N; i++)
            {
                this.T.Add(tmp *= this.Units[i - 1].B * this.S[i]);
            }

            this.T.Add(tmp * this.Units[this.N - 1].B);
        }

        public Vector3D F(int i) => ((Matrix)this.T[i]).ColumnAsVector3D(3);
        
        // That function return vector ( dFxqi, dFyqi, dFzqi )
        public Vector3D Get_dF(int i) => ((Matrix)this.dT[i]).ColumnAsVector3D(3);

        public Vector3D GetZAxis(int i) => ((Matrix)this.T[i]).ColumnAsVector3D(2);

        /// <summary>
        /// Вычисление максимально возможной длины манипулятора,
        /// которая равна сумме длин всех звеньев плюс макисмальные длины звеньев поступательного типа
        /// </summary>
        // TODO: Добавить учёт максимально возможной длины поступательных звеньев
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

        public double[] LagrangeMethodToThePoint(Point3D p, out double cond, bool withCond)
        {
            var dQ = new double[this.N];

            var f = this.F(this.N);
            var d = new Point3D(
                p.X - f.X,
                p.Y - f.Y,
                p.Z - f.Z);

            var C = this.C;
            var detC = Matrix.Det3D(C);

            cond = 0;
            if (withCond)
            {
                if (detC == 0)
                {
                    cond = double.MaxValue;
                }
                else
                {
                    cond = C.NormF() * C.Invert3D(detC).NormF();
                }
            }

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
                var dF = this.Get_dF(i);
                dQ[i] = ((μ.X * dF.X) + (μ.Y * dF.Y) + (μ.Z * dF.Z)) / (2 * this.A[i]);
            }

            return dQ;
        }

        #region Temp for RRPR arm

        public double[] RRPR_LagrangeMethodToThePoint(Point3D p, out double cond, bool withCond)
        {
            var dQ = new double[this.N];

            var f = this.F(this.N);
            var d = new Point3D(
                p.X - f.X,
                p.Y - f.Y,
                p.Z - f.Z);

            var C = new Matrix(3, 3);
            // C xx
            C[0, 0] = this.expected_dFxdq1 * this.expected_dFxdq1 + this.expected_dFxdq2 * this.expected_dFxdq2 + this.expected_dFxdq3 * this.expected_dFxdq3 + this.expected_dFxdq4 * this.expected_dFxdq4;
            // C xy
            C[0, 1] = this.expected_dFxdq1 * this.expected_dFydq1 + this.expected_dFxdq2 * this.expected_dFydq2 + this.expected_dFxdq3 * this.expected_dFydq3 + this.expected_dFxdq4 * this.expected_dFydq4;
            // C xz
            C[0, 2] = this.expected_dFxdq1 * this.expected_dFzdq1 + this.expected_dFxdq2 * this.expected_dFzdq2 + this.expected_dFxdq3 * this.expected_dFzdq3 + this.expected_dFxdq4 * this.expected_dFzdq4;
            // C yy
            C[1, 1] = this.expected_dFydq1 * this.expected_dFydq1 + this.expected_dFydq2 * this.expected_dFydq2 + this.expected_dFydq3 * this.expected_dFydq3 + this.expected_dFydq4 * this.expected_dFydq4;
            // C yz
            C[1, 2] = this.expected_dFydq1 * this.expected_dFzdq1 + this.expected_dFydq2 * this.expected_dFzdq2 + this.expected_dFydq3 * this.expected_dFzdq3 + this.expected_dFydq4 * this.expected_dFzdq4;
            // C zz
            C[2, 2] = this.expected_dFzdq1 * this.expected_dFzdq1 + this.expected_dFzdq2 * this.expected_dFzdq2 + this.expected_dFzdq3 * this.expected_dFzdq3 + this.expected_dFzdq4 * this.expected_dFzdq4;
            // C yx
            C[1, 0] = this.C[0, 1];
            // C zx
            C[2, 0] = this.C[0, 2];
            // C zy
            C[2, 1] = this.C[1, 2];

            var detC = Matrix.Det3D(C);

            cond = 0;
            if (withCond)
            {
                if (detC == 0)
                {
                    cond = double.MaxValue;
                }
                else
                {
                    cond = C.NormF() * C.Invert3D(detC).NormF();
                }
            }

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

            dQ[0] = (μ.X * expected_dFxdq1 + μ.Y * expected_dFydq1 + μ.Z * expected_dFzdq1) / (2 * this.A[0]);
            dQ[1] = (μ.X * expected_dFxdq2 + μ.Y * expected_dFydq2 + μ.Z * expected_dFzdq2) / (2 * this.A[1]);
            dQ[2] = (μ.X * expected_dFxdq3 + μ.Y * expected_dFydq3 + μ.Z * expected_dFzdq3) / (2 * this.A[2]);
            dQ[3] = (μ.X * expected_dFxdq4 + μ.Y * expected_dFydq4 + μ.Z * expected_dFzdq4) / (2 * this.A[3]);

            return dQ;
        }

        private double expected_dFxdq1 =>
            -Math.Sin(this.Units[0].Q)
            * ((Math.Cos(this.Units[1].Q) * Math.Cos(this.Units[3].Q)
                + Math.Sin(this.Units[1].Q) * Math.Sin(this.Units[3].Q)) * this.Units[3].B.ColumnAsVector3D(3).Length
               + Math.Cos(this.Units[1].Q) * (this.Units[2].B.ColumnAsVector3D(3).Length + this.Units[2].Q
                                                                                         + this.Units[1].B
                                                                                             .ColumnAsVector3D(3)
                                                                                             .Length));

        private double expected_dFxdq2 =>
            -Math.Sin(this.Units[1].Q)
            * (Math.Cos(this.Units[0].Q) * Math.Cos(this.Units[3].Q) * this.Units[3].B.ColumnAsVector3D(3).Length
               + Math.Cos(this.Units[0].Q) * (this.Units[2].B.ColumnAsVector3D(3).Length + this.Units[2].Q
                                                                                         + this.Units[1].B
                                                                                             .ColumnAsVector3D(3)
                                                                                             .Length))
            + this.Units[3].B.ColumnAsVector3D(3).Length * Math.Cos(this.Units[0].Q) * Math.Cos(this.Units[1].Q)
            * Math.Sin(this.Units[3].Q);

        private double expected_dFxdq3 => Math.Cos(this.Units[0].Q) * Math.Cos(this.Units[1].Q);

        private double expected_dFxdq4 =>
            -this.Units[3].B.ColumnAsVector3D(3).Length * Math.Cos(this.Units[0].Q) * Math.Cos(this.Units[1].Q)
            * Math.Sin(this.Units[3].Q) + this.Units[3].B.ColumnAsVector3D(3).Length * Math.Cos(this.Units[0].Q)
                                                                                     * Math.Sin(this.Units[1].Q)
                                                                                     * Math.Cos(this.Units[3].Q);

        private double expected_dFydq1 =>
            Math.Cos(this.Units[0].Q)
            * (this.Units[3].B.ColumnAsVector3D(3).Length
               * (Math.Cos(this.Units[1].Q) * Math.Cos(this.Units[3].Q)
                  + Math.Sin(this.Units[1].Q) * Math.Sin(this.Units[3].Q)) + Math.Cos(this.Units[1].Q)
               * (this.Units[1].B.ColumnAsVector3D(3).Length + this.Units[2].Q
                                                             + this.Units[2].B.ColumnAsVector3D(3).Length));

        private double expected_dFydq2 =>
            -Math.Sin(this.Units[1].Q)
            * (Math.Sin(this.Units[0].Q) * Math.Cos(this.Units[3].Q) * this.Units[3].B.ColumnAsVector3D(3).Length
               + Math.Sin(this.Units[0].Q) * (this.Units[1].B.ColumnAsVector3D(3).Length + this.Units[2].Q
                                                                                         + this.Units[2].B
                                                                                             .ColumnAsVector3D(3)
                                                                                             .Length))
            + this.Units[3].B.ColumnAsVector3D(3).Length * Math.Sin(this.Units[0].Q) * Math.Cos(this.Units[1].Q)
            * Math.Sin(this.Units[3].Q);

        private double expected_dFydq3 => Math.Sin(this.Units[0].Q) * Math.Cos(this.Units[1].Q);

        private double expected_dFydq4 =>
            -this.Units[3].B.ColumnAsVector3D(3).Length * Math.Sin(this.Units[0].Q) * Math.Cos(this.Units[1].Q)
            * Math.Sin(this.Units[3].Q) + this.Units[3].B.ColumnAsVector3D(3).Length * Math.Sin(this.Units[0].Q)
                                                                                     * Math.Sin(this.Units[1].Q)
                                                                                     * Math.Cos(this.Units[3].Q);

        private double expected_dFzdq1 => 0;

        private double expected_dFzdq2 =>
            Math.Cos(this.Units[1].Q) * (this.Units[3].B.ColumnAsVector3D(3).Length * Math.Cos(this.Units[3].Q)
                                         + this.Units[1].B.ColumnAsVector3D(3).Length
                                         + this.Units[2].B.ColumnAsVector3D(3).Length + this.Units[2].Q)
            + Math.Sin(this.Units[1].Q) * Math.Sin(this.Units[3].Q) * this.Units[3].B.ColumnAsVector3D(3).Length;

        private double expected_dFzdq3 => Math.Sin(this.Units[1].Q);

        private double expected_dFzdq4 =>
            -this.Units[3].B.ColumnAsVector3D(3).Length * (Math.Sin(this.Units[1].Q) * Math.Sin(this.Units[3].Q)
                                                           + Math.Cos(this.Units[1].Q) * Math.Cos(this.Units[3].Q));

        #endregion

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

        public void Build_dS()
        {
            for (var i = 0; i < this.N; i++)
            {
                switch (this.Units[i].Type)
                {
                    case 'R':
                        this.dS[i] = new Matrix(4, 4);
                        this.dS[i][0, 0] = -Math.Sin(this.Units[i].Q);
                        this.dS[i][0, 1] = -Math.Cos(this.Units[i].Q);
                        this.dS[i][1, 0] = Math.Cos(this.Units[i].Q);
                        this.dS[i][1, 1] = -Math.Sin(this.Units[i].Q);
                        break;
                    case 'P':
                        this.dS[i] = new Matrix(4, 4);
                        this.dS[i][2, 3] = 1;
                        break;
                    default:
                        throw new Exception("Unexpected unit type");
                }
            }
        }

        // TODO: refactor this method, use already calculated matrix multiplication in T
        // чтобы заново не перемножать одни и те же матрицы
        private Matrix Calc_dF(int index)
        {
            var dF = new Matrix(4, 4);
            dF.ToE();
            dF *= this.RootB;
            for (var i = 0; i < this.N; i++)
            {
                dF *= i == index ? this.dS[i] : this.S[i];
                dF *= this.Units[i].B;
            }

            return dF;
        }

        public void Calc_dT()
        {
            this.dT = new ArrayList();
            for (var i = 0; i < this.N; i++)
            {
                this.dT.Add(this.Calc_dF(i));
            }
        }

        /// <summary>
        /// D is Matrix of gradients Fx, Fy and Fz
        /// ( Fxq1 Fxq2 ... Fxqn )
        /// ( Fyq1 Fyq2 ... Fyqn )
        /// ( Fzq1 Fzq2 ... Fzqn )
        /// </summary>
        public void Build_D()
        {
            for (var i = 0; i < this.N; i++)
            {
                var b = this.Get_dF(i);
                this.D[0, i] = b.X;
                this.D[1, i] = b.Y;
                this.D[2, i] = b.Z;
            }
        }

        // Вычисляем матрицу коэффициентов для метода Лагранжа
        public void Calc_C()
        {
            for (var i = 0; i < this.N; i++)
            {
                // C xx
                this.C[0, 0] += this.D[0, i] * this.D[0, i];
                
                // C xy
                this.C[0, 1] += this.D[0, i] * this.D[1, i];

                // C xz
                this.C[0, 2] += this.D[0, i] * this.D[2, i];

                // C yy
                this.C[1, 1] += this.D[1, i] * this.D[1, i];

                // C yz
                this.C[1, 2] += this.D[1, i] * this.D[2, i];

                // C zz
                this.C[2, 2] += this.D[2, i] * this.D[2, i];
            }

            // C yx
            this.C[1, 0] = this.C[0, 1];
            
            // C zx
            this.C[2, 0] = this.C[0, 2];
            
            // C zy
            this.C[2, 1] = this.C[1, 2];
        }
    }
}
