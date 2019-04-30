using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectARM
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

        public Matrix(int rows, int columns)
        {
            M = new double[this.rows = rows, this.columns = columns];
            for (int i = 0; i < rows; i++)
            for (int j = 0; j < columns; j++)
                M[i, j] = 0;
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

        /// <summary>
        /// Returns invertible matrix
        /// </summary>
        //public static Matrix Inverse(Matrix M)
        //{
        //    if(M.rows != M.columns)
        //        throw new Exception("Matrix is not square");

        //    var inverseM = new Matrix(M.rows, M.rows);

        //    return inverseM;
        //}

        //TODO: remove this method when we use 'unsafe' context
        public void SwapRows(int i, int j)
        {
            for (int k = 0; k < columns; k++)
            {
                var tmp = M[i, k];
                M[i, k] = M[j, k];
                M[j, k] = tmp;
            }
        }

        public int IndexOfMaxElemInRow(int row)
        {
            int indexOfMax = 0;
            var max = M[row, 0];
            for (int i = 1; i < columns; i++)
            {
                if (M[row, i] > max)
                {
                    max = M[row, i];
                    indexOfMax = i;
                }
            }
            return indexOfMax;
        }

        public static Matrix Inverse(Matrix M)
        {
            // assumes determinant is not 0
            // that is, the matrix does have an inverse
            int n = M.rows;
            Matrix result = new Matrix(n, n); // make a copy of matrix
            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                    result[i, j] = M[i, j];

            Matrix lum; // combined lower & upper
            int[] perm;
            int toggle;
            toggle = MatrixDecompose(M, out lum, out perm);

            double[] b = new double[n];
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                    if (i == perm[j])
                        b[j] = 1.0;
                    else
                        b[j] = 0.0;

                double[] x = Helper(lum, b);
                for (int j = 0; j < n; ++j)
                    result[j, i] = x[j];
            }
            return result;
        }

        static int MatrixDecompose(Matrix m, out Matrix lum, out int[] perm)
        {
            // Crout's LU decomposition for matrix determinant and inverse
            // stores combined lower & upper in lum[][]
            // stores row permuations into perm[]
            // returns +1 or -1 according to even or odd number of row permutations
            // lower gets dummy 1.0s on diagonal (0.0s above)
            // upper gets lum values on diagonal (0.0s below)

            int toggle = +1; // even (+1) or odd (-1) row permutatuions
            int n = m.rows;

            // make a copy of m[][] into result lu[][]
            lum = new Matrix(n, n);
            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                    lum[i, j] = m[i, j];


            // make perm[]
            perm = new int[n];
            for (int i = 0; i < n; ++i)
                perm[i] = i;

            for (int j = 0; j < n - 1; ++j) // process by column. note n-1 
            {
                double max = Math.Abs(lum[j, j]);
                int piv = j;

                for (int i = j + 1; i < n; ++i) // find pivot index
                {
                    double xij = Math.Abs(lum[i, j]);
                    if (xij > max)
                    {
                        max = xij;
                        piv = i;
                    }
                }

                if (piv != j)
                {
                    lum.SwapRows(j, piv);

                    int t = perm[piv]; // swap perm elements
                    perm[piv] = perm[j];
                    perm[j] = t;

                    toggle = -toggle;
                }

                double xjj = lum[j, j];
                if (xjj != 0.0)
                {
                    for (int i = j + 1; i < n; ++i)
                    {
                        double xij = lum[i, j] / xjj;
                        lum[i, j] = xij;
                        for (int k = j + 1; k < n; ++k)
                            lum[i, k] -= xij * lum[j, k];
                    }
                }

            }

            return toggle;
        }

        static double[] Helper(Matrix luMatrix, double[] b)
        {
            int n = luMatrix.rows;
            double[] x = new double[n];
            b.CopyTo(x, 0);

            for (int i = 1; i < n; ++i)
            {
                double sum = x[i];
                for (int j = 0; j < i; ++j)
                    sum -= luMatrix[i, j] * x[j];
                x[i] = sum;
            }

            x[n - 1] /= luMatrix[n - 1, n - 1];
            for (int i = n - 2; i >= 0; --i)
            {
                double sum = x[i];
                for (int j = i + 1; j < n; ++j)
                    sum -= luMatrix[i, j] * x[j];
                x[i] = sum / luMatrix[i, i];
            }

            return x;
        }

        static double MatrixDeterminant(Matrix matrix)
        {
            Matrix lum;
            int[] perm;
            int toggle = MatrixDecompose(matrix, out lum, out perm);
            double result = toggle;
            for (int i = 0; i < lum.rows; ++i)
                result *= lum[i, i];
            return result;
        }

        static Matrix ExtractLower(Matrix lum)
        {
            // lower part of an LU Doolittle decomposition (dummy 1.0s on diagonal, 0.0s above)
            int n = lum.rows;
            var result = new Matrix(n, n);
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    if (i == j)
                        result[i, j] = 1.0;
                    else if (i > j)
                        result[i, j] = lum[i, j];
                }
            }
            return result;
        }

        static Matrix ExtractUpper(Matrix lum)
        {
            // upper part of an LU (lu values on diagional and above, 0.0s below)
            int n = lum.rows;
            var result = new Matrix(n, n);
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    if (i <= j)
                        result[i, j] = lum[i, j];
                }
            }
            return result;
        }

        #region  Turn on 'Allow unsafe code'

        static unsafe void SMul(double* rmDestination, double* rmX, int xn, int xm, double* rmY, int ym)
        {
            for (double* _dest_end = rmDestination + xn * ym; rmDestination < _dest_end; rmDestination += ym)
                for (double* _x_i_end = rmX + xm, _dest_i_end = rmDestination + ym, _y_tmp = rmY; rmX < _x_i_end; rmX++)
                {
                    //сохраняем текущее значение для многократного использования
                    double cache_val = *rmX;
                    for (double* _dest_ij = rmDestination; _dest_ij < _dest_i_end; _dest_ij++, _y_tmp++)
                        *_dest_ij += cache_val * *_y_tmp;
                }
        }

        static unsafe double[,] SMul(double[,] X, double[,] Y)
        {
            int xm = X.GetLength(1);
            if (xm == Y.GetLength(1))
            {
                int xn = X.GetLength(0);
                int ym = Y.GetLength(1);
                double[,] result = new double[xn, ym];
                if (result.Length > 0)
                    fixed (double* p_result = &result[0, 0])
                    fixed (double* p_x = &X[0, 0])
                    fixed (double* p_y = &Y[0, 0])
                        SMul(p_result, p_x, xn, xm, p_y, ym);
                return result;
            }
            else throw new ArgumentException();
        }

        static unsafe double Det(double* rmX, int n)
        {
            double* mtx_u_ii, mtx_ii_j;
            double* mtx_end = rmX + n * (n - 1), mtx_u_ii_j = null;
            double val, det = 1;
            int d = 0;
            // rmX указывает на (i,i) элемент на каждом шаге и называется ведущим
            for (double* mtx_ii_end = rmX + n; rmX < mtx_end; rmX += n + 1, mtx_ii_end += n, d++)
            {
                // Ищем максимальный элемент в столбце(под ведущим) 
                {
                    //Ищем максимальный элемент и его позицию
                    val = Math.Abs(*(mtx_ii_j = rmX));
                    for (mtx_u_ii = rmX + n; mtx_u_ii < mtx_end; mtx_u_ii += n)
                    {
                        if (val < Math.Abs(*mtx_u_ii))
                            val = Math.Abs(*(mtx_ii_j = mtx_u_ii));
                    }
                    //Если максимальный эдемент = 0 -> матрица вырожденная
                    if (val == 0) return double.NaN;
                    //Если ведущий элемент не является максимальным - делаем перестановку строк и меняем знак определителя
                    else if (mtx_ii_j != rmX)
                    {
                        det = -det;
                        for (mtx_u_ii = rmX; mtx_u_ii < mtx_ii_end; mtx_ii_j++, mtx_u_ii++)
                        {
                            val = *mtx_u_ii;
                            *mtx_u_ii = *mtx_ii_j;
                            *mtx_ii_j = val;
                        }
                    }
                }
                //Обнуляем элементы под ведущим
                for (mtx_u_ii = rmX + n, mtx_u_ii_j = mtx_end + n; mtx_u_ii < mtx_u_ii_j; mtx_u_ii += d)
                {
                    val = *(mtx_u_ii++) / *rmX;
                    for (mtx_ii_j = rmX + 1; mtx_ii_j < mtx_ii_end; mtx_u_ii++, mtx_ii_j++)
                        *mtx_u_ii -= *mtx_ii_j * val;
                }
                det *= *rmX;
            }
            return det *= *rmX;
        }

        static unsafe double Det(double[,] A)
        {
            int n = A.GetLength(0);
            if (n == A.GetLength(1))
            {
                double[] temp = new double[A.Length];
                Buffer.BlockCopy(A, 0, temp, 0, temp.Length * sizeof(double));
                fixed (double* pm = &temp[0]) return Det(pm, n);
            }
            else throw new RankException();
        }

        static unsafe bool SolveSqSysEq(double* result, int rcol, double* mtx, int n)
        {
            double* mtx_u_ii, mtx_ii_j;
            double val;
            double* mtx_end = mtx + n * n, result_i, mtx_u_ii_j = null;
            int d = 0;
            for (double* mtx_ii = mtx, mtx_ii_end = mtx + n, result_i_end = result + rcol; mtx_ii < mtx_end; result = result_i_end, result_i_end += rcol, mtx_ii += n + 1, mtx_ii_end += n, d++)
            {
                {
                    val = Math.Abs(*(mtx_ii_j = mtx_ii));
                    for (mtx_u_ii = mtx_ii + n, result_i = result + rcol; mtx_u_ii < mtx_end; mtx_u_ii += n, result_i += rcol)
                    {
                        if (val < Math.Abs(*mtx_u_ii))
                        {
                            val = Math.Abs(*(mtx_ii_j = mtx_u_ii));
                            mtx_u_ii_j = result_i;
                        }
                    }
                    if (val == 0) return false;
                    else if (mtx_ii_j != mtx_ii)
                    {
                        for (result_i = result; result_i < result_i_end; val = *mtx_u_ii_j, *mtx_u_ii_j = *result_i, *result_i = val, mtx_u_ii_j++, result_i++) ;
                        for (mtx_u_ii = mtx_ii; mtx_u_ii < mtx_ii_end; val = *mtx_u_ii, *mtx_u_ii = *mtx_ii_j, *mtx_ii_j = val, mtx_ii_j++, mtx_u_ii++) ;
                    }
                }
                for (mtx_u_ii = mtx_ii - n, result_i = result - rcol; mtx_u_ii > mtx; mtx_u_ii -= n, result_i -= rcol)
                {
                    val = *(mtx_u_ii) / *mtx_ii;
                    for (mtx_ii_j = mtx_ii + 1, mtx_u_ii_j = mtx_u_ii + 1; mtx_ii_j < mtx_ii_end; mtx_u_ii_j++, mtx_ii_j++)
                        *mtx_u_ii_j -= *mtx_ii_j * val;
                    for (mtx_ii_j = result, mtx_u_ii_j = result_i; mtx_ii_j < result_i_end; mtx_u_ii_j++, mtx_ii_j++)
                        *mtx_u_ii_j -= *mtx_ii_j * val;
                }
                for (mtx_u_ii = mtx_ii + n, result_i = result + rcol; mtx_u_ii < mtx_end; mtx_u_ii += d)
                {
                    val = *(mtx_u_ii++) / *mtx_ii;
                    for (mtx_ii_j = mtx_ii + 1; mtx_ii_j < mtx_ii_end; mtx_u_ii++, mtx_ii_j++) *mtx_u_ii -= *mtx_ii_j * val;
                    for (mtx_ii_j = result; mtx_ii_j < result_i_end; result_i++, mtx_ii_j++) *result_i -= *mtx_ii_j * val;
                }
            }
            for (mtx_end--, result--, n++; mtx_end >= mtx; mtx_end -= n)
            {
                val = *mtx_end;
                for (result_i = result - rcol; result > result_i; result--)
                    *result /= val;
            }
            return true;
        }

        static unsafe double[,] Inverse(double[,] A)
        {
            int n = A.GetLength(0);
            if (n == A.GetLength(1))
            {
                double[,] Result = new double[n, n];
                if (n > 0)
                {
                    fixed (double* pR = &Result[0, 0])
                    {
                        for (double* pr = pR, pr_end = pr + A.Length; pr < pr_end; pr += n + 1) *pr = 1;
                        fixed (double* pA = &(A.Clone() as double[,])[0, 0])
                            if (SolveSqSysEq(pR, n, pA, n)) return Result;
                            else return null;
                    }
                }
                return Result;
            }
            else throw new IndexOutOfRangeException();
        }

        #endregion
    }
}
