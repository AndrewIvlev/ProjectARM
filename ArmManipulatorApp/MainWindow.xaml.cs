namespace ArmManipulatorApp
{
    using System.Windows;
    using System.Windows.Forms.DataVisualization.Charting;

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
                this.MainGrid,
                this.Viewport3D,
                this.ArmConfigTextBox,
                this.DeltaChart);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // TODO: Move it to ApplicationViewModel.cs
            // Все графики находятся в пределах области построения ChartArea, создадим ее
            this.DeltaChart.ChartAreas.Add(new ChartArea("Default"));

            // Добавим линию, и назначим ее в ранее созданную область "Default"
            this.DeltaChart.Series.Add(new Series("Series1"));
            this.DeltaChart.Series["Series1"].ChartArea = "Default";
            this.DeltaChart.Series["Series1"].ChartType = SeriesChartType.Line;

            // добавим данные линии
            string[] axisXData = { "a", "b", "c" };
            double[] axisYData = { 0.1, 1.5, 1.9 };
            this.DeltaChart.Series["Series1"].Points.DataBindXY(axisXData, axisYData);
        }
    }
}
