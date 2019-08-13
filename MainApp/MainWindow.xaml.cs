using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using MainApp.ViewModel;
using ManipulationSystemLibrary;
using ManipulationSystemLibrary.MathModel;
using Microsoft.Win32;
using Newtonsoft.Json;
using OxyPlot;

namespace MainApp
{
    // TODO: Critically needed refactoring, using MVVM Pattern [bug#13]

    public partial class MainWindow : Window
    {
        /// <summary>
        /// 0 - camera rotation;
        /// 1 - trajectory creation;
        /// 2 - trajectory editing;
        /// 3 - up/down trajectory points mod;
        /// </summary>
        private byte mouseMod;
        private byte keyboardMod;
        private Point MousePos;
        private Point offset;
        private double coeff; // Задаёт отношение реальных физических величин манипулятора от пиксельной характеристики виртуальной 3D модели манипулятора: len(px) = coeff * len(cm)
        private double OffsetY = 0; //0.5; // Сдвиг вверх от сцены, для того чтобы модель в горизонтальном положении лежала на сцене, а не тонула в ней наполовину

        #region Trajectory

        private ModelVisual3D trajectoryPointCursor;

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

        #endregion

        private List<ModelVisual3D> manipModelVisual3D; // Count of this list should be (model.n + 1)
        private Storyboard storyboard;

        private List<Point3D> listTrajectoryPoints; // list for spliting trajectory

        public MainWindow()
        {
            InitializeComponent();
            DataContext =
                new ApplicationViewModel(new DefaultDialogService(), new JsonFileService());

            // TODO: Remove all to ApplicationViewModel :
            this.manipModelVisual3D = new List<ModelVisual3D>();
            this.listTrajectoryPoints = new List<Point3D>();
            storyboard = new Storyboard();
            offset = new Point(504, 403);
            coeff = 1; // 0.5;
            mouseMod = 0;
            keyboardMod = 0;
            this.trajectoryLenght = 0;

        }

        #region Trajectory
        
        

        /// <summary>
        /// Open existing trajectory
        /// </summary>

        #region Trajectory creation mod

        private void UpDownTrajectoryPoints_Click(object sender, RoutedEventArgs e)
        {
            mouseMod = 0;
            keyboardMod = 1;
            Viewport3D.Children.Remove(this.trajectoryPointCursor);
            this.indexTrajectoryPoint = 1;
        }

        private void FinishBuildingPath_Click(object sender, RoutedEventArgs e)
        {
            keyboardMod = 0;
            PathBuilderGrid_Grid.Visibility = Visibility.Hidden;
            foreach (var p in this.trajectoryPointsVisual3D)
                this.listTrajectoryPoints.Add(p.center);
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
                    if (this.indexTrajectoryPoint == this.trajectoryPointsVisual3D.Count - 1) return;
                    this.indexTrajectoryPoint++;
                    break;
                case Key.P: //Previous point
                    if (this.indexTrajectoryPoint == 1) return;
                    this.indexTrajectoryPoint--;
                    break;
            }
        }

        private void ChangePathPointY(bool isUp)
        {
            var delta = isUp ? 0.25 : -0.25;
            this.trajectoryPointsVisual3D[this.indexTrajectoryPoint].SetY(this.trajectoryPointsVisual3D[this.indexTrajectoryPoint].center.Y + delta);

            (this.trajectoryPointsVisual3D[this.indexTrajectoryPoint].trajectoryModelVisual3D.Transform as TranslateTransform3D).OffsetY += delta;

            NeighborhoodLinesRotation();
        }

