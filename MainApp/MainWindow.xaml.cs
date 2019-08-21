using System.Windows;
using MainApp.Common;
using MainApp.ViewModel;

namespace MainApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.DataContext = new ApplicationViewModel(new DefaultDialogService(), new JsonFileService());
        }
    }
}
