namespace MainApp.Common
{
    using ManipulationSystemLibrary;
    using ManipulationSystemLibrary.MathModel;
 
    public interface IFileService
    {
        Arm OpenArm(string filename);
        Trajectory OpenTrack(string filename);

        void SaveArm(string filename, Arm arm);
        void SaveTrack(string filename, Trajectory arm);
    }
}
