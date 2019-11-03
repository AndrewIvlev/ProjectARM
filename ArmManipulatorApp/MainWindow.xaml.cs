namespace ArmManipulatorApp
{
    using System.Collections.Generic;
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
        BackgroundWorker animationWorker;

        private int IterationCount;
        private List<double> DeltaList;
        private List<double> CondList;
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
            this.worker.WorkerSupportsCancellation = true;
            this.worker.WorkerReportsProgress = true;
            this.worker.DoWork += this.worker_DoWork;
            this.worker.ProgressChanged += this.worker_ProgressChanged;
            this.worker.RunWorkerCompleted += this.worker_RunWorkerCompleted;

            this.animationWorker = new BackgroundWorker();
            this.animationWorker.WorkerReportsProgress = true;
            this.animationWorker.DoWork += this.animationWorker_DoWork;
            this.animationWorker.ProgressChanged += this.animationWorker_ProgressChanged;
            this.animationWorker.RunWorkerCompleted += this.animationWorker_RunWorkerCompleted;
            
            this.WithCond = false;
            this.WithRepeatPlanning = false;
            this.ThresholdForPlanning = double.MaxValue;
            
            this.Chart.ChartAreas.Add(new ChartArea("Default"));
            this.Chart.Series.Add(new Series("DeltaSeries"));
            this.Chart.Series["DeltaSeries"].ChartArea = "Default";
            this.Chart.Series["DeltaSeries"].ChartType = SeriesChartType.Line;
            this.Chart.Series.Add(new Series("CondSeries"));
            this.Chart.Series["CondSeries"].ChartArea = "Default";
            this.Chart.Series["CondSeries"].ChartType = SeriesChartType.Line;

            // Add some values for chart display
            int[] axisXData = { 0, 50, 100 };
            double[] axisYData = { 5.3, 1.3, 7.3 };
            this.Chart.Series["DeltaSeries"].Points.DataBindXY(axisXData, axisYData);
        }

        #region Arm movement planning region

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.appViewModel.PlanningMovementAlongTrajectory(
                e,
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
            this.PathPlanningProgressBar.IsIndeterminate = this.WithRepeatPlanning;
            this.PathPlanningProgressBar.Maximum = this.appViewModel.track3D.track.SplitPoints.Count;
            this.PathPlanningProgressBar.Value = 0;

            this.worker.RunWorkerAsync();
        }
        
        private void CancelPlanningTrack_Click(object sender, RoutedEventArgs e)
        {
            this.worker.CancelAsync();
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Chart.Series["DeltaSeries"].Points.Clear();
            this.Chart.Series["DeltaSeries"].Points.DataBindXY(
                Enumerable.Range(0, this.IterationCount).ToArray(),
                this.DeltaList);

            this.Chart.Series["CondSeries"].Points.Clear();
            this.Chart.Series["CondSeries"].Points.DataBindXY(
                Enumerable.Range(0, this.IterationCount).ToArray(),
                this.CondList);
        }

        #endregion

        #region Arm move animation region

        private void BeginAnimation_Click(object sender, RoutedEventArgs e)
        {
            this.SliderAnimation.Maximum = this.IterationCount;

            this.animationWorker.RunWorkerAsync();
        }

        private void PauseAnimation_Click(object sender, RoutedEventArgs e)
        {
            this.animationWorker.CancelAsync();
        }

        private void animationWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.appViewModel.BeginAnimation(e, sender);
        }

        private void animationWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.SliderAnimation.Value = e.ProgressPercentage;
        }

        private void animationWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }

        private void AnimationSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
