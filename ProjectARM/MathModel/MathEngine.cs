using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace ProjectARM
{
    class MathEngine
    {
        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!1
        //Здесь не должно быть графики, только вычисления.
        //графику написать в другом месте
        //Видимо точки также нужно разделить. на те что нужны для вычисления обобщенных координат и те что нужны для отрисовки
        //будет одна функция перевода траектории для вычислений в траекторию для графики (из одних координат в другие)
        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!1

        public static List<Dpoint> MovingAlongTheTrajectory(Trajectory S, MathModel modelMnpltr, BackgroundWorker worker)
        {
            List<Dpoint> DeltaPoints = new List<Dpoint>();
            for (int i = 1; i < S.NumOfExtraPoints; i++)
            {
                worker.ReportProgress((int)((float)i / S.NumOfExtraPoints * 100));

                modelMnpltr.LagrangeMethodToThePoint(S.ExactExtraPoints[i]);

                DeltaPoints.Add(new Dpoint(i, modelMnpltr.GetPointError(S.ExactExtraPoints[i])));
            }

            return DeltaPoints;
        }
    }
}
