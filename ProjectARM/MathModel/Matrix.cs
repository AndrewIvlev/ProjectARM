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
            M = new double[rows, columns];
            for (int i = 0; i < rows; i++)
            for (int j = 0; j < columns; j++)
                M[i, j] = 0;
        }

        public double this[int i, int j]
        {
            get => M[i, j];
            set => M[i, j] = value;
        }

        public static Matrix Transp(Matrix M)
        {
            var transpM = new Matrix(M.columns, M.rows);

            for (int i = 0; i < M.rows; i++)
            for (int j = 0; j < M.columns; j++)
                transpM[j, i] = M[i, j];

            return transpM;
        }

        static unsafe public void _SMul(double* rmDestination, double* rmX, int xn, int xm, double* rmY, int ym)
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

        static unsafe public double[,] SMul(double[,] X, double[,] Y)
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
                        _SMul(p_result, p_x, xn, xm, p_y, ym);
                return result;
            }
            else throw new ArgumentException();
        }
        static unsafe double _Det(double* rmX, int n)
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
                    val = System.Math.Abs(*(mtx_ii_j = rmX));
                    for (mtx_u_ii = rmX + n; mtx_u_ii < mtx_end; mtx_u_ii += n)
                    {
                        if (val < System.Math.Abs(*mtx_u_ii))
                            val = System.Math.Abs(*(mtx_ii_j = mtx_u_ii));
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
        public unsafe static double Det(double[,] A)
        {
            int n = A.GetLength(0);
            if (n == A.GetLength(1))
            {
                double[] temp = new double[A.Length];
                Buffer.BlockCopy(A, 0, temp, 0, temp.Length * sizeof(double));
                fixed (double* pm = &temp[0]) return _Det(pm, n);
            }
            else throw new RankException();
        }
        static unsafe public bool _SolveSqSysEq(double* result, int rcol, double* mtx, int n)
        {
            double* mtx_u_ii, mtx_ii_j;
            double val;
            double* mtx_end = mtx + n * n, result_i, mtx_u_ii_j = null;
            int d = 0;
            for (double* mtx_ii = mtx, mtx_ii_end = mtx + n, result_i_end = result + rcol; mtx_ii < mtx_end; result = result_i_end, result_i_end += rcol, mtx_ii += n + 1, mtx_ii_end += n, d++)
            {
                {
                    val = System.Math.Abs(*(mtx_ii_j = mtx_ii));
                    for (mtx_u_ii = mtx_ii + n, result_i = result + rcol; mtx_u_ii < mtx_end; mtx_u_ii += n, result_i += rcol)
                    {
                        if (val < System.Math.Abs(*mtx_u_ii))
                        {
                            val = System.Math.Abs(*(mtx_ii_j = mtx_u_ii));
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
        static unsafe public double[,] Inverse(double[,] A)
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
                            if (_SolveSqSysEq(pR, n, pA, n)) return Result;
                            else return null;
                    }
                }
                return Result;
            }
            else throw new IndexOutOfRangeException();
        }
    }
}
