namespace ArmManipulatorApp.MathModel.Trajectory
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media.Media3D;

    using ArmManipulatorArm.MathModel;

    public class Trajectory
    {
        public List<Point3D> AnchorPoints;
        public List<Point3D> SplitPoints;

        /// <summary>
        /// Function for interpolation
        /// P(s) = (X(s), Y(s), Z(s))
        /// </summary>
        public List<double> StepsValue;

        public double Length;
        public bool IsSplit;

        public Trajectory()
        {
            this.IsSplit = false;
            this.AnchorPoints = new List<Point3D>();
            this.SplitPoints = new List<Point3D>();
            this.StepsValue = new List<double>();
        }

        public Trajectory(Point3D StartPoint)
        {
            this.IsSplit = false;
            this.AnchorPoints = new List<Point3D> { StartPoint };
            this.SplitPoints = new List<Point3D>();
            this.StepsValue = new List<double>();
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
            var minDist = 8192.0;
            foreach (var anchorPoint in this.AnchorPoints)
            {
                var dist = MathFunctions.NormaVector(O - anchorPoint);
                if (dist < minDist)
                {
                    minDist = dist;
                    index = AnchorPoints.IndexOf(anchorPoint);
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
        
        public double GetLen()
        {
            double len = 0;
            for (var i = 1; i < this.AnchorPoints.Count; i++)
            {
                len += MathFunctions.NormaVector(this.AnchorPoints[i - 1] - this.AnchorPoints[i]);
            }

            return len;
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

        public List<double> GetListOfDistanceBetweenSplitPoints()
        {
            var list = new List<double>();
            for (var i = 0; i < this.SplitPoints.Count - 1; i++)
            {
                list.Add(MathFunctions.NormaVector(this.SplitPoints[i] - this.SplitPoints[i + 1]));
            }

            return list;
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

        public void Calc_Steps()
        {
            this.StepsValue.Clear();
            var len = this.GetLen();
            for (var i = 0; i < this.AnchorPoints.Count; i++)
            {
                var step = 0.0;
                for (var j = 0; j < i; j++)
                {
                    step += MathFunctions.NormaVector(this.AnchorPoints[j] - this.AnchorPoints[j + 1]);
                }

                this.StepsValue.Add(step / len);
            }
        }

        public void SplitViaInterpolation(DoWorkEventArgs e, object sender, double delta)
        {
            this.Calc_Steps();
            this.SplitPoints.Clear();
            var stepK = this.StepsValue[0];
            while (stepK < this.StepsValue[this.AnchorPoints.Count - 1])
            {
                this.SplitPoints.Add(this.LagrangePolynomial(stepK));
                var deltaStep = delta / MathFunctions.NormaVector((Vector3D)this.DerivativeLagrangePolynomial(stepK));
                stepK += deltaStep;
            }

            ((BackgroundWorker)sender).ReportProgress(1);

            Console.WriteLine("Track divided successfully");
            Console.WriteLine("----Number of all points is " + this.SplitPoints.Count);
        }

        public Point3D LagrangePolynomial(double s)
        {
            var res = new Point3D();
            for (var i = 0; i < this.AnchorPoints.Count; i++)
            {
                res.X = this.LagrangePolynomial_X(s);
                res.Y = this.LagrangePolynomial_Y(s);
                res.Z = this.LagrangePolynomial_Z(s);
            }

            return res;
        }

        public Point3D DerivativeLagrangePolynomial(double s)
        {
            var res = new Point3D();
            for (var i = 0; i < this.AnchorPoints.Count; i++)
            {
                res.X = this.LagrangePolynomial_X_dS(s);
                res.Y = this.LagrangePolynomial_Y_dS(s);
                res.Z = this.LagrangePolynomial_Z_dS(s);
            }

            return res;
        }

        public double LagrangePolynomial_X(double s)
        {
            var res = 0.0;
            for (var i = 0; i < this.AnchorPoints.Count; i++)
            {
                var p = 1.0;
                for (var j = 0; j < this.AnchorPoints.Count; j++)
                {
                    if (j != i)
                    {
                        p *= (s - this.StepsValue[j]) / (this.StepsValue[i] - this.StepsValue[j]);
                    }
                }

                res += this.AnchorPoints[i].X * p;
            }

            return res;
        }

        public double LagrangePolynomial_Y(double s)
        {
            var res = 0.0;
            for (var i = 0; i < this.AnchorPoints.Count; i++)
            {
                var p = 1.0;
                for (var j = 0; j < this.AnchorPoints.Count; j++)
                {
                    if (j != i)
                    {
                        p *= (s - this.StepsValue[j]) / (this.StepsValue[i] - this.StepsValue[j]);
                    }
                }

                res += this.AnchorPoints[i].Y * p;
            }

            return res;
        }

        public double LagrangePolynomial_Z(double s)
        {
            var res = 0.0;
            for (var i = 0; i < this.AnchorPoints.Count; i++)
            {
                var p = 1.0;
                for (var j = 0; j < this.AnchorPoints.Count; j++)
                {
                    if (j != i)
                    {
                        p *= (s - this.StepsValue[j]) / (this.StepsValue[i] - this.StepsValue[j]);
                    }
                }

                res += this.AnchorPoints[i].Z * p;
            }

            return res;
        }

        public double LagrangePolynomial_X_dS(double s)
        {
            var res = 0.0;
            for (var k = 0; k < this.AnchorPoints.Count; k++)
            {
                var p = 0.0;
                for (var i = 0; i < this.AnchorPoints.Count; i++)
                {
                    if (i != k)
                    {
                        var d = 1.0;
                        for (var j = 0; j < this.AnchorPoints.Count; j++)
                        {
                            if (j != k)
                            {
                                if (j != i)
                                {
                                    d *= s - this.StepsValue[j];
                                }
                            }
                        }

                        p += d;
                    }
                }

                for (var j = 0; j < this.AnchorPoints.Count; j++)
                {
                    if (j != k)
                    {
                        p /= this.StepsValue[k] - this.StepsValue[j];
                    }
                }

                res += this.AnchorPoints[k].X * p;
            }
                
            return res;
        }

        public double LagrangePolynomial_Y_dS(double s)
        {
            var res = 0.0;
            for (var k = 0; k < this.AnchorPoints.Count; k++)
            {
                var p = 0.0;
                for (var i = 0; i < this.AnchorPoints.Count; i++)
                {
                    if (i != k)
                    {
                        var d = 1.0;
                        for (var j = 0; j < this.AnchorPoints.Count; j++)
                        {
                            if (j != k)
                            {
                                if (j != i)
                                {
                                    d *= s - this.StepsValue[j];
                                }
                            }
                        }

                        p += d;
                    }
                }

                for (var j = 0; j < this.AnchorPoints.Count; j++)
                {
                    if (j != k)
                    {
                        p /= this.StepsValue[k] - this.StepsValue[j];
                    }
                }

                res += this.AnchorPoints[k].Y * p;
            }

            return res;
        }

        public double LagrangePolynomial_Z_dS(double s)
        {
            var res = 0.0;
            for (var k = 0; k < this.AnchorPoints.Count; k++)
            {
                var p = 0.0;
                for (var i = 0; i < this.AnchorPoints.Count; i++)
                {
                    if (i != k)
                    {
                        var d = 1.0;
                        for (var j = 0; j < this.AnchorPoints.Count; j++)
                        {
                            if (j != k)
                            {
                                if (j != i)
                                {
                                    d *= s - this.StepsValue[j];
                                }
                            }
                        }

                        p += d;
                    }
                }

                for (var j = 0; j < this.AnchorPoints.Count; j++)
                {
                    if (j != k)
                    {
                        p /= this.StepsValue[k] - this.StepsValue[j];
                    }
                }

                res += this.AnchorPoints[k].Z * p;
            }

            return res;
        }

        #endregion
    }
}
