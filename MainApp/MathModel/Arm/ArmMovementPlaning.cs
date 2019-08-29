namespace ArmManipulatorArm.MathModel.Arm
{
    using System.Windows.Media.Media3D;

    public static class ArmMovementPlaning
    {
        private static Arm arm;
        
        public static void Init(Arm arm)
        {
            ArmMovementPlaning.arm = arm;
        }

        public static double GetPointError(Point3D p) =>
            MathFunctions.NormaVector(new Point3D(
                p.X - arm.F(arm.N).X,
                p.Y - arm.F(arm.N).Y,
                p.Z - arm.F(arm.N).Z));

        public static double[] LagrangeMethodToThePoint(Point3D p)
        {
            var resultQ = new double[arm.N];

            var f = arm.F(arm.N);
            var d = new Point3D(
                p.X - f.X,
                p.Y - f.Y,
                p.Z - f.Z);

            var C = arm.C;
            var detC = Matrix.Det3D(C);
            var Cx = Matrix.ConcatAsColumn(C, d, 0);
            var detCx = Matrix.Det3D(Cx);
            var Cy = Matrix.ConcatAsColumn(C, d, 1);
            var detCy = Matrix.Det3D(Cy);
            var Cz = Matrix.ConcatAsColumn(C, d, 2);
            var detCz = Matrix.Det3D(Cz);

            var μ = new Point3D(
                detCx / detC,
                detCy / detC,
                detCz / detC);

            for (var i = 0; i < arm.N; i++)
            {
                var dF = arm.GetdF(i);
                resultQ[i] += (μ.X * dF.X + μ.Y * dF.Y + μ.Z * dF.Z) / (2 * arm.A[i, i]);
            }

            return resultQ;
        }
    }
}
