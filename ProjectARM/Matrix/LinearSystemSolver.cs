using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectARM
{
    public static class LinearSystemSolver
    {
        public static Vector3D CramerMethod(double[,] A, Vector3D b)
        {
            Vector3D X = new Vector3D(0, 0, 0);
            double det = A[0, 0] * A[1, 1] - A[0, 1] * A[1, 0];
            if (det != 0)
            {
                double detx1 = b.X * A[1, 1] - A[0, 1] * b.Y;
                X.X = detx1 / det;
                double detx2 = A[0, 0] * b.Y - b.X * A[1, 0];
                X.Y = detx2 / det;
            }
            else return new Vector3D(0, 0, 0);
            return X;
        }

        //public static double[] GaussMethod(Matrix A, Matrix b)
        //{
        //    var x = new double[A.columns];
        //    int i, j, k = 1;
        //    double alpha, sum = 0;
        //    for (j = 0; j < A.columns; j++)
        //    {
        //        if (A[j, j] == 0)
        //        {
        //            i = A.IndexOfMaxElemInRow(j);
        //            A.SwapRows(j, i);
        //            var tmp = b[j, 0];
        //            b[j, 0] = b[i, 0];
        //            b[i, 0] = tmp;
        //        }
        //        if (A[j, j] == 0)
        //        {
        //            Console.WriteLine("No solutions of the system.");
        //            k = 0;
        //            break;
        //        }
        //        for (i = j + 1; i < size; i++)
        //        {
        //            alpha = A[i, j] / A[j, j];
        //            for (k = j; k < size; k++)
        //                A[i, k] -= alpha * A[j, k];
        //            b[i, 0] -= alpha * b[j, 0];
        //        }
        //    }
        //    if (k != 0)
        //    {
        //        for (i = size - 1; i >= 0; i--)
        //        {
        //            sum = 0;
        //            for (j = i + 1; j < A.columns; j++)
        //                sum += (A[i, j] * x[j]);
        //            x[i] = (b[i, 0] - sum) / (A[i, i]);
        //        }
        //    }
        //    return x;
        //}

        //void Check(double** A, double* b, double* x, int size)
        //{
        //    double sum = 0, error = -1;
        //    double* Ab = new double[size];
        //    int f;
        //    cout << "1.Solution x\n2.My x\n";
        //    cin >> f;
        //    if (f == 2)
        //    {
        //        cout << "Input x to test:\n";
        //        for (int i = 0; i < size; i++)
        //        {
        //            cout << "x" << i + 1 << "=";
        //            cin >> x[i];
        //        }
        //    }
        //    for (int i = 0; i < size; i++)
        //    {
        //        sum = 0;
        //        for (int j = 0; j < size; j++)
        //        {
        //            sum += A[i][j] * x[j];
        //        }
        //        Ab[i] = sum - b[i];
        //    }
        //    error = detVect(Ab, size);
        //    if (error == -1) cout << "x solution system Ax=b\n";
        //    else
        //    {
        //        cout.precision(3);
        //        cout << "x can be solution system Ax=b then Error=";
        //        cout.width(7);
        //        cout << scientific;
        //        cout << error;
        //        cout << endl;
        //    }
        //}
    }
}
