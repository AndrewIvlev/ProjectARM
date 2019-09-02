using System.Windows;
using System.Windows.Controls;

namespace ArmManipulatorApp.Graphics3DModel.Model3D
{
    using System.Windows.Media.Media3D;

    public class CameraModel3D
    {
        public PerspectiveCamera PerspectiveCamera;

        public Point3D Position;
        public Vector3D LookDirection;
        public double FieldOfView;

        public ScaleTransform3D Zoom;
        public RotateTransform3D RotX;
        public AxisAngleRotation3D AngleRotX;
        public RotateTransform3D RotY;
        public AxisAngleRotation3D AngleRotY;

        public CameraModel3D(double distanceFromCenter)
        {
            this.PerspectiveCamera = new PerspectiveCamera();
            var position = new Point3D(distanceFromCenter, distanceFromCenter, distanceFromCenter);
            PerspectiveCamera.Position = position;
            var lookDirection = new Vector3D(- position.X, - position.Y, - position.Z);
            PerspectiveCamera.LookDirection = lookDirection;
            PerspectiveCamera.FieldOfView = 60;

            
            Zoom = new ScaleTransform3D {CenterX = 0, CenterY = 0, CenterZ = 0};

            RotY = new RotateTransform3D();
            var axisAngleRotY = new AxisAngleRotation3D {Axis = new Vector3D(0, 1, 0)};
            RotY.Rotation = axisAngleRotY;
            AngleRotY = new AxisAngleRotation3D {Axis = new Vector3D(0, 1, 0)};
            RotY.Rotation = AngleRotY;

            RotX = new RotateTransform3D();
            var axisAngleRotX = new AxisAngleRotation3D {Axis = new Vector3D(1, 0, 0)};
            RotX.Rotation = axisAngleRotX;
            AngleRotX = new AxisAngleRotation3D {Axis = new Vector3D(1, 0, 0)};
            RotX.Rotation = AngleRotX;

            var transformGroup = new Transform3DGroup();
            transformGroup.Children.Add(Zoom);
            transformGroup.Children.Add(RotY);
            transformGroup.Children.Add(RotX);
            PerspectiveCamera.Transform = transformGroup;
        }
    }
}
