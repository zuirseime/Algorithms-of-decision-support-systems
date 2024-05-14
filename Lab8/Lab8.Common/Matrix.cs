namespace Lab8.Common;

public struct Matrix : ICloneable {
    public readonly int Width => _plan.GetLength(1);
    public readonly int Height => _plan.GetLength(0);

    private double[] _suppliers = null!;
    private double[] _supplies = null!;
    private double[] _rowPotentials = null!;
    private readonly double[][] rowLayers => [_suppliers, _supplies, _rowPotentials];

    private double[] _customers = null!;
    private double[] _orders = null!;
    private double[] _colPotentials = null!;
    private readonly double[][] colLayers => [_customers, _orders, _colPotentials];

    private readonly Dictionary<char, double[][]> axes => new() {
        { 'y', rowLayers },
        { 'x', colLayers }
    };

    private double[,] _plan;
    private double[,] _costs;
    private double[,] _indirectCosts;
    private readonly double[][,] layers => [_plan, _costs, _indirectCosts];

    public double[] this[char axis, int layer] {
        get => axes[axis][layer];
        private set => axes[axis][layer] = value;
    }

    public double this[char axis, int layer, int i] {
        get => axes[axis][layer][i];
        set => axes[axis][layer][i] = value;
    }

    public double[,] this[int layer] {
        get => layers[layer];
        private set => layers[layer] = value;
    }

    public double this[int layer, int i, int j] {
        get => layers[layer][i, j];
        set => layers[layer][i, j] = value;
    }

    private int Offset {
        get {
            int maxLength = int.MinValue;

            for (int i = 0; i < _plan.GetLength(0); i++) {
                for (int j = 0; j < _plan.GetLength(1); j++) {
                    maxLength = Math.Max(maxLength, $"{Globals.Round(_plan[i, j])}".Length);
                }
            }

            return maxLength + 3;
        }
    }

    public Matrix(double[,] data) : this(new double[data.GetLength(0), data.GetLength(1)], (double[,])data.Clone()) { }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Matrix(double[,] plan, double[,] costs) {
        _plan = (double[,])plan.Clone();
        _costs = (double[,])costs.Clone();
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public void FillIndirectCosts() {
        var indirectCosts = Enumerable.Repeat(Enumerable.Repeat(double.NaN, Width).ToArray(), Height).ToArray();
        _indirectCosts = Convert(indirectCosts);
    }

    public void FillPotentialsWithNaN() {
        _rowPotentials = Enumerable.Repeat(double.NaN, Height).ToArray();
        _colPotentials = Enumerable.Repeat(double.NaN, Width).ToArray();
    }

    public void Load(double[] rows, double[] cols) {
        if (rows.Length != Height)
            throw GetSizeException(nameof(rows));
        if (cols.Length != Width)
            throw GetSizeException(nameof(cols));

        _supplies = (double[])rows.Clone();
        _orders = (double[])cols.Clone();

        _suppliers = (double[])rows.Clone();
        _customers = (double[])cols.Clone();

        if (!IsClosed()) Close();
    }

    private bool IsClosed() => _suppliers.Sum() == _customers.Sum();

    private void Close() {
        double supplies = _suppliers.Sum();
        double orders = _customers.Sum();
        double delta = Math.Abs(supplies - orders);

        if (_suppliers.Sum() < _customers.Sum()) {
            _suppliers = ExtendAxis(_suppliers, delta);
            _supplies = ExtendAxis(_supplies, delta);
            _costs = ExtendMatrix(_costs, true);
            _plan = ExtendMatrix(_plan, true);
        } else {
            _customers = ExtendAxis(_customers, delta);
            _orders = ExtendAxis(_orders, delta);
            _costs = ExtendMatrix(_costs, false);
            _plan = ExtendMatrix(_plan, false);
        }
    }

    private static double[] ExtendAxis(double[] axis, double value) {
        double[] result = new double[axis.Length + 1];
        for (int i = 0; i < axis.Length; i++) result[i] = axis[i];
        result[^1] = value;
        return result;
    }

    private static double[,] ExtendMatrix(double[,] matrix, bool extendRows) {
        double[,] result;
        if (extendRows) {
            result = new double[matrix.GetLength(0) + 1, matrix.GetLength(1)];
            CopyMatrix(matrix, result);
        } else {
            result = new double[matrix.GetLength(0), matrix.GetLength(1) + 1];
            CopyMatrix(matrix, result);
        }
        return result;
    }

    private static void CopyMatrix(double[,] source, double[,] destination) {
        for (int i = 0; i < source.GetLength(0); i++) {
            for (int j = 0; j < source.GetLength(1); j++) {
                destination[i, j] = source[i, j];
            }
        }
    }

    private static double[,] Convert(double[][] data) {
        int rows = data.GetLength(0);
        int cols = data.Max(r => r.Length);

        double[,] temp = new double[rows, cols];
        for (int row = 0; row < rows; row++) {
            for (int col = 0; col < cols; col++)
                temp[row, col] = data[row][col];
        }

        return temp;
    }

    public static Matrix Parse(string matrix) {
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

        return new Matrix(data);
    }

    private static ArgumentException GetSizeException(string param) =>
        new ArgumentException($"{nameof(Matrix)}: {nameof(Load)}(..., {param})");

    public override string ToString() => ToString(0, 0);

    public string ToString(int contentLayer = 0, int headerLayer = 0, bool full = true) {
        string result = string.Empty;
        var extendedTableau = GetExtendedArray(contentLayer, headerLayer, full);

        for (int row = 0; row < extendedTableau.GetLength(0); row++) {
            for (int col = 0; col < extendedTableau.GetLength(1); col++)
                result += extendedTableau[row, col];
            result += "\n";
        }

        return result;
    }

    private string[,] GetExtendedArray(int contentLayer, int headerLayer, bool full) {
        if (_plan is null) throw new ArgumentNullException(nameof(_plan));

        int rows = Height + (full ? 1 : 0);
        int cols = Width + (full ? 1 : 0);

        string[,] extendedTable = new string[rows, cols];

        if (full) {
            int leftEdge = _suppliers.Max(h => h.ToString().Length);

            extendedTable[0, 0] = "".PadLeft(leftEdge) + ' ';

            for (int row = 1; row < rows; row++) {
                extendedTable[row, 0] = this['y', headerLayer, row - 1].ToString().PadLeft(leftEdge) + ' ';
            }

            for (int col = 1; col < cols; col++) {
                extendedTable[0, col] = this['x', headerLayer, col - 1].ToString().PadLeft(Offset) + ' ';
            }

            for (int row = 1; row < rows; row++) {
                for (int col = 1; col < cols; col++) {
                    var value = this[contentLayer, row - 1, col - 1];
                    extendedTable[row, col] = $"{(double.IsNaN(value) ? "-" : Globals.Round(value))}".PadLeft(Offset) + ' ';
                }
            }
        } else {
            for (int row = 0; row < rows; row++) {
                for (int col = 0; col < cols; col++) {
                    var value = this[contentLayer, row, col];
                    extendedTable[row, col] = $"{(double.IsNaN(value) ? "-" : Globals.Round(value))}".PadLeft(Offset) + ' ';
                }
            }
        }

        return extendedTable;
    }

    public object Clone() {
        double[,] plan = (double[,])this[0].Clone();
        double[,] costs = (double[,])this[1].Clone();
        double[] rows = (double[])this['y', 0].Clone();
        double[] cols = (double[])this['x', 0].Clone();

        Matrix matrix = new(plan, costs);
        matrix.Load(rows, cols);

        return matrix;
    }
}
