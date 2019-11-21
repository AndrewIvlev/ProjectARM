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

        public static Vector3D ConvertFromRealToVirtual(Vector3D vector, double coeff)
            => new Vector3D(vector.X * coeff, vector.Y * coeff, vector.Z * coeff);

        public static Vector3D ConvertFromVirtualToReal(Vector3D vector, double coeff)
            => new Vector3D(vector.X / coeff, vector.Y / coeff, vector.Z / coeff);
    }
}
