using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using MainApp.Common;
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

        private Storyboard storyboard;


        public MainWindow()
        {
            InitializeComponent();
            DataContext =
                new ApplicationViewModel(new DefaultDialogService(), new JsonFileService());

            // TODO: Remove all to ApplicationViewModel :
            storyboard = new Storyboard();
            offset = new Point(504, 403);
            coeff = 1; // 0.5;
            mouseMod = 0;
            keyboardMod = 0;

        }

        #region Trajectory creation mod

        #region Path
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
            if (arm == null)
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

                        ButtonsForCreatingNewTrajectory.Visibility = Visibility.Visible;
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
    }
}
