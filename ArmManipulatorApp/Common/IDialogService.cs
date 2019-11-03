namespace ArmManipulatorApp.Common
{
    public interface IDialogService
    {
        void ShowMessage(string message);
        string FilePath { get; set; }
        bool OpenFileDialog(string directory);
        bool SaveFileDialog();
    }
}
