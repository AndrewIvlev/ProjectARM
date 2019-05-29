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
        /// 3 - up/down path points mod;
        /// </summary>
        private byte mouseMod;
        private byte keyboardMod;
        private Point MousePos;
        private Point offset;
        private double coeff; // Задаёт отношение реальных физических величин манипулятора от пиксельной характеристики виртуальной 3D модели манипулятора: len(px) = coeff * len(cm)
        private double OffsetY = 0; //0.5; // Сдвиг вверх от сцены, для того чтобы модель в горизонтальном положении лежала на сцене, а не тонула в ней наполовину

        #region PATH
        private ModelVisual3D pathPointCursor;

        private struct PathPoint
        {
            public ModelVisual3D pointModelVisual3D;
            public Point3D center;

            public void SetY(double y)
            {
                center.Y = y;
            }
        }
        private List<PathPoint> pathPointsVisual3D;
        private int indexPathPoint; //TODO: remove it, do smarter

        struct PathLine
        {
            public ModelVisual3D lineModelVisual3D;
            public Point3D start;
            public Point3D end;
        }
        private List<PathLine> pathLinesVisual3D;
        #endregion

        private List<ModelVisual3D> manipModelVisual3D;
        private Storyboard storyboard;

        private MatrixMathModel model;
        private double[][] arrayQ; // Array of the generalized coordinates vectors for moving animation player
        private List<Point3D> listPathPoints;

        public MainWindow()
        {
            InitializeComponent();
            manipModelVisual3D = new List<ModelVisual3D>();
            storyboard = new Storyboard();
            arrayQ = new double[256][]; //TODO: move to method where we already know how many points in the path and change 256 to it
            offset = new Point(504, 403);
            coeff = 1; // 0.5;
            mouseMod = 0;
            keyboardMod = 0;
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
            var tmpQ = new double[model.n - 1];
            model.q.CopyTo(tmpQ, 0);
            arrayQ[0] = tmpQ;
            model.q[0] = DegreeToRadian(0); // R
            model.q[1] = DegreeToRadian(90); // R
            model.q[2] = DegreeToRadian(30); // R
            model.q[3] = DegreeToRadian(90); // R
            arrayQ[1] = model.q;
            model.CalculationMetaData();
            AddTransformationsForManipulator();
        }

        #endregion

        #region Path

        private void NewPath_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (model == null)
            {
                MessageBox.Show("Firstly create manipulator model!");
                return;
            }

            RotX.Angle = -71;
            RotY.Angle = -45;
            mouseMod = 1;
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

            listPathPoints = new List<Point3D>
            {
                new Point3D(8.2, 3.3, 4),
                new Point3D(7.7, 4, 4.5),
                new Point3D(7.3, 4.5, 4.65),
                new Point3D(6, 5, 4.8),
                new Point3D(5, 5.5, 5),
                new Point3D(5, 6, 5.5),
                new Point3D(5, 6.5, 6),
                new Point3D(4, 7, 6)
            };

            //Array.Clear(arrayQ, 0, arrayQ.Length);
            for (int i = 0; i < listPathPoints.Count; i++)
            {
                model.CalculationMetaData();
                Point3D pathPoint = listPathPoints[i];
                model.LagrangeMethodToThePoint(pathPoint);
                //var tmpQ = new double[model.n - 1];
                //model.q.CopyTo(tmpQ, 0);
                //arrayQ[i] = tmpQ;
                //Thread.Sleep(1500);
                ManipulatorTransformUpdate(model.q);
                var shvat = model.F(model.n - 1);
            }
        }

        private void UpDownPathPoints_Click(object sender, RoutedEventArgs e)
        {
            mouseMod = 0;
            keyboardMod = 1;
            this.Viewport3D.Children.Remove(pathPointCursor);
            indexPathPoint = 1;
        }

        private void FinishBuildingPath_Click(object sender, RoutedEventArgs e)
        {
            keyboardMod = 0;
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (keyboardMod != 1) return;
            switch (e.Key)
            {
                /*** Raise/lower point along Y axis ***/
                case Key.W:
                    ChangePathPointY(true);
                    break;
                case Key.S:
                    ChangePathPointY(false);
                    break;

                /*** Point selection ***/
                //Каким-то образом нужно подсвечивать сферу точки, которая выбирается
                //Можно например применить к сфере ScaleTransform3D увеличив временно её в размерах
                case Key.N: //Next point
                    if (indexPathPoint >= pathPointsVisual3D.Count) return;
                    indexPathPoint++;
                    break;
                case Key.P: //Previous point
                    if (indexPathPoint == 1) return;
                    indexPathPoint--;
                    break;
            }
        }

        private void ChangePathPointY(bool isUp)
        {
            var delta = isUp ? 0.25 : -0.25;
            pathPointsVisual3D[indexPathPoint].SetY(pathPointsVisual3D[indexPathPoint].center.Y + delta);

            (pathPointsVisual3D[indexPathPoint].pointModelVisual3D.Transform as TranslateTransform3D).OffsetY += delta;

            NeighborhoodLinesRotation();
        }

        private void NeighborhoodLinesRotation()
        {
            var rightLineLenght = (pathLinesVisual3D[indexPathPoint - 1].end - pathLinesVisual3D[indexPathPoint - 1].start).Length;
            var h = pathLinesVisual3D[indexPathPoint].end.Y - pathLinesVisual3D[indexPathPoint].start.Y;
            var rightLineAngle = Math.Asin(h / rightLineLenght);
            (((pathLinesVisual3D[indexPathPoint - 1]
                .lineModelVisual3D
                .Transform as Transform3DGroup)
                .Children[0] as RotateTransform3D)
                .Rotation as AxisAngleRotation3D)
                .Angle = rightLineAngle;

            var leftLineLenght = (pathLinesVisual3D[indexPathPoint].end - pathLinesVisual3D[indexPathPoint].start).Length;
            h = pathLinesVisual3D[indexPathPoint].end.Y - pathLinesVisual3D[indexPathPoint].start.Y;
            var leftLineAngle = Math.Asin(h / leftLineLenght);
            (((pathLinesVisual3D[indexPathPoint]
                .lineModelVisual3D
                .Transform as Transform3DGroup)
                .Children[1] as RotateTransform3D)
                .Rotation as AxisAngleRotation3D)
                .Angle = leftLineAngle;
        }

        #region Canvas Events

        /// <summary>
        /// По клику ЛКМ по сцене мы либо перемещаем камеру,
        /// либо создаём траекторию пути, либо редактируем траекторию пути.
        /// </summary>
        private void Canvas_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            switch (mouseMod)
            {
                case 0:
                    MousePos = e.GetPosition(this);
                    break;
                case 1:
                    if (model == null)
                    {
                        MessageBox.Show("Firstly create manipulator model!");
                        return;
                    }
                    if (pathPointsVisual3D == null)
                    {
                        pathPointsVisual3D = new List<PathPoint>();
                        pathLinesVisual3D = new List<PathLine>();
                        var firstPathPoint3D = new ModelVisual3D();
                        var firstPpathPoint = new MeshGeometry3D();
                        var p = model.F(model.n - 1);
                        PathPoint firstPoint;
                        firstPoint.center = new Point3D(p.X, p.Y + this.OffsetY, p.Z);

                        AddSphere(firstPpathPoint, firstPoint.center, 0.2, 8, 8);
                        var firstPointBrush = Brushes.GreenYellow;
                        var firstPointMaterial = new DiffuseMaterial(firstPointBrush);
                        var firstPathPointGeometryModel = new GeometryModel3D(firstPpathPoint, firstPointMaterial);
                        firstPathPoint3D.Content = firstPathPointGeometryModel;
                        firstPathPoint3D.Transform = new TranslateTransform3D();
                        this.Viewport3D.Children.Add(firstPathPoint3D);

                        firstPoint.pointModelVisual3D = firstPathPoint3D;
                        pathPointsVisual3D.Add(firstPoint);
                    }

                    PointHitTestParameters hitParams = new PointHitTestParameters(e.GetPosition(this));
                    var X = (hitParams.HitPoint.X - offset.X) * 0.0531177;
                    var Z = (hitParams.HitPoint.Y - offset.Y) * 0.0531177;

                    PathPoint pathPoint;
                    pathPoint.center = new Point3D(X, pathPointsVisual3D.First().center.Y, Z);

                    //TODO: Extract to method next 8 line
                    var point = new MeshGeometry3D();
                    AddSphere(point, pathPoint.center, 0.2, 8, 8);
                    var pointBrush = Brushes.Purple;
                    var pointMaterial = new DiffuseMaterial(pointBrush);
                    var pathPointGeometryModel = new GeometryModel3D(point, pointMaterial);
                    var pathPointModelVisual3D = new ModelVisual3D();
                    pathPointModelVisual3D.Content = pathPointGeometryModel;
                    pathPointModelVisual3D.Transform = new TranslateTransform3D();
                    this.Viewport3D.Children.Add(pathPointModelVisual3D);

                    pathPoint.pointModelVisual3D = pathPointModelVisual3D;
                    pathPointsVisual3D.Add(pathPoint);


                    PathLine pathLine;
                    pathLine.start = pathPointsVisual3D[pathPointsVisual3D.Count - 2].center;
                    pathLine.end = pathPointsVisual3D.Last().center;

                    //TODO: Extract to method next 8 line
                    var line = new MeshGeometry3D();
                    LineByTwoPoints(line, pathLine.start, pathLine.end, 0.13);
                    var lineBrush = Brushes.MediumPurple;
                    var lineMaterial = new DiffuseMaterial(lineBrush);
                    var lineGeometryModel = new GeometryModel3D(line, lineMaterial);
                    var pathLineModelVisual3D = new ModelVisual3D();
                    pathLineModelVisual3D.Content = lineGeometryModel;
                    this.Viewport3D.Children.Add(pathLineModelVisual3D);

                    pathLine.lineModelVisual3D = pathLineModelVisual3D;
                    pathLinesVisual3D.Add(pathLine);
                    AddRotateTransform(pathLine);
                    break;
                case 2:
                    //TODO: Editing path mode
                    break;
                default:
                    break;
            }
        }

        private void AddRotateTransform(PathLine pathLine)
        {
            var transformGroup = new Transform3DGroup();

            var rotateTransformByStart = new RotateTransform3D();
            rotateTransformByStart.CenterX = pathLine.start.X;
            rotateTransformByStart.CenterY = pathLine.start.Y;
            rotateTransformByStart.CenterZ = pathLine.start.Z;

            var rotateTransformByEnd = new RotateTransform3D();
            rotateTransformByEnd.CenterX = pathLine.end.X;
            rotateTransformByEnd.CenterY = pathLine.end.Y;
            rotateTransformByEnd.CenterZ = pathLine.end.Z;

            var angleRotationByStart = new AxisAngleRotation3D
            {
                Axis = Vector3D.CrossProduct(
                    new Vector3D(pathLine.end.X - pathLine.start.X,
                                 pathLine.end.Y - pathLine.start.Y,
                                 pathLine.end.Z - pathLine.start.Z),
                    new Vector3D(0, 1, 0)),
                Angle = 0
            };

            var angleRotationByEnd = new AxisAngleRotation3D
            {
                Axis = Vector3D.CrossProduct(
                     new Vector3D(0, 1, 0),
                     new Vector3D(pathLine.end.X - pathLine.start.X,
                                  pathLine.end.Y - pathLine.start.Y,
                                  pathLine.end.Z - pathLine.start.Z)),
                Angle = 0
            };

            rotateTransformByStart.Rotation = angleRotationByStart;
            transformGroup.Children.Add(rotateTransformByStart);

            rotateTransformByEnd.Rotation = angleRotationByEnd;
            transformGroup.Children.Add(rotateTransformByEnd);

            pathLine.lineModelVisual3D.Transform = transformGroup;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            switch (mouseMod)
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
                    if (pathPointCursor != null)
                        this.Viewport3D.Children.Remove(pathPointCursor);
                    var myModel3DGroup = new Model3DGroup();
                    pathPointCursor = new ModelVisual3D();

                    PointHitTestParameters hitParams = new PointHitTestParameters(e.GetPosition(this));
                    var myGeometryModel = GetCircleModel(0.5, new Vector3D(0, 1, 0),
                        new Point3D((hitParams.HitPoint.X - offset.X) * 0.0531177, 0, (hitParams.HitPoint.Y - offset.Y) * 0.0531177), 14);
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
            if (this.ScaleTransform3D.ScaleX < 1)
            {
                this.ScaleTransform3D.ScaleX += (double)e.Delta / 555;
                this.ScaleTransform3D.ScaleY += (double)e.Delta / 555;
                this.ScaleTransform3D.ScaleZ += (double)e.Delta / 555;
            }
            else
            {
                this.ScaleTransform3D.ScaleX += (double)e.Delta / 333;
                this.ScaleTransform3D.ScaleY += (double)e.Delta / 333;
                this.ScaleTransform3D.ScaleZ += (double)e.Delta / 333;
            }
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
        /// Add transformations for each unit of manipulatorModelVisual3D
        /// </summary>
        private void AddTransformationsForManipulator()
        {
            var transformGroup = new Transform3DGroup[model.n - 1];
            for (int i = 0; i < model.n - 1; i++)
                transformGroup[i] = new Transform3DGroup();

            for (int i = 0; i < model.n - 1; i++)
            {
                var transformation = GetTransformationByUnitType(i);

                for (int j = i; j < model.n - 1; j++)
                    transformGroup[j].Children.Add(transformation as Transform3D);
            }

            // Трансформация звеньев RotateTransform3D для 'R' должна быть применена ко всем последующим звеньям,
            // а для звена 'P' только ScaleTransform3D(только для линии) и для всех последующих TranslateTransform3D
            for (int i = 1; i < model.n; i++)
                manipModelVisual3D[i].Transform = transformGroup[i - 1];
        }

        private void ManipulatorTransformUpdate(double[] q)
        {
            for (int j = 0; j < model.n - 1; j++)
            {
                for (int i = j + 1; i < model.n; i++)
                {
                    switch (model.units[i].type)
                    {
                        case 'R':
                            (((manipModelVisual3D[i]
                                .Transform as Transform3DGroup)
                                .Children[j] as RotateTransform3D)
                                .Rotation as AxisAngleRotation3D)
                                .Angle = q[j];
                            break;
                        case 'P':
                            break;
                    }
                }
            }
        }

        private Transform3D GetTransformationByUnitType(int unitIndex)
        {
            Transform3D transformation = null;

            var center = model.F(unitIndex);
            switch (model.units[unitIndex + 1].type)
            { 
                case 'R':
                    transformation = new RotateTransform3D();
                    (transformation as RotateTransform3D).CenterX = center.X;
                    (transformation as RotateTransform3D).CenterY = center.Y;
                    (transformation as RotateTransform3D).CenterZ = center.Z;

                    var angleRotation = new AxisAngleRotation3D
                    {
                        Axis = model.GetZAxis(unitIndex),
                        Angle = RadianToDegree(model.q[unitIndex])
                    };
                    (transformation as RotateTransform3D).Rotation = angleRotation;
                    break;
                case 'P':
                    transformation = new ScaleTransform3D();
                    (transformation as ScaleTransform3D).CenterX = center.X;
                    (transformation as ScaleTransform3D).CenterY = center.Y;
                    (transformation as ScaleTransform3D).CenterZ = center.Z;
                    var prismaticAxis = model.GetZAxis(unitIndex);
                    (transformation as ScaleTransform3D).ScaleX = 1.2; // prismaticAxis.X + arrayQ[0][unitIndex] / model.q[unitIndex];
                    (transformation as ScaleTransform3D).ScaleY = 1.2; // prismaticAxis.Y + arrayQ[0][unitIndex] / model.q[unitIndex];
                    (transformation as ScaleTransform3D).ScaleZ = 1.2; // prismaticAxis.Z + arrayQ[0][unitIndex] / model.q[unitIndex];
                    break;
            }

            return transformation;
        }

        private double DegreeToRadian(double angle) => Math.PI * angle / 180.0;

        private double RadianToDegree(double angle) => 180.0 * angle / Math.PI;

        private void CreateManipulator3DVisualModel(MatrixMathModel model)
        {
            if (manipModelVisual3D.Count > 0)
            {
                foreach (var arm in manipModelVisual3D)
                    this.Viewport3D.Children.Remove(arm);
            }
            var sup = new Vector3D(0, 0, 0); // startUnitPoint
            for (int i = 0; i < model.n; i++)
            {
                var arm = new ModelVisual3D();
                var jointsAndUnitsModelGroup = new Model3DGroup();
                var unit = new MeshGeometry3D();
                var joint = new MeshGeometry3D();
                
                var eup = model.F(i); // endUnitPoint
                LineByTwoPoints(unit, new Point3D(sup.X * coeff, sup.Y * coeff + OffsetY, sup.Z * coeff),
                                      new Point3D(eup.X * coeff, eup.Y * coeff + OffsetY, eup.Z * coeff), 0.25);
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
                var storyBoard = new Storyboard();

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
            Point3D point1, Point3D point2, Vector3D up, double thickness)
        {
            AddSegment(mesh, point1, point2, up, thickness, false);
        }

        private void AddSegment(MeshGeometry3D mesh,
            Point3D point1, Point3D point2, Vector3D up, double thickness,
            bool extend)
        {
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

        private void LineByTwoPoints(MeshGeometry3D mesh, Point3D start, Point3D end, double thickness)
        {
            Vector3D up = new Vector3D(0, 1, 0);
            AddSegment(mesh, new Point3D(start.X, start.Y, start.Z), new Point3D(end.X, end.Y, end.Z), up, thickness, true);
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
