using Lab1.Core;

namespace Lab1.Tests;

[TestClass]
public class RankTests {
    [TestMethod]
    public void Matrix2x4Rank() {
        double[,] matrixValues = {
            { 1, 2, 3, 4 },
            { 2, 4, 6, 8 }
        };
        int expectedRank = 1;

        Matrix matrix = new() { Data = matrixValues };
        int rank = matrix.Rank();

        Assert.AreEqual(expectedRank, rank);
    }

    [TestMethod]
    public void Matrix3x3Rank() {
        double[,] matrixValues = {
            { 2, 5, 4 },
            { -3, 1, -2 },
            { -1, 6, 2 }
        };
        int expectedRank = 2;

        Matrix matrix = new() { Data = matrixValues };
        int rank = matrix.Rank();

        Assert.AreEqual(expectedRank, rank);
    }

    [TestMethod]
    public void Matrix4x2Rank() {
        double[,] matrixValues = {
            { 1, 2 },
            { 3, 6 },
            { 5, 10 },
            { 4, 8 }
        };
        int expectedRank = 1;

        Matrix matrix = new() { Data = matrixValues };
        int rank = matrix.Rank();

        Assert.AreEqual(expectedRank, rank);
    }

    [TestMethod]
    public void Matrix3x3Rank2() {
        double[,] matrixValues = {
            { 6, 2, 5 },
            { -3, 4, -1 },
            { -1, 4, 3 }
        };
        int expectedRank = 3;

        Matrix matrix = new() { Data = matrixValues };
        int rank = matrix.Rank();

        Assert.AreEqual(expectedRank, rank);
    }

    [TestMethod]
    public void Matrix3x3Rank3() {
        double[,] matrixValues = {
            { -1, 5, 4 },
            { -2, 7, 5 },
            { -3, 4, 1 }
        };
        int expectedRank = 2;

        Matrix matrix = new() { Data = matrixValues };
        int rank = matrix.Rank();

        Assert.AreEqual(expectedRank, rank);
    }

    [TestMethod]
    public void Matrix4x4Rank() {
        double[,] matrixValues = {
            { 1, 2, 3, 4 },
            { -2, 5, -1, 3 },
            { 2, 4, 6, 8 },
            { -1, 7, 2, 7 }
        };
        int expectedRank = 2;

        Matrix matrix = new() { Data = matrixValues };
        int rank = matrix.Rank();

        Assert.AreEqual(expectedRank, rank);
    }
}