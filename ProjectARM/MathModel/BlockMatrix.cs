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
            M = new double[,] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 } };
        }

        public void SetByIJ(int i, int j, double value)
        {
            M[i, j] = value;
        }

        public DPoint GetLastColumn()
        {
            return new DPoint(M[0, 3], M[1, 3], M[2, 3]);
        }
        //Умножение блочных матриц размера 3х4
        public static BlockMatrix operator *(BlockMatrix A, BlockMatrix B)
        {
            BlockMatrix AB = new BlockMatrix();
            AB.M[0, 0] = A.M[0, 0] * B.M[0, 0] + A.M[0, 1] * B.M[1, 0] + A.M[0, 2] * B.M[2, 0];
            AB.M[0, 1] = A.M[0, 0] * B.M[0, 1] + A.M[0, 1] * B.M[1, 1] + A.M[0, 2] * B.M[2, 1];
            AB.M[0, 2] = A.M[0, 0] * B.M[0, 2] + A.M[0, 1] * B.M[1, 2] + A.M[0, 2] * B.M[2, 2];
            AB.M[0, 3] = A.M[0, 0] * B.M[0, 3] + A.M[0, 1] * B.M[1, 3] + A.M[0, 2] * B.M[2, 3] + A.M[0, 3];
            AB.M[1, 0] = A.M[1, 0] * B.M[0, 0] + A.M[1, 1] * B.M[1, 0] + A.M[1, 2] * B.M[2, 0];
            AB.M[1, 1] = A.M[1, 0] * B.M[0, 1] + A.M[1, 1] * B.M[1, 1] + A.M[1, 2] * B.M[2, 1];
            AB.M[1, 2] = A.M[1, 0] * B.M[0, 2] + A.M[1, 1] * B.M[1, 2] + A.M[1, 2] * B.M[2, 2];
            AB.M[1, 3] = A.M[1, 0] * B.M[0, 3] + A.M[1, 1] * B.M[1, 3] + A.M[1, 2] * B.M[2, 3] + A.M[1, 3];
            AB.M[2, 0] = A.M[2, 0] * B.M[0, 0] + A.M[2, 1] * B.M[1, 0] + A.M[2, 2] * B.M[2, 0];
            AB.M[2, 1] = A.M[2, 0] * B.M[0, 1] + A.M[2, 1] * B.M[1, 1] + A.M[2, 2] * B.M[2, 1];
            AB.M[2, 2] = A.M[2, 0] * B.M[0, 2] + A.M[2, 1] * B.M[1, 2] + A.M[2, 2] * B.M[2, 2];
            AB.M[2, 3] = A.M[2, 0] * B.M[0, 3] + A.M[2, 1] * B.M[1, 3] + A.M[2, 2] * B.M[2, 3] + A.M[2, 3];
            return AB;
        }
    }
}
