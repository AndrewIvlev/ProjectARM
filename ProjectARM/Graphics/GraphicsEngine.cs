using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;

namespace ProjectARM
{
    static class GraphicsEngine
    {
        //Нужно графику так, чтобы можно было приостанавливать демонстрацию движения манипулятора, воспроизовадить и т.д
        //А мб получится реализовать замедленное/ускоренное воспроизведение движения манипулятора, было бы круто

        //Создать контейнер с данными  о движении манипулятора и прогрессе преодолевания им пути

        //Добавить таймер на форму и использовать его при отрисовки движения манипулятора
        /*public static void ShowMovementManipulator(Graphics gr)
        {
            int timeout = 1000;
            for (int i = 1; i < S.NumOfExtraPoints; i++)
            {
                S.Show(gr);
                Thread.Sleep(timeout);
                S.ShowNextPoints(gr, i);
                S.ShowPastPoints(gr, i);
                mnpltr.Move(gr);
            }
        }*/
    }
}
