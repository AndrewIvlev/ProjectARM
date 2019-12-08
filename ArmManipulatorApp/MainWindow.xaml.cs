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
        BackgroundWorker splittingTrackWorker;
        BackgroundWorker planningWorker;
        BackgroundWorker animationWorker;

        private int IterationCount;
        private List<double> bList;
        private List<double> dList;
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
                this.PathLength);
            this.DataContext = this.appViewModel;

            this.planningWorker = new BackgroundWorker();
            this.planningWorker.WorkerSupportsCancellation = true;
            this.planningWorker.WorkerReportsProgress = true;
            this.planningWorker.DoWork += this.PlanningWorkerDoWork;
            this.planningWorker.ProgressChanged += this.PlanningWorkerProgressChanged;
            this.planningWorker.RunWorkerCompleted += this.PlanningWorkerRunPlanningWorkerCompleted;

            this.splittingTrackWorker = new BackgroundWorker();
            this.splittingTrackWorker.WorkerSupportsCancellation = true;
            this.splittingTrackWorker.WorkerReportsProgress = true;
            this.splittingTrackWorker.DoWork += this.SplittingTrackWorkerDoWork;
            this.splittingTrackWorker.ProgressChanged += this.SplittingTrackWorkerProgressChanged;
            this.splittingTrackWorker.RunWorkerCompleted += this.SplittingTrackWorkerRunSplittingTrackWorkerCompleted;

            this.animationWorker = new BackgroundWorker();
            this.animationWorker.WorkerReportsProgress = true;
            this.animationWorker.DoWork += this.animationWorker_DoWork;
            this.animationWorker.ProgressChanged += this.animationWorker_ProgressChanged;
            this.animationWorker.RunWorkerCompleted += this.animationWorker_RunWorkerCompleted;
            
            this.WithCond = false;
            this.WithRepeatPlanning = false;
            this.ThresholdForPlanning = double.MaxValue;
            
            this.Chart.ChartAreas.Add(new ChartArea("Default"));
            this.Chart.Series.Add(new Series("bSeries"));
            this.Chart.Series["bSeries"].ChartArea = "Default";
            this.Chart.Series["bSeries"].ChartType = SeriesChartType.Line;
            this.Chart.Series.Add(new Series("dSeries"));
            this.Chart.Series["dSeries"].ChartArea = "Default";
            this.Chart.Series["dSeries"].ChartType = SeriesChartType.Line;
            this.Chart.Series.Add(new Series("CondSeries"));
            this.Chart.Series["CondSeries"].ChartArea = "Default";
            this.Chart.Series["CondSeries"].ChartType = SeriesChartType.Line;

            // Add some values for chart display
            int[] axisXData = { 0, 50, 100 };
            double[] axisYData = { 5.3, 1.3, 7.3 };
            this.Chart.Series["bSeries"].Points.DataBindXY(axisXData, axisYData);
        }

        #region Splitting trajectory region

        private void SplittingTrackWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            this.appViewModel.SplitTrajectoryCommand(
                e,
                sender,
                this.StepInMeterToSplitTextBox.Text,
                this.NumberOfPointsToSplitTextBox.Text);
        }

        private void SplittingTrackWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.PathSplittingProgressBar.Value = e.ProgressPercentage;
        }

        private void StartSplittingTrack_Click(object sender, RoutedEventArgs e)
        {
            this.WithCond = (bool)this.WithConditionNumberRadioButton.IsChecked;

            this.WithRepeatPlanning = (bool)this.WithRepeatPlanningCheckBox.IsChecked;
            this.ThresholdForPlanning = this.WithRepeatPlanning ? double.Parse(this.ThresholdForRepeatPlanning.Text) : double.MaxValue;
            this.PathSplittingProgressBar.IsIndeterminate = this.WithRepeatPlanning;
            this.PathSplittingProgressBar.Maximum = this.appViewModel.track3D.track.SplitPoints.Count;
            this.PathSplittingProgressBar.Value = 0;

            this.splittingTrackWorker.RunWorkerAsync();
        }

        private void CancelSplittingTrack_Click(object sender, RoutedEventArgs e)
        {
            this.splittingTrackWorker.CancelAsync();
        }

        private void SplittingTrackWorkerRunSplittingTrackWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Chart.Series["bSeries"].Points.Clear();
            this.Chart.Series["bSeries"].Points.DataBindXY(
                Enumerable.Range(0, this.IterationCount).ToArray(),
                this.bList);
        }

        #endregion

        #region Arm movement planning region

        private void PlanningWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            this.appViewModel.PlanningMovementAlongTrajectory(
                e,
                sender,
                this.WithCond,
                this.WithRepeatPlanning,
                this.ThresholdForPlanning,
                out this.IterationCount,
                out this.bList,
                out this.dList,
                out this.CondList);
        }

        private void PlanningWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
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

            this.planningWorker.RunWorkerAsync();
        }
        
        private void CancelPlanningTrack_Click(object sender, RoutedEventArgs e)
        {
            this.planningWorker.CancelAsync();
        }

        private void PlanningWorkerRunPlanningWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Chart.Series["bSeries"].Points.Clear();
            this.Chart.Series["bSeries"].Points.DataBindXY(
                Enumerable.Range(0, this.IterationCount).ToArray(),
                this.bList);

            //this.Chart.Series["dSeries"].Points.Clear();
            //this.Chart.Series["dSeries"].Points.DataBindXY(
            //    Enumerable.Range(0, this.IterationCount).ToArray(),
            //    this.dList);

            //this.Chart.Series["CondSeries"].Points.Clear();
            //this.Chart.Series["CondSeries"].Points.DataBindXY(
            //    Enumerable.Range(0, this.IterationCount).ToArray(),
            //    this.CondList);
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
