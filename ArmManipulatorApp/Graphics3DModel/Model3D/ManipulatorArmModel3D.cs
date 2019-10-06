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

    using MainApp.Graphics3DModel.Model3D;

    public class ManipulatorArmModel3D
    {
        public Arm arm;

        public List<ModelVisual3D> armModelVisual3D;
        
        private Storyboard storyboard;

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
            this.storyboard = new Storyboard();
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
        
        // TODO: complete this method
        public void ManipulatorMoveAnimation()
        {
            var timelineCollection = new TimelineCollection();
            for (var i = 1; i < this.arm.N; i++)
            {
                var j = 0;
                DoubleAnimation animation = new DoubleAnimation();
                animation.To = 40;
                animation.DecelerationRatio = 1;
                animation.Duration = TimeSpan.FromSeconds(0.15);
                animation.AutoReverse = true;
                (((this.armModelVisual3D[j].Transform as Transform3DGroup).Children[1] as RotateTransform3D)
                 .Rotation as AxisAngleRotation3D).BeginAnimation(AxisAngleRotation3D.AngleProperty, animation);
                //Storyboard.SetTarget(animation1, button1);
                //Storyboard.SetTargetProperty(animation1, new PropertyPath(MarginProperty));
            }

            this.storyboard.Children = timelineCollection;
            this.storyboard.Begin();
        }

        public void TransformUpdate(double[] q)
        {
            for (var i = 0; i < this.arm.N; i++)
            {
                switch (this.arm.Units[i].Type)
                {
                    case 'R':
                        var rotAxis = this.arm.GetZAxis(i);
                        this.arm.CalcMetaDataForStanding();
                        var centerRot = this.arm.F(i);
                        for (var j = 2 * (i + 1); j < 2 * (this.arm.N + 1) + 1; j++)
                        {
                            // TODO: omg it seems to me that the CenterX, CenterY and CenterZ no corresponds to my xyz axis ((
                            // Кажется проблема здеськак минимум в том, что оси у меня располагаются иначе дефолтовых
                            // Ещё возможна ошибка в rotAxis
                            ((this.armModelVisual3D[j].Transform as Transform3DGroup)
                             .Children[1] as RotateTransform3D)
                                .CenterX = centerRot.X;
                            ((this.armModelVisual3D[j].Transform as Transform3DGroup)
                             .Children[1] as RotateTransform3D)
                                .CenterY = centerRot.Y;
                            ((this.armModelVisual3D[j].Transform as Transform3DGroup)
                             .Children[1] as RotateTransform3D)
                                .CenterZ = centerRot.Z;

                            (((this.armModelVisual3D[j].Transform as Transform3DGroup)
                              .Children[1] as RotateTransform3D)
                             .Rotation as AxisAngleRotation3D)
                                .Axis = rotAxis;

                            (((this.armModelVisual3D[j].Transform as Transform3DGroup)
                              .Children[1] as RotateTransform3D)
                             .Rotation as AxisAngleRotation3D)
                                .Angle = MathFunctions.RadianToDegree(q[i]);
                        }

                        break;
                    case 'P':
                        var prismaticAxis = this.arm.GetZAxis(i);
                        for (var j = 2 * (i + 1); j < 2 * (this.arm.N + 1) + 1; j++)
                        {
                            if (j == 2 * (i + 1) + 1) continue;

                            ((armModelVisual3D[j].Transform as Transform3DGroup)
                             .Children[0] as TranslateTransform3D)
                                .OffsetX += prismaticAxis.X * q[i];
                            ((armModelVisual3D[j].Transform as Transform3DGroup)
                             .Children[0] as TranslateTransform3D)
                                .OffsetY += prismaticAxis.Y * q[i];
                            ((armModelVisual3D[j].Transform as Transform3DGroup)
                             .Children[0] as TranslateTransform3D)
                                .OffsetZ += prismaticAxis.Z * q[i];
                        }

                        break;
                }
            }
        }
    }
}
