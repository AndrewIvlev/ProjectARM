using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;

namespace projarm
{
    class MathEngine
    {
        public static bool MovingAlongThePath(Graphics gr, Path S, MathModel ModelMnpltr, Manipulator mnpltr, BackgroundWorker worker, ref List<double[]> DeltaPoints)
        {
            double[] dq = new double[mnpltr.numOfUnits - 2]; //numofUnits - 2 = числу степеней подвижности манипулятора,
                                                            //так как нулевое звено и последнее статические
            for (int i = 1; i < S.NumOfExtraPoints; i++)
            {
                worker.ReportProgress((int)((float)i / S.NumOfExtraPoints * 100));
                if (worker.CancellationPending)
                    return false;

                dq = (double[])ModelMnpltr.LagrangeMethod(ref mnpltr.Q, S.ExtraPoints[i]).Clone();
                Thread.Sleep(50);
                //if (dq[0] + dq[1] + dq[2] + dq[3] == 0) continue;
                mnpltr.Move(gr, dq);
                //DeltaPoints.Add(new double[2] { i, ModelMnpltr.GetPointError(mnpltr.Q, dq, S.ExtraPoints[i]) });
            }
            return true;
        }
    }
}
