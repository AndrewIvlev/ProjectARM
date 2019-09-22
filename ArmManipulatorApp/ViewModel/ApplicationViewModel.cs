namespace ArmManipulatorApp.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Forms.DataVisualization.Charting;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Media3D;
    using System.Windows.Navigation;

    using ArmManipulatorApp.Common;
    using ArmManipulatorApp.Graphics3DModel.Model3D;
    using ArmManipulatorApp.MathModel.Trajectory;

    using MainApp.Common;

    using Point3D = System.Windows.Media.Media3D.Point3D;

    public class ApplicationViewModel
    {
        private ManipulatorArmModel3D armModel3D;
        private TrajectoryModel3D track3D;
        private CameraModel3D camera;
        private SceneModel3D scene;

        IFileService fileService;
        IDialogService dialogService;

        private Viewport3D viewport;
        private TextBox manipTextBox;
        private Chart deltaChart;

        #region Temporary region for refactoring
        private byte keyboardMod;
        private Point MousePos;
        private Point offset;

        /// <summary>
        /// Задаёт отношение реальных физических величин манипулятора
        /// от пиксельной характеристики виртуальной 3D модели манипулятора: len(px) = coeff * len(cm)
        /// </summary>
        private double coeff;
        #endregion

        public ApplicationViewModel(IDialogService dialogService,
            IFileService fileService,
            Viewport3D viewport,
            TextBox manipTextBox,
            Chart deltaChart)
        {
            this.dialogService = dialogService;
            this.fileService = fileService;
            this.viewport = viewport;
            this.manipTextBox = manipTextBox;
            this.deltaChart = deltaChart;

            offset = new Point(504, 403);
            coeff = 1; // 0.5;
            keyboardMod = 0;
        }

        #region Manipulator

        private RelayCommand openArmCommand;
        public RelayCommand OpenArmCommand
        {
            get
            {
                return this.openArmCommand ?? 
                       (this.openArmCommand = new RelayCommand(
                            obj =>
                                   {
                                       try
                                       {
                                           if (this.dialogService.OpenFileDialog())
                                           {
                                               this.armModel3D = new ManipulatorArmModel3D(
                                                   this.fileService.OpenArm(
                                                       this.dialogService.FilePath));
                                               this.armModel3D.arm.DefaultA();
                                               this.armModel3D.arm.CalcMetaDataForStanding();
                                               this.armModel3D.BuildModelVisual3DCollection();

                                               // After parsing manipulator configuration file
                                               // on the screen appears 3D scene with axis and manipulator
                                               var maxArmLength = this.armModel3D.arm.MaxLength();
                                               this.camera = new CameraModel3D(2 * maxArmLength);
                                               this.scene = new SceneModel3D(10 * maxArmLength, 3);
                                               
                                               this.viewport.Camera = camera.PerspectiveCamera;
                                               this.viewport.Children.Add(scene.ModelVisual3D);
                                               foreach (var mv in armModel3D.armModelVisual3D)
                                                   this.viewport.Children.Add(mv);

                                               this.manipTextBox.Text = File.ReadAllText(this.dialogService.FilePath);
                                               this.dialogService.ShowMessage("File open!");
                                           }
                                       }
                                       catch (Exception ex)
                                       {
                                           this.dialogService.ShowMessage(ex.Message);
                                       }
                                   }));
            }
        }

        #endregion

        #region Trajectory

        private RelayCommand createNewTrajectoryCommand;
        public RelayCommand CreateNewTrajectoryCommand
        {
            get
            {
                return this.createNewTrajectoryCommand ??
                       (this.createNewTrajectoryCommand = new RelayCommand(
                               obj =>
                               {
                                   try
                                   {
                                       var firstPoint = this.armModel3D.arm.Fn();
                                       this.track3D = new TrajectoryModel3D(new Trajectory((Point3D)firstPoint));
                                       foreach (var mv in this.track3D.trackModelVisual3D)
                                           this.viewport.Children.Add(mv);

                                       UserControlMod.Mod = UserMod.TrajectoryAnchorPointCreation;
                                   }
                                   catch (Exception ex)
                                   {
                                       this.dialogService.ShowMessage(ex.Message);
                                   }
                               }));
            }
        }

        private RelayCommand openExistingTrajectoryCommand;
        public RelayCommand OpenExistingTrajectoryCommand
        {
            get
            {
                return openExistingTrajectoryCommand ??
                       (openExistingTrajectoryCommand = new RelayCommand(obj =>
                               {
                                   try
                                   {
                                       if (this.dialogService.OpenFileDialog())
                                       {
                                           this.track3D = new TrajectoryModel3D(fileService.OpenTrack(this.dialogService.FilePath));
                                           this.dialogService.ShowMessage("File open!");
                                       }
                                   }
                                   catch (Exception ex)
                                   {
                                       this.dialogService.ShowMessage(ex.Message);
                                   }
                               }));
            }
        }

        private RelayCommand saveTrajectoryCommand;
        public RelayCommand SaveTrajectoryCommand
        {
            get
            {
                return saveTrajectoryCommand ??
                       (saveTrajectoryCommand = new RelayCommand(obj =>
                       {
                           try
                           {
                               if (dialogService.SaveFileDialog() == true)
                               {
                                   fileService.SaveTrack(dialogService.FilePath, track3D.track);
                                   dialogService.ShowMessage("Файл сохранен");
                               }
                           }
                           catch (Exception ex)
                           {
                               dialogService.ShowMessage(ex.Message);
                           }
                       }));
            }
        }
        
        private RelayCommand splitByQtyTrajectoryCommand;
        public RelayCommand SplitByQtyTrajectoryCommand =>
            splitByQtyTrajectoryCommand ??
            (splitByQtyTrajectoryCommand = new RelayCommand(obj =>
                    {
                       try
                       {
                       }
                       catch (Exception ex)
                       {
                           this.dialogService.ShowMessage(ex.Message);
                       }
                    }));
        
        private RelayCommand splitByStepTrajectoryCommand;
        public RelayCommand SplitByStepTrajectoryCommand => 
            splitByStepTrajectoryCommand ??
            (splitByStepTrajectoryCommand = new RelayCommand(obj => 
                    {
                       try
                       {
                       }
                       catch (Exception ex)
                       {
                           this.dialogService.ShowMessage(ex.Message);
                       }
                    }));

        #endregion

        #region Planning trajectory

        private RelayCommand trackPlanningCommand;
        public RelayCommand TrackPlanningCommand =>
            trackPlanningCommand ??
            (trackPlanningCommand = new RelayCommand(obj =>
                    {
                        try
                        {
                            //public static List<double[]> PlanningTrajectory(Trajectory S, Arm model, List<Point3D> DeltaPoints, BackgroundWorker worker)
                            //{
                            //    var q = new List<double[]>();

                            //    for (var i = 1; i < S.NumOfExtraPoints; i++)
                            //    {
                            //        var tmpQ = new double[model.n - 1];
                            //        worker.ReportProgress((int)((float)i / S.NumOfExtraPoints * 100));
                            //        for (var j = 0; j < model.n - 1; j++)
                            //        {
                            //            model.LagrangeMethodToThePoint(S.ExactExtra[i - 1]);
                            //            tmpQ[j] = model.q[j];
                            //        }
                            //        q.Add(tmpQ);
                            //        DeltaPoints.Add(new Point3D(i - 1, model.GetPointError(S.ExactExtra[i - 1]), 0));
                            //    }

                            //    return q;
                            //}
                        }
                        catch (Exception ex)
                        {
                            this.dialogService.ShowMessage(ex.Message);
                        }
                    }));

        #endregion

        #region Moving animation
        
        private RelayCommand startStopAnimation;
        public RelayCommand StartStopAnimation =>
            startStopAnimation ??
            (startStopAnimation = new RelayCommand(obj =>
                    {
                        try
                        {
                        }
                        catch (Exception ex)
                        {
                            this.dialogService.ShowMessage(ex.Message);
                        }
                    }));

        private RelayCommand pauseAnimation;
        public RelayCommand PauseAnimation =>
            pauseAnimation ??
            (pauseAnimation = new RelayCommand(obj =>
                    {
                        try
                        {
                        }
                        catch (Exception ex)
                        {
                            this.dialogService.ShowMessage(ex.Message);
                        }
                    }));

        #endregion

        #region Viewport3D events

        public ICommand MouseWheel
        {
            get
            {
                return new RelayCommand(
                    obj =>
                    {
                        try
                        {
                            // TODO: fix this to independent from canvas size
                            if (camera.Zoom.ScaleX < 1)
                            {
                                camera.Zoom.ScaleX += (double) (obj as MouseWheelEventArgs).Delta / 555;
                                camera.Zoom.ScaleY += (double) (obj as MouseWheelEventArgs).Delta / 555;
                                camera.Zoom.ScaleZ += (double) (obj as MouseWheelEventArgs).Delta / 555;
                            }
                            else
                            {
                                camera.Zoom.ScaleX += (double) (obj as MouseWheelEventArgs).Delta / 333;
                                camera.Zoom.ScaleY += (double) (obj as MouseWheelEventArgs).Delta / 333;
                                camera.Zoom.ScaleZ += (double) (obj as MouseWheelEventArgs).Delta / 333;
                            }
                        }
                        catch (Exception ex)
                        {
                            this.dialogService.ShowMessage(ex.Message);
                        }
                    },
                    (obj) => this.camera != null);
            }
        }        
        
        public ICommand MouseMove
        {
            get
            {
                return new RelayCommand(
                    obj =>
                    {
                        try
                        {
                            switch (UserControlMod.Mod)
                            {
                                case UserMod.CameraRotation:
                                    if (Mouse.LeftButton == MouseButtonState.Pressed)
                                    {
                                        var nextMousePos = Mouse.GetPosition(obj as IInputElement);
                                        var dxdy = new Point(nextMousePos.X - MousePos.X, nextMousePos.Y - MousePos.Y);
                                        camera.AngleRotZ.Angle += dxdy.Y;
                                        camera.AngleRotZ.Angle -= dxdy.X;
                                        MousePos = nextMousePos;
                                    }
                                    else if (Mouse.LeftButton == MouseButtonState.Released)
                                    {
                                        MousePos = Mouse.GetPosition(obj as IInputElement);
                                    }

                                    break;
                                case UserMod.TrajectoryAnchorPointCreation:
                                    //if (this.trackModelVisual3D != null)
                                    //    Viewport3D.Children.Remove(this.trackModelVisual3D);
                                    //var myModel3DGroup = new Model3DGroup();
                                    //this.trackModelVisual3D = new ModelVisual3D();

                                    ////TODO: fix moving path cursor (when resize window this shit doesn't work), remove this fucking coeffs (0.0531177)
                                    var hitParams = new PointHitTestParameters(Mouse.GetPosition(obj as IInputElement));
                                    track3D.MoveCursorPathPoint(new Point3D(
                                        (hitParams.HitPoint.X - offset.X) * 0.0531177,
                                        (hitParams.HitPoint.Y - offset.Y) * 0.0531177,
                                        0));
                                    //myModel3DGroup.Children.Add(myGeometryModel);
                                    //this.trackModelVisual3D.Content = myModel3DGroup;
                                    //Viewport3D.Children.Add(this.trackModelVisual3D);
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            this.dialogService.ShowMessage(ex.Message);
                        }
                    },
                    (obj) => this.camera != null);
            }
        }
        
        /// <summary>
        /// По клику ЛКМ по сцене мы либо перемещаем камеру,
        /// либо создаём траекторию пути, либо редактируем траекторию пути.
        /// </summary>
        public ICommand MouseLeftButtonDown
        {
            get
            {
                return new RelayCommand(
                    obj =>
                    {
                        try
                        {
                            switch (UserControlMod.Mod)
                            {
                                case UserMod.CameraRotation:
                                    MousePos = Mouse.GetPosition(obj as IInputElement);
                                    break;
                                case UserMod.TrajectoryAnchorPointCreation:
                                    if (this.armModel3D == null)
                                    {
                                        MessageBox.Show("Firstly create manipulator model!");
                                        return;
                                    }
                                    
                                    //this.track3D.AddAnchorPoint(new Point3D(MousePos.X, MousePos.Y, 0));

                                    break;
                                case UserMod.TrajectoryEditing:
                                    //TODO: Editing path mode
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            this.dialogService.ShowMessage(ex.Message);
                        }
                    },
                    (obj) => this.camera != null);
            }
        }

        #endregion

        #region Temporary region for refactoring
        
        #region Trajectory creation mod

        #region Path
        private void UpDownTrajectoryPoints_Click(object sender, RoutedEventArgs e)
        {
            UserControlMod.Mod = UserMod.CameraRotation;
            keyboardMod = 1;
            //Viewport3D.Children.Remove(this.trackModelVisual3D);
            //this.indexTrajectoryPoint = 1;
        }

        private void FinishBuildingPath_Click(object sender, RoutedEventArgs e)
        {
            keyboardMod = 0;
            //PathBuilderGrid_Grid.Visibility = Visibility.Hidden;
            //foreach (var p in this.trajectoryPointsVisual3D)
              //  this.listTrajectoryPoints.Add(p.center);
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
                 //   if (this.indexTrajectoryPoint == this.trajectoryPointsVisual3D.Count - 1) return;
                //    this.indexTrajectoryPoint++;
                    break;
                case Key.P: //Previous point
                //    if (this.indexTrajectoryPoint == 1) return;
                //    this.indexTrajectoryPoint--;
                    break;
            }
        }

        private void ChangePathPointY(bool isUp)
        {
            var delta = isUp ? 0.25 : -0.25;
           // this.trajectoryPointsVisual3D[this.indexTrajectoryPoint].SetY(this.trajectoryPointsVisual3D[this.indexTrajectoryPoint].center.Y + delta);

           // (this.trajectoryPointsVisual3D[this.indexTrajectoryPoint].trajectoryModelVisual3D.Transform as TranslateTransform3D).OffsetY += delta;

            NeighborhoodLinesRotation();
        }

        private void NeighborhoodLinesRotation()
        {
            //Temporary solution
            //Insert new ModelVisual3D model of path line except old

            //TrajectoryLine trajectoryLine;
            //trajectoryLine.start = this.trajectoryPointsVisual3D[this.indexTrajectoryPoint - 1].center;
            //trajectoryLine.end = this.trajectoryPointsVisual3D[this.indexTrajectoryPoint].center;

            ////TODO: Extract to method next 8 line
            //var line = new MeshGeometry3D();
            //LineByTwoPoints(line, trajectoryLine.start, trajectoryLine.end, 0.13);
            //var lineBrush = Brushes.MediumPurple;
            //var lineMaterial = new DiffuseMaterial(lineBrush);
            //var lineGeometryModel = new GeometryModel3D(line, lineMaterial);
            //var pathLineModelVisual3D = new ModelVisual3D();
            //pathLineModelVisual3D.Content = lineGeometryModel;
            //Viewport3D.Children.Remove(this.trajectoryLinesVisual3D[this.indexTrajectoryPoint - 1].lineModelVisual3D);
            //Viewport3D.Children.Insert(this.indexTrajectoryPoint - 1, pathLineModelVisual3D);

            //trajectoryLine.lineModelVisual3D = pathLineModelVisual3D;
            //this.trajectoryLinesVisual3D.RemoveAt(this.indexTrajectoryPoint - 1);
            //this.trajectoryLinesVisual3D.Insert(this.indexTrajectoryPoint - 1, trajectoryLine);

            //if (this.indexTrajectoryPoint == this.trajectoryPointsVisual3D.Count - 1) return;

            //trajectoryLine.start = this.trajectoryPointsVisual3D[this.indexTrajectoryPoint].center;
            //trajectoryLine.end = this.trajectoryPointsVisual3D[this.indexTrajectoryPoint + 1].center;

            ////TODO: Extract to method next 8 line
            //line = new MeshGeometry3D();
            //LineByTwoPoints(line, trajectoryLine.start, trajectoryLine.end, 0.13);
            //lineBrush = Brushes.MediumPurple;
            //lineMaterial = new DiffuseMaterial(lineBrush);
            //lineGeometryModel = new GeometryModel3D(line, lineMaterial);
            //pathLineModelVisual3D = new ModelVisual3D();
            //pathLineModelVisual3D.Content = lineGeometryModel;
            //Viewport3D.Children.Remove(this.trajectoryLinesVisual3D[this.indexTrajectoryPoint].lineModelVisual3D);
            //Viewport3D.Children.Insert(this.indexTrajectoryPoint, pathLineModelVisual3D);

            //trajectoryLine.lineModelVisual3D = pathLineModelVisual3D;
            //this.trajectoryLinesVisual3D.RemoveAt(this.indexTrajectoryPoint);
            //this.trajectoryLinesVisual3D.Insert(this.indexTrajectoryPoint, trajectoryLine);

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

        private void SplitPathByStep_Button_Click(object sender, RoutedEventArgs e)
        {
            double step;
            //if (!double.TryParse(StepInCmToSplit_TextBox.Text, out step))
            //{
            //    MessageBox.Show("Invalid input of split step!");
            //    return;
            //}
            //SplitPath(this.listTrajectoryPoints, step);
        }

        #endregion

        #endregion

        #region Path Planning (when arm and trajectory exists

        private void PathPlanningButton_Click(object sender, RoutedEventArgs e)
        {
            if (armModel3D == null)
            {
                MessageBox.Show("Firstly create manipulator model!");
                return;
            }

            //Array.Clear(arrayQ, 0, arrayQ.Length);
            //var delta = new Delta();
            //var deltaViewModel = new DeltaPlotModel();
            //for (var i = 1; i < this.listSplitTrajectoryPoints.Count; i++)
            //{
            //    var pathPoint = this.listSplitTrajectoryPoints[i];
            //    model.Move(pathPoint);
            //    //var tmpQ = new double[model.n];
            //    //model.q.CopyTo(tmpQ, 0);
            //    //arrayQ[i] = tmpQ;
            //    //Thread.Sleep(1500);

            //    //ManipulatorTransformUpdate(model.q);//TODO:think what do with q
            //    var p = model.F(model.N);

            //    delta.DesiredPoints.Add(this.listSplitTrajectoryPoints[i]);
            //    delta.RealPoints.Add((Point3D)p);
            //}

            //delta.CalcDeltas();
            //var serializer = new JsonSerializer();
            //var stringWriter = new StringWriter();
            //using (var writer = new JsonTextWriter(stringWriter))
            //{
            //    writer.QuoteName = false;
            //    serializer.Serialize(writer, delta);
            //}
            //var json = stringWriter.ToString();
            //var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\ArmManipulatorApp_Tests\\Deltas\\deltas.txt");
            //File.WriteAllText(filePath, json);

            //for (var i = 0; i < delta.Deltas.Count; i++)
            //    deltaViewModel.Points.Add(new DataPoint(delta.Deltas[i], i));
        }

        #endregion
        #endregion
    }
}
