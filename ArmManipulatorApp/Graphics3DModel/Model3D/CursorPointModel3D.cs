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
            this.ModelVisual3D = new ModelVisual3D { Content = circleCursor };
            this.position = position;
            this.viewport = viewport;
        }

        public void Show()
        {
            this.viewport.Children.Add(this.ModelVisual3D);
        }

        public void Hide()
        {
            this.viewport.Children.Remove(this.ModelVisual3D);
        }
        
        //TODO: do it with Transformation
        public void Move(Point3D offset)
        {
            var meshCircle = new MeshGeometry3D();
            MeshGeometry3DHelper.AddSphere(meshCircle, offset, 4, 8, 8);
            var brush3 = Brushes.Purple;
            var material3 = new DiffuseMaterial(brush3);
            var circleCursor = new GeometryModel3D(meshCircle, material3);
            this.ModelVisual3D = new ModelVisual3D { Content = circleCursor };
        }
    }
}
