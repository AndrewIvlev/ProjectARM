using System;
using System.IO;
using ArmManipulatorArm.MathModel.Arm;
using ArmManipulatorArm.MathModel.Matrix;
using Newtonsoft.Json;
using NUnit.Framework;

namespace ArmManipulatorApp_Tests
{
    using System.Windows.Media.Media3D;

    [TestFixture]
    public class AllUnitTests
    {
        private string ManipulatorConfigDirectory;
        
        [SetUp]
        public void SetUp()
        {
            ManipulatorConfigDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory, "ManipConfigFiles");
        }

        [TestCase("3RPR.json")]
        [TestCase("PR.json")]
        [TestCase("PRR.json")]
        [TestCase("R.json")]
        [TestCase("RP.json")]
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
                new Matrix(new[,] {
                    { 1.0, 0.0, 0.0, 0.0 },
                    { 0.0, 1.0, 0.0, 0.0 },
                    { 0.0, 0.0, 1.0, 0.1},
                    {0.0, 0.0, 0.0, 1.0 }
                }),
                new[]
                {
                    new Unit{
                        Type = 'R',
                        Q = 0,
                        B = new Matrix(new[,]
                        {
                            { 0.0, 0.0, 1.0, 0.0 },
                            { 1.0, 0.0, 0.0, 0.0 },
                            { 0.0, 1.0, 0.0, 0.4 },
                            {0.0, 0.0, 0.0, 1.0 }
                            })
                    },
                    new Unit
                    {
                        Type = 'R',
                        Q = 0,
                        B = new Matrix(new[,]
                        {
                            { 0.0, 0.0, 1.0, 0.4 },
                            { 1.0, 0.0, 0.0, 0.0 },
                            { 0.0, 1.0, 0.0, 0.0 },
                            {0.0, 0.0, 0.0, 1.0 }
                        })
                    },
                    new Unit{
                        Type = 'R',
                        Q = 0,
                        B = new Matrix(new[,]
                        {
                            { 1.0, 0.0, 0.0, 0.0 },
                            { 0.0, 1.0, 0.0, 0.0 },
                            { 0.0, 0.0, 1.0, 0.2 },
                            {0.0, 0.0, 0.0, 1.0 }
                        })

                    },
                    new Unit{
                        Type = 'P',
                        Q = 0,
                        B = new Matrix(new[,]
                        {
                            { 0.0, 0.0, 1.0, 0.0 },
                            { 1.0, 0.0, 0.0, 0.0 },
                            { 0.0, 1.0, 0.0, 0.2 },
                            {0.0, 0.0, 0.0, 1.0 }
                        })
                    },
                    new Unit{
                        Type = 'R',
                        Q = 0,
                        B = new Matrix(new[,]
                        {
                            { -1.0, 0.0, 0.0, 0.0 },
                            { 0.0, 0.0, 1.0, 0.1 },
                            { 0.0, 1.0, 0.0, 0.0 },
                            {0.0, 0.0, 0.0, 1.0 }
                        })
                    }
                });

            #endregion

            Assert.IsTrue(arm == armFromJson);
        }

        [Test]
        public void ManipulatorConfigurationToJson()
        {
            #region Expected Json string

            const string expectedJson =
                "{" +
                "\"N\":5," +
                "\"RootB\":{" +
                    "\"M\":[" +
                        "[-1.0,0.0,0.0,0.0]," +
                        "[0.0,0.0,1.0,1.0]," +
                        "[0.0,1.0,0.0,0.0]," +
                        "[0.0,0.0,0.0,1.0]" +
                    "]}," +
                "\"Units\":[" +
                    "{\"Type\":\"R\"," +
                    "\"Q\":0.0," +
                    "\"B\":{\"M\":[" +
                        "[-1.0,0.0,0.0,0.0]," +
                        "[0.0,0.0,1.0,0.0]," +
                        "[0.0,1.0,0.0,2.0]," +
                        "[0.0,0.0,0.0,1.0]" +
                    "]}}," + 
                "{\"Type\":\"R\"," +
                    "\"Q\":0.0," +
                    "\"B\":{\"M\":[" +
                        "[0.0,0.0,1.0,2.0]," +
                        "[1.0,0.0,0.0,0.0]," +
                        "[0.0,1.0,0.0,0.0]," +
                        "[0.0,0.0,0.0,1.0]" +
                "]}}," +
                    "{\"Type\":\"R\"," +
                "\"Q\":0.0," +
                    "\"B\":{\"M\":[" +
                        "[1.0,0.0,0.0,0.0]," +
                        "[0.0,1.0,0.0,0.0]," +
                        "[0.0,0.0,1.0,1.0]," +
                        "[0.0,0.0,0.0,1.0]" +
                "]}}," +
                    "{\"Type\":\"P\"," +
                "\"Q\":0.0," +
                "\"B\":{\"M\":[" +
                "[0.0,0.0,1.0,0.0]," +
                "[1.0,0.0,0.0,0.0]," +
                "[0.0,1.0,0.0,3.0]," +
                "[0.0,0.0,0.0,1.0]" + 
                "]}}," +
                    "{\"Type\":\"R\"," +
                "\"Q\":0.0," +
                "\"B\":{\"M\":[" +
                "[-1.0,0.0,0.0,0.0]," +
                "[0.0,0.0,1.0,1.0]," +
                "[0.0,1.0,0.0,0.0]," +
                "[0.0,0.0,0.0,1.0]" + 
                "]}}]}";
            
            #endregion

            #region Actual Arm

            var model = new Arm(
                new Matrix(new [,] { 
                    {-1.0, 0.0, 0.0, 0.0 },
                    { 0.0, 0.0, 1.0, 1.0 },
                    { 0.0, 1.0, 0.0, 0.0},
                    {0.0, 0.0, 0.0, 1.0 }
                }),
                new []
                {
                    new Unit{
                        Type = 'R',
                        Q = 0,
                        B = new Matrix(new[,]
                        {
                            {-1.0, 0.0, 0.0, 0.0 },
                            { 0.0, 0.0, 1.0, 0.0 },
                            { 0.0, 1.0, 0.0, 2.0 },
                            {0.0, 0.0, 0.0, 1.0 }
                            })
                    },
                    new Unit
                    {
                        Type = 'R',
                        Q = 0,
                        B = new Matrix(new[,]
                        {
                            { 0.0, 0.0, 1.0, 2.0 },
                            { 1.0, 0.0, 0.0, 0.0 },
                            { 0.0, 1.0, 0.0, 0.0 },
                            {0.0, 0.0, 0.0, 1.0 }
                        })
                    },
                    new Unit{
                        Type = 'R',
                        Q = 0,
                        B = new Matrix(new[,]
                        {
                            { 1.0, 0.0, 0.0, 0.0 },
                            { 0.0, 1.0, 0.0, 0.0 },
                            { 0.0, 0.0, 1.0, 1.0 },
                            {0.0, 0.0, 0.0, 1.0 }
                        })

                    },
                    new Unit{
                        Type = 'P',
                        Q = 0,
                        B = new Matrix(new[,]
                        {
                            { 0.0, 0.0, 1.0, 0.0 },
                            { 1.0, 0.0, 0.0, 0.0 },
                            { 0.0, 1.0, 0.0, 3.0 },
                            {0.0, 0.0, 0.0, 1.0 }
                        })
                    },
                    new Unit{
                        Type = 'R',
                        Q = 0,
                        B = new Matrix(new[,]
                        {
                            { -1.0, 0.0, 0.0, 0.0 },
                            { 0.0, 0.0, 1.0, 1.0 },
                            { 0.0, 1.0, 0.0, 0.0 },
                            {0.0, 0.0, 0.0, 1.0 }
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

        [Test]
        public void Det3DCalculateCorrect()
        {
            var expectedDet = -1;
            var A = new Matrix(3, 3)
            {
                [0, 0] = 2, [0, 1] = 5, [0, 2] = 7,
                [1, 0] = 6, [1, 1] = 3, [1, 2] = 4,
                [2, 0] = 5, [2, 1] = -2, [2, 2] = -3
            };

            var actualDet = Matrix.Det3D(A);

            Assert.AreEqual(expectedDet, actualDet);
        }

        [Test]
        public void System3x3Solver_worksCorrect()
        {
            var expectedX = new Vector3D(1, 2, 3);
            var A = new Matrix(3, 3)
            {
                [0, 0] = 1, [0, 1] = 0, [0, 2] = 1,
                [1, 0] = 0, [1, 1] = 2, [1, 2] = -1,
                [2, 0] = 3, [2, 1] = -1, [2, 2] = 0
            };

            var b = new Vector3D(4, 1, 1);

            var actualX = Matrix.System3x3Solver(A, b);

            Assert.AreEqual(expectedX, actualX);
        }

        [Test]
        public void System3x3Solver_worksCorrect2()
        {
            var expectedX = new Vector3D(0, -2, 3);
            var A = new Matrix(3, 3)
            {
                [0, 0] = 1, [0, 1] = 2, [0, 2] = 1,
                [1, 0] = -3, [1, 1] = -1, [1, 2] = -1,
                [2, 0] = -2, [2, 1] = 2, [2, 2] = 3
            };

            var b = new Vector3D(-1, -1, 5);

            var actualX = Matrix.System3x3Solver(A, b);

            Assert.AreEqual(expectedX, actualX);
        }

        [Test]
        public void MatrixFrobeniusNorm()
        {
            var expectedNorm = 13;
            var A = new Matrix(3, 3)
                        {
                            [0, 0] = 2, [0, 1] = 3, [0, 2] = 5,
                            [1, 0] = 3, [1, 1] = 1, [1, 2] = 4,
                            [2, 0] = 5, [2, 1] = 4, [2, 2] = 8
                        };

            var actualNorm = A.NormF();

            Assert.AreEqual(expectedNorm, actualNorm, "Frobenius norm fo matrix A calculated incorrectly");
        }

        [Test]
        public void MatrixFrobeniusNorm2()
        {
            var expectedNorm = 12.609520212918492;
            var A = new Matrix(3, 3)
                        {
                            [0, 0] = 1, [0, 1] = 0, [0, 2] = -4,
                            [1, 0] = -2, [1, 1] = 8, [1, 2] = 0,
                            [2, 0] = 3, [2, 1] = 4, [2, 2] = 7
                        };

            var actualNorm = A.NormF();

            Assert.AreEqual(expectedNorm, actualNorm, "Frobenius norm fo matrix A calculated incorrectly");
        }

        [Test]
        public void InvertMatrixIsCorrect()
        {
            var A = new Matrix(3, 3)
            {
                [0, 0] = 2, [0, 1] = 5, [0, 2] = 7,
                [1, 0] = 6, [1, 1] = 3, [1, 2] = 4,
                [2, 0] = 5, [2, 1] = -2, [2, 2] = -3
            };

            var expInvertA = new Matrix(3, 3)
            {
                [0, 0] = 1, [0, 1] = -1, [0, 2] = 1,
                [1, 0] = -38, [1, 1] = 41, [1, 2] = -34,
                [2, 0] = 27, [2, 1] = -29, [2, 2] = 24
            };

            Console.WriteLine("A matrix:");
            A.Print();
            var actualInvertA = A.Invert3D(Matrix.Det3D(A));
            Console.WriteLine("Invert A matrix:");
            actualInvertA.Print();

            var E = new Matrix(3);
            E.ToE();

            var shouldBeE = A * actualInvertA;
            Assert.IsTrue(shouldBeE == E);

            Assert.IsTrue(expInvertA == actualInvertA);
        }

        [Test]
        public void InvertMatrixIsCorrect2()
        {
            var A = new Matrix(3, 3)
                        {
                            [0, 0] = 2, [0, 1] = 2, [0, 2] = 5,
                            [1, 0] = 3, [1, 1] = 1, [1, 2] = 4,
                            [2, 0] = 4, [2, 1] = 1, [2, 2] = 8
                        };

            var expInvertA = new Matrix(3, 3)
            {
                [0, 0] = -4.0 / 13.0, [0, 1] = 11.0 / 13.0, [0, 2] = -3.0 / 13.0,
                [1, 0] = 8.0 / 13.0, [1, 1] = 4.0 / 13.0, [1, 2] = -7.0 / 13,
                [2, 0] = 1.0 / 13.0, [2, 1] = -6.0 / 13, [2, 2] = 4.0 / 13.0
            };

            var actualInvertA = A.Invert3D(Matrix.Det3D(A));
            actualInvertA.Print();

            Assert.IsTrue(expInvertA == actualInvertA);
        }

        [Test]
        [Ignore("TODO: Research why condition number calculation is incorrect")]
        public void ConditionNumberCalculation()
        {
            var expectedCond = 51;
            var A = new Matrix(3, 3)
                        {
                            [0, 0] = 2, [0, 1] = 3, [0, 2] = 5,
                            [1, 0] = 3, [1, 1] = 1, [1, 2] = 4,
                            [2, 0] = 5, [2, 1] = 4, [2, 2] = 8
                        };

            var actualCond = A.ConditionNumber();

            Assert.AreEqual(expectedCond, actualCond, "Condition numbers is not equal");
        }

        [Test]
        public void CalculationTMatrixIsCorrect()
        {
            var fileName = "3RPR.json";
            var jsonFilePath = Path.Combine(ManipulatorConfigDirectory, fileName);
            var arm = JsonConvert.DeserializeObject<Arm>(File.ReadAllText(jsonFilePath));
            arm.Build_S_ForAllUnits_ByUnitsType();

            // Calculating expected result
            var expT = arm.RootB 
                       * arm.S[0] * arm.Units[0].B
                       * arm.S[1] * arm.Units[1].B 
                       * arm.S[2] * arm.Units[2].B
                       * arm.S[3] * arm.Units[3].B 
                       * arm.S[4] * arm.Units[4].B;
            expT.Print();

            // Getting actual result
            arm.Calc_T();
            var actualT = (Matrix)arm.T[arm.N];
            actualT.Print();

            Assert.IsTrue(expT == actualT);
        }

        [Test]
        public void CalculationPreTMatrixIsCorrect()
        {
            var fileName = "3RPR.json";
            var jsonFilePath = Path.Combine(ManipulatorConfigDirectory, fileName);
            var arm = JsonConvert.DeserializeObject<Arm>(File.ReadAllText(jsonFilePath));
            arm.Build_S_ForAllUnits_ByUnitsType();

            // Calculating expected result
            var expT = arm.RootB
                       * arm.S[0] * arm.Units[0].B
                       * arm.S[1] * arm.Units[1].B
                       * arm.S[2] * arm.Units[2].B
                       * arm.S[3] * arm.Units[3].B;
            expT.Print();

            // Getting actual result
            arm.Calc_T();
            var actualT = (Matrix)arm.T[arm.N - 1];
            actualT.Print();

            Assert.IsTrue(expT == actualT);
        }

        [Test]
        public void CalculationPrePreTMatrixIsCorrect()
        {
            var fileName = "3RPR.json";
            var jsonFilePath = Path.Combine(ManipulatorConfigDirectory, fileName);
            var arm = JsonConvert.DeserializeObject<Arm>(File.ReadAllText(jsonFilePath));
            arm.Build_S_ForAllUnits_ByUnitsType();

            // Calculating expected result
            var expT = arm.RootB
                       * arm.S[0] * arm.Units[0].B
                       * arm.S[1] * arm.Units[1].B
                       * arm.S[2] * arm.Units[2].B;
            expT.Print();

            // Getting actual result
            arm.Calc_T();
            var actualT = (Matrix)arm.T[arm.N - 2];
            actualT.Print();

            Assert.IsTrue(expT == actualT);
        }   
        
        [Test]
        public void CalculationPrePrePreTMatrixIsCorrect()
        {
            var fileName = "3RPR.json";
            var jsonFilePath = Path.Combine(ManipulatorConfigDirectory, fileName);
            var arm = JsonConvert.DeserializeObject<Arm>(File.ReadAllText(jsonFilePath));
            arm.Build_S_ForAllUnits_ByUnitsType();

            // Calculating expected result
            var expT = arm.RootB
                       * arm.S[0] * arm.Units[0].B
                       * arm.S[1] * arm.Units[1].B;
            expT.Print();

            // Getting actual result
            arm.Calc_T();
            var actualT = (Matrix)arm.T[arm.N - 3];
            actualT.Print();

            Assert.IsTrue(expT == actualT);
        }
        
        [Test]
        public void CalculationPrePrePrePreTMatrixIsCorrect()
        {
            var fileName = "3RPR.json";
            var jsonFilePath = Path.Combine(ManipulatorConfigDirectory, fileName);
            var arm = JsonConvert.DeserializeObject<Arm>(File.ReadAllText(jsonFilePath));
            arm.Build_S_ForAllUnits_ByUnitsType();

            // Calculating expected result
            var expT = arm.RootB * arm.S[0] * arm.Units[0].B;
            expT.Print();

            // Getting actual result
            arm.Calc_T();
            var actualT = (Matrix)arm.T[arm.N - 4];
            actualT.Print();

            Assert.IsTrue(expT == actualT);
        }

        [TestCase(new []{0.0, 0.0, 0.0, 0.0}, ExpectedResult = new []{40.0, 0.0, 20.0})]
        [TestCase(new []{0.0, 1.57079632679, 0.0, 0.0}, ExpectedResult = new []{ 1.9586355432497926E-10d, 0.0, 60.0})]
        [TestCase(new []{1.5, 1.5, 1.0, -1.4}, ExpectedResult = new []{ -0.53171233269779827, -7.4978988944619385, 53.314837876865518})]
        [TestCase(new []{1.5, 1.5, 3.0, 2.0}, ExpectedResult = new []{ 0.7859011526976365, 11.082322191155701, 48.123079171891767})]
        [TestCase(new []{3.14, -3.14, 3.0, 1.5}, ExpectedResult = new[] { 33.723173129315335, -0.053709378155719502, 29.921253070736867})]
        public double[] ArmCalculationGrabCoordinatesByF(double[] q)
        {
            var fileName = "RRPR.json";
            var jsonFilePath = Path.Combine(ManipulatorConfigDirectory, fileName);
            var arm = JsonConvert.DeserializeObject<Arm>(File.ReadAllText(jsonFilePath));

            arm.SetQ(q);
            arm.Build_S_ForAllUnits_ByUnitsType();
            arm.Calc_T();
            var expectedF = arm.F(arm.N);

            return new[] { expectedF.X, expectedF.Y, expectedF.Z };
        }

        [TestCase(new[] { 0.158270036075493, 0.156193858992778, 12.8400865737088, 0.0155346145872388}, 0.000000000000001)]
        [TestCase(new[] { 3.14, -3.14, 5, 1.52}, 0.00000000000001)]
        public void Calculation_dFdqi_IsCorrect(double[] q, double permissibleError)
        {
            var fileName = "RRPR.json";
            var jsonFilePath = Path.Combine(ManipulatorConfigDirectory, fileName);
            var arm = JsonConvert.DeserializeObject<Arm>(File.ReadAllText(jsonFilePath));
            var len = arm.GetLenAsArray();

            var expected_dFxdq1 = -Math.Sin(q[0]) * ((Math.Cos(q[1]) * Math.Cos(q[3]) + Math.Sin(q[1]) * Math.Sin(q[3])) * len[4] + Math.Cos(q[1]) * (len[2] + q[2] + len[3]));
            var expected_dFxdq2 = -Math.Sin(q[1]) * (Math.Cos(q[0]) * Math.Cos(q[3]) * len[4] + Math.Cos(q[0]) * (len[2] + q[2] + len[3]))+ len[4] * Math.Cos(q[0]) * Math.Cos(q[1]) * Math.Sin(q[3]);
            var expected_dFxdq3 = Math.Cos(q[0]) * Math.Cos(q[1]);
            var expected_dFxdq4 = -len[4] * Math.Cos(q[0]) * Math.Cos(q[1]) * Math.Sin(q[3]) + len[4] * Math.Cos(q[0]) * Math.Sin(q[1]) * Math.Cos(q[3]);

            var expected_dFydq1 = Math.Cos(q[0]) * (len[4] * (Math.Cos(q[1]) * Math.Cos(q[3]) + Math.Sin(q[1]) * Math.Sin(q[3])) + Math.Cos(q[1]) * (len[2] + q[2] + len[3]));
            var expected_dFydq2 = -Math.Sin(q[1]) * (Math.Sin(q[0]) * Math.Cos(q[3]) * len[4] + Math.Sin(q[0]) * (len[2] + q[2] + len[3])) + len[4] * Math.Sin(q[0]) * Math.Cos(q[1]) * Math.Sin(q[3]);
            var expected_dFydq3 = Math.Sin(q[0]) * Math.Cos(q[1]);
            var expected_dFydq4 = -len[4] * Math.Sin(q[0]) * Math.Cos(q[1]) * Math.Sin(q[3]) + len[4] * Math.Sin(q[0]) * Math.Sin(q[1]) * Math.Cos(q[3]);

            var expected_dFzdq1 = 0;
            var expected_dFzdq2 = Math.Cos(q[1]) * (len[4] * Math.Cos(q[3]) + len[2] + len[3] + q[2]) + Math.Sin(q[1]) * Math.Sin(q[3]) * len[4];
            var expected_dFzdq3 = Math.Sin(q[1]);
            var expected_dFzdq4 = -len[4] * (Math.Sin(q[1]) * Math.Sin(q[3]) + Math.Cos(q[1]) * Math.Cos(q[3]));

            arm.SetQ(q);
            arm.Build_S_ForAllUnits_ByUnitsType();
            arm.Build_dS();
            arm.Calc_dT();
            arm.Build_D();
            var dFdq1 = arm.Get_dF(0);
            var dFdq2 = arm.Get_dF(1);
            var dFdq3 = arm.Get_dF(2);
            var dFdq4 = arm.Get_dF(3);

            Assert.IsTrue(Math.Abs(expected_dFxdq1 - dFdq1.X) < permissibleError, $"Big error by dFxdq1 which actual {dFdq1.X} but expected is {expected_dFxdq1}");
            Assert.IsTrue(Math.Abs(expected_dFxdq2 - dFdq2.X) < permissibleError, $"Big error by dFxdq2 which actual {dFdq2.X} but expected is {expected_dFxdq2}");
            Assert.IsTrue(Math.Abs(expected_dFxdq3 - dFdq3.X) < permissibleError, $"Big error by dFxdq3 which actual {dFdq3.X} but expected is {expected_dFxdq3}");
            Assert.IsTrue(Math.Abs(expected_dFxdq4 - dFdq4.X) < permissibleError, $"Big error by dFxdq4 which actual {dFdq4.X} but expected is {expected_dFxdq4}");

            Assert.IsTrue(Math.Abs(expected_dFydq1 - dFdq1.Y) < permissibleError, $"Big error by dFydq1 which actual {dFdq1.Y} but expected is {expected_dFydq1}");
            Assert.IsTrue(Math.Abs(expected_dFydq2 - dFdq2.Y) < permissibleError, $"Big error by dFydq2 which actual {dFdq2.Y} but expected is {expected_dFydq2}");
            Assert.IsTrue(Math.Abs(expected_dFydq3 - dFdq3.Y) < permissibleError, $"Big error by dFydq3 which actual {dFdq3.Y} but expected is {expected_dFydq3}");
            Assert.IsTrue(Math.Abs(expected_dFydq4 - dFdq4.Y) < permissibleError, $"Big error by dFydq4 which actual {dFdq4.Y} but expected is {expected_dFydq4}");

            Assert.IsTrue(Math.Abs(expected_dFzdq1 - dFdq1.Z) < permissibleError, $"Big error by dFzdq1 which actual {dFdq1.Z} but expected is {expected_dFzdq1}");
            Assert.IsTrue(Math.Abs(expected_dFzdq2 - dFdq2.Z) < permissibleError, $"Big error by dFzdq2 which actual {dFdq2.Z} but expected is {expected_dFzdq2}");
            Assert.IsTrue(Math.Abs(expected_dFzdq3 - dFdq3.Z) < permissibleError, $"Big error by dFzdq3 which actual {dFdq3.Z} but expected is {expected_dFzdq3}");
            Assert.IsTrue(Math.Abs(expected_dFzdq4 - dFdq4.Z) < permissibleError, $"Big error by dFzdq4 which actual {dFdq4.Z} but expected is {expected_dFzdq4}");
        }

        [Test]
        public void Calculation_dS_MatricesIsCorrect()
        {
            var fileName = "3RPR.json";
            var jsonFilePath = Path.Combine(ManipulatorConfigDirectory, fileName);
            var arm = JsonConvert.DeserializeObject<Arm>(File.ReadAllText(jsonFilePath));
            arm.Build_S_ForAllUnits_ByUnitsType();

            // Expected dS matrices:
            var dS1 = new Matrix(4, 4) 
                          {
                              [0, 0] = 0, [0, 1] = -1, [0, 2] = 0, [0, 3] = 0,
                              [1, 0] = 1, [1, 1] = 0, [1, 2] = 0, [1, 3] = 0,
                              [2, 0] = 0, [2, 1] = 0, [2, 2] = 0, [2, 3] = 0,
                              [3, 0] = 0, [3, 1] = 0, [3, 2] = 0, [3, 3] = 0
                          };
            var dS2 = dS1;
            var dS3 = dS1;
            var dS4 = new Matrix(4, 4)
                          {
                              [0, 0] = 0, [0, 1] = 0, [0, 2] = 0, [0, 3] = 0,
                              [1, 0] = 0, [1, 1] = 0, [1, 2] = 0, [1, 3] = 0,
                              [2, 0] = 0, [2, 1] = 0, [2, 2] = 0, [2, 3] = 1,
                              [3, 0] = 0, [3, 1] = 0, [3, 2] = 0, [3, 3] = 0
                          };
            var dS5 = dS1;

            // Actual dS matrices:
            arm.Build_dS();
            var actual_dS1 = arm.dS[0];
            var actual_dS2 = arm.dS[1];
            var actual_dS3 = arm.dS[2];
            var actual_dS4 = arm.dS[3];
            var actual_dS5 = arm.dS[4];

            Assert.IsTrue(dS1 == actual_dS1);
            Assert.IsTrue(dS2 == actual_dS2);
            Assert.IsTrue(dS3 == actual_dS3);
            Assert.IsTrue(dS4 == actual_dS4);
            Assert.IsTrue(dS5 == actual_dS5);
        }

        [Test]
        public void CalculationDTMatricesIsCorrect()
        {
            var fileName = "3RPR.json";
            var jsonFilePath = Path.Combine(ManipulatorConfigDirectory, fileName);
            var arm = JsonConvert.DeserializeObject<Arm>(File.ReadAllText(jsonFilePath));
            arm.Build_S_ForAllUnits_ByUnitsType();
            arm.Build_dS();
            
            var exp_dT1 = arm.RootB
                          * arm.dS[0] * arm.Units[0].B
                          * arm.S[1] * arm.Units[1].B
                          * arm.S[2] * arm.Units[2].B
                          * arm.S[3] * arm.Units[3].B
                          * arm.S[4] * arm.Units[4].B;

            var exp_dT2 = arm.RootB
                          * arm.S[0] * arm.Units[0].B
                          * arm.dS[1] * arm.Units[1].B
                          * arm.S[2] * arm.Units[2].B
                          * arm.S[3] * arm.Units[3].B
                          * arm.S[4] * arm.Units[4].B;
            
            var exp_dT3 = arm.RootB
                          * arm.S[0] * arm.Units[0].B
                          * arm.S[1] * arm.Units[1].B
                          * arm.dS[2] * arm.Units[2].B
                          * arm.S[3] * arm.Units[3].B
                          * arm.S[4] * arm.Units[4].B;

            var exp_dT4 = arm.RootB
                          * arm.S[0] * arm.Units[0].B
                          * arm.S[1] * arm.Units[1].B
                          * arm.S[2] * arm.Units[2].B
                          * arm.dS[3] * arm.Units[3].B
                          * arm.S[4] * arm.Units[4].B;
            var exp_dT5 = arm.RootB
                          * arm.S[0] * arm.Units[0].B
                          * arm.S[1] * arm.Units[1].B
                          * arm.S[2] * arm.Units[2].B
                          * arm.S[3] * arm.Units[3].B
                          * arm.dS[4] * arm.Units[4].B;

            arm.Calc_dT();
            var actual_dT1 = (Matrix)arm.dT[0];
            var actual_dT2 = (Matrix)arm.dT[1];
            var actual_dT3 = (Matrix)arm.dT[2];
            var actual_dT4 = (Matrix)arm.dT[3];
            var actual_dT5 = (Matrix)arm.dT[4];

            Assert.IsTrue(exp_dT1 == actual_dT1);
            Assert.IsTrue(exp_dT2 == actual_dT2);
            Assert.IsTrue(exp_dT3 == actual_dT3);
            Assert.IsTrue(exp_dT4 == actual_dT4);
            Assert.IsTrue(exp_dT5 == actual_dT5);
        }

        [Test]
        public void CalculationDMatrixIsCorrect()
        {
            var fileName = "3RPR.json";
            var jsonFilePath = Path.Combine(ManipulatorConfigDirectory, fileName);
            var arm = JsonConvert.DeserializeObject<Arm>(File.ReadAllText(jsonFilePath));
            arm.Build_S_ForAllUnits_ByUnitsType();
            arm.Build_dS();
            arm.Calc_dT();

            var dT1 = (Matrix)arm.dT[0];
            var dT2 = (Matrix)arm.dT[1];
            var dT3 = (Matrix)arm.dT[2];
            var dT4 = (Matrix)arm.dT[3];
            var dT5 = (Matrix)arm.dT[4];
            // D is Matrix of gradients Fx, Fy and Fz
            // ( Fxq1 Fxq2 ... Fxqn )
            // ( Fyq1 Fyq2 ... Fyqn )
            // ( Fzq1 Fzq2 ... Fzqn )
            var expD = new Matrix(3, 5)
                           {
                               [0, 0] = dT1[0, 3], [0, 1] = dT2[0, 3], [0, 2] = dT3[0, 3], [0, 3] = dT4[0, 3], [0, 4] = dT5[0, 3],
                               [1, 0] = dT1[1, 3], [1, 1] = dT2[1, 3], [1, 2] = dT3[1, 3], [1, 3] = dT4[1, 3], [1, 4] = dT5[1, 3],
                               [2, 0] = dT1[2, 3], [2, 1] = dT2[2, 3], [2, 2] = dT3[2, 3], [2, 3] = dT4[2, 3], [2, 4] = dT5[2, 3],
                           };

            arm.Build_D();
            var actualD = arm.D;

            Assert.IsTrue(expD == actualD);
        }

        [TestCase(new[] { 0.0, 0.0, 0.0, 0.0 }, 0)]
        [TestCase(new[] { 1.5, 1.5, 3.0, 2.0 }, 0.000000000001)]
        [TestCase(new[] { 0.0, 1.57079632679, 0.0, 0.0 }, 0.000000000001)]
        [TestCase(new[] { 1.5, 1.5, 1.0, -1.4 }, 0.0000000001)]
        [TestCase(new[] { 3.14, -3.14, 5.0, 1.52 }, 0.0000000001)]
        public void CalculationCMatrixIsCorrect(double[] q, double permissibleError)
        {
            var fileName = "RRPR.json";
            var jsonFilePath = Path.Combine(ManipulatorConfigDirectory, fileName);
            var arm = JsonConvert.DeserializeObject<Arm>(File.ReadAllText(jsonFilePath));
            var len = arm.GetLenAsArray();

            var expected_dFxdq1 = -Math.Sin(q[0]) * ((Math.Cos(q[1]) * Math.Cos(q[3]) + Math.Sin(q[1]) * Math.Sin(q[3])) * len[4] + Math.Cos(q[1]) * (len[2] + q[2] + len[3]));
            var expected_dFxdq2 = -Math.Sin(q[1]) * (Math.Cos(q[0]) * Math.Cos(q[3]) * len[4] + Math.Cos(q[0]) * (len[2] + q[2] + len[3])) + len[4] * Math.Cos(q[0]) * Math.Cos(q[1]) * Math.Sin(q[3]);
            var expected_dFxdq3 = Math.Cos(q[0]) * Math.Cos(q[1]);
            var expected_dFxdq4 = -len[4] * Math.Cos(q[0]) * Math.Cos(q[1]) * Math.Sin(q[3]) + len[4] * Math.Cos(q[0]) * Math.Sin(q[1]) * Math.Cos(q[3]);

            var expected_dFydq1 = Math.Cos(q[0]) * (len[4] * (Math.Cos(q[1]) * Math.Cos(q[3]) + Math.Sin(q[1]) * Math.Sin(q[3])) + Math.Cos(q[1]) * (len[2] + q[2] + len[3]));
            var expected_dFydq2 = -Math.Sin(q[1]) * (Math.Sin(q[0]) * Math.Cos(q[3]) * len[4] + Math.Sin(q[0]) * (len[2] + q[2] + len[3])) + len[4] * Math.Sin(q[0]) * Math.Cos(q[1]) * Math.Sin(q[3]);
            var expected_dFydq3 = Math.Sin(q[0]) * Math.Cos(q[1]);
            var expected_dFydq4 = -len[4] * Math.Sin(q[0]) * Math.Cos(q[1]) * Math.Sin(q[3]) + len[4] * Math.Sin(q[0]) * Math.Sin(q[1]) * Math.Cos(q[3]);

            var expected_dFzdq1 = 0;
            var expected_dFzdq2 = Math.Cos(q[1]) * (len[4] * Math.Cos(q[3]) + len[2] + len[3] + q[2]) + Math.Sin(q[1]) * Math.Sin(q[3]) * len[4];
            var expected_dFzdq3 = Math.Sin(q[1]);
            var expected_dFzdq4 = -len[4] * (Math.Sin(q[1]) * Math.Sin(q[3]) + Math.Cos(q[1]) * Math.Cos(q[3]));

            arm.SetQ(q);
            arm.Build_S_ForAllUnits_ByUnitsType();
            arm.Build_dS();
            arm.Calc_dT();
            arm.Build_D();

            var expC = new Matrix(3, 3)
                       {
                           // C xx
                           [0, 0] = expected_dFxdq1 * expected_dFxdq1 / arm.A[0] + expected_dFxdq2 * expected_dFxdq2 / arm.A[1] + expected_dFxdq3 * expected_dFxdq3 / arm.A[2] + expected_dFxdq4 * expected_dFxdq4 / arm.A[3],
                           // C xy
                           [0, 1] = expected_dFxdq1 * expected_dFydq1 / arm.A[0] + expected_dFxdq2 * expected_dFydq2 / arm.A[1] + expected_dFxdq3 * expected_dFydq3 / arm.A[2] + expected_dFxdq4 * expected_dFydq4 / arm.A[3],
                           // C xz
                           [0, 2] =  expected_dFxdq1 * expected_dFzdq1 / arm.A[0] + expected_dFxdq2 * expected_dFzdq2 / arm.A[1] + expected_dFxdq3 * expected_dFzdq3 / arm.A[2] + expected_dFxdq4 * expected_dFzdq4 / arm.A[3],
                           // C yx
                           [1, 0] =  expected_dFydq1 * expected_dFxdq1 / arm.A[0] + expected_dFydq2 * expected_dFxdq2 / arm.A[1] + expected_dFydq3 * expected_dFxdq3 / arm.A[2] + expected_dFydq4 * expected_dFxdq4 / arm.A[3],
                           // C yy
                           [1, 1] =  expected_dFydq1 * expected_dFydq1 / arm.A[0] + expected_dFydq2 * expected_dFydq2 / arm.A[1] + expected_dFydq3 * expected_dFydq3 / arm.A[2] + expected_dFydq4 * expected_dFydq4 / arm.A[3],
                           // C yz
                           [1, 2] =  expected_dFydq1 * expected_dFzdq1 / arm.A[0] + expected_dFydq2 * expected_dFzdq2 / arm.A[1] + expected_dFydq3 * expected_dFzdq3 / arm.A[2] + expected_dFydq4 * expected_dFzdq4 / arm.A[3],
                           // C zx
                           [2, 0] =  expected_dFxdq1 * expected_dFzdq1 / arm.A[0] + expected_dFxdq2 * expected_dFzdq2 / arm.A[1] + expected_dFxdq3 * expected_dFzdq3 / arm.A[2] + expected_dFxdq4 * expected_dFzdq4 / arm.A[3],
                           // C zy
                           [2, 1] = expected_dFydq1 * expected_dFzdq1 / arm.A[0] + expected_dFydq2 * expected_dFzdq2 / arm.A[1] + expected_dFydq3 * expected_dFzdq3 / arm.A[2] + expected_dFydq4 * expected_dFzdq4 / arm.A[3],
                           // C zz
                           [2, 2] =  expected_dFzdq1* expected_dFzdq1 / arm.A[0] + expected_dFzdq2* expected_dFzdq2 / arm.A[1] + expected_dFzdq3* expected_dFzdq3 / arm.A[2] + expected_dFzdq4* expected_dFzdq4 / arm.A[3]
            };
            Console.WriteLine(@"Expected C:\n");
            expC.Print();

            arm.Calc_C();
            var actualC = arm.C;
            Console.WriteLine(@"Actual C:\n");
            actualC.Print();

            Assert.IsTrue(Matrix.IsEqualWithPermissibleError(expC, actualC, permissibleError));
        }
    }
}
