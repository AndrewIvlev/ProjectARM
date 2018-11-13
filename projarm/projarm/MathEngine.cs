using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace projarm
{
    class MathEngine
    {
        public static bool MovingAlongThePath(Graphics gr, Path S, MathModel ModelMnpltr, Manipulator mnpltr, BackgroundWorker worker)
        {
            double[] dq = new double[mnpltr.numOfUnits - 2]; //numofUnits - 2 = числу степеней подвижности манипулятора,
                                                             //так как нулевое звено и последнее статические
            foreach (double[] xy in S.ExactExtraPoint)
            {
                //Thread.Sleep(500);
                worker.ReportProgress((int)((float)S.ExactExtraPoint.IndexOf(xy) / S.ExtraPoint.Count * 100));
                if (worker.CancellationPending)
                    return false;

                dq = (double[])ModelMnpltr.LagrangeMethod(ref mnpltr.Q, xy).Clone();
                if (dq[0] + dq[1] + dq[2] + dq[3] == 0) continue;
                mnpltr.Move(gr, dq);
            }
            // Для демонстрации работы функции
            /*double[] q = new double[4] {1, -1, 140, 1};
            mnpltr.Move(gr, q);
            
            double[] dq = new double[4] { 0, 0, 0, 0 };
            for ( int i = 0; i < 60; i++)
            {
                for (int j = 0; j < 4; j++)
                    dq[j] = 1;
                Thread.Sleep(500);
                mnpltr.Move(gr, dq);worker.ReportProgress((int)((float)i / S.ExtraPoint.Count * 100));
                if (worker.CancellationPending)
                    return false;
            }*/

            return true;
        }
    }
}
