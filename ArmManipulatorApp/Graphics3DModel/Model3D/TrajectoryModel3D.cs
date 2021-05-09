namespace ArmManipulatorApp.Graphics3DModel.Model3D
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using System.Windows.Shapes;

    using ArmManipulatorApp.MathModel.Trajectory;

    using MainApp.Graphics3DModel.Model3D;
    using Brushes = System.Windows.Media.Brushes;

    public class TrajectoryModel3D
    {
        public Trajectory track;

        public List<ModelVisual3D> trackModelVisual3D;
        public List<ModelVisual3D> splitTrackModelVisual3D;

        /// <summary>
        /// Задаёт отношение реальных физических величин манипулятора
        /// от пиксельной характеристики виртуальной 3D модели манипулятора: len(px) = coeff * len(cm)
        /// </summary>
        private double coeff;
        private double pointRadius;
        private double trackLineRadius;

        public TrajectoryModel3D(Trajectory track, double thickness, double coeff = 1)
        {
            this.coeff = coeff;
            this.pointRadius = thickness * this.coeff / 2;
            this.trackLineRadius = this.pointRadius * 0.90;
            this.track = track;
            this.trackModelVisual3D = new List<ModelVisual3D>();
            this.splitTrackModelVisual3D = new List<ModelVisual3D>();
            
            this.trackModelVisual3D.Add(
                this.CreateAnchorPointModelVisual3D(VRConvert.ConvertFromRealToVirtual(this.track.AnchorPoints[0], this.coeff), this.pointRadius));
            for (var i = 1; i < this.track.AnchorPoints.Count; i++)
            {
                var trackLineMV3D = this.CreateTrajectoryLineModelVisual3D(
                    VRConvert.ConvertFromRealToVirtual(
                        this.track.AnchorPoints[i - 1],
                        this.coeff),
                    VRConvert.ConvertFromRealToVirtual(this.track.AnchorPoints[i], this.coeff),
                    this.trackLineRadius);

                this.trackModelVisual3D.Add(trackLineMV3D);
                this.trackModelVisual3D.Add(
                    this.CreateAnchorPointModelVisual3D(VRConvert.ConvertFromRealToVirtual(this.track.AnchorPoints[i], this.coeff), this.pointRadius));
            }

            foreach (var splitPoint in this.track.SplitPoints)
            {
                this.splitTrackModelVisual3D.Add(
                    this.CreateSplitPointModelVisual3D(VRConvert.ConvertFromRealToVirtual(splitPoint, this.coeff)));
            }
        }

        public void AddAnchorPoint(Point3D newVirtualPoint)
        {
            this.track.AddAnchorPoint(VRConvert.ConvertFromVirtualToReal(newVirtualPoint, this.coeff));

            var trackLineMV3D = this.CreateTrajectoryLineModelVisual3D(
                VRConvert.ConvertFromRealToVirtual(
                    this.track.AnchorPoints[this.track.AnchorPoints.Count - 2],
                    this.coeff),
                newVirtualPoint,
                this.trackLineRadius);
            var trackAnchorPointMV3D = this.CreateAnchorPointModelVisual3D(newVirtualPoint, this.pointRadius);

            this.trackModelVisual3D.Add(trackLineMV3D);
            this.trackModelVisual3D.Add(trackAnchorPointMV3D);
        }

        private ModelVisual3D CreateAnchorPointModelVisual3D(Point3D center, double radius)
        {
            var anchorPointModelVisual3D = new ModelVisual3D();
            var anchorPointMeshGeometry3D = new MeshGeometry3D();
            MeshGeometry3DHelper.AddSphere(anchorPointMeshGeometry3D, center, radius, 8, 8);
            var anchorPointBrush = Brushes.GreenYellow;
            var anchorPointMaterial = new DiffuseMaterial(anchorPointBrush);
            var anchorPointGeometryModel = new GeometryModel3D(anchorPointMeshGeometry3D, anchorPointMaterial);
            anchorPointModelVisual3D.Content = anchorPointGeometryModel;
            //// Translate Transform for up/down anchor points in Edit Trajectory Mode.
            anchorPointModelVisual3D.Transform = new TranslateTransform3D();
            return anchorPointModelVisual3D;
        }
        
        private ModelVisual3D CreateSplitPointModelVisual3D(Point3D center)
        {
            var splitPointModelVisual3D = new ModelVisual3D();
            var splitPointMeshGeometry3D = new MeshGeometry3D();
            MeshGeometry3DHelper.AddSphere(splitPointMeshGeometry3D, center, 6, 8, 8);
            var splitPointBrush = System.Windows.Media.Brushes.Red;
            var splitPointMaterial = new DiffuseMaterial(splitPointBrush);
            var splitPointGeometryModel = new GeometryModel3D(splitPointMeshGeometry3D, splitPointMaterial);
            splitPointModelVisual3D.Content = splitPointGeometryModel;
            return splitPointModelVisual3D;
        }

        private ModelVisual3D CreateTrajectoryLineModelVisual3D(Point3D startPoint, Point3D endPoint, double radius)
        {
            var trajectoryLineModelVisual3D = new ModelVisual3D();
            var trajectoryLineMeshGeometry3D = new MeshGeometry3D();
            MeshGeometry3DHelper.AddSmoothCylinder(
                trajectoryLineMeshGeometry3D,
                new Point3D(startPoint.X, startPoint.Y, startPoint.Z),
                new Vector3D(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y, endPoint.Z - startPoint.Z),
                radius);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="with3DLines"> if it false then only track points will be displayed</param>
        public void ShowInterpolatedTrack(bool with3DLines)
        {
            this.splitTrackModelVisual3D.Add(
                this.CreateAnchorPointModelVisual3D(VRConvert.ConvertFromRealToVirtual(this.track.SplitPoints[0], this.coeff), this.pointRadius));
            for (var i = 1; i < this.track.SplitPoints.Count; i++)
            {
                if (with3DLines)
                {
                    var trackLineMV3D = this.CreateTrajectoryLineModelVisual3D(
                        VRConvert.ConvertFromRealToVirtual(
                            this.track.SplitPoints[i - 1],
                            this.coeff),
                        VRConvert.ConvertFromRealToVirtual(this.track.SplitPoints[i], this.coeff),
                        this.trackLineRadius);

                    this.splitTrackModelVisual3D.Add(trackLineMV3D);
                }

                this.splitTrackModelVisual3D.Add(
                    this.CreateAnchorPointModelVisual3D(VRConvert.ConvertFromRealToVirtual(this.track.SplitPoints[i], this.coeff), this.pointRadius));
            }
        }

        public void SplitPathWithInterpolation(DoWorkEventArgs e, object sender, double step)
        {
            this.track.SplitViaInterpolation(e, sender, step);
        }

        public void SplitPath(DoWorkEventArgs e, object sender, double step)
        {
            this.track.SplitTrack(e, sender, step);

            // foreach (var splitPoint in this.track.SplitPoints)
            // {
            //     this.splitTrackModelVisual3D.Add(this.CreateSplitPointModelVisual3D(VRConvert.ConvertFromRealToVirtual(splitPoint, this.coeff)));
            // }
        }

        /// <summary>
        /// Change z coordinate of anchor point
        /// </summary>
        /// <param name="indexOfAnchorPoint"> begin from second point of trajectory,
        /// because first point pinned to manipulator end point</param>
        /// <param name="deltaZ">real deltaZ in cm</param>
        public void ChangeAnchorPointZ(int indexOfAnchorPoint, double deltaZ)
        {
            if (indexOfAnchorPoint == 0)
            {
                throw new Exception("Can't change z coordinate of first trajectory point!");
            }

            this.track.AnchorPointOffsetZ(indexOfAnchorPoint, deltaZ);

            ((TranslateTransform3D)this.trackModelVisual3D[indexOfAnchorPoint * 2].Transform).OffsetZ += deltaZ * this.coeff;

            if (indexOfAnchorPoint == this.track.AnchorPoints.Count - 1)
            {
                this.trackModelVisual3D[indexOfAnchorPoint * 2 - 1] = this.CreateTrajectoryLineModelVisual3D(
                    VRConvert.ConvertFromRealToVirtual(this.track.AnchorPoints[indexOfAnchorPoint - 1], this.coeff),
                    VRConvert.ConvertFromRealToVirtual(this.track.AnchorPoints[indexOfAnchorPoint], this.coeff),
                    this.trackLineRadius);
            }
            else
            {
                this.trackModelVisual3D[indexOfAnchorPoint * 2 - 1] = this.CreateTrajectoryLineModelVisual3D(
                    VRConvert.ConvertFromRealToVirtual(this.track.AnchorPoints[indexOfAnchorPoint - 1], this.coeff),
                    VRConvert.ConvertFromRealToVirtual(this.track.AnchorPoints[indexOfAnchorPoint], this.coeff),
                    this.trackLineRadius);
                this.trackModelVisual3D[indexOfAnchorPoint * 2 + 1] = this.CreateTrajectoryLineModelVisual3D(
                    VRConvert.ConvertFromRealToVirtual(this.track.AnchorPoints[indexOfAnchorPoint], this.coeff),
                    VRConvert.ConvertFromRealToVirtual(this.track.AnchorPoints[indexOfAnchorPoint + 1], this.coeff),
                    this.trackLineRadius);
            }
        }

        public void Show(ref Viewport3D viewport)
        {
            foreach (var mv in this.trackModelVisual3D)
            {
                viewport.Children.Add(mv);
            }
        }

        public void Hide()
        {

        }

        // TODO: remove it 
        //private void NeighborhoodLinesRotation()
        //{
        //    //TODO: Extract to method next 8 line
        //    var line = new MeshGeometry3D();
        //    LineByTwoPoints(line, trajectoryLine.start, trajectoryLine.end, 0.13);
        //    var lineBrush = Brushes.MediumPurple;
        //    var lineMaterial = new DiffuseMaterial(lineBrush);
        //    var lineGeometryModel = new GeometryModel3D(line, lineMaterial);
        //    var pathLineModelVisual3D = new ModelVisual3D();
        //    pathLineModelVisual3D.Content = lineGeometryModel;
        //    Viewport3D.Children.Remove(this.trajectoryLinesVisual3D[this.indexTrajectoryPoint - 1].lineModelVisual3D);
        //    Viewport3D.Children.Insert(this.indexTrajectoryPoint - 1, pathLineModelVisual3D);

        //    trajectoryLine.lineModelVisual3D = pathLineModelVisual3D;
        //    this.trajectoryLinesVisual3D.RemoveAt(this.indexTrajectoryPoint - 1);
        //    this.trajectoryLinesVisual3D.Insert(this.indexTrajectoryPoint - 1, trajectoryLine);

        //    if (this.indexTrajectoryPoint == this.trajectoryPointsVisual3D.Count - 1) return;

        //    trajectoryLine.start = this.trajectoryPointsVisual3D[this.indexTrajectoryPoint].center;
        //    trajectoryLine.end = this.trajectoryPointsVisual3D[this.indexTrajectoryPoint + 1].center;

        //    //TODO: Extract to method next 8 line
        //    line = new MeshGeometry3D();
        //    LineByTwoPoints(line, trajectoryLine.start, trajectoryLine.end, 0.13);
        //    lineBrush = Brushes.MediumPurple;
        //    lineMaterial = new DiffuseMaterial(lineBrush);
        //    lineGeometryModel = new GeometryModel3D(line, lineMaterial);
        //    pathLineModelVisual3D = new ModelVisual3D();
        //    pathLineModelVisual3D.Content = lineGeometryModel;
        //    Viewport3D.Children.Remove(this.trajectoryLinesVisual3D[this.indexTrajectoryPoint].lineModelVisual3D);
        //    Viewport3D.Children.Insert(this.indexTrajectoryPoint, pathLineModelVisual3D);

        //    trajectoryLine.lineModelVisual3D = pathLineModelVisual3D;
        //    this.trajectoryLinesVisual3D.RemoveAt(this.indexTrajectoryPoint);
        //    this.trajectoryLinesVisual3D.Insert(this.indexTrajectoryPoint, trajectoryLine);

        //    there is the bug :(bug#2
        //     Because lenght changed when we up or down path point
        //     And there is need to use ScaleTransform3D with RotateTransform3D
        //     var rightLineLenght = (pathLinesVisual3D[indexPathPoint - 1].end - pathLinesVisual3D[indexPathPoint - 1].start).Length;
        //    var h = pathLinesVisual3D[indexPathPoint].end.Y - pathLinesVisual3D[indexPathPoint].start.Y;
        //    var rightLineAngle = Math.Asin(h / rightLineLenght);
        //    (((pathLinesVisual3D[indexPathPoint - 1]
        //    .lineModelVisual3D
        //    .Transform as Transform3DGroup)
        //    .Children[0] as RotateTransform3D)
        //    .Rotation as AxisAngleRotation3D)
        //    .Angle = rightLineAngle;

        //    var leftLineLenght = (pathLinesVisual3D[indexPathPoint].end - pathLinesVisual3D[indexPathPoint].start).Length;
        //    h = pathLinesVisual3D[indexPathPoint].end.Y - pathLinesVisual3D[indexPathPoint].start.Y;
        //    var leftLineAngle = Math.Asin(h / leftLineLenght);
        //    (((pathLinesVisual3D[indexPathPoint]
        //    .lineModelVisual3D
        //    .Transform as Transform3DGroup)
        //    .Children[1] as RotateTransform3D)
        //    .Rotation as AxisAngleRotation3D)
        //    .Angle = leftLineAngle;
        //}
    }
}
