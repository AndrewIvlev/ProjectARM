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
        public override void Move(Graphics gr) { }
        /*public static Unit operator =(Unit A, Unit B)
        {
            A.start = B.start;
            A.end = B.end;
            A.lenght = B.lenght;
            A.angle = B.angle;
            return A;
        }*/
        public override void Show(Graphics gr)
        {
            start.Show(gr);
            gr.DrawLine(new Pen(Color.DarkBlue, 4), start.Dot, end.Dot);
            end.Show(gr);
        }
        public override void Hide(Graphics gr)
        {
            gr.DrawLine(new Pen(Color.LightBlue, 4), start.Dot, end.Dot);
            start.Hide(gr);
            end.Hide(gr);
        }
    }
}
