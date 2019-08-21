using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.Graphics.Model3D
{
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    using ManipulationSystemLibrary;

    public class TrajectoryModel3D
    {
        public Trajectory track;

        private ModelVisual3D trajectoryPointCursor;
        private List<Point3D> listTrajectoryPoints; // list for spliting trajectory
        private class TrajectoryPoint
        {
            public ModelVisual3D trajectoryModelVisual3D;
            public Point3D center;

            public void SetY(double y)
            {
                center.Y = y;
            }
        }
        private List<TrajectoryPoint> trajectoryPointsVisual3D;
        private int indexTrajectoryPoint; //TODO: remove it, do smarter

        struct TrajectoryLine
        {
            public ModelVisual3D lineModelVisual3D;
            public Point3D start;
            public Point3D end;
        }
        private List<TrajectoryLine> trajectoryLinesVisual3D;
        private List<Point3D> listSplitTrajectoryPoints;

        private double trajectoryLenght;

        public TrajectoryModel3D(Trajectory track)
        {
            this.track = track;
            this.trajectoryLenght = 0;
            this.listTrajectoryPoints = new List<Point3D>();
        }

        private void ShowSplitPath(List<Point3D> listSplitPathPoints)
        {
            foreach (var p in listSplitPathPoints)
            {
                //TODO: Extract to method next 8 line
                var point = new MeshGeometry3D();
                AddSphere(point, new Point3D(p.X, p.Y, p.Z), 0.2, 8, 8);
                var pointBrush = Brushes.DarkRed;
                var pointMaterial = new DiffuseMaterial(pointBrush);
                var pathPointGeometryModel = new GeometryModel3D(point, pointMaterial);
                var pathPointModelVisual3D = new ModelVisual3D();
                pathPointModelVisual3D.Content = pathPointGeometryModel;
                pathPointModelVisual3D.Transform = new TranslateTransform3D();
                Viewport3D.Children.Add(pathPointModelVisual3D);
            }
        }
    }
}
