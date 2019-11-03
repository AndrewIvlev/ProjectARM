using System;
using System.Windows.Media.Media3D;
using Newtonsoft.Json;

namespace ArmManipulatorArm.MathModel.Matrix
{
    public class Matrix
    {
        public double[,] M;

        [JsonIgnore]
        public int Rows;

        [JsonIgnore]
        public int Columns;

        public Matrix()
        {
            M = new double[Rows = 0, Columns = 0];
        }

        public Matrix(int n)
        {
            M = new double[Rows = n, Columns = n];
            for (var i = 0; i < Rows; i++)
            for (var j = 0; j < Columns; j++)
                M[i, j] = 0;
        }

        public Matrix(int rows, int columns)
        {
            M = new double[Rows = rows, Columns = columns];
            for (var i = 0; i < rows; i++)
            for (var j = 0; j < columns; j++)
                M[i, j] = 0;
        }

        public Matrix(Matrix A)
        {
            M = new double[Rows = A.Rows, Columns = A.Columns];
            for (var i = 0; i < Rows; i++)
            for (var j = 0; j < Columns; j++)
                M[i, j] = A.M[i, j];
        }

        public double this[int i, int j]
        {
            get => M[i, j];
            set => M[i, j] = value;
        }

        public static bool operator ==(Matrix a, Matrix b)
        {
            if (a.Rows != b.Rows || a.Columns != b.Columns)
                return false;

            for(var i = 0; i < a.Rows; i++)
            {
                for (var j = 0; j < a.Columns; j++)
                {
                    if (Math.Abs(a[i, j] - b[i, j]) > 0)
                    {
                        Console.WriteLine($"Element left matrix [{i},{j}] = {a[i, j]} not equal to element of right matrix = {b[i, j]}");
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool operator !=(Matrix a, Matrix b)
        {
            if (a.Rows != b.Rows || a.Columns != b.Columns)
                return true;

            var isEqual = true;
            for (var i = 0; i < a.Rows; i++)
            for (var j = 0; j < a.Columns; j++)
                if (Math.Abs(a[i, j] - b[i, j]) > 0)
                    isEqual = false;

            return !isEqual;
        }

        // TODO: use Parallel.For method from the .NET Task Parallel Library
        public static Matrix operator *(Matrix A, Matrix B)
        {
            if (A.Columns != B.Rows)
                throw new Exception(
                    "Matrices are not conformable");

            var AB = new Matrix(A.Rows, B.Columns);

            for (var j = 0; j < B.Columns; j++)
                for (var i = 0; i < A.Rows; i++)
                for (var k = 0; k < A.Columns; k++)
                    AB[i, j] += A[i, k] * B[k, j];

            return AB;
        }

        public static double Det2D(Matrix m) => m[0, 0] * m[1, 1] - m[0, 1] * m[1, 0];

        public static double Det3D(Matrix m) =>
            m[0, 0] * (m[1, 1] * m[2, 2] - m[2, 1] * m[1, 2]) 
            - m[0, 1] * (m[1, 0] * m[2, 2] - m[1, 2] * m[2, 0])
            + m[0, 2] * (m[1, 0] * m[2, 1] - m[2, 0] * m[1, 1]);

        public Matrix Invert3D(double det)
        {
            if (this.Rows != 3 || this.Columns != 3)
            {
                throw new Exception("Matrix should be 3x3 size.");
            }

            var ad11 = Det2D(new Matrix(2)
                                 {
                                     [0, 0] = this[1, 1], [0, 1] = this[1, 2],
                                     [1, 0] = this[2, 1], [1, 1] = this[2, 2]
                                 });
            var ad12 = -Det2D(new Matrix(2)
                                 {
                                     [0, 0] = this[1, 0], [0, 1] = this[1, 2],
                                     [1, 0] = this[2, 0], [1, 1] = this[2, 2]
                                 });
            var ad13 = Det2D(new Matrix(2)
                                 {
                                     [0, 0] = this[1, 0], [0, 1] = this[1, 1],
                                     [1, 0] = this[2, 0], [1, 1] = this[2, 1]
                                 });
            var ad21 = -Det2D(new Matrix(2)
                                 {
                                     [0, 0] = this[0, 1], [0, 1] = this[0, 2],
                                     [1, 0] = this[2, 1], [1, 1] = this[2, 2]
                                 });
            var ad22 = Det2D(new Matrix(2)
                                 {
                                     [0, 0] = this[0, 0], [0, 1] = this[0, 2],
                                     [1, 0] = this[2, 0], [1, 1] = this[2, 2]
                                 });
            var ad23 = -Det2D(new Matrix(2)
                                 {
                                     [0, 0] = this[0, 0], [0, 1] = this[0, 1],
                                     [1, 0] = this[2, 0], [1, 1] = this[2, 1]
                                 });
            var ad31 = Det2D(new Matrix(2)
                                 {
                                     [0, 0] = this[0, 1], [0, 1] = this[0, 2],
                                     [1, 0] = this[1, 1], [1, 1] = this[1, 2]
                                 });
            var ad32 = -Det2D(new Matrix(2)
                                 {
                                     [0, 0] = this[0, 0], [0, 1] = this[0, 2],
                                     [1, 0] = this[1, 0], [1, 1] = this[1, 2]
                                 });
            var ad33 = Det2D(new Matrix(2)
                                 {
                                     [0, 0] = this[0, 0], [0, 1] = this[0, 1],
                                     [1, 0] = this[1, 0], [1, 1] = this[1, 1]
                                 });

            return new Matrix(3)
                       {
                            [0, 0] = ad11 / det, [0, 1] = ad21 / det, [0, 2] = ad31 / det,
                            [1, 0] = ad12 / det, [1, 1] = ad22 / det, [1, 2] = ad32 / det,
                            [2, 0] = ad13 / det, [2, 1] = ad23 / det, [2, 2] = ad33 / det
                       };
        }

        public void SwapColumns(int c1, int c2)
        {
            for (var i = 0; i < this.Rows; i++)
            {
                var tmp = this.M[i, c1];
                this.M[i, c1] = this.M[i, c2];
                this.M[i, c2] = tmp;
            }
        }

        public void SwapRows(int r1, int r2)
        {
            for (var i = 0; i < this.Columns; i++)
            {
                var tmp = this.M[r1, i];
                this.M[r1, i] = this.M[r2, i];
                this.M[r2, i] = tmp;
            }
        }

        public double NormF()
        {
            var frobeniusNorm = 0.0;
            for (var i = 0; i < this.Rows; i++)
            {
                for (var j = 0; j < this.Columns; j++)
                {
                    frobeniusNorm += this[i, j] * this[i, j];
                }
            }

            return Math.Sqrt(frobeniusNorm);
        }

        public void ToE()
        {
            if (this.Rows != this.Columns)
            {
                throw new Exception("Matrix should be square size.");
            }
            for (var i = 0; i < this.Rows; i++)
            {
                for (var j = 0; j < this.Columns; j++)
                {
                    this.M[i, j] = i == j ? 1 : 0;
                }
            }
        }

        /// <summary>
        /// Returns the transpose matrix
        /// </summary>
        public static Matrix Transpose(Matrix M)
        {
            var transM = new Matrix(M.Columns, M.Rows);

            for (var i = 0; i < M.Rows; i++)
            for (var j = 0; j < M.Columns; j++)
                transM[j, i] = M[i, j];

            return transM;
        }

        /// <summary>
        /// Replacing column in matrix with vector
        /// </summary>
        /// <param name="a">Matrix</param>
        /// <param name="v">Column</param>
        /// <param name="i">Index</param>
        public static Matrix ConcatAsColumn(Matrix a, Point3D v, int i)
        {
            if (a.Rows != 3)
                throw new ArgumentOutOfRangeException();

            var res = new Matrix(a)
            {
                [0, i] = v.X,
                [1, i] = v.Y,
                [2, i] = v.Z
            };
            
            return res;
        }

        public void Print()
        {
            for (var i = 0; i < this.Rows; i++)
            {
                for (var j = 0; j < this.Columns; j++)
                {
                    Console.Write(this[i, j] + @" ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}
