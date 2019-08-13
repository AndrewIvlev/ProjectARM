using System.ComponentModel;
using System.Runtime.CompilerServices;
using ManipulationSystemLibrary.MathModel;

namespace MainApp.ViewModel
{
    using System.IO;

    using Newtonsoft.Json;

    public class ManipulatorArmModel3D : INotifyPropertyChanged
    {
        private Arm manipulator;
 
        public ManipulatorArmModel3D(Arm manip)
        {
            manipulator = manip;
        }
 
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
