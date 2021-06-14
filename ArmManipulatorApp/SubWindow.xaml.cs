using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ArmManipulatorArm.MathModel.Arm;

namespace ArmManipulatorApp
{
    public partial class SubWindow : Window
    {
        public SubWindow(List<double> deltaList, List<double[]> qList, Arm arm)
        {
            InitializeComponent();

            this.WindowState = WindowState.Maximized;
            
            // this.Chart.MouseWheel += chart1_MouseWheel; uncoment if you want zoom the first chart

            var chartArea = new ChartArea("Default");
            chartArea.AxisY.Title = "Погрешность(м)";
            chartArea.AxisY.LabelStyle.Format = "0.000";
            this.Chart.ChartAreas.Add(chartArea);
            this.Chart.Series.Add(new Series("deltaSeries"));
            this.Chart.Series["deltaSeries"].Color = System.Drawing.Color.Black;
            this.Chart.Series["deltaSeries"].ChartArea = "Default";
            this.Chart.Series["deltaSeries"].ChartType = SeriesChartType.Line;
            this.Chart.Series["deltaSeries"].Points.DataBindXY(
                Enumerable.Range(0, deltaList.Count).ToArray(),
                deltaList);

            var qValueChartArea = new ChartArea("funcQArea");
            qValueChartArea.AxisY.Title = "Значение целевой ф-ии Q";
            qValueChartArea.AxisY.LabelStyle.Format = "0.000";
            this.Chart.ChartAreas.Add(qValueChartArea);
            this.Chart.Series.Add(new Series("funcQSeries"));
            this.Chart.Series["funcQSeries"].Color = System.Drawing.Color.Red;
            this.Chart.Series["funcQSeries"].ChartArea = "funcQArea";
            this.Chart.Series["funcQSeries"].ChartType = SeriesChartType.Point;
            var qValuesList = new List<double>();
            qList.ForEach(q => qValuesList.Add(arm.FunctionQ(q)));
            this.Chart.Series["funcQSeries"].Points.DataBindXY(
                Enumerable.Range(0, deltaList.Count).ToArray(), qValuesList);

            var i = 0;
            foreach (var unit in arm.Units)
            {
                var chartAreaQs = new ChartArea($"forQs{i}");
                chartAreaQs.AxisY.Title = $"q{i}";
                chartAreaQs.AxisY.LabelStyle.Format = "0.000";
                this.Chart.ChartAreas.Add(chartAreaQs);
                this.Chart.Series.Add(new Series($"qSeries{i}"));
                this.Chart.Series[$"qSeries{i}"].Color = System.Drawing.Color.Black;
                this.Chart.Series[$"qSeries{i}"].ChartArea = $"forQs{i}";
                this.Chart.Series[$"qSeries{i}"].ChartType = SeriesChartType.Point;
                this.Chart.Series.Add(new Series($"qSeries{i}left"));
                this.Chart.Series[$"qSeries{i}left"].Color = System.Drawing.Color.Blue;
                this.Chart.Series[$"qSeries{i}left"].ChartArea = $"forQs{i}";
                this.Chart.Series[$"qSeries{i}left"].ChartType = SeriesChartType.Line;
                this.Chart.Series.Add(new Series($"qSeries{i}right"));
                this.Chart.Series[$"qSeries{i}right"].Color = System.Drawing.Color.Red;
                this.Chart.Series[$"qSeries{i}right"].ChartArea = $"forQs{i}";
                this.Chart.Series[$"qSeries{i}right"].ChartType = SeriesChartType.Line;
                
                var arrayQ = new List<double>();
                var arrayQleftLimits = new List<double>();
                var arrayQrightLimits = new List<double>();
                foreach (var q in qList)
                {
                    arrayQ.Add(q[i]);
                    arrayQleftLimits.Add(unit.qMin);
                    arrayQrightLimits.Add(unit.qMax);
                }
                this.Chart.Series[$"qSeries{i}"].Points.DataBindXY(
                    Enumerable.Range(0, qList.Count).ToArray(), arrayQ);
                this.Chart.Series[$"qSeries{i}left"].Points.DataBindXY(
                    Enumerable.Range(0, qList.Count).ToArray(), arrayQleftLimits);
                this.Chart.Series[$"qSeries{i}right"].Points.DataBindXY(
                    Enumerable.Range(0, qList.Count).ToArray(), arrayQrightLimits);
                i++;
            }

            this.Chart.ChartAreas.Last().AxisX.Title = "Итерации";

            var h = 0;
            var total = this.Chart.ChartAreas.Count;
            var part = 100 / total;
            foreach (var area in this.Chart.ChartAreas)
            {
                area.Position.Auto = false;
                area.Position.Width = 95;
                area.Position.Height = part;
                area.Position.X = 1;
                area.Position.Y = h;
                h += part;

                area.AxisX.IsMarginVisible = false;
                area.AxisY.IsMarginVisible = false;
                area.AxisX.ScaleView.Zoomable = true;
                area.AxisY.ScaleView.Zoomable = true;
            }
        }

        private void chart1_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            var chart = (Chart)sender;
            var xAxis = chart.ChartAreas[0].AxisX;
            var yAxis = chart.ChartAreas[0].AxisY;

            try
            {
                if (e.Delta < 0) // Scrolled down.
                {
                    xAxis.ScaleView.ZoomReset();
                    yAxis.ScaleView.ZoomReset();
                }
                else if (e.Delta > 0) // Scrolled up.
                {
                    var xMin = xAxis.ScaleView.ViewMinimum;
                    var xMax = xAxis.ScaleView.ViewMaximum;
                    var yMin = yAxis.ScaleView.ViewMinimum;
                    var yMax = yAxis.ScaleView.ViewMaximum;

                    var posXStart = xAxis.PixelPositionToValue(e.Location.X) - (xMax - xMin) / 4;
                    var posXFinish = xAxis.PixelPositionToValue(e.Location.X) + (xMax - xMin) / 4;
                    var posYStart = yAxis.PixelPositionToValue(e.Location.Y) - (yMax - yMin) / 4;
                    var posYFinish = yAxis.PixelPositionToValue(e.Location.Y) + (yMax - yMin) / 4;

                    xAxis.ScaleView.Zoom(posXStart, posXFinish);
                    yAxis.ScaleView.Zoom(posYStart, posYFinish);
                }
            }
            catch { }
        }
    }
}
