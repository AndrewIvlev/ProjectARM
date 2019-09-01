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
        public RotateTransform3D RotY;

        public CameraModel3D(double distanceFromCenter)
        {
            this.PerspectiveCamera = new PerspectiveCamera();
            var position = new Point3D(distanceFromCenter, distanceFromCenter / 2, distanceFromCenter);
            PerspectiveCamera.Position = position;
            var lookDirection = new Vector3D(- position.X, - position.Y / 2, - position.Z);
            PerspectiveCamera.LookDirection = lookDirection;
            PerspectiveCamera.FieldOfView = 60;

            
            Zoom = new ScaleTransform3D {CenterX = 0, CenterY = 0, CenterZ = 0};

            RotY = new RotateTransform3D();
            var axisAngleRotY = new AxisAngleRotation3D {Axis = new Vector3D(0, 1, 0)};
            RotY.Rotation = axisAngleRotY;

            RotX = new RotateTransform3D();
            var axisAngleRotX = new AxisAngleRotation3D {Axis = new Vector3D(1, 0, 0)};
            RotX.Rotation = axisAngleRotX;
            
            var transformGroup = new Transform3DGroup();
            transformGroup.Children.Add(Zoom);
            transformGroup.Children.Add(RotY);
            transformGroup.Children.Add(RotX);
            PerspectiveCamera.Transform = transformGroup;
        }
    }
}
