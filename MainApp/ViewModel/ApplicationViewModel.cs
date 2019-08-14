namespace MainApp.ViewModel
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;

    using MainApp.Common;

    using ManipulationSystemLibrary;
    using ManipulationSystemLibrary.MathModel;

    public class ApplicationViewModel : INotifyPropertyChanged
    {
        private Arm arm;
        private Trajectory track;

        private ManipulatorArmModel3D armModel3D;
        private TrajectoryModel3D track3D;

        IFileService fileService;
        IDialogService dialogService;

        public ApplicationViewModel(IDialogService dialogService, IFileService fileService)
        {
            this.dialogService = dialogService;
            this.fileService = fileService;

            // Здесь можно задать значения (по умлолчанию) для arm и track
            // arm = new ManipulatorArmModel3D();
        }

        #region Manipulator

        private RelayCommand openArmCommand;
        public RelayCommand OpenArmCommand =>
            openArmCommand ??
               (openArmCommand = new RelayCommand(obj =>
                       {
                           try
                           {
                               if (dialogService.OpenFileDialog())
                               {
                                   arm = fileService.OpenArm(dialogService.FilePath);
                                   arm.DefaultA();
                                   arm.CalcMetaDataForStanding();

                                   armModel3D = new ManipulatorArmModel3D(arm);
                                   //CreateManipulator3DVisualModel(model);
                                   //AddTransformationsForManipulator();
                                   //ManipulatorTransformUpdate(model.q);

                                   dialogService.ShowMessage("File open!");
                               }
                           }
                           catch (Exception ex)
                           {
                               dialogService.ShowMessage(ex.Message);
                           }
                       }));

        #endregion

        #region Trajectory

        private RelayCommand createNewTrajectoryCommand;
        public RelayCommand CreateNewTrajectoryCommand =>
            createNewTrajectoryCommand ??
            (createNewTrajectoryCommand = new RelayCommand(obj => 
                    {
                        try
                        {
                        }
                        catch (Exception ex)
                        {
                        }
                    }));

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
                                       if (dialogService.OpenFileDialog())
                                       {
                                           track = fileService.OpenTrack(dialogService.FilePath);
                                           dialogService.ShowMessage("File open!");
                                       }
                                   }
                                   catch (Exception ex)
                                   {
                                       dialogService.ShowMessage(ex.Message);
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
                                   fileService.SaveTrack(dialogService.FilePath, track);
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
                        }
                    }));
        
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
