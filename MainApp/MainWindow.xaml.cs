namespace ArmManipulatorApp
{
    using System.Windows;
    using System.Windows.Input;

    using ArmManipulatorApp.Common;
    using ArmManipulatorApp.ViewModel;

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.DataContext = new ApplicationViewModel(new DefaultDialogService(), new JsonFileService());
        }
    }
}
