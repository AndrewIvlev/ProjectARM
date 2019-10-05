namespace ArmManipulatorApp.Graphics3DModel.Model3D
{
    using System.Windows.Controls;
    using System.Windows.Media.Media3D;

    public class PointSelectorModel3D : CursorPointModel3D
    {
        public int selectedPointIndex;

        public PointSelectorModel3D(Point3D position)
            : base(position)
        {
            this.selectedPointIndex = 0;
        }
    }
}
