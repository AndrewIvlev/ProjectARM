namespace ArmManipulatorApp.Graphics3DModel.Model3D
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    using MainApp.Graphics3DModel.Model3D;

    public class CursorPointModel3D
    {
        public ModelVisual3D ModelVisual3D;

        private Viewport3D viewport;

        public Point3D position;

        public CursorPointModel3D(Viewport3D viewport, Point3D position)
        {
            var meshCircle = new MeshGeometry3D();
            MeshGeometry3DHelper.AddSphere(meshCircle, position, 4, 8, 8);
            var brush3 = Brushes.Purple;
            var material3 = new DiffuseMaterial(brush3);
            var circleCursor = new GeometryModel3D(meshCircle, material3);
            var translateTransform = new TranslateTransform3D();
            this.ModelVisual3D = new ModelVisual3D { Content = circleCursor, Transform = translateTransform};
            this.position = position;
            this.viewport = viewport;
        }
        
        public void Move(Point3D offset)
        {
            ((TranslateTransform3D)this.ModelVisual3D.Transform).OffsetX += offset.X;
            ((TranslateTransform3D)this.ModelVisual3D.Transform).OffsetY += offset.Y;
            ((TranslateTransform3D)this.ModelVisual3D.Transform).OffsetZ += offset.Z;
        }
    }
}
