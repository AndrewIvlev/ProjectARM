using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManipulationSystemLibrary
{
    public class Matrix
    {
        public double[,] M;
        public int rows;
        public int columns;

        public Matrix()
        {
            M = new double[rows = 0, columns = 0];
        }

        public Matrix(int n)
        {
            M = new double[rows = n, columns = n];
            for (int i = 0; i < rows; i++)
            for (int j = 0; j < columns; j++)
                M[i, j] = 0;
        }

        public Matrix(int rows, int columns)
        {
            M = new double[this.rows = rows, this.columns = columns];
            for (int i = 0; i < rows; i++)
            for (int j = 0; j < columns; j++)
                M[i, j] = 0;
        }

        public Matrix(Matrix A)
        {
            M = new double[rows = A.rows, columns = A.columns];
            for (int i = 0; i < rows; i++)
            for (int j = 0; j < columns; j++)
                M[i, j] = A.M[i, j];
        }

        public double this[int i, int j]
        {
            get => M[i, j];
            set => M[i, j] = value;
        }

        public static bool operator ==(Matrix A, Matrix B)
        {
            if (A.rows != B.rows || A.columns != B.columns)
                return false;

            for(int i = 0; i < A.rows; i++)
                for (int j = 0; j < A.columns; j++)
                    if (A[i, j] != B[i, j])
                        return false;

            return true;
        }

        public static bool operator !=(Matrix A, Matrix B)
        {
            if (A.rows != B.rows || A.columns != B.columns)
                return true;

            var isEqual = true;
            for (int i = 0; i < A.rows; i++)
            for (int j = 0; j < A.columns; j++)
                if (A[i, j] != B[i, j])
                    isEqual = false;

            return !isEqual;
        }

        // TODO: use Parallel.For method from the .NET Task Parallel Library
        public static Matrix operator *(Matrix A, Matrix B)
        {
            if (A.columns != B.rows)
                throw new Exception(
                    "Matrices are not conformable");

            var AB = new Matrix(A.rows, B.columns);

            for (int j = 0; j < B.columns; j++)
                for (int i = 0; i < A.rows; i++)
                for (int k = 0; k < A.columns; k++)
                    AB[i, j] += A[i, k] * B[k, j];

            return AB;
        }

        /// <summary>
        /// Returns the transpose matrix
        /// </summary>
        public static Matrix Transpose(Matrix M)
        {
            var transpM = new Matrix(M.columns, M.rows);

            for (int i = 0; i < M.rows; i++)
            for (int j = 0; j < M.columns; j++)
                transpM[j, i] = M[i, j];

            return transpM;
        }
    }
}
