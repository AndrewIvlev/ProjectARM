using System;
using System.IO;
using System.Windows.Media.Media3D;
using ManipulationSystemLibrary;
using Newtonsoft.Json;
using NUnit.Framework;

namespace ProjectARM_Tests
{
    [TestFixture]
    public class AllUnitTests
    {
        public string ManipConfigDirectory;
        private readonly double error = 0.01;

        [SetUp]
        public void SetUp()
        {
            ManipConfigDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory, "ManipConfigFiles");
        }
        
        [TestCase("SRRPR.json", 42, 1, 1)]
        public void MatrixMM_LagrangeMethodToThePointTest(string fileName, double pX, double pY, double pZ)
        {
            var filePath = Path.Combine(ManipConfigDirectory, fileName);
            var sr = new StreamReader(filePath);
            var jsonStringMatrixMathModel = sr.ReadToEnd();
            var manipConfig = JsonConvert.DeserializeObject<MatrixMathModel>(jsonStringMatrixMathModel);
            var model = new MatrixMathModel(manipConfig);
            
            Console.WriteLine($"Current q = {model.q}");
            model.DefaultA();
            model.CalculationMetaData();
            model.LagrangeMethodToThePoint(new Point3D(pX, pY, pZ));
            Console.WriteLine($"After one iteration q = {model.q}");

            var Fq = model.F(model.n - 1);
            Assert.True(Fq.X < pX + error && Fq.X > pX - error);
            Assert.True(Fq.Y < pY + error && Fq.Y > pY - error);
            Assert.True(Fq.Z < pZ + error && Fq.Z > pZ - error);
        }

        [TestCase("SRRPR.json", 1, 99, 0)]
        public void ExplicitMM_LagrangeMethodToThePointTest(string fileName, double px, double py, double pz)
        {
            var path = Path.Combine(ManipConfigDirectory, fileName);
            var sr = new StreamReader(path);
            var jsonString = sr.ReadToEnd();
            var manipConfig = JsonConvert.DeserializeObject<ExplicitMathModel>(jsonString);
            manipConfig.AllAngleToRadianFromDegree();
            var model = new ExplicitMathModel(manipConfig);

            Console.WriteLine($"Current q = {model.q}");
            model.DefaultA();
            model.LagrangeMethodToThePoint(new Point3D(px, py, pz));
            Console.WriteLine($"After one iteration q = {model.q}");

            Assert.IsTrue(model.q.Equals(new double[] { 0, 1, 0, 1 })); //TODO: calc real q for that assert
        }

        [Test]
        public void ManipConfigToJson()
        {
            #region Expected Json string

            const string expectedJson = 
                "{\"q\":[0.0,0.0,0.0,0.0],\"units\":[{\"type\":\"S\",\"len\":0.0,\"angle\":0.0," +
                "\"B\":{\"M\":[[0.0,0.0,1.0,0.0],[1.0,0.0,0.0,0.0],[0.0,1.0,0.0,0.0]],\"rows\":" +
                "0,\"columns\":0}},{\"type\":\"R\",\"len\":13.0,\"angle\":0.0,\"B\":{\"M\":[[1." +
                "0,0.0,0.0,13.0],[0.0,0.0,-1.0,0.0],[0.0,1.0,0.0,0.0]],\"rows\":0,\"columns\":0" +
                "}},{\"type\":\"C\",\"len\":16.0,\"angle\":0.0,\"B\":{\"M\":[[0.0,0.0,1.0,16.0]" +
                ",[0.0,1.0,0.0,0.0],[-1.0,0.0,0.0,0.0]],\"rows\":0,\"columns\":0}},{\"type\":\"" +
                "P\",\"len\":4.0,\"angle\":0.0,\"B\":{\"M\":[[-1.0,0.0,0.0,0.0],[0.0,0.0,1.0,0." +
                "0],[0.0,1.0,0.0,4.0]],\"rows\":0,\"columns\":0}},{\"type\":\"R\",\"len\":11.0," +
                "\"angle\":0.0,\"B\":{\"M\":[[-1.0,0.0,0.0,0.0],[0.0,0.0,1.0,11.0],[0.0,1.0,0.0" +
                ",0.0]],\"rows\":0,\"columns\":0}}],\"n\":5}";

            #endregion

            #region Actual Matrix Math Model

            MatrixMathModel model = new MatrixMathModel(5, new[]
            {
                new unit{type = 'S', len = 0,  angle = 0, B = new BlockMatrix(
                    new double[,]{
                        {0, 0, 1, 0},
                        {1, 0, 0, 0},
                        {0, 1, 0, 0}
                    })
                },
                new unit{type = 'R', len = 13, angle = 0, B = new BlockMatrix(
                    new double[,]{
                        {1, 0, 0, 13},
                        {0, 0, -1, 0},
                        {0, 1, 0, 0}
                    })
                },
                new unit{type = 'C', len = 16, angle = 0, B = new BlockMatrix(
                    new double[,]{
                        {0, 0, 1, 16},
                        {0, 1, 0, 0},
                        {-1, 0, 0, 0}
                    })
                },
                new unit{type = 'P', len = 4, angle = 0, B = new BlockMatrix(
                    new double[,]{
                        {-1, 0, 0, 0},
                        {0, 0, 1, 0},
                        {0, 1, 0, 4}
                    })
                },
                new unit{type = 'R', len = 11, angle = 0, B = new BlockMatrix(
                    new double[,]{
                        {-1, 0, 0, 0},
                        {0, 0, 1, 11},
                        {0, 1, 0, 0}
                    })
                }
            });

            #endregion

            var acturalJson = JsonConvert.SerializeObject(model);
            
            Assert.AreEqual(expectedJson, acturalJson);
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
    }
}
