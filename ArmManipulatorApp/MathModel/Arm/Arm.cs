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
        
        [JsonIgnore] public Matrix A { get; set; }
        
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

            A = new Matrix(N, N);
            D = new Matrix(3, N);
            C = new Matrix(3, 3);
            S = new BlockMatrix[N];
            dS = new BlockMatrix[N];
            for (var i = 0; i < N; i++)
            {
                S[i] = new BlockMatrix();
                dS[i] = new BlockMatrix();
            }
        }

        #region Seters & Geters
        
        public void SetA(double[] A)
        {
            for (var i = 0; i < N; i++)
            {
                if (Math.Abs(A[i]) < 0)
                    throw new Exception("The coefficient must be non-zero.");
                this.A[i, i] = A[i];
            }
        }

        public void DefaultA()
        {
            for (var i = 0; i < N; i++)
            {
                for (var j = 0; j < N; j++)
                {
                    this.A[i, j] = i == j ? 1 : 0;
                }
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

        public Vector3D F(int i) => ((BlockMatrix)this.T[i]).ColumnAsVector3D(3);

        public Vector3D Fn() => ((BlockMatrix)this.T[this.N]).ColumnAsVector3D(3);

        //That function return vector ( dFxqi, dFyqi, dFzqi )
        public Vector3D GetdF(int i) => ((BlockMatrix) dT[i]).ColumnAsVector3D(3);

        public Vector3D GetZAxis(int i) => ((BlockMatrix) T[i]).ColumnAsVector3D(2);

        /// <summary>
        /// Вычисление максимально возможной длины манипулятора,
        /// которая равна сумме длин всех звеньев плюс макисмальные длины звеньев поступательного типа
        /// </summary>
        /// <returns></returns>
        public double MaxLength() => this.Units.Sum(unit => unit.GetLength());

        //public void AllAngleToRadianFromDegree()
        //{
        //    for (int i = 0; i < N; i++)
        //        units[i].angle = - DegreeToRadian(units[i].angle);
        //}

        #endregion

        public void CalcMetaDataForStanding()
        {
            ArmMetaDataCalculator.Init(this);
            ArmMetaDataCalculator.CalcSByUnitsType();
            ArmMetaDataCalculator.CalcT();
        }

        public double[] Move(Point3D p)
        {
            ArmMetaDataCalculator.Init(this);
            ArmMetaDataCalculator.CalcSByUnitsType();
            ArmMetaDataCalculator.CalcT();
            
            ArmMetaDataCalculator.CalcdS();
            ArmMetaDataCalculator.CalcdT();
            ArmMetaDataCalculator.CalcD();
            ArmMetaDataCalculator.CalcC();

            ArmMovementPlaning.Init(this);
            var dQ = ArmMovementPlaning.LagrangeMethodToThePoint(p);
            this.OffsetQ(dQ);
            return dQ;
        }
        

        public double GetPointError(Point3D p) =>
            MathFunctions.NormaVector(new Point3D(
                p.X - this.F(this.N).X,
                p.Y - this.F(this.N).Y,
                p.Z - this.F(this.N).Z));

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

    }
}
