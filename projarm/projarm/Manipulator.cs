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
        Unit[] mnp;
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
            for(int i = 0; i < numOfUnits; i++)
                mnp[i].Show(gr);
        }
        public override void Hide(Graphics gr) { }
        public override void Move(Graphics gr, double q) { }
    }
}
