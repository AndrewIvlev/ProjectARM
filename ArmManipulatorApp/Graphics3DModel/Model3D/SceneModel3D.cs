namespace ArmManipulatorApp.Graphics3DModel.Model3D
{
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    using MainApp.Graphics3DModel.Model3D;

    /// <summary>
    /// Surface plane and coordinate system
    /// </summary>
    public class SceneModel3D
    {
        public ModelVisual3D ModelVisual3D;

        public SceneModel3D(double sideLength, double axisThickness)
        {
            this.ModelVisual3D = new ModelVisual3D();
            var sceneModel3DGroup = new Model3DGroup();

            // Scene plane building
            var halfSideLength = sideLength / 2;
            var axisRadius = axisThickness / 2;
            var planeGeometryModel3D = new GeometryModel3D();
            var planeMeshGeometry3D = new MeshGeometry3D();
            MeshGeometry3DHelper.AddTriangle(
                planeMeshGeometry3D,
                new Point3D(halfSideLength, -halfSideLength, 0),
                new Point3D(halfSideLength, halfSideLength, 0),
                new Point3D(-halfSideLength, halfSideLength, 0));
            MeshGeometry3DHelper.AddTriangle(
                planeMeshGeometry3D,
                new Point3D(halfSideLength, -halfSideLength, 0),
                new Point3D(-halfSideLength, halfSideLength, 0),
                new Point3D(-halfSideLength, -halfSideLength, 0));
            planeGeometryModel3D.Geometry = planeMeshGeometry3D;
            planeGeometryModel3D.Material = new DiffuseMaterial(Brushes.DimGray);

            // Ox axis
            var xAxisGeometryModel3D = new GeometryModel3D();
            var xAxisMeshGeometry3D = new MeshGeometry3D();
            MeshGeometry3DHelper.AddSmoothCylinder(
                xAxisMeshGeometry3D,
                new Point3D(0, 0, 0),
                new Vector3D(halfSideLength, 0, 0),
                axisRadius);
            xAxisGeometryModel3D.Geometry = xAxisMeshGeometry3D;
            xAxisGeometryModel3D.Material = new DiffuseMaterial(Brushes.Green);

            // Oy axis
            var yAxisGeometryModel3D = new GeometryModel3D();
            var yAxisMeshGeometry3D = new MeshGeometry3D();
            MeshGeometry3DHelper.AddSmoothCylinder(
                yAxisMeshGeometry3D,
                new Point3D(0, 0, 0), 
                new Vector3D(0, halfSideLength, 0),
                axisRadius);
            yAxisGeometryModel3D.Geometry = yAxisMeshGeometry3D;
            yAxisGeometryModel3D.Material = new DiffuseMaterial(Brushes.Red);

            // Oz axis
            var zAxisGeometryModel3D = new GeometryModel3D();
            var zAxisMeshGeometry3D = new MeshGeometry3D();
            MeshGeometry3DHelper.AddSmoothCylinder(
                zAxisMeshGeometry3D,
                new Point3D(0, 0, 0), 
                new Vector3D(0, 0, halfSideLength),
                axisRadius);
            zAxisGeometryModel3D.Geometry = zAxisMeshGeometry3D;
            zAxisGeometryModel3D.Material = new DiffuseMaterial(Brushes.Blue);

            sceneModel3DGroup.Children.Add(planeGeometryModel3D);
            sceneModel3DGroup.Children.Add(xAxisGeometryModel3D);
            sceneModel3DGroup.Children.Add(yAxisGeometryModel3D);
            sceneModel3DGroup.Children.Add(zAxisGeometryModel3D);
            this.ModelVisual3D.Content = sceneModel3DGroup;
        }
    }
}
