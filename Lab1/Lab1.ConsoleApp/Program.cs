using Lab1.Core;

namespace Lab1.ConsoleApp;

internal class Program {
    static void Main(string[] args) {
        InverseMatrix();
        //MatrixRank();
    }

    private static void InverseMatrix() {
        double[,] matrixValues = {
            { 5, -3, 7 },
            { -1, 4, 3 },
            { 6, -2, 5 }
        };

        Matrix matrix = new() { Data = matrixValues };

        string protocol = string.Empty;
        Matrix inverseMatrix = matrix.Invert(ref protocol);

        Console.WriteLine($"Inverse Matrix:\n{inverseMatrix}");
    }

    private static void MatrixRank() {
        double[,] matrixValues = {
            { 1, 2 },
            { 3, 6 },
            { 5, 10 },
            { 4, 8 }
        };

        Matrix matrix = new() { Data = matrixValues };

        Console.WriteLine($"Matrix rank: {matrix.Rank()}");
    }
}
