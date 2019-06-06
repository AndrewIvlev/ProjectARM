using System;
using System.Windows.Media.Media3D;
using ManipulationSystemLibrary.MathModel;
using ManipulationSystemLibrary.Matrix;

namespace ManipulationSystemLibrary
{
    public class ExplicitMathModel : MathModel.MathModel
    {
        delegate double function(double[] q);
        static readonly function[] dFxpodqi = { dFxpodq1, dFxpodq2, dFxpodq3, dFxpodq4 };
        static readonly function[] dFypodqi = { dFypodq1, dFypodq2, dFypodq3, dFypodq4 };
        private static double[] Len;

        public ExplicitMathModel() { }

        public ExplicitMathModel(int n) : base(n)
        {
            Len = new double[n];
            for (var i = 0; i < n; i++)
                Len[i] = GetUnitLen(i);
        }
        public ExplicitMathModel(int n, Unit[] units) : base(n, units)
        {
            Len = new double[n];
            for (var i = 0; i < n; i++)
                Len[i] = GetUnitLen(i);
        }

        public ExplicitMathModel(MathModel.MathModel model) : base(model)
        {
            Len = new double[N];
            for (var i = 0; i < N; i++)
                Len[i] = GetUnitLen(i);
        }

        public override void LagrangeMethodToThePoint(Point3D p)
        {
            var diagonalElement = dFxpodq1(Q) * dFypodq1(Q)
                                + dFxpodq2(Q) * dFypodq2(Q)
                                + dFxpodq3(Q) * dFypodq3(Q)
                                + dFxpodq4(Q) * dFypodq4(Q);

            double[,] dFs = {
                { Math.Pow(dFxpodq1(Q), 2) + Math.Pow(dFxpodq2(Q), 2) + Math.Pow(dFxpodq3(Q), 2) + Math.Pow(dFxpodq4(Q), 2), diagonalElement },
                { diagonalElement, Math.Pow(dFypodq1(Q), 2) + Math.Pow(dFypodq2(Q), 2) + Math.Pow(dFypodq3(Q), 2) + Math.Pow(dFypodq4(Q), 2) }
               };

            var d = new Point3D(p.X - Fx(Q), p.Y - Fy(Q), 0);
            var μ = LinearSystemSolver.CramersRule(dFs, d);

            for (var i = 0; i < 4; i++)
                Q[i] += MagicFunc(μ, Q, dFs[i, i], dFxpodqi[i], dFypodqi[i]);
        }

        public override double GetPointError(Point3D p) => NormaVectora(new Point3D(p.X - Fx(Q), p.Y - Fy(Q), 0));
        
        public Point3D SolutionVerification(Matrix.Matrix a, Point3D b, Point3D x) =>
            new Point3D(
                b.X - a[0, 0] * x.X - a[0, 1] * x.Y,
                b.Y - a[1, 0] * x.X - a[1, 1] * x.Y,
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
