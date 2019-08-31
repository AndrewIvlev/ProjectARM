namespace ArmManipulatorApp.Graphics3DModel.Model3D
{
    using System.Windows.Media.Media3D;

    public class CameraModel3D
    {
        public PerspectiveCamera PerspectiveCamera;

        public Point3D Position;
        public Vector3D LookDirection;
        public double FieldOfView;

        public CameraModel3D(double distanceFromCenter)
        {
            this.PerspectiveCamera = new PerspectiveCamera();
            var position = new Point3D(distanceFromCenter, distanceFromCenter / 2, distanceFromCenter);
            PerspectiveCamera.Position = position;
            var lookDirection = new Vector3D(- position.X, - position.Y, - position.Z);
            PerspectiveCamera.LookDirection = lookDirection;
            PerspectiveCamera.FieldOfView = 60;

            var rotY = new RotateTransform3D();
            var axisAngleRotY = new AxisAngleRotation3D {Axis = new Vector3D(0, 1, 0)};
            rotY.Rotation = axisAngleRotY;
            var rotX = new RotateTransform3D();
            var axisAngleRotX = new AxisAngleRotation3D {Axis = new Vector3D(1, 0, 0)};
            rotX.Rotation = axisAngleRotX;
            var zoom = new ScaleTransform3D {CenterX = 0, CenterY = 0, CenterZ = 0};
            
            var transformGroup = new Transform3DGroup();
            transformGroup.Children.Add(rotY);
            transformGroup.Children.Add(rotX);
            transformGroup.Children.Add(zoom);
            PerspectiveCamera.Transform = transformGroup;
        }

        public void Move()
        {

        }

        public void Zoom()
        {

        }
    }
}
