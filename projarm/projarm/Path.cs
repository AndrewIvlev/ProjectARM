using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace projarm
{
    public struct dpoint
    {
        public double x;
        public double y;
        //double len; длина от this точки до начала пути
        public dpoint(double _x, double _y)
        {
            x = _x;
            y = _y;
        }
    }

    class Path
    {
        public List<Point> AnchorPoints; // Опорные точки
        public dpoint[] ExtraPoints; // Дополнительные точки
        public Int32 NumOfExtraPoints;

        public Path()
        {
            AnchorPoints = new List<Point>();
            ExtraPoints = new dpoint[512];
            NumOfExtraPoints = 0;
        }
        public Path(Point StartPoint)
        {
            AnchorPoints = new List<Point>() { StartPoint };
            ExtraPoints = new dpoint[512];
            NumOfExtraPoints = 0;
        }
        public void Clear()
        {
            AnchorPoints.Clear();
            Array.Clear(ExtraPoints, 0, ExtraPoints.Length);
        }
        public double DistanceBetweenPoints(Point A, Point B)
        {
            return Math.Sqrt(Math.Pow(B.X - A.X, 2) + Math.Pow(B.Y - A.Y, 2));
        }
        public double DistanceBetweenDPoints(dpoint p1, dpoint p2)
        {
            return Math.Sqrt(Math.Pow(p2.x - p1.x, 2) + Math.Pow(p2.y - p1.y, 2));
        }
        public double DistanceBerweenPointAndDpoint(Point P, dpoint dp)
        {
            return Math.Sqrt(Math.Pow(dp.x - P.X, 2) + Math.Pow(dp.y - P.Y, 2));
        }
        public int NearestPointIndex(Point O) //Возвращает индекс ближайшей опорной точки к точке О
        {
            int index = -1;
            double MinDist = 8192;
            foreach (Point P in AnchorPoints)
            {
                double dist = DistanceBetweenPoints(O, P);
                if (dist < MinDist)
                {
                    MinDist = dist;
                    index = AnchorPoints.IndexOf(P);
                }
            }
            return index;
        }
        public void AddAnchorPoint(Point NAP) //NAP = New Anchor Point
        {
            AnchorPoints.Add(NAP);
        }
        public double GetLen()
        {
            double len = 0;
            for ( int i = 1; i < AnchorPoints.Count; i++)
                len += DistanceBetweenPoints(AnchorPoints[i - 1], AnchorPoints[i]);
            return len;
        }
        public dpoint ToDpoint(Point P)
        {
            return new dpoint(P.X, P.Y);
        }
        public double[] GetSteps()
        {
            double len = GetLen();
            double[] res = new double[AnchorPoints.Count];
            for (int i = 0; i < AnchorPoints.Count; i++)
                res[i] = DistanceBetweenPoints(AnchorPoints[0], AnchorPoints[i]) / len;
            return res;
        }
        public void SplitPath(double step)
        {
            int index = 1;
            ExtraPoints[0] = ToDpoint(AnchorPoints[0]);
            for (int i = 1; i < AnchorPoints.Count; i++)
            {
                int j = 0;
                double lambda = 0;
                double x = AnchorPoints[i - 1].X;
                double y = AnchorPoints[i - 1].Y;
                double dist = DistanceBetweenPoints(AnchorPoints[i - 1], AnchorPoints[i]);
                do
                {
                    lambda = (step * j) / (dist - step * j);
                    x = (AnchorPoints[i - 1].X + lambda * AnchorPoints[i].X) / (1 + lambda);
                    y = (AnchorPoints[i - 1].Y + lambda * AnchorPoints[i].Y) / (1 + lambda);
                    ExtraPoints[index++] = new dpoint(x, y);
                    j++;
                }
                while (DistanceBerweenPointAndDpoint(AnchorPoints[i - 1], new dpoint(x, y)) + step < dist);
            }
            ExtraPoints[index] = ToDpoint(AnchorPoints[AnchorPoints.Count - 1]);
            NumOfExtraPoints = ++index;
        }
        public void SplitPath(Int32 k)
        {
            int index = 1;
            double step = GetLen() / k; // Шаг = длину всего пути делим на количество доп точек
            ExtraPoints[0] = ToDpoint(AnchorPoints[0]);
            for (int i = 1; i < AnchorPoints.Count; i++)
            {
                int j = 0;
                double lambda = 0;
                double x = AnchorPoints[i - 1].X;
                double y = AnchorPoints[i - 1].Y;
                double dist = DistanceBetweenPoints(AnchorPoints[i - 1], AnchorPoints[i]);
                do
                {
                    lambda = (step * j) / (dist - step * j);
                    x = (AnchorPoints[i - 1].X + lambda * AnchorPoints[i].X) / (1 + lambda);
                    y = (AnchorPoints[i - 1].Y + lambda * AnchorPoints[i].Y) / (1 + lambda);
                    ExtraPoints[index++] = new dpoint(x, y);
                    j++;
                }
                while (DistanceBerweenPointAndDpoint(AnchorPoints[i - 1], new dpoint(x, y)) + step < dist);
            }
            ExtraPoints[index] = ToDpoint(AnchorPoints[AnchorPoints.Count - 1]);
            NumOfExtraPoints = ++index;
            //NumOfExtraPoints = k + AnchorPoints.Count;
            if (++index == NumOfExtraPoints)
                ;//Ни одной точки не потерялось
            else
                ;//Потерялось NumOfExtraPoints - index точек
            /* Другой вариант:
             * k = _k; 
            int i = 0;
            double[] steps = GetSteps();
            for (int j = 0; j < k; j++)
            {
                double step = (1.0 / k) * j;
                if (step > steps[i]) i++;
                if (i >= AnchorPoints.Count - 1) break;
                ExtraPoints[j].x = AnchorPoints[i].X + (step - steps[i]) / (steps[i + 1] - steps[i]) * (AnchorPoints[i + 1].X - AnchorPoints[i].X);
                ExtraPoints[j].y = AnchorPoints[i].Y + (step - steps[i]) / (steps[i + 1] - steps[i]) * (AnchorPoints[i + 1].Y - AnchorPoints[i].Y);
            }
            */
        }
        public void ExtraClear()
        {
            Array.Clear(ExtraPoints, 0, NumOfExtraPoints);
        }
        public void ShowExtraPoints(Graphics gr)
        {
            for ( int i = 0; i < NumOfExtraPoints; i++)
                gr.FillEllipse(new SolidBrush(System.Drawing.Color.Red), new Rectangle((int)ExtraPoints[i].x - 3, (int)ExtraPoints[i].y - 3, 6, 6));
        }
        public void ExactExtraPointOffset(Point offset)
        {
            for (int i = 0; i < ExtraPoints.Length; i++)
            {
                ExtraPoints[i].x -= offset.X;
                ExtraPoints[i].y = offset.Y - ExtraPoints[i].y;
            }
        }
        public void TransExtraPoints(Int32 k, double CoeftoRealW)
        {
            for ( int i = 0; i < k; i++)
            {
                ExtraPoints[i].x *= CoeftoRealW;
                ExtraPoints[i].y *= CoeftoRealW;
            }
        }
        public void TransferFunction(Point offset, Int32 k, double CoeftoRealW)
        {
            for (int i = 0; i < k; i++)
            {
                ExtraPoints[i].x -= offset.X;
                ExtraPoints[i].y = offset.Y - ExtraPoints[i].y;
                ExtraPoints[i].x *= CoeftoRealW;
                ExtraPoints[i].y *= CoeftoRealW;
            }
        }
        public void TransferFunction(Point offset, double CoeftoRealW)
        {
            for (int i = 0; i < NumOfExtraPoints; i++)
            {
                ExtraPoints[i].x -= offset.X;
                ExtraPoints[i].y = offset.Y - ExtraPoints[i].y;
                ExtraPoints[i].x *= CoeftoRealW;
                ExtraPoints[i].y *= CoeftoRealW;
            }
        }
        public void Show(Graphics gr)
        {
            foreach (Point P in AnchorPoints)
            {
                int index = AnchorPoints.IndexOf(P);
                if (index + 1 < AnchorPoints.Count)
                {
                    Point NextP = AnchorPoints[index + 1];
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
            foreach (Point P in AnchorPoints)
            {
                int index = AnchorPoints.IndexOf(P);
                if (index + 1 < AnchorPoints.Count)
                {
                    Point NextP = AnchorPoints[index + 1];
                    gr.DrawLine(new Pen(Color.LightBlue, 4), P, NextP);
                }
                gr.FillEllipse(new SolidBrush(System.Drawing.Color.LightBlue),
                        new Rectangle(P.X - 6, P.Y - 6, 12, 12));
            }
        }
    }
}
