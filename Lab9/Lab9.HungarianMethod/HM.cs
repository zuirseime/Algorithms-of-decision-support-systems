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
            if (CullOffLines()) {
                Log.WriteLine("The matrix of optimal assignments is found");
                break;
            }

            Log.WriteLine("The matrix of optimal assignments is not found");

            double min = _matrix.Data.Min(i => i.State == State.None).Value;
            Log.WriteLine($"The minimum among the uncrossed elements: {min}");
            ModifyMatrix(min);
            _matrix.RestoreStates();
        }

        BuildAssignments();

        return (_matrix, GetCost(defaultMatrix));
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

        Log.WriteLine("Total cost of work:");
        Log.WriteLine($"S = {string.Join(" + ", additives)} = {cost}");
        return cost;
    }

    private void BuildAssignments() {
        _matrix.RestoreStates();

        while (_matrix.Data.Any(i => i.Value == 0 && i.State == State.None)) {
            GetAssignmentMatrix();
        }

        GetAssignments();

        Log.WriteLine("The assignment matrix:");
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

        Log.WriteLine("Cost matrix after adding and subtracting 'min' to the corresponding elements:");
        Log.WriteLine(_matrix);
    }

    private bool CullOffLines() {
        Log.WriteLine("Search for the matrix of optimal assignments:");
        Log.WriteLine("Cross out all zeros:");
        int rows = _matrix.Height;
        int cols = _matrix.Width;

        bool col = true;
        int count = 0;
        while (true) {
            count++;

            if (col) CullOff(rows, cols, false);
            else CullOff(rows, cols, true);

            if (!_matrix.Data.Any(i => i.Value == 0 && i.State == State.None))
                break;

            col = !col;
        }

        Log.WriteLine("Cost matrix after crossing out rows and columns with zeros:");
        Log.WriteLine(_matrix.ToString(true));
        Log.WriteLine($"Number of job assignments: {count}, total work: {_matrix.Width}");

        return count == _matrix.Width;
    }

    private void CullOff(int rows, int cols, bool horizontal) {
        int iMax = GetMaxZeroNumber(rows, cols, horizontal);

        bool hasZero = false;
        for (int i = 0; i < (horizontal ? cols : rows); i++) {
            MatrixItem item = horizontal ? _matrix[iMax, i] : _matrix[i, iMax];
            if (item.Value == 0 && item.State == State.None && !hasZero) {
                if (horizontal) _matrix.Data[iMax, i].State = State.Zero;
                else _matrix.Data[i, iMax].State = State.Zero;

                hasZero = true;
                continue;
            }

            if (horizontal) CullOffZeros(iMax, i, State.Vertical, State.Horizontal);
            else CullOffZeros(i, iMax, State.Horizontal, State.Vertical);
        }

        if (_full) Log.WriteLine(_matrix.ToString(true));
    }

    private int GetMaxZeroNumber(int rows, int cols, bool horizontal) {
        Func<int, double> countFunc = horizontal ?
            row => _matrix.Data.CountInRow(i => i.Value == 0 && i.State == State.None, row) :
            col => _matrix.Data.CountInColumn(i => i.Value == 0 && i.State == State.None, col);

        double max = double.MinValue;
        int iMax = 0;

        for (int i = 0; i < (horizontal ? rows : cols); i++) {
            double value = countFunc(i);
            if (max < value) {
                max = value;
                iMax = i;
            }
        }

        return iMax;
    }

    private void CullOffZeros(int row, int col, State oldState, State newState) {
        if (_matrix[row, col].State != State.Zero) {
            _matrix.Data[row, col].State = _matrix[row, col].State 
                                        != oldState ? newState : State.Overlaped;
        }
    }
}