using System.Windows.Media.Media3D;

namespace ArmManipulatorArm.MathModel.Matrix
{
    using System;

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
            base.Rows = 3;
            base.Columns = 4;
            M = new double[,] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 } };
        }
        public BlockMatrix(double[,] mDoubles)
        {
            base.Rows = 3;
            base.Columns = 4;
            M = new double[3, 4];
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    M[i, j] = mDoubles[i, j];
                }
            }
        }

        public static bool operator ==(BlockMatrix a, BlockMatrix b)
        {
            if (a.Rows != b.Rows || a.Columns != b.Columns)
                return false;

            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    if (Math.Abs(a[i, j] - b[i, j]) > 0)
                    {
                        Console.WriteLine($"Element left matrix [{i},{j}] = {a[i, j]} not equal to element of right matrix = {b[i, j]}");
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool operator !=(BlockMatrix a, BlockMatrix b)
        {
            return !(a == b);
        }

        public Vector3D ColumnAsVector3D(int i) => new Vector3D(M[0, i], M[1, i], M[2, i]);
        
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
