using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace projarm
{
    class Joint : IGraphics
    {
        public char type; //S - Static, R - Revolute, P - Prismatic, G - Gripper
        public Point dot;
        public Point offset;

        public Point Dot
        {
            get
            {
                return dot;
            }
            set
            {
                dot.X = offset.X + value.X;
                dot.Y = offset.Y - value.Y;
            }
        }
        public Joint(Joint j)
        {
            type = j.type;
            dot = j.dot;
        }
        public Joint(char _type)
        {
            type = _type;
            Dot = new Point(0, 0);
        }
        public Joint(char _type, Point _offset)
        {
            type = _type;
            offset = _offset;
            Dot = new Point(0, 0);
        }
        public void DotClone(Joint j) => dot = j.dot;
        public void TransferFunction(double len, double angle)
        {
            Dot = new Point((int)(Dot.X + len * Math.Cos(angle)) - offset.X,
                offset.Y - (int)(Dot.Y + len * Math.Sin(angle)));
        }
        public void Show(Graphics gr)
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
                default:
                    break;
            }
        }
        public void Hide(Graphics gr) => gr.FillEllipse(new SolidBrush(System.Drawing.Color.LightBlue), new Rectangle(Dot.X - 5, Dot.Y - 5, 10, 10));
        public void Move(Graphics gr) { }
    }
}
