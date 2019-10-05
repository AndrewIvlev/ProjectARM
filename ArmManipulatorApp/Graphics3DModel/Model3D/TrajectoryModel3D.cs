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
                this.trackModelVisual3D.Add(
                    this.CreateAnchorPointModelVisual3D(
                        new Point3D(coeff * anchorPoint.X, coeff * anchorPoint.Y, coeff * anchorPoint.Z)));
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

        public void AddAnchorPoint(Point3D newVirtualPoint)
        {
            this.track.AnchorPoints.Add(this.ConvertFromVirtualToReal(newVirtualPoint));

            this.trackModelVisual3D.Add(this.CreateTrajectoryLineModelVisual3D(newVirtualPoint));
            this.trackModelVisual3D.Add(this.CreateAnchorPointModelVisual3D(newVirtualPoint));
        }

        private ModelVisual3D CreateAnchorPointModelVisual3D(Point3D center)
        {
            var anchorPointModelVisual3D = new ModelVisual3D();
            var anchorPointMeshGeometry3D = new MeshGeometry3D();
            MeshGeometry3DHelper.AddSphere(anchorPointMeshGeometry3D, center, 7, 8, 8);
            var anchorPointBrush = Brushes.GreenYellow;
            var anchorPointMaterial = new DiffuseMaterial(anchorPointBrush);
            var anchorPointGeometryModel = new GeometryModel3D(anchorPointMeshGeometry3D, anchorPointMaterial);
            anchorPointModelVisual3D.Content = anchorPointGeometryModel;
            //// Translate Transform for up/down anchor points in Edit Trajectory Mode.
            anchorPointModelVisual3D.Transform = new TranslateTransform3D();
            return anchorPointModelVisual3D;
        }

        private ModelVisual3D CreateTrajectoryLineModelVisual3D(Point3D endPoint)
        {
            var trajectoryLineModelVisual3D = new ModelVisual3D();
            var trajectoryLineMeshGeometry3D = new MeshGeometry3D();
            var startPoint = this.track.AnchorPoints[this.track.AnchorPoints.Count - 1];
            MeshGeometry3DHelper.AddSmoothCylinder(
                trajectoryLineMeshGeometry3D,
                new Point3D(startPoint.X, startPoint.Y, startPoint.Z),
                new Vector3D(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y, endPoint.Z - startPoint.Z),
                6);
            var anchorPointBrush = Brushes.MediumPurple;
            var anchorPointMaterial = new DiffuseMaterial(anchorPointBrush);
            var anchorPointGeometryModel = new GeometryModel3D(trajectoryLineMeshGeometry3D, anchorPointMaterial);
            trajectoryLineModelVisual3D.Content = anchorPointGeometryModel;

            //// Translate Transform for up/down anchor points in Edit Trajectory Mode.
            var rotateTransformByStart = new RotateTransform3D();
            rotateTransformByStart.CenterX = startPoint.X;
            rotateTransformByStart.CenterY = startPoint.Y;
            rotateTransformByStart.CenterZ = startPoint.Z;

            var rotateTransformByEnd = new RotateTransform3D();
            rotateTransformByEnd.CenterX = endPoint.X;
            rotateTransformByEnd.CenterY = endPoint.Y;
            rotateTransformByEnd.CenterZ = endPoint.Z;

            var angleRotationByStart = new AxisAngleRotation3D();
            angleRotationByStart.Axis = Vector3D.CrossProduct(
                new Vector3D(
                    endPoint.X - startPoint.X,
                    endPoint.Y - startPoint.Y,
                    endPoint.Z - startPoint.Z),
                new Vector3D(0, 1, 0));
            angleRotationByStart.Angle = 0;

            var angleRotationByEnd = new AxisAngleRotation3D();
            angleRotationByEnd.Axis = Vector3D.CrossProduct(
                new Vector3D(0, 1, 0),
                new Vector3D(
                    endPoint.X - startPoint.X,
                    endPoint.Y - startPoint.Y,
                    endPoint.Z - startPoint.Z));
            angleRotationByEnd.Angle = 0;

            rotateTransformByStart.Rotation = angleRotationByStart;
            rotateTransformByEnd.Rotation = angleRotationByEnd;
            
            var transformGroup = new Transform3DGroup();
            transformGroup.Children.Add(rotateTransformByStart);
            transformGroup.Children.Add(rotateTransformByEnd);
            trajectoryLineModelVisual3D.Transform = transformGroup;
            return trajectoryLineModelVisual3D;
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

        public void ChangeAnchorPointZ(int indexOfAnchorPoint, double delta)
        {
            this.trajectoryPointsVisual3D[this.indexTrajectoryPoint].SetY(this.trajectoryPointsVisual3D[this.indexTrajectoryPoint].center.Y + delta);

            (this.trajectoryPointsVisual3D[this.indexTrajectoryPoint].trajectoryModelVisual3D.Transform as TranslateTransform3D).OffsetY += delta;
            this.NeighborhoodLinesRotation();
        }

        private void NeighborhoodLinesRotation()
        {
            // Temporary solution
            // Insert new ModelVisual3D model of path line except old

            // TrajectoryLine trajectoryLine;
            // trajectoryLine.start = this.trajectoryPointsVisual3D[this.indexTrajectoryPoint - 1].center;
            // trajectoryLine.end = this.trajectoryPointsVisual3D[this.indexTrajectoryPoint].center;

            ////TODO: Extract to method next 8 line
            // var line = new MeshGeometry3D();
            // LineByTwoPoints(line, trajectoryLine.start, trajectoryLine.end, 0.13);
            // var lineBrush = Brushes.MediumPurple;
            // var lineMaterial = new DiffuseMaterial(lineBrush);
            // var lineGeometryModel = new GeometryModel3D(line, lineMaterial);
            // var pathLineModelVisual3D = new ModelVisual3D();
            // pathLineModelVisual3D.Content = lineGeometryModel;
            // Viewport3D.Children.Remove(this.trajectoryLinesVisual3D[this.indexTrajectoryPoint - 1].lineModelVisual3D);
            // Viewport3D.Children.Insert(this.indexTrajectoryPoint - 1, pathLineModelVisual3D);

            // trajectoryLine.lineModelVisual3D = pathLineModelVisual3D;
            // this.trajectoryLinesVisual3D.RemoveAt(this.indexTrajectoryPoint - 1);
            // this.trajectoryLinesVisual3D.Insert(this.indexTrajectoryPoint - 1, trajectoryLine);

            // if (this.indexTrajectoryPoint == this.trajectoryPointsVisual3D.Count - 1) return;

            // trajectoryLine.start = this.trajectoryPointsVisual3D[this.indexTrajectoryPoint].center;
            // trajectoryLine.end = this.trajectoryPointsVisual3D[this.indexTrajectoryPoint + 1].center;

            ////TODO: Extract to method next 8 line
            // line = new MeshGeometry3D();
            // LineByTwoPoints(line, trajectoryLine.start, trajectoryLine.end, 0.13);
            // lineBrush = Brushes.MediumPurple;
            // lineMaterial = new DiffuseMaterial(lineBrush);
            // lineGeometryModel = new GeometryModel3D(line, lineMaterial);
            // pathLineModelVisual3D = new ModelVisual3D();
            // pathLineModelVisual3D.Content = lineGeometryModel;
            // Viewport3D.Children.Remove(this.trajectoryLinesVisual3D[this.indexTrajectoryPoint].lineModelVisual3D);
            // Viewport3D.Children.Insert(this.indexTrajectoryPoint, pathLineModelVisual3D);

            // trajectoryLine.lineModelVisual3D = pathLineModelVisual3D;
            // this.trajectoryLinesVisual3D.RemoveAt(this.indexTrajectoryPoint);
            // this.trajectoryLinesVisual3D.Insert(this.indexTrajectoryPoint, trajectoryLine);

            // there is the bug :( bug#2
            // Because lenght changed when we up or down path point
            // And there is need to use ScaleTransform3D with RotateTransform3D
            // var rightLineLenght = (pathLinesVisual3D[indexPathPoint - 1].end - pathLinesVisual3D[indexPathPoint - 1].start).Length;
            // var h = pathLinesVisual3D[indexPathPoint].end.Y - pathLinesVisual3D[indexPathPoint].start.Y;
            // var rightLineAngle = Math.Asin(h / rightLineLenght);
            // (((pathLinesVisual3D[indexPathPoint - 1]
            // .lineModelVisual3D
            // .Transform as Transform3DGroup)
            // .Children[0] as RotateTransform3D)
            // .Rotation as AxisAngleRotation3D)
            // .Angle = rightLineAngle;

            // var leftLineLenght = (pathLinesVisual3D[indexPathPoint].end - pathLinesVisual3D[indexPathPoint].start).Length;
            // h = pathLinesVisual3D[indexPathPoint].end.Y - pathLinesVisual3D[indexPathPoint].start.Y;
            // var leftLineAngle = Math.Asin(h / leftLineLenght);
            // (((pathLinesVisual3D[indexPathPoint]
            // .lineModelVisual3D
            // .Transform as Transform3DGroup)
            // .Children[1] as RotateTransform3D)
            // .Rotation as AxisAngleRotation3D)
            // .Angle = leftLineAngle;
        }

        // TODO: create class for next convert method:
        private Point3D ConvertFromRealToVirtual(Point3D point)
            => new Point3D(point.X * this.coeff, point.Y * this.coeff, point.Z * this.coeff);

        private Point3D ConvertFromVirtualToReal(Point3D point)
            => new Point3D(point.X / this.coeff, point.Y / this.coeff, point.Z / this.coeff);
    }
}
