using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ManipulationSystemLibrary;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace ManipApp
{
    using System.Collections.Generic;
    using System.Windows.Media.Animation;
    using OxyPlot;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 0 - camera rotation;
        /// 1 - path creation;
        /// 2 - path editing;
        /// </summary>
        private byte MouseMod;
        private Point MousePos;
        private Point offset;
        private ModelVisual3D pathPointCursor;
        private List<ModelVisual3D> manipModelVisual3D;

        private MatrixMathModel model;
        private List<double[]> listQ; // Generalized coordinates vector

        public MainWindow()
        {
            InitializeComponent();
            manipModelVisual3D = new List<ModelVisual3D>();
            listQ = new List<double[]>();
            offset = new Point(540, 405);
            MouseMod = 0;
        }

        #region Menu Strip

        #region Manipulator

        private void OpenManipulatorFile_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() != true) return;
            var jsonStringMatrixMathModel = File.ReadAllText(openFileDialog.FileName);
            var manipConfig = JsonConvert.DeserializeObject<MatrixMathModel>(jsonStringMatrixMathModel);
            model = new MatrixMathModel(manipConfig);
            model.DefaultA();
            model.CalculationMetaData();
            CreateManipulator3DVisualModel(model);
        }

        #endregion

        #region Path

        private void NewPath_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            RotX.Angle = -78;
            RotY.Angle = -45;
            MouseMod = 1;
        }

        private void SplitPathByPointsQty_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SplitPathWithStep_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
        
        #endregion

        #endregion

        private void PathPlanningButton_Click(object sender, RoutedEventArgs e)
        {
            if (model == null)
            {
                MessageBox.Show("Firstly create manipulator model!");
                return;
            }

            var listPathPoints = new List<Point3D>
            {
                new Point3D(43, 0, 0),
                new Point3D(42.5, 0.5, 0.3),
                new Point3D(42, 0.7, 0.5),
                new Point3D(42, 1, 0.8),
                new Point3D(42, 2, 1),
                new Point3D(42, 2, 1),
                new Point3D(42, 2, 1),
                new Point3D(42, 2, 1)
            };

            listQ.Clear();
            foreach (var pathPoint in listPathPoints)
            {
                model.CalculationMetaData();
                model.LagrangeMethodToThePoint(pathPoint);
                listQ.Add(model.q);
            }
        }

        #region Canvas Events

        /// <summary>
        /// По клику ЛКМ по сцене мы либо перемещаем камеру,
        /// либо создаём траекторию пути, либо редактируем траекторию пути.
        /// </summary>
        private void Canvas_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            switch (MouseMod)
            {
                case 0:
                    MousePos = e.GetPosition(this);
                    break;
                case 1:
                    var myModel3DGroup = new Model3DGroup();
                    var myModelVisual3D = new ModelVisual3D();
                    var mesh = new MeshGeometry3D();

                    PointHitTestParameters hitParams = new PointHitTestParameters(e.GetPosition(this));
                    var myGeometryModel = GetCircleModel(0.1, new Vector3D(0, 1, 0),
                        new Point3D((hitParams.HitPoint.X - offset.X) * 0.0216049, 0, (hitParams.HitPoint.Y - offset.Y) * 0.0216049), 14);
                    myModel3DGroup.Children.Add(myGeometryModel);
                    myModelVisual3D.Content = myModel3DGroup;
                    this.Viewport3D.Children.Add(myModelVisual3D);
                    //path.AddAnchorPoint(e.GetPosition());
                    //if (path.GetLen() != 0) label3.Text = $"Trajectory lenght = {(CoeftoRealW() * path.GetLen()).ToString("#.0000000000")}cm";
                    //comboBox1.Items.Clear();
                    //path.Show(PicBoxGraphics);
                    break;
                case 2:
                    //if (MousePressed == 0)
                    //{
                    //    index = path.NearestPointIndex(e.Location);
                    //    if (Math.Pow((e.Location.X - path.AnchorPoints[index].X), 2) +
                    //        Math.Pow((e.Location.Y + path.AnchorPoints[index].Y), 2) <= 40)
                    //    {
                    //        path.AnchorPoints[index] = e.Location;
                    //    }
                    //    MousePressed = 1;
                    //    return;
                    //}
                    //if (MousePressed == 1)
                    //{
                    //    MousePressed = 0;
                    //    return;
                    //}
                    break;
                case 3:
                    //following the cursor
                    break;
                default:
                    break;
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            switch (MouseMod)
            {
                case 0:
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        //Moving mouse with holding mouse button
                        var nextMousePos = e.GetPosition(this);
                        var dxdy = new Point(nextMousePos.X - MousePos.X, nextMousePos.Y - MousePos.Y);
                        RotX.Angle += dxdy.Y;
                        RotY.Angle += dxdy.X;
                        //if (RotX.Angle > 90 || -90 > RotX.Angle) RotX.Angle = 0;
                        //if (RotY.Angle > 180 || -180 > RotY.Angle) RotY.Angle = 0;
                        MousePos = nextMousePos;
                    }
                    else if (e.LeftButton == MouseButtonState.Released)
                    {
                        //Moving mouse when mouse button up
                    }
                    break;
                case 1:
                    if(pathPointCursor != null)
                        this.Viewport3D.Children.Remove(pathPointCursor);
                    var myModel3DGroup = new Model3DGroup();
                    pathPointCursor = new ModelVisual3D();

                    PointHitTestParameters hitParams = new PointHitTestParameters(e.GetPosition(this));
                    var myGeometryModel = GetCircleModel(0.1, new Vector3D(0, 1, 0),
                        new Point3D((hitParams.HitPoint.X - offset.X) * 0.0216049, 0, (hitParams.HitPoint.Y - offset.Y) * 0.0216049), 14);
                    myModel3DGroup.Children.Add(myGeometryModel);
                    pathPointCursor.Content = myModel3DGroup;
                    this.Viewport3D.Children.Add(pathPointCursor);
                    break;
            }
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
        }

        // TODO: fix this method
        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Camera zoom
            this.ScaleTransform3D.ScaleX += e.Delta / 120;
            this.ScaleTransform3D.ScaleY += e.Delta / 120;
            this.ScaleTransform3D.ScaleZ += e.Delta / 120;
        }

        #endregion

        #region Graphics


        private void ManipulatorMoveAnimation()
        {
            var timelineCollection = new TimelineCollection();
            for (int i = 1; i < model.n; i++)
            {
                var animation1 = new ThicknessAnimation();
                animation1.From = new Thickness(5);
                animation1.To = new Thickness(25);
                animation1.Duration = TimeSpan.FromSeconds(5);
                //Storyboard.SetTarget(animation1, button1);
                Storyboard.SetTargetProperty(animation1, new PropertyPath(MarginProperty));
            }

            var storyboard = new Storyboard();
            storyboard.Children = timelineCollection;

            storyboard.Begin();
        }

        /// <summary>
        /// Add MatrixTransform3D for each unit of manipulatorModelVisual3D
        /// </summary>
        private void AddMatrixTransformationForManipulator()
        {
            foreach (var arm in manipModelVisual3D)
            {
                var i = manipModelVisual3D.IndexOf(arm);
                var matrixTransform3D = new MatrixTransform3D();
                var endMatrix3D = BlockMatrixConvert(model.T[i] as BlockMatrix);
                matrixTransform3D.Matrix = endMatrix3D;
                arm.Transform = matrixTransform3D;
            }
            var shit = this.Viewport3D;
        }

        private Matrix3D BlockMatrixConvert(BlockMatrix bm) => new Matrix3D
        {
            M11 = bm[0, 0], M21 = bm[0, 1], M31 = bm[0, 2], OffsetX = bm[0, 3],
            M12 = bm[1, 0], M22 = bm[1, 1], M32 = bm[1, 2], OffsetY = bm[1, 3],
            M13 = bm[2, 0], M23 = bm[2, 1], M33 = bm[2, 2], OffsetZ = bm[2, 3],
            M14 = 0,        M24 = 0,        M34 = 0,        M44 = 1
        };

        private void CreateManipulator3DVisualModel(MatrixMathModel model)
        {
            if (manipModelVisual3D.Count > 0)
            {
                foreach (var arm in manipModelVisual3D)
                    this.Viewport3D.Children.Remove(arm);
            }
            
            var coeff = 0.5;
            var OffsetY = 0.5;
            var sup = new Point3D(0, 0, 0); // startUnitPoint
            for (int i = 0; i < model.n; i++)
            {
                var arm = new ModelVisual3D();
                var jointsAndUnitsModelGroup = new Model3DGroup();
                var unit = new MeshGeometry3D();
                var joint = new MeshGeometry3D();

                var eup = model.F(i); // endUnitPoint
                LineByTwoPoints(unit, new Point3D(sup.X * coeff, sup.Y * coeff + OffsetY, sup.Z * coeff), new Point3D(eup.X * coeff, eup.Y * coeff + OffsetY, eup.Z * coeff));
                AddSphere(joint, new Point3D(eup.X * coeff, eup.Y * coeff + OffsetY, eup.Z * coeff), 0.4, 8, 8);
                sup = eup;

                var unitBrush = Brushes.CornflowerBlue;
                var unitMaterial = new DiffuseMaterial(unitBrush);
                var myGeometryModel = new GeometryModel3D(unit, unitMaterial);
                jointsAndUnitsModelGroup.Children.Add(myGeometryModel);

                var jointBrush = Brushes.OrangeRed;
                var jointMaterial = new DiffuseMaterial(jointBrush);
                myGeometryModel = new GeometryModel3D(joint, jointMaterial);
                jointsAndUnitsModelGroup.Children.Add(myGeometryModel);

                arm.Content = jointsAndUnitsModelGroup;
                this.Viewport3D.Children.Add(arm);
                manipModelVisual3D.Add(arm);
            }
        }

        // Set the vector's length.
        private Vector3D ScaleVector(Vector3D vector, double length)
        {
            double scale = length / vector.Length;
            return new Vector3D(
                vector.X * scale,
                vector.Y * scale,
                vector.Z * scale);
        }

        // Add a triangle to the indicated mesh.
        // Do not reuse points so triangles don't share normals.
        private void AddTriangle(MeshGeometry3D mesh, Point3D point1, Point3D point2, Point3D point3)
        {
            // Create the points.
            int index1 = mesh.Positions.Count;
            mesh.Positions.Add(point1);
            mesh.Positions.Add(point2);
            mesh.Positions.Add(point3);

            // Create the triangle.
            mesh.TriangleIndices.Add(index1++);
            mesh.TriangleIndices.Add(index1++);
            mesh.TriangleIndices.Add(index1);
        }// Make a thin rectangular prism between the two points.
        
        // If extend is true, extend the segment by half the
        // thickness so segments with the same end points meet nicely.
        private void AddSegment(MeshGeometry3D mesh,
            Point3D point1, Point3D point2, Vector3D up)
        {
            AddSegment(mesh, point1, point2, up, false);
        }

        private void AddSegment(MeshGeometry3D mesh,
            Point3D point1, Point3D point2, Vector3D up,
            bool extend)
        {
            const double thickness = 0.25;

            // Get the segment's vector.
            Vector3D v = point2 - point1;

            if (extend)
            {
                // Increase the segment's length on both ends by thickness / 2.
                Vector3D n = ScaleVector(v, thickness / 2.0);
                point1 -= n;
                point2 += n;
            }

            // Get the scaled up vector.
            Vector3D n1 = ScaleVector(up, thickness / 2.0);

            // Get another scaled perpendicular vector.
            Vector3D n2 = Vector3D.CrossProduct(v, n1);
            n2 = ScaleVector(n2, thickness / 2.0);

            // Make a skinny box.
            // p1pm means point1 PLUS n1 MINUS n2.
            Point3D p1pp = point1 + n1 + n2;
            Point3D p1mp = point1 - n1 + n2;
            Point3D p1pm = point1 + n1 - n2;
            Point3D p1mm = point1 - n1 - n2;
            Point3D p2pp = point2 + n1 + n2;
            Point3D p2mp = point2 - n1 + n2;
            Point3D p2pm = point2 + n1 - n2;
            Point3D p2mm = point2 - n1 - n2;

            // Sides.
            AddTriangle(mesh, p1pp, p1mp, p2mp);
            AddTriangle(mesh, p1pp, p2mp, p2pp);

            AddTriangle(mesh, p1pp, p2pp, p2pm);
            AddTriangle(mesh, p1pp, p2pm, p1pm);

            AddTriangle(mesh, p1pm, p2pm, p2mm);
            AddTriangle(mesh, p1pm, p2mm, p1mm);

            AddTriangle(mesh, p1mm, p2mm, p2mp);
            AddTriangle(mesh, p1mm, p2mp, p1mp);

            // Ends.
            AddTriangle(mesh, p1pp, p1pm, p1mm);
            AddTriangle(mesh, p1pp, p1mm, p1mp);

            AddTriangle(mesh, p2pp, p2mp, p2mm);
            AddTriangle(mesh, p2pp, p2mm, p2pm);
        }
        
        // Add a sphere.
        private void AddSphere(MeshGeometry3D mesh, Point3D center,
            double radius, int num_phi, int num_theta)
        {
            double phi0, theta0;
            double dphi = Math.PI / num_phi;
            double dtheta = 2 * Math.PI / num_theta;

            phi0 = 0;
            double y0 = radius * Math.Cos(phi0);
            double r0 = radius * Math.Sin(phi0);
            for (int i = 0; i < num_phi; i++)
            {
                double phi1 = phi0 + dphi;
                double y1 = radius * Math.Cos(phi1);
                double r1 = radius * Math.Sin(phi1);

                // Point ptAB has phi value A and theta value B.
                // For example, pt01 has phi = phi0 and theta = theta1.
                // Find the points with theta = theta0.
                theta0 = 0;
                Point3D pt00 = new Point3D(
                    center.X + r0 * Math.Cos(theta0),
                    center.Y + y0,
                    center.Z + r0 * Math.Sin(theta0));
                Point3D pt10 = new Point3D(
                    center.X + r1 * Math.Cos(theta0),
                    center.Y + y1,
                    center.Z + r1 * Math.Sin(theta0));
                for (int j = 0; j < num_theta; j++)
                {
                    // Find the points with theta = theta1.
                    double theta1 = theta0 + dtheta;
                    Point3D pt01 = new Point3D(
                        center.X + r0 * Math.Cos(theta1),
                        center.Y + y0,
                        center.Z + r0 * Math.Sin(theta1));
                    Point3D pt11 = new Point3D(
                        center.X + r1 * Math.Cos(theta1),
                        center.Y + y1,
                        center.Z + r1 * Math.Sin(theta1));

                    // Create the triangles.
                    AddTriangle(mesh, pt00, pt11, pt10);
                    AddTriangle(mesh, pt00, pt01, pt11);

                    // Move to the next value of theta.
                    theta0 = theta1;
                    pt00 = pt01;
                    pt10 = pt11;
                }

                // Move to the next value of phi.
                phi0 = phi1;
                y0 = y1;
                r0 = r1;
            }
        }

        private void LineByTwoPoints(MeshGeometry3D mesh, Point3D start, Point3D end)
        {
            Vector3D up = new Vector3D(0, 1, 0);
            AddSegment(mesh, new Point3D(start.X, start.Y, start.Z), new Point3D(end.X, end.Y, end.Z), up, true);
        }

        /// <summary>
        /// Generates a model of a Circle given specified parameters
        /// </summary>
        /// <param name="radius">Radius of circle</param>
        /// <param name="normal">Vector normal to circle's plane</param>
        /// <param name="center">Center position of the circle</param>
        /// <param name="resolution">Number of slices to iterate the circumference of the circle</param>
        /// <returns>A GeometryModel3D representation of the circle</returns>
        private GeometryModel3D GetCircleModel(double radius, Vector3D normal, Point3D center, int resolution)
        {
            var geo = new MeshGeometry3D();

            // Generate the circle in the XZ-plane
            // Add the center first
            geo.Positions.Add(new Point3D(0, 0, 0));

            // Iterate from angle 0 to 2*PI
            double t = 2 * Math.PI / resolution;
            for (int i = 0; i < resolution; i++)
            {
                geo.Positions.Add(new Point3D(radius * Math.Cos(t * i), 0, -radius * Math.Sin(t * i)));
            }

            // Add points to MeshGeoemtry3D
            for (int i = 0; i < resolution; i++)
            {
                var a = 0;
                var b = i + 1;
                var c = (i < (resolution - 1)) ? i + 2 : 1;

                geo.TriangleIndices.Add(a);
                geo.TriangleIndices.Add(b);
                geo.TriangleIndices.Add(c);
            }

            var brush3 = Brushes.Purple;
            var material3 = new DiffuseMaterial(brush3);
            var mod = new GeometryModel3D(geo, material3);

            // Create transforms
            var trn = new Transform3DGroup();
            // Up Vector (normal for XZ-plane)
            var up = new Vector3D(0, 1, 0);
            // Set normal length to 1
            normal.Normalize();
            var axis = Vector3D.CrossProduct(up, normal); // Cross product is rotation axis
            var angle = Vector3D.AngleBetween(up, normal); // Angle to rotate
            trn.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(axis, angle)));
            trn.Children.Add(new TranslateTransform3D(new Vector3D(center.X, center.Y, center.Z)));

            mod.Transform = trn;
            
            return mod;
        }

        #endregion
    }
}
