using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;

namespace ProjectARM
{
    class MathEngine
    {
        public static bool MovingAlongThePath(Path S, MatrixMathModel ModelMnpltr, Manipulator mnpltr,
            Graphics gr, BackgroundWorker worker, ref List<Dpoint> DeltaPoints)
        {
            for (int i = 1; i < S.NumOfExtraPoints; i++)
            {
                //S.NumOfExtraPoints < 180 ? Thread.Sleep(2222) : Thread.Sleep(222);
                worker.ReportProgress((int)((float)i / S.NumOfExtraPoints * 100));
                if (worker.CancellationPending)
                    return false;

                //ModelMnpltr.LagrangeMethod(ref mnpltr.Q, S.ExactExtraPoints[i]);
                S.Show(gr);
                S.ShowNextPoints(gr, i);
                S.ShowPastPoints(gr, i);
                mnpltr.Move(gr);

                //DeltaPoints.Add(new Dpoint(i, ModelMnpltr.GetPointError(mnpltr.Q, S.ExactExtraPoints[i])));
            }
            return true;
        }
    }
}
