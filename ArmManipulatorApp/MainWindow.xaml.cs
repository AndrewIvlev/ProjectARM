﻿namespace ArmManipulatorApp
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
            //this.WindowState = WindowState.Maximized;
            this.appViewModel = new ApplicationViewModel(
                new DefaultDialogService(),
                new JsonFileService(),
                this.Viewport3D,
                this.ArmConfigTextBox,
                this.VectorQTextBox,
                //this.PathLength,
                this.SplitStepPathLabel,
                this.IterationCountLabel,
                this.SumOfRepeatedIterationCountLabel,
                this.SumOfLambdaRecalculationCountLabel,
                this.AverageDeltaLabel,
                this.Chart,
                this.WithConditionNumberCheckBox,
                this.WithBalancingCheckBox,
                this.WithInterpolationCheckBox,
                this.LagrangeMethodRadioButton,
                this.LagrangeMethodWithProjectionRadioButton,
                this.ActiveSetMethodRadioButton,
                this.WithoutRepeatPlanningRadioButton,
                this.WithRepeatPlanningByThresholdRadioButton,
                this.WithRepeatPlanningByNumberTimesRadioButton,
                this.PathSplittingProgressBar,
                this.PathPlanningProgressBar,
                this.StepInMeterToSplitTextBox,
                this.NumberOfPointsToSplitTextBox,
                this.RepeatNumberTimesPlanningTextBox,
                this.ThresholdForRepeatPlanning,
                this.ThresholdForBalancing,
                this.WorkingTime,
                this.SliderAnimation,
                this.OpenQVectorsWindowsButton);

            this.DataContext = this.appViewModel;
        }
    }
}
