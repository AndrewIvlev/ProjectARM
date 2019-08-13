using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.Graphics.Interfaces
{
    interface IPoint3D
    {
        double X { get; set; }
        double Y { get; set; }
        double Z { get; set; }
        string Coordinates { get; }

        IVector3D DistanceTo(IPoint3D endPoint);
    }
}
