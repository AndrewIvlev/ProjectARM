using MainApp.Graphics3DModel.Model3D;

namespace ArmManipulatorApp.Graphics3DModel.Model3D
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

    using ArmManipulatorArm.MathModel;
    using ArmManipulatorArm.MathModel.Arm;

    public class ManipulatorArmModel3D
    {
        public Arm arm;

        public List<ModelVisual3D> armModelVisual3D; // Count of this list should be (model.n + 1)

        private Storyboard storyboard;

        /// <summary>
        /// Задаёт отношение реальных физических величин манипулятора
        /// от пиксельной характеристики виртуальной 3D модели манипулятора: len(px) = coeff * len(cm)
        /// </summary>
        private double coeff;

        public ManipulatorArmModel3D(Arm arm)
        {
            this.arm = arm;
            armModelVisual3D = new List<ModelVisual3D>();
            storyboard = new Storyboard();
            //CreateManipulator3DVisualModel();
        }

        // TODO: rename to BuildArmModelVisual3D
        private void CreateManipulator3DVisualModel()
        {
            if (armModelVisual3D.Count > 0)
            {
                //foreach (var arm in armModelVisual3D)
                    //Viewport3D.Children.Remove(arm);
            }

            var sup = new Vector3D(0, 0, 0); // startUnitPoint
            for (var i = 0; i < arm.N + 1; i++)
            {
                var unit = new ModelVisual3D();
                var jointsAndUnitsModelGroup = new Model3DGroup();
                var unitMesh = new MeshGeometry3D();
                var jointMesh = new MeshGeometry3D();

                var eup = arm.F(i); // endUnitPoint
                MeshGeometry3DHelper.AddParallelepiped(
                    unitMesh, 
                    new Point3D(sup.X * coeff, sup.Y * coeff, sup.Z * coeff),
                    new Point3D(eup.X * coeff, eup.Y * coeff, eup.Z * coeff), 
                    0.25);
                MeshGeometry3DHelper.AddSphere(jointMesh, new Point3D(eup.X * coeff, eup.Y * coeff, eup.Z * coeff), 0.4, 8, 8);
                sup = eup;

                var unitBrush = Brushes.CornflowerBlue;
                var unitMaterial = new DiffuseMaterial(unitBrush);
                var myGeometryModel = new GeometryModel3D(unitMesh, unitMaterial);
                jointsAndUnitsModelGroup.Children.Add(myGeometryModel);

                var jointBrush = Brushes.OrangeRed;
                var jointMaterial = new DiffuseMaterial(jointBrush);
                myGeometryModel = new GeometryModel3D(jointMesh, jointMaterial);
                jointsAndUnitsModelGroup.Children.Add(myGeometryModel);

                unit.Content = jointsAndUnitsModelGroup;
                var storyBoard = new Storyboard();

                //Viewport3D.Children.Add(arm); TODO: binding Viewport3D
                armModelVisual3D.Add(unit);
            }
        }

        private void ManipulatorMoveAnimation()
        {
            var timelineCollection = new TimelineCollection();
            for (var i = 1; i < arm.N; i++)
            {
                var animation1 = new ThicknessAnimation();
                animation1.From = new Thickness(5);
                animation1.To = new Thickness(25);
                animation1.Duration = TimeSpan.FromSeconds(5);
                //Storyboard.SetTarget(animation1, button1);
                //Storyboard.SetTargetProperty(animation1, new PropertyPath(MarginProperty));
            }

            storyboard.Children = timelineCollection;

            storyboard.Begin();
        }

        /// <summary>
        /// Add transformations for each unit of manipulatorModelVisual3D
        /// </summary>
        private void AddTransformationsForManipulator()
        {
            var transformGroup = new Transform3DGroup[arm.N];
            for (var i = 0; i < arm.N; i++)
            {
                Transform3D transformation = null;

                var center = arm.F(i);
                switch (arm.Units[i].Type)
                {
                    #region case R
                    case 'R':
                        transformation = new RotateTransform3D();
                        (transformation as RotateTransform3D).CenterX = center.X;
                        (transformation as RotateTransform3D).CenterY = center.Y;
                        (transformation as RotateTransform3D).CenterZ = center.Z;

                        var angleRotation = new AxisAngleRotation3D
                        {
                            Axis = arm.GetZAxis(i),
                            Angle = MathFunctions.RadianToDegree(arm.Units[i].Q)
                        };

                        (transformation as RotateTransform3D).Rotation = angleRotation;

                        transformGroup[i] = new Transform3DGroup();
                        for (var j = i; j < arm.N; j++)
                            transformGroup[j].Children.Add(transformation);

                        break;
                    #endregion
                    case 'P':
                        transformation = new TranslateTransform3D();

                        transformGroup[i] = new Transform3DGroup();
                        for (var j = i + 1; j < arm.N; j++)
                            transformGroup[j].Children.Add(transformation);

                        break;
                }
            }
            // Трансформация звеньев RotateTransform3D для 'R' должна быть применена ко всем последующим звеньям,
            // а для звена 'P' только ScaleTransform3D(только для линии) и для всех последующих TranslateTransform3D
            for (var i = 1; i < arm.N + 1; i++)
                armModelVisual3D[i].Transform = transformGroup[i - 1];
        }

        private void ManipulatorTransformUpdate(double[] q)
        {

            for (var i = 1; i < this.arm.N + 1; i++)
            {

                switch (this.arm.Units[i - 1].Type)
                {
                    case 'R':
                        for (var j = i + 1; j < this.arm.N; j++)
                        {
                            (((armModelVisual3D[j]
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

                        var sup = this.arm.F(i - 1); // startUnitPoint
                        var eup = this.arm.F(i); // endUnitPoint
                        MeshGeometry3DHelper.AddParallelepiped(unit, new Point3D(sup.X * coeff, sup.Y * coeff, sup.Z * coeff),
                                              new Point3D(eup.X * coeff, eup.Y * coeff, eup.Z * coeff), 0.25);
                        MeshGeometry3DHelper.AddSphere(joint, new Point3D(eup.X * coeff, eup.Y * coeff, eup.Z * coeff), 0.4, 8, 8);

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

                        //Viewport3D.Children.Remove(armModelVisual3D[i + 1]);
                        //Viewport3D.Children.Insert(i + 1, arm);

                        armModelVisual3D.Remove(arm);
                        armModelVisual3D.Insert(i + 1, arm);
                        #endregion

                        prismaticAxis = this.arm.GetZAxis(i);
                        for (var j = i + 2; j < this.arm.N; j++)
                        {
                            ((armModelVisual3D[j]
                                .Transform as Transform3DGroup)
                                .Children[i] as TranslateTransform3D)
                                .OffsetX += prismaticAxis.X * q[i];
                            ((armModelVisual3D[j]
                                .Transform as Transform3DGroup)
                                .Children[i] as TranslateTransform3D)
                                .OffsetY += prismaticAxis.Y * q[i];
                            ((armModelVisual3D[j]
                                .Transform as Transform3DGroup)
                                .Children[i] as TranslateTransform3D)
                                .OffsetZ += prismaticAxis.Z * q[i];
                        }

                        break;
                }
            }
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

            // Add points to MeshGeometry3D
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
    }
}
