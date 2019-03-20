using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ProjectARM
{
    class Unit : IGraphics
    {
        public double lenght;
        public double angle;
        public Joint start;
        public Joint end;

        public Unit(Joint startJ, Joint endJ, double len, double a)
        {
            start = new Joint(startJ);
            end = new Joint(endJ);
            lenght = len;
            angle = a;
        }

        public void Show(Graphics gr)
        {
            start.Show(gr);
            gr.DrawLine(new Pen(Color.DarkBlue, 4), start.Dot, end.Dot);
            end.Show(gr);
        }

        public void Hide(Graphics gr)
        {
            gr.DrawLine(new Pen(Color.LightBlue, 4), start.Dot, end.Dot);
            start.Hide(gr);
            end.Hide(gr);
        }
    }
}
