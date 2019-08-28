namespace MainApp
{
    using System.Windows;
    using System.Windows.Input;

    using MainApp.Common;
    using MainApp.ViewModel;

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.DataContext = new ApplicationViewModel(new DefaultDialogService(), new JsonFileService());
        }
    }
}
