using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManipulationSystemLibrary
{
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
            Type = 'T';
            Q = 0;
            B = new BlockMatrix();
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
    }
}
