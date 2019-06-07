using System;
using Newtonsoft.Json;

namespace ManipulationSystemLibrary.Matrix
{
    //TODO: make this class as template class, not only for double matrix
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

        protected bool Equals(Matrix other)
        {
            if (Rows != other.Rows || Columns != other.Columns)
                return false;
            for (var i = 0; i < Rows; i++)
            for (var j = 0; j < Columns; j++)
                if (this.M[i, j] != other.M[i, j])
                    return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Matrix) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (M != null ? M.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Rows;
                hashCode = (hashCode * 397) ^ Columns;
                return hashCode;
            }
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
    }
}
