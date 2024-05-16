using System.Linq;

namespace Lab9.HungarianMethod;

public class HM {
    private Matrix _matrix;
    private bool _full = false;

    public (Matrix, double) Run(string matrix) {
        _ = Matrix.TryParse(matrix, out _matrix);
        Matrix defaultMatrix = (Matrix)_matrix.Clone();

        Console.WriteLine("A cost matrix:");
        Console.WriteLine(_matrix);
        
        Decrease(true, _matrix.Height, _matrix.Width);
        Decrease(false, _matrix.Width, _matrix.Height);

        while (true) {
            if (CullOffLines()) break;

            double min = _matrix.Data.Min(i => i.State == State.None).Value;
            ModifyMatrix(min);
            _matrix.RestoreStates();

            Console.WriteLine(_matrix);
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

        Console.WriteLine($"Cost = {string.Join(" + ", additives)} = {cost}");
        return cost;
    }

    private void BuildAssignments() {
        _matrix.RestoreStates();

        while (_matrix.Data.Any(i => i.Value == 0 && i.State == State.None)) {
            GetAssignmentMatrix();
        }

        GetAssignments();
        Console.WriteLine(_matrix);
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

        Console.WriteLine($"Search for the minimum elements in each {type} and subtract it from each element in the {type}:");
        double[] mins = FindMinimums(byRow, outerCount);

        Subtract(mins, byRow, outerCount, innerCount);

        Console.WriteLine("\nThe cost matrix after subtracting:");
        Console.WriteLine(_matrix);
    }

    private double[] FindMinimums(bool byRow, int outer) {
        double[] result = new double[outer];

        for (int o = 0; o < outer; o++) {
            double min = (byRow ? _matrix.Data.MinInRow(o) : _matrix.Data.MinInColumn(o)).Value;
            result[o] = min;
            string type = byRow ? "row" : "column";
            Console.WriteLine($"The minimum value in {type} {o + 1} is {min}");
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

        Console.WriteLine(_matrix.ToString(true));

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

        if (_full) Console.WriteLine(_matrix.ToString(true));
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

        if (_full) Console.WriteLine(_matrix.ToString(true));
    }
}

public static class ArrayExtensions {
    public static T[] GetRow<T>(this T[,] matrix, int row) =>
        Enumerable.Range(0, matrix.GetLength(1)).Select(x => matrix[row, x]).ToArray();
    public static T[] GetColumn<T>(this T[,] matrix, int col) =>
        Enumerable.Range(0, matrix.GetLength(0)).Select(y => matrix[y, col]).ToArray();

    public static T MinInRow<T>(this T[,] matrix, int row) => GetRow(matrix, row).Min()!;
    public static T MinInColumn<T>(this T[,] matrix, int col) => GetColumn(matrix, col).Min()!;

    public static int CountInRow<T>(this T[,] matrix, T value, int row) where T : IComparable =>
        Enumerable.Range(0, matrix.GetLength(1)).Count(col => matrix[row, col].CompareTo(value) == 0);
    public static int CountInColumn<T>(this T[,] matrix, T value, int col) where T : IComparable =>
        Enumerable.Range(0, matrix.GetLength(0)).Count(row => matrix[row, col].CompareTo(value) == 0);

    public static int CountInRow<T>(this T[,] matrix, Func<T, bool> predicate, int row) where T : IComparable =>
        Enumerable.Range(0, matrix.GetLength(1)).Count(col => predicate(matrix[row, col]));
    public static int CountInColumn<T>(this T[,] matrix, Func<T, bool> predicate, int col) where T : IComparable =>
        Enumerable.Range(0, matrix.GetLength(0)).Count(row => predicate(matrix[row, col]));

    public static bool Contains<T>(this T[,] matrix, T value) where T : IComparable =>
        Enumerable.Range(0, matrix.GetLength(0)).Any(r => 
            Enumerable.Range(0, matrix.GetLength(1)).Any(c => 
                matrix[r, c].CompareTo(value) == 0));

    public static bool Any<T>(this T[,] matrix, Func<T, bool> predicate) =>
        matrix.Cast<T>().Any(predicate);

    public static T Min<T>(this T[,] matrix) => matrix.Cast<T>().Min()!;
    public static T Min<T>(this T[,] matrix, Func<T, bool> predicate) => 
        matrix.Cast<T>().Where(item => predicate(item)).Min()!;
}