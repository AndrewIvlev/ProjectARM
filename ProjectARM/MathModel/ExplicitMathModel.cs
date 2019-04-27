using System;

namespace ProjectARM
{
    public class ExplicitMathModel : MathModel
    {
        delegate double function(double[] q);
        static function[] dFxpodqi = {new function(dFxpodq1), new function(dFxpodq2), new function(dFxpodq3), new function(dFxpodq4)};
        static function[] dFypodqi = {new function(dFypodq1), new function(dFypodq2), new function(dFypodq3), new function(dFypodq4)};
        private static double[] len;

        public ExplicitMathModel(int n) : base(n)
        {
            for (int i = 0; i < n; i++)
                len[i] = units[i].len;
        }
        public ExplicitMathModel(int n, unit[] units) : base(n, units)
        {
            for (int i = 0; i < n; i++)
                len[i] = units[i].len;
        }

        public override void LagrangeMethodToThePoint(DPoint p)
        {
            double diag = dFxpodq1(q) * dFypodq1(q) + dFxpodq2(q) * dFypodq2(q) + dFxpodq3(q) * dFypodq3(q) + dFxpodq4(q) * dFypodq4(q);
            double[,] A = {
                { Math.Pow(dFxpodq1(q), 2) + Math.Pow(dFxpodq2(q), 2) + Math.Pow(dFxpodq3(q), 2) + Math.Pow(dFxpodq4(q), 2), diag },
                { diag, Math.Pow(dFypodq1(q), 2) + Math.Pow(dFypodq2(q), 2) + Math.Pow(dFypodq3(q), 2) + Math.Pow(dFypodq4(q), 2) }
               };
            
            DPoint b = new DPoint(p.X - Fx(q), p.Y - Fy(q), 0);
            DPoint μ = LinearSystemSolver.CramerMethod(A, b);

            for (int i = 0; i < 4; i++)
                q[i] += MagicFunc(μ, q, A[i, i], dFxpodqi[i], dFypodqi[i]);
        }

        public override double GetPointError(DPoint p) => NormaVectora(new DPoint((p.X - Fx(q)), (p.Y - Fy(q)), 0));

        public static double NormaVectora(DPoint p) => Math.Sqrt(Math.Pow(p.X, 2) + Math.Pow(p.Y, 2));
        
        public DPoint SolutionVerification(double[,] A, DPoint b, DPoint X) =>
            new DPoint(
                b.X - A[0, 0] * X.X - A[0, 1] * X.Y,
                b.Y - A[1, 0] * X.X - A[1, 1] * X.Y,
                0);

        private static double MagicFunc(DPoint μ, double[] q, double a, function dFxpodqi, function dFypodqi) => (μ.X * dFxpodqi(q) + μ.Y * dFypodqi(q)) / (2 * a);

        private static double Fx(double[] q) => len[0] * Math.Cos(q[0]) + (len[1] + len[2] + q[2]) * Math.Cos(q[0] + q[1]) + len[3] * Math.Cos(q[0] + q[1] + q[3]);

        private static double Fy(double[] q) => len[0] * Math.Sin(q[0]) + (len[1] + len[2] + q[2]) * Math.Sin(q[0] + q[1]) + len[3] * Math.Sin(q[0] + q[1] + q[3]);

        private static double dFxpodq1(double[] q) => -len[0] * Math.Sin(q[0]) - (len[1] + len[2] + q[2]) * Math.Sin(q[0] + q[1]) - len[3] * Math.Sin(q[0] + q[1] + q[3]);

        private static double dFxpodq2(double[] q) => -(len[1] + len[2] + q[2]) * Math.Sin(q[0] + q[1]) - len[3] * Math.Sin(q[0] + q[1] + q[3]);

        private static double dFxpodq3(double[] q) => Math.Cos(q[0] + q[1]);

        private static double dFxpodq4(double[] q) => -len[3] * Math.Sin(q[0] + q[1] + q[3]);

        private static double dFypodq1(double[] q) => len[0] * Math.Cos(q[0]) + (len[1] + len[2] + q[2]) * Math.Cos(q[0] + q[1]) + len[3] * Math.Cos(q[0] + q[1] + q[3]);

        private static double dFypodq2(double[] q) => (len[1] + len[2] + q[2]) * Math.Cos(q[0] + q[1]) + len[3] * Math.Cos(q[0] + q[1] + q[3]);

        private static double dFypodq3(double[] q) => Math.Sin(q[0] + q[1]);

        private static double dFypodq4(double[] q) => len[3] * Math.Cos(q[0] + q[1] + q[3]);
    }
}
