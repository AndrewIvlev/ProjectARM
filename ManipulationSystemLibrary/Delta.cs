using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace ManipulationSystemLibrary
{
    public class Delta
    {
        public List<Point3D> DesiredPoints;
        public List<Point3D> RealPoints;
        public List<double> deltas;

        public Delta()
        {
            DesiredPoints = new List<Point3D>();
            RealPoints = new List<Point3D>();
            deltas = new List<double>();
        }

        public void CalcDeltas()
        {
            for(int i = 0; i < RealPoints.Count; i++)
                deltas.Add((DesiredPoints[i] - RealPoints[i]).Length);
        }
    }
}
