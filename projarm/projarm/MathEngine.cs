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
        public static bool MovingAlongThePath(Path S, MathModel ModelMnpltr, Manipulator mnpltr,
            Graphics gr, BackgroundWorker worker, ref List<dpoint> DeltaPoints)
        {
            for (int i = 0; i < S.NumOfExtraPoints; i++)
            {
                Thread.Sleep(100);
                worker.ReportProgress((int)((float)i / S.NumOfExtraPoints * 100));
                if (worker.CancellationPending)
                    return false;

                ModelMnpltr.LagrangeMethod(ref mnpltr.Q, S.ExtraPoints[i]);
                //if (dq[0] + dq[1] + dq[2] + dq[3] == 0) continue;
                mnpltr.Move(gr);
                //gr.FillEllipse(new SolidBrush(System.Drawing.Color.Green), new Rectangle((int)S.ExtraPoints[i].x - 3, (int)S.ExtraPoints[i].y - 3, 6, 6));
                DeltaPoints.Add(new dpoint(i, ModelMnpltr.GetPointError(mnpltr.Q, S.ExtraPoints[i])));
            }
            return true;
            //Для демонстрации работы функции
            mnpltr.Q = new double[4] { 0, 0, 0, 0 };
            mnpltr.Move(gr);
            
            for ( int i = 0; i < 180; i++)
            {
                //for (int j = 0; j < 4; j++)
                    mnpltr.Q[0] += MathModel.DegreeToRadian(1);
                Thread.Sleep(100);
                mnpltr.Move(gr);
                worker.ReportProgress((int)((float)i / 180 * 100));
                if (worker.CancellationPending)
                    return false;
            }
            return true;
        }
    }
}
