using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ArmManipulatorApp.Graphics3DModel.Model3D
{
    using System.Windows.Media.Media3D;
    
    /// <summary>
    /// Surface plane and coordinate system
    /// </summary>
    public class SceneModel3D
    {
        public ModelVisual3D ModelVisual3D;

        public SceneModel3D(double sideLength)
        {
            this.ModelVisual3D = new ModelVisual3D();
            var sceneGeometryModel3D = new GeometryModel3D();
            var sceneMeshGeometry3D = new MeshGeometry3D
            {
                TriangleIndices = Int32Collection.Parse("0, 1, 2, 2, 3, 0"),
                Positions = Point3DCollection.Parse(
                    $"-{sideLength},0,{sideLength} " +
                    $"{sideLength},0,{sideLength} " +
                    $"{sideLength},0,-{sideLength} " +
                    $"-{sideLength},0,-{sideLength}")
            };
            var sceneMaterialGroup = new MaterialGroup();
            sceneMaterialGroup.Children.Add(new DiffuseMaterial(Brushes.DimGray));
            sceneGeometryModel3D.Geometry = sceneMeshGeometry3D;
            sceneGeometryModel3D.Material = sceneMaterialGroup;
            ModelVisual3D.Content = sceneGeometryModel3D;
        }
    }
}
