namespace ArmManipulatorArm.MathModel.Arm
{
    using System;

    using ArmManipulatorArm.MathModel.Matrix;

    public class Unit
    {
        /// <summary>
        ///  R - Revolute joint
        ///  P - Prismatic joint
        /// </summary>
        public char Type;

        /// <summary>
        /// Generalized coordinates vector
        /// </summary>
        public double Q;

        /// <summary>
        /// Matrix orientation and position of unit
        /// </summary>
        public BlockMatrix B;

        public Unit()
        {
            this.Type = 'T';
            this.Q = 0;
            this.B = new BlockMatrix();
        }

        public static bool operator ==(Unit a, Unit b)
        {
            if (a.Type != b.Type)
            {
                Console.WriteLine("Type of units is not equal.");
                return false;
            }

            if (a.Q != b.Q)
            {
                Console.WriteLine("Generalized coordinates are not equal.");
                return false;
            }

            if (a.B != b.B)
            {
                Console.WriteLine("Orientation matrices are not equal.");
                return false;
            }

            return true;
        }

        public static bool operator !=(Unit a, Unit b)
        {
            return !(a == b);
        }

        public double GetLength() => B.ColumnAsVector3D(3).Length;
    }
}
