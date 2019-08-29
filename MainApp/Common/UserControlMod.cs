using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.Common
{
    public enum UserMod
    {
        CameraRotationMod,
        TrajectoryAnchorPointCreationMod,
        TrajectoryAnchorPointUpAndDownMod,
        TrajectoryEditingMod
    }

    public static class UserControlMod
    {
        public static UserMod mod;
    }
}
