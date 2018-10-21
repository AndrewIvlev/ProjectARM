using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace projarm
{
    class Unit : GrRoot
    {
        double lenght;
        double angle;

        Joint start;
        Joint end;

        public Unit(Joint startJ, Joint endJ, double len, double a)
        {
            start = new Joint(startJ);
            end = new Joint(endJ);
            lenght = len;
            angle = a;
        }
        public override void Show(Graphics gr)
        {
            if ( (end.Type != 'S'))
            {
                gr.DrawLine(new Pen(Color.Blue, 4), start.Dot, end.Dot);
                start.Show(gr);
            }
            end.Show(gr);
        }
        public override void Hide(Graphics gr)
        {
            gr.DrawLine(new Pen(Color.White, 4), start.Dot, end.Dot);
            start.Show(gr);
            end.Show(gr);
        }
        public override void Move(Graphics gr, double q)
        {

        }
    }
}
