namespace ArmManipulatorApp.Graphics3DModel.Model3D
{
    using System.Windows.Media.Media3D;

    public class CameraModel3D
    {
        public PerspectiveCamera PerspectiveCamera;

        public Point3D Position;
        public Vector3D LookDirection;
        public double FieldOfView;
        public double DefaultDistanceFromCenter;

        public ScaleTransform3D Zoom;
        public RotateTransform3D RotX;
        public AxisAngleRotation3D AngleRotX;
        public RotateTransform3D RotY;
        public AxisAngleRotation3D AngleRotY;
        public RotateTransform3D RotZ;
        public AxisAngleRotation3D AngleRotZ;

        public CameraModel3D(double distanceFromCenter)
        {
            this.PerspectiveCamera = new PerspectiveCamera();
            this.DefaultDistanceFromCenter = distanceFromCenter;
            var position = new Point3D(distanceFromCenter, distanceFromCenter, distanceFromCenter);
            this.PerspectiveCamera.Position = position;
            var lookDirection = new Vector3D(-position.X, -position.Y, -position.Z);
            this.PerspectiveCamera.LookDirection = lookDirection;
            this.PerspectiveCamera.UpDirection = new Vector3D(0, 0, 1);
            this.PerspectiveCamera.FieldOfView = 60;

            this.Zoom = new ScaleTransform3D { CenterX = 0, CenterY = 0, CenterZ = 0 };

            this.RotX = new RotateTransform3D();
            var axisAngleRotX = new AxisAngleRotation3D { Axis = new Vector3D(1, 0, 0) };
            this.RotX.Rotation = axisAngleRotX;
            this.AngleRotX = new AxisAngleRotation3D { Axis = new Vector3D(1, 0, 0) };
            this.RotX.Rotation = this.AngleRotX;

            this.RotY = new RotateTransform3D();
            var axisAngleRotY = new AxisAngleRotation3D { Axis = new Vector3D(0, 1, 0) };
            this.RotY.Rotation = axisAngleRotY;
            this.AngleRotY = new AxisAngleRotation3D { Axis = new Vector3D(0, 1, 0) };
            this.RotY.Rotation = this.AngleRotY;

            this.RotZ = new RotateTransform3D();
            var axisAngleRotZ = new AxisAngleRotation3D { Axis = new Vector3D(0, 0, 1) };
            this.RotZ.Rotation = axisAngleRotZ;
            this.AngleRotZ = new AxisAngleRotation3D { Axis = new Vector3D(0, 0, 1) };
            this.RotZ.Rotation = this.AngleRotZ;

            var transformGroup = new Transform3DGroup();
            transformGroup.Children.Add(this.Zoom);
            transformGroup.Children.Add(this.RotY);
            transformGroup.Children.Add(this.RotX);
            transformGroup.Children.Add(this.RotZ);
            this.PerspectiveCamera.Transform = transformGroup;
        }

        public void ViewFromAbove()
        {            
            var position = new Point3D(0, 0, this.DefaultDistanceFromCenter);
            this.PerspectiveCamera.Position = position;
            var lookDirection = new Vector3D(0, 0, -this.DefaultDistanceFromCenter);
            this.PerspectiveCamera.LookDirection = lookDirection;
            this.PerspectiveCamera.UpDirection = new Vector3D(0, 1, 0);
        }

        public void DefaultView()
        {
            var position = new Point3D(this.DefaultDistanceFromCenter, this.DefaultDistanceFromCenter, this.DefaultDistanceFromCenter);
            this.PerspectiveCamera.Position = position;
            var lookDirection = new Vector3D(-position.X, -position.Y, -position.Z);
            this.PerspectiveCamera.LookDirection = lookDirection;
            this.PerspectiveCamera.UpDirection = new Vector3D(0, 0, 1);
        }
    }
}
