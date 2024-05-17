using Lab9.Common;

namespace Lab9.HungarianMethod;

public class HM {
    private Matrix _matrix;
    private bool _full = false;

    public (Matrix, double) Run(string matrix) {
        _ = Matrix.TryParse(matrix, out _matrix);
        Matrix defaultMatrix = (Matrix)_matrix.Clone();

        Log.WriteLine("A cost matrix:");
        Log.WriteLine(_matrix);
        
        Decrease(true, _matrix.Height, _matrix.Width);
        Decrease(false, _matrix.Width, _matrix.Height);

        while (true) {
            if (CullOffLines()) break;

            double min = _matrix.Data.Min(i => i.State == State.None).Value;
            ModifyMatrix(min);
            _matrix.RestoreStates();

            Log.WriteLine(_matrix);
        }

        BuildAssignments();

        return (_matrix, GetCost(defaultMatrix));
    }

    private double GetCost(Matrix matrix) {
        double cost = 0;
        List<double> additives = [];

        for (int row = 0; row < _matrix.Height; row++) {
            for (int col = 0; col < _matrix.Width; col++) {
                if (_matrix.Data[row, col].Value == 1) {
                    var value = matrix.Data[row, col].Value;
                    additives.Add(value);
                    cost += value;
                }
            }
        }

        Log.WriteLine($"Cost = {string.Join(" + ", additives)} = {cost}");
        return cost;
    }

    private void BuildAssignments() {
        _matrix.RestoreStates();

        while (_matrix.Data.Any(i => i.Value == 0 && i.State == State.None)) {
            GetAssignmentMatrix();
        }

        GetAssignments();
        Log.WriteLine(_matrix);
    }

    private void GetAssignments() {
        for (int row = 0; row < _matrix.Height; row++) {
            for (int col = 0; col < _matrix.Width; col++) {
                if (_matrix[row, col].State == State.Picked)
                    _matrix.Data[row, col].Value = 1;
                else _matrix.Data[row, col].Value = 0;
            }
        }
    }

    private void GetAssignmentMatrix() {
        for (int i = 0; i < _matrix.Height; i++) {
            int zeros = _matrix.Data.CountInRow(z => z.Value == 0 && z.State != State.Erased, i);
            if (zeros != 1) continue;
            EraseExtraZeros(i);
        }
    }

    private void EraseExtraZeros(int i) {
        for (int col = 0; col < _matrix.Width; col++) {
            if (_matrix[i, col].Value != 0 || _matrix[i, col].State == State.Erased) continue;

            _matrix.Data[i, col].State = State.Picked;
            for (int row = 0; row < _matrix.Height; row++) {
                if (row == i || _matrix[row, col].Value != 0) continue;

                _matrix.Data[row, col].State = State.Erased;
            }
        }
    }

    private void ModifyMatrix(double min) {
        for (int row = 0; row < _matrix.Height; row++) {
            for (int col = 0; col < _matrix.Width; col++) {
                if (_matrix[row, col].State == State.None)
                    _matrix.Data[row, col].Value -= min;
                else if (_matrix[row, col].State == State.Overlaped)
                    _matrix.Data[row, col].Value += min;
            }
        }
    }

    private void Decrease(bool byRow, int outerCount, int innerCount) {
        string type = byRow ? "row" : "column";

        Log.WriteLine($"Search for the minimum elements in each {type} and subtract it from each element in the {type}:");
        double[] mins = FindMinimums(byRow, outerCount);

        Subtract(mins, byRow, outerCount, innerCount);

        Log.WriteLine("\nThe cost matrix after subtracting:");
        Log.WriteLine(_matrix);
    }

    private double[] FindMinimums(bool byRow, int outer) {
        double[] result = new double[outer];

        for (int o = 0; o < outer; o++) {
            double min = (byRow ? _matrix.Data.MinInRow(o) : _matrix.Data.MinInColumn(o)).Value;
            result[o] = min;
            string type = byRow ? "row" : "column";
            Log.WriteLine($"The minimum value in {type} {o + 1} is {min}");
        }
        return result;
    }

    private void Subtract(double[] mins, bool byRow, int outer, int inner) {
        for (int o = 0; o < outer; o++) {
            for (int i = 0; i < inner; i++) {
                if (byRow) _matrix[o, i] -= mins[o];
                else _matrix[i, o] -= mins[o];
            }
        }
    }

    private bool CullOffLines() {
        int rows = _matrix.Height;
        int cols = _matrix.Width;

        bool col = true;
        int count = 0;
        while (true) {
            count++;

            if (col) CullOffVertical(rows, cols);
            else CullOffHorizontal(rows, cols);

            if (!_matrix.Data.Any(i => i.Value == 0 && i.State == State.None))
                break;

            col = !col;
        }

        Log.WriteLine(_matrix.ToString(true));

        return count == _matrix.Width;
    }

    private void CullOffHorizontal(int rows, int cols) {
        double max = double.MinValue;
        int rMax = 0;
        for (int row = 0; row < rows; row++) {
            var value = _matrix.Data.CountInRow(i => i.Value == 0 && i.State == State.None, row);
            if (max < value) {
                max = value;
                rMax = row;
            }
        }

        bool hasZero = false;
        for (int col = 0; col < cols; col++) {
            if (_matrix[rMax, col].Value == 0 && _matrix[rMax, col].State == State.None && !hasZero) {
                _matrix.Data[rMax, col].State = State.Zero;
                hasZero = true;
                continue;
            }

            if (_matrix[rMax, col].State != State.Zero) {
                _matrix.Data[rMax, col].State =
                    _matrix[rMax, col].State != State.Vertical
                    ? State.Horizontal : State.Overlaped;
            }
        }

        if (_full) Log.WriteLine(_matrix.ToString(true));
    }

    private void CullOffVertical(int rows, int cols) {
        double max = double.MinValue;
        int cMax = 0;
        for (int col = 0; col < cols; col++) {
            var value = _matrix.Data.CountInColumn(i => i.Value == 0 && i.State == State.None, col);
            if (max < value) {
                max = value;
                cMax = col;
            }
        }

        bool hasZero = false;
        for (int row = 0; row < rows; row++) {
            if (_matrix[row, cMax].Value == 0 && _matrix[row, cMax].State == State.None && !hasZero) {
                _matrix.Data[row, cMax].State = State.Zero;
                hasZero = true;
                continue;
            }

            if (_matrix[row, cMax].State != State.Zero) {
                _matrix.Data[row, cMax].State =
                    _matrix[row, cMax].State != State.Horizontal
                    ? State.Vertical : State.Overlaped;
            }
        }

        if (_full) Log.WriteLine(_matrix.ToString(true));
    }
}