namespace ArmManipulatorApp.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Forms;
    using System.Windows.Forms.DataVisualization.Charting;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Media3D;
    using System.Windows.Threading;

    using ArmManipulatorApp.Common;
    using ArmManipulatorApp.Graphics3DModel;
    using ArmManipulatorApp.Graphics3DModel.Model3D;
    using ArmManipulatorApp.MathModel.Trajectory;

    using MainApp.Common;

    using Newtonsoft.Json;

    using CheckBox = System.Windows.Controls.CheckBox;
    using RadioButton = System.Windows.Controls.RadioButton;
    using KeyEventArgs = System.Windows.Input.KeyEventArgs;
    using Label = System.Windows.Controls.Label;
    using Point3D = System.Windows.Media.Media3D.Point3D;
    using ProgressBar = System.Windows.Controls.ProgressBar;
    using TextBox = System.Windows.Controls.TextBox;

    public class ApplicationViewModel
    {
        private ManipulatorArmModel3D armModel3D;

        private TrajectoryModel3D track3D;

        private CameraModel3D camera;

        private SceneModel3D scene;

        private CursorPointModel3D cursorForAnchorPointCreation;

        private PointSelectorModel3D pointSelector;

        IFileService fileService;

        IDialogService dialogService;

        private Viewport3D viewport;

        private TextBox armTextBox;

        private TextBox VectorQTextBox;

        private Label pathLengthLabel;

        private Label SplitStepPathLabel;

        private Label IterationCountLabel;

        private Label AverageDeltaLabel;

        // Buffer of all calculated q's for animation
        private List<double[]> qList;

        private Point MousePos;

        /// <summary>
        /// Задаёт отношение реальных физических величин манипулятора
        /// от пиксельной характеристики виртуальной 3D модели манипулятора: len(px) = coeff * len(cm)
        /// </summary>
        private double coeff;

        private double thickness;

        private bool ShowAllMessageBox = false;

        Stopwatch timePlanning;

        BackgroundWorker splittingTrackWorker;

        BackgroundWorker planningWorker;

        BackgroundWorker animationWorker;

        private Chart Chart;

        private CheckBox WithConditionNumberCheckBox;

        private CheckBox WithBalancingCheckBox;

        private CheckBox WithInterpolationCheckBox;

        private RadioButton WithoutRepeatPlanningRadioButton;

        private RadioButton WithRepeatPlanningByThresholdRadioButton;

        private RadioButton WithRepeatPlanningByNumberTimesRadioButton;

        private ProgressBar PathSplittingProgressBar;

        private ProgressBar PathPlanningProgressBar;

        private TextBox StepInMeterToSplitTextBox;

        private TextBox NumberOfPointsToSplitTextBox;

        private TextBox RepeatNumberTimesPlanningTextBox;

        private TextBox ThresholdForRepeatPlanning;
        
        private TextBox ThresholdForBalancingTextBox;

        private Label WorkingTime;

        private Slider SliderAnimation;

        private int IterationCount;

        private List<double> bList;

        private List<double> dList;

        private List<double> deltaList;

        private List<double> CondList;

        private List<double> DistanceBetweenSplitPoints;

        private double ThresholdForPlanning;

        private double ThresholdForBalancing;

        private double stepInMeterToSplit;

        private int NumberTimesRepeatPlanning;
        
        private bool SplitTrackWithInterpolation;

        private bool WithCond;

        private bool WithBalancing;

        public ApplicationViewModel(
            IDialogService dialogService,
            IFileService fileService,
            Viewport3D viewport,
            TextBox armTextBox,
            TextBox vectorQTextBox,
            Label pathLength,
            Label SplitStepPathLabel,
            Label IterationCountLabel,
            Label AverageDeltaLabel,
            Chart Chart,
            CheckBox WithConditionNumberCheckBox,
            CheckBox WithBalancingCheckBox,
            CheckBox WithInterpolationCheckBox,
            RadioButton WithoutRepeatPlanningRadioButton,
            RadioButton WithRepeatPlanningByThresholdRadioButton,
            RadioButton WithRepeatPlanningByNumberTimesRadioButton,
            ProgressBar PathSplittingProgressBar,
            ProgressBar PathPlanningProgressBar,
            TextBox StepInMeterToSplitTextBox,
            TextBox NumberOfPointsToSplitTextBox,
            TextBox RepeatNumberTimesPlanningTextBox,
            TextBox ThresholdForRepeatPlanning,
            TextBox ThresholdForBalancingTextBox,
            Label WorkingTime,
            Slider SliderAnimation)
        {
            this.dialogService = dialogService;
            this.fileService = fileService;

            this.viewport = viewport;
            this.armTextBox = armTextBox;
            this.VectorQTextBox = vectorQTextBox;
            this.pathLengthLabel = pathLength;
            this.SplitStepPathLabel = SplitStepPathLabel;
            this.IterationCountLabel = IterationCountLabel;
            this.AverageDeltaLabel = AverageDeltaLabel;
            this.PathSplittingProgressBar = PathSplittingProgressBar;
            this.PathPlanningProgressBar = PathPlanningProgressBar;
            this.StepInMeterToSplitTextBox = StepInMeterToSplitTextBox;
            this.NumberOfPointsToSplitTextBox = NumberOfPointsToSplitTextBox;
            this.RepeatNumberTimesPlanningTextBox = RepeatNumberTimesPlanningTextBox;
            this.ThresholdForRepeatPlanning = ThresholdForRepeatPlanning;
            this.ThresholdForBalancingTextBox = ThresholdForBalancingTextBox;
            this.WorkingTime = WorkingTime;
            this.SliderAnimation = SliderAnimation;
            this.qList = new List<double[]>();
            this.coeff = 10;

            this.planningWorker = new BackgroundWorker();
            this.planningWorker.WorkerSupportsCancellation = true;
            this.planningWorker.WorkerReportsProgress = true;
            this.planningWorker.DoWork += this.PlanningWorkerDoWork;
            this.planningWorker.ProgressChanged += this.PlanningWorkerProgressChanged;
            this.planningWorker.RunWorkerCompleted += this.PlanningWorkerRunPlanningWorkerCompleted;

            this.splittingTrackWorker = new BackgroundWorker();
            this.splittingTrackWorker.WorkerSupportsCancellation = true;
            this.splittingTrackWorker.WorkerReportsProgress = true;
            this.splittingTrackWorker.DoWork += this.SplittingTrackWorkerDoWork;
            this.splittingTrackWorker.ProgressChanged += this.SplittingTrackWorkerProgressChanged;
            this.splittingTrackWorker.RunWorkerCompleted += this.SplittingTrackWorkerRunSplittingTrackWorkerCompleted;

            this.animationWorker = new BackgroundWorker();
            this.animationWorker.WorkerSupportsCancellation = true;
            this.animationWorker.WorkerReportsProgress = true;
            this.animationWorker.DoWork += this.animationWorker_DoWork;
            this.animationWorker.ProgressChanged += this.animationWorker_ProgressChanged;
            this.animationWorker.RunWorkerCompleted += this.animationWorker_RunWorkerCompleted;

            this.WithCond = false;
            this.WithBalancing = false;
            this.ThresholdForBalancing = double.MaxValue;
            this.ThresholdForPlanning = double.MaxValue;
            this.NumberTimesRepeatPlanning = 1;
            this.WithConditionNumberCheckBox = WithConditionNumberCheckBox;
            this.WithBalancingCheckBox = WithBalancingCheckBox;
            this.WithInterpolationCheckBox = WithInterpolationCheckBox;
            this.WithoutRepeatPlanningRadioButton = WithoutRepeatPlanningRadioButton;
            this.WithRepeatPlanningByThresholdRadioButton = WithRepeatPlanningByThresholdRadioButton;
            this.WithRepeatPlanningByNumberTimesRadioButton = WithRepeatPlanningByNumberTimesRadioButton;

            #region Charts initializing

            this.Chart = Chart;
            var chartArea = new ChartArea("Default");
            chartArea.AxisX.IsMarginVisible = false;
            chartArea.AxisY.IsMarginVisible = false;
            chartArea.AxisX.Title = "iteration";
            chartArea.AxisY.Title = "Δ(м)";
            chartArea.AxisY2.Title = "cond";
            this.Chart.ChartAreas.Add(chartArea);


            this.Chart.Series.Add(new Series("bSeries"));
            this.Chart.Series["bSeries"].ChartArea = "Default";
            this.Chart.Series["bSeries"].ChartType = SeriesChartType.Line;

            this.Chart.Series.Add(new Series("dSeries"));
            this.Chart.Series["dSeries"].ChartArea = "Default";
            this.Chart.Series["dSeries"].ChartType = SeriesChartType.Line;

            this.Chart.Series.Add(new Series("deltaSeries"));
            this.Chart.Series["deltaSeries"].Color = System.Drawing.Color.Red;
            this.Chart.Series["deltaSeries"].ChartArea = "Default";
            this.Chart.Series["deltaSeries"].ChartType = SeriesChartType.Line;

            this.Chart.Series.Add(new Series("CondSeries"));
            this.Chart.Series["CondSeries"].Color = System.Drawing.Color.Green;
            this.Chart.Series["CondSeries"].ChartArea = "Default";
            this.Chart.Series["CondSeries"].ChartType = SeriesChartType.Line;
            this.Chart.Series["CondSeries"].YAxisType = AxisType.Secondary;

            this.Chart.Series.Add(new Series("SplitPointsDistance"));
            this.Chart.Series["SplitPointsDistance"].ChartArea = "Default";
            this.Chart.Series["SplitPointsDistance"].ChartType = SeriesChartType.Line;

            // Add some values for chart display
            int[] axisXData = { 0, 1 };
            double[] axisYData = { 0.0, 1.0 };
            this.Chart.Series["bSeries"].Points.DataBindXY(axisXData, axisYData);
            
            #endregion
        }

        #region Manipulator

        private RelayCommand newArmCommand;

        public RelayCommand NewArmCommand
        {
            get
            {
                return this.newArmCommand ?? (this.newArmCommand = new RelayCommand(
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

        private RelayCommand openArmCommand;

        public RelayCommand OpenArmCommand
        {
            get
            {
                return this.openArmCommand ?? (this.openArmCommand = new RelayCommand(
                                                   obj =>
                                                       {
                                                           try
                                                           {
                                                               if (this.dialogService.OpenFileDialog(
                                                                   "ArmManipulatorApp_Tests\\ManipConfigFiles"))
                                                               {
                                                                   if (this.armModel3D != null)
                                                                   {
                                                                       this.viewport.Children.Clear();
                                                                   }

                                                                   this.armModel3D = new ManipulatorArmModel3D(
                                                                       this.fileService.OpenArm(
                                                                           this.dialogService.FilePath),
                                                                       this.coeff);
                                                                   this.armModel3D.arm
                                                                       .Build_S_ForAllUnits_ByUnitsType();
                                                                   this.armModel3D.arm.Calc_T();
                                                                   var maxArmLength = this.armModel3D.arm.MaxLength();

                                                                   this.thickness =
                                                                       (maxArmLength / this.armModel3D.arm.N) * 0.13;
                                                                   this.armModel3D.BuildModelVisual3DCollection(
                                                                       this.thickness);

                                                                   // After parsing manipulator configuration file
                                                                   // on the screen appears 3D scene with axis and manipulator
                                                                   this.camera = new CameraModel3D(
                                                                       this.coeff * maxArmLength * 2);
                                                                   this.scene = new SceneModel3D(
                                                                       this.coeff * maxArmLength * 2,
                                                                       this.coeff * this.thickness * 0.5);

                                                                   this.viewport.Camera = this.camera.PerspectiveCamera;
                                                                   this.viewport.Children.Add(this.scene.ModelVisual3D);
                                                                   foreach (var mv in this.armModel3D.armModelVisual3D)
                                                                   {
                                                                       this.viewport.Children.Add(mv);
                                                                   }

                                                                   this.viewport.Children.Add(
                                                                       new ModelVisual3D
                                                                           {
                                                                               Content = new AmbientLight(
                                                                                   Brushes.White.Color)
                                                                           });

                                                                   this.armTextBox.Text = File.ReadAllText(
                                                                       this.dialogService.FilePath);
                                                                   this.VectorQTextBox.Text =
                                                                       JsonConvert.SerializeObject(
                                                                           this.armModel3D.arm.GetQ());
                                                                   if (this.ShowAllMessageBox)
                                                                       this.dialogService.ShowMessage(
                                                                           "Файл манипулятора открыт.");
                                                               }
                                                           }
                                                           catch (Exception ex)
                                                           {
                                                               this.dialogService.ShowMessage(ex.Message);
                                                           }
                                                       }));
            }
        }

        private RelayCommand saveArmCommand;

        public RelayCommand SaveArmCommand
        {
            get
            {
                return this.saveArmCommand ?? (this.saveArmCommand = new RelayCommand(
                                                   obj =>
                                                       {
                                                           try
                                                           {
                                                               if (this.dialogService.SaveFileDialog())
                                                               {
                                                                   this.fileService.SaveArm(
                                                                       this.dialogService.FilePath,
                                                                       this.armModel3D.arm);
                                                                   this.dialogService.ShowMessage(
                                                                       "Файл манипулятора сохранен.");
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
                return this.createNewTrajectoryCommand ?? (this.createNewTrajectoryCommand = new RelayCommand(
                                                               obj =>
                                                                   {
                                                                       try
                                                                       {
                                                                           this.RemoveAnchorTrackFromViewport();

                                                                           this.thickness =
                                                                               (this.armModel3D.arm.MaxLength()
                                                                                / this.armModel3D.arm.N) * 0.13;
                                                                           var firstPoint = this.armModel3D.arm.F(
                                                                               this.armModel3D.arm.N);
                                                                           this.track3D = new TrajectoryModel3D(
                                                                               new Trajectory((Point3D)firstPoint),
                                                                               this.thickness,
                                                                               this.coeff);

                                                                           //this.AddAnchorTrackToViewport();

                                                                           this.camera = new CameraModel3D(
                                                                               this.coeff * this.armModel3D.arm
                                                                                   .MaxLength() * 2);
                                                                           this.camera.ViewFromAbove();
                                                                           this.viewport.Camera =
                                                                               this.camera.PerspectiveCamera;
                                                                           UserControlMod.Mod =
                                                                               UserMod.TrajectoryAnchorPointCreation;
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
                return this.openExistingTrajectoryCommand ?? (this.openExistingTrajectoryCommand = new RelayCommand(
                                                                  obj =>
                                                                      {
                                                                          try
                                                                          {
                                                                              if (this.dialogService.OpenFileDialog(
                                                                                  "ArmManipulatorApp_Tests\\Tracks"))
                                                                              {
                                                                                  this.RemoveAnchorTrackFromViewport();
                                                                                  this.track3D = new TrajectoryModel3D(
                                                                                      this.fileService.OpenTrack(
                                                                                          this.dialogService.FilePath),
                                                                                      this.thickness,
                                                                                      this.coeff);
                                                                                  this.AddAnchorTrackToViewport();
                                                                                  if (this.ShowAllMessageBox)
                                                                                      this.dialogService.ShowMessage(
                                                                                          "Файл траектории открыт.");
                                                                              }
                                                                          }
                                                                          catch (Exception ex)
                                                                          {
                                                                              this.dialogService.ShowMessage(
                                                                                  ex.Message);
                                                                          }
                                                                      }));
            }
        }

        private RelayCommand saveTrajectoryCommand;

        public RelayCommand SaveTrajectoryCommand
        {
            get
            {
                return this.saveTrajectoryCommand ?? (this.saveTrajectoryCommand = new RelayCommand(
                                                          obj =>
                                                              {
                                                                  try
                                                                  {
                                                                      if (this.dialogService.SaveFileDialog())
                                                                      {
                                                                          this.fileService.SaveTrack(
                                                                              this.dialogService.FilePath,
                                                                              this.track3D.track);
                                                                          this.dialogService.ShowMessage(
                                                                              "Файл траектории сохранен.");
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
                                           if (UserControlMod.Mod != UserMod.TrajectoryAnchorPointCreation)
                                           {
                                               this.camera = new CameraModel3D(
                                                   this.coeff * this.armModel3D.arm.MaxLength() * 2);
                                               this.camera.ViewFromAbove();
                                               this.viewport.Camera = this.camera.PerspectiveCamera;
                                               UserControlMod.Mod = UserMod.TrajectoryAnchorPointCreation;
                                           }
                                           else
                                           {
                                               this.track3D.AddAnchorPoint(this.cursorForAnchorPointCreation.position);
                                               this.AddAnchorTrackToViewport();

                                               this.pathLengthLabel.Content =
                                                   $"Длина пути = {this.track3D.track.Length} м";
                                           }
                                       }
                                       catch (Exception ex)
                                       {
                                           this.dialogService.ShowMessage(ex.Message);
                                       }
                                   }));
            }
        }

        private RelayCommand finishAddingTrajectoryAnchorPointsCommand;

        public RelayCommand FinishAddingTrajectoryAnchorPointsCommand
        {
            get
            {
                return this.finishAddingTrajectoryAnchorPointsCommand
                       ?? (this.finishAddingTrajectoryAnchorPointsCommand = new RelayCommand(
                               obj =>
                                   {
                                       try
                                       {
                                           UserControlMod.Mod = UserMod.CameraRotation;
                                           this.viewport.Children.Remove(
                                               this.cursorForAnchorPointCreation.ModelVisual3D);
                                           this.camera.DefaultView();
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
                                           UserControlMod.Mod = UserMod.CameraRotation;
                                           var trajectoryLastPoint =
                                               this.track3D.track.AnchorPoints[
                                                   this.track3D.track.AnchorPoints.Count - 1];
                                           this.pointSelector = new PointSelectorModel3D(
                                               VRConvert.ConvertFromRealToVirtual(trajectoryLastPoint, this.coeff),
                                               (this.thickness * this.coeff / 2) * 0.8,
                                               this.track3D.track.AnchorPoints.Count - 1);
                                           this.viewport.Children.Add(this.pointSelector.ModelVisual3D);
                                       }
                                       catch (Exception ex)
                                       {
                                           this.dialogService.ShowMessage(ex.Message);
                                       }
                                   }));
            }
        }

        public ICommand OnKeyDownHandler
        {
            get
            {
                return new RelayCommand(
                    obj =>
                        {
                            try
                            {
                                if (this.pointSelector != null)
                                {
                                    var deltaZ = this.thickness;
                                    switch (((KeyEventArgs)obj).Key)
                                    {
                                        case Key.W: // Increase z coordinate of point
                                            this.RemoveAnchorTrackFromViewport();
                                            this.track3D.ChangeAnchorPointZ(
                                                this.pointSelector.selectedPointIndex,
                                                deltaZ);
                                            this.AddAnchorTrackToViewport();
                                            this.pointSelector.MoveByOffset(new Point3D(0, 0, deltaZ * this.coeff));

                                            this.pathLengthLabel.Content =
                                                $"Длина пути = {this.track3D.track.Length} м";
                                            break;
                                        case Key.S: // Decrease z coordinate of point
                                            this.RemoveAnchorTrackFromViewport();
                                            this.track3D.ChangeAnchorPointZ(
                                                this.pointSelector.selectedPointIndex,
                                                -deltaZ);
                                            this.AddAnchorTrackToViewport();
                                            this.pointSelector.MoveByOffset(new Point3D(0, 0, -deltaZ * this.coeff));

                                            this.pathLengthLabel.Content =
                                                $"Длина пути = {this.track3D.track.Length} м";
                                            break;
                                        case Key.D: // Select next point
                                            if (this.pointSelector.selectedPointIndex
                                                != this.track3D.track.AnchorPoints.Count - 1)
                                            {
                                                this.pointSelector.selectedPointIndex++;
                                                var newSelectorPosition = VRConvert.ConvertFromRealToVirtual(
                                                    this.track3D.track.AnchorPoints[this.pointSelector
                                                        .selectedPointIndex],
                                                    this.coeff);
                                                this.pointSelector.MoveTo(newSelectorPosition);
                                            }

                                            break;
                                        case Key.A: // Select previous point
                                            if (this.pointSelector.selectedPointIndex != 1)
                                            {
                                                this.pointSelector.selectedPointIndex--;
                                                var newSelectorPosition = VRConvert.ConvertFromRealToVirtual(
                                                    this.track3D.track.AnchorPoints[this.pointSelector
                                                        .selectedPointIndex],
                                                    this.coeff);
                                                this.pointSelector.MoveTo(newSelectorPosition);
                                            }

                                            break;
                                    }
                                }
                                else if (UserControlMod.Mod == UserMod.TrajectoryAnchorPointCreation
                                         && ((KeyEventArgs)obj).Key == Key.Enter)
                                {
                                    // Like in AddTrajectoryAnchorPointsCommand
                                    this.RemoveAnchorTrackFromViewport();
                                    this.track3D.AddAnchorPoint(this.cursorForAnchorPointCreation.position);
                                    this.AddAnchorTrackToViewport();

                                    this.pathLengthLabel.Content = $"Длина пути = {this.track3D.track.Length} м";
                                }
                            }
                            catch (Exception ex)
                            {
                                this.dialogService.ShowMessage(ex.Message);
                            }
                        });
            }
        }

        public void SplitTrajectory(
            DoWorkEventArgs e,
            object sender,
            double stepInMToSplitStr,
            out List<double> distanceBetweenSplitPoints,
            out int CountOfSplitPoints)
        {
            if (this.SplitTrackWithInterpolation)
            {
                this.track3D.SplitPathWithInterpolation(e, sender, stepInMToSplitStr);
            }
            else
            {
                this.track3D.SplitPath(e, sender, stepInMToSplitStr);
            }

            distanceBetweenSplitPoints = this.track3D.track.GetListOfDistanceBetweenSplitPoints();
            CountOfSplitPoints = this.track3D.track.SplitPoints.Count;
            if (this.ShowAllMessageBox)
            {
                this.dialogService.ShowMessage("Путь успешно разделён.");
            }
        }

        private void RemoveAnchorTrackFromViewport()
        {
            if (this.track3D == null) return;
            foreach (var mv in this.track3D.trackModelVisual3D)
            {
                this.viewport.Children.Remove(mv);
            }

            this.track3D.trackModelVisual3D.Clear();
        }

        private void AddAnchorTrackToViewport()
        {
            foreach (var mv in this.track3D.trackModelVisual3D)
            {
                this.viewport.Children.Add(mv);
            }
        }

        private void RemoveSplitTrackFromViewport()
        {
            if (this.track3D == null) return;
            foreach (var mv in this.track3D.splitTrackModelVisual3D)
            {
                this.viewport.Children.Remove(mv);
            }

            this.track3D.splitTrackModelVisual3D.Clear();
        }

        private void AddSplitTrackToViewport()
        {
            foreach (var mv in this.track3D.splitTrackModelVisual3D)
            {
                this.viewport.Children.Add(mv);
            }
        }

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
                                if (UserControlMod.Mod == UserMod.TrajectoryAnchorPointCreation)
                                {
                                    return;
                                }

                                // TODO: rewrite this without magic number 1024
                                this.camera.Zoom.ScaleX += (double)((MouseWheelEventArgs)obj).Delta / 1024;
                                this.camera.Zoom.ScaleY += (double)((MouseWheelEventArgs)obj).Delta / 1024;
                                this.camera.Zoom.ScaleZ += (double)((MouseWheelEventArgs)obj).Delta / 1024;
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
                                            var dXdY = new Point(
                                                nextMousePos.X - this.MousePos.X,
                                                nextMousePos.Y - this.MousePos.Y);
                                            this.camera.AngleRotZ.Angle += dXdY.Y;
                                            this.camera.AngleRotZ.Angle -= dXdY.X;
                                            this.MousePos = nextMousePos;
                                        }
                                        else if (Mouse.LeftButton == MouseButtonState.Released)
                                        {
                                            this.MousePos = Mouse.GetPosition(obj as IInputElement);
                                        }

                                        break;
                                    case UserMod.TrajectoryAnchorPointCreation:
                                        if (Mouse.LeftButton == MouseButtonState.Pressed)
                                        {
                                            if (this.cursorForAnchorPointCreation == null)
                                            {
                                                var trajectoryFirstPoint =
                                                    this.track3D.track.AnchorPoints[
                                                        this.track3D.track.AnchorPoints.Count - 1];
                                                this.cursorForAnchorPointCreation = new CursorPointModel3D(
                                                    new Point3D(
                                                        trajectoryFirstPoint.X * this.coeff,
                                                        trajectoryFirstPoint.Y * this.coeff,
                                                        trajectoryFirstPoint.Z * this.coeff),
                                                    (this.thickness * this.coeff / 2) * 0.8);
                                                this.viewport.Children.Add(
                                                    this.cursorForAnchorPointCreation.ModelVisual3D);
                                            }
                                            else
                                            {
                                                var nextMousePos = Mouse.GetPosition(obj as IInputElement);
                                                this.viewport.Children.Remove(
                                                    this.cursorForAnchorPointCreation.ModelVisual3D);
                                                this.cursorForAnchorPointCreation.MoveByOffset(
                                                    VRConvert.ConvertFromVirtualToReal(
                                                        new Point3D(
                                                            nextMousePos.X - this.MousePos.X,
                                                            -nextMousePos.Y + this.MousePos.Y,
                                                            0),
                                                        this.coeff));
                                                this.viewport.Children.Add(
                                                    this.cursorForAnchorPointCreation.ModelVisual3D);
                                                this.MousePos = nextMousePos;
                                            }
                                        }
                                        else if (Mouse.LeftButton == MouseButtonState.Released)
                                        {
                                            this.MousePos = Mouse.GetPosition(obj as IInputElement);
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
                                        // Creation trajectory point migrate to button
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

        #region Settings
        
        public ICommand ChangeVectorQFromTextBox
        {
            get
            {
                return new RelayCommand(
                    obj =>
                    {
                        try
                        {
                            this.armModel3D.arm.SetQ(
                                JsonConvert.DeserializeObject<double[]>(this.VectorQTextBox.Text));
                            this.armModel3D.arm.Build_S_ForAllUnits_ByUnitsType();
                            this.armModel3D.arm.Calc_T();

                            this.armModel3D.BeginAnimation(
                                JsonConvert.DeserializeObject<double[]>(this.VectorQTextBox.Text));
                        }
                        catch (Exception ex)
                        {
                            this.dialogService.ShowMessage(ex.Message);
                        }
                    });
            }
        }

        #endregion

        #region Splitting trajectory region

        private void SplittingTrackWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            this.SplitTrajectory(
                e,
                sender,
                this.stepInMeterToSplit,
                out this.DistanceBetweenSplitPoints,
                out this.IterationCount);
        }

        private void SplittingTrackWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.PathSplittingProgressBar.Value = e.ProgressPercentage;
        }

        public ICommand StartSplittingTrack_Click
        {
            get
            {
                return new RelayCommand(
                    obj =>
                        {
                            try
                            {
                                if (this.StepInMeterToSplitTextBox.Text != string.Empty
                                    && this.NumberOfPointsToSplitTextBox.Text != string.Empty)
                                {
                                    this.dialogService.ShowMessage("Нужно выбрать только один вариант разбиения.");
                                }
                                else if (this.StepInMeterToSplitTextBox.Text != string.Empty)
                                {
                                    if (double.TryParse(this.StepInMeterToSplitTextBox.Text, out this.stepInMeterToSplit))
                                    {
                                        this.SplitStepPathLabel.Content = $"Шаг разбиения пути = {this.stepInMeterToSplit} м";
                                        this.SplitTrackWithInterpolation = (bool)this.WithInterpolationCheckBox.IsChecked;

                                        this.PathSplittingProgressBar.IsIndeterminate = true;
                                        this.PathSplittingProgressBar.Maximum = 1;
                                        this.PathSplittingProgressBar.Value = 0;
                                        this.RemoveAnchorTrackFromViewport();
                                        this.RemoveSplitTrackFromViewport();

                                        this.RemoveAnchorTrackFromViewport();

                                        this.splittingTrackWorker.RunWorkerAsync();
                                    }
                                    else
                                    {
                                        this.dialogService.ShowMessage("Некорректный ввод шага разбиения!");
                                    }
                                }
                                else if (int.TryParse(this.NumberOfPointsToSplitTextBox.Text, out var numberOfSplitPoints))
                                {
                                    var trackLen = this.track3D.track.GetLen();
                                    this.stepInMeterToSplit = trackLen / (double)numberOfSplitPoints;
                                    this.SplitStepPathLabel.Content = $"Шаг разбиения пути = {this.stepInMeterToSplit} м";
                                    this.RemoveAnchorTrackFromViewport();

                                    this.SplitTrackWithInterpolation = (bool)this.WithInterpolationCheckBox.IsChecked;

                                    this.PathSplittingProgressBar.IsIndeterminate = true;
                                    this.PathSplittingProgressBar.Maximum = 1;
                                    this.PathSplittingProgressBar.Value = 0;
                                    this.RemoveAnchorTrackFromViewport();
                                    this.RemoveSplitTrackFromViewport();

                                    this.RemoveAnchorTrackFromViewport();

                                    this.splittingTrackWorker.RunWorkerAsync();
                                }
                                else
                                {
                                    this.dialogService.ShowMessage("Некорректный ввод шага разбиения!");
                                }
                            }
                            catch (Exception ex)
                            {
                                this.dialogService.ShowMessage(ex.Message);
                            }
                        });
            }
        }

        public ICommand CancelSplittingTrack_Click
        {
            get
            {
                return new RelayCommand(
                    obj =>
                        {
                            try
                            {
                                this.splittingTrackWorker.CancelAsync();
                            }
                            catch (Exception ex)
                            {
                                this.dialogService.ShowMessage(ex.Message);
                            }
                        });
            }
        }

        private void SplittingTrackWorkerRunSplittingTrackWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.PathSplittingProgressBar.IsIndeterminate = false;

            this.ClearAllChartSeries(this.Chart);
            this.Chart.Series["SplitPointsDistance"].Points.DataBindXY(
                Enumerable.Range(0, this.IterationCount - 1).ToArray(),
                this.DistanceBetweenSplitPoints);

            this.track3D.ShowInterpolatedTrack();
            this.AddSplitTrackToViewport();

            this.pathLengthLabel.Content = $"Длина пути = {this.track3D.track.GetLen()} м";
        }

        #endregion

        #region Arm movement planning region

        public void PlanningMovementAlongTrajectory(
            DoWorkEventArgs e,
            object sender,
            out int resIterCount,
            out List<double> bList,
            out List<double> dList,
            out List<double> deltaList,
            out List<double> condList)
        {
            var splitPointsCount = this.track3D.track.SplitPoints.Count;
            bList = new List<double>();
            dList = new List<double>();
            deltaList = new List<double>();
            condList = new List<double>();

            this.qList.Clear();

            resIterCount = 0;
            var k = this.NumberTimesRepeatPlanning; // repeat planning to the same point k times
            for (var i = 1; i < splitPointsCount; i++)
            {
                var point = this.track3D.track.SplitPoints[i];

                for (var j = 0; j < k; j++)
                {
                    var cond = this.WithCond ? 0.0 : 1.0;
                    this.armModel3D.arm.LagrangeMethodToThePoint(
                        point,
                        out var b,
                        out var d,
                        out var delta,
                        ref cond,
                        this.ThresholdForBalancing);

                    this.qList.Add(this.armModel3D.arm.GetQ());

                    bList.Add(b);
                    dList.Add(d);
                    deltaList.Add(delta);
                    if (this.WithCond)
                        condList.Add(cond);

                    if (this.ThresholdForPlanning < double.MaxValue)
                    {
                        if (b > this.ThresholdForPlanning)
                        {
                            i--;
                        }
                    }
                    ++resIterCount;
                }

                ((BackgroundWorker)sender).ReportProgress(i);
                if (((BackgroundWorker)sender).CancellationPending == true)
                {
                    e.Cancel = true;
                    return;
                }
            }

            e.Cancel = true;
        }

        private void PlanningWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            this.PlanningMovementAlongTrajectory(
                e,
                sender,
                out this.IterationCount,
                out this.bList,
                out this.dList,
                out this.deltaList,
                out this.CondList);
        }

        private void PlanningWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.PathPlanningProgressBar.Value = e.ProgressPercentage;
        }

        public ICommand PlanningTrack_Click
        {
            get
            {
                return new RelayCommand(
                    obj =>
                        {
                            try
                            {
                                this.WithCond = (bool)this.WithConditionNumberCheckBox.IsChecked;
                                this.Chart.ChartAreas["Default"].AxisY2.Enabled = this.WithCond ? AxisEnabled.True : AxisEnabled.False;

                                this.WithBalancing = (bool)this.WithBalancingCheckBox.IsChecked;
                                if (this.WithBalancing)
                                {
                                    this.ThresholdForBalancing = double.Parse(this.ThresholdForBalancingTextBox.Text);
                                }
                                else
                                {
                                    this.ThresholdForBalancing = 0;
                                }

                                if ((bool)this.WithRepeatPlanningByThresholdRadioButton.IsChecked)
                                {
                                    this.ThresholdForPlanning = double.Parse(this.ThresholdForRepeatPlanning.Text);
                                    this.NumberTimesRepeatPlanning = 1;
                                }
                                else if ((bool)this.WithRepeatPlanningByNumberTimesRadioButton.IsChecked)
                                {
                                    this.NumberTimesRepeatPlanning = int.Parse(this.RepeatNumberTimesPlanningTextBox.Text);
                                    this.ThresholdForPlanning = double.MaxValue;
                                }
                                else
                                {
                                    this.NumberTimesRepeatPlanning = 1;
                                    this.ThresholdForPlanning = double.MaxValue;
                                }

                                //this.PathPlanningProgressBar.IsIndeterminate = !(bool)this.WithoutRepeatPlanningRadioButton.IsChecked;
                                this.PathPlanningProgressBar.Maximum = this.track3D.track.SplitPoints.Count - 1;
                                this.PathPlanningProgressBar.Value = 0;

                                this.timePlanning = Stopwatch.StartNew();
                                this.planningWorker.RunWorkerAsync();
                            }
                            catch (Exception ex)
                            {
                                this.dialogService.ShowMessage(ex.Message);
                            }
                        });
            }
        }

        public ICommand CancelPlanningTrack_Click
        {
            get
            {
                return new RelayCommand(
                    obj =>
                        {
                            try
                            {
                                this.planningWorker.CancelAsync();
                                this.timePlanning.Stop();
                                this.WorkingTime.Content = $"Продолжительность планирования = {timePlanning.ElapsedMilliseconds} мс";
                                this.IterationCountLabel.Content = $"Число итераций = {this.IterationCount}";
                                this.AverageDeltaLabel.Content = $"Средняя ошибка перемещения = {String.Format("{0:0.######}", this.deltaList.Average())} м";
                            }
                            catch (Exception ex)
                            {
                                this.dialogService.ShowMessage(ex.Message);
                            }
                        });
            }
        }

        private void PlanningWorkerRunPlanningWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.timePlanning.Stop();
            this.WorkingTime.Content = $"Продолжительность планирования = {timePlanning.ElapsedMilliseconds} мс";
            this.IterationCountLabel.Content = $"Число итераций = {this.IterationCount}";
            this.AverageDeltaLabel.Content = $"Средняя ошибка перемещения = {String.Format("{0:0.#####}", this.deltaList.Average())} м";

            this.armModel3D.arm.SetQ(0.0);
            this.VectorQTextBox.Text = JsonConvert.SerializeObject(this.armModel3D.arm.GetQ());
            this.armModel3D.arm.Build_S_ForAllUnits_ByUnitsType();
            this.armModel3D.arm.Calc_T();

            this.armModel3D.BeginAnimation(
                JsonConvert.DeserializeObject<double[]>(this.VectorQTextBox.Text));

            this.ClearAllChartSeries(this.Chart);

            //    this.Chart.Series["bSeries"].Points.Clear();
            //    this.Chart.Series["bSeries"].Points.DataBindXY(
            //        Enumerable.Range(0, this.IterationCount).ToArray(),
            //        this.bList);

            //    this.Chart.Series["dSeries"].Points.Clear();
            //    this.Chart.Series["dSeries"].Points.DataBindXY(
            //        Enumerable.Range(0, this.IterationCount).ToArray(),
            //        this.dList);

            if (this.WithCond)
            {
                this.Chart.Series["deltaSeries"].Points.DataBindXY(
                    Enumerable.Range(0, this.IterationCount).ToArray(),
                    this.deltaList);

                this.Chart.Series["CondSeries"].Points.DataBindXY(
                    Enumerable.Range(0, this.IterationCount).ToArray(),
                    this.CondList);
            }
            else
            {
                this.Chart.Series["deltaSeries"].Points.DataBindXY(
                    Enumerable.Range(0, this.IterationCount).ToArray(),
                    this.deltaList);
            }
        }

        #endregion

        #region Arm move animation region

        public void BeginAnimation(DoWorkEventArgs e, object sender, Viewport3D viewport3D)
        {
            // Temporary for building arm by q values
            //App.Current.Dispatcher.Invoke(
            //    DispatcherPriority.SystemIdle,
            //    new Action(delegate
            //        {
            //            var qq = new double[]{0.785398, -0.226893, 10, 1.5708}; //45* 13* 10 -90*
            //            foreach (var mv in this.armModel3D.armModelVisual3D)
            //            {
            //                viewport3D.Children.Remove(mv);
            //            }

            //            viewport3D.UpdateLayout();

            //            this.armModel3D.arm.SetQ(qq);
            //            this.armModel3D.arm.Build_S_ForAllUnits_ByUnitsType();
            //            this.armModel3D.arm.Calc_T();

            //            this.armModel3D.ClearModelVisual3DCollection();
            //            this.armModel3D.BuildModelVisual3DCollection(this.thickness);

            //            foreach (var mv in this.armModel3D.armModelVisual3D)
            //            {
            //                viewport3D.Children.Add(mv);
            //            }

            //            e.Cancel = true;
            //        }));

            //e.Cancel = true;
            //return;

            for (var i = 0; i < this.qList.Count; i++)
            {
                var q = this.qList[i];

                //Thread.Sleep(60); //TODO: add value from speed slider
                App.Current.Dispatcher.Invoke(
                    DispatcherPriority.SystemIdle,
                    new Action(
                        delegate
                        {
                            // TODO: Доделать анимацию по нормальному (с помощью Storyboard и transformation)
                            //this.armModel3D.arm.OffsetQ(dQ);
                            //this.armModel3D.arm.Build_S_ForAllUnits_ByUnitsType();
                            //this.armModel3D.arm.Calc_T();
                            //this.armModel3D.BeginAnimation(dQ);

                            // костыль анимации ;(
                            foreach (var mv in this.armModel3D.armModelVisual3D)
                            {
                                viewport3D.Children.Remove(mv);
                            }

                            viewport3D.UpdateLayout();

                            this.armModel3D.arm.SetQ(q);
                            this.armModel3D.arm.Build_S_ForAllUnits_ByUnitsType();
                            this.armModel3D.arm.Calc_T();

                            this.armModel3D.ClearModelVisual3DCollection();
                            this.armModel3D.BuildModelVisual3DCollection(this.thickness);

                            foreach (var mv in this.armModel3D.armModelVisual3D)
                            {
                                viewport3D.Children.Add(mv);
                            }
                        }));

                ((BackgroundWorker)sender).ReportProgress(i);

                if (((BackgroundWorker)sender).CancellationPending == true)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        public ICommand BeginAnimation_Click
        {
            get
            {
                return new RelayCommand(
                    obj =>
                        {
                            try
                            {
                                this.SliderAnimation.Maximum = this.IterationCount;

                                this.animationWorker.RunWorkerAsync();
                            }
                            catch (Exception ex)
                            {
                                this.dialogService.ShowMessage(ex.Message);
                            }
                        });
            }
        }

        public ICommand PauseAnimation_Click
        {
            get
            {
                return new RelayCommand(
                    obj =>
                        {
                            try
                            {
                                this.animationWorker.CancelAsync();
                            }
                            catch (Exception ex)
                            {
                                this.dialogService.ShowMessage(ex.Message);
                            }
                        });
            }
        }

        private void animationWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.BeginAnimation(e, sender, this.viewport);
        }

        private void animationWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.SliderAnimation.Value = e.ProgressPercentage;
            //this.Viewport3D.UpdateLayout();
        }

        private void animationWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }

        private void AnimationSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        }

        #endregion

        #region private

        private void ClearAllChartSeries(Chart chart)
        {
            foreach (var series in chart.Series)
            {
                series.Points.Clear();
            }
        }

        #endregion
    }
}
