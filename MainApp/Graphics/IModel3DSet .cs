using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.Graphics
{
    interface IModel3DSet
    {
        string Description { get; set; }
        ICollection<IModel3D> Models { get; }
    }
}
