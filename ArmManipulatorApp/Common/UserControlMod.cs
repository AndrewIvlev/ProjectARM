using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainApp.Common
{
    public enum UserMod
    {
        CameraRotation,
        TrajectoryAnchorPointCreation,
        TrajectoryAnchorPointUpAndDown,
        TrajectoryEditing
    }

    public static class UserControlMod
    {
        public static UserMod Mod;
    }
}
