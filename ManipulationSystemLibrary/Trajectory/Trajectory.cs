﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media.Media3D;

namespace ManipulationSystemLibrary
{
    public class Trajectory
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

        public Point3D[] ExactExtra; // Точные дополнительные точки

        public int NumOfExtraPoints;

        public bool IsSplit;

        public Trajectory()
        {
            IsSplit = false;
            AnchorPoints = new List<Point>();
            ExtraPoints = new List<Point>();
            ExactExtra = new Point3D[3333]; // 3333 is magic number // p.s. it is the max number of ExactExtraPoint
            NumOfExtraPoints = 0;
        }

        public Trajectory(Point StartPoint)
        {
            IsSplit = false;
            AnchorPoints = new List<Point> { StartPoint };
            ExtraPoints = new List<Point>();
            ExactExtra = new Point3D[1024];
            NumOfExtraPoints = 0;
        }

        public void Clear()
        {
            IsSplit = false;
            AnchorPoints.Clear();
            ExtraPoints.Clear();
            Array.Clear(ExactExtra, 0, ExactExtra.Length);
        }

        public double DistanceBetweenPoints(Point A, Point B) => Math.Sqrt(Math.Pow(B.X - A.X, 2) + Math.Pow(B.Y - A.Y, 2));
        public double DistanceBetweenPoints(Point3D p1, Point3D p2) => Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        public double DistanceBerweenPoints(Point P, Point3D dp) => Math.Sqrt(Math.Pow(dp.X - P.X, 2) + Math.Pow(dp.Y - P.Y, 2));

        public int NearestPointIndex(Point O) //Возвращает индекс ближайшей опорной точки к точке О
        {
            int index = -1;
            double MinDist = 8192;
            foreach (var P in AnchorPoints)
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

        public Point3D ToPoint3D(Point P) => new Point3D(P.X, P.Y, 0);

        public void ExactExtraPointsClear() => Array.Clear(ExactExtra, 0, NumOfExtraPoints);

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
                    ExactExtra[index++] = new Point3D(x, y, 0);
                    ExtraPoints.Add(new Point((int)x, (int)y));
                    j++;
                }
                while (DistanceBerweenPoints(AnchorPoints[i - 1], new Point3D(x, y, 0)) + step < dist);
            }
            ExactExtra[index++] = ToPoint3D(AnchorPoints[AnchorPoints.Count - 1]);
            ExtraPoints.Add(AnchorPoints[AnchorPoints.Count - 1]);
            NumOfExtraPoints = index;
            IsSplit = true;
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
                    ExactExtra[index++] = new Point3D(x, y, 0);
                    ExtraPoints.Add(new Point((int)x, (int)y));
                    j++;
                }
                while (DistanceBerweenPoints(AnchorPoints[i - 1], new Point3D(x, y, 0)) + step < dist);
            }
            ExactExtra[index++] = ToPoint3D(AnchorPoints[AnchorPoints.Count - 1]);
            ExtraPoints.Add(AnchorPoints[AnchorPoints.Count - 1]);
            NumOfExtraPoints = index;
            IsSplit = true;
        }

        #endregion

        public void ExactExtraPointOffset(Point offset)
        {
            for (int i = 0; i < NumOfExtraPoints; i++)
            {
                ExactExtra[i].X -= offset.X;
                ExactExtra[i].Y = offset.Y - ExactExtra[i].Y;
            }
        }
        public void TransExactExtraPoints(int k, double CoeftoRealW)
        {
            for ( int i = 0; i < k; i++)
            {
                ExactExtra[i].X *= CoeftoRealW;
                ExactExtra[i].Y *= CoeftoRealW;
            }
        }
        public void TransferFunction(Point offset, double CoeftoRealW)
        {
            for (int i = 0; i < NumOfExtraPoints; i++)
            {
                ExactExtra[i].X = ExactExtra[i].X - offset.X;
                ExactExtra[i].Y = offset.Y - ExactExtra[i].Y;
                ExactExtra[i].X *= CoeftoRealW;
                ExactExtra[i].Y *= CoeftoRealW;
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
                double Xcurr = ExactExtra[i].X;
                double Xnext = ExactExtra[i + 1].X;
                if (Xcurr == Xnext) continue;
                ExactExtra[i].Y = interpolate[j].InterpolateX(Xcurr);
                ExtraPoints.Add(new Point((int)Xcurr, (int)ExactExtra[i].Y));
                if (Xnext > Xcurr) continue;
                else if(interpolate[0].GetCount() != 1) j++;
                do
                {
                    ExactExtra[i].Y = interpolate[j].InterpolateX(Xcurr);
                    ExtraPoints.Add(new Point((int)Xcurr, (int)ExactExtra[i].Y));
                    if (++i == NumOfExtraPoints - 1) break;
                    Xcurr = ExactExtra[i].X;
                    Xnext = ExactExtra[i + 1].X;
                    if (Xcurr == Xnext) i++;
                } while (Xnext < Xcurr);
                ExactExtra[i].Y = interpolate[j].InterpolateX(Xcurr);
                ExtraPoints.Add(new Point((int)Xcurr, (int)ExactExtra[i].Y));
                if (i == NumOfExtraPoints - 1) break;
                i--;
                j++;
            }
            ExactExtra[NumOfExtraPoints - 1].Y = interpolate[j].InterpolateX(ExactExtra[NumOfExtraPoints - 1].X);
            ExtraPoints.Add(new Point((int)ExactExtra[NumOfExtraPoints - 1].X, (int)ExactExtra[NumOfExtraPoints - 1].Y));
        }
        
        #endregion
    }
}
