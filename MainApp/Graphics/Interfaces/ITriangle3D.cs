using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.Graphics.Interfaces
{
    interface ITriangle3D
    {
        IModel3D Model3D { get; }
        IPoint3D Point1 { get; set; }
        IPoint3D Point2 { get; set; }
        IPoint3D Point3 { get; set; }
    }
}
