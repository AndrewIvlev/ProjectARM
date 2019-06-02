namespace ManipApp
{
    using System.Collections.Generic;

    using OxyPlot;

    public class DeltaPlotViewModel
    {
        public DeltaPlotViewModel()
        {
            this.Title = "Delta";
            this.Points = new List<DataPoint>
            {
                new DataPoint(0, 0)
            };
        }

        public string Title { get; private set; }

        public IList<DataPoint> Points { get; private set; }
    }
}
