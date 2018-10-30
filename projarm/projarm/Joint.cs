using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace projarm
{
    class Joint : GrRoot
    {
        public char type; //S - Static, R - Revolute, P - Prismatic, G - Gripper
        public Point dot;

        public Point Dot
        {
            get
            {
                return dot;
            }
            set
            {
                dot.X = 629 + value.X;
                dot.Y = 545 - value.Y;
            }
        }
        public Joint(Joint j)
        {
            type = j.type;
            dot = j.dot;
        }
        public Joint(char _type, Point p)
        {
            type = _type;
            Dot = p;
        }
        public Joint(char _type)
        {
            type = _type;
            Dot = new Point(0, 0);
        }
        public void DotClone(Joint j)
        {
            dot = j.dot;
        }
        public void TransferFunction(double len, double angle)
        {
            angle *= -0.01745f;
            Dot = new Point((int)(Dot.X + len * Math.Cos(angle)) - 629,
                545 - (int)(Dot.Y + len * Math.Sin(angle)));
        }
        public override void Show(Graphics gr)
        {
            switch (type)
            {
                case 'S':
                    gr.FillEllipse(new SolidBrush(System.Drawing.Color.Black),
                        new Rectangle(Dot.X - 5, Dot.Y - 5, 10, 10));
                    break;
                case 'R':
                    gr.FillEllipse(new SolidBrush(System.Drawing.Color.Green),
                        new Rectangle(Dot.X - 5, Dot.Y - 5, 10, 10));
                    break;
                case 'P':
                    gr.FillEllipse(new SolidBrush(System.Drawing.Color.Yellow),
                        new Rectangle(Dot.X - 5, Dot.Y - 5, 10, 10));
                    break;
                case 'G':
                    gr.FillEllipse(new SolidBrush(System.Drawing.Color.Red),
                        new Rectangle(Dot.X - 5, Dot.Y - 5, 10, 10));
                    break;
                case 'D':
                    gr.FillEllipse(new SolidBrush(System.Drawing.Color.Purple),
                        new Rectangle(dot.X - 5, dot.Y - 5, 10, 10));
                    break;
                default:
                    break;
            }
        }
        public override void Hide(Graphics gr)
        {
            gr.FillEllipse(new SolidBrush(System.Drawing.Color.White),
                new Rectangle(Dot.X - 5, Dot.Y - 5, 10, 10));
        }
        public override void Move(Graphics gr, double q)
        {
        }
    }
}
