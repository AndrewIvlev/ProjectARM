using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ProjectARM
{
    interface IGraphics
    {
        void Show(Graphics gr);
        void Hide(Graphics gr);
        void Move(Graphics gr);
    }
}
