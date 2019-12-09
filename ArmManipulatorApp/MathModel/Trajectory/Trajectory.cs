namespace ArmManipulatorApp.MathModel.Trajectory
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media.Media3D;

    public class Trajectory
    {
        public List<Point3D> AnchorPoints;
        public List<Point3D> SplitPoints;

        public double Length;
        public bool IsSplit;

        public Trajectory()
        {
            this.IsSplit = false;
            this.AnchorPoints = new List<Point3D>();
            this.SplitPoints = new List<Point3D>();
        }

        public Trajectory(Point3D StartPoint)
        {
            this.IsSplit = false;
            this.AnchorPoints = new List<Point3D> { StartPoint };
            this.SplitPoints = new List<Point3D>();
        }

        public Trajectory(List<Point3D> anchorPoints, List<Point3D> splitPoints = null)
        {
            this.AnchorPoints = anchorPoints;
            this.SplitPoints = splitPoints;
            this.IsSplit = splitPoints != null;
        }

        public int NearestPointIndex(Point3D O) //Возвращает индекс ближайшей опорной точки к точке О
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

        //NAP = New Anchor Point
        public void AddAnchorPoint(Point3D NAP)
        {
            var lastPoint = this.AnchorPoints.Last();
            this.AnchorPoints.Add(NAP);
            this.Length += (lastPoint - this.AnchorPoints.Last()).Length;
        }

        public void AnchorPointOffsetZ(int index, double deltaZ)
        {
            if (index == 0 || index > this.AnchorPoints.Count - 1)
            {
                throw new ArgumentOutOfRangeException();
            }

            var oldPoint = this.AnchorPoints[index];
            this.AnchorPoints[index] = new Point3D(oldPoint.X, oldPoint.Y, oldPoint.Z + deltaZ);

            var oldLengthLeft = (this.AnchorPoints[index - 1] - oldPoint).Length;
            var newLengthLeft = (this.AnchorPoints[index - 1] - this.AnchorPoints[index]).Length;
            if (index == this.AnchorPoints.Count - 1)
            {
                this.Length = this.Length - oldLengthLeft + newLengthLeft;
            }
            else
            {
                var oldLengthRight = (oldPoint - this.AnchorPoints[index + 1]).Length;
                var newLengthRight = (this.AnchorPoints[index] - this.AnchorPoints[index + 1]).Length;
                this.Length = this.Length - oldLengthLeft - oldLengthRight + newLengthLeft + newLengthRight;
            }
        }

        public double DistanceBetweenPoints(Point3D p1, Point3D p2) => Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));

        public double GetLen()
        {
            double len = 0;
            for ( var i = 1; i < AnchorPoints.Count; i++)
                len += DistanceBetweenPoints(AnchorPoints[i - 1], AnchorPoints[i]);
            return len;
        }

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

        public void SplitTrack(DoWorkEventArgs e, object sender, double step)
        {
            Console.WriteLine("Try to divide track with step " + step);
            this.SplitPoints.Clear();
            for (var i = 1; i < this.AnchorPoints.Count; i++)
            {
                var j = 0;
                var x = this.AnchorPoints[i - 1].X;
                var y = this.AnchorPoints[i - 1].Y;
                var z = this.AnchorPoints[i - 1].Z;
                var dist = (this.AnchorPoints[i - 1] - this.AnchorPoints[i]).Length;
                do
                {
                    var lambda = (step * j) / (dist - step * j);
                    x = (this.AnchorPoints[i - 1].X + lambda * this.AnchorPoints[i].X) / (1 + lambda);
                    y = (this.AnchorPoints[i - 1].Y + lambda * this.AnchorPoints[i].Y) / (1 + lambda);
                    z = (this.AnchorPoints[i - 1].Z + lambda * this.AnchorPoints[i].Z) / (1 + lambda);
                    this.SplitPoints.Add(new Point3D(x, y, z));
                    j++;
                }
                while ((this.AnchorPoints[i - 1] - new Point3D(x, y, z)).Length + step < dist);
            }

            this.SplitPoints.Add(this.AnchorPoints[this.AnchorPoints.Count - 1]);
            ((BackgroundWorker)sender).ReportProgress(1);

            Console.WriteLine("Track divided successfully");
            Console.WriteLine("----Number of all points is " + this.SplitPoints.Count);
        }

        public void SplitTrack(DoWorkEventArgs e, object sender, int numSplitPoint)
        {
            throw new NotImplementedException();
            //((BackgroundWorker)sender).ReportProgress(0);

            //if (((BackgroundWorker)sender).CancellationPending == true)
            //{
            //    e.Cancel = true;
            //    return;
            //}
        }

        // TODO: remove it 2D realization
        //public void SplitTrajectory(double step)
        //{
        //    var index = 0;
        //    for (var i = 1; i < AnchorPoints.Count; i++)
        //    {
        //        var j = 0;
        //        double lambda = 0;
        //        double x = AnchorPoints[i - 1].X;
        //        double y = AnchorPoints[i - 1].Y;
        //        var dist = DistanceBetweenPoints(AnchorPoints[i - 1], AnchorPoints[i]);
        //        do
        //        {
        //            lambda = (step * j) / (dist - step * j);
        //            x = (AnchorPoints[i - 1].X + lambda * AnchorPoints[i].X) / (1 + lambda);
        //            y = (AnchorPoints[i - 1].Y + lambda * AnchorPoints[i].Y) / (1 + lambda);
        //            ExactExtra[index++] = new Point3D(x, y, 0);
        //            SplitPoints.Add(new Point3D((int)x, (int)y, 0));
        //            j++;
        //        }
        //        while (DistanceBetweenPoints(AnchorPoints[i - 1], new Point3D(x, y, 0)) + step < dist);
        //    }
        //    ExactExtra[index++] = AnchorPoints[AnchorPoints.Count - 1];
        //    SplitPoints.Add(AnchorPoints[AnchorPoints.Count - 1]);
        //    NumOfExtraPoints = index;
        //    IsSplit = true;
        //}
        
        // TODO: remove it 2D realization
        //public void SplitTrajectory(int k)
        //{
        //    var index = 0;
        //    var step = GetLen() / k; // Шаг = длину всего пути делим на количество доп точек
        //    for (var i = 1; i < AnchorPoints.Count; i++)
        //    {
        //        var j = 0;
        //        double lambda = 0;
        //        double x = AnchorPoints[i - 1].X;
        //        double y = AnchorPoints[i - 1].Y;
        //        var dist = DistanceBetweenPoints(AnchorPoints[i - 1], AnchorPoints[i]);
        //        do
        //        {
        //            lambda = (step * j) / (dist - step * j);
        //            x = (AnchorPoints[i - 1].X + lambda * AnchorPoints[i].X) / (1 + lambda);
        //            y = (AnchorPoints[i - 1].Y + lambda * AnchorPoints[i].Y) / (1 + lambda);
        //            ExactExtra[index++] = new Point3D(x, y, 0);
        //            SplitPoints.Add(new Point3D((int)x, (int)y, 0));
        //            j++;
        //        }

        //        while (DistanceBetweenPoints(AnchorPoints[i - 1], new Point3D(x, y, 0)) + step < dist);
        //    }
        //    ExactExtra[index++] = AnchorPoints[AnchorPoints.Count - 1];
        //    SplitPoints.Add(AnchorPoints[AnchorPoints.Count - 1]);
        //    NumOfExtraPoints = index;
        //    IsSplit = true;
        //}

        #endregion

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

        //public void Interpolate(LagrangeInterpolate[] interpolate)
        //{
        //    var j = 0;
        //    for (var i = 0; i < NumOfExtraPoints - 1; i++)
        //    {
        //        var Xcurr = ExactExtra[i].X;
        //        var Xnext = ExactExtra[i + 1].X;
        //        if (Xcurr == Xnext) continue;
        //        ExactExtra[i].Y = interpolate[j].InterpolateX(Xcurr);
        //        SplitPoints.Add(new Point3D((int)Xcurr, (int)ExactExtra[i].Y, 0));
        //        if (Xnext > Xcurr) continue;
        //        if(interpolate[0].GetCount() != 1) j++;
        //        do
        //        {
        //            ExactExtra[i].Y = interpolate[j].InterpolateX(Xcurr);
        //            SplitPoints.Add(new Point3D((int)Xcurr, (int)ExactExtra[i].Y, 0));
        //            if (++i == NumOfExtraPoints - 1) break;
        //            Xcurr = ExactExtra[i].X;
        //            Xnext = ExactExtra[i + 1].X;
        //            if (Xcurr == Xnext) i++;
        //        } while (Xnext < Xcurr);
        //        ExactExtra[i].Y = interpolate[j].InterpolateX(Xcurr);
        //        SplitPoints.Add(new Point3D((int)Xcurr, (int)ExactExtra[i].Y, 0));
        //        if (i == NumOfExtraPoints - 1) break;
        //        i--;
        //        j++;
        //    }
        //    ExactExtra[NumOfExtraPoints - 1].Y = interpolate[j].InterpolateX(ExactExtra[NumOfExtraPoints - 1].X);
        //    SplitPoints.Add(new Point3D((int)ExactExtra[NumOfExtraPoints - 1].X, (int)ExactExtra[NumOfExtraPoints - 1].Y, 0));
        //}
        
        #endregion
    }
}
