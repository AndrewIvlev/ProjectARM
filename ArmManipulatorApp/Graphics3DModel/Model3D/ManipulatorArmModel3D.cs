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

        public void ClearModelVisual3DCollection()
        {
            this.armModelVisual3D.Clear();
        }

        public void BuildModelVisual3DCollection(double thickness)
        {
            var boneRadius = thickness * this.coeff / 2;
            var jointRadius = (thickness * this.coeff / 2) * 1.7;

            var sup = new Point3D(0, 0, 0); // Start Unit Point
            var eup = (Point3D)this.arm.F(0); // End Unit Point

            // Zero unit of arm:
            this.armModelVisual3D.Add(
                this.CreateArmBoneModelVisual3D(
                    VRConvert.ConvertFromRealToVirtual(sup, this.coeff),
                    VRConvert.ConvertFromRealToVirtual(eup, this.coeff),
                    boneRadius));
            var jointBrush = Brushes.Black;
            this.armModelVisual3D.Add(
                this.CreateArmJointModelVisual3D(VRConvert.ConvertFromRealToVirtual(eup, this.coeff), jointRadius, jointBrush));

            sup = eup;

            // 'P' and 'R' units:
            for (var i = 0; i < this.arm.N; i++)
            {
                eup = (Point3D)this.arm.F(i + 1);
                
                jointBrush = this.arm.Units[i].Type == 'R' ? Brushes.OrangeRed : Brushes.Green;
                this.armModelVisual3D.Add(
                    this.CreateArmJointModelVisual3D(
                        VRConvert.ConvertFromRealToVirtual(sup, this.coeff),
                        jointRadius,
                        jointBrush,
                        i + 1));

                this.armModelVisual3D.Add(
                    this.CreateArmBoneModelVisual3D(
                        VRConvert.ConvertFromRealToVirtual(sup, this.coeff),
                        VRConvert.ConvertFromRealToVirtual(eup, this.coeff),
                        boneRadius,
                        i + 1));

                sup = eup;
            }
            
            // Grip unit:
            jointBrush = Brushes.Blue;
            this.armModelVisual3D.Add(
                this.CreateArmJointModelVisual3D(
                    VRConvert.ConvertFromRealToVirtual(sup, this.coeff),
                    jointRadius,
                    jointBrush,
                    this.arm.N));
        }

        private ModelVisual3D CreateArmBoneModelVisual3D(Point3D start, Point3D end, double radius, int addTransforms = 0)
        {
            var bone = new ModelVisual3D();
            var boneMesh = new MeshGeometry3D();
            MeshGeometry3DHelper.AddSmoothCylinder(
                boneMesh,
                start,
                new Vector3D(end.X - start.X, end.Y - start.Y, end.Z - start.Z),
                radius);
            var boneMaterial = new DiffuseMaterial(Brushes.CornflowerBlue);
            var boneGeometryModel = new GeometryModel3D(boneMesh, boneMaterial);
            bone.Content = boneGeometryModel;

            return this.AddTransformGroup(bone, addTransforms);
        }

        private ModelVisual3D CreateArmJointModelVisual3D(Point3D center, double radius, SolidColorBrush brush, int addTransforms = 0)
        {
            var joint = new ModelVisual3D();
            var jointMesh = new MeshGeometry3D();
            MeshGeometry3DHelper.AddSphere(jointMesh, center, radius, 8, 8);
            var jointMaterial = new DiffuseMaterial(brush);
            var jointGeometryModel = new GeometryModel3D(jointMesh, jointMaterial);
            joint.Content = jointGeometryModel;

            return this.AddTransformGroup(joint, addTransforms);
        }

        private ModelVisual3D AddTransformGroup(ModelVisual3D mv, int countOfTransforms)
        {
            var transGroup = new Transform3DGroup();
            for (var i = 0; i < countOfTransforms; i++)
            {
                transGroup.Children.Add(new TranslateTransform3D());

                var rotTrans = new RotateTransform3D { Rotation = new AxisAngleRotation3D() };
                transGroup.Children.Add(rotTrans);
            }
            
            mv.Transform = transGroup;

            return mv;
        }

        public void BeginAnimation(double[] dQ)
        {
            for (var i = 0; i < this.arm.N; i++)
            {
                switch (this.arm.Units[i].Type)
                {
                    case 'R':
                        var rotAxis = VRConvert.ConvertFromRealToVirtual(this.arm.GetZAxis(i), this.coeff);
                        var centerRot = VRConvert.ConvertFromRealToVirtual((Point3D)this.arm.F(i), this.coeff);
                        for (var j = 2 * (i + 1); j < 2 * (this.arm.N + 1) + 1; j++)
                        {
                            ((this.armModelVisual3D[j]
                                  .Transform as Transform3DGroup)
                             .Children[2 * i + 1] as RotateTransform3D)
                                .CenterX = centerRot.X;
                            ((this.armModelVisual3D[j]
                                  .Transform as Transform3DGroup)
                             .Children[2 * i + 1] as RotateTransform3D)
                                .CenterY = centerRot.Y;
                            ((this.armModelVisual3D[j]
                                  .Transform as Transform3DGroup)
                             .Children[2 * i + 1] as RotateTransform3D)
                                .CenterZ = centerRot.Z;

                            (((this.armModelVisual3D[j]
                                   .Transform as Transform3DGroup)
                              .Children[2 * i + 1] as RotateTransform3D)
                             .Rotation as AxisAngleRotation3D)
                                .Axis = rotAxis;

                            (((this.armModelVisual3D[j].Transform as Transform3DGroup)
                              .Children[2 * i + 1] as RotateTransform3D)
                             .Rotation as AxisAngleRotation3D)
                                .Angle = MathFunctions.RadianToDegree(dQ[i]);
                        }

                        break;
                    case 'P':
                        var prismaticAxis = VRConvert.ConvertFromRealToVirtual(this.arm.GetZAxis(i), this.coeff);
                        for (var j = 2 * (i + 1); j < 2 * (this.arm.N + 1) + 1; j++)
                        {
                            if (j == 2 * (i + 1) + 1) continue;

                            ((this.armModelVisual3D[j]
                                  .Transform as Transform3DGroup)
                             .Children[2 * i] as TranslateTransform3D)
                                .OffsetX = prismaticAxis.X * dQ[i];
                            ((this.armModelVisual3D[j]
                                  .Transform as Transform3DGroup)
                             .Children[2 * i] as TranslateTransform3D)
                                .OffsetY = prismaticAxis.Y * dQ[i];
                            ((this.armModelVisual3D[j]
                                  .Transform as Transform3DGroup)
                             .Children[2 * i] as TranslateTransform3D)
                                .OffsetZ = prismaticAxis.Z * dQ[i];
                        }

                        break;
                }
            }
        }
    }
}
