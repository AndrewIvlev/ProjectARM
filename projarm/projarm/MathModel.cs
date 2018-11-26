using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace projarm
{
    public class MathModel
    {
        public delegate double delegates(ref double[] q);
        public static delegates[] dFxpodqi = new delegates[]
                                         {
                                      new delegates(dFxpodq1),
                                      new delegates(dFxpodq2),
                                      new delegates(dFxpodq3),
                                      new delegates(dFxpodq4)
                                         };
        public static delegates[] dFypodqi = new delegates[]
                                         {
                                      new delegates(dFypodq1),
                                      new delegates(dFypodq2),
                                      new delegates(dFypodq3),
                                      new delegates(dFypodq4)
                                         };
        public static double[] len;
        public static double[] angle;
        readonly int N;

        public static double sin(double angle)
        {
            return Math.Sin(0.0174533 * angle);
        }
        public static double cos(double angle)
        {
            return Math.Cos(0.0174533 * angle);
        }
        public MathModel(int _N)
        {
            N = _N;
            len = new double[N];
            angle = new double[N];
        }
        public MathModel(double[] _len, double[] _angle)
        {
            len = new double[N];
            angle = new double[N];

            for (int i = 0; i < N; i++)
            {
                len[i] = _len[i];
                angle[i] = _angle[i];
            }
        }
        public double[] LagrangeMethod(ref double[] q, double[] xy)
        {
            double[] dq = new double[4] {0, 0, 0, 0};
            double[,] A = new double[,] {
                { Math.Pow(dFxpodq1(ref q), 2) + Math.Pow(dFxpodq2(ref q), 2) + Math.Pow(dFxpodq3(ref q), 2) + Math.Pow(dFxpodq4(ref q), 2),
                dFxpodq1(ref q) * dFypodq1(ref q) + dFxpodq2(ref q) * dFypodq2(ref q) + dFxpodq3(ref q) * dFypodq3(ref q) + dFxpodq4(ref q) * dFypodq4(ref q) },
                { dFxpodq1(ref q) * dFypodq1(ref q) + dFxpodq2(ref q) * dFypodq2(ref q) + dFxpodq3(ref q) * dFypodq3(ref q) + dFxpodq4(ref q) * dFypodq4(ref q),
                Math.Pow(dFypodq1(ref q), 2) + Math.Pow(dFypodq2(ref q), 2) + Math.Pow(dFypodq3(ref q), 2) + Math.Pow(dFypodq4(ref q), 2) }
               };
            double[] b = new double[2] {xy[0] - Fx(q), xy[1] - Fy(q)};
            double[] μ = CramerMethod(A, b);
            ;
            double[] error = SolutionVerification(A, b, μ);
            //if (Math.Sqrt(error[0] * error[0] + error[1] * error[1]) > 1) return null;
            //if (μ[0] + μ[1] == 0) return dq;
            for (int i = 0; i < 4; i++)
                dq[i] = MagicFunc(μ, q, dFxpodqi[i], dFypodqi[i]);
            return dq;
        }
        public double MagicFunc(double[] μ, double[] q, delegates dFxpoqi, delegates dFypodqi)
        {
            return (μ[0] * dFxpoqi(ref q) + μ[1] * dFypodqi(ref q)) / 2;
        }
        public double[] CramerMethod(double[,] A, double[] b)
        {
            double[] x = new double[2] {0, 0};
            double det = A[0, 0] * A[1, 1] - A[0, 1] * A[1, 0];
            if (det != 0)
            {
                double detx1 = b[0] * A[1, 1] - A[0, 1] * b[1];
                x[0] = detx1 / det;
                double detx2 = A[0, 0] * b[1] - b[0] * A[1, 0];
                x[1] = detx2 / det;
            }
            else return null;
            return x;
        }
        public double[] SolutionVerification(double[,] A, double[] b, double[] x)
        {
            double[] error = new double[2];
            error[0] = b[0] - A[0, 0] * x[0] - A[0, 1] * x[1];
            error[1] = b[1] - A[1, 0] * x[0] - A[1, 1] * x[1];
            return error;
        }
        public double Fx(double[] q)
        {
            return len[0] * cos(q[0]) + (len[1] + len[2] + q[2]) * cos(q[0] + q[1]) + len[3] * cos(q[0] + q[1] + q[3]);
        }
        public double Fy(double[] q)
        {
            return len[0] * sin(q[0]) + (len[1] + len[2] + q[2]) * sin(q[0] + q[1]) + len[3] * sin(q[0] + q[1] + q[3]);
        }
        public static double dFxpodq1(ref double[] q)
        {
            return -len[0] * sin(q[0]) - (len[1] + len[2] + q[2]) * sin(q[0] + q[1]) - len[3] * sin(q[0] + q[1] + q[3]);
        }
        public static double dFxpodq2(ref double[] q)
        {
            return -(len[1] + len[2] + q[2]) * sin(q[0] + q[1]) - len[3] * sin(q[0] + q[1] + q[3]);
        }
        public static double dFxpodq3(ref double[] q)
        {
            return cos(q[0] + q[1]);
        }
        public static double dFxpodq4(ref double[] q)
        {
            return -len[3] * sin(q[0] + q[1] + q[3]);
        }
        public static double dFypodq1(ref double[] q)
        {
            return len[0] * cos(q[0]) + (len[1] + len[2] + q[2]) * cos(q[0] + q[1]) + len[3] * cos(q[0] + q[1] + q[3]);
        }
        public static double dFypodq2(ref double[] q)
        {
            return (len[1] + len[2] + q[2]) * cos(q[0] + q[1]) + len[3] * cos(q[0] + q[1] + q[3]);
        }
        public static double dFypodq3(ref double[] q)
        {
            return sin(q[0] + q[1]);
        }
        public static double dFypodq4(ref double[] q)
        {
            return len[3] * cos(q[0] + q[1] + q[3]);
        }
        public Point GetPointError(double[] Q, double[] dq, double[] xy)
        {
            for (int i = 0; i < Q.Length; i++)
                Q[i] += dq[i];
            return new Point((int)(xy[0] - Fx(Q)), (int)(xy[1] - Fy(Q))); 
        }
    }
}
