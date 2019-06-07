using System;
using System.IO;
using System.Windows.Media.Media3D;
using ManipulationSystemLibrary.MathModel;
using ManipulationSystemLibrary.Matrix;
using Newtonsoft.Json;
using NUnit.Framework;

namespace ManipulationSystemLibraryTests
{
    [TestFixture]
    public class AllUnitTests
    {
        public string ManipulatorConfigDirectory;
        private readonly double error = 0.01;

        [SetUp]
        public void SetUp()
        {
            ManipulatorConfigDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory, "ManipConfigFiles");
        }
        
        [TestCase("RRPR.json", 15, 4, 1)]
        public void MatrixMM_LagrangeMethodToThePointTest(string fileName, double pX, double pY, double pZ)
        {
            var filePath = Path.Combine(ManipulatorConfigDirectory, fileName);
            var sr = new StreamReader(filePath);
            var jsonStringMatrixMathModel = sr.ReadToEnd();
            var mathModel = JsonConvert.DeserializeObject<MathModel>(jsonStringMatrixMathModel);
            var model = new MatrixMathModel(mathModel);
            model.DefaultA();
            model.CalculationMetaData();
            model.LagrangeMethodToThePoint(new Point3D(pX, pY, pZ));

            var fq = model.F( - 1);
            Assert.True(fq.X < pX + error && fq.X > pX - error);
            Assert.True(fq.Y < pY + error && fq.Y > pY - error);
            Assert.True(fq.Z < pZ + error && fq.Z > pZ - error);
        }

        [TestCase("RRPR.json", 1, 99, 0)]
        public void ExplicitMM_LagrangeMethodToThePointTest(string fileName, double px, double py, double pz)
        {
            var path = Path.Combine(ManipulatorConfigDirectory, fileName);
            var sr = new StreamReader(path);
            var jsonString = sr.ReadToEnd();
            var model = JsonConvert.DeserializeObject<ExplicitMathModel>(jsonString);
            //manipulatorConfig.AllAngleToRadianFromDegree();
            
            model.DefaultA();
            model.LagrangeMethodToThePoint(new Point3D(px, py, pz));

            Assert.IsTrue(true);
        }

        [Test]
        public void ManipulatorConfigToJson()
        {
            #region Expected Json string

            const string expectedJson =
                "{" +
                "\"N\":5," +
                "\"RootB\":{" +
                    "\"M\":[" +
                        "[-1.0,0.0,0.0,0.0]," +
                        "[0.0,0.0,1.0,1.0]," +
                        "[0.0,1.0,0.0,0.0]" +
                    "]}," +
                "\"Units\":[" +
                    "{\"Type\":\"R\"," +
                    "\"Q\":0.0," +
                    "\"B\":{\"M\":[" +
                        "[-1.0,0.0,0.0,0.0]," +
                        "[0.0,0.0,1.0,0.0]," +
                        "[0.0,1.0,0.0,2.0]]}}," +
                    "{\"Type\":\"R\"," +
                    "\"Q\":0.0," +
                    "\"B\":{\"M\":[" +
                        "[0.0,0.0,1.0,2.0]," +
                        "[1.0,0.0,0.0,0.0]," +
                        "[0.0,1.0,0.0,0.0]]}}," +
                    "{\"Type\":\"R\"," +
                "\"Q\":0.0," +
                    "\"B\":{\"M\":[" +
                        "[1.0,0.0,0.0,0.0]," +
                        "[0.0,1.0,0.0,0.0]," +
                        "[0.0,0.0,1.0,1.0]]}}," +
                    "{\"Type\":\"P\"," +
                "\"Q\":0.0," +
                "\"B\":{\"M\":[" +
                "[0.0,0.0,1.0,0.0]," +
                "[1.0,0.0,0.0,0.0]," +
                "[0.0,1.0,0.0,3.0]]}}," +
                    "{\"Type\":\"R\"," +
                "\"Q\":0.0," +
                "\"B\":{\"M\":[" +
                "[-1.0,0.0,0.0,0.0]," +
                "[0.0,0.0,1.0,1.0]," +
                "[0.0,1.0,0.0,0.0]]}}" +
                "]}";
            
            #endregion

            #region Actual Matrix Math Model

            var model = new MathModel(
                5,
                new BlockMatrix(new double[,] { 
                    {-1.0, 0.0, 0.0, 0.0 },
                    { 0.0, 0.0, 1.0, 1.0 },
                    { 0.0, 1.0, 0.0, 0.0}
                }),
                new []
                {
                    new Unit{
                        Type = 'R',
                        Q = 0,
                        B = new BlockMatrix(new double[,]
                        {
                            {-1.0, 0.0, 0.0, 0.0 },
                            { 0.0, 0.0, 1.0, 0.0 },
                            { 0.0, 1.0, 0.0, 2.0 }
                            })
                    },
                    new Unit
                    {
                        Type = 'R',
                        Q = 0,
                        B = new BlockMatrix(new double[,]
                        {
                            { 0.0, 0.0, 1.0, 2.0 },
                            { 1.0, 0.0, 0.0, 0.0 },
                            { 0.0, 1.0, 0.0, 0.0 }
                        })
                    },
                    new Unit{
                        Type = 'R',
                        Q = 0,
                        B = new BlockMatrix(new double[,]
                        {
                            { 1.0, 0.0, 0.0, 0.0 },
                            { 0.0, 1.0, 0.0, 0.0 },
                            { 0.0, 0.0, 1.0, 1.0 }
                        })

                    },
                    new Unit{
                        Type = 'P',
                        Q = 0,
                        B = new BlockMatrix(new double[,]
                        {
                            { 0.0, 0.0, 1.0, 0.0 },
                            { 1.0, 0.0, 0.0, 0.0 },
                            { 0.0, 1.0, 0.0, 3.0 }
                        })
                    },
                    new Unit{
                        Type = 'R',
                        Q = 0,
                        B = new BlockMatrix(new double[,]
                        {
                            { -1.0, 0.0, 0.0, 0.0 },
                            { 0.0, 0.0, 1.0, 1.0 },
                            { 0.0, 1.0, 0.0, 0.0 }
                        })
                    }
                });

            #endregion

            var actualJson = JsonConvert.SerializeObject(model);
            
            Assert.AreEqual(expectedJson, actualJson);
        }

        [Test]
        public void MatrixMultiplication()
        {
            var a = new Matrix(3, 2)
            {
                [0, 0] = 5, [0, 1] = 3,
                [1, 0] = 4, [1, 1] = 6,
                [2, 0] = 2, [2, 1] = 1
            };
            var b = new Matrix(2, 5)
            {
                [0, 0] = 8, [0, 1] = 7, [0, 2] = 1, [0, 3] = 4, [0, 4] = 2,
                [1, 0] = 2, [1, 1] = 1, [1, 2] = 7, [1, 3] = 0, [1, 4] = 5
            };
            var expectedAb = new Matrix(3, 5)
            {
                [0, 0] = 46, [0, 1] = 38, [0, 2] = 26, [0, 3] = 20, [0, 4] = 25,
                [1, 0] = 44, [1, 1] = 34, [1, 2] = 46, [1, 3] = 16, [1, 4] = 38,
                [2, 0] = 18, [2, 1] = 15, [2, 2] = 9,  [2, 3] = 8,  [2, 4] = 9
            };

            var actualMultiplicationAonB = a * b;

            Assert.IsTrue(expectedAb == actualMultiplicationAonB);
        }
    }
}
