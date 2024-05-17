using Lab9.Common;
using Lab9.SimplexAlgorithm.Models;

namespace Lab9.SimplexAlgorithm.Modules;
public class Solution : Module {
    protected Tableau _tableau;

    public Roots Roots { get; protected set; }
    public double Value => _tableau.Data![_tableau.Height - 1, _tableau.Width - 1] * (IsMax ? 1 : -1);
    internal static bool IsMax { get; set; }

    public override string ToString() {
        int rows = Globals.MatrixSize.Height;
        int cols = Globals.MatrixSize.Width;

        double[,] matrix = new double[rows, cols];

        for (int row = 0; row < rows; row++) {
            for (int col = 0; col < cols; col++) {
                matrix[row, col] = Roots[row * cols + col];
            }
        }

        string result = string.Empty;
        var extendedTableau = GetStringMatrix(matrix);

        for (int row = 0; row < extendedTableau.GetLength(0); row++) {
            for (int col = 0; col < extendedTableau.GetLength(1); col++)
                result += extendedTableau[row, col];
            result += "\n";
        }

        return result;
    }

    private string[,] GetStringMatrix(double[,] matrix) {

        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        string[,] extendedTable = new string[rows, cols];

        extendedTable[0, 0] = "".PadLeft(Offset(matrix)) + ' ';

        for (int row = 0; row < rows; row++) {
            for (int col = 0; col < cols; col++) {
                var value = matrix[row, col];
                extendedTable[row, col] = $"{Globals.Round(value)}".PadLeft(Offset(matrix)) + ' ';
            }
        }

        return extendedTable;
    }

    private int Offset(double[,] matrix) {
        int maxLength = int.MinValue;

        for (int i = 0; i < matrix.GetLength(0); i++) {
            for (int j = 0; j < matrix.GetLength(1); j++) {
                maxLength = Math.Max(maxLength, $"{Globals.Round(matrix[i, j])}".Length);
            }
        }

        return maxLength + 3;
    }
}
