using System;
using System.Windows.Media.Media3D;
using ManipulationSystemLibrary.Matrix;

namespace ManipulationSystemLibrary.MathModel
{
    public class ExplicitMathModel : MathModel
    {
        private static Point3D[] _dFdq;
        private static double Fx;
        private static double Fy;
        private static double[] _len;
        
        public ExplicitMathModel(int n) : base(n)
        {
            _len = new double[n];
            for (var i = 0; i < n; i++)
                _len[i] = GetUnitLen(i);
            _dFdq = new Point3D[n];
        }

        public override void CalculationMetaData()
        {
            _dFdq[0].X = -_len[0] * Math.Sin(Units[0].Q) - (_len[1] + _len[2] + Units[2].Q) * Math.Sin(Units[0].Q + Units[1].Q) - _len[3] * Math.Sin(Units[0].Q + Units[1].Q + Units[3].Q);
            _dFdq[1].X = -(_len[1] + _len[2] + Units[2].Q) * Math.Sin(Units[0].Q + Units[1].Q) - _len[3] * Math.Sin(Units[0].Q + Units[1].Q + Units[3].Q);
            _dFdq[2].X = Math.Cos(Units[0].Q + Units[1].Q);
            _dFdq[3].X = -_len[3] * Math.Sin(Units[0].Q + Units[1].Q + Units[3].Q);
            _dFdq[0].Y = _len[0] * Math.Cos(Units[0].Q) + (_len[1] + _len[2] + Units[2].Q) * Math.Cos(Units[0].Q + Units[1].Q) + _len[3] * Math.Cos(Units[0].Q + Units[1].Q + Units[3].Q);
            _dFdq[1].Y = (_len[1] + _len[2] + Units[2].Q) * Math.Cos(Units[0].Q + Units[1].Q) + _len[3] * Math.Cos(Units[0].Q + Units[1].Q + Units[3].Q);
            _dFdq[2].Y = Math.Sin(Units[0].Q + Units[1].Q);
            _dFdq[3].Y = _len[3] * Math.Cos(Units[0].Q + Units[1].Q + Units[3].Q);


            Fx = _len[0] * Math.Cos(Units[0].Q) + (_len[1] + _len[2] + Units[2].Q) * Math.Cos(Units[0].Q + Units[1].Q) + _len[3] * Math.Cos(Units[0].Q + Units[1].Q + Units[3].Q);
            Fy = _len[0] * Math.Sin(Units[0].Q) + (_len[1] + _len[2] + Units[2].Q) * Math.Sin(Units[0].Q + Units[1].Q) + _len[3] * Math.Sin(Units[0].Q + Units[1].Q + Units[3].Q);
        }

        public override void LagrangeMethodToThePoint(Point3D p)
        {
            CalculationMetaData();

            var diagonalElement = _dFdq[0].X * _dFdq[0].Y
                                + _dFdq[1].X * _dFdq[1].Y
                                + _dFdq[2].X * _dFdq[2].Y
                                + _dFdq[3].X * _dFdq[3].Y;
            double[,] dFs = {
                { Math.Pow(_dFdq[0].X, 2) + Math.Pow(_dFdq[1].X, 2) + Math.Pow(_dFdq[2].X, 2) + Math.Pow(_dFdq[3].X, 2), diagonalElement },
                { diagonalElement, Math.Pow(_dFdq[0].Y, 2) + Math.Pow(_dFdq[1].Y, 2) + Math.Pow(_dFdq[2].Y, 2) + Math.Pow(_dFdq[3].Y, 2) }
               };

            var d = new Point3D(p.X - Fx, p.Y - Fy, 0);
            var μ = LinearSystemSolver.CramersRule(dFs, d);

            for (var i = 0; i < N; i++)
                Units[i].Q += MagicFunc(μ, dFs[i, i], _dFdq[i].X, _dFdq[i].Y);
        }

        private static double MagicFunc(Point3D μ, double a, double dFxPodQi, double dFyPodQi) => (μ.X * dFxPodQi + μ.Y * dFyPodQi) / (2 * a);

        public override double GetPointError(Point3D p) => NormaVector(new Point3D(p.X - Fx, p.Y - Fy, 0));
        
        public Point3D SolutionVerification(Matrix.Matrix a, Point3D b, Point3D x) =>
            new Point3D(
                b.X - a[0, 0] * x.X - a[0, 1] * x.Y,
                b.Y - a[1, 0] * x.X - a[1, 1] * x.Y,
                0);
    }
}
