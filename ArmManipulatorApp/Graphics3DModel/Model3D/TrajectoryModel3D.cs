namespace ArmManipulatorApp.Graphics3DModel.Model3D
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using System.Windows.Shapes;

    using ArmManipulatorApp.MathModel.Trajectory;

    using MainApp.Graphics3DModel.Model3D;

    public class TrajectoryModel3D
    {
        public Trajectory track;

        public List<ModelVisual3D> trackModelVisual3D;

        /// <summary>
        /// Задаёт отношение реальных физических величин манипулятора
        /// от пиксельной характеристики виртуальной 3D модели манипулятора: len(px) = coeff * len(cm)
        /// </summary>
        private double coeff;

        public TrajectoryModel3D(Trajectory track, double coeff = 1)
        {
            this.coeff = coeff;
            this.track = track;
            this.trackModelVisual3D = new List<ModelVisual3D>();

            foreach (var anchorPoint in this.track.AnchorPoints)
            {
                var anchorPointModelVisual3D = new ModelVisual3D();
                var anchorPointMeshGeometry3D = new MeshGeometry3D();
                var graphPoint = new Point3D(coeff * anchorPoint.X, coeff * anchorPoint.Y, coeff * anchorPoint.Z);
                MeshGeometry3DHelper.AddSphere(anchorPointMeshGeometry3D, graphPoint, 7, 8, 8);
                var anchorPointBrush = Brushes.GreenYellow;
                var anchorPointMaterial = new DiffuseMaterial(anchorPointBrush);
                var anchorPointGeometryModel = new GeometryModel3D(anchorPointMeshGeometry3D, anchorPointMaterial);
                anchorPointModelVisual3D.Content = anchorPointGeometryModel;
                //// Translate Transform for up/down anchor points in Edit Trajectory Mode.
                anchorPointModelVisual3D.Transform = new TranslateTransform3D();
                this.trackModelVisual3D.Add(anchorPointModelVisual3D);
            }

            foreach (var splitPoint in this.track.SplitPoints)
            {
                // TODO: change to lines except points
                var anchorPointModelVisual3D = new ModelVisual3D();
                var anchorPointMeshGeometry3D = new MeshGeometry3D();
                MeshGeometry3DHelper.AddSphere(anchorPointMeshGeometry3D, splitPoint, 4, 8, 8);
                var anchorPointBrush = Brushes.Red;
                var anchorPointMaterial = new DiffuseMaterial(anchorPointBrush);
                var anchorPointGeometryModel = new GeometryModel3D(anchorPointMeshGeometry3D, anchorPointMaterial);
                anchorPointModelVisual3D.Content = anchorPointGeometryModel;
                this.trackModelVisual3D.Add(anchorPointModelVisual3D);
            }
        }

        private void ShowSplitPath(List<Point3D> listSplitPathPoints)
        {
            foreach (var p in listSplitPathPoints)
            {
                var point = new MeshGeometry3D();
                //// AddSphere(point, new Point3D(p.X, p.Y, p.Z), 0.2, 8, 8);
                var pointBrush = Brushes.DarkRed;
                var pointMaterial = new DiffuseMaterial(pointBrush);
                var pathPointGeometryModel = new GeometryModel3D(point, pointMaterial);
                var pathPointModelVisual3D = new ModelVisual3D();
                pathPointModelVisual3D.Content = pathPointGeometryModel;
                pathPointModelVisual3D.Transform = new TranslateTransform3D();
                //// Viewport3D.Children.Add(pathPointModelVisual3D);
            }
        }

        public void AddAnchorPoint(Point3D newPoint)
        {

            //var pathPoint = new TrajectoryPoint();
            //pathPoint.center = new Point3D(X, Y, this.trajectoryPointsVisual3D.First().center.Z);

            //TODO: Extract to method next 8 line
            var point = new MeshGeometry3D();
            //MeshGeometry3DHelper.AddSphere(point, pathPoint.center, 0.2, 8, 8);
            var pointBrush = Brushes.Purple;
            var pointMaterial = new DiffuseMaterial(pointBrush);
            var pathPointGeometryModel = new GeometryModel3D(point, pointMaterial);
            var pathPointModelVisual3D = new ModelVisual3D();
            pathPointModelVisual3D.Content = pathPointGeometryModel;
            pathPointModelVisual3D.Transform = new TranslateTransform3D();

            //pathPoint.trajectoryModelVisual3D = pathPointModelVisual3D;
            //this.trajectoryPointsVisual3D.Add(pathPoint);

            //AddPathLine(this.trajectoryPointsVisual3D[this.trajectoryPointsVisual3D.Count - 2].center, this.trajectoryPointsVisual3D.Last().center);
        }

        //private void AddPathLine(Point3D start, Point3D end)
        //{
        //    TrajectoryLine trajectoryLine;
        //    trajectoryLine.start = start;
        //    trajectoryLine.end = end;

        //    //TODO: Extract to method next 8 line
        //    var line = new MeshGeometry3D();
        //    LineByTwoPoints(line, trajectoryLine.start, trajectoryLine.end, 0.13);
        //    var lineBrush = Brushes.MediumPurple;
        //    var lineMaterial = new DiffuseMaterial(lineBrush);
        //    var lineGeometryModel = new GeometryModel3D(line, lineMaterial);
        //    var pathLineModelVisual3D = new ModelVisual3D();
        //    pathLineModelVisual3D.Content = lineGeometryModel;
        //    Viewport3D.Children.Add(pathLineModelVisual3D);

        //    trajectoryLine.lineModelVisual3D = pathLineModelVisual3D;
        //    this.trajectoryLinesVisual3D.Add(trajectoryLine);
        //    AddRotateTransform(trajectoryLine);
        //}

        //private void AddRotateTransform(TrajectoryLine trajectoryLine)
        //{
        //    var transformGroup = new Transform3DGroup();

        //    var rotateTransformByStart = new RotateTransform3D();
        //    rotateTransformByStart.CenterX = trajectoryLine.start.X;
        //    rotateTransformByStart.CenterY = trajectoryLine.start.Y;
        //    rotateTransformByStart.CenterZ = trajectoryLine.start.Z;

        //    var rotateTransformByEnd = new RotateTransform3D();
        //    rotateTransformByEnd.CenterX = trajectoryLine.end.X;
        //    rotateTransformByEnd.CenterY = trajectoryLine.end.Y;
        //    rotateTransformByEnd.CenterZ = trajectoryLine.end.Z;

        //    var angleRotationByStart = new AxisAngleRotation3D
        //    {
        //        Axis = Vector3D.CrossProduct(
        //            new Vector3D(trajectoryLine.end.X - trajectoryLine.start.X,
        //                         trajectoryLine.end.Y - trajectoryLine.start.Y,
        //                         trajectoryLine.end.Z - trajectoryLine.start.Z),
        //            new Vector3D(0, 1, 0)),
        //        Angle = 0
        //    };

        //    var angleRotationByEnd = new AxisAngleRotation3D
        //    {
        //        Axis = Vector3D.CrossProduct(
        //             new Vector3D(0, 1, 0),
        //             new Vector3D(trajectoryLine.end.X - trajectoryLine.start.X,
        //                          trajectoryLine.end.Y - trajectoryLine.start.Y,
        //                          trajectoryLine.end.Z - trajectoryLine.start.Z)),
        //        Angle = 0
        //    };

        //    rotateTransformByStart.Rotation = angleRotationByStart;
        //    transformGroup.Children.Add(rotateTransformByStart);

        //    rotateTransformByEnd.Rotation = angleRotationByEnd;
        //    transformGroup.Children.Add(rotateTransformByEnd);

        //    trajectoryLine.lineModelVisual3D.Transform = transformGroup;
        //}


        // TODO: remove it (used for 2D Graphics)
        //public void ShowNextPoints(Graphics gr, int j)
        //{
        //    for (var i = j; i < NumOfExtraPoints; i++)
        //        gr.FillEllipse(new SolidBrush(Color.Red), new Rectangle(SplitPoints[i].X - 3, SplitPoints[i].Y - 3, 6, 6));
        //}

        //public void ShowPastPoints(Graphics gr, int j)
        //{
        //    for (var i = 0; i < j; i++)
        //        gr.FillEllipse(new SolidBrush(Color.Green), new Rectangle(SplitPoints[i].X - 3, SplitPoints[i].Y - 3, 6, 6));
        //}

        //public void ShowExtraPoints(Graphics gr)
        //{
        //    for (var i = 0; i < NumOfExtraPoints; i++)
        //        gr.FillEllipse(new SolidBrush(Color.Red), new Rectangle(SplitPoints[i].X - 3, SplitPoints[i].Y - 3, 6, 6));
        //    foreach (var P in AnchorPoints)
        //    {
        //        gr.FillEllipse(new SolidBrush(Color.Purple), new Rectangle(P.X - 6, P.Y - 6, 12, 12));
        //        var index = AnchorPoints.IndexOf(P);
        //        if (index > 9) gr.DrawString($"{index}", new Font("Arial", 8), new SolidBrush(Color.Yellow), P.X - 8, P.Y - 7);
        //        else gr.DrawString($"{index}", new Font("Arial", 9), new SolidBrush(Color.Yellow), P.X - 5, P.Y - 6);
        //    }
        //}

        //public void ShowExtraPoints(Graphics gr, Color color)
        //{
        //    var pen = new Pen(Color.Purple, 4);
        //    var brushMyColor = new SolidBrush(color);
        //    var brush = new SolidBrush(Color.Purple);
        //    for (var i = 0; i < NumOfExtraPoints - 1; i++)
        //    {
        //        //gr.DrawLine(pen, SplitPoints[i], SplitPoints[i + 1]);
        //        gr.FillEllipse(brush, new Rectangle(SplitPoints[i].X - 6, SplitPoints[i].Y - 6, 12, 12));
        //        gr.FillEllipse(brushMyColor, new Rectangle(SplitPoints[i].X - 3, SplitPoints[i].Y - 3, 6, 6));
        //    }

        //    gr.DrawLine(pen, SplitPoints[NumOfExtraPoints - 2], SplitPoints[NumOfExtraPoints - 1]);
        //    gr.FillEllipse(brush, new Rectangle(SplitPoints[NumOfExtraPoints - 1].X - 6, SplitPoints[NumOfExtraPoints - 1].Y - 6, 12, 12));
        //    gr.FillEllipse(brushMyColor, new Rectangle(SplitPoints[NumOfExtraPoints - 1].X - 3, SplitPoints[NumOfExtraPoints - 1].Y - 3, 6, 6));
        //    foreach (var P in AnchorPoints)
        //    {
        //        gr.FillEllipse(brush, new Rectangle(P.X - 6, P.Y - 6, 12, 12));
        //        var index = AnchorPoints.IndexOf(P);
        //        if (index > 9) gr.DrawString($"{index}", new Font("Arial", 8), new SolidBrush(Color.Yellow), P.X - 8, P.Y - 7);
        //        else gr.DrawString($"{index}", new Font("Arial", 9), new SolidBrush(Color.Yellow), P.X - 5, P.Y - 6);
        //    }
        //}

        //public void Show(Graphics gr)
        //{
        //    foreach (var P in AnchorPoints)
        //    {
        //        var index = AnchorPoints.IndexOf(P);
        //        if (index + 1 < AnchorPoints.Count)
        //        {
        //            var NextP = AnchorPoints[index + 1];
        //            gr.DrawLine(new Pen(Color.Purple, 4), P, NextP);
        //        }
        //        gr.FillEllipse(new SolidBrush(Color.Purple),
        //                new Rectangle(P.X - 6, P.Y - 6, 12, 12));
        //        if (index > 9)
        //            gr.DrawString($"{index}", new Font("Arial", 8),
        //            new SolidBrush(Color.Yellow), P.X - 8, P.Y - 7);
        //        else
        //            gr.DrawString($"{index}", new Font("Arial", 9),
        //            new SolidBrush(Color.Yellow), P.X - 5, P.Y - 6);
        //    }
        //}
        //public void Hide(Graphics gr)
        //{
        //    foreach (var P in AnchorPoints)
        //    {
        //        var index = AnchorPoints.IndexOf(P);
        //        if (index + 1 < AnchorPoints.Count)
        //        {
        //            var NextP = AnchorPoints[index + 1];
        //            gr.DrawLine(new Pen(Color.LightBlue, 8), P, NextP);
        //        }
        //        gr.FillEllipse(new SolidBrush(Color.LightBlue),
        //                new Rectangle(P.X - 8, P.Y - 8, 16, 16));
        //    }
        //}
    }
}
