using System;
using System.Drawing;
using System.Collections.Generic;
using ArmManipulatorApp.MathModel.Trajectory;

namespace ArmManipulatorApp.Graphics3DModel.Model3D
{
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using System.Windows.Shapes;

    using ArmManipulatorApp.Common;

    public class TrajectoryModel3D : Notifier
    {
        public Trajectory track;

        public ModelVisual3D trajectoryPointCursor;
        private List<Point3D> listTrajectoryPoints; // list for spliting trajectory
        private class TrajectoryPoint
        {
            public ModelVisual3D trajectoryModelVisual3D;
            public Point3D center;

            public void SetY(double y)
            {
                center.Y = y;
            }
        }
        private List<TrajectoryPoint> trajectoryPointsVisual3D;
        private int indexTrajectoryPoint; //TODO: remove it, do smarter

        private List<TrajectoryLine> trajectoryLinesVisual3D;
        struct TrajectoryLine
        {
            public ModelVisual3D lineModelVisual3D;
            public Point3D start;
            public Point3D end;
        }

        private List<Point3D> listSplitTrajectoryPoints;

        private double trajectoryLenght;

        public TrajectoryModel3D(Trajectory track)
        {
            this.track = track;
            this.trajectoryLenght = 0;
            this.listTrajectoryPoints = new List<Point3D>();
        }

        private void ShowSplitPath(List<Point3D> listSplitPathPoints)
        {
            foreach (var p in listSplitPathPoints)
            {
                //TODO: Extract to method next 8 line
                var point = new MeshGeometry3D();
               // AddSphere(point, new Point3D(p.X, p.Y, p.Z), 0.2, 8, 8);
                var pointBrush = Brushes.DarkRed;
                var pointMaterial = new DiffuseMaterial(pointBrush);
                var pathPointGeometryModel = new GeometryModel3D(point, pointMaterial);
                var pathPointModelVisual3D = new ModelVisual3D();
                pathPointModelVisual3D.Content = pathPointGeometryModel;
                pathPointModelVisual3D.Transform = new TranslateTransform3D();
                //Viewport3D.Children.Add(pathPointModelVisual3D);
            }
        }

        // TODO: remove it (used for 2D Graphics)
        //public void ShowNextPoints(Graphics gr, int j)
        //{
        //    for (var i = j; i < NumOfExtraPoints; i++)
        //        gr.FillEllipse(new SolidBrush(Color.Red), new Rectangle(ExtraPoints[i].X - 3, ExtraPoints[i].Y - 3, 6, 6));
        //}

        //public void ShowPastPoints(Graphics gr, int j)
        //{
        //    for (var i = 0; i < j; i++)
        //        gr.FillEllipse(new SolidBrush(Color.Green), new Rectangle(ExtraPoints[i].X - 3, ExtraPoints[i].Y - 3, 6, 6));
        //}

        //public void ShowExtraPoints(Graphics gr)
        //{
        //    for (var i = 0; i < NumOfExtraPoints; i++)
        //        gr.FillEllipse(new SolidBrush(Color.Red), new Rectangle(ExtraPoints[i].X - 3, ExtraPoints[i].Y - 3, 6, 6));
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
        //        //gr.DrawLine(pen, ExtraPoints[i], ExtraPoints[i + 1]);
        //        gr.FillEllipse(brush, new Rectangle(ExtraPoints[i].X - 6, ExtraPoints[i].Y - 6, 12, 12));
        //        gr.FillEllipse(brushMyColor, new Rectangle(ExtraPoints[i].X - 3, ExtraPoints[i].Y - 3, 6, 6));
        //    }

        //    gr.DrawLine(pen, ExtraPoints[NumOfExtraPoints - 2], ExtraPoints[NumOfExtraPoints - 1]);
        //    gr.FillEllipse(brush, new Rectangle(ExtraPoints[NumOfExtraPoints - 1].X - 6, ExtraPoints[NumOfExtraPoints - 1].Y - 6, 12, 12));
        //    gr.FillEllipse(brushMyColor, new Rectangle(ExtraPoints[NumOfExtraPoints - 1].X - 3, ExtraPoints[NumOfExtraPoints - 1].Y - 3, 6, 6));
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
