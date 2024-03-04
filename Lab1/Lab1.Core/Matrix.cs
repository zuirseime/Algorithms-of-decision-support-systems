using System.Diagnostics;

namespace Lab1.Core;
public class Matrix {
    private static int round = 3;

    public double[,] Data { get; set; }
    public static int Round { 
        get => round;
        set {
            if (value >= 0)
                round = value;
        }
    }
    private int Offset {
        get {
            int maxLength = int.MinValue;

            for (int i = 0; i < Data.GetLength(0); i++) {
                for (int j = 0; j < Data.GetLength(1); j++) {
                    if ($"{Math.Round(Data[i, j], Round)}".Length > maxLength) {
                        maxLength = $"{Math.Round(Data[i, j], Round)}".Length;
                    }
                }
            }

            return maxLength;
        }
    }

    public Matrix Invert(ref string protocol) {
        int order = Data.GetLength(0);
        if (Data.GetLength(1) != order) {
            protocol += "Matrix must be square to find its inverse.";
            return new Matrix() { Data = new double[0, 0] };
        }

        protocol += $"""
            Finding the inverse matrix:

            Input matrix:
            {this}
            Calculation protocol:
            

            """;

        double[,] matrix = (double[,])Data.Clone();
        for (int i = 0; i < order; i++) {
            UsualJordanExclusions(ref matrix, i, i);
            protocol += $"""
                Step #{i + 1}

                Solving element: A[{i + 1}, {i + 1}] = {Math.Round(matrix[i, i], round)}

                The matrix after performing the usual Jordan exclusions:
                {new Matrix() { Data = matrix }}

                """;
        }

        Matrix outMatrix = new() { Data = matrix };
        protocol += $"""
            Inverse matrix:
            {outMatrix}

            """;

        return outMatrix;
    }

    public int Rank() {
        double[,] matrix = (double[,])Data.Clone();
        int rank = 0;

        int order = Math.Min(matrix.GetLength(0), matrix.GetLength(1));

        for (int i = 0; i < order; i++) {
            if (matrix[i, i] != 0) {
                UsualJordanExclusions(ref matrix, i, i);
                rank++;
            }
        }

        return rank;
    }

    public static Matrix SolveLinearSystem(double[,] coefficients, double[,] constants, ref string protocol) {
        int order = coefficients.GetLength(0);

        protocol += """
            Finding solutions of a system of linear algebraic equations using the inverse matrix:


            """;

        Matrix matrix = new() { Data = coefficients };
        Matrix invertedMatrix = matrix.Invert(ref protocol);

        protocol += $"""
            Input matrix B:
            {new Matrix() { Data = constants }}

            """;

        return new Matrix { Data = FindXs(invertedMatrix.Data, constants, ref protocol) };
    }

    private static double[,] FindXs(double[,] coefficients, double[,] constants, ref string protocol) {
        int order = coefficients.GetLength(0);
        double[,] x = new double[order, 1];

        protocol += "Calculation of solutions:\n\n";

        for (int i = 0; i < order; i++) {
            double sum = 0;
            protocol += $"X[{i + 1}] = ";
            for (int j = 0; j < order; j++) {
                sum += coefficients[i, j] * constants[j, 0];
                protocol += $"{Math.Round(coefficients[i, j], round)} * " +
                            $"{Math.Round(constants[j, 0], round)}" +
                            $"{(j < order - 1 ? " + " : " = ")}";
            }

            x[i, 0] = sum;
            protocol += $"{Math.Round(sum, round)}\n";
        }

        return x;
    }

    private static void UsualJordanExclusions(ref double[,] matrix, int row, int col) {
        double[,] modMatrix = (double[,])matrix.Clone();

        modMatrix[row, col] = 1;
        for (int i = 0; i < matrix.GetLength(0); i++) {
            for (int j = 0; j < matrix.GetLength(1); j++) {
                if (i == row && j != col) {
                    modMatrix[i, j] *= -1;
                }
                if (i != row && j != col) {
                    modMatrix[i, j] = matrix[i, j] * matrix[row, col] -
                                      matrix[i, col] * matrix[row, j];
                }

                modMatrix[i, j] /= matrix[row, col];
            }
        }

        matrix = (double[,])modMatrix.Clone();
    }

    public static Matrix Parse(string str) {
        if (string.IsNullOrEmpty(str))
            throw new FormatException("Incorrect data format.");

        var delimiters = new char[] { ' ', '\t' };

        string[] rows = str.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        int numRows = rows.Length;
        int numCols = rows[0].Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length;

        double[,] data = new double[numRows, numCols];
        for (int row = 0; row < numRows; row++) {
            string[] elements = rows[row].Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

            if (elements.Length != numCols)
                throw new ArgumentException($"Row {row + 1} contains a differnet number of elements.");

            for (int col = 0; col < numCols; col++) {
                _ = double.TryParse(elements[col], out data[row, col]);
            }
        }

        return new Matrix() { Data = data };
    }

    public static bool TryParse(string str, out Matrix matrix) {
        matrix = new() { Data = new double[0, 0] };
        bool valid = false;

        try {
            matrix = Parse(str);
            valid = true;
        } catch (FormatException ex) {
            Debug.WriteLine(ex.Message);
        } catch (ArgumentException ex) {
            Debug.WriteLine(ex.Message);
        }

        return valid;
    }

    public override string ToString() {
        string result = string.Empty;

        for (int row = 0; row < Data.GetLength(0); row++) {
            for (int col = 0; col < Data.GetLength(1); col++) {
                result += $"{Math.Round(Data[row, col], round)}".PadLeft(Offset) + " ";
            }
            result += '\n';
        }

        return result;
    }
}
