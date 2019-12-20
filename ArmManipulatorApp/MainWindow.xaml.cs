namespace ArmManipulatorApp
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Forms.DataVisualization.Charting;
    using System.Windows.Media.Animation;

    using ArmManipulatorApp.Common;
    using ArmManipulatorApp.ViewModel;

    // TODO: refactor by MVVM pattern

    public partial class MainWindow : Window
    {
        BackgroundWorker splittingTrackWorker;
        BackgroundWorker planningWorker;
        BackgroundWorker animationWorker;

        private int IterationCount;
        private List<double> bList;
        private List<double> dList;
        private List<double> deltaList;
        private List<double> CondList;
        private List<double> DistanceBetweenSplitPoints;
        private double ThresholdForPlanning;
        private string StepInMeterToSplit;
        private string NumberOfPointsToSplit;
        private bool WithRepeatPlanning;
        private bool WithCond;
        private bool SplitByStep;

        Stopwatch timePlanning;

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
            this.animationWorker.WorkerSupportsCancellation = true;
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

            this.Chart.Series.Add(new Series("deltaSeries"));
            this.Chart.Series["deltaSeries"].ChartArea = "Default";
            this.Chart.Series["deltaSeries"].ChartType = SeriesChartType.Line;

            this.Chart.Series.Add(new Series("CondSeries"));
            this.Chart.Series["CondSeries"].ChartArea = "Default";
            this.Chart.Series["CondSeries"].ChartType = SeriesChartType.Line;

            this.Chart.Series.Add(new Series("SplitPointsDistance"));
            this.Chart.Series["SplitPointsDistance"].ChartArea = "Default";
            this.Chart.Series["SplitPointsDistance"].ChartType = SeriesChartType.Line;

            // Add some values for chart display
            //int[] axisXData = { 0, 50, 100 };
            //double[] axisYData = { 5.3, 1.3, 7.3 };
            //this.Chart.Series["bSeries"].Points.DataBindXY(axisXData, axisYData);
        }

        #region Splitting trajectory region

        private void SplittingTrackWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            if (this.SplitByStep)
            {
                this.appViewModel.SplitTrajectory(e, sender, double.Parse(this.StepInMeterToSplit), out this.DistanceBetweenSplitPoints, out this.IterationCount);
            }
            else
            {
                this.appViewModel.SplitTrajectory(e, sender, int.Parse(this.NumberOfPointsToSplit));
            }
        }

        private void SplittingTrackWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.PathSplittingProgressBar.Value = e.ProgressPercentage;
        }

        private void StartSplittingTrack_Click(object sender, RoutedEventArgs e)
        {
            this.StepInMeterToSplit = this.StepInMeterToSplitTextBox.Text;
            this.NumberOfPointsToSplit = this.NumberOfPointsToSplitTextBox.Text;
            
            if (this.StepInMeterToSplit != string.Empty && this.NumberOfPointsToSplit != string.Empty)
            {
                MessageBox.Show("Нужно выбрать только один вариант разбиения.");
            }
            else if (this.StepInMeterToSplit != string.Empty)
            {
                if (double.TryParse(this.StepInMeterToSplit, out var step))
                {
                    this.SplitByStep = true;

                    this.PathSplittingProgressBar.IsIndeterminate = true;
                    this.PathSplittingProgressBar.Maximum = 1;
                    this.PathSplittingProgressBar.Value = 0;

                    this.splittingTrackWorker.RunWorkerAsync();
                }
                else
                {
                    MessageBox.Show("Некорректный ввод шага разбиения!");
                }
            }
            else if (int.TryParse(this.NumberOfPointsToSplit, out var numberOfSplitPoints))
            {
                this.SplitByStep = false;

                this.PathSplittingProgressBar.Maximum = numberOfSplitPoints;
                this.PathSplittingProgressBar.Value = 0;

                this.splittingTrackWorker.RunWorkerAsync();
            }
            else
            {
                MessageBox.Show("Некорректный ввод шага разбиения!");
            }
        }

        private void CancelSplittingTrack_Click(object sender, RoutedEventArgs e)
        {
            this.splittingTrackWorker.CancelAsync();
        }

        private void SplittingTrackWorkerRunSplittingTrackWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.PathSplittingProgressBar.IsIndeterminate = false;

            this.Chart.Series["SplitPointsDistance"].Points.Clear();
            this.Chart.Series["SplitPointsDistance"].Points.DataBindXY(
                Enumerable.Range(0, this.IterationCount - 1).ToArray(),
                this.DistanceBetweenSplitPoints);

            if (this.appViewModel.SplitTrackWithInterpolation)
            {
                this.appViewModel.ShowSplitTrack();
            }
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
                out this.deltaList,
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
               
            this.timePlanning = Stopwatch.StartNew();
            this.planningWorker.RunWorkerAsync();
        }
        
        private void CancelPlanningTrack_Click(object sender, RoutedEventArgs e)
        {
            this.planningWorker.CancelAsync();
            this.timePlanning.Stop();
            this.WorkingTime.Content = $"Время работы алгоритма = {this.timePlanning.ElapsedMilliseconds * 60} сек.";
        }

        private void PlanningWorkerRunPlanningWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Chart.Series["SplitPointsDistance"].Points.Clear();
         
            //    this.Chart.Series["bSeries"].Points.Clear();
            //    this.Chart.Series["bSeries"].Points.DataBindXY(
            //        Enumerable.Range(0, this.IterationCount).ToArray(),
            //        this.bList);

            //    this.Chart.Series["dSeries"].Points.Clear();
            //    this.Chart.Series["dSeries"].Points.DataBindXY(
            //        Enumerable.Range(0, this.IterationCount).ToArray(),
            //        this.dList);

            this.Chart.Series["deltaSeries"].Points.Clear();
            this.Chart.Series["deltaSeries"].Points.DataBindXY(
                Enumerable.Range(0, this.IterationCount).ToArray(),
                this.deltaList);

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
            this.appViewModel.BeginAnimation(e, sender, this.Viewport3D);
        }

        private void animationWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.SliderAnimation.Value = e.ProgressPercentage;
            //this.Viewport3D.UpdateLayout();
        }

        private void animationWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }

        private void AnimationSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        }

        #endregion
    }
}
