using System;
using System.Collections;

namespace ArmManipulatorArm.MathModel.Arm
{
    using ArmManipulatorArm.MathModel.Matrix;

    public static class ArmMetaDataCalculator
    {
        private static Arm arm;

        public static void Init(Arm arm)
        {
            ArmMetaDataCalculator.arm = arm;
        }

        public static void CalcdS()
        {
            for (var i = 0; i < arm.N; i++)
            {
                switch (arm.Units[i].Type)
                {
                    case 'R':
                        arm.dS[i] = new BlockMatrix();
                        arm.dS[i][0, 0] = -Math.Sin(arm.Units[i].Q);
                        arm.dS[i][0, 1] = -Math.Cos(arm.Units[i].Q);
                        arm.dS[i][1, 0] =  Math.Cos(arm.Units[i].Q);
                        arm.dS[i][1, 1] = -Math.Sin(arm.Units[i].Q);
                        arm.dS[i][2, 2] = 0;
                        break;
                    case 'P':
                        arm.dS[i] = new BlockMatrix();
                        arm.dS[i][0, 0] = 0;
                        arm.dS[i][1, 1] = 0;
                        arm.dS[i][2, 2] = 0;
                        arm.dS[i][2, 3] = 1;
                        break;
                    default:
                        throw new Exception("Unexpected unit type");
                }
            }
        }

        // TODO: refactor this method, use already calculated matrix 
        // multiplication in T чтобы заново не перемножать одни и те же матрицы
        private static BlockMatrix CalcdF(int index)
        {
            var dF = new BlockMatrix();
            dF *= arm.RootB;
            for (var i = 1; i < arm.N; i++)
            {
                dF *= i == index ? arm.dS[i] : arm.S[i];
                dF *= arm.Units[i].B;
            }

            return dF;
        }

        public static void CalcdT()
        {
            arm.dT = new ArrayList();
            for (var i = 0; i < arm.N; i++)
                arm.dT.Add(CalcdF(i));
        }
        
        /// <summary>
        /// D is Matrix of gradients Fx, Fy and Fz
        /// ( Fxq1 Fxq2 ... Fxqn )
        /// ( Fyq1 Fyq2 ... Fyqn )
        /// ( Fzq1 Fzq2 ... Fzqn )
        /// </summary>
        public static void CalcD()
        {
            for (var i = 0; i < arm.N; i++)
            {
                var b = arm.GetdF(i);
                arm.D[0, i] = b.X;
                arm.D[1, i] = b.Y;
                arm.D[2, i] = b.Z;
            }
        }

        // Вычисляем матрицу коэффициентов
        public static void CalcC()
        {
            for (var i = 0; i < arm.N; i++)
            {
                arm.C[0, 0] += arm.D[0, i] * arm.D[0, i];
                arm.C[0, 1] += arm.D[0, i] * arm.D[1, i];
                arm.C[0, 2] += arm.D[0, i] * arm.D[2, i];
                arm.C[1, 1] += arm.D[1, i] * arm.D[1, i];
                arm.C[1, 2] += arm.D[1, i] * arm.D[2, i];
                arm.C[2, 2] += arm.D[2, i] * arm.D[2, i];
            }
            arm.C[1, 0] = arm.C[0, 1];
            arm.C[2, 0] = arm.C[0, 2];
            arm.C[2, 1] = arm.C[1, 2];
        }

        public static void CalcSByUnitsType()
        {
            for (var i = 0; i < arm.N; i++)
            {
                switch (arm.Units[i].Type)
                {
                    case 'R':
                        arm.S[i] = new BlockMatrix();
                        arm.S[i][0, 0] =  Math.Cos(arm.Units[i].Q);
                        arm.S[i][0, 1] = -Math.Sin(arm.Units[i].Q);
                        arm.S[i][1, 0] =  Math.Sin(arm.Units[i].Q);
                        arm.S[i][1, 1] =  Math.Cos(arm.Units[i].Q);
                        break;
                    case 'P':
                        arm.S[i] = new BlockMatrix();
                        arm.S[i][2, 3] = arm.Units[i].Q;
                        break;
                    default:
                        throw new Exception("Unexpected unit type");
                }
            }
        }

        // TODO: Сделать такой же массив, умножая с правой стороны
        public static void CalcT()
        {
            arm.T = new ArrayList();
            var tmp = new BlockMatrix();

            arm.T.Add(tmp = arm.RootB * arm.S[0]);
            for (var i = 1; i < arm.N; i++)
                arm.T.Add(tmp *= arm.Units[i - 1].B * arm.S[i]);

            arm.T.Add(tmp *= arm.Units[arm.N - 1].B);
        }

    }
}
