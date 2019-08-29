namespace ArmManipulatorApp.Common
{
    using System.IO;

    using ManipulationSystemLibrary;
    using ManipulationSystemLibrary.MathModel;

    using Newtonsoft.Json;

    class JsonFileService : IFileService
    {
        public Arm OpenArm(string filename) =>
            JsonConvert.DeserializeObject<Arm>(File.ReadAllText(filename));

        public void SaveArm(string filename, Arm arm) =>
            File.WriteAllText(filename, JsonConvert.SerializeObject(arm));
        
        public Trajectory OpenTrack(string filename) =>
            JsonConvert.DeserializeObject<Trajectory>(File.ReadAllText(filename));

        public void SaveTrack(string filename, Trajectory track) =>
            File.WriteAllText(filename, JsonConvert.SerializeObject(track));
    }
}
