using System;

namespace ProjectARM
{
    public class ExplicitMathModel : MathModel
    {
        public double[] q;
        delegate double function(double[] q);
        static function[] dFxpodqi = {new function(dFxpodq1), new function(dFxpodq2), new function(dFxpodq3), new function(dFxpodq4)};
        static function[] dFypodqi = {new function(dFypodq1), new function(dFypodq2), new function(dFypodq3), new function(dFypodq4)};

        public ExplicitMathModel(int _N)
        {
            N = _N;
            len = new double[N];
            angle = new double[N];
            a = new double[N];
            for (int i = 0; i < N; i++)
                a[i] = 1;
        }
        public ExplicitMathModel(double[] _len, double[] _angle)
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

        public override double MaxL(double[] UnitTypePmaxLen)
        {
            double MaxL = 0;
            for (int i = 0; i < N; i++)             //Вычисление максимально возможной длины
                MaxL += len[i];                     //манипулятора, которая равна сумме длин всех звеньев
            foreach (double d in UnitTypePmaxLen)   //плюс макисмальные длины звеньев типа Р
                MaxL += d;
            return MaxL;
        }

        public override void LagrangeMethodToThePoint(Dpoint p)
        {
            double diag = dFxpodq1(q) * dFypodq1(q) + dFxpodq2(q) * dFypodq2(q) + dFxpodq3(q) * dFypodq3(q) + dFxpodq4(q) * dFypodq4(q);
            double[,] A = {
                { Math.Pow(dFxpodq1(q), 2) + Math.Pow(dFxpodq2(q), 2) + Math.Pow(dFxpodq3(q), 2) + Math.Pow(dFxpodq4(q), 2), diag },
                { diag, Math.Pow(dFypodq1(q), 2) + Math.Pow(dFypodq2(q), 2) + Math.Pow(dFypodq3(q), 2) + Math.Pow(dFypodq4(q), 2) }
               };
            double F = Fx(q);
            Dpoint b = new Dpoint(p.x - Fx(q), p.y - Fy(q));
            Dpoint μ = CramerMethod(A, b);
            
            /*dpoint error = SolutionVerification(A, b, μ); double er = Math.Sqrt(error.x * error.x + error.y * error.y);*/

            for (int i = 0; i < 4; i++)
                q[i] += MagicFunc(μ, q, a[i], dFxpodqi[i], dFypodqi[i]);
        }
        private double MagicFunc(Dpoint μ, double[] q, double a, function dFxpodqi, function dFypodqi) => (μ.x * dFxpodqi(q) + μ.y * dFypodqi(q)) / (2 * a);
        public Dpoint CramerMethod(double[,] A, Dpoint b)
        {
            Dpoint X = new Dpoint(0, 0);
            double det = A[0, 0] * A[1, 1] - A[0, 1] * A[1, 0];
            if (det != 0)
            {
                double detx1 = b.x * A[1, 1] - A[0, 1] * b.y;
                X.x = detx1 / det;
                double detx2 = A[0, 0] * b.y - b.x * A[1, 0];
                X.y = detx2 / det;
            }
            else return new Dpoint(0, 0);
            return X;
        }
        public Dpoint SolutionVerification(double[,] A, Dpoint b, Dpoint X)
        {
            Dpoint error;
            error.x = b.x - A[0, 0] * X.x - A[0, 1] * X.y;
            error.y = b.y - A[1, 0] * X.x - A[1, 1] * X.y;
            return error;
        }
        public static double Fx(double[] q) => len[0] * Math.Cos(q[0]) + (len[1] + len[2] + q[2]) * Math.Cos(q[0] + q[1]) + len[3] * Math.Cos(q[0] + q[1] + q[3]);
        public static double Fy(double[] q) => len[0] * Math.Sin(q[0]) + (len[1] + len[2] + q[2]) * Math.Sin(q[0] + q[1]) + len[3] * Math.Sin(q[0] + q[1] + q[3]);
        private static double dFxpodq1(double[] q) => -len[0] * Math.Sin(q[0]) - (len[1] + len[2] + q[2]) * Math.Sin(q[0] + q[1]) - len[3] * Math.Sin(q[0] + q[1] + q[3]);
        private static double dFxpodq2(double[] q) => -(len[1] + len[2] + q[2]) * Math.Sin(q[0] + q[1]) - len[3] * Math.Sin(q[0] + q[1] + q[3]);
        private static double dFxpodq3(double[] q) => Math.Cos(q[0] + q[1]);
        private static double dFxpodq4(double[] q) => -len[3] * Math.Sin(q[0] + q[1] + q[3]);
        private static double dFypodq1(double[] q) => len[0] * Math.Cos(q[0]) + (len[1] + len[2] + q[2]) * Math.Cos(q[0] + q[1]) + len[3] * Math.Cos(q[0] + q[1] + q[3]);
        private static double dFypodq2(double[] q) => (len[1] + len[2] + q[2]) * Math.Cos(q[0] + q[1]) + len[3] * Math.Cos(q[0] + q[1] + q[3]);
        private static double dFypodq3(double[] q) => Math.Sin(q[0] + q[1]);
        private static double dFypodq4(double[] q) => len[3] * Math.Cos(q[0] + q[1] + q[3]);
        public override double GetPointError(Dpoint p) => NormaVectora(new Dpoint((p.x - Fx(q)), (p.y - Fy(q))));
        public static double NormaVectora(Dpoint p) =>  Math.Sqrt(Math.Pow(p.x, 2) + Math.Pow(p.y, 2));
    }
}
