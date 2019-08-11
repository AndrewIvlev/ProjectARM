﻿using System;
using System.IO;
using ManipulationSystemLibrary;
using ManipulationSystemLibrary.MathModel;
using Newtonsoft.Json;
using NUnit.Framework;

namespace ManipulationSystemLibraryTests
{
    [TestFixture]
    public class AllUnitTests
    {
        public string ManipulatorConfigDirectory;

        [SetUp]
        public void SetUp()
        {
            ManipulatorConfigDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory, "ManipConfigFiles");
        }

        [TestCase("13R-OctopusTentacle.json")]
        [TestCase("3RPR.json")]
        [TestCase("ConfigFromGreenBook.json")]
        [TestCase("PR.json")]
        [TestCase("RRPR.json")]
        [TestCase("RRRR.json")]
        public void CanDeserializeMathModel(string fileName)
        {
            var jsonFilePath = Path.Combine(ManipulatorConfigDirectory, fileName);
            try
            {
                var arm = JsonConvert.DeserializeObject<Arm>(File.ReadAllText(jsonFilePath));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test]
        public void DeserializeMathModelEqual()
        {
            var jsonFilePath = Path.Combine(ManipulatorConfigDirectory, "3RPR.json");

            var armFromJson = JsonConvert.DeserializeObject<Arm>(File.ReadAllText(jsonFilePath));

            #region Actual Arm

            var arm = new Arm(
                new BlockMatrix(new[,] {
                    {-1.0, 0.0, 0.0, 0.0 },
                    { 0.0, 0.0, 1.0, 1.0 },
                    { 0.0, 1.0, 0.0, 0.0}
                }),
                new[]
                {
                    new Unit{
                        Type = 'R',
                        Q = 0,
                        B = new BlockMatrix(new[,]
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
                        B = new BlockMatrix(new[,]
                        {
                            { 0.0, 0.0, 1.0, 2.0 },
                            { 1.0, 0.0, 0.0, 0.0 },
                            { 0.0, 1.0, 0.0, 0.0 }
                        })
                    },
                    new Unit{
                        Type = 'R',
                        Q = 0,
                        B = new BlockMatrix(new[,]
                        {
                            { 1.0, 0.0, 0.0, 0.0 },
                            { 0.0, 1.0, 0.0, 0.0 },
                            { 0.0, 0.0, 1.0, 1.0 }
                        })

                    },
                    new Unit{
                        Type = 'P',
                        Q = 0,
                        B = new BlockMatrix(new[,]
                        {
                            { 0.0, 0.0, 1.0, 0.0 },
                            { 1.0, 0.0, 0.0, 0.0 },
                            { 0.0, 1.0, 0.0, 3.0 }
                        })
                    },
                    new Unit{
                        Type = 'R',
                        Q = 0,
                        B = new BlockMatrix(new[,]
                        {
                            { -1.0, 0.0, 0.0, 0.0 },
                            { 0.0, 0.0, 1.0, 1.0 },
                            { 0.0, 1.0, 0.0, 0.0 }
                        })
                    }
                });

            #endregion

            Assert.IsTrue(arm == armFromJson);
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

            #region Actual Arm

            var model = new Arm(
                new BlockMatrix(new [,] { 
                    {-1.0, 0.0, 0.0, 0.0 },
                    { 0.0, 0.0, 1.0, 1.0 },
                    { 0.0, 1.0, 0.0, 0.0}
                }),
                new []
                {
                    new Unit{
                        Type = 'R',
                        Q = 0,
                        B = new BlockMatrix(new[,]
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
                        B = new BlockMatrix(new[,]
                        {
                            { 0.0, 0.0, 1.0, 2.0 },
                            { 1.0, 0.0, 0.0, 0.0 },
                            { 0.0, 1.0, 0.0, 0.0 }
                        })
                    },
                    new Unit{
                        Type = 'R',
                        Q = 0,
                        B = new BlockMatrix(new[,]
                        {
                            { 1.0, 0.0, 0.0, 0.0 },
                            { 0.0, 1.0, 0.0, 0.0 },
                            { 0.0, 0.0, 1.0, 1.0 }
                        })

                    },
                    new Unit{
                        Type = 'P',
                        Q = 0,
                        B = new BlockMatrix(new[,]
                        {
                            { 0.0, 0.0, 1.0, 0.0 },
                            { 1.0, 0.0, 0.0, 0.0 },
                            { 0.0, 1.0, 0.0, 3.0 }
                        })
                    },
                    new Unit{
                        Type = 'R',
                        Q = 0,
                        B = new BlockMatrix(new[,]
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
            var A = new Matrix(3, 2)
            {
                [0, 0] = 5, [0, 1] = 3,
                [1, 0] = 4, [1, 1] = 6,
                [2, 0] = 2, [2, 1] = 1
            };
            var B = new Matrix(2, 5)
            {
                [0, 0] = 8, [0, 1] = 7, [0, 2] = 1, [0, 3] = 4, [0, 4] = 2,
                [1, 0] = 2, [1, 1] = 1, [1, 2] = 7, [1, 3] = 0, [1, 4] = 5
            };
            var expectedAB = new Matrix(3, 5)
            {
                [0, 0] = 46, [0, 1] = 38, [0, 2] = 26, [0, 3] = 20, [0, 4] = 25,
                [1, 0] = 44, [1, 1] = 34, [1, 2] = 46, [1, 3] = 16, [1, 4] = 38,
                [2, 0] = 18, [2, 1] = 15, [2, 2] = 9,  [2, 3] = 8,  [2, 4] = 9
            };

            var actualAB = A * B;

            Assert.IsTrue(expectedAB == actualAB);
        }
    }
}
