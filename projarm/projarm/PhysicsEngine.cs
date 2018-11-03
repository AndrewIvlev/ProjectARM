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
        public static bool MovingAlongThePath(Graphics gr, Path S, MathModel ModelMnpltr, Manipulator mnpltr,
                                                byte numOfUnits, Int32 k, BackgroundWorker worker)
        {
            double[] dq = new double[4] { 0, 0, 0, 0 };
            //double[] dq = new double[numOfUnits - 2];
            
            for (int i = 0; i < k; i++)
            {
                dq = (double[])ModelMnpltr.LagrangeMethod(ref mnpltr.Q, S.ExactExtraPoint[i]).Clone();
                for (int j = 0; j < numOfUnits - 2; j++)
                {
                    mnpltr.Q[j] += dq[j];
                    mnpltr.Move(gr);
                }
                Thread.Sleep(100);
                worker.ReportProgress((int)((float)i / k * 100));
                if (worker.CancellationPending)
                    return false;
            }
            return true;
        }
    }
}
