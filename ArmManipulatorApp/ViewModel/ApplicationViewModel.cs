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

    using KeyEventArgs = System.Windows.Input.KeyEventArgs;
    using Label = System.Windows.Controls.Label;
    using Point3D = System.Windows.Media.Media3D.Point3D;
    using TextBox = System.Windows.Controls.TextBox;

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
        private Label pathLengthLabel;
        
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
        private bool SplitTrackWithInterpolation = false;

        public ApplicationViewModel(IDialogService dialogService,
            IFileService fileService,
            Viewport3D viewport,
            TextBox armTextBox,
            TextBox vectorQTextBox,
            Label pathLength)
        {
            this.dialogService = dialogService;
            this.fileService = fileService;

            this.viewport = viewport;
            this.armTextBox = armTextBox;
            this.VectorQTextBox = vectorQTextBox;
            this.pathLengthLabel = pathLength;

            this.qList = new List<double[]>();
            this.coeff = 10;
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
                                                   this.viewport.Children.Clear();
                                               }

                                               this.armModel3D = new ManipulatorArmModel3D(
                                                   this.fileService.OpenArm(this.dialogService.FilePath),
                                                   this.coeff);
                                               this.armModel3D.arm.Build_S_ForAllUnits_ByUnitsType();
                                               this.armModel3D.arm.Calc_T();
                                               var maxArmLength = this.armModel3D.arm.MaxLength();

                                               this.thickness = (maxArmLength / this.armModel3D.arm.N) * 0.13;
                                               this.armModel3D.BuildModelVisual3DCollection(this.thickness);

                                               // After parsing manipulator configuration file
                                               // on the screen appears 3D scene with axis and manipulator
                                               this.camera = new CameraModel3D(this.coeff * maxArmLength * 2);
                                               this.scene = new SceneModel3D(this.coeff * maxArmLength * 2, this.coeff * this.thickness * 0.5);
                                               
                                               this.viewport.Camera = this.camera.PerspectiveCamera;
                                               this.viewport.Children.Add(this.scene.ModelVisual3D);
                                               foreach (var mv in this.armModel3D.armModelVisual3D)
                                               {
                                                   this.viewport.Children.Add(mv);
                                               }

                                               this.viewport.Children.Add(
                                                   new ModelVisual3D
                                                       {
                                                           Content = new AmbientLight(Brushes.White.Color)
                                                       });

                                               this.armTextBox.Text = File.ReadAllText(this.dialogService.FilePath);
                                               this.VectorQTextBox.Text = JsonConvert.SerializeObject(this.armModel3D.arm.GetQ());
                                               if (this.ShowAllMessageBox)
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
                                        if (this.dialogService.SaveFileDialog())
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

                                        this.thickness = (this.armModel3D.arm.MaxLength() / this.armModel3D.arm.N) * 0.13;
                                        var firstPoint = this.armModel3D.arm.F(this.armModel3D.arm.N);
                                        this.track3D = new TrajectoryModel3D(
                                            new Trajectory((Point3D)firstPoint),
                                            this.viewport,
                                            this.thickness,
                                            this.coeff);
                                        foreach (var mv in this.track3D.trackModelVisual3D)
                                        {
                                            this.viewport.Children.Add(mv);
                                        }

                                        this.camera = new CameraModel3D(this.coeff * this.armModel3D.arm.MaxLength() * 2);
                                        this.camera.ViewFromAbove();
                                        this.viewport.Camera = this.camera.PerspectiveCamera;
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
                                           this.track3D = new TrajectoryModel3D(this.fileService.OpenTrack(this.dialogService.FilePath), this.viewport, this.thickness, this.coeff);
                                           this.track3D.AddAnchorTrackToViewport();
                                           if (this.ShowAllMessageBox)
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
                               if (this.dialogService.SaveFileDialog())
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
                                               this.camera = new CameraModel3D(this.coeff * this.armModel3D.arm.MaxLength() * 2);
                                               this.camera.ViewFromAbove();
                                               this.viewport.Camera = this.camera.PerspectiveCamera;
                                               UserControlMod.Mod = UserMod.TrajectoryAnchorPointCreation;
                                           }
                                           else
                                           {
                                               this.track3D.RemoveAnchorTrackFromViewport();
                                               this.track3D.AddAnchorPoint(this.cursorForAnchorPointCreation.position);
                                               this.track3D.AddAnchorTrackToViewport();

                                               this.pathLengthLabel.Content = $"Длина пути = {this.track3D.track.Length} м.";
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
                                            this.track3D.RemoveAnchorTrackFromViewport();
                                            this.track3D.ChangeAnchorPointZ(this.pointSelector.selectedPointIndex, deltaZ);
                                            this.track3D.AddAnchorTrackToViewport();
                                            this.pointSelector.MoveByOffset(new Point3D(0, 0, deltaZ * this.coeff));

                                            this.pathLengthLabel.Content = $"Длина пути = {this.track3D.track.Length} м.";
                                            break;
                                        case Key.S: // Decrease z coordinate of point
                                            this.track3D.RemoveAnchorTrackFromViewport();
                                            this.track3D.ChangeAnchorPointZ(this.pointSelector.selectedPointIndex, -deltaZ);
                                            this.track3D.AddAnchorTrackToViewport();
                                            this.pointSelector.MoveByOffset(new Point3D(0, 0, -deltaZ * this.coeff));

                                            this.pathLengthLabel.Content = $"Длина пути = {this.track3D.track.Length} м.";
                                            break;
                                        case Key.D: // Select next point
                                            if (this.pointSelector.selectedPointIndex
                                                != this.track3D.track.AnchorPoints.Count - 1)
                                            {
                                                this.pointSelector.selectedPointIndex++;
                                                var newSelectorPosition = VRConvert.ConvertFromRealToVirtual(
                                                    this.track3D.track.AnchorPoints[this.pointSelector.selectedPointIndex],
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

                                    this.pathLengthLabel.Content = $"Длина пути = {this.track3D.track.Length} м.";
                                }
                            }
                            catch (Exception ex)
                            {
                                this.dialogService.ShowMessage(ex.Message);
                            }
                        });
            }
        }

        public void SplitTrajectory(DoWorkEventArgs e, object sender, double stepInMToSplitStr, out List<double> distanceBetweenSplitPoints, out int CountOfSplitPoints)
        {
            this.track3D.SplitPath(e, sender, stepInMToSplitStr, this.SplitTrackWithInterpolation);
         
            distanceBetweenSplitPoints = this.track3D.track.GetListOfDistanceBetweenSplitPoints();
            CountOfSplitPoints = this.track3D.track.SplitPoints.Count;
            if (this.ShowAllMessageBox)
                this.dialogService.ShowMessage("Путь успешно разделён.");
        }

        public void SplitTrajectory(DoWorkEventArgs e, object sender, int numberOfSplitPoints)
        {
            this.track3D.SplitPath(e, sender, numberOfSplitPoints);
            if (this.ShowAllMessageBox)
                this.dialogService.ShowMessage("Путь успешно разделён.");
        }

        public ICommand InterpolateTrajectoryCommand
        {
            get
            {
                return new RelayCommand(
                    obj =>
                        {
                            try
                            {
                                var trackLen = this.track3D.track.GetLen();
                                if (trackLen > 0)
                                {
                                    this.SplitTrackWithInterpolation = true;
                                    this.dialogService.ShowMessage(
                                        "Интерполяция траектории произведена успешно.\nВведите шаг и разделите траекторию.");
                                }
                                else
                                {
                                    throw new Exception("Что-то пошло не так ;(");
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

        #region Planning trajectory

        public void PlanningMovementAlongTrajectory(
            DoWorkEventArgs e,
            object sender,
            bool withCond,
            bool withRepeatPlan,
            double threshold,
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
            for (var i = 1; i < splitPointsCount; i++)
            {
                var point = this.track3D.track.SplitPoints[i];

                this.armModel3D.arm.LagrangeMethodToThePoint(
                    point,
                    out var b,
                    out var d,
                    out var delta,
                    out var cond,
                    withCond);
                this.qList.Add(this.armModel3D.arm.GetQ());

                bList.Add(b);
                dList.Add(d);
                deltaList.Add(delta);
                condList.Add(cond);
                
                ++resIterCount;
                if (withRepeatPlan)
                {
                    if (b > threshold)
                    {
                        i--;
                    }
                }
                else
                {
                    ((BackgroundWorker)sender).ReportProgress(resIterCount + 1);
                }

                if (((BackgroundWorker)sender).CancellationPending == true)
                {
                    e.Cancel = true;
                    return;
                }
            }

            e.Cancel = true;
        }

        #endregion

        #region Moving animation

        public void BeginAnimation(
            DoWorkEventArgs e,
            object sender,
            Viewport3D viewport3D)
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

                Thread.Sleep(60); //TODO: add value from speed slider
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
                                                    trajectoryFirstPoint.Z * this.coeff),
                                                (this.thickness * this.coeff / 2) * 0.8);
                                            this.viewport.Children.Add(this.cursorForAnchorPointCreation.ModelVisual3D);
                                        }
                                        else
                                        {                                
                                            var nextMousePos = Mouse.GetPosition(obj as IInputElement);
                                            this.viewport.Children.Remove(this.cursorForAnchorPointCreation.ModelVisual3D);
                                            this.cursorForAnchorPointCreation.MoveByOffset(
                                                VRConvert.ConvertFromVirtualToReal(new Point3D(
                                                    nextMousePos.X - this.MousePos.X,
                                                    -nextMousePos.Y + this.MousePos.Y,
                                                    0),
                                                    this.coeff));
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
    }
}
