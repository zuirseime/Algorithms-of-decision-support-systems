using Lab8.Common;
using Lab8.SimplexAlgorithm.Models;

namespace Lab8.SimplexAlgorithm;
public class ProblemBuilder {
    private double[,] _matrix = null!;
    private double[] _suppliers = null!;
    private double[] _customers = null!;

    public Constraints Constraints { get; private set; } = null!;
    public Function Function { get; private set; } = null!;

    public void Run(string matrix, string suppliers, string customers, bool max = true) {
        _matrix = Parse(matrix);
        _suppliers = StringToArray(suppliers);
        _customers = StringToArray(customers);

        BuildFunction(max);
        BuildConstraints();
    }

    private void BuildFunction(bool max) {
        List<double> funcCoeffs = [];
        for (int row = 0; row < _matrix.GetLength(0); row++) {
            for (int col = 0; col < _matrix.GetLength(1); col++) {
                funcCoeffs.Add(_matrix[row, col]);
            }
        }

        Function = new Function([.. funcCoeffs], 0, 'x', max);
    }

    private void BuildConstraints() {
        List<Constraint> constraints = [];

        int rows = _matrix.GetLength(0);
        int cols = _matrix.GetLength(1);
        Globals.MatrixSize = new System.Drawing.Size(cols, rows);

        for (int row = 0; row < rows; row++) {
            double[] constCoeffs = new double[rows * cols];
            for (int col = 0; col < cols; col++) {
                constCoeffs[row * cols + col] = -1;
            }
            Constraint constraint = new(constCoeffs, -_suppliers[row], Relation.GreaterOrEqual);
            constraints.Add(constraint);
        }

        for (int col = 0; col < cols; col++) {
            double[] constCoeffs = new double[rows * cols];
            for (int row = 0; row < rows; row++) {
                constCoeffs[row * cols + col] = 1;
            }
            Constraint constraint = new(constCoeffs, _customers[col], Relation.GreaterOrEqual);
            constraints.Add(constraint);
        }

        Constraints = new Constraints([.. constraints]);
    }

    private double[] VerticalConstraints(int rows, int cols) {
        double[] result = new double[rows * cols];

        for (int j = 0; j < cols; j++) {
            for (int i = 0; i < rows; i++) {
                result[j * rows + i] = 1;
            }
        }

        return result;
    }

    private double[] HorizontalConstraints(int rows, int cols) {
        double[] result = new double[rows * cols];

        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                result[i * cols + j] = -1;
            }
        }

        return result;
    }

    private static double[,] Parse(string matrix) {
        if (string.IsNullOrEmpty(matrix))
            throw new FormatException("Incorrect data format.");

        var delimiters = new char[] { ' ', '\t' };

        string[] rows = matrix.Split('\n', StringSplitOptions.RemoveEmptyEntries);

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

        return data;
    }

    private static double[] StringToArray(string content)
        => content.Split().Select(double.Parse).ToArray();
}
