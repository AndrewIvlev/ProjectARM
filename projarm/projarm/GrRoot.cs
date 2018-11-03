using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace projarm
{
    abstract class GrRoot
    {
        public abstract void Show(Graphics gr);
        public abstract void Hide(Graphics gr);
        public abstract void Move(Graphics gr);
    }
}
