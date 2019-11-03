namespace ArmManipulatorApp
{
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Forms.DataVisualization.Charting;
    using System.Windows.Media.Animation;

    using ArmManipulatorApp.Common;
    using ArmManipulatorApp.ViewModel;

    public partial class MainWindow : Window
    {
        BackgroundWorker worker;

        private int IterationCount;
        private double[] DeltaList;
        private double[] CondList;
        private double ThresholdForPlanning;
        private bool WithRepeatPlanning;
        private bool WithCond;

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
                this.StepInCmToSplitTextBox,
                this.NumberOfPointsToSplitTextBox);
            this.DataContext = this.appViewModel;

            this.worker = new BackgroundWorker();
            this.worker.WorkerReportsProgress = true;
            this.worker.DoWork += this.worker_DoWork;
            this.worker.ProgressChanged += this.worker_ProgressChanged;
            this.worker.RunWorkerCompleted += this.worker_RunWorkerCompleted;

            this.WithCond = false;
            this.WithRepeatPlanning = false;
            this.ThresholdForPlanning = double.MaxValue;
            
            this.DeltaChart.ChartAreas.Add(new ChartArea("Default"));
            this.DeltaChart.Series.Add(new Series("DeltaSeries"));
            this.DeltaChart.Series["DeltaSeries"].ChartArea = "Default";
            this.DeltaChart.Series["DeltaSeries"].ChartType = SeriesChartType.Line;
            this.DeltaChart.Series.Add(new Series("CondSeries"));
            this.DeltaChart.Series["CondSeries"].ChartArea = "Default";
            this.DeltaChart.Series["CondSeries"].ChartType = SeriesChartType.Line;

            // Add some values for chart display
            int[] axisXData = { 0, 50, 100 };
            double[] axisYData = { 5.3, 1.3, 7.3 };
            this.DeltaChart.Series["DeltaSeries"].Points.DataBindXY(axisXData, axisYData);
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.appViewModel.PlanningMovementAlongTrajectory(
                sender,
                this.WithCond,
                this.WithRepeatPlanning,
                this.ThresholdForPlanning,
                out this.IterationCount,
                out this.DeltaList,
                out this.CondList);
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.PathPlanningProgressBar.Value = e.ProgressPercentage;
        }

        private void PlanningTrack_Click(object sender, RoutedEventArgs e)
        {
            this.WithCond = (bool)this.WithConditionNumberRadioButton.IsChecked;

            this.WithRepeatPlanning = (bool)this.WithRepeatPlanningCheckBox.IsChecked;
            this.ThresholdForPlanning = this.WithRepeatPlanning ? double.Parse(this.ThresholdForRepeatPlanning.Text) : double.MaxValue;
            this.PathPlanningProgressBar.IsIndeterminate = this.WithRepeatPlanning ? true : false;
            this.PathPlanningProgressBar.Maximum = this.appViewModel.track3D.track.SplitPoints.Count;
            this.PathPlanningProgressBar.Value = 0;

            this.worker.RunWorkerAsync();
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.DeltaChart.Series["DeltaSeries"].Points.Clear();
            this.DeltaChart.Series["DeltaSeries"].Points.DataBindXY(
                Enumerable.Range(0, this.IterationCount).ToArray(),
                this.DeltaList);
            this.DeltaChart.Series["CondSeries"].Points.Clear();
            this.DeltaChart.Series["CondSeries"].Points.DataBindXY(
                Enumerable.Range(0, this.IterationCount).ToArray(),
                this.CondList);
        }
    }
}
