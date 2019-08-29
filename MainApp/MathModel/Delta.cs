using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace ManipulationSystemLibrary
{
    public class Delta
    {
        public List<Point3D> DesiredPoints;
        public List<Point3D> RealPoints;
        public List<double> Deltas;

        public Delta()
        {
            DesiredPoints = new List<Point3D>();
            RealPoints = new List<Point3D>();
            Deltas = new List<double>();
        }

        public void CalcDeltas()
        {
            for(var i = 0; i < RealPoints.Count; i++)
                Deltas.Add((DesiredPoints[i] - RealPoints[i]).Length);
        }
    }
}
