using ArmManipulatorApp.MathModel.Trajectory;
using ArmManipulatorArm.MathModel.Arm;

namespace ArmManipulatorApp.Common
{
    public interface IFileService
    {
        Arm OpenArm(string filename);
        Trajectory OpenTrack(string filename);

        void SaveArm(string filename, Arm arm);
        void SaveTrack(string filename, Trajectory arm);
    }
}
