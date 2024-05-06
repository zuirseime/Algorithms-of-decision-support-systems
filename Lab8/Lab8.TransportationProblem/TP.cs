using Lab8.Common;

namespace Lab8.TransportationProblem;

public abstract class TP {
    protected Matrix _matrix;
    protected TPResult _result;

    public TPResult Run(string matrix, string inputs, string outputs) {
        return Run(Matrix.Parse(matrix), StringToArray(inputs), StringToArray(outputs));
    }

    public TPResult Run(Matrix matrix, double[] inputs, double[] outputs) {
        _matrix = matrix;

        try {
            _matrix.Load(outputs, inputs);
        } catch (ArgumentException ex) {

        }

        FindReferencePlan();
        FindOptimalPlan();

        return _result;
    }

    protected virtual void FindReferencePlan() {
        Console.WriteLine("Finding reference plan:");
    }

    private void FindOptimalPlan() {
        Console.WriteLine("Finding optimal plan:");

        while (true) {
            FindPotentials();
            FindIndirectCosts();
            if (!FindProblemCells(out int row, out int col)) break;
        }
    }

    private void FindPotentials() {
        _matrix['y', 2, 0] = 0;
        int r = 0, c = 0;
        while (true) {
            if (_matrix[0, r, c] != 0) {
                if (double.IsNaN(_matrix['x', 2, c]) && !double.IsNaN(_matrix['y', 2, r])) {
                    _matrix['x', 2, c] = _matrix[1, r, c] - _matrix['y', 2, r];
                }
                if (double.IsNaN(_matrix['y', 2, r]) && !double.IsNaN(_matrix['x', 2, c])) {
                    _matrix['y', 2, r] = _matrix[1, r, c] - _matrix['x', 2, c];
                }
            }

            c++;
            if (c >= _matrix.Width) {
                c = 0;
                r++;
            }

            if (r >= _matrix.Height) {
                if (_matrix['x', 2].Contains(double.NaN) || _matrix['y', 2].Contains(double.NaN)) {
                    r = c = 0;
                } else break;
            }
        }

        Console.WriteLine(_matrix.ToString(true));
    }

    private void FindIndirectCosts() {
        for (int r = 0; r < _matrix.Height; r++) {
            for (int c = 0; c < _matrix.Width; c++) {
                if (_matrix[0, r, c] == 0) {
                    _matrix[2, r, c] = _matrix['x', 2, c] + _matrix['y', 2, r];
                }
            }
        }

        Console.WriteLine(_matrix.ToString(true, 2));
    }

    private bool FindProblemCells(out int row, out int col) {
        row = int.MinValue;
        col = int.MinValue;
        double maxDiff = double.MinValue;

        for (int r = 0; r < _matrix.Height; r++) {
            for (int c = 0; c < _matrix.Width; c++) {
                double diff = _matrix[2, r, c] - _matrix[1, r, c];
                if (diff > maxDiff) {
                    maxDiff = diff;
                    row = r;
                    col = c;
                }
            }
        }

        Console.WriteLine($"\n\n[{row}, {col}] = {maxDiff}");
        return row == int.MinValue || col == int.MinValue;
    }

    private void FindCellToOptimize(int row, int col) {
        while (true) {

        }
    }

    protected double FindTotalCost() {
        double sum = 0;
        for (int r = 0; r < _matrix.Height; r++) {
            for (int c = 0; c < _matrix.Width; c++) {
                sum += _matrix[0, r, c] * _matrix[1, r, c];
            }
        }
        return sum;
    }

    protected static double[] StringToArray(string text) 
        => text.Split().Select(double.Parse).ToArray();
}

