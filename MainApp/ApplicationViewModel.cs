using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.ViewModel
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Collections.ObjectModel;
 
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        private ManipulatorArm3DModel arm;

        private Trajectory3DModel track;

        //public ObservableCollection<Phone> Phones { get; set; }
        //public Phone SelectedPhone
        //{
        //    get { return selectedPhone; }
        //    set
        //    {
        //        selectedPhone = value;
        //        OnPropertyChanged("SelectedPhone");
        //    }
        //}

        public ApplicationViewModel()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

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
    }
}
