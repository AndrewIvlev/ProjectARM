using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace projarm 
{
    class Manipulator : GrRoot
    {
        public Unit[] mnp;
        //Joint Prev, Curr;
        byte currUn;
        byte numOfUnits;

        public Manipulator(byte _numOfUnits)
        {
            numOfUnits = _numOfUnits;
            mnp = new Unit[numOfUnits];
            currUn = 0;
        }
        public void addUnit(Unit newU)
        {
            mnp[currUn++] = newU;
        }
        public override void Show(Graphics gr)
        {
            for (int i = 0; i < numOfUnits; i++)
                mnp[i].Show(gr);
        }
        public override void Move(Graphics gr, double q) { }
        public override void Hide(Graphics gr)
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
                char type = mnp[i].start.type;
                double len = mnp[i].lenght;
                double angle = mnp[i].angle;

                switch (type)
                {
                    case 'S':
                        mnp[i + 1].start.TransferFunction(len, angle);
                        break;
                    case 'R':
                        anglemnpltr += mnp[i].angle = q[i];
                        mnp[i].end.DotClone(mnp[i].start);
                        mnp[i].end.TransferFunction(len, anglemnpltr);
                        mnp[i + 1].start.DotClone(mnp[i].end);
                        break;
                    case 'P':
                        len = mnp[i].lenght = q[i];
                        mnp[i].end.DotClone(mnp[i].start);
                        mnp[i].end.TransferFunction(len, anglemnpltr);
                        mnp[i + 1].start.DotClone(mnp[i].end);
                        break;
                    case 'G':
                        mnp[i].end.DotClone(mnp[i].start);
                        break;
                    default:
                        break;
                }
            }
            Show(gr);
        }
    }
}
