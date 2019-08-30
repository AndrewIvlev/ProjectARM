using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmManipulatorApp.Graphics3DModel.Model3D
{
    using System.Windows.Media.Media3D;

    using ArmManipulatorApp.Common;

    using SystemPoint3D = System.Windows.Media.Media3D.Point3D;

    public class CameraModel3D : Notifier
    {
        private PerspectiveCamera camera;

        public CameraModel3D(SystemPoint3D center)
        {
            this.camera = new PerspectiveCamera();
            camera.Position = center;
        }

        public void Move()
        {

        }

        public void Zoom()
        {

        }
    }
}