        private void NeighborhoodLinesRotation()
        {
            //Temporary solution
            //Insert new ModelVisual3D model of path line except old

            TrajectoryLine trajectoryLine;
            trajectoryLine.start = this.trajectoryPointsVisual3D[this.indexTrajectoryPoint - 1].center;
            trajectoryLine.end = this.trajectoryPointsVisual3D[this.indexTrajectoryPoint].center;

            //TODO: Extract to method next 8 line
            var line = new MeshGeometry3D();
            LineByTwoPoints(line, trajectoryLine.start, trajectoryLine.end, 0.13);
            var lineBrush = Brushes.MediumPurple;
            var lineMaterial = new DiffuseMaterial(lineBrush);
            var lineGeometryModel = new GeometryModel3D(line, lineMaterial);
            var pathLineModelVisual3D = new ModelVisual3D();
            pathLineModelVisual3D.Content = lineGeometryModel;
            Viewport3D.Children.Remove(this.trajectoryLinesVisual3D[this.indexTrajectoryPoint - 1].lineModelVisual3D);
            Viewport3D.Children.Insert(this.indexTrajectoryPoint - 1, pathLineModelVisual3D);

            trajectoryLine.lineModelVisual3D = pathLineModelVisual3D;
            this.trajectoryLinesVisual3D.RemoveAt(this.indexTrajectoryPoint - 1);
            this.trajectoryLinesVisual3D.Insert(this.indexTrajectoryPoint - 1, trajectoryLine);

            if (this.indexTrajectoryPoint == this.trajectoryPointsVisual3D.Count - 1) return;

            trajectoryLine.start = this.trajectoryPointsVisual3D[this.indexTrajectoryPoint].center;
            trajectoryLine.end = this.trajectoryPointsVisual3D[this.indexTrajectoryPoint + 1].center;

            //TODO: Extract to method next 8 line
            line = new MeshGeometry3D();
            LineByTwoPoints(line, trajectoryLine.start, trajectoryLine.end, 0.13);
            lineBrush = Brushes.MediumPurple;
            lineMaterial = new DiffuseMaterial(lineBrush);
            lineGeometryModel = new GeometryModel3D(line, lineMaterial);
            pathLineModelVisual3D = new ModelVisual3D();
            pathLineModelVisual3D.Content = lineGeometryModel;
            Viewport3D.Children.Remove(this.trajectoryLinesVisual3D[this.indexTrajectoryPoint].lineModelVisual3D);
            Viewport3D.Children.Insert(this.indexTrajectoryPoint, pathLineModelVisual3D);

            trajectoryLine.lineModelVisual3D = pathLineModelVisual3D;
            this.trajectoryLinesVisual3D.RemoveAt(this.indexTrajectoryPoint);
            this.trajectoryLinesVisual3D.Insert(this.indexTrajectoryPoint, trajectoryLine);

            //there is the bug :( bug#2
            //Because lenght changed when we up or down path point
            //And there is need to use ScaleTransform3D with RotateTransform3D
            //var rightLineLenght = (pathLinesVisual3D[indexPathPoint - 1].end - pathLinesVisual3D[indexPathPoint - 1].start).Length;
            //var h = pathLinesVisual3D[indexPathPoint].end.Y - pathLinesVisual3D[indexPathPoint].start.Y;
            //var rightLineAngle = Math.Asin(h / rightLineLenght);
            //(((pathLinesVisual3D[indexPathPoint - 1]
            //    .lineModelVisual3D
            //    .Transform as Transform3DGroup)
            //    .Children[0] as RotateTransform3D)
            //    .Rotation as AxisAngleRotation3D)
            //    .Angle = rightLineAngle;

            //var leftLineLenght = (pathLinesVisual3D[indexPathPoint].end - pathLinesVisual3D[indexPathPoint].start).Length;
            //h = pathLinesVisual3D[indexPathPoint].end.Y - pathLinesVisual3D[indexPathPoint].start.Y;
            //var leftLineAngle = Math.Asin(h / leftLineLenght);
            //(((pathLinesVisual3D[indexPathPoint]
            //    .lineModelVisual3D
            //    .Transform as Transform3DGroup)
            //    .Children[1] as RotateTransform3D)
            //    .Rotation as AxisAngleRotation3D)
            //    .Angle = leftLineAngle;
        }

        #endregion

        #region Splitting trajectory

