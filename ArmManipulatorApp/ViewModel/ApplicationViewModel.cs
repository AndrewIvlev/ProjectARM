namespace ArmManipulatorApp.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Forms.DataVisualization.Charting;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Threading;

    using ArmManipulatorApp.Common;
    using ArmManipulatorApp.Graphics3DModel;
    using ArmManipulatorApp.Graphics3DModel.Model3D;
    using ArmManipulatorApp.MathModel.Trajectory;

    using ArmManipulatorArm.MathModel;
    using ArmManipulatorArm.MathModel.Matrix;

    using MainApp.Common;

    using Newtonsoft.Json;

    using Point3D = System.Windows.Media.Media3D.Point3D;

    public class ApplicationViewModel
    {
        private ManipulatorArmModel3D armModel3D;
        public TrajectoryModel3D track3D;
        private CameraModel3D camera;
        private SceneModel3D scene;
        private CursorPointModel3D cursorForAnchorPointCreation;
        private PointSelectorModel3D pointSelector;

        IFileService fileService;
        IDialogService dialogService;

        private Viewport3D viewport;
        private TextBox armTextBox;
        private TextBox VectorQTextBox;
        private TextBox stepInCmToSplitTextBox;
        private TextBox numberOfPointsToSplitTextBox;
        
        // Buffer of all calculated q's for animation
        private List<double[]> dQList;

        private Point MousePos;

        /// <summary>
        /// Задаёт отношение реальных физических величин манипулятора
        /// от пиксельной характеристики виртуальной 3D модели манипулятора: len(px) = coeff * len(cm)
        /// </summary>
        private double coeff;

        public ApplicationViewModel(IDialogService dialogService,
            IFileService fileService,
            Viewport3D viewport,
            TextBox armTextBox,
            TextBox vectorQTextBox,
            TextBox stepInCmToSplitTextBox,
            TextBox numberOfPointsToSplitTextBox)
        {
            this.dialogService = dialogService;
            this.fileService = fileService;

            this.viewport = viewport;
            this.armTextBox = armTextBox;
            this.VectorQTextBox = vectorQTextBox;
            this.stepInCmToSplitTextBox = stepInCmToSplitTextBox;
            this.numberOfPointsToSplitTextBox = numberOfPointsToSplitTextBox;

            this.dQList = new List<double[]>();
            this.coeff = 3;
        }

        #region Manipulator

        private RelayCommand newArmCommand;
        public RelayCommand NewArmCommand
        {
            get
            {
                return this.newArmCommand ?? 
                       (this.newArmCommand = new RelayCommand(
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
                return this.openArmCommand ?? 
                       (this.openArmCommand = new RelayCommand(
                            obj =>
                                   {
                                       try
                                       {
                                           if (this.dialogService.OpenFileDialog("ArmManipulatorApp_Tests\\ManipConfigFiles"))
                                           {
                                               if (this.armModel3D != null)
                                               {
                                                   foreach (var mv in this.armModel3D.armModelVisual3D)
                                                   {
                                                       this.viewport.Children.Remove(mv);
                                                   }
                                               }

                                               this.armModel3D = new ManipulatorArmModel3D(
                                                   this.fileService.OpenArm(this.dialogService.FilePath),
                                                   this.coeff);
                                               this.armModel3D.arm.CalcSByUnitsType();
                                               this.armModel3D.arm.CalcT();
                                               this.armModel3D.BuildModelVisual3DCollection();

                                               // After parsing manipulator configuration file
                                               // on the screen appears 3D scene with axis and manipulator
                                               var maxArmLength = this.armModel3D.arm.MaxLength();
                                               this.camera = new CameraModel3D(10 * maxArmLength);
                                               this.scene = new SceneModel3D(10 * maxArmLength, 3);
                                               
                                               this.viewport.Camera = this.camera.PerspectiveCamera;
                                               this.viewport.Children.Add(this.scene.ModelVisual3D);
                                               foreach (var mv in this.armModel3D.armModelVisual3D)
                                               {
                                                   this.viewport.Children.Add(mv);
                                               }

                                               this.armTextBox.Text = File.ReadAllText(this.dialogService.FilePath);
                                               this.VectorQTextBox.Text = JsonConvert.SerializeObject(this.armModel3D.arm.GetQ());
                                               this.dialogService.ShowMessage("Файл манипулятора открыт.");
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
                return this.saveArmCommand ?? 
                       (this.saveArmCommand = new RelayCommand(
                            obj =>
                                {
                                    try
                                    {
                                        if (this.dialogService.SaveFileDialog() == true)
                                        {
                                            this.fileService.SaveArm(this.dialogService.FilePath, this.armModel3D.arm);
                                            this.dialogService.ShowMessage("Файл манипулятора сохранен.");
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
                                            {
                                                this.viewport.Children.Remove(mv);
                                            }
                                        }

                                        var firstPoint = this.armModel3D.arm.F(this.armModel3D.arm.N);
                                        this.track3D = new TrajectoryModel3D(
                                            new Trajectory((Point3D)firstPoint),
                                            this.viewport,
                                            this.coeff);
                                        foreach (var mv in this.track3D.trackModelVisual3D)
                                        {
                                            this.viewport.Children.Add(mv);
                                        }
                                        
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
                                       if (this.dialogService.OpenFileDialog("ArmManipulatorApp_Tests\\Tracks"))
                                       {
                                           this.track3D?.RemoveAnchorTrackFromViewport();
                                           this.track3D = new TrajectoryModel3D(this.fileService.OpenTrack(this.dialogService.FilePath), this.viewport, this.coeff);
                                           this.track3D.AddAnchorTrackToViewport();
                                           this.dialogService.ShowMessage("Файл траектории открыт.");
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
                                   this.dialogService.ShowMessage("Файл траектории сохранен.");
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
                                               this.camera.ViewFromAbove();
                                               UserControlMod.Mod = UserMod.TrajectoryAnchorPointCreation;
                                           }
                                           else
                                           {
                                               this.track3D.RemoveAnchorTrackFromViewport();
                                               this.track3D.AddAnchorPoint(this.cursorForAnchorPointCreation.position);
                                               this.track3D.AddAnchorTrackToViewport();
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
                                           this.viewport.Children.Remove(this.cursorForAnchorPointCreation.ModelVisual3D);
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
                                               this.track3D.track.AnchorPoints[this.track3D.track.AnchorPoints.Count - 1];
                                           this.pointSelector = new PointSelectorModel3D(
                                               VRConvert.ConvertFromRealToVirtual(trajectoryLastPoint, this.coeff),
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
                                    var deltaZ = 0.5;
                                    switch (((KeyEventArgs)obj).Key)
                                    {
                                        case Key.W: // Increase z coordinate of point
                                            this.track3D.RemoveAnchorTrackFromViewport();
                                            this.track3D.ChangeAnchorPointZ(this.pointSelector.selectedPointIndex, deltaZ);
                                            this.track3D.AddAnchorTrackToViewport();
                                            this.pointSelector.MoveByOffset(VRConvert.ConvertFromRealToVirtual(new Point3D(0, 0, deltaZ), this.coeff));
                                            break;
                                        case Key.S: // Decrease z coordinate of point
                                            this.track3D.RemoveAnchorTrackFromViewport();
                                            this.track3D.ChangeAnchorPointZ(this.pointSelector.selectedPointIndex, -deltaZ);
                                            this.track3D.AddAnchorTrackToViewport();
                                            this.pointSelector.MoveByOffset(VRConvert.ConvertFromRealToVirtual(new Point3D(0, 0, -deltaZ), this.coeff));
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
                                else if (UserControlMod.Mod == UserMod.TrajectoryAnchorPointCreation && ((KeyEventArgs)obj).Key == Key.Enter)
                                {
                                    // Like in AddTrajectoryAnchorPointsCommand
                                    this.track3D.RemoveAnchorTrackFromViewport();
                                    this.track3D.AddAnchorPoint(this.cursorForAnchorPointCreation.position);
                                    this.track3D.AddAnchorTrackToViewport();
                                }
                            }
                            catch (Exception ex)
                            {
                                this.dialogService.ShowMessage(ex.Message);
                            }
                        });
            }
        }

        public ICommand SplitTrajectoryCommand
        {
            get
            {
                return new RelayCommand(
                    obj =>
                        {
                            try
                            {
                                var stepInCmToSplitStr = this.stepInCmToSplitTextBox.Text;
                                var numberOfPointsToSplitStr = this.numberOfPointsToSplitTextBox.Text;
                                if (stepInCmToSplitStr != string.Empty && numberOfPointsToSplitStr != string.Empty)
                                {
                                    MessageBox.Show("Please choose only one option.");
                                }
                                else if (stepInCmToSplitStr != string.Empty)
                                {
                                    if (double.TryParse(stepInCmToSplitStr, out var step))
                                    {
                                        this.track3D.RemoveSplitTrackFromViewport();
                                        this.track3D.SplitPath(step);
                                        this.track3D.AddSplitTrackToViewport();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Invalid input of split step!");
                                    }
                                }
                                else if (int.TryParse(numberOfPointsToSplitStr, out var numberOfSplitPoints))
                                {
                                    this.track3D.RemoveSplitTrackFromViewport();
                                    this.track3D.SplitPath(numberOfSplitPoints);
                                    this.track3D.AddSplitTrackToViewport();
                                }
                                else
                                {
                                    MessageBox.Show("Invalid input of split step!");
                                }
                            }
                            catch (Exception ex)
                            {
                                this.dialogService.ShowMessage(ex.Message);
                            }
                        });
            }
        }

        #endregion

        #region Planning trajectory
        
        public void PlanningMovementAlongTrajectory(object sender, bool withCond, bool withRepeatPlan, double threshold, out int resIterCount, out double[] deltaList, out double[] condList)
        {
            var iterationCount = 1;
            var splitPointsCount = this.track3D.track.SplitPoints.Count;
            deltaList = new double[splitPointsCount];
            condList = new double[splitPointsCount];

            this.armModel3D.arm.CalcSByUnitsType();
            this.armModel3D.arm.CalcT();
            var indexOfStartPoint = 1;
            for (var i = indexOfStartPoint; i < splitPointsCount; i++)
            {
                var point = this.track3D.track.SplitPoints[i];

                this.armModel3D.arm.CalcdS();
                this.armModel3D.arm.CalcdT();
                this.armModel3D.arm.CalcD();
                this.armModel3D.arm.CalcC();
                var dQ = this.armModel3D.arm.LagrangeMethodToThePoint(
                    point,
                    out var cond,
                    withCond);
                this.armModel3D.arm.OffsetQ(dQ);
                //this.dQList.Add(dQ);

                this.armModel3D.arm.CalcSByUnitsType();
                this.armModel3D.arm.CalcT();

                var delta = this.armModel3D.arm.GetPointError(point);
                deltaList[i] = delta;
                condList[i] = cond;
                
                ++iterationCount;
                if (withRepeatPlan)
                {
                    if (delta > threshold)
                    {
                        i--;
                    }
                }
                else
                {
                    (sender as BackgroundWorker).ReportProgress(iterationCount);
                }
            }

            resIterCount = iterationCount;
        }

        #endregion

        #region Moving animation

        private RelayCommand startStopAnimation;
        public RelayCommand StartStopAnimation
            => this.startStopAnimation ?? (this.startStopAnimation = new RelayCommand(
                                               obj =>
                                                   {
                                                       try
                                                       {
                                                           // RunWorkerAsync
                                                           foreach (var dQ in this.dQList)
                                                           {
                                                               this.armModel3D.arm.OffsetQ(dQ);
                                                               this.armModel3D.arm.CalcSByUnitsType();
                                                               this.armModel3D.arm.CalcT();
                                                               this.armModel3D.BeginAnimation(dQ);
                                                                    
                                                               //Thread.Sleep(600);
                                                           }
                                                       }
                                                       catch (Exception ex)
                                                       {
                                                           this.dialogService.ShowMessage(ex.Message);
                                                       }
                                                   }));

        private RelayCommand pauseAnimation;
        public RelayCommand PauseAnimation
            => this.pauseAnimation ?? (this.pauseAnimation = new RelayCommand(
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
                                        var dXdY = new Point(nextMousePos.X - this.MousePos.X, nextMousePos.Y - this.MousePos.Y);
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
                                                this.track3D.track.AnchorPoints[this.track3D.track.AnchorPoints.Count - 1];
                                            this.cursorForAnchorPointCreation = new CursorPointModel3D(
                                                new Point3D(
                                                    trajectoryFirstPoint.X * this.coeff,
                                                    trajectoryFirstPoint.Y * this.coeff,
                                                    trajectoryFirstPoint.Z * this.coeff));
                                            this.viewport.Children.Add(this.cursorForAnchorPointCreation.ModelVisual3D);
                                        }
                                        else
                                        {                                
                                            var nextMousePos = Mouse.GetPosition(obj as IInputElement);
                                            this.viewport.Children.Remove(this.cursorForAnchorPointCreation.ModelVisual3D);
                                            this.cursorForAnchorPointCreation.MoveByOffset(
                                                new Point3D(
                                                    nextMousePos.X - this.MousePos.X,
                                                    -nextMousePos.Y + this.MousePos.Y,
                                                    0));
                                            this.viewport.Children.Add(this.cursorForAnchorPointCreation.ModelVisual3D);
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
                                this.armModel3D.arm.SetQ(JsonConvert.DeserializeObject<double[]>(this.VectorQTextBox.Text));
                                this.armModel3D.arm.CalcSByUnitsType();
                                this.armModel3D.arm.CalcT();

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
    }
}
