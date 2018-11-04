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
    class PhysicsEngine
    {
        public static bool MovingAlongThePath(Graphics gr, Path S, MathModel ModelMnpltr, Manipulator mnpltr, BackgroundWorker worker)
        {
            double[] dq = new double[mnpltr.numOfUnits - 2]; //numofUnits - 2 = числу степеней подвижности манипулятора,
                                                            //так как нулевое звено и последнее статические
            
            for (int i = 0; i < S.ExtraPoint.Count; i++)
            {
                dq = (double[])ModelMnpltr.LagrangeMethod(ref mnpltr.Q, S.ExactExtraPoint[i]).Clone();
                for (int j = 0; j < mnpltr.numOfUnits - 2; j++)
                {
                    mnpltr.Q[j] += dq[j];
                    mnpltr.Move(gr);
                }
                worker.ReportProgress((int)((float)i / S.ExtraPoint.Count * 100));
                if (worker.CancellationPending)
                    return false;
            }
            return true;
        }
    }
}
