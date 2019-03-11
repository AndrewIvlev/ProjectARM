using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectARM
{
    class BlockMatrix : Vector
    {
        private double[,] M;
        public double[,] MT
        {
            get
            {
                return M;
            }
            set
            {
                for(int i = 0; i < 2; i++)
                    for( int j = 0; j < 3; j++)
                        M[i, j] = value[i, j];
            }
        }

        public BlockMatrix()
        {
            M = new double[2, 3] { { 1, 0, 0 }, { 0, 1, 0 } };
        }
        public BlockMatrix(char type, double q)
        {
            if (type == 'R')
                M = new double[2, 3] { { Math.Cos(q), -Math.Sin(q), 0 }, { Math.Sin(q), Math.Cos(q), 0 } };
            else if (type == 'P')
                M = new double[2, 3] { { 1, 0, q }, { 0, 1, 0 } };
        }
        public BlockMatrix(BlockMatrix _M)
        {
            M = _M.M;
        }
        public static BlockMatrix operator *(BlockMatrix A, BlockMatrix B)
        {
            //Умножение блочных матриц размера 2х3
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
