using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace projarm
{
    class Path
    {
        public List<Point> AnchorPoint; // Опорные точки
        public List<Point> ExtraPoint; // Дополнительные точки
        public List<double[]> ExactExtraPoint; // Точные доп точки
        public int NumOfAnchorPoints;          //Число опорных точек
        public double len;
        //int k;

        public Path()
        {
            len = 0;
            NumOfAnchorPoints++;
            AnchorPoint = new List<Point>();
            ExtraPoint = new List<Point>();
            ExactExtraPoint = new List<double[]>();
        }
        public Path(Point StartPoint)
        {
            NumOfAnchorPoints++;
            AnchorPoint = new List<Point>() { StartPoint };
            ExtraPoint = new List<Point>();
            ExactExtraPoint = new List<double[]>();
        }
        public double DistanceBetweenPoints(Point A, Point B)
        {
            return Math.Sqrt(Math.Pow(A.X - B.X, 2) + Math.Pow(A.Y - B.Y, 2));
        }
        public void AddAnchorPoint(Point NAP) //NAP = New Anchot Point
        {
            AnchorPoint.Add(NAP);
            NumOfAnchorPoints++;
            int index = AnchorPoint.IndexOf(NAP);
            if (index != 0)
                len += DistanceBetweenPoints(AnchorPoint[index - 1], NAP);
        }
        public void SplitPath(int k)    //Разбить путь на k отрезков, с шагом step принадлежащем отрезку [0,1]
        {
            double step = len / (k - NumOfAnchorPoints); // Шаг = длину всего пути делим на число К с вычетом количества опорных точек

            ExtraPoint.AddRange(AnchorPoint.ToArray());
            foreach (Point P in AnchorPoint)
            {
                int index = AnchorPoint.IndexOf(P);
                if (index != 0)
                {
                    int i = 1;
                    double lambda = step;
                    double dist = DistanceBetweenPoints(AnchorPoint[index - 1], P);
                    while(lambda < dist)
                    {
                        lambda = (step * i) / (dist - step * i);
                        double x = (AnchorPoint[index - 1].X + lambda * P.X) / (1 + lambda);
                        double y = (AnchorPoint[index - 1].Y + lambda * P.Y) / (1 + lambda);
                        ExactExtraPoint.Insert(index - 1 + i, new double[] { x, y });
                        Point newExtraP = new Point((int)x, (int)y);
                        ExtraPoint.Insert(index - 1 + i++, newExtraP);
                    }
                }
                ExactExtraPoint.Add(new double[] { P.X, P.Y });
            }
        }
        public void ShowExtraPoints(Graphics gr)
        {
            foreach (Point P in ExtraPoint)
            {
                gr.FillEllipse(new SolidBrush(System.Drawing.Color.Red),
                        new Rectangle(P.X - 1, P.Y - 1, 3, 3));
            }
        }
        public void Show(Graphics gr)
        {
            foreach (Point P in AnchorPoint)
            {
                int index = AnchorPoint.IndexOf(P);
                if (index + 1 < AnchorPoint.Count)
                {
                    Point NextP = AnchorPoint[index + 1];
                    gr.DrawLine(new Pen(Color.Purple, 4), P, NextP);
                }
                gr.FillEllipse(new SolidBrush(System.Drawing.Color.Purple),
                        new Rectangle(P.X - 6, P.Y - 6, 12, 12));
                if (index > 9)
                    gr.DrawString($"{index}", new Font("Arial", 8),
                    new SolidBrush(Color.Yellow), P.X - 8, P.Y - 7);
                else
                    gr.DrawString($"{index}", new Font("Arial", 9),
                    new SolidBrush(Color.Yellow), P.X - 5, P.Y - 6);
            }
        }
        public void Hide(Graphics gr)
        {

        }
        public void Move(Graphics gr)
        {

        }
        public void MoveAnchorPoint(Graphics gr)
        {

        }
    }
}
