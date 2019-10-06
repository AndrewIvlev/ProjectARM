using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmManipulatorApp.Graphics3DModel
{
    using System.Windows.Media.Media3D;

    public static class VRConvert
    {
        public static Point3D ConvertFromRealToVirtual(Point3D point, double coeff)
            => new Point3D(point.X * coeff, point.Y * coeff, point.Z * coeff);

        public static Point3D ConvertFromVirtualToReal(Point3D point, double coeff)
            => new Point3D(point.X / coeff, point.Y / coeff, point.Z / coeff);
    }
}
