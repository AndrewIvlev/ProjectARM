using System.Collections.Generic;
using OxyPlot;

namespace MainApp.ViewModel
{
    public class DeltaPlotViewModel
    {
        public DeltaPlotViewModel()
        {
            Title = "Delta";
            Points = new List<DataPoint>
            {
                new DataPoint(0, 0)
            };
        }

        public string Title { get; private set; }

        public IList<DataPoint> Points { get; private set; }
    }
}
