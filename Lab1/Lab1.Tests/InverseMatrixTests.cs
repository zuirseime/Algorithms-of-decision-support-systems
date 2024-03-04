using Lab1.Core;

namespace Lab1.Tests;

[TestClass]
public class InverseMatrixTests {
    [TestMethod]
    public void InverseMatrixTest1() {
        double[,] matrixValues = {
            { 5, -3, 7 },
            { -1, 4, 3 },
            { 6, -2, 5 }
        };
        double[,] expectedMatrix = {
            { -0.28, -0.011, 0.398 },
            { -0.247, 0.183, 0.237 },
            { 0.237, 0.086, -0.183 }
        };

        Matrix matrix = new() { Data = matrixValues };

        string protocol = string.Empty;
        Matrix inverseMatrix = matrix.Invert(ref protocol);

        for (int i = 0; i < inverseMatrix.Data.GetLength(0); i++) {
            for (int j = 0; j < inverseMatrix.Data.GetLength(1); j++) {
                inverseMatrix.Data[i, j] = Math.Round(inverseMatrix.Data[i, j], Matrix.Round);
            }
        }

        CollectionAssert.AreEqual(expectedMatrix, inverseMatrix.Data);
    }

    [TestMethod]
    public void InverseMatrixTest2() {
        double[,] matrixValues = {
            { 6, 2, 5 },
            { -3, 4, -1 },
            { 1, 4, 3 }
        };
        double[,] expectedMatrix = {
            { 0.5, 0.438, -0.688 },
            { 0.25, 0.406, -0.281 },
            { -0.5, -0.688, 0.938 }
        };

        Matrix matrix = new() { Data = matrixValues };

        string protocol = string.Empty;
        Matrix inverseMatrix = matrix.Invert(ref protocol);

        for (int i = 0; i < inverseMatrix.Data.GetLength(0); i++) {
            for (int j = 0; j < inverseMatrix.Data.GetLength(1); j++) {
                inverseMatrix.Data[i, j] = Math.Round(inverseMatrix.Data[i, j], Matrix.Round);
            }
        }

        CollectionAssert.AreEqual(expectedMatrix, inverseMatrix.Data);
    }

    [TestMethod]
    public void InverseMatrixTest3() {
        double[,] matrixValues = {
            { 2, -1, 3 },
            { -1, 2, 2 },
            { 1, 1, 1 }
        };
        double[,] expectedMatrix = {
            { 0, -0.333, 0.667 },
            { -0.25, 0.083, 0.583 },
            { 0.25, 0.25, -0.25 }
        };

        Matrix matrix = new() { Data = matrixValues };

        string protocol = string.Empty;
        Matrix inverseMatrix = matrix.Invert(ref protocol);

        for (int i = 0; i < inverseMatrix.Data.GetLength(0); i++) {
            for (int j = 0; j < inverseMatrix.Data.GetLength(1); j++) {
                inverseMatrix.Data[i, j] = Math.Round(inverseMatrix.Data[i, j], Matrix.Round);
            }
        }

        CollectionAssert.AreEqual(expectedMatrix, inverseMatrix.Data);
    }
}
