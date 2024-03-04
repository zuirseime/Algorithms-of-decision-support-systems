using Lab1.Core;

namespace Lab1.Tests;

[TestClass]
public class LinearSystemTests {
    [TestMethod]
    public void LinearSystemTest1() {
        double[,] coeffitients = {
            { 5, -3, 7 },
            { -1, 4, 3 },
            { 6, -2, 5 }
        };
        double[,] constants = {
            { 13 },
            { 13 },
            { 12 }
        };
        double[,] expectedResult = {
            { 1 },
            { 2 },
            { 2 },
        };

        string protocol = string.Empty;
        Matrix result = Matrix.SolveLinearSystem(coeffitients, constants, ref protocol);
        for (int i = 0; i < result.Data.GetLength(0); i++) {
            for (int j = 0; j < result.Data.GetLength(1); j++) {
                result.Data[i, j] = Math.Round(result.Data[i, j]);
            }
        }

        Console.WriteLine($"{result}\n{protocol}");
        CollectionAssert.AreEqual(expectedResult, result.Data);
    }

    [TestMethod]
    public void LinearSystemTest2() {
        double[,] coeffitients = {
            { 6, 2, 5 },
            { -3, 4, -1 },
            { 1, 4, 3 }
        };
        double[,] constants = {
            { 1 },
            { 6 },
            { 6 }
        };
        double[,] expectedResult = {
            { -1 },
            { 1 },
            { 1 }
        };

        string protocol = string.Empty;
        Matrix result = Matrix.SolveLinearSystem(coeffitients, constants, ref protocol);
        for (int i = 0; i < result.Data.GetLength(0); i++) {
            for (int j = 0; j < result.Data.GetLength(1); j++) {
                result.Data[i, j] = Math.Round(result.Data[i, j]);
            }
        }

        Console.WriteLine($"{result}\n{protocol}");
        CollectionAssert.AreEqual(expectedResult, result.Data);
    }
}
