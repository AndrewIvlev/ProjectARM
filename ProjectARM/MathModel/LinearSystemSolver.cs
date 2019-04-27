using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectARM
{
    public static class LinearSystemSolver
    {
        public static DPoint CramerMethod(double[,] A, DPoint b)
        {
            DPoint X = new DPoint(0, 0, 0);
            double det = A[0, 0] * A[1, 1] - A[0, 1] * A[1, 0];
            if (det != 0)
            {
                double detx1 = b.X * A[1, 1] - A[0, 1] * b.Y;
                X.X = detx1 / det;
                double detx2 = A[0, 0] * b.Y - b.X * A[1, 0];
                X.Y = detx2 / det;
            }
            else return new DPoint(0, 0, 0);
            return X;
        }

        //public static double[] GausMethod(double[,] A, double[] b)
        //{
        //    var x = new double[]
        //    int i, j, k = 1;
        //    double alpha, sum = 0;

        //    for (j = 0; j < size; j++)
        //    {
        //        if (A[j][j] == 0)
        //        {
        //            int i = MaxElemInStr(A, size, j);
        //            SwapString(A, size, j, i);
        //            int tmp = b[j];
        //            b[j] = b[i];
        //            b[i] = tmp;
                    
        //        }
        //        if (A[j][j] == 0) { cout << "No solutions of the system.\n"; k = 0; break; }
        //        for (i = j + 1; i < size; i++)
        //        {
        //            alpha = A[i][j] / A[j][j];
        //            for (k = j; k < size; k++)
        //                A[i][k] -= alpha * A[j][k];
        //            b[i] -= alpha * b[j];
        //        }
        //    }
        //    if (k != 0)
        //    {
        //        for (i = size - 1; i >= 0; i--)
        //        {
        //            sum = 0;
        //            for (j = i + 1; j < size; j++)
        //                sum += (A[i][j] * x[j]);
        //            x[i] = (b[i] - sum) / (A[i][i]);
        //        }
        //        //output x
        //        for (i = 0; i < size; i++)
        //        {
        //            cout << "x" << i + 1 << "=";
        //            cout << x[i];
        //        }
        //    }
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
        //            cout << endl;
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
