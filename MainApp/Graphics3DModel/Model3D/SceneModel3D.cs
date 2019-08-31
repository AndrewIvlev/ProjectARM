using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using MainApp.Graphics3DModel.Model3D;

namespace ArmManipulatorApp.Graphics3DModel.Model3D
{
    using System.Windows.Media.Media3D;
    
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
            var planeGeometryModel3D = new GeometryModel3D();
            var planeMeshGeometry3D = new MeshGeometry3D();
            MeshGeometry3DHelper.AddTriangle(planeMeshGeometry3D,
                new Point3D(-sideLength, 0, sideLength),
                new Point3D(sideLength, 0, sideLength),
                new Point3D(sideLength, 0, -sideLength));
            MeshGeometry3DHelper.AddTriangle(planeMeshGeometry3D,
                new Point3D(-sideLength, 0, sideLength),
                new Point3D(-sideLength, 0, -sideLength),
                new Point3D(sideLength, 0, -sideLength));
            planeGeometryModel3D.Geometry = planeMeshGeometry3D;
            planeGeometryModel3D.Material = new DiffuseMaterial(Brushes.DimGray);
            
            // Coordinate system building
            // Ox axis
            var xAxisGeometryModel3D = new GeometryModel3D();
            var xAxisMeshGeometry3D = new MeshGeometry3D();
            MeshGeometry3DHelper.AddParallelepiped(xAxisMeshGeometry3D,
                new Point3D(), new Point3D(sideLength, 0, 0), axisThickness);
            xAxisGeometryModel3D.Geometry = xAxisMeshGeometry3D;
            xAxisGeometryModel3D.Material = new DiffuseMaterial(Brushes.Green);

            // Oy axis
            var yAxisGeometryModel3D = new GeometryModel3D();
            var yAxisMeshGeometry3D = new MeshGeometry3D();
            MeshGeometry3DHelper.AddParallelepiped(yAxisMeshGeometry3D,
                new Point3D(), new Point3D(0, sideLength, 0), axisThickness);
            yAxisGeometryModel3D.Geometry = yAxisMeshGeometry3D;
            yAxisGeometryModel3D.Material = new DiffuseMaterial(Brushes.Red);

            // Oz axis
            var zAxisGeometryModel3D = new GeometryModel3D();
            var zAxisMeshGeometry3D = new MeshGeometry3D();
            MeshGeometry3DHelper.AddParallelepiped(zAxisMeshGeometry3D,
                new Point3D(), new Point3D(0, 0, sideLength), axisThickness);
            zAxisGeometryModel3D.Geometry = zAxisMeshGeometry3D;
            zAxisGeometryModel3D.Material = new DiffuseMaterial(Brushes.Blue);

            sceneModel3DGroup.Children.Add(planeGeometryModel3D);
            sceneModel3DGroup.Children.Add(xAxisGeometryModel3D);
            sceneModel3DGroup.Children.Add(yAxisGeometryModel3D);
            sceneModel3DGroup.Children.Add(zAxisGeometryModel3D);
            ModelVisual3D.Content = sceneModel3DGroup;
        }
    }
}
