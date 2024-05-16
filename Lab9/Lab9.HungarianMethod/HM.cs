
namespace Lab9.HungarianMethod;

public class HM {
    private Matrix _matrix;

    public Matrix Run(string matrix) {
        _ = Matrix.TryParse(matrix, out _matrix);

        Console.WriteLine("A cost matrix:");
        Console.WriteLine(_matrix);
        
        Decrease(true, _matrix.Height, _matrix.Width);
        Decrease(false, _matrix.Width, _matrix.Height);

        while (true) {
            CullOffLines();

            // TODO: Change true
            if (true) break;
        }

        return _matrix;
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

    private void CullOffLines() {
        int rows = _matrix.Height;
        int cols = _matrix.Width;

        while (true) {
            CullOffHorizontal(rows, cols);
            CullOffVertical(rows, cols);

            if (!_matrix.Data.Contains(new MatrixItem(0)))
                break;
        }

        Console.WriteLine(_matrix.ToString(true));
    }

    private void CullOffHorizontal(int rows, int cols) {
        double min = double.MaxValue;
        int rMin = 0;
        for (int row = 0; row < rows; row++) {
            var value = _matrix.Data.CountInRow(new MatrixItem(0), row);
            if (min > value) {
                min = value;
                rMin = row;
            }
        }

        for (int col = 0; col < cols; col++) {
            if (_matrix[rMin, col] != 0) {
                _matrix._data[rMin, col].State =
                    _matrix._data[rMin, col].State != State.Vertical
                    ? State.Horizontal : State.Overlaped;
            } else _matrix._data[rMin, col].State = State.Zero;
        }
    }

    private void CullOffVertical(int rows, int cols) {
        double min = double.MaxValue;
        int cMin = 0;
        for (int col = 0; col < cols; col++) {
            var value = _matrix.Data.CountInColumn(new MatrixItem(0), col);
            if (min > value) {
                min = value;
                cMin = col;
            }
        }

        for (int row = 0; row < rows; row++) {
            if (_matrix[row, cMin] != 0) {
                _matrix._data[row, cMin].State =
                    _matrix._data[row, cMin].State != State.Horizontal
                    ? State.Vertical : State.Overlaped;
            } else _matrix._data[row, cMin].State = State.Zero;
        }
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

    public static bool Contains<T>(this T[,] matrix, T value) where T : IComparable =>
        Enumerable.Range(0, matrix.GetLength(0)).Any(r => 
            Enumerable.Range(0, matrix.GetLength(1)).Any(c => 
                matrix[r, c].CompareTo(value) == 0));
}