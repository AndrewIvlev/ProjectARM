namespace ManipulationSystemLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media.Media3D;

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
            var index = -1;
            double MinDist = 8192;
            foreach (var P in AnchorPoints)
            {
                var dist = DistanceBetweenPoints(O, P);
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
            for ( var i = 1; i < AnchorPoints.Count; i++)
                len += DistanceBetweenPoints(AnchorPoints[i - 1], AnchorPoints[i]);
            return len;
        }

        public Point3D ToPoint3D(Point P) => new Point3D(P.X, P.Y, 0);

        public void ExactExtraPointsClear() => Array.Clear(ExactExtra, 0, NumOfExtraPoints);

        public static void Swap<T>(IList<T> list, int indexA, int indexB)
        {
            var tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }

        public double[] GetSteps()
        {
            var len = GetLen();
            var res = new double[AnchorPoints.Count];
            for (var i = 0; i < AnchorPoints.Count; i++)
                res[i] = DistanceBetweenPoints(AnchorPoints[0], AnchorPoints[i]) / len;
            return res;
        }

        #region Split Trajectory

        public void SplitTrajectory(double step)
        {
            var index = 0;
            for (var i = 1; i < AnchorPoints.Count; i++)
            {
                var j = 0;
                double lambda = 0;
                double x = AnchorPoints[i - 1].X;
                double y = AnchorPoints[i - 1].Y;
                var dist = DistanceBetweenPoints(AnchorPoints[i - 1], AnchorPoints[i]);
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
            var index = 0;
            var step = GetLen() / k; // Шаг = длину всего пути делим на количество доп точек
            for (var i = 1; i < AnchorPoints.Count; i++)
            {
                var j = 0;
                double lambda = 0;
                double x = AnchorPoints[i - 1].X;
                double y = AnchorPoints[i - 1].Y;
                var dist = DistanceBetweenPoints(AnchorPoints[i - 1], AnchorPoints[i]);
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

        // TODO: remove it
        //private void SplitPath(List<Point3D> listPathPoints, double step)
        //{
        //    var index = 0;
        //    this.listSplitTrajectoryPoints = new List<Point3D>();
        //    for (var i = 1; i < listPathPoints.Count; i++)
        //    {
        //        var j = 0;
        //        double lambda = 0;
        //        var x = listPathPoints[i - 1].X;
        //        var y = listPathPoints[i - 1].Y;
        //        var z = listPathPoints[i - 1].Z;
        //        var dist = (listPathPoints[i - 1] - listPathPoints[i]).Length;
        //        do
        //        {
        //            lambda = (step * j) / (dist - step * j);
        //            x = (listPathPoints[i - 1].X + lambda * listPathPoints[i].X) / (1 + lambda);
        //            y = (listPathPoints[i - 1].Y + lambda * listPathPoints[i].Y) / (1 + lambda);
        //            z = (listPathPoints[i - 1].Z + lambda * listPathPoints[i].Z) / (1 + lambda);
        //            this.listSplitTrajectoryPoints.Add(new Point3D(x, y, z));
        //            index++;
        //            j++;
        //        }
        //        while ((listPathPoints[i - 1] - new Point3D(x, y, z)).Length + step < dist);
        //    }

        //    index++;
        //    this.listSplitTrajectoryPoints.Add(listPathPoints[listPathPoints.Count - 1]);

        //    ShowSplitPath(this.listSplitTrajectoryPoints);
        //}

        #endregion

        public void ExactExtraPointOffset(Point offset)
        {
            for (var i = 0; i < NumOfExtraPoints; i++)
            {
                ExactExtra[i].X -= offset.X;
                ExactExtra[i].Y = offset.Y - ExactExtra[i].Y;
            }
        }

        public void TransExactExtraPoints(int k, double CoeftoRealW)
        {
            for (var i = 0; i < k; i++)
            {
                ExactExtra[i].X *= CoeftoRealW;
                ExactExtra[i].Y *= CoeftoRealW;
            }
        }
        
        public void TransferFunction(Point offset, double CoeftoRealW)
        {
            for (var i = 0; i < NumOfExtraPoints; i++)
            {
                ExactExtra[i].X = ExactExtra[i].X - offset.X;
                ExactExtra[i].Y = offset.Y - ExactExtra[i].Y;
                ExactExtra[i].X *= CoeftoRealW;
                ExactExtra[i].Y *= CoeftoRealW;
            }
        }

        #region Interpolation

        public void Interpolate(ref LagrangeInterpolate[] interpolate)
        {
            var j = 0;
            for (var i = 0; i < AnchorPoints.Count - 1; i++)
            {
                var Xcurr = AnchorPoints[i].X;
                var Xnext = AnchorPoints[i + 1].X;
                interpolate[j].Add(Xcurr, AnchorPoints[i].Y);
                if (Xnext > Xcurr) continue;
                if ( interpolate[0].GetCount() != 1) j++;
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
            var j = 0;
            for (var i = 0; i < NumOfExtraPoints - 1; i++)
            {
                var Xcurr = ExactExtra[i].X;
                var Xnext = ExactExtra[i + 1].X;
                if (Xcurr == Xnext) continue;
                ExactExtra[i].Y = interpolate[j].InterpolateX(Xcurr);
                ExtraPoints.Add(new Point((int)Xcurr, (int)ExactExtra[i].Y));
                if (Xnext > Xcurr) continue;
                if(interpolate[0].GetCount() != 1) j++;
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
