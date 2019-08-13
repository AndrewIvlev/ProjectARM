using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.Graphics
{
    interface IModel3D
    {
        string Description { get; set; }
        ICollection<IPoint3D> Points { get; }
        ICollection<ITriangle3D> Triangles { get; }
    }
}
