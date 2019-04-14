using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace ProjectARM
{
    class MathEngine
    {
        // Начало планирования со следующей точки пути
        public static double[][] MovingAlongTheTrajectory(Trajectory S, MathModel modelMnpltr, List<DPoint> DeltaPoints, BackgroundWorker worker)
        {
            double[][] q = new double[S.NumOfExtraPoints][];
            for (int i = 0; i < S.NumOfExtraPoints; i++)
                q[i] = new double[MathModel.N - 1];
            
            for (int i = 1; i < S.NumOfExtraPoints; i++)
            {
                worker.ReportProgress((int)((float)i / S.NumOfExtraPoints * 100));
                for (int j = 0; j < MathModel.N - 1; j++)
                    q[i - 1][j] = modelMnpltr.LagrangeMethodToThePoint(S.ExactExtraPoints[i - 1])[j];

                DeltaPoints.Add(new DPoint(i -  1, modelMnpltr.GetPointError(S.ExactExtraPoints[i - 1])));
            }

            return q;
        }

        // Начало планирования с текущей точки пути
        /*public static double[][] MovingAlongTheTrajectory(Trajectory S, MathModel modelMnpltr, List<DPoint> DeltaPoints, BackgroundWorker worker)
        {
            double[][] q = new double[S.NumOfExtraPoints][];
            for (int i = 0; i < S.NumOfExtraPoints; i++)
                q[i] = new double[MathModel.N - 1];

            for (int i = 0; i < S.NumOfExtraPoints; i++)
            {
                worker.ReportProgress((int)((float)(i + 1) / S.NumOfExtraPoints * 100));
                for (int j = 0; j < MathModel.N - 1; j++)
                    q[i][j] = modelMnpltr.LagrangeMethodToThePoint(S.ExactExtraPoints[i])[j];

                DeltaPoints.Add(new DPoint(i, modelMnpltr.GetPointError(S.ExactExtraPoints[i])));
            }

            return q;
        }*/
    }
}
