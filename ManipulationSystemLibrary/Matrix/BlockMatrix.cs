using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace ManipulationSystemLibrary
{
    /// <summary>
    /// Block matrices with size 3x4
    /// Example:
    /// ( 1 0 0 0 )
    /// ( 0 1 0 0 )
    /// ( 0 0 1 0 )
    /// </summary>
    public class BlockMatrix : Matrix
    {
        public BlockMatrix()
        {
            M = new double[,] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 } };
        }
        public BlockMatrix(double[,] mDoubles)
        {
            M = new double[3, 4];
            for (int i = 0; i < 3; i++)
            for (int j = 0; j < 4; j++)
                M[i, j] = mDoubles[i, j];
        }

        public Vector3D GetLastColumn() => new Vector3D(M[0, 3], M[1, 3], M[2, 3]);
        
        public static BlockMatrix operator *(BlockMatrix A, BlockMatrix B) => new BlockMatrix
            {
                [0, 0] = A[0, 0] * B[0, 0] + A[0, 1] * B[1, 0] + A[0, 2] * B[2, 0],
                [0, 1] = A[0, 0] * B[0, 1] + A[0, 1] * B[1, 1] + A[0, 2] * B[2, 1],
                [0, 2] = A[0, 0] * B[0, 2] + A[0, 1] * B[1, 2] + A[0, 2] * B[2, 2],
                [0, 3] = A[0, 0] * B[0, 3] + A[0, 1] * B[1, 3] + A[0, 2] * B[2, 3] + A[0, 3],
                [1, 0] = A[1, 0] * B[0, 0] + A[1, 1] * B[1, 0] + A[1, 2] * B[2, 0],
                [1, 1] = A[1, 0] * B[0, 1] + A[1, 1] * B[1, 1] + A[1, 2] * B[2, 1],
                [1, 2] = A[1, 0] * B[0, 2] + A[1, 1] * B[1, 2] + A[1, 2] * B[2, 2],
                [1, 3] = A[1, 0] * B[0, 3] + A[1, 1] * B[1, 3] + A[1, 2] * B[2, 3] + A[1, 3],
                [2, 0] = A[2, 0] * B[0, 0] + A[2, 1] * B[1, 0] + A[2, 2] * B[2, 0],
                [2, 1] = A[2, 0] * B[0, 1] + A[2, 1] * B[1, 1] + A[2, 2] * B[2, 1],
                [2, 2] = A[2, 0] * B[0, 2] + A[2, 1] * B[1, 2] + A[2, 2] * B[2, 2],
                [2, 3] = A[2, 0] * B[0, 3] + A[2, 1] * B[1, 3] + A[2, 2] * B[2, 3] + A[2, 3]
            };
    }
}
