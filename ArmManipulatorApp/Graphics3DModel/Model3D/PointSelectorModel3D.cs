namespace ArmManipulatorApp.Graphics3DModel.Model3D
{
    using System.Windows.Controls;
    using System.Windows.Media.Media3D;

    public class PointSelectorModel3D : CursorPointModel3D
    {
        public int selectedPointIndex;

        public PointSelectorModel3D(Viewport3D viewport, Point3D position)
            : base(viewport, position)
        {
            this.selectedPointIndex = 0;
        }
    }
}
