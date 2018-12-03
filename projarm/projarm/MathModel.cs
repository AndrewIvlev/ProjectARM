using System;

namespace projarm
{
    public class MathModel
    {
        delegate double delegates(ref double[] q);
        static delegates[] dFxpodqi = {new delegates(dFxpodq1), new delegates(dFxpodq2), new delegates(dFxpodq3), new delegates(dFxpodq4)};
        static delegates[] dFypodqi = {new delegates(dFypodq1), new delegates(dFypodq2), new delegates(dFypodq3), new delegates(dFypodq4)};
        public static double[] len;
        public static double[] angle;
        private static double[] a;
        readonly int N;

        public double MaxL(double[] UnitTypePmaxLen)
        {
            double MaxL = 0;
            for (int i = 0; i < N; i++)             //Вычисление максимально возможной длины
                MaxL += len[i];                     //манипулятора, которая равна сумме длин всех звеньев
            foreach (double d in UnitTypePmaxLen)   //плюс макисмальные длины звеньев типа Р
                MaxL += d;
            return MaxL;
        }

        public static double DegreeToRadian(double angle) => Math.PI * angle / 180.0;
        public static double RadianToDegree(double angle) => angle * (180.0 / Math.PI);

        public MathModel(int _N)
        {
            N = _N;
            len = new double[N];
            angle = new double[N];
            a = new double[N];
            for (int i = 0; i < N; i++)
                a[i] = 1;
        }
        public MathModel(double[] _len, double[] _angle)
        {
            len = new double[N];
            angle = new double[N];
            a = new double[N];

            for (int i = 0; i < N; i++)
            {
                a[i] = 1;
                len[i] = _len[i];
                angle[i] = _angle[i];
            }
        }
        public void ClearLenAndAngle()
        {
            for (int i = 0; i < N; i++)
            {
                len[i] = 0;
                angle[i] = 0;
            }
        }
        public void LagrangeMethod(ref double[] q, dpoint p)
        {
            double diag = dFxpodq1(ref q) * dFypodq1(ref q) + dFxpodq2(ref q) * dFypodq2(ref q) + dFxpodq3(ref q) * dFypodq3(ref q) + dFxpodq4(ref q) * dFypodq4(ref q);
            double[,] A = {
                { Math.Pow(dFxpodq1(ref q), 2) + Math.Pow(dFxpodq2(ref q), 2) + Math.Pow(dFxpodq3(ref q), 2) + Math.Pow(dFxpodq4(ref q), 2), diag },
                { diag, Math.Pow(dFypodq1(ref q), 2) + Math.Pow(dFypodq2(ref q), 2) + Math.Pow(dFypodq3(ref q), 2) + Math.Pow(dFypodq4(ref q), 2) }
               };
            dpoint b = new dpoint(p.x - Fx(q), p.y - Fy(q));
            dpoint μ = CramerMethod(A, b);
            /*dpoint error = SolutionVerification(A, b, μ);
            double er = Math.Sqrt(error.x * error.x + error.y * error.y);
            if (er > 1);*/
            for (int i = 0; i < 4; i++)
                q[i] += MagicFunc(μ, q, a[i], dFxpodqi[i], dFypodqi[i]);
        }
        private double MagicFunc(dpoint μ, double[] q, double a, delegates dFxpodqi, delegates dFypodqi) => (μ.x * dFxpodqi(ref q) + μ.y * dFypodqi(ref q)) / (2 * a);
        public dpoint CramerMethod(double[,] A, dpoint b)
        {
            dpoint X = new dpoint(0, 0);
            double det = A[0, 0] * A[1, 1] - A[0, 1] * A[1, 0];
            if (det != 0)
            {
                double detx1 = b.x * A[1, 1] - A[0, 1] * b.y;
                X.x = detx1 / det;
                double detx2 = A[0, 0] * b.y - b.x * A[1, 0];
                X.y = detx2 / det;
            }
            else return new dpoint(0, 0);
            return X;
        }
        public dpoint SolutionVerification(double[,] A, dpoint b, dpoint X)
        {
            dpoint error;
            error.x = b.x - A[0, 0] * X.x - A[0, 1] * X.y;
            error.y = b.y - A[1, 0] * X.x - A[1, 1] * X.y;
            return error;
        }
        private double Fx(double[] q) => len[0] * Math.Cos(q[0]) + (len[1] + len[2] + q[2]) * Math.Cos(q[0] + q[1]) + len[3] * Math.Cos(q[0] + q[1] + q[3]);
        private double Fy(double[] q) => len[0] * Math.Sin(q[0]) + (len[1] + len[2] + q[2]) * Math.Sin(q[0] + q[1]) + len[3] * Math.Sin(q[0] + q[1] + q[3]);
        private static double dFxpodq1(ref double[] q) => -len[0] * Math.Sin(q[0]) - (len[1] + len[2] + q[2]) * Math.Sin(q[0] + q[1]) - len[3] * Math.Sin(q[0] + q[1] + q[3]);
        private static double dFxpodq2(ref double[] q) => -(len[1] + len[2] + q[2]) * Math.Sin(q[0] + q[1]) - len[3] * Math.Sin(q[0] + q[1] + q[3]);
        private static double dFxpodq3(ref double[] q) => Math.Cos(q[0] + q[1]);
        private static double dFxpodq4(ref double[] q) => -len[3] * Math.Sin(q[0] + q[1] + q[3]);
        private static double dFypodq1(ref double[] q) => len[0] * Math.Cos(q[0]) + (len[1] + len[2] + q[2]) * Math.Cos(q[0] + q[1]) + len[3] * Math.Cos(q[0] + q[1] + q[3]);
        private static double dFypodq2(ref double[] q) => (len[1] + len[2] + q[2]) * Math.Cos(q[0] + q[1]) + len[3] * Math.Cos(q[0] + q[1] + q[3]);
        private static double dFypodq3(ref double[] q) => Math.Sin(q[0] + q[1]);
        private static double dFypodq4(ref double[] q) => len[3] * Math.Cos(q[0] + q[1] + q[3]);
        public double GetPointError(double[] Q, dpoint p) => NormaVectora(new dpoint((p.x - Fx(Q)), (p.y - Fy(Q))));
        public static double NormaVectora(dpoint p) =>  Math.Sqrt(Math.Pow(p.x, 2) + Math.Pow(p.y, 2));
    }
}
