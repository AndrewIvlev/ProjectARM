using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectARM
{
    public class BlockMatrix
    {
        private double[,] M;

        public BlockMatrix()
        {
            M = new double[3, 4] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 } };
        }

        //Умножение блочных матриц размера 3х
        public static BlockMatrix operator *(BlockMatrix A, BlockMatrix B)
        {
            BlockMatrix AB = new BlockMatrix();
            AB.M[0, 0] = A.M[0, 0] * B.M[0, 0] + A.M[0, 1] * B.M[1, 0];
            AB.M[0, 1] = A.M[0, 0] * B.M[0, 1] + A.M[0, 1] * B.M[1, 1];
            AB.M[0, 2] = A.M[0, 0] * B.M[0, 2] + A.M[0, 1] * B.M[1, 2] + A.M[0, 2];
            AB.M[1, 0] = A.M[1, 0] * B.M[0, 0] + A.M[1, 1] * B.M[1, 0];
            AB.M[1, 1] = A.M[1, 0] * B.M[0, 1] + A.M[1, 1] * B.M[1, 1];
            AB.M[1, 2] = A.M[1, 0] * B.M[0, 2] + A.M[1, 1] * B.M[1, 2] + A.M[1, 2];
            return AB;
        }
    }
}
