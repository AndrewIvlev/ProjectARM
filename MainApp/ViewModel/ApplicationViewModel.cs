using MainApp.Common;
using ManipulationSystemLibrary;
using ManipulationSystemLibrary.MathModel;

namespace MainApp.ViewModel
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;

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
        public RelayCommand OpenArmCommand
        {
            get
            {
                return openArmCommand ??
                       (openArmCommand = new RelayCommand(obj =>
                       {
                           try
                           {
                               if (dialogService.OpenFileDialog())
                               {
                                   arm = fileService.Open(dialogService.FilePath);
                                   
                                   model.DefaultA();
                                   model.CalcMetaDataForStanding();
                                   CreateManipulator3DVisualModel(model);
                                   //AddTransformationsForManipulator();
                                   //ManipulatorTransformUpdate(model.q);

                                   dialogService.ShowMessage("Файл открыт");
                               }
                           }
                           catch (Exception ex)
                           {
                               dialogService.ShowMessage(ex.Message);
                           }
                       }));
            }
        }

        // Начало планирования со следующей точки пути
        //public static List<double[]> MovingAlongTheTrajectory(Trajectory S, Arm model, List<Point3D> DeltaPoints, BackgroundWorker worker)
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

        // Начало планирования с текущей точки пути
        /*public static double[][] MovingAlongTheTrajectory(Trajectory S, Arm model, List<Point3D> DeltaPoints, BackgroundWorker worker)
        {
            double[][] q = new double[S.NumOfExtraPoints][];
            for (int i = 0; i < S.NumOfExtraPoints; i++)
                q[i] = new double[model.n - 1];

            for (int i = 0; i < S.NumOfExtraPoints; i++)
            {
                worker.ReportProgress((int)((float)(i + 1) / S.NumOfExtraPoints * 100));
                for (int j = 0; j < model.n - 1; j++)
                    q[i][j] = model.LagrangeMethodToThePoint(S.ExactExtra[i])[j];

                DeltaPoints.Add(new Point3D(i, model.GetPointError(S.ExactExtra[i])));
            }

            return q;
        }*/

        #endregion

        #region Trajectory

        private RelayCommand createNewTrajectoryCommand;
        public RelayCommand CreateNewTrajectoryCommand
        {
            get
            {
                return createNewTrajectoryCommand ??
                       (createNewTrajectoryCommand = new RelayCommand(obj => 
                               {
                                   try
                                   {
                                       if (dialogService.SaveFileDialog() == true)
                                       {
                                           fileService.Save(dialogService.FilePath, Phones.ToList());
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
                                       if (dialogService.SaveFileDialog() == true)
                                       {
                                           fileService.Save(dialogService.FilePath, Phones.ToList());
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
                                   fileService.Save(dialogService.FilePath, Phones.ToList());
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
        public RelayCommand SplitByQtyTrajectoryCommand
        {
            get
            {
                return splitByQtyTrajectoryCommand ??
                       (splitByQtyTrajectoryCommand = new RelayCommand(obj =>
                       {
                           try
                           {
                               if (dialogService.SaveFileDialog() == true)
                               {
                                   fileService.Save(dialogService.FilePath, Phones.ToList());
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
        
        private RelayCommand splitByStepTrajectoryCommand;
        public RelayCommand SplitByStepTrajectoryCommand
        {
            get
            {
                return splitByStepTrajectoryCommand ??
                       (splitByStepTrajectoryCommand = new RelayCommand(obj =>
                       {
                           try
                           {
                               if (dialogService.SaveFileDialog() == true)
                               {
                                   fileService.Save(dialogService.FilePath, Phones.ToList());
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

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
