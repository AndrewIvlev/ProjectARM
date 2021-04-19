namespace ArmManipulatorArm.MathModel.Arm
{
    using System;

    using ArmManipulatorArm.MathModel.Matrix;
    using Newtonsoft.Json;

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
        public double qMin;
        public double qMax;
        [JsonIgnore] public bool v; // false if Q is <= qMin or >= qMax

        /// <summary>
        /// Matrix orientation and position of unit
        /// </summary>
        public Matrix B;

        public Unit()
        {
            this.Type = 'T';
            this.Q = 0;
            this.qMin = double.MinValue;
            this.qMax = double.MaxValue;
            this.v = true;
            this.B = new Matrix(4, 4);
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

            if (a.qMin != b.qMin)
            {
                Console.WriteLine("qMin values are not equal.");
                return false;
            }

            if (a.qMax != b.qMax)
            {
                Console.WriteLine("qMax values are not equal.");
                return false;
            }

            if (a.v != b.v)
            {
                Console.WriteLine("V values are not equal.");
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
