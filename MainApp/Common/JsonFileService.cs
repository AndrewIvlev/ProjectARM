namespace MainApp.Common
{
    using System.IO;

    using ManipulationSystemLibrary.MathModel;

    using Newtonsoft.Json;

    class JsonFileService : IFileService
    {
        public Arm Open(string filename) =>
            JsonConvert.DeserializeObject<Arm>(File.ReadAllText(filename));

        public void Save(string filename, Arm arm) =>
            File.WriteAllText(filename, JsonConvert.SerializeObject(arm));
    }
}
