using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace projarm
{
    class MathEngine
    {
        public static bool MovingAlongThePath(Graphics gr, Path S, MathModel ModelMnpltr, Manipulator mnpltr, BackgroundWorker worker, ref List<Point> ForDeltaPoints)
        {
            double[] dq = new double[mnpltr.numOfUnits - 2]; //numofUnits - 2 = числу степеней подвижности манипулятора,
                                                             //так как нулевое звено и последнее статические

            foreach (double[] xy in S.ExactExtraPoint)
            {
                worker.ReportProgress((int)((float)S.ExactExtraPoint.IndexOf(xy) / S.ExtraPoint.Count * 100));
                if (worker.CancellationPending)
                    return false;

                dq = (double[])ModelMnpltr.LagrangeMethod(ref mnpltr.Q, xy).Clone();
                
                //if (dq[0] + dq[1] + dq[2] + dq[3] == 0) continue;
                mnpltr.Move(gr, dq);
                ForDeltaPoints.Add(ModelMnpltr.GetPointError(mnpltr.Q, dq, xy));/////////
            }
            return true;
        }
        public static double NormaVectora(Point p)
        {
            return Math.Sqrt(Math.Pow(p.X, 2) + Math.Pow(p.Y, 2));                
        }
    }
}
