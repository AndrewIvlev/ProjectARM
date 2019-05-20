using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        private MatrixMathModel model;

        public MainWindow()
        {
            InitializeComponent();
            MouseMod = 0;
        }

        private void OpenManipulatorFile_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() != true) return;
            var jsonStringMatrixMathModel = File.ReadAllText(openFileDialog.FileName);
            var manipConfig = JsonConvert.DeserializeObject<MatrixMathModel>(jsonStringMatrixMathModel);
            model = new MatrixMathModel(manipConfig);
        }
        
        private void SplitPathByPointsQty_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SplitPathWithStep_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void PathPlanningButton_Click(object sender, RoutedEventArgs e)
        {
            Model3DGroup myModel3DGroup = new Model3DGroup();
            GeometryModel3D myGeometryModel = new GeometryModel3D();
            ModelVisual3D myModelVisual3D = new ModelVisual3D();
            MeshGeometry3D mesh = new MeshGeometry3D();
            model.LagrangeMethodToThePoint(new Vector3D(42, 1, 1));
            var Fq = model.F(model.n - 1);
            AddSmoothCylinder(mesh, (Point3D)model.F(1), model.F(1), 1, 4);
            myGeometryModel.Geometry = mesh;
            myModel3DGroup.Children.Add(myGeometryModel);
            myModelVisual3D.Content = myModel3DGroup;
            Viewport3D.Children.Add(myModelVisual3D);
            //foreach (var unit in model.units)
            //{
            //    var i = Array.IndexOf(model.units, unit);
            //    switch (unit.type)
            //    {
            //        case 'S':
            //            break;
            //        case 'R':
            //            AddSmoothCylinder(mesh, (Point3D)model.F(i), , 1, 4);
            //            break;
            //        case 'P':
            //            break;
            //    }
            //    Viewport3D.Children.Add(mesh);
            //}
        }

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
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {
        }

        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Camera zoom
        }
        
        // Add a cylinder with smooth sides.
        private void AddSmoothCylinder(MeshGeometry3D mesh,
            Point3D end_point, Vector3D axis, double radius, int num_sides)
        {
            // Get two vectors perpendicular to the axis.
            Vector3D v1;
            if ((axis.Z < -0.01) || (axis.Z > 0.01))
                v1 = new Vector3D(axis.Z, axis.Z, -axis.X - axis.Y);
            else
                v1 = new Vector3D(-axis.Y - axis.Z, axis.X, axis.X);
            Vector3D v2 = Vector3D.CrossProduct(v1, axis); //Векторное произведение

            // Make the vectors have length radius.
            v1 *= (radius / v1.Length);
            v2 *= (radius / v2.Length);

            // Make the top end cap.
            // Make the end point.
            int pt0 = mesh.Positions.Count; // Index of end_point.
            mesh.Positions.Add(end_point);

            // Make the top points.
            double theta = 0;
            double dtheta = 2 * Math.PI / num_sides;
            for (int i = 0; i < num_sides; i++)
            {
                mesh.Positions.Add(end_point +
                    Math.Cos(theta) * v1 +
                    Math.Sin(theta) * v2);
                theta += dtheta;
            }

            // Make the top triangles.
            int pt1 = mesh.Positions.Count - 1; // Index of last point.
            int pt2 = pt0 + 1;                  // Index of first point.
            for (int i = 0; i < num_sides; i++)
            {
                mesh.TriangleIndices.Add(pt0);
                mesh.TriangleIndices.Add(pt1);
                mesh.TriangleIndices.Add(pt2);
                pt1 = pt2++;
            }

            // Make the bottom end cap.
            // Make the end point.
            pt0 = mesh.Positions.Count; // Index of end_point2.
            Point3D end_point2 = end_point + axis;
            mesh.Positions.Add(end_point2);

            // Make the bottom points.
            theta = 0;
            for (int i = 0; i < num_sides; i++)
            {
                mesh.Positions.Add(end_point2 +
                    Math.Cos(theta) * v1 +
                    Math.Sin(theta) * v2);
                theta += dtheta;
            }

            // Make the bottom triangles.
            theta = 0;
            pt1 = mesh.Positions.Count - 1; // Index of last point.
            pt2 = pt0 + 1;                  // Index of first point.
            for (int i = 0; i < num_sides; i++)
            {
                mesh.TriangleIndices.Add(num_sides + 1);    // end_point2
                mesh.TriangleIndices.Add(pt2);
                mesh.TriangleIndices.Add(pt1);
                pt1 = pt2++;
            }

            // Make the sides.
            // Add the points to the mesh.
            int first_side_point = mesh.Positions.Count;
            theta = 0;
            for (int i = 0; i < num_sides; i++)
            {
                Point3D p1 = end_point +
                    Math.Cos(theta) * v1 +
                    Math.Sin(theta) * v2;
                mesh.Positions.Add(p1);
                Point3D p2 = p1 + axis;
                mesh.Positions.Add(p2);
                theta += dtheta;
            }

            // Make the side triangles.
            pt1 = mesh.Positions.Count - 2;
            pt2 = pt1 + 1;
            int pt3 = first_side_point;
            int pt4 = pt3 + 1;
            for (int i = 0; i < num_sides; i++)
            {
                mesh.TriangleIndices.Add(pt1);
                mesh.TriangleIndices.Add(pt2);
                mesh.TriangleIndices.Add(pt4);

                mesh.TriangleIndices.Add(pt1);
                mesh.TriangleIndices.Add(pt4);
                mesh.TriangleIndices.Add(pt3);

                pt1 = pt3;
                pt3 += 2;
                pt2 = pt4;
                pt4 += 2;
            }
        }
    }
}