        private void SplitPathByPointsQty_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            SplitingByStepOfPoints_Grid.Visibility = Visibility.Hidden;
            SplitingByNumberOfPoints_Grid.Visibility = Visibility.Visible;
        }

        private void SplitPathWithStep_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            SplitingByNumberOfPoints_Grid.Visibility = Visibility.Hidden;
            SplitingByStepOfPoints_Grid.Visibility = Visibility.Visible;
        }

        private void SplitPathByStep_Button_Click(object sender, RoutedEventArgs e)
        {
            double step;
            if (!double.TryParse(StepInCmToSplit_TextBox.Text, out step))
            {
                MessageBox.Show("Invalid input of split step!");
                return;
            }

            SplitPath(this.listTrajectoryPoints, step);
        }

        private void SplitPath(List<Point3D> listPathPoints, double step)
        {
            var index = 0;
            this.listSplitTrajectoryPoints = new List<Point3D>();
            for (var i = 1; i < listPathPoints.Count; i++)
            {
                var j = 0;
                double lambda = 0;
                var x = listPathPoints[i - 1].X;
                var y = listPathPoints[i - 1].Y;
                var z = listPathPoints[i - 1].Z;
                var dist = (listPathPoints[i - 1] - listPathPoints[i]).Length;
                do
                {
                    lambda = (step * j) / (dist - step * j);
                    x = (listPathPoints[i - 1].X + lambda * listPathPoints[i].X) / (1 + lambda);
                    y = (listPathPoints[i - 1].Y + lambda * listPathPoints[i].Y) / (1 + lambda);
                    z = (listPathPoints[i - 1].Z + lambda * listPathPoints[i].Z) / (1 + lambda);
                    this.listSplitTrajectoryPoints.Add(new Point3D(x, y, z));
                    index++;
                    j++;
                }
                while ((listPathPoints[i - 1] - new Point3D(x, y, z)).Length + step < dist);
            }
            index++;
            this.listSplitTrajectoryPoints.Add(listPathPoints[listPathPoints.Count - 1]);

            ShowSplitPath(this.listSplitTrajectoryPoints);
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

        #endregion

        #endregion

        #region Path Planning (when arm and trajectory exists

        private void PathPlanningButton_Click(object sender, RoutedEventArgs e)
        {
            if (model == null)
            {
                MessageBox.Show("Firstly create manipulator model!");
                return;
            }

            //Array.Clear(arrayQ, 0, arrayQ.Length);
            var delta = new Delta();
            var deltaViewModel = new DeltaPlotModel();
            for (var i = 1; i < this.listSplitTrajectoryPoints.Count; i++)
            {
                var pathPoint = this.listSplitTrajectoryPoints[i];
                model.Move(pathPoint);
                //var tmpQ = new double[model.n];
                //model.q.CopyTo(tmpQ, 0);
                //arrayQ[i] = tmpQ;
                //Thread.Sleep(1500);

                //ManipulatorTransformUpdate(model.q);//TODO:think what do with q
                var p = model.F(model.N);

                delta.DesiredPoints.Add(this.listSplitTrajectoryPoints[i]);
                delta.RealPoints.Add((Point3D)p);
            }

            delta.CalcDeltas();
            var serializer = new JsonSerializer();
            var stringWriter = new StringWriter();
            using (var writer = new JsonTextWriter(stringWriter))
            {
                writer.QuoteName = false;
                serializer.Serialize(writer, delta);
            }
            var json = stringWriter.ToString();
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\ManipulationSystemLibraryTests\\Deltas\\deltas.txt");
            File.WriteAllText(filePath, json);

            for (var i = 0; i < delta.Deltas.Count; i++)
                deltaViewModel.Points.Add(new DataPoint(delta.Deltas[i], i));
            var plotWindow = new PlotWindow
            {
                Owner = this,
                deltaPlotViewModel = deltaViewModel
            };
            plotWindow.Show();
        }
        
        #endregion

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
                    if (this.trajectoryPointsVisual3D == null)
                    {
                        this.trajectoryPointsVisual3D = new List<TrajectoryPoint>();
                        this.trajectoryLinesVisual3D = new List<TrajectoryLine>();
                        var firstPathPoint3D = new ModelVisual3D();
                        var firstPpathPoint = new MeshGeometry3D();
                        var p = model.F(model.N);
                        var firstPoint = new TrajectoryPoint();
                        firstPoint.center = new Point3D(p.X, p.Y + OffsetY, p.Z);

                        AddSphere(firstPpathPoint, firstPoint.center, 0.2, 8, 8);
                        var firstPointBrush = Brushes.GreenYellow;
                        var firstPointMaterial = new DiffuseMaterial(firstPointBrush);
                        var firstPathPointGeometryModel = new GeometryModel3D(firstPpathPoint, firstPointMaterial);
                        firstPathPoint3D.Content = firstPathPointGeometryModel;
                        firstPathPoint3D.Transform = new TranslateTransform3D();
                        Viewport3D.Children.Add(firstPathPoint3D);

                        firstPoint.trajectoryModelVisual3D = firstPathPoint3D;
                        this.trajectoryPointsVisual3D.Add(firstPoint);

                        PathBuilderGrid_Grid.Visibility = Visibility.Visible;
                    }

                    var hitParams = new PointHitTestParameters(e.GetPosition(this));
                    var X = (hitParams.HitPoint.X - offset.X) * 0.0531177;
                    var Z = (hitParams.HitPoint.Y - offset.Y) * 0.0531177;

                    var pathPoint = new TrajectoryPoint();
                    pathPoint.center = new Point3D(X, this.trajectoryPointsVisual3D.First().center.Y, Z);

                    //TODO: Extract to method next 8 line
                    var point = new MeshGeometry3D();
                    AddSphere(point, pathPoint.center, 0.2, 8, 8);
                    var pointBrush = Brushes.Purple;
                    var pointMaterial = new DiffuseMaterial(pointBrush);
                    var pathPointGeometryModel = new GeometryModel3D(point, pointMaterial);
                    var pathPointModelVisual3D = new ModelVisual3D();
                    pathPointModelVisual3D.Content = pathPointGeometryModel;
                    pathPointModelVisual3D.Transform = new TranslateTransform3D();
                    Viewport3D.Children.Add(pathPointModelVisual3D);

                    pathPoint.trajectoryModelVisual3D = pathPointModelVisual3D;
                    this.trajectoryPointsVisual3D.Add(pathPoint);

                    AddPathLine(this.trajectoryPointsVisual3D[this.trajectoryPointsVisual3D.Count - 2].center, this.trajectoryPointsVisual3D.Last().center);

                    this.trajectoryLenght += (this.trajectoryPointsVisual3D[this.trajectoryPointsVisual3D.Count - 2].center - this.trajectoryPointsVisual3D.Last().center).Length;
                    PathLenght.Content = $"Path lenght = {this.trajectoryLenght.ToString("#.000")} cm";
                    break;
                case 2:
                    //TODO: Editing path mode
                    break;
            }
        }

        private void AddPathLine(Point3D start, Point3D end)
        {
            TrajectoryLine trajectoryLine;
            trajectoryLine.start = start;
            trajectoryLine.end = end;

            //TODO: Extract to method next 8 line
            var line = new MeshGeometry3D();
            LineByTwoPoints(line, trajectoryLine.start, trajectoryLine.end, 0.13);
            var lineBrush = Brushes.MediumPurple;
            var lineMaterial = new DiffuseMaterial(lineBrush);
            var lineGeometryModel = new GeometryModel3D(line, lineMaterial);
            var pathLineModelVisual3D = new ModelVisual3D();
            pathLineModelVisual3D.Content = lineGeometryModel;
            Viewport3D.Children.Add(pathLineModelVisual3D);

            trajectoryLine.lineModelVisual3D = pathLineModelVisual3D;
            this.trajectoryLinesVisual3D.Add(trajectoryLine);
            //AddRotateTransform(pathLine); uncomment when bug#2 closed
        }

        private void AddRotateTransform(TrajectoryLine trajectoryLine)
        {
            var transformGroup = new Transform3DGroup();

            var rotateTransformByStart = new RotateTransform3D();
            rotateTransformByStart.CenterX = trajectoryLine.start.X;
            rotateTransformByStart.CenterY = trajectoryLine.start.Y;
            rotateTransformByStart.CenterZ = trajectoryLine.start.Z;

            var rotateTransformByEnd = new RotateTransform3D();
            rotateTransformByEnd.CenterX = trajectoryLine.end.X;
            rotateTransformByEnd.CenterY = trajectoryLine.end.Y;
            rotateTransformByEnd.CenterZ = trajectoryLine.end.Z;

            var angleRotationByStart = new AxisAngleRotation3D
            {
                Axis = Vector3D.CrossProduct(
                    new Vector3D(trajectoryLine.end.X - trajectoryLine.start.X,
                                 trajectoryLine.end.Y - trajectoryLine.start.Y,
                                 trajectoryLine.end.Z - trajectoryLine.start.Z),
                    new Vector3D(0, 1, 0)),
                Angle = 0
            };

            var angleRotationByEnd = new AxisAngleRotation3D
            {
                Axis = Vector3D.CrossProduct(
                     new Vector3D(0, 1, 0),
                     new Vector3D(trajectoryLine.end.X - trajectoryLine.start.X,
                                  trajectoryLine.end.Y - trajectoryLine.start.Y,
                                  trajectoryLine.end.Z - trajectoryLine.start.Z)),
                Angle = 0
            };

            rotateTransformByStart.Rotation = angleRotationByStart;
            transformGroup.Children.Add(rotateTransformByStart);

            rotateTransformByEnd.Rotation = angleRotationByEnd;
            transformGroup.Children.Add(rotateTransformByEnd);

            trajectoryLine.lineModelVisual3D.Transform = transformGroup;
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
                    if (this.trajectoryPointCursor != null)
                        Viewport3D.Children.Remove(this.trajectoryPointCursor);
                    var myModel3DGroup = new Model3DGroup();
                    this.trajectoryPointCursor = new ModelVisual3D();

                    //TODO: fix moving path cursor (when resize window this shit doesn't work), remove this fucking coeffs (0.0531177)
                    var hitParams = new PointHitTestParameters(e.GetPosition(this));
                    var myGeometryModel = GetCircleModel(0.5, new Vector3D(0, 1, 0),
                        new Point3D((hitParams.HitPoint.X - offset.X) * 0.0531177, 0, (hitParams.HitPoint.Y - offset.Y) * 0.0531177), 14);
                    myModel3DGroup.Children.Add(myGeometryModel);
                    this.trajectoryPointCursor.Content = myModel3DGroup;
                    Viewport3D.Children.Add(this.trajectoryPointCursor);
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
            if (ScaleTransform3D.ScaleX < 1)
            {
                ScaleTransform3D.ScaleX += (double)e.Delta / 555;
                ScaleTransform3D.ScaleY += (double)e.Delta / 555;
                ScaleTransform3D.ScaleZ += (double)e.Delta / 555;
            }
            else
            {
                ScaleTransform3D.ScaleX += (double)e.Delta / 333;
                ScaleTransform3D.ScaleY += (double)e.Delta / 333;
                ScaleTransform3D.ScaleZ += (double)e.Delta / 333;
            }
        }

        #endregion

        #region Graphics


        private void ManipulatorMoveAnimation()
        {
            var timelineCollection = new TimelineCollection();
            for (var i = 1; i < model.N; i++)
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
            var transformGroup = new Transform3DGroup[model.N];
            for (var i = 0; i < model.N; i++)
            {
                Transform3D transformation = null;

                var center = model.F(i);
                switch (model.Units[i].Type)
                {
                    #region case R
                    case 'R':
                        transformation = new RotateTransform3D();
                        (transformation as RotateTransform3D).CenterX = center.X;
                        (transformation as RotateTransform3D).CenterY = center.Y;
                        (transformation as RotateTransform3D).CenterZ = center.Z;

                        var angleRotation = new AxisAngleRotation3D
                        {
                            Axis = model.GetZAxis(i),
                            Angle = RadianToDegree(model.Units[i].Q)
                        };

                        (transformation as RotateTransform3D).Rotation = angleRotation;

                        transformGroup[i] = new Transform3DGroup();
                        for (var j = i; j < model.N; j++)
                            transformGroup[j].Children.Add(transformation);

                        break;
                    #endregion
                    case 'P':
                        transformation = new TranslateTransform3D();

                        transformGroup[i] = new Transform3DGroup();
                        for (var j = i + 1; j < model.N; j++)
                            transformGroup[j].Children.Add(transformation);

                        break;
                }
            }
            // Трансформация звеньев RotateTransform3D для 'R' должна быть применена ко всем последующим звеньям,
            // а для звена 'P' только ScaleTransform3D(только для линии) и для всех последующих TranslateTransform3D
            for (var i = 1; i < model.N + 1; i++)
                manipModelVisual3D[i].Transform = transformGroup[i - 1];
        }

        private void ManipulatorTransformUpdate(double[] q)
        {
            
            for (var i = 1; i < model.N + 1; i++)
            {

                switch (model.Units[i - 1].Type)
                {
                    case 'R':
                        for (var j = i + 1; j < model.N; j++)
                        {
                            (((manipModelVisual3D[j]
                                .Transform as Transform3DGroup)
                                .Children[i] as RotateTransform3D)
                                .Rotation as AxisAngleRotation3D)
                                .Angle = q[i];
                        }
                        break;
                    case 'P':
                        #region Remove old and insert new P unit model visual 3d
                        var prismaticAxis = new Vector3D();
                        var unit = new MeshGeometry3D();
                        var joint = new MeshGeometry3D();

                        var sup = model.F(i - 1); // startUnitPoint
                        var eup = model.F(i); // endUnitPoint
                        LineByTwoPoints(unit, new Point3D(sup.X * coeff, sup.Y * coeff + OffsetY, sup.Z * coeff),
                                              new Point3D(eup.X * coeff, eup.Y * coeff + OffsetY, eup.Z * coeff), 0.25);
                        AddSphere(joint, new Point3D(eup.X * coeff, eup.Y * coeff + OffsetY, eup.Z * coeff), 0.4, 8, 8);

                        var unitBrush = Brushes.CornflowerBlue;
                        var unitMaterial = new DiffuseMaterial(unitBrush);
                        var myGeometryModel = new GeometryModel3D(unit, unitMaterial);
                        var jointsAndUnitsModelGroup = new Model3DGroup();
                        jointsAndUnitsModelGroup.Children.Add(myGeometryModel);

                        var jointBrush = Brushes.OrangeRed;
                        var jointMaterial = new DiffuseMaterial(jointBrush);
                        myGeometryModel = new GeometryModel3D(joint, jointMaterial);
                        jointsAndUnitsModelGroup.Children.Add(myGeometryModel);
                        var arm = new ModelVisual3D();
                        arm.Content = jointsAndUnitsModelGroup;

                        Viewport3D.Children.Remove(manipModelVisual3D[i + 1]);
                        Viewport3D.Children.Insert(i + 1, arm);

                        manipModelVisual3D.Remove(arm);
                        manipModelVisual3D.Insert(i + 1, arm);
                        #endregion

                        prismaticAxis = model.GetZAxis(i);
                        for (var j = i + 2; j < model.N; j++)
                        {
                            ((manipModelVisual3D[j]
                                .Transform as Transform3DGroup)
                                .Children[i] as TranslateTransform3D)
                                .OffsetX += prismaticAxis.X * q[i];
                            ((manipModelVisual3D[j]
                                .Transform as Transform3DGroup)
                                .Children[i] as TranslateTransform3D)
                                .OffsetY += prismaticAxis.Y * q[i];
                            ((manipModelVisual3D[j]
                                .Transform as Transform3DGroup)
                                .Children[i] as TranslateTransform3D)
                                .OffsetZ += prismaticAxis.Z * q[i];
                        }
                        break;
                }
            }
        }

        private double DegreeToRadian(double angle) => Math.PI * angle / 180.0;

        private double RadianToDegree(double angle) => 180.0 * angle / Math.PI;

        private void CreateManipulator3DVisualModel(Arm model)
        {
            if (manipModelVisual3D.Count > 0)
            {
                foreach (var arm in manipModelVisual3D)
                    Viewport3D.Children.Remove(arm);
            }
            var sup = new Vector3D(0, 0, 0); // startUnitPoint
            for (var i = 0; i < model.N + 1; i++)
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

                Viewport3D.Children.Add(arm);
                manipModelVisual3D.Add(arm);
            }
        }

        // Set the vector's length.
        private Vector3D ScaleVector(Vector3D vector, double length)
        {
            var scale = length / vector.Length;
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
            var index1 = mesh.Positions.Count;
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
            var v = point2 - point1;

            if (extend)
            {
                // Increase the segment's length on both ends by thickness / 2.
                var n = ScaleVector(v, thickness / 2.0);
                point1 -= n;
                point2 += n;
            }

            // Get the scaled up vector.
            var n1 = ScaleVector(up, thickness / 2.0);

            // Get another scaled perpendicular vector.
            var n2 = Vector3D.CrossProduct(v, n1);
            n2 = ScaleVector(n2, thickness / 2.0);

            // Make a skinny box.
            // p1pm means point1 PLUS n1 MINUS n2.
            var p1pp = point1 + n1 + n2;
            var p1mp = point1 - n1 + n2;
            var p1pm = point1 + n1 - n2;
            var p1mm = point1 - n1 - n2;
            var p2pp = point2 + n1 + n2;
            var p2mp = point2 - n1 + n2;
            var p2pm = point2 + n1 - n2;
            var p2mm = point2 - n1 - n2;

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
            var dphi = Math.PI / num_phi;
            var dtheta = 2 * Math.PI / num_theta;

            phi0 = 0;
            var y0 = radius * Math.Cos(phi0);
            var r0 = radius * Math.Sin(phi0);
            for (var i = 0; i < num_phi; i++)
            {
                var phi1 = phi0 + dphi;
                var y1 = radius * Math.Cos(phi1);
                var r1 = radius * Math.Sin(phi1);

                // Point ptAB has phi value A and theta value B.
                // For example, pt01 has phi = phi0 and theta = theta1.
                // Find the points with theta = theta0.
                theta0 = 0;
                var pt00 = new Point3D(
                    center.X + r0 * Math.Cos(theta0),
                    center.Y + y0,
                    center.Z + r0 * Math.Sin(theta0));
                var pt10 = new Point3D(
                    center.X + r1 * Math.Cos(theta0),
                    center.Y + y1,
                    center.Z + r1 * Math.Sin(theta0));
                for (var j = 0; j < num_theta; j++)
                {
                    // Find the points with theta = theta1.
                    var theta1 = theta0 + dtheta;
                    var pt01 = new Point3D(
                        center.X + r0 * Math.Cos(theta1),
                        center.Y + y0,
                        center.Z + r0 * Math.Sin(theta1));
                    var pt11 = new Point3D(
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
            var up = new Vector3D(0, 0, 1);
            AddSegment(mesh, new Point3D(start.X, start.Y, start.Z), new Point3D(end.X, end.Y, end.Z), up, thickness);
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
            var t = 2 * Math.PI / resolution;
            for (var i = 0; i < resolution; i++)
            {
                geo.Positions.Add(new Point3D(radius * Math.Cos(t * i), 0, -radius * Math.Sin(t * i)));
            }

            // Add points to MeshGeoemtry3D
            for (var i = 0; i < resolution; i++)
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
