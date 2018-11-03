using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projarm
{
    class GausMethod
    {
        /*
        void SwapElem(double a, double b)
        {
            double tmp = a;
            a = b;
            b = tmp;
        }
        void SwapString(double** M, int size, int i, int j)
        {
            double* tmp = M[i];
            M[i] = M[j];
            M[j] = tmp;
        }
        double detVect(double* v, int size)
        {
            double res = 0;
            for (int i = 0; i < size; i++)
                res += v[i] * v[i];
            return sqrt(res);
        }
        int MaxElemInStr(double** M, int size, int j)
        {
            int pos = j;
            double max = fabs(M[j][j]);

            for (int i = j + 1; i < size; i++)
            {
                if (max < fabs(M[i][j]))
                {
                    max = fabs(M[i][j]);
                    pos = i;
                }
            }
            return pos;
        }
        void GausMethod(double** A, double* b, double* x, int size)
        {
            int i, j, k = 1;
            double alpha, sum = 0;

            for (j = 0; j < size; j++)
            {
                if (A[j][j] == 0)
                {
                    int i = MaxElemInStr(A, size, j);
                    SwapString(A, size, j, i);
                    int tmp = b[j];
                    b[j] = b[i];
                    b[i] = tmp;

                    if (i != j)
                    {
                        AxbOut(A, b, size);
                    }
                }
                if (A[j][j] == 0) { cout << "No solutions of the system.\n"; k = 0; break; }
                for (i = j + 1; i < size; i++)
                {
                    alpha = A[i][j] / A[j][j];
                    for (k = j; k < size; k++)
                        A[i][k] -= alpha * A[j][k];
                    b[i] -= alpha * b[j];
                }
                AxbOut(A, b, size);
            }
            if (k)
            {
                for (i = size - 1; i >= 0; i--)
                {
                    sum = 0;
                    for (j = i + 1; j < size; j++)
                        sum += (A[i][j] * x[j]);
                    x[i] = (b[i] - sum) / (A[i][i]);
                }
                //output x
                cout.precision(4);
                for (i = 0; i < size; i++)
                {
                    cout << "x" << i + 1 << "=";
                    cout << fixed;
                    cout.width(7);
                    cout << x[i];
                    cout << "  ";
                }
                cout << endl;
            }
        }
        void Check(double** A, double* b, double* x, int size)
        {
            double sum = 0, error = -1;
            double* Ab = new double[size];

            int f;
            cout << "1.Solution x\n2.My x\n";
            cin >> f;
            if (f == 2)
            {
                cout << "Input x to test:\n";
                for (int i = 0; i < size; i++)
                {
                    cout << "x" << i + 1 << "=";
                    cin >> x[i];
                    cout << endl;
                }
            }

            for (int i = 0; i < size; i++)
            {
                sum = 0;
                for (int j = 0; j < size; j++)
                {
                    sum += A[i][j] * x[j];
                }
                Ab[i] = sum - b[i];
            }
            error = detVect(Ab, size);
            if (error == -1) cout << "x solution system Ax=b\n";
            else
            {
                cout.precision(3);
                cout << "x can be solution system Ax=b then Error=";
                cout.width(7);
                cout << scientific;
                cout << error;
                cout << endl;
            }
        }
    */
    }
}
