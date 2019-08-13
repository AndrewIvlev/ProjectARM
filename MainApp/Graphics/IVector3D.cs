﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.Graphics
{
    interface IVector3D
    {
        double X { get; set; }
        double Y { get; set; }
        double Z { get; set; }
        double Norm { get; }

        IVector3D Add(IVector3D vector);
        IVector3D Subtract(IVector3D vector);
        IVector3D Multiply(double factor);

        IVector3D CrossProduct(IVector3D vector);
        double DotProduct(IVector3D vector);
    }
}
