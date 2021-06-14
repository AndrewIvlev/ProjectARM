namespace ArmManipulatorArm.MathModel.Arm
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
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
        [JsonIgnore] public Matrix diagA { get; set; }

        [JsonIgnore] private readonly int maxV = 4; // замораживаем не более maxV обобщённых координат
        [JsonIgnore] public int RepeatedIterCount = 3; // число перепроверок выхода на ограничители на одной итерации
        [JsonIgnore] public int MaxLambdaRecalculationCount = 30; // лямбды пересчитываются не более 30 раз
        [JsonIgnore] private int curV;

        [JsonIgnore] public ArrayList T;
        [JsonIgnore] public Matrix D;
        [JsonIgnore] public Matrix rightResidueD; // слогаемые правой части матричного уравнения
        [JsonIgnore] public Matrix C;
        [JsonIgnore] public ArrayList dT;
        [JsonIgnore] public Matrix[] S;
        [JsonIgnore] public Matrix[] dS;

        [JsonIgnore] private double detC;

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
            this.diagA = new Matrix(this.N, this.N);
            this.D = new Matrix(3, this.N);
            this.rightResidueD = new Matrix(3, this.N);
            this.C = new Matrix(3, 3);
            this.S = new Matrix[this.N];
            this.dS = new Matrix[this.N];
            for (var i = 0; i < this.N; i++)
            {
                this.A[i] = 1.0 / this.N;
                this.diagA[i, i] = this.A[i];
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
                this.diagA[i, i] = this.A[i];
            }
        }

        public void SetQ(double[] newQ)
        {
            for (var i = 0; i < this.N; i++)
            {
                this.Units[i].Q = newQ[i];
            }
        }

        public void SetQ(double allQ)
        {
            for (var i = 0; i < this.N; i++)
            {
                this.Units[i].Q = allQ;
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

        /// <summary>
        /// Решение задачи по нахождению минимума вектора обобщённых координат
        /// </summary>
        /// <param name="p">Желаемое положение схвата</param>
        /// <param name="delta">Погрешность положения</param>
        /// <param name="cond">Число обусловленности. Если приходит 0 то считаем, если 1 - не считаем</param>
        public void LagrangeMethodToThePoint(Point3D p, out double delta, ref double cond, double condTreshold)
        {
            // Console.WriteLine($"Current q = " + JsonConvert.SerializeObject(this.GetQ()) + "\n");
            // Console.WriteLine("Planning trajectory to the point " + p);
            this.Build_S_ForAllUnits_ByUnitsType();
            this.Calc_T();
            var f = this.F(this.N);
            var D = new Vector3D(
                p.X - f.X,
                p.Y - f.Y,
                p.Z - f.Z);
            // Console.WriteLine("Desired grip offset " + D);
            //d = MathFunctions.NormaVector(D); // Желаемое смещение

            this.Build_dS();
            this.Calc_dT();
            this.Build_D();
            this.Calc_C();
            // Console.WriteLine("Matrix C:");
            //this.C.Print();
            this.detC = Matrix.Det3D(this.C);
            // Console.WriteLine("Determinant of matrix C is " + this.detC + "\n");


            if (cond == 0)
            {
                if (this.detC == 0)
                {
                    cond = double.MaxValue;
                }
                else
                {
                    cond = this.C.NormF() * this.C.Invert3D(this.detC).NormF();
                    // Console.WriteLine("Condition number of matrix C is " + cond + "\n");
                }
            }

            // Balancing by condition number
            if (condTreshold > 0)
            {
                var norm1 = this.C.EuclidNormOfRow(0);
                var norm2 = this.C.EuclidNormOfRow(1);
                var norm3 = this.C.EuclidNormOfRow(2);

                var diagNorm = new Matrix(3, 3)
                {
                    [0, 0] = 1.0 / norm1, [0, 1] = 0, [0, 2] = 0,
                    [1, 0] = 0, [1, 1] = 1.0 / norm2, [1, 2] = 0,
                    [2, 0] = 0, [2, 1] = 0, [2, 2] = 1.0 / norm3
                };

                this.C = diagNorm * this.C;
                this.detC = Matrix.Det3D(this.C);
                D = diagNorm * D;

                cond = this.C.NormF() * this.C.Invert3D(this.detC).NormF();
                Console.WriteLine("Condition number of matrix C is " + cond + "\n");
            }

            var μ = Matrix.System3x3Solver(this.C, this.detC, D);
            // Console.WriteLine($"mu = {μ}\n");

            var dQ = new double[this.N];
            for (var i = 0; i < this.N; i++)
            {
                var dF = this.Get_dF(i);
                this.Print_dF(i);
                dQ[i] = ((μ.X * dF.X) + (μ.Y * dF.Y) + (μ.Z * dF.Z)) / this.A[i];
            }

            // Console.WriteLine($"dq = {JsonConvert.SerializeObject(dQ)}\n");
            // Console.WriteLine($"Value of Q function = {this.functionQ()}");


            this.OffsetQ(dQ);
            this.Build_S_ForAllUnits_ByUnitsType();
            this.Calc_T();
            var newF = this.F(this.N);
            var newD = new Vector3D(
                newF.X - f.X,
                newF.Y - f.Y,
                newF.Z - f.Z);

            //b = MathFunctions.NormaVector(newD); // Реальное смещение

            var Delta = new Vector3D(
                newF.X - p.X,
                newF.Y - p.Y,
                newF.Z - p.Z);

            delta = MathFunctions.NormaVector(Delta);
        }

        // TODO: добавить out параметр repeatedIterCount
        public void LagrangeMethodWithProjectionToThePoint(Point3D p, ref List<double> delta, ref List<int> countOfLeftLimitAchievements, ref List<int> countOfRightLimitAchievements, ref double cond, double condTreshold, ref int resIterCount, out int repeatedIterCount)
        {
            this.Build_S_ForAllUnits_ByUnitsType();
            this.Calc_T();
            var f = this.F(this.N);
            var D = new Vector3D(
                p.X - f.X,
                p.Y - f.Y,
                p.Z - f.Z);
            // Console.WriteLine("Desired grip offset " + D);

            //d = MathFunctions.NormaVector(D); // Желаемое смещение

            this.Build_dS();
            this.Calc_dT();
            this.Build_D();
            this.Calc_C();
            this.detC = Matrix.Det3D(this.C);
            // Console.WriteLine("Determinant of matrix C is " + this.detC + "\n");


            if (cond == 0)
            {
                if (this.detC == 0)
                {
                    cond = double.MaxValue;
                }
                else
                {
                    cond = this.C.NormF() * this.C.Invert3D(this.detC).NormF();
                    // Console.WriteLine("Condition number of matrix C is " + cond + "\n");
                }
            }

            // Balancing by condition number
            if (condTreshold > 0)
            {
                var norm1 = this.C.EuclidNormOfRow(0);
                var norm2 = this.C.EuclidNormOfRow(1);
                var norm3 = this.C.EuclidNormOfRow(2);

                var diagNorm = new Matrix(3, 3)
                {
                    [0, 0] = 1.0 / norm1,
                    [0, 1] = 0,
                    [0, 2] = 0,
                    [1, 0] = 0,
                    [1, 1] = 1.0 / norm2,
                    [1, 2] = 0,
                    [2, 0] = 0,
                    [2, 1] = 0,
                    [2, 2] = 1.0 / norm3
                };

                this.C = diagNorm * this.C;
                this.detC = Matrix.Det3D(this.C);
                D = diagNorm * D;

                cond = this.C.NormF() * this.C.Invert3D(this.detC).NormF();
                Console.WriteLine("Condition number of matrix C is " + cond + "\n");
            }

            var μ = Matrix.System3x3Solver(this.C, this.detC, D);

            var dQ = new double[this.N];
            for (var i = 0; i < this.N; i++)
            {
                var dF = this.Get_dF(i);
                this.Print_dF(i);
                dQ[i] = ((μ.X * dF.X) + (μ.Y * dF.Y) + (μ.Z * dF.Z)) / this.A[i];
            }

            Vector3D Delta;
            // "замораживание" - отключение вычисления производных dF для qi у которых v = false.
            // вместо dF подставляются константные значения проекции qi

            this.curV = 0;
            var newdQ = this.GetProjectionOfQForLimitations(dQ, ref this.curV, out var currCountOfLeftLimitAchievements, out var currCountOfRightLimitAchievements);
            countOfLeftLimitAchievements.Add(currCountOfLeftLimitAchievements);
            countOfRightLimitAchievements.Add(currCountOfRightLimitAchievements);
            repeatedIterCount = 0;
            while (this.curV > 0)
            {
                this.OffsetQ(newdQ);
                this.Build_S_ForAllUnits_ByUnitsType();
                this.Calc_T();

                f = this.F(this.N);
                Delta = new Vector3D(
                    f.X - p.X,
                    f.Y - p.Y,
                    f.Z - p.Z);
                
                repeatedIterCount++;
                if (repeatedIterCount > RepeatedIterCount)
                {
                    Console.WriteLine($"RepeatedIterCount exceeded max value: {RepeatedIterCount}");
                    break;
                }

                this.Build_dS();
                this.Calc_dT();
                //this.Build_D(true);
                this.Build_D();
                this.Calc_C(true);
                this.detC = Matrix.Det3D(this.C);

                var _dFConstLims = dFConstLims(newdQ);
                D = new Vector3D(
                     p.X - f.X - _dFConstLims.X,
                     p.Y - f.Y - _dFConstLims.Y,
                     p.Z - f.Z - _dFConstLims.Z);
                //D = Matrix.SubtractToVector3D(D, this.rightResidueD);
                μ = Matrix.System3x3Solver(this.C, this.detC, D);

                var tmpV = 0;
                for (var i = 0; i < this.N; i++)
                {
                    if (!this.Units[i].v)
                    {
                        tmpV++;
                        continue;
                    }
                    var dF = this.Get_dF(i);
                    newdQ[i] = ((μ.X * dF.X) + (μ.Y * dF.Y) + (μ.Z * dF.Z)) / this.A[i];
                }
                dQ = newdQ;
                this.curV = 0;
                newdQ = this.GetProjectionOfQForLimitations(dQ, ref this.curV, out currCountOfLeftLimitAchievements, out currCountOfRightLimitAchievements);
            }

            this.OffsetQ(dQ);
            this.Build_S_ForAllUnits_ByUnitsType();
            this.Calc_T();
            var newF = this.F(this.N);
            var newD = new Vector3D(
                newF.X - f.X,
                newF.Y - f.Y,
                newF.Z - f.Z);

            // b = MathFunctions.NormaVector(newD); // Реальное смещение

            Delta = new Vector3D(
                newF.X - p.X,
                newF.Y - p.Y,
                newF.Z - p.Z);

            resIterCount++;
            delta.Add(MathFunctions.NormaVector(Delta));
        }

        // TODO: добавить out параметр repeatedIterCount
        public void ActiveSetMethod(Point3D p, ref List<double> delta, ref List<int> countOfLeftLimitAchievements, ref List<int> countOfRightLimitAchievements, ref double cond, double condTreshold, ref int resIterCount, out int repeatedIterCount, out int lambdaRecalculatedCount)
        {
            #region Гипотеза #1 - множество активных ограничений пусто

            this.Build_S_ForAllUnits_ByUnitsType();
            this.Calc_T();
            var f = this.F(this.N);
            var D = new Vector3D(
                p.X - f.X,
                p.Y - f.Y,
                p.Z - f.Z);

            //d = MathFunctions.NormaVector(D); // Желаемое смещение

            this.Build_dS();
            this.Calc_dT();
            this.Build_D();
            this.Calc_C();

            this.detC = Matrix.Det3D(this.C);
            var μ = Matrix.System3x3Solver(this.C, this.detC, D);

            var dQ = new double[this.N];
            for (var i = 0; i < this.N; i++)
            {
                var dF = this.Get_dF(i);
                dQ[i] = ((μ.X * dF.X) + (μ.Y * dF.Y) + (μ.Z * dF.Z)) / this.A[i];
            }

            #endregion

            Dictionary<int, double> λ_left = null;
            Dictionary<int, double> λ_right = null;
            // Находим номера нарушенных ограничений и пересчитываем матрицы
            this.curV = 0;
            var leftJ = this.GetLeftJ(dQ);
            var rightJ = this.GetRightJ(dQ);
            if (!this.CompareLeftRightSets(leftJ, rightJ))
                throw new Exception("Пересечение множества левых и правых ограничений");
            var newCountOfLeftLimitAchievements = leftJ.Count;
            var newCountOfRightLimitAchievements = rightJ.Count;
            countOfLeftLimitAchievements.Add(newCountOfLeftLimitAchievements);
            countOfRightLimitAchievements.Add(newCountOfRightLimitAchievements);
            repeatedIterCount = 0;
            lambdaRecalculatedCount = 0;
            while (this.curV > 0)
            {
                repeatedIterCount++;
                if (repeatedIterCount > RepeatedIterCount)
                {
                    Console.WriteLine($"RepeatedIterCount exceeded max value: {RepeatedIterCount}");
                    break;
                }
                var tmpCountOfLeftLimitAchievements = newCountOfLeftLimitAchievements;
                var tmpCountOfRightLimitAchievements = newCountOfRightLimitAchievements;
                var currIterLambdaRecalculationCount = 0;
                lambdaRecalculatedCount--;
                do
                {
                    tmpCountOfLeftLimitAchievements = newCountOfLeftLimitAchievements;
                    tmpCountOfRightLimitAchievements = newCountOfRightLimitAchievements;
                    //countOfLeftLimitAchievements.Add(newCountOfLeftLimitAchievements);
                    //countOfRightLimitAchievements.Add(newCountOfRightLimitAchievements);
                    this.OffsetQ(dQ);
                    this.Build_S_ForAllUnits_ByUnitsType();
                    this.Calc_T();
                    f = this.F(this.N);

                    this.Calc_dT();
                    this.Build_D();
                    this.Calc_C(leftJ, rightJ);
                    this.detC = Matrix.Det3D(this.C);

                    var sumOfdFLeftJ = SumOfdFLeftJ(leftJ);
                    var sumOfdFRightJ = SumOfdFRightJ(rightJ);
                    D = new Vector3D(
                         -(p.X - f.X - sumOfdFLeftJ.X - sumOfdFRightJ.X),
                         -(p.Y - f.Y - sumOfdFLeftJ.Y - sumOfdFRightJ.Y),
                         -(p.Z - f.Z - sumOfdFLeftJ.Z - sumOfdFRightJ.Z));
                    μ = Matrix.System3x3Solver(this.C, this.detC, D);

                    λ_left = new Dictionary<int, double>();
                    for (int j = 0; j < leftJ.Count; j++)
                    {
                        var indexJ = (int)leftJ[j];
                        var dF = this.Get_dF(indexJ);
                        var dQj = this.Units[indexJ].qMin - this.Units[indexJ].Q;
                        var condidate_λ_left = μ.X * dF.X + μ.Y * dF.Y + μ.Z * dF.Z + this.A[indexJ] * dQj;
                        if (condidate_λ_left < 0.0)
                            leftJ.Remove(indexJ);
                        else
                            λ_left[indexJ] = condidate_λ_left;
                    }

                    λ_right = new Dictionary<int, double>();
                    for (int j = 0; j < rightJ.Count; j++)
                    {
                        var indexJ = (int)rightJ[j];
                        var dF = this.Get_dF(indexJ);
                        var dQj = this.Units[indexJ].qMax - this.Units[indexJ].Q;
                        var condidate_λ_right = -(μ.X * dF.X + μ.Y * dF.Y + μ.Z * dF.Z) - this.A[indexJ] * dQj;
                        if (condidate_λ_right < 0.0)
                            rightJ.Remove(indexJ);
                        else
                            λ_right[indexJ] = condidate_λ_right;
                    }
                    newCountOfLeftLimitAchievements = leftJ.Count;
                    newCountOfRightLimitAchievements = rightJ.Count;
                    lambdaRecalculatedCount++;
                    currIterLambdaRecalculationCount++;
                    dQ = new double[this.N];
                    for (var i = 0; i < this.N; i++)
                    {
                        var dF = this.Get_dF(i);

                        var λi_left = leftJ.Contains(i) ? λ_left[i] : 0.0; // the same as var λi_left = λ_left[i];
                        var λi_right = rightJ.Contains(i) ? λ_right[i] : 0.0; // the same as var λi_right = λ_right[i];

                        dQ[i] = -((μ.X * dF.X) + (μ.Y * dF.Y) + (μ.Z * dF.Z) - λi_left + λi_right) / this.A[i];
                    }
                    if (currIterLambdaRecalculationCount > this.MaxLambdaRecalculationCount)
                    {
                        break;
                    }

                } while (newCountOfLeftLimitAchievements != tmpCountOfLeftLimitAchievements || newCountOfRightLimitAchievements != tmpCountOfRightLimitAchievements);


                this.curV = 0;
                leftJ = this.GetLeftJ(dQ);
                rightJ = this.GetRightJ(dQ);
                if (!this.CompareLeftRightSets(leftJ, rightJ))
                    throw new Exception("Пересечение множества левых и правых ограничений");
                newCountOfLeftLimitAchievements = leftJ.Count;
                newCountOfRightLimitAchievements = rightJ.Count;
            }

            this.OffsetQ(dQ);
            this.Build_S_ForAllUnits_ByUnitsType();
            this.Calc_T();
            var newF = this.F(this.N);
            var newD = new Vector3D(
                newF.X - f.X,
                newF.Y - f.Y,
                newF.Z - f.Z);
            var Delta = new Vector3D(
                newF.X - p.X,
                newF.Y - p.Y,
                newF.Z - p.Z);
            delta.Add(MathFunctions.NormaVector(Delta));
            resIterCount++;

            if (cond == 0)
            {
                if (this.detC == 0)
                {
                    cond = double.MaxValue;
                }
                else
                {
                    cond = this.C.NormF() * this.C.Invert3D(this.detC).NormF();
                    // Console.WriteLine("Condition number of matrix C is " + cond + "\n");
                }
            }
        }

        // если есть пересечение -> false, если нет пересечений  -> true
        public bool CompareLeftRightSets(ArrayList left, ArrayList right)
        {
            if (left.Count > right.Count)
            {
                foreach(var j in left)
                {
                    if (right.Contains(j))
                        return false;
                }
                return true;
            }
            else
            {
                foreach (var j in right)
                {
                    if (left.Contains(j))
                        return false;
                }
                return true;
            }
        }

        public ArrayList GetLeftJ(double[] dQ)
        {
            var leftJ = new ArrayList();
            for (int i = 0; i < Units.Length; i++)
            {
                if (this.maxV / 2 > curV)
                {
                    var unit = this.Units[i];
                    if (unit.qMin > unit.Q + dQ[i])
                    {
                        leftJ.Add(i);
                        curV++;
                    }
                }
                else
                    break;
            }
            return leftJ;
        }

        public ArrayList GetRightJ(double[] dQ)
        {
            var rightJ = new ArrayList();
            for (int i = 0; i < Units.Length; i++)
            {
                if (this.maxV / 2 > curV)
                {
                    var unit = this.Units[i];
                    if (unit.qMax < unit.Q + dQ[i])
                    {
                        rightJ.Add(i);
                        curV++;
                    }
                }
                else
                    break;
            }
            return rightJ;
        }

        private Vector3D dFConstLims(double[] dQ)
        {
            var result = new Vector3D();
            for (var i = 0; i < Units.Length; i++)
            {
                if (!Units[i].v)
                {
                    var dFj = this.Get_dF(i);
                    result.X += dFj.X * dQ[i];
                    result.Y += dFj.Y * dQ[i];
                    result.Z += dFj.Z * dQ[i];
                }
            }
            return result;
        }

        private Vector3D SumOfdFLeftJ(ArrayList leftJ)
        {
            var result = new Vector3D();
            foreach(int j in leftJ)
            {
                var dFj = this.Get_dF(j);
                var dQ = this.Units[j].qMin - this.Units[j].Q;
                result.X += dFj.X * dQ;
                result.Y += dFj.Y * dQ;
                result.Z += dFj.Z * dQ;
            }
            return result;
        }

        private Vector3D SumOfdFRightJ(ArrayList ritghtJ)
        {
            var result = new Vector3D();
            foreach (int j in ritghtJ)
            {
                var dFj = this.Get_dF(j);
                var dQ = this.Units[j].qMax - this.Units[j].Q;
                result.X += dFj.X * dQ;
                result.Y += dFj.Y * dQ;
                result.Z += dFj.Z * dQ;
            }
            return result;
        }

        public double FunctionQ(double[] q)
        {
            var res = 0.0;
            for (var i = 0; i < this.N; i++)
            {
                res += this.A[i] * Math.Pow(q[i], 2);
            }

            return res;
        }

        public double FunctionQ()
        {
            var res = 0.0;
            for (var i = 0; i < this.N; i++)
            {
                res += this.A[i] * Math.Pow(this.Units[i].Q, 2);
            }

            return res;
        }

        private double[] GetProjectionOfQForLimitations(double[] dQ, ref int curV, out int countOfLeftLimitAchievements, out int countOfRightLimitAchievements)
        {
            countOfLeftLimitAchievements = 0;
            countOfRightLimitAchievements = 0;
            var newdQ = new double[this.N];
            for (var i = 0; i < this.N; i++)
            {
                if (!MathFunctions.SegmentContains(this.Units[i].qMin, this.Units[i].qMax, this.Units[i].Q + dQ[i]) || this.maxV < curV)
                {
                    if (this.Units[i].Q + dQ[i] < this.Units[i].qMin)
                        countOfLeftLimitAchievements++;
                    if (this.Units[i].Q + dQ[i] > this.Units[i].qMax)
                        countOfRightLimitAchievements++;
                    var projectionQ = MathFunctions.Projection(this.Units[i].qMin, this.Units[i].qMax, this.Units[i].Q + dQ[i]);
                    newdQ[i] = projectionQ - this.Units[i].Q;
                    this.Units[i].v = false;
                    curV++;
                }
                else
                {
                    this.Units[i].v = true;
                    newdQ[i] = dQ[i];
                }
            }
            return newdQ;
        }

        private bool IsAnyVFalse()
        {
            for (var i = 0; i < this.N; i++)
            {
                if (this.Units[i].v == false)
                    return true;
            }
            return false;
        }

        #region Legacy of hardcoded RRPR arm

        public double[] RRPR_LagrangeMethodToThePoint(Point3D p, out double cond, bool withCond)
        {
            var dQ = new double[this.N];

            var f = this.F(this.N);
            var d = new Vector3D(
                p.X - f.X,
                p.Y - f.Y,
                p.Z - f.Z);

            var C = new Matrix(3, 3);
            // C xx
            C[0, 0] = this.expected_dFxdq1 * this.expected_dFxdq1 / this.A[0] + this.expected_dFxdq2 * this.expected_dFxdq2 / this.A[1] + this.expected_dFxdq3 * this.expected_dFxdq3 / this.A[2] + this.expected_dFxdq4 * this.expected_dFxdq4 / this.A[3];
            // C xy
            C[0, 1] = this.expected_dFxdq1 * this.expected_dFydq1 / this.A[0] + this.expected_dFxdq2 * this.expected_dFydq2 / this.A[1] + this.expected_dFxdq3 * this.expected_dFydq3 / this.A[2] + this.expected_dFxdq4 * this.expected_dFydq4 / this.A[3];
            // C xz
            C[0, 2] = this.expected_dFxdq1 * this.expected_dFzdq1 / this.A[0] + this.expected_dFxdq2 * this.expected_dFzdq2 / this.A[1] + this.expected_dFxdq3 * this.expected_dFzdq3 / this.A[2] + this.expected_dFxdq4 * this.expected_dFzdq4 / this.A[3];
            // C yy
            C[1, 1] = this.expected_dFydq1 * this.expected_dFydq1 / this.A[0] + this.expected_dFydq2 * this.expected_dFydq2 / this.A[1] + this.expected_dFydq3 * this.expected_dFydq3 / this.A[2] + this.expected_dFydq4 * this.expected_dFydq4 / this.A[3];
            // C yz
            C[1, 2] = this.expected_dFydq1 * this.expected_dFzdq1 / this.A[0] + this.expected_dFydq2 * this.expected_dFzdq2 / this.A[1] + this.expected_dFydq3 * this.expected_dFzdq3 / this.A[2] + this.expected_dFydq4 * this.expected_dFzdq4 / this.A[3];
            // C zz
            C[2, 2] = this.expected_dFzdq1 * this.expected_dFzdq1 / this.A[0] + this.expected_dFzdq2 * this.expected_dFzdq2 / this.A[1] + this.expected_dFzdq3 * this.expected_dFzdq3 / this.A[2] + this.expected_dFzdq4 * this.expected_dFzdq4 / this.A[3];
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
        public void Build_D()//bool withLimitations = false)
        {
            for (var i = 0; i < this.N; i++)
            {
                //if (withLimitations)
                //{
                //    if (!this.Units[i].v && this.maxV > curV)
                //    {
                //        var b = ((Matrix)this.dT[i]).ColumnAsVector3D(3);
                //        this.rightResidueD[0, i] = b.X * this.Units[i].Q;
                //        this.rightResidueD[1, i] = b.Y * this.Units[i].Q;
                //        this.rightResidueD[2, i] = b.Z * this.Units[i].Q;
                //    }
                //    else 
                //    {
                //        var b = ((Matrix)this.dT[i]).ColumnAsVector3D(3);
                //        this.D[0, i] = b.X;
                //        this.D[1, i] = b.Y;
                //        this.D[2, i] = b.Z;
                //    }
                //}
                //else
                //{
                    var b = ((Matrix)this.dT[i]).ColumnAsVector3D(3);
                    this.D[0, i] = b.X;
                    this.D[1, i] = b.Y;
                    this.D[2, i] = b.Z;
                //}
            }
        }

        // That function return vector ( dFxqi, dFyqi, dFzqi )
        public Vector3D Get_dF(int i)
        {
            var dFdQi = this.D.ColumnAsVector3D(i);
            return dFdQi;
        }

        public void Print_dF(int i)
        {
            var dFdQi = this.D.ColumnAsVector3D(i);
            Console.WriteLine($"dFxdQ{i + 1} = {dFdQi.X}");
            Console.WriteLine($"dFydQ{i + 1} = {dFdQi.Y}");
            Console.WriteLine($"dFzdQ{i + 1} = {dFdQi.Z}");
            Console.WriteLine();
        }

        // Вычисляем матрицу коэффициентов для метода Лагранжа
        public void Calc_C(bool withLimitations = false)
        {
            this.C = new Matrix(3, 3);
            for (var i = 0; i < this.N; i++)
            {
                if (withLimitations && !this.Units[i].v)
                {
                    continue;
                }
                var dF = this.Get_dF(i);
                
                // C xx
                this.C[0, 0] += dF.X * dF.X / this.A[i];
                
                // C xy
                this.C[0, 1] += dF.X * dF.Y / this.A[i];

                // C xz
                this.C[0, 2] += dF.X * dF.Z / this.A[i];

                // C yy
                this.C[1, 1] += dF.Y * dF.Y / this.A[i];

                // C yz
                this.C[1, 2] += dF.Y * dF.Z / this.A[i];

                // C zz
                this.C[2, 2] += dF.Z * dF.Z / this.A[i];
            }

            // C yx
            this.C[1, 0] = this.C[0, 1];
            
            // C zx
            this.C[2, 0] = this.C[0, 2];
            
            // C zy
            this.C[2, 1] = this.C[1, 2];
        }

        public void Calc_C(ArrayList leftJ, ArrayList rightJ)
        {
            //var tmpV = 0;
            this.C = new Matrix(3, 3);
            for (var i = 0; i < this.N; i++)
            {
                if (leftJ.Contains(i) || rightJ.Contains(i))// && tmpV < this.maxV)
                {
                    //tmpV++;
                    continue;
                }
                var dF = this.Get_dF(i);

                // C xx
                this.C[0, 0] += dF.X * dF.X / this.A[i];

                // C xy
                this.C[0, 1] += dF.X * dF.Y / this.A[i];

                // C xz
                this.C[0, 2] += dF.X * dF.Z / this.A[i];

                // C yy
                this.C[1, 1] += dF.Y * dF.Y / this.A[i];

                // C yz
                this.C[1, 2] += dF.Y * dF.Z / this.A[i];

                // C zz
                this.C[2, 2] += dF.Z * dF.Z / this.A[i];
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
