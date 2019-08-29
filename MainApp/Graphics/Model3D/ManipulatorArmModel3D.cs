namespace MainApp.Graphics.Model3D
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Media3D;

    using ManipulationSystemLibrary.MathModel;

    public class ManipulatorArmModel3D : INotifyPropertyChanged
    {
        public Arm arm;

        private List<ModelVisual3D> manipModelVisual3D; // Count of this list should be (model.n + 1)
 
        public ManipulatorArmModel3D(Arm arm)
        {
            this.arm = arm;
            this.manipModelVisual3D = new List<ModelVisual3D>();
        }

        private void CreateManipulator3DVisualModel(Arm model)
        {
            if (manipModelVisual3D.Count > 0)
            {
                foreach (var arm in manipModelVisual3D)
                    Viewport3D.Children.Remove(arm);
            }
            var sup = new Vector3D(0, 0, 0); // startUnitPoint
            for (var i = 0; i < model.N + 1; i++)
            {
                var arm = new ModelVisual3D();
                var jointsAndUnitsModelGroup = new Model3DGroup();
                var unit = new MeshGeometry3D();
                var joint = new MeshGeometry3D();

                var eup = model.F(i); // endUnitPoint
                LineByTwoPoints(unit, new Point3D(sup.X * coeff, sup.Y * coeff + OffsetY, sup.Z * coeff),
                    new Point3D(eup.X * coeff, eup.Y * coeff + OffsetY, eup.Z * coeff), 0.25);
                AddSphere(joint, new Point3D(eup.X * coeff, eup.Y * coeff + OffsetY, eup.Z * coeff), 0.4, 8, 8);
                sup = eup;

                var unitBrush = Brushes.CornflowerBlue;
                var unitMaterial = new DiffuseMaterial(unitBrush);
                var myGeometryModel = new GeometryModel3D(unit, unitMaterial);
                jointsAndUnitsModelGroup.Children.Add(myGeometryModel);

                var jointBrush = Brushes.OrangeRed;
                var jointMaterial = new DiffuseMaterial(jointBrush);
                myGeometryModel = new GeometryModel3D(joint, jointMaterial);
                jointsAndUnitsModelGroup.Children.Add(myGeometryModel);

                arm.Content = jointsAndUnitsModelGroup;
                var storyBoard = new Storyboard();

                Viewport3D.Children.Add(arm);
                manipModelVisual3D.Add(arm);
            }
        }

        private void ManipulatorMoveAnimation()
        {
            var timelineCollection = new TimelineCollection();
            for (var i = 1; i < model.N; i++)
            {
                var animation1 = new ThicknessAnimation();
                animation1.From = new Thickness(5);
                animation1.To = new Thickness(25);
                animation1.Duration = TimeSpan.FromSeconds(5);
                //Storyboard.SetTarget(animation1, button1);
                Storyboard.SetTargetProperty(animation1, new PropertyPath(MarginProperty));
            }

            var storyboard = new Storyboard();
            storyboard.Children = timelineCollection;

            storyboard.Begin();
        }

        /// <summary>
        /// Add transformations for each unit of manipulatorModelVisual3D
        /// </summary>
        private void AddTransformationsForManipulator()
        {
            var transformGroup = new Transform3DGroup[model.N];
            for (var i = 0; i < model.N; i++)
            {
                Transform3D transformation = null;

                var center = model.F(i);
                switch (model.Units[i].Type)
                {
                    #region case R
                    case 'R':
                        transformation = new RotateTransform3D();
                        (transformation as RotateTransform3D).CenterX = center.X;
                        (transformation as RotateTransform3D).CenterY = center.Y;
                        (transformation as RotateTransform3D).CenterZ = center.Z;

                        var angleRotation = new AxisAngleRotation3D
                        {
                            Axis = model.GetZAxis(i),
                            Angle = RadianToDegree(model.Units[i].Q)
                        };

                        (transformation as RotateTransform3D).Rotation = angleRotation;

                        transformGroup[i] = new Transform3DGroup();
                        for (var j = i; j < model.N; j++)
                            transformGroup[j].Children.Add(transformation);

                        break;
                    #endregion
                    case 'P':
                        transformation = new TranslateTransform3D();

                        transformGroup[i] = new Transform3DGroup();
                        for (var j = i + 1; j < model.N; j++)
                            transformGroup[j].Children.Add(transformation);

                        break;
                }
            }
            // Трансформация звеньев RotateTransform3D для 'R' должна быть применена ко всем последующим звеньям,
            // а для звена 'P' только ScaleTransform3D(только для линии) и для всех последующих TranslateTransform3D
            for (var i = 1; i < model.N + 1; i++)
                manipModelVisual3D[i].Transform = transformGroup[i - 1];
        }

        private void ManipulatorTransformUpdate(double[] q)
        {

            for (var i = 1; i < model.N + 1; i++)
            {

                switch (model.Units[i - 1].Type)
                {
                    case 'R':
                        for (var j = i + 1; j < model.N; j++)
                        {
                            (((manipModelVisual3D[j]
                                .Transform as Transform3DGroup)
                                .Children[i] as RotateTransform3D)
                                .Rotation as AxisAngleRotation3D)
                                .Angle = q[i];
                        }
                        break;
                    case 'P':
                        #region Remove old and insert new P unit model visual 3d
                        var prismaticAxis = new Vector3D();
                        var unit = new MeshGeometry3D();
                        var joint = new MeshGeometry3D();

                        var sup = model.F(i - 1); // startUnitPoint
                        var eup = model.F(i); // endUnitPoint
                        LineByTwoPoints(unit, new Point3D(sup.X * coeff, sup.Y * coeff + OffsetY, sup.Z * coeff),
                                              new Point3D(eup.X * coeff, eup.Y * coeff + OffsetY, eup.Z * coeff), 0.25);
                        AddSphere(joint, new Point3D(eup.X * coeff, eup.Y * coeff + OffsetY, eup.Z * coeff), 0.4, 8, 8);

                        var unitBrush = Brushes.CornflowerBlue;
                        var unitMaterial = new DiffuseMaterial(unitBrush);
                        var myGeometryModel = new GeometryModel3D(unit, unitMaterial);
                        var jointsAndUnitsModelGroup = new Model3DGroup();
                        jointsAndUnitsModelGroup.Children.Add(myGeometryModel);

                        var jointBrush = Brushes.OrangeRed;
                        var jointMaterial = new DiffuseMaterial(jointBrush);
                        myGeometryModel = new GeometryModel3D(joint, jointMaterial);
                        jointsAndUnitsModelGroup.Children.Add(myGeometryModel);
                        var arm = new ModelVisual3D();
                        arm.Content = jointsAndUnitsModelGroup;

                        Viewport3D.Children.Remove(manipModelVisual3D[i + 1]);
                        Viewport3D.Children.Insert(i + 1, arm);

                        manipModelVisual3D.Remove(arm);
                        manipModelVisual3D.Insert(i + 1, arm);
                        #endregion

                        prismaticAxis = model.GetZAxis(i);
                        for (var j = i + 2; j < model.N; j++)
                        {
                            ((manipModelVisual3D[j]
                                .Transform as Transform3DGroup)
                                .Children[i] as TranslateTransform3D)
                                .OffsetX += prismaticAxis.X * q[i];
                            ((manipModelVisual3D[j]
                                .Transform as Transform3DGroup)
                                .Children[i] as TranslateTransform3D)
                                .OffsetY += prismaticAxis.Y * q[i];
                            ((manipModelVisual3D[j]
                                .Transform as Transform3DGroup)
                                .Children[i] as TranslateTransform3D)
                                .OffsetZ += prismaticAxis.Z * q[i];
                        }
                        break;
                }
            }
        }

        // Set the vector's length.
        private Vector3D ScaleVector(Vector3D vector, double length)
        {
            var scale = length / vector.Length;
            return new Vector3D(
                vector.X * scale,
                vector.Y * scale,
                vector.Z * scale);
        }

        // Add a triangle to the indicated mesh.
        // Do not reuse points so triangles don't share normals.
        private void AddTriangle(MeshGeometry3D mesh, Point3D point1, Point3D point2, Point3D point3)
        {
            // Create the points.
            var index1 = mesh.Positions.Count;
            mesh.Positions.Add(point1);
            mesh.Positions.Add(point2);
            mesh.Positions.Add(point3);

            // Create the triangle.
            mesh.TriangleIndices.Add(index1++);
            mesh.TriangleIndices.Add(index1++);
            mesh.TriangleIndices.Add(index1);
        }// Make a thin rectangular prism between the two points.

        // If extend is true, extend the segment by half the
        // thickness so segments with the same end points meet nicely.
        private void AddSegment(MeshGeometry3D mesh,
            Point3D point1, Point3D point2, Vector3D up, double thickness)
        {
            AddSegment(mesh, point1, point2, up, thickness, false);
        }

        private void AddSegment(MeshGeometry3D mesh,
            Point3D point1, Point3D point2, Vector3D up, double thickness,
            bool extend)
        {
            // Get the segment's vector.
            var v = point2 - point1;

            if (extend)
            {
                // Increase the segment's length on both ends by thickness / 2.
                var n = ScaleVector(v, thickness / 2.0);
                point1 -= n;
                point2 += n;
            }

            // Get the scaled up vector.
            var n1 = ScaleVector(up, thickness / 2.0);

            // Get another scaled perpendicular vector.
            var n2 = Vector3D.CrossProduct(v, n1);
            n2 = ScaleVector(n2, thickness / 2.0);

            // Make a skinny box.
            // p1pm means point1 PLUS n1 MINUS n2.
            var p1pp = point1 + n1 + n2;
            var p1mp = point1 - n1 + n2;
            var p1pm = point1 + n1 - n2;
            var p1mm = point1 - n1 - n2;
            var p2pp = point2 + n1 + n2;
            var p2mp = point2 - n1 + n2;
            var p2pm = point2 + n1 - n2;
            var p2mm = point2 - n1 - n2;

            // Sides.
            AddTriangle(mesh, p1pp, p1mp, p2mp);
            AddTriangle(mesh, p1pp, p2mp, p2pp);

            AddTriangle(mesh, p1pp, p2pp, p2pm);
            AddTriangle(mesh, p1pp, p2pm, p1pm);

            AddTriangle(mesh, p1pm, p2pm, p2mm);
            AddTriangle(mesh, p1pm, p2mm, p1mm);

            AddTriangle(mesh, p1mm, p2mm, p2mp);
            AddTriangle(mesh, p1mm, p2mp, p1mp);

            // Ends.
            AddTriangle(mesh, p1pp, p1pm, p1mm);
            AddTriangle(mesh, p1pp, p1mm, p1mp);

            AddTriangle(mesh, p2pp, p2mp, p2mm);
            AddTriangle(mesh, p2pp, p2mm, p2pm);
        }

        // Add a sphere.
        private void AddSphere(MeshGeometry3D mesh, Point3D center,
            double radius, int num_phi, int num_theta)
        {
            double phi0, theta0;
            var dphi = Math.PI / num_phi;
            var dtheta = 2 * Math.PI / num_theta;

            phi0 = 0;
            var y0 = radius * Math.Cos(phi0);
            var r0 = radius * Math.Sin(phi0);
            for (var i = 0; i < num_phi; i++)
            {
                var phi1 = phi0 + dphi;
                var y1 = radius * Math.Cos(phi1);
                var r1 = radius * Math.Sin(phi1);

                // Point ptAB has phi value A and theta value B.
                // For example, pt01 has phi = phi0 and theta = theta1.
                // Find the points with theta = theta0.
                theta0 = 0;
                var pt00 = new Point3D(
                    center.X + r0 * Math.Cos(theta0),
                    center.Y + y0,
                    center.Z + r0 * Math.Sin(theta0));
                var pt10 = new Point3D(
                    center.X + r1 * Math.Cos(theta0),
                    center.Y + y1,
                    center.Z + r1 * Math.Sin(theta0));
                for (var j = 0; j < num_theta; j++)
                {
                    // Find the points with theta = theta1.
                    var theta1 = theta0 + dtheta;
                    var pt01 = new Point3D(
                        center.X + r0 * Math.Cos(theta1),
                        center.Y + y0,
                        center.Z + r0 * Math.Sin(theta1));
                    var pt11 = new Point3D(
                        center.X + r1 * Math.Cos(theta1),
                        center.Y + y1,
                        center.Z + r1 * Math.Sin(theta1));

                    // Create the triangles.
                    AddTriangle(mesh, pt00, pt11, pt10);
                    AddTriangle(mesh, pt00, pt01, pt11);

                    // Move to the next value of theta.
                    theta0 = theta1;
                    pt00 = pt01;
                    pt10 = pt11;
                }

                // Move to the next value of phi.
                phi0 = phi1;
                y0 = y1;
                r0 = r1;
            }
        }

        private void LineByTwoPoints(MeshGeometry3D mesh, Point3D start, Point3D end, double thickness)
        {
            var up = new Vector3D(0, 0, 1);
            AddSegment(mesh, new Point3D(start.X, start.Y, start.Z), new Point3D(end.X, end.Y, end.Z), up, thickness);
        }

        /// <summary>
        /// Generates a model of a Circle given specified parameters
        /// </summary>
        /// <param name="radius">Radius of circle</param>
        /// <param name="normal">Vector normal to circle's plane</param>
        /// <param name="center">Center position of the circle</param>
        /// <param name="resolution">Number of slices to iterate the circumference of the circle</param>
        /// <returns>A GeometryModel3D representation of the circle</returns>
        private GeometryModel3D GetCircleModel(double radius, Vector3D normal, Point3D center, int resolution)
        {
            var geo = new MeshGeometry3D();

            // Generate the circle in the XZ-plane
            // Add the center first
            geo.Positions.Add(new Point3D(0, 0, 0));

            // Iterate from angle 0 to 2*PI
            var t = 2 * Math.PI / resolution;
            for (var i = 0; i < resolution; i++)
            {
                geo.Positions.Add(new Point3D(radius * Math.Cos(t * i), 0, -radius * Math.Sin(t * i)));
            }

            // Add points to MeshGeoemtry3D
            for (var i = 0; i < resolution; i++)
            {
                var a = 0;
                var b = i + 1;
                var c = (i < (resolution - 1)) ? i + 2 : 1;

                geo.TriangleIndices.Add(a);
                geo.TriangleIndices.Add(b);
                geo.TriangleIndices.Add(c);
            }

            var brush3 = Brushes.Purple;
            var material3 = new DiffuseMaterial(brush3);
            var mod = new GeometryModel3D(geo, material3);

            // Create transforms
            var trn = new Transform3DGroup();
            // Up Vector (normal for XZ-plane)
            var up = new Vector3D(0, 1, 0);
            // Set normal length to 1
            normal.Normalize();
            var axis = Vector3D.CrossProduct(up, normal); // Cross product is rotation axis
            var angle = Vector3D.AngleBetween(up, normal); // Angle to rotate
            trn.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(axis, angle)));
            trn.Children.Add(new TranslateTransform3D(new Vector3D(center.X, center.Y, center.Z)));

            mod.Transform = trn;

            return mod;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
