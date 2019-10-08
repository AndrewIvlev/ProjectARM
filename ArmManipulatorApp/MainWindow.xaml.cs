namespace ArmManipulatorApp
{
    using System.Windows;
    using System.Windows.Media.Animation;

    using ArmManipulatorApp.Common;
    using ArmManipulatorApp.ViewModel;

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.DataContext = new ApplicationViewModel(
                new DefaultDialogService(),
                new JsonFileService(),
                this.Viewport3D, 
                (this.Resources["StoryboardArmMoveAnimation"] as Storyboard),
                this.ArmConfigTextBox,
                this.StepInCmToSplitTextBox,
                this.NumberOfPointsToSplitTextBox,
                this.DeltaChart);
        }
    }
}
