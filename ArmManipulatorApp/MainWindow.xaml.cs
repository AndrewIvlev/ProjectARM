namespace ArmManipulatorApp
{
    using System.Windows;

    using ArmManipulatorApp.Common;
    using ArmManipulatorApp.ViewModel;

    public partial class MainWindow : Window
    {
        private ApplicationViewModel appViewModel;

        public MainWindow()
        {
            this.InitializeComponent();
            this.appViewModel = new ApplicationViewModel(
                new DefaultDialogService(),
                new JsonFileService(),
                this.Viewport3D,
                this.ArmConfigTextBox,
                this.VectorQTextBox,
                this.PathLength,
                this.Chart,
                this.ChartUpper,
                this.ChartLower,
                this.WithConditionNumberCheckBox,
                this.WithInterpolationCheckBox,
                this.WithRepeatPlanningRadioButton,
                this.PathSplittingProgressBar,
                this.PathPlanningProgressBar,
                this.StepInMeterToSplitTextBox,
                this.NumberOfPointsToSplitTextBox,
                this.ThresholdForRepeatPlanning,
                //this.WorkingTime,
                this.SliderAnimation);

            this.DataContext = this.appViewModel;
        }
    }
}
