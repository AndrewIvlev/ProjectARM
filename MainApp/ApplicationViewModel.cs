namespace MainApp.ViewModel
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;

    public class ApplicationViewModel : INotifyPropertyChanged
    {
        private ManipulatorArm3DModel arm;
        private Trajectory3DModel track;

        IFileService fileService;
        IDialogService dialogService;

        public ApplicationViewModel(IDialogService dialogService, IFileService fileService)
        {
            this.dialogService = dialogService;
            this.fileService = fileService;

            // Здесь можно задать значения (по умлолчанию) для arm и track
            // arm = new ManipulatorArm3DModel();
        }
        // команда сохранения файла
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

        // команда открытия файла
        private RelayCommand openTrajectoryCommand;
        public RelayCommand OpenTrajectoryCommand
        {
            /*var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() != true)
            return;
            CreateManipulatorFromJson();*/
            get
            {
                return openTrajectoryCommand ??
                       (openTrajectoryCommand = new RelayCommand(obj =>
                               {
                                   try
                                   {
                                       if (dialogService.OpenFileDialog() == true)
                                       {
                                           var phones = fileService.Open(dialogService.FilePath);
                                           Phones.Clear();
                                           foreach (var p in phones)
                                               Phones.Add(p);
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
        
        // команда открытия файла
        private RelayCommand openArmCommand;
        public RelayCommand OpenArmCommand
        {
            /*var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() != true)
            return;
            CreateManipulatorFromJson();*/
            get
            {
                return openArmCommand ??
                       (openArmCommand = new RelayCommand(obj =>
                               {
                                   try
                                   {
                                       if (dialogService.OpenFileDialog() == true)
                                       {
                                           var phones = fileService.Open(dialogService.FilePath);
                                           Phones.Clear();
                                           foreach (var p in phones)
                                               Phones.Add(p);
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

        // команда добавления нового объекта
        //private RelayCommand addCommand;
        //public RelayCommand AddCommand
        //{
        //    get
        //    {
        //        return addCommand ??
        //               (addCommand = new RelayCommand(obj =>
        //                       {
        //                           Phone phone = new Phone();
        //                           Phones.Insert(0, phone);
        //                           SelectedPhone = phone;
        //                       }));
        //    }
        //}

        public void CreateManipulatorFromJson(string jsonFilePath)
        {
            arm.ConvertFromJson(jsonFilePath);

            model.DefaultA();
            model.CalcMetaDataForStanding();
            CreateManipulator3DVisualModel(model);

            //model.SetQ(new double[] {
            //    DegreeToRadian(-45),
            //    DegreeToRadian(30),
            //    DegreeToRadian(90),
            //    0,
            //    DegreeToRadian(60)
            //});

            //AddTransformationsForManipulator();
            //ManipulatorTransformUpdate(model.q);
        }

        private RelayCommand createNewTrajectoryCommand;
        public RelayCommand CreateNewTrajectoryCommand
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
        
        private RelayCommand openExistingTrajectoryCommand;
        public RelayCommand OpenExistingTrajectoryCommand
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
        splitByQtyTrajectoryCommand
            splitByStepTrajectoryCommand
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
