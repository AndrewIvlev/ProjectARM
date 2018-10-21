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
        readonly char type; //S - Start, R - Revolute, P - Prismatic, G - Gripper
        Point dot;
        double q;
        public char Type
        {
            get
            {
                return type;
            }
        }
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
            q = j.q;
        }
        public Joint(char _type, Point p)
        {
            type = _type;
            Dot = p;
        }
        public Joint(char _type, double StartQ, Point p)
        {
            type = _type;
            q = StartQ;
            Dot = p;
        }
        public override void Show(Graphics gr)
        {
            switch (type)
            {
                case 'S':
                    gr.FillEllipse(new SolidBrush(System.Drawing.Color.Black), new Rectangle(Dot.X - 5, Dot.Y - 5, 10, 10));
                    break;
                case 'R':
                    gr.FillEllipse(new SolidBrush(System.Drawing.Color.Green), new Rectangle(Dot.X - 5, Dot.Y - 5, 10, 10));
                    break;
                case 'P':
                    gr.FillEllipse(new SolidBrush(System.Drawing.Color.Green), new Rectangle(Dot.X - 5, Dot.Y - 5, 10, 10));
                    break;
                case 'G':
                    gr.FillEllipse(new SolidBrush(System.Drawing.Color.Red), new Rectangle(Dot.X - 5, Dot.Y - 5, 10, 10));
                    break;
                default:
                    break;
            }
        }
        public override void Hide(Graphics gr)
        {
            gr.FillEllipse(new SolidBrush(System.Drawing.Color.White), new Rectangle(Dot.X - 5, Dot.Y - 5, 10, 10));
        }
        public override void Move(Graphics gr, double q)
        { }
    }
}
