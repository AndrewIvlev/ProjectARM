using System;
using System.Windows.Media.Media3D;
using Newtonsoft.Json;

namespace ManipulationSystemLibrary
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
                for (var j = 0; j < a.Columns; j++)
                    if (Math.Abs(a[i, j] - b[i, j]) > 0)
                        return false;

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


        public static double Det3D(Matrix m) =>
            m[0, 0] * (m[1, 1] * m[2, 2] - m[2, 1] * m[1, 2])
            - m[0, 1] * (m[1, 0] * m[2, 2] - m[1, 2] * m[2, 0])
            + m[0, 2] * (m[1, 0] * m[2, 2] - m[2, 0] * m[1, 1]);

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
    }
}
