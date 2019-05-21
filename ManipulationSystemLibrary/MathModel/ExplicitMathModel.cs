using System;
using System.Windows.Media.Media3D;

namespace ManipulationSystemLibrary
{
    public class ExplicitMathModel : MathModel
    {
        delegate double function(double[] q);
        static readonly function[] dFxpodqi = { dFxpodq1, dFxpodq2, dFxpodq3, dFxpodq4 };
        static readonly function[] dFypodqi = { dFypodq1, dFypodq2, dFypodq3, dFypodq4 };
        private static double[] Len;

        public ExplicitMathModel() { }

        public ExplicitMathModel(int n) : base(n)
        {
            Len = new double[n];
            for (int i = 0; i < n; i++)
                Len[i] = units[i].len;
        }
        public ExplicitMathModel(int n, unit[] units) : base(n, units)
        {
            Len = new double[n];
            for (int i = 0; i < n; i++)
                Len[i] = units[i].len;
        }

        public ExplicitMathModel(MathModel model) : base(model)
        {
            Len = new double[n];
            for (int i = 0; i < n; i++)
                Len[i] = units[i].len;
        }

        public override void LagrangeMethodToThePoint(Point3D p)
        {
            double diag = dFxpodq1(q) * dFypodq1(q) + dFxpodq2(q) * dFypodq2(q) + 
                dFxpodq3(q) * dFypodq3(q) + dFxpodq4(q) * dFypodq4(q);

            double[,] D = {
                { Math.Pow(dFxpodq1(q), 2) + Math.Pow(dFxpodq2(q), 2) + Math.Pow(dFxpodq3(q), 2) + Math.Pow(dFxpodq4(q), 2), diag },
                { diag, Math.Pow(dFypodq1(q), 2) + Math.Pow(dFypodq2(q), 2) + Math.Pow(dFypodq3(q), 2) + Math.Pow(dFypodq4(q), 2) }
               };

            Point3D d = new Point3D(p.X - Fx(q), p.Y - Fy(q), 0);
            Point3D μ = LinearSystemSolver.CramerMethod(D, d);

            for (int i = 0; i < 4; i++)
                q[i] += MagicFunc(μ, q, D[i, i], dFxpodqi[i], dFypodqi[i]);
        }

        public override double GetPointError(Point3D p) => NormaVectora(new Point3D(p.X - Fx(q), p.Y - Fy(q), 0));
        
        public Point3D SolutionVerification(Matrix A, Point3D b, Point3D x) =>
            new Point3D(
                b.X - A[0, 0] * x.X - A[0, 1] * x.Y,
                b.Y - A[1, 0] * x.X - A[1, 1] * x.Y,
                0);

        private static double MagicFunc(Point3D μ, double[] q, double a, function dFxpodqi, function dFypodqi) => (μ.X * dFxpodqi(q) + μ.Y * dFypodqi(q)) / (2 * a);

        private static double Fx(double[] q) => Len[0] * Math.Cos(q[0]) + (Len[1] + Len[2] + q[2]) * Math.Cos(q[0] + q[1]) + Len[3] * Math.Cos(q[0] + q[1] + q[3]);

        private static double Fy(double[] q) => Len[0] * Math.Sin(q[0]) + (Len[1] + Len[2] + q[2]) * Math.Sin(q[0] + q[1]) + Len[3] * Math.Sin(q[0] + q[1] + q[3]);

        private static double dFxpodq1(double[] q) => -Len[0] * Math.Sin(q[0]) - (Len[1] + Len[2] + q[2]) * Math.Sin(q[0] + q[1]) - Len[3] * Math.Sin(q[0] + q[1] + q[3]);

        private static double dFxpodq2(double[] q) => -(Len[1] + Len[2] + q[2]) * Math.Sin(q[0] + q[1]) - Len[3] * Math.Sin(q[0] + q[1] + q[3]);

        private static double dFxpodq3(double[] q) => Math.Cos(q[0] + q[1]);

        private static double dFxpodq4(double[] q) => -Len[3] * Math.Sin(q[0] + q[1] + q[3]);

        private static double dFypodq1(double[] q) => Len[0] * Math.Cos(q[0]) + (Len[1] + Len[2] + q[2]) * Math.Cos(q[0] + q[1]) + Len[3] * Math.Cos(q[0] + q[1] + q[3]);

        private static double dFypodq2(double[] q) => (Len[1] + Len[2] + q[2]) * Math.Cos(q[0] + q[1]) + Len[3] * Math.Cos(q[0] + q[1] + q[3]);

        private static double dFypodq3(double[] q) => Math.Sin(q[0] + q[1]);

        private static double dFypodq4(double[] q) => Len[3] * Math.Cos(q[0] + q[1] + q[3]);
    }
}
