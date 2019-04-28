using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace ProjectARM
{
    class MathEngine
    {
        // Начало планирования со следующей точки пути
        public static List<double[]> MovingAlongTheTrajectory(Trajectory S, MathModel model, List<Vector3D> DeltaPoints, BackgroundWorker worker)
        {
            var q = new List<double[]>();
            
            for (int i = 1; i < S.NumOfExtraPoints; i++)
            {
                var tmpQ = new double[model.n - 1];
                worker.ReportProgress((int)((float)i / S.NumOfExtraPoints * 100));
                for (int j = 0; j < model.n - 1; j++)
                {
                    model.LagrangeMethodToThePoint(S.ExactExtra[i - 1]);
                    tmpQ[j] = model.q[j];
                }
                q.Add(tmpQ);
                DeltaPoints.Add(new Vector3D(i - 1, model.GetPointError(S.ExactExtra[i - 1]), 0));
            }

            return q;
        }

        // Начало планирования с текущей точки пути
        /*public static double[][] MovingAlongTheTrajectory(Trajectory S, MathModel model, List<Vector3D> DeltaPoints, BackgroundWorker worker)
        {
            double[][] q = new double[S.NumOfExtraPoints][];
            for (int i = 0; i < S.NumOfExtraPoints; i++)
                q[i] = new double[model.n - 1];

            for (int i = 0; i < S.NumOfExtraPoints; i++)
            {
                worker.ReportProgress((int)((float)(i + 1) / S.NumOfExtraPoints * 100));
                for (int j = 0; j < model.n - 1; j++)
                    q[i][j] = model.LagrangeMethodToThePoint(S.ExactExtra[i])[j];

                DeltaPoints.Add(new Vector3D(i, model.GetPointError(S.ExactExtra[i])));
            }

            return q;
        }*/
    }
}
