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
            ManipConfigDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory, "ManipConfig");
        }

        //[TestCase("SRCRPRPR.json")]
        //[TestCase("SRRPR.json", 100, 0, 0)]
        //[TestCase("SRRPR.json", 0, 10, 90)]
        [TestCase("SRPR.json", 0, 4, 96)]
        public void LagrangeMethodToThePointTest(string fileName, double px, double py, double pz)
        {
            var path = Path.Combine(ManipConfigDirectory, fileName);
            var sr = new StreamReader(path);
            var jsonString = sr.ReadToEnd();
            var manipConfig = JsonConvert.DeserializeObject<MatrixMathModel>(jsonString);
            manipConfig.AllAngleToRadianFromDegree();
            var model = new MatrixMathModel(manipConfig);
            
            Console.WriteLine($"Current q = {model.q}");
            model.LagrangeMethodToThePoint(new Vector3D(px, py, pz));
            Console.WriteLine($"After one iteration q = {model.q}");

            Assert.IsTrue(model.q.Equals(new double[] {0, 1, 0, 1})); //TODO: calc real q for that assert
        }

        [Test]
        public void ManipConfigToJson()
        {
            string expectedJSON =
                "{\"q\":[0.0,0.0,0.0,0.0],\"A\":{\"M\":[[1.0,0.0,0.0,0.0],[0.0,1.0,0.0,0.0],[0.0,0.0,1.0,0.0],[0.0,0.0,0.0,1.0]]," +
                "\"rows\":4,\"columns\":4},\"units\":[{\"type\":\"S\",\"len\":0.0,\"angle\":0.0},{\"type\":\"R\",\"len\":25.0,\"angle\":0.0}," +
                "{\"type\":\"R\",\"len\":25.0,\"angle\":0.0},{\"type\":\"P\",\"len\":30.0,\"angle\":0.0},{\"type\":\"R\",\"len\":20.0,\"angle\":0.0}],\"n\":5}";
            MatrixMathModel model = new MatrixMathModel(5, new[]
            {
                new unit{type = 'S', len = 0,  angle = 0},
                new unit{type = 'R', len = 25, angle = 0},
                new unit{type = 'R', len = 25, angle = 0},
                new unit{type = 'P', len = 30, angle = 0},
                new unit{type = 'R', len = 20, angle = 0}
            });
            
            string acturalJSON = JsonConvert.SerializeObject(model); ;
            
            Assert.AreEqual(expectedJSON, acturalJSON);
        }

        [Test]
        public void MatrixMultiplication()
        {
            var A = new Matrix(3, 2)
            {
                [0, 0] = 5, [0, 1] = 3,
                [1, 0] = 4, [1, 1] = 6,
                [2, 0] = 2, [2, 1] = 1,
            };
            var B = new Matrix(2, 5)
            {
                [0, 0] = 8, [0, 1] = 7, [0, 2] = 1, [0, 3] = 4, [0, 4] = 2,
                [1, 0] = 2, [1, 1] = 1, [1, 2] = 7, [1, 3] = 0, [1, 4] = 5,
            };
            var expectedAb = new Matrix(3, 5)
            {
                [0, 0] = 46, [0, 1] = 38, [0, 2] = 26, [0, 3] = 20, [0, 4] = 25,
                [1, 0] = 44, [1, 1] = 34, [1, 2] = 46, [1, 3] = 16, [1, 4] = 38,
                [2, 0] = 18, [2, 1] = 15, [2, 2] = 9,  [2, 3] = 8,  [2, 4] = 9
            };

            var actualAB = A * B;

            Assert.IsTrue(expectedAb == actualAB);
        }

        [Test]
        public void InversMatrix()
        {
            var M = new Matrix(4, 4)
            {
                [0, 0] = 3, [0, 1] = 7, [0, 2] = 2, [0, 3] = 5,
                [1, 0] = 1, [1, 1] = 8, [1, 2] = 4, [1, 3] = 2,
                [2, 0] = 2, [2, 1] = 1, [2, 2] = 9, [2, 3] = 3,
                [3, 0] = 5, [3, 1] = 4, [3, 2] = 7, [3, 3] = 1,
            };
            var expectedInverseM = new Matrix(4, 4)
            {
                [0, 0] = 0.097,  [0, 1] = -0.183, [0, 2] = -0.115, [0, 3] = 0.224,
                [1, 0] = -0.019, [1, 1] = 0.146,  [1, 2] = -0.068, [1, 3] = 0.010,
                [2, 0] = -0.087, [2, 1] = 0.064,  [2, 2] = 0.103,  [2, 3] = -0.002,
                [3, 0] = 0.204,  [3, 1] = -0.120, [3, 2] = 0.123,  [3, 3] = -0.147,
            };

            var actualInveseM = Matrix.Inverse(M);

            //TODO: сделать сравнение матриц с выбором погрешности
            Assert.IsTrue(expectedInverseM == actualInveseM);
        }
    }
}
