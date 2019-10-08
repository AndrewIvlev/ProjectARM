namespace ArmManipulatorApp.Graphics3DModel.Model3D
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Media3D;

    using ArmManipulatorArm.MathModel;
    using ArmManipulatorArm.MathModel.Arm;

    using MainApp.Graphics3DModel.Model3D;

    public class ManipulatorArmModel3D
    {
        public Arm arm;

        public List<ModelVisual3D> armModelVisual3D;
        
        /// <summary>
        /// Задаёт отношение реальных физических величин манипулятора
        /// от пиксельной характеристики виртуальной 3D модели манипулятора: len(px) = coeff * len(cm)
        /// </summary>
        private double coeff;

        public ManipulatorArmModel3D(Arm arm, double coeff = 1)
        {
            this.coeff = coeff;
            this.arm = arm;
            this.armModelVisual3D = new List<ModelVisual3D>();
        }

        public void ClearModelVisual3DCollection(Viewport3D viewport)
        {
            foreach (var mv in this.armModelVisual3D)
            {
                viewport.Children.Remove(mv);
            }
        }

        public void BuildModelVisual3DCollection()
        {
            var sup = new Point3D(0, 0, 0); // Start Unit Point
            var eup = (Point3D)this.arm.F(0); // End Unit Point

            // Zero unit of arm:
            this.armModelVisual3D.Add(
                this.CreateArmBoneModelVisual3D(
                    VRConvert.ConvertFromRealToVirtual(sup, this.coeff),
                    VRConvert.ConvertFromRealToVirtual(eup, this.coeff)));
            var jointBrush = Brushes.Black;
            this.armModelVisual3D.Add(
                this.CreateArmJointModelVisual3D(VRConvert.ConvertFromRealToVirtual(eup, this.coeff), jointBrush));

            sup = eup;

            // 'P' and 'R' units:
            for (var i = 0; i < this.arm.N; i++)
            {
                eup = (Point3D)this.arm.F(i + 1);
                
                jointBrush = this.arm.Units[i].Type == 'R' ? Brushes.OrangeRed : Brushes.Green;
                this.armModelVisual3D.Add(
                    this.CreateArmJointModelVisual3D(VRConvert.ConvertFromRealToVirtual(sup, this.coeff), jointBrush));

                this.armModelVisual3D.Add(
                    this.CreateArmBoneModelVisual3D(
                        VRConvert.ConvertFromRealToVirtual(sup, this.coeff),
                        VRConvert.ConvertFromRealToVirtual(eup, this.coeff)));

                sup = eup;
            }
            
            // Grip unit:
            jointBrush = Brushes.Blue;
            this.armModelVisual3D.Add(
                this.CreateArmJointModelVisual3D(VRConvert.ConvertFromRealToVirtual(sup, this.coeff), jointBrush));
        }

        private ModelVisual3D CreateArmBoneModelVisual3D(Point3D start, Point3D end)
        {
            var bone = new ModelVisual3D();
            var boneMesh = new MeshGeometry3D();
            MeshGeometry3DHelper.AddSmoothCylinder(
                boneMesh,
                start,
                new Vector3D(end.X - start.X, end.Y - start.Y, end.Z - start.Z),
                6);
            var boneMaterial = new DiffuseMaterial(Brushes.CornflowerBlue);
            var boneGeometryModel = new GeometryModel3D(boneMesh, boneMaterial);
            bone.Content = boneGeometryModel;

            var transGroup = new Transform3DGroup();
            transGroup.Children.Add(new TranslateTransform3D());
            var rotTrans = new RotateTransform3D { Rotation = new AxisAngleRotation3D() };
            transGroup.Children.Add(rotTrans);
            bone.Transform = transGroup;

            return bone;
        }

        private ModelVisual3D CreateArmJointModelVisual3D(Point3D center, SolidColorBrush brush)
        {
            var joint = new ModelVisual3D();
            var jointMesh = new MeshGeometry3D();
            MeshGeometry3DHelper.AddSphere(jointMesh, center, 9, 8, 8);
            var jointMaterial = new DiffuseMaterial(brush);
            var jointGeometryModel = new GeometryModel3D(jointMesh, jointMaterial);
            joint.Content = jointGeometryModel;

            var transGroup = new Transform3DGroup();
            transGroup.Children.Add(new TranslateTransform3D());
            var rotTrans = new RotateTransform3D { Rotation = new AxisAngleRotation3D() };
            transGroup.Children.Add(rotTrans);
            joint.Transform = transGroup;

            return joint;
        }

        public void BeginAnimation(double[] dQ, ref Storyboard storyboard)
        {
            for (var i = 0; i < this.arm.N; i++)
            {
                var timeLineCollection = new TimelineCollection();
                switch (this.arm.Units[i].Type)
                {
                    case 'R':
                        var rotAxis = VRConvert.ConvertFromRealToVirtual(this.arm.GetZAxis(i), this.coeff);
                        this.arm.CalcMetaDataForStanding();
                        var centerRot = VRConvert.ConvertFromRealToVirtual((Point3D)this.arm.F(i), this.coeff);
                        
                        for (var j = 2 * (i + 1); j < 2 * (this.arm.N + 1) + 1; j++)
                        {
                            ((this.armModelVisual3D[j]
                                  .Transform as Transform3DGroup)
                             .Children[1] as RotateTransform3D)
                                .CenterX = centerRot.X;
                            ((this.armModelVisual3D[j]
                                  .Transform as Transform3DGroup)
                             .Children[1] as RotateTransform3D)
                                .CenterY = centerRot.Y;
                            ((this.armModelVisual3D[j]
                                  .Transform as Transform3DGroup)
                             .Children[1] as RotateTransform3D)
                                .CenterZ = centerRot.Z;

                            (((this.armModelVisual3D[j]
                                   .Transform as Transform3DGroup)
                              .Children[1] as RotateTransform3D)
                             .Rotation as AxisAngleRotation3D)
                                .Axis = rotAxis;

                            //(((this.armModelVisual3D[j].Transform as Transform3DGroup)
                            //  .Children[1] as RotateTransform3D)
                            // .Rotation as AxisAngleRotation3D)
                            //    .Angle = MathFunctions.RadianToDegree(dQ[i]);
                            
                            var animation = new DoubleAnimation();
                            animation.To = MathFunctions.RadianToDegree(dQ[i]);;
                            animation.DecelerationRatio = 1;
                            animation.Duration = TimeSpan.FromSeconds(0.15);
                            animation.AutoReverse = false;

                            Storyboard.SetTarget(
                                animation,
                                ((this.armModelVisual3D[j]
                                       .Transform as Transform3DGroup)
                                  .Children[1] as RotateTransform3D)
                                 .Rotation as AxisAngleRotation3D);
                            Storyboard.SetTargetProperty(animation, new PropertyPath(AxisAngleRotation3D.AngleProperty));
                            timeLineCollection.Add(animation);
                        }

                        break;
                    case 'P':
                        var prismaticAxis = VRConvert.ConvertFromRealToVirtual(this.arm.GetZAxis(i), this.coeff);
                        for (var j = 2 * (i + 1); j < 2 * (this.arm.N + 1) + 1; j++)
                        {
                            if (j == 2 * (i + 1) + 1) continue;

                            //((this.armModelVisual3D[j]
                            //      .Transform as Transform3DGroup)
                            // .Children[0] as TranslateTransform3D)
                            //    .OffsetX += prismaticAxis.X * dQ[i] * this.coeff;
                            //((this.armModelVisual3D[j]
                            //      .Transform as Transform3DGroup)
                            // .Children[0] as TranslateTransform3D)
                            //    .OffsetY += prismaticAxis.Y * dQ[i] * this.coeff;
                            //((this.armModelVisual3D[j]
                            //      .Transform as Transform3DGroup)
                            // .Children[0] as TranslateTransform3D)
                            //    .OffsetZ += prismaticAxis.Z * dQ[i] * this.coeff;
                            
                            var animationX = new DoubleAnimation();
                            animationX.To = dQ[i] * this.coeff;;
                            animationX.DecelerationRatio = 1;
                            animationX.Duration = TimeSpan.FromSeconds(0.15);
                            animationX.AutoReverse = false;
                            
                            var animationY = new DoubleAnimation();
                            animationY.To = dQ[i] * this.coeff;;
                            animationY.DecelerationRatio = 1;
                            animationY.Duration = TimeSpan.FromSeconds(0.15);
                            animationY.AutoReverse = false;
                            
                            var animationZ = new DoubleAnimation();
                            animationZ.To = dQ[i] * this.coeff;;
                            animationZ.DecelerationRatio = 1;
                            animationZ.Duration = TimeSpan.FromSeconds(0.15);
                            animationZ.AutoReverse = false;

                            Storyboard.SetTarget(
                                animationX,
                                ((this.armModelVisual3D[j]
                                      .Transform as Transform3DGroup)
                                 .Children[0] as TranslateTransform3D));
                            Storyboard.SetTargetProperty(animationX, new PropertyPath(TranslateTransform3D.OffsetXProperty));
                            
                            Storyboard.SetTarget(
                                animationY,
                                ((this.armModelVisual3D[j]
                                      .Transform as Transform3DGroup)
                                 .Children[0] as TranslateTransform3D));
                            Storyboard.SetTargetProperty(animationY, new PropertyPath(TranslateTransform3D.OffsetYProperty));
                            
                            Storyboard.SetTarget(
                                animationZ,
                                ((this.armModelVisual3D[j]
                                      .Transform as Transform3DGroup)
                                 .Children[0] as TranslateTransform3D));
                            Storyboard.SetTargetProperty(animationZ, new PropertyPath(TranslateTransform3D.OffsetZProperty));

                            timeLineCollection.Add(animationX);
                            timeLineCollection.Add(animationY);
                            timeLineCollection.Add(animationZ);
                        }

                        break;
                }

                storyboard.Children = timeLineCollection;
            }
        }
    }
}
