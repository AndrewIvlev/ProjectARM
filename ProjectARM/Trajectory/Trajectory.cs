using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ProjectARM
{
    public struct Dpoint
    {
        public double x;
        public double y;

        public Dpoint(double _x, double _y)
        {
            x = _x;
            y = _y;
        }
    }

    class Trajectory
    {
        /// <summary>
        /// Опорные точки
        /// Список хранит координаты опорных точек в графических координатах
        /// </summary>
        public List<Point> AnchorPoints;
        /// <summary>
        /// Дополнительные точки
        /// Список хранит координаты  в графических координатах
        /// </summary>
        public List<Point> ExtraPoints;
        public Dpoint[] ExactExtraPoints; // Точные дополнительные точки
        public int NumOfExtraPoints;

        public Trajectory()
        {
            AnchorPoints = new List<Point>();
            ExtraPoints = new List<Point>();
            ExactExtraPoints = new Dpoint[3333]; // 3333 is magic number // p.s. it is the max number of ExactExtraPoint
            NumOfExtraPoints = 0;
        }

        public Trajectory(Point StartPoint)
        {
            AnchorPoints = new List<Point>() { StartPoint };
            ExtraPoints = new List<Point>();
            ExactExtraPoints = new Dpoint[1024];
            NumOfExtraPoints = 0;
        }

        public void Clear()
        {
            AnchorPoints.Clear();
            ExtraPoints.Clear();
            Array.Clear(ExactExtraPoints, 0, ExactExtraPoints.Length);
        }

        public double DistanceBetweenPoints(Point A, Point B) => Math.Sqrt(Math.Pow(B.X - A.X, 2) + Math.Pow(B.Y - A.Y, 2));
        public double DistanceBetweenDPoints(Dpoint p1, Dpoint p2) => Math.Sqrt(Math.Pow(p2.x - p1.x, 2) + Math.Pow(p2.y - p1.y, 2));
        public double DistanceBerweenPointAndDpoint(Point P, Dpoint dp) => Math.Sqrt(Math.Pow(dp.x - P.X, 2) + Math.Pow(dp.y - P.Y, 2));

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

        public void AddAnchorPoint(Point NAP) => AnchorPoints.Add(NAP); //NAP = New Anchor Point

        public double GetLen()
        {
            double len = 0;
            for ( int i = 1; i < AnchorPoints.Count; i++)
                len += DistanceBetweenPoints(AnchorPoints[i - 1], AnchorPoints[i]);
            return len;
        }

        public Dpoint ToDpoint(Point P) => new Dpoint(P.X, P.Y);

        public void ExactExtraPointsClear() => Array.Clear(ExactExtraPoints, 0, NumOfExtraPoints);

        public static void Swap<T>(IList<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }

        public double[] GetSteps()
        {
            double len = GetLen();
            double[] res = new double[AnchorPoints.Count];
            for (int i = 0; i < AnchorPoints.Count; i++)
                res[i] = DistanceBetweenPoints(AnchorPoints[0], AnchorPoints[i]) / len;
            return res;
        }

        #region Split Trajectory
        public void SplitTrajectory(double step)
        {
            int index = 0;
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
                    ExactExtraPoints[index++] = new Dpoint(x, y);
                    ExtraPoints.Add(new Point((int)x, (int)y));
                    j++;
                }
                while (DistanceBerweenPointAndDpoint(AnchorPoints[i - 1], new Dpoint(x, y)) + step < dist);
            }
            ExactExtraPoints[index++] = ToDpoint(AnchorPoints[AnchorPoints.Count - 1]);
            ExtraPoints.Add(AnchorPoints[AnchorPoints.Count - 1]);
            NumOfExtraPoints = index;
        }
        public void SplitTrajectory(int k)
        {
            int index = 0;
            double step = GetLen() / k; // Шаг = длину всего пути делим на количество доп точек
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
                    ExactExtraPoints[index++] = new Dpoint(x, y);
                    ExtraPoints.Add(new Point((int)x, (int)y));
                    j++;
                }
                while (DistanceBerweenPointAndDpoint(AnchorPoints[i - 1], new Dpoint(x, y)) + step < dist);
            }
            ExactExtraPoints[index++] = ToDpoint(AnchorPoints[AnchorPoints.Count - 1]);
            ExtraPoints.Add(AnchorPoints[AnchorPoints.Count - 1]);
            NumOfExtraPoints = index;
            /*NumOfExtraPoints = k + AnchorPoints.Count;
            if (index == NumOfExtraPoints)
                ;//Ни одной точки не потерялось
            else
                ;//Потерялось NumOfExtraPoints - index точек
            */
            /* Другой вариант:
             * k = _k; 
            int i = 0;
            double[] steps = GetSteps();
            for (int j = 0; j < k; j++)
            {
                double step = (1.0 / k) * j;
                if (step > steps[i]) i++;
                if (i >= AnchorPoints.Count - 1) break;
                ExactExtraPoints[j].x = AnchorPoints[i].X + (step - steps[i]) / (steps[i + 1] - steps[i]) * (AnchorPoints[i + 1].X - AnchorPoints[i].X);
                ExactExtraPoints[j].y = AnchorPoints[i].Y + (step - steps[i]) / (steps[i + 1] - steps[i]) * (AnchorPoints[i + 1].Y - AnchorPoints[i].Y);
            }
            */
        }
        #endregion

        public void ExactExtraPointOffset(Point offset)
        {
            for (int i = 0; i < NumOfExtraPoints; i++)
            {
                ExactExtraPoints[i].x -= offset.X;
                ExactExtraPoints[i].y = offset.Y - ExactExtraPoints[i].y;
            }
        }
        public void TransExactExtraPoints(int k, double CoeftoRealW)
        {
            for ( int i = 0; i < k; i++)
            {
                ExactExtraPoints[i].x *= CoeftoRealW;
                ExactExtraPoints[i].y *= CoeftoRealW;
            }
        }
        public void TransferFunction(Point offset, double CoeftoRealW)
        {
            for (int i = 0; i < NumOfExtraPoints; i++)
            {
                ExactExtraPoints[i].x = ExactExtraPoints[i].x - offset.X;
                ExactExtraPoints[i].y = offset.Y - ExactExtraPoints[i].y;
                ExactExtraPoints[i].x *= CoeftoRealW;
                ExactExtraPoints[i].y *= CoeftoRealW;
            }
        }


        #region Shows and Hide
        public void ShowNextPoints(Graphics gr, int j)
        {
            for (int i = j; i < NumOfExtraPoints; i++)
                gr.FillEllipse(new SolidBrush(System.Drawing.Color.Red), new Rectangle(ExtraPoints[i].X - 3, ExtraPoints[i].Y - 3, 6, 6));
        }
        public void ShowPastPoints(Graphics gr, int j)
        {
            for (int i = 0; i < j; i++)
                gr.FillEllipse(new SolidBrush(System.Drawing.Color.Green), new Rectangle(ExtraPoints[i].X - 3, ExtraPoints[i].Y - 3, 6, 6));
        }
        public void ShowExtraPoints(Graphics gr)
        {
            for (int i = 0; i < NumOfExtraPoints; i++)
                gr.FillEllipse(new SolidBrush(System.Drawing.Color.Red), new Rectangle(ExtraPoints[i].X - 3, ExtraPoints[i].Y - 3, 6, 6));
            foreach (Point P in AnchorPoints)
            {
                gr.FillEllipse(new SolidBrush(System.Drawing.Color.Purple), new Rectangle(P.X - 6, P.Y - 6, 12, 12));
                int index = AnchorPoints.IndexOf(P);
                if (index > 9) gr.DrawString($"{index}", new Font("Arial", 8), new SolidBrush(Color.Yellow), P.X - 8, P.Y - 7);
                else gr.DrawString($"{index}", new Font("Arial", 9), new SolidBrush(Color.Yellow), P.X - 5, P.Y - 6);
            }
        }

        public void ShowExtraPoints(Graphics gr, Color color)
        {
            Pen pen = new Pen(Color.Purple, 4);
            SolidBrush brushMyColor = new SolidBrush(color);
            SolidBrush brush = new SolidBrush(System.Drawing.Color.Purple);
            for (int i = 0; i < NumOfExtraPoints - 1; i++)
            {
                //gr.DrawLine(pen, ExtraPoints[i], ExtraPoints[i + 1]);
                gr.FillEllipse(brush, new Rectangle(ExtraPoints[i].X - 6, ExtraPoints[i].Y - 6, 12, 12));
                gr.FillEllipse(brushMyColor, new Rectangle(ExtraPoints[i].X - 3, ExtraPoints[i].Y - 3, 6, 6));
            }

            gr.DrawLine(pen, ExtraPoints[NumOfExtraPoints - 2], ExtraPoints[NumOfExtraPoints - 1]);
            gr.FillEllipse(brush, new Rectangle(ExtraPoints[NumOfExtraPoints - 1].X - 6, ExtraPoints[NumOfExtraPoints - 1].Y - 6, 12, 12));
            gr.FillEllipse(brushMyColor, new Rectangle(ExtraPoints[NumOfExtraPoints - 1].X - 3, ExtraPoints[NumOfExtraPoints - 1].Y - 3, 6, 6));
            foreach (Point P in AnchorPoints)
            {
                gr.FillEllipse(brush, new Rectangle(P.X - 6, P.Y - 6, 12, 12));
                int index = AnchorPoints.IndexOf(P);
                if (index > 9) gr.DrawString($"{index}", new Font("Arial", 8), new SolidBrush(Color.Yellow), P.X - 8, P.Y - 7);
                else gr.DrawString($"{index}", new Font("Arial", 9), new SolidBrush(Color.Yellow), P.X - 5, P.Y - 6);
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
                    gr.DrawLine(new Pen(Color.LightBlue, 8), P, NextP);
                }
                gr.FillEllipse(new SolidBrush(System.Drawing.Color.LightBlue),
                        new Rectangle(P.X - 8, P.Y - 8, 16, 16));
            }
        }
        #endregion

        #region Interpolation
        public void Interpolate(ref LagrangeInterpolate[] interpolate)
        {
            int j = 0;
            for (int i = 0; i < AnchorPoints.Count - 1; i++)
            {
                int Xcurr = AnchorPoints[i].X;
                int Xnext = AnchorPoints[i + 1].X;
                interpolate[j].Add(Xcurr, AnchorPoints[i].Y);
                if (Xnext > Xcurr) continue;
                else if ( interpolate[0].GetCount() != 1) j++;
                do
                {
                    interpolate[j].Add(Xcurr, AnchorPoints[i].Y);
                    if (++i == AnchorPoints.Count - 1) break;
                    Xcurr = AnchorPoints[i].X;
                    Xnext = AnchorPoints[i + 1].X;
                } while (Xnext < Xcurr);
                interpolate[j].Add(Xcurr, AnchorPoints[i].Y);
                if (i == AnchorPoints.Count - 1) break;
                i--;
                j++;
            }
            interpolate[j].Add(AnchorPoints[AnchorPoints.Count - 1].X, AnchorPoints[AnchorPoints.Count - 1].Y);
        }
        public void Interpolate(LagrangeInterpolate[] interpolate)
        {
            int j = 0;
            for (int i = 0; i < NumOfExtraPoints - 1; i++)
            {
                double Xcurr = ExactExtraPoints[i].x;
                double Xnext = ExactExtraPoints[i + 1].x;
                if (Xcurr == Xnext) continue;
                ExactExtraPoints[i].y = interpolate[j].InterpolateX(Xcurr);
                ExtraPoints.Add(new Point((int)Xcurr, (int)ExactExtraPoints[i].y));
                if (Xnext > Xcurr) continue;
                else if(interpolate[0].GetCount() != 1) j++;
                do
                {
                    ExactExtraPoints[i].y = interpolate[j].InterpolateX(Xcurr);
                    ExtraPoints.Add(new Point((int)Xcurr, (int)ExactExtraPoints[i].y));
                    if (++i == NumOfExtraPoints - 1) break;
                    Xcurr = ExactExtraPoints[i].x;
                    Xnext = ExactExtraPoints[i + 1].x;
                    if (Xcurr == Xnext) i++;
                } while (Xnext < Xcurr);
                ExactExtraPoints[i].y = interpolate[j].InterpolateX(Xcurr);
                ExtraPoints.Add(new Point((int)Xcurr, (int)ExactExtraPoints[i].y));
                if (i == NumOfExtraPoints - 1) break;
                i--;
                j++;
            }
            ExactExtraPoints[NumOfExtraPoints - 1].y = interpolate[j].InterpolateX(ExactExtraPoints[NumOfExtraPoints - 1].x);
            ExtraPoints.Add(new Point((int)ExactExtraPoints[NumOfExtraPoints - 1].x, (int)ExactExtraPoints[NumOfExtraPoints - 1].y));
        }
        
        #endregion
    }
}
