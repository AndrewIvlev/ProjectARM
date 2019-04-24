using System;
using System.IO;
using Newtonsoft.Json;
using NUnit.Framework;
using ProjectARM;

namespace ProjectARM_Tests
{
    [TestFixture]
    public class AllUnitTests
    {
        public string ManipConfigDirectory;

        [SetUp]
        public void SetUp()
        {
            //ManipConfigDirectory = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "ManipConfig");
            ManipConfigDirectory = @"D:\repo\ProjectARM\ProjectARM\ManipConfig\";
        }

        //[TestCase("SRRPR.json", 100, 0, 0)]
        [TestCase("SRRPR.json", 99, 1, 0)]
        //[TestCase("SRCRPRPR.json")]
        public void LagrangeMethodToThePointTest(string fileName, double px, double py, double pz)
        {
            var path = Path.Combine(ManipConfigDirectory, fileName);
            var sr = new StreamReader(path);
            var jsonString = sr.ReadToEnd();
            var manipConfig = JsonConvert.DeserializeObject<MatrixMathModel>(jsonString);
            var model = new MatrixMathModel(manipConfig);
            
            model.LagrangeMethodToThePoint(new DPoint(px, py, pz));

            Assert.IsTrue(model.q.Equals(new double[] {0, 1, 0, 1})); //TODO: calc real q for that assert
        }

        [Test]
        public void ManipConfigToJson()
        {
            MatrixMathModel model = new MatrixMathModel(5, new[]
            {
                new unit{type = 'S', len = 0, angle = 0},
                new unit{type = 'R', len = 25, angle = 0},
                new unit{type = 'R', len = 25, angle = 0},
                new unit{type = 'P', len = 30, angle = 0},
                new unit{type = 'R', len = 20, angle = 0}
            });
            var jsonString = JsonConvert.SerializeObject(model);
            string tmp = jsonString;
        }
    }
}
