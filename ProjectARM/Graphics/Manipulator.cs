using System;
using System.Drawing;

namespace ProjectARM
{
    class Manipulator : IGraphics
    {
        public byte numOfUnits;
        public Unit[] mnp;
        byte currUn;

        public Manipulator(byte _numOfUnits)
        {
            numOfUnits = _numOfUnits;
            mnp = new Unit[numOfUnits];
            currUn = 0;
        }
        public void AddUnit(Unit newU)
        {
            mnp[currUn++] = newU;
        }
        public void Show(Graphics gr)
        {
            for (int i = 0; i < numOfUnits; i++)
                mnp[i].Show(gr);
        }
        public void Hide(Graphics gr)
        {
            for (int i = 0; i < numOfUnits; i++)
                mnp[i].Hide(gr);
        }
        public void Move(Graphics gr, double[] q)
        {
            Hide(gr);
            double anglemnpltr = 0f;
            for (int i = 1; i < numOfUnits; i++)
            {
                switch (mnp[i].start.type)
                {
                    case 'R':
                        anglemnpltr -= q[i - 1];
                        mnp[i].end.DotClone(mnp[i].start);
                        mnp[i].end.TransferFunction(mnp[i].lenght, anglemnpltr);
                        mnp[i + 1].start.DotClone(mnp[i].end);
                        break;
                    case 'P':
                        mnp[i].end.DotClone(mnp[i].start);
                        mnp[i].end.TransferFunction(mnp[i].lenght + q[i - 1], anglemnpltr);
                        mnp[i + 1].start.DotClone(mnp[i].end);
                        break;
                    default:
                        break;
                }
            }
            Show(gr);
        }
    }
}