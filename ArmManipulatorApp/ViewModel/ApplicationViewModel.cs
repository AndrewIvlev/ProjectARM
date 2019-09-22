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
        private CursorPointModel3D cursorForAnchorPointCreation;

        IFileService fileService;
        IDialogService dialogService;

        private Viewport3D viewport;
        private TextBox armTextBox;
        private Chart deltaChart;

        private byte keyboardMod;
        private Point MousePos;

        /// <summary>
        /// Offset for calculation mouse position from 2D coordinates to 3D then camera is looking on xy plate
        /// </summary>
        private Point offset;

        /// <summary>
        /// Задаёт отношение реальных физических величин манипулятора
        /// от пиксельной характеристики виртуальной 3D модели манипулятора: len(px) = coeff * len(cm)
        /// </summary>
        private double coeff = 3;

        public ApplicationViewModel(IDialogService dialogService,
            IFileService fileService,
            Viewport3D viewport,
            TextBox armTextBox,
            Chart deltaChart)
        {
            this.dialogService = dialogService;
            this.fileService = fileService;
            this.viewport = viewport;
            this.armTextBox = armTextBox;
            this.deltaChart = deltaChart;

            this.offset = new Point(631, 196);
            this.coeff = 3;
            this.keyboardMod = 0;
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
                                                   this.fileService.OpenArm(this.dialogService.FilePath),
                                                   this.coeff);
                                               this.armModel3D.arm.DefaultA();
                                               this.armModel3D.arm.CalcMetaDataForStanding();
                                               this.armModel3D.BuildModelVisual3DCollection();

                                               // After parsing manipulator configuration file
                                               // on the screen appears 3D scene with axis and manipulator
                                               var maxArmLength = this.armModel3D.arm.MaxLength();
                                               this.camera = new CameraModel3D(2 * maxArmLength);
                                               this.scene = new SceneModel3D(10 * maxArmLength, 3);
                                               
                                               this.viewport.Camera = this.camera.PerspectiveCamera;
                                               this.viewport.Children.Add(this.scene.ModelVisual3D);
                                               foreach (var mv in this.armModel3D.armModelVisual3D)
                                                   this.viewport.Children.Add(mv);

                                               this.armTextBox.Text = File.ReadAllText(this.dialogService.FilePath);
                                               ////this.dialogService.ShowMessage("File open!");
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

        /// <summary>
        /// If track3D != null then delete it and create new track3D
        /// </summary>
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
                                        if (this.track3D != null)
                                        {
                                            foreach (var mv in this.track3D.trackModelVisual3D)
                                                this.viewport.Children.Remove(mv);
                                        }

                                        var firstPoint = this.armModel3D.arm.Fn();
                                        this.track3D = new TrajectoryModel3D(
                                            new Trajectory((Point3D)firstPoint),
                                            this.coeff);
                                        foreach (var mv in this.track3D.trackModelVisual3D)
                                            this.viewport.Children.Add(mv);
                                        
                                        this.camera.ViewFromAbove();
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
                return this.openExistingTrajectoryCommand ??
                       (this.openExistingTrajectoryCommand = new RelayCommand(obj =>
                               {
                                   try
                                   {
                                       if (this.dialogService.OpenFileDialog())
                                       {
                                           this.track3D = new TrajectoryModel3D(this.fileService.OpenTrack(this.dialogService.FilePath), this.coeff);
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
                return this.saveTrajectoryCommand ??
                       (this.saveTrajectoryCommand = new RelayCommand(obj =>
                       {
                           try
                           {
                               if (this.dialogService.SaveFileDialog() == true)
                               {
                                   this.fileService.SaveTrack(this.dialogService.FilePath, this.track3D.track);
                                   this.dialogService.ShowMessage("Файл сохранен");
                               }
                           }
                           catch (Exception ex)
                           {
                               this.dialogService.ShowMessage(ex.Message);
                           }
                       }));
            }
        }
        
        private RelayCommand addTrajectoryAnchorPointsCommand;
        public RelayCommand AddTrajectoryAnchorPointsCommand
        {
            get
            {
                return this.addTrajectoryAnchorPointsCommand
                       ?? (this.addTrajectoryAnchorPointsCommand = new RelayCommand(
                               obj =>
                                   {
                                       try
                                       {
                                           this.camera.ViewFromAbove();
                                           UserControlMod.Mod = UserMod.TrajectoryAnchorPointCreation;
                                       }
                                       catch (Exception ex)
                                       {
                                           this.dialogService.ShowMessage(ex.Message);
                                       }
                                   }));
            }
        }

        private RelayCommand upDownTrajectoryAnchorPointsCommand;
        public RelayCommand UpDownTrajectoryAnchorPointsCommand
        {
            get
            {
                return this.upDownTrajectoryAnchorPointsCommand
                       ?? (this.upDownTrajectoryAnchorPointsCommand = new RelayCommand(
                               obj =>
                                   {
                                       try
                                       {
                                       }
                                       catch (Exception ex)
                                       {
                                           this.dialogService.ShowMessage(ex.Message);
                                       }
                                   }));
            }
        }

        private RelayCommand splitByQtyTrajectoryCommand;
        public RelayCommand SplitByQtyTrajectoryCommand
        {
            get
            {
                return this.splitByQtyTrajectoryCommand
                       ?? (this.splitByQtyTrajectoryCommand = new RelayCommand(
                               obj =>
                                   {
                                       try
                                       {
                                       }
                                       catch (Exception ex)
                                       {
                                           this.dialogService.ShowMessage(ex.Message);
                                       }
                                   }));
            }
        }

        private RelayCommand splitByStepTrajectoryCommand;
        public RelayCommand SplitByStepTrajectoryCommand
        {
            get
            {
                return this.splitByStepTrajectoryCommand
                       ?? (this.splitByStepTrajectoryCommand = new RelayCommand(
                               obj =>
                                   {
                                       try
                                       {
                                       }
                                       catch (Exception ex)
                                       {
                                           this.dialogService.ShowMessage(ex.Message);
                                       }
                                   }));
            }
        }

        #endregion

        #region Planning trajectory

        private RelayCommand trackPlanningCommand;
        public RelayCommand TrackPlanningCommand => this.trackPlanningCommand ??
            (this.trackPlanningCommand = new RelayCommand(obj =>
                    {
                        try
                        {
                            // public static List<double[]> PlanningTrajectory(Trajectory S, Arm model, List<Point3D> DeltaPoints, BackgroundWorker worker)
                            // {
                            // var q = new List<double[]>();

                            // for (var i = 1; i < S.NumOfExtraPoints; i++)
                            // {
                            // var tmpQ = new double[model.n - 1];
                            // worker.ReportProgress((int)((float)i / S.NumOfExtraPoints * 100));
                            // for (var j = 0; j < model.n - 1; j++)
                            // {
                            // model.LagrangeMethodToThePoint(S.ExactExtra[i - 1]);
                            // tmpQ[j] = model.q[j];
                            // }
                            // q.Add(tmpQ);
                            // DeltaPoints.Add(new Point3D(i - 1, model.GetPointError(S.ExactExtra[i - 1]), 0));
                            // }

                            // return q;
                            // }
                        }
                        catch (Exception ex)
                        {
                            this.dialogService.ShowMessage(ex.Message);
                        }
                    }));

        #endregion

        #region Moving animation
        
        private RelayCommand startStopAnimation;
        public RelayCommand StartStopAnimation => this.startStopAnimation ??
            (this.startStopAnimation = new RelayCommand(obj =>
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
        public RelayCommand PauseAnimation => this.pauseAnimation ??
            (this.pauseAnimation = new RelayCommand(obj =>
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
                            if (this.camera.Zoom.ScaleX < 1)
                            {
                                this.camera.Zoom.ScaleX += (double)((MouseWheelEventArgs)obj).Delta / 555;
                                this.camera.Zoom.ScaleY += (double)((MouseWheelEventArgs)obj).Delta / 555;
                                this.camera.Zoom.ScaleZ += (double)((MouseWheelEventArgs)obj).Delta / 555;
                            }
                            else
                            {
                                this.camera.Zoom.ScaleX += (double)((MouseWheelEventArgs)obj).Delta / 333;
                                this.camera.Zoom.ScaleY += (double)((MouseWheelEventArgs)obj).Delta / 333;
                                this.camera.Zoom.ScaleZ += (double)((MouseWheelEventArgs)obj).Delta / 333;
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
                                        var dxdy = new Point(nextMousePos.X - this.MousePos.X, nextMousePos.Y - this.MousePos.Y);
                                        this.camera.AngleRotZ.Angle += dxdy.Y;
                                        this.camera.AngleRotZ.Angle -= dxdy.X;
                                        this.MousePos = nextMousePos;
                                    }
                                    else if (Mouse.LeftButton == MouseButtonState.Released)
                                    {
                                        this.MousePos = Mouse.GetPosition(obj as IInputElement);
                                    }

                                    break;
                                case UserMod.TrajectoryAnchorPointCreation:
                                    var mousePosition = Mouse.GetPosition(obj as Viewport3D);
                                    if (this.cursorForAnchorPointCreation != null)
                                    {
                                        this.cursorForAnchorPointCreation.Hide();
                                        var X = (mousePosition.X - offset.X);
                                        var Y = (offset.Y - mousePosition.Y);
                                        this.cursorForAnchorPointCreation.Move(
                                            new Point3D(
                                                X,
                                                Y,
                                                this.track3D.track.AnchorPoints[this.track3D.track.AnchorPoints.Count - 1].Z));
                                        var vpHeight = this.viewport.ActualHeight;
                                        var vpWidth = this.viewport.ActualWidth;
                                        this.cursorForAnchorPointCreation.Show();
                                    }
                                    else
                                    {
                                        this.cursorForAnchorPointCreation = new CursorPointModel3D(
                                            this.viewport,
                                            new Point3D(
                                                mousePosition.X,
                                                mousePosition.Y,
                                                this.track3D.track.AnchorPoints[this.track3D.track.AnchorPoints.Count - 1].Z));
                                        this.cursorForAnchorPointCreation.Show();
                                    }

                                    break;
                                case UserMod.TrajectoryAnchorPointUpAndDown:
                                    // TODO:
                                    break;
                                case UserMod.TrajectoryEditing:
                                    // TODO:
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
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
                                    this.MousePos = Mouse.GetPosition(obj as IInputElement);
                                    break;
                                case UserMod.TrajectoryAnchorPointCreation:
                                    var hitParams = new PointHitTestParameters(Mouse.GetPosition(obj as IInputElement));
                                    
                                    var X = (hitParams.HitPoint.X - offset.X) * 0.0531177;
                                    var Y = (hitParams.HitPoint.Y - offset.Y) * 0.0531177;
                                    this.track3D.AddAnchorPoint(new Point3D(MousePos.X, MousePos.Y, 0));
                                    break;
                                case UserMod.TrajectoryEditing:
                                    // TODO:
                                    break;
                                case UserMod.TrajectoryAnchorPointUpAndDown:
                                    // TODO:
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
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
            this.keyboardMod = 1;

            // Viewport3D.Children.Remove(this.trackModelVisual3D);
            // this.indexTrajectoryPoint = 1;
        }

        private void FinishBuildingPath_Click(object sender, RoutedEventArgs e)
        {
            this.keyboardMod = 0;

            // PathBuilderGrid_Grid.Visibility = Visibility.Hidden;
            // foreach (var p in this.trajectoryPointsVisual3D)
            // this.listTrajectoryPoints.Add(p.center);
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (this.keyboardMod != 1) return;
            switch (e.Key)
            {
                /*** Raise/lower point along Y axis ***/
                case Key.W:
                    this.ChangePathPointY(true);
                    break;
                case Key.S:
                    this.ChangePathPointY(false);
                    break;

                /*** Point selection ***/
                // Каким-то образом нужно подсвечивать сферу точки, которая выбирается
                // Можно например применить к сфере ScaleTransform3D увеличив временно её в размерах
                case Key.N: // Next point
                 // if (this.indexTrajectoryPoint == this.trajectoryPointsVisual3D.Count - 1) return;
                // this.indexTrajectoryPoint++;
                    break;
                case Key.P: // Previous point
                // if (this.indexTrajectoryPoint == 1) return;
                // this.indexTrajectoryPoint--;
                    break;
            }
        }

        private void ChangePathPointY(bool isUp)
        {
            var delta = isUp ? 0.25 : -0.25;

            // this.trajectoryPointsVisual3D[this.indexTrajectoryPoint].SetY(this.trajectoryPointsVisual3D[this.indexTrajectoryPoint].center.Y + delta);

            // (this.trajectoryPointsVisual3D[this.indexTrajectoryPoint].trajectoryModelVisual3D.Transform as TranslateTransform3D).OffsetY += delta;
            this.NeighborhoodLinesRotation();
        }

        private void NeighborhoodLinesRotation()
        {
            // Temporary solution
            // Insert new ModelVisual3D model of path line except old

            // TrajectoryLine trajectoryLine;
            // trajectoryLine.start = this.trajectoryPointsVisual3D[this.indexTrajectoryPoint - 1].center;
            // trajectoryLine.end = this.trajectoryPointsVisual3D[this.indexTrajectoryPoint].center;

            ////TODO: Extract to method next 8 line
            // var line = new MeshGeometry3D();
            // LineByTwoPoints(line, trajectoryLine.start, trajectoryLine.end, 0.13);
            // var lineBrush = Brushes.MediumPurple;
            // var lineMaterial = new DiffuseMaterial(lineBrush);
            // var lineGeometryModel = new GeometryModel3D(line, lineMaterial);
            // var pathLineModelVisual3D = new ModelVisual3D();
            // pathLineModelVisual3D.Content = lineGeometryModel;
            // Viewport3D.Children.Remove(this.trajectoryLinesVisual3D[this.indexTrajectoryPoint - 1].lineModelVisual3D);
            // Viewport3D.Children.Insert(this.indexTrajectoryPoint - 1, pathLineModelVisual3D);

            // trajectoryLine.lineModelVisual3D = pathLineModelVisual3D;
            // this.trajectoryLinesVisual3D.RemoveAt(this.indexTrajectoryPoint - 1);
            // this.trajectoryLinesVisual3D.Insert(this.indexTrajectoryPoint - 1, trajectoryLine);

            // if (this.indexTrajectoryPoint == this.trajectoryPointsVisual3D.Count - 1) return;

            // trajectoryLine.start = this.trajectoryPointsVisual3D[this.indexTrajectoryPoint].center;
            // trajectoryLine.end = this.trajectoryPointsVisual3D[this.indexTrajectoryPoint + 1].center;

            ////TODO: Extract to method next 8 line
            // line = new MeshGeometry3D();
            // LineByTwoPoints(line, trajectoryLine.start, trajectoryLine.end, 0.13);
            // lineBrush = Brushes.MediumPurple;
            // lineMaterial = new DiffuseMaterial(lineBrush);
            // lineGeometryModel = new GeometryModel3D(line, lineMaterial);
            // pathLineModelVisual3D = new ModelVisual3D();
            // pathLineModelVisual3D.Content = lineGeometryModel;
            // Viewport3D.Children.Remove(this.trajectoryLinesVisual3D[this.indexTrajectoryPoint].lineModelVisual3D);
            // Viewport3D.Children.Insert(this.indexTrajectoryPoint, pathLineModelVisual3D);

            // trajectoryLine.lineModelVisual3D = pathLineModelVisual3D;
            // this.trajectoryLinesVisual3D.RemoveAt(this.indexTrajectoryPoint);
            // this.trajectoryLinesVisual3D.Insert(this.indexTrajectoryPoint, trajectoryLine);

            // there is the bug :( bug#2
            // Because lenght changed when we up or down path point
            // And there is need to use ScaleTransform3D with RotateTransform3D
            // var rightLineLenght = (pathLinesVisual3D[indexPathPoint - 1].end - pathLinesVisual3D[indexPathPoint - 1].start).Length;
            // var h = pathLinesVisual3D[indexPathPoint].end.Y - pathLinesVisual3D[indexPathPoint].start.Y;
            // var rightLineAngle = Math.Asin(h / rightLineLenght);
            // (((pathLinesVisual3D[indexPathPoint - 1]
            // .lineModelVisual3D
            // .Transform as Transform3DGroup)
            // .Children[0] as RotateTransform3D)
            // .Rotation as AxisAngleRotation3D)
            // .Angle = rightLineAngle;

            // var leftLineLenght = (pathLinesVisual3D[indexPathPoint].end - pathLinesVisual3D[indexPathPoint].start).Length;
            // h = pathLinesVisual3D[indexPathPoint].end.Y - pathLinesVisual3D[indexPathPoint].start.Y;
            // var leftLineAngle = Math.Asin(h / leftLineLenght);
            // (((pathLinesVisual3D[indexPathPoint]
            // .lineModelVisual3D
            // .Transform as Transform3DGroup)
            // .Children[1] as RotateTransform3D)
            // .Rotation as AxisAngleRotation3D)
            // .Angle = leftLineAngle;
        }

        #endregion

        #region Splitting trajectory

        private void SplitPathByStep_Button_Click(object sender, RoutedEventArgs e)
        {
            double step;

            // if (!double.TryParse(StepInCmToSplit_TextBox.Text, out step))
            // {
            // MessageBox.Show("Invalid input of split step!");
            // return;
            // }
            // SplitPath(this.listTrajectoryPoints, step);
        }

        #endregion

        #endregion

        #region Path Planning (when arm and trajectory exists

        private void PathPlanningButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.armModel3D == null)
            {
                MessageBox.Show("Firstly create manipulator model!");
                return;
            }

            // Array.Clear(arrayQ, 0, arrayQ.Length);
            // var delta = new Delta();
            // var deltaViewModel = new DeltaPlotModel();
            // for (var i = 1; i < this.listSplitTrajectoryPoints.Count; i++)
            // {
            // var pathPoint = this.listSplitTrajectoryPoints[i];
            // model.Move(pathPoint);
            // //var tmpQ = new double[model.n];
            // //model.q.CopyTo(tmpQ, 0);
            // //arrayQ[i] = tmpQ;
            // //Thread.Sleep(1500);

            // //ManipulatorTransformUpdate(model.q);//TODO:think what do with q
            // var p = model.F(model.N);

            // delta.DesiredPoints.Add(this.listSplitTrajectoryPoints[i]);
            // delta.RealPoints.Add((Point3D)p);
            // }

            // delta.CalcDeltas();
            // var serializer = new JsonSerializer();
            // var stringWriter = new StringWriter();
            // using (var writer = new JsonTextWriter(stringWriter))
            // {
            // writer.QuoteName = false;
            // serializer.Serialize(writer, delta);
            // }
            // var json = stringWriter.ToString();
            // var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\ArmManipulatorApp_Tests\\Deltas\\deltas.txt");
            // File.WriteAllText(filePath, json);

            // for (var i = 0; i < delta.Deltas.Count; i++)
            // deltaViewModel.Points.Add(new DataPoint(delta.Deltas[i], i));
        }

        #endregion
        #endregion
    }
}
