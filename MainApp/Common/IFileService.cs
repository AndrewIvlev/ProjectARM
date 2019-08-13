namespace MainApp.Common
{
    using ManipulationSystemLibrary.MathModel;
 
    public interface IFileService
    {
        Arm Open(string filename);
        void Save(string filename, Arm arm);
    }
}
