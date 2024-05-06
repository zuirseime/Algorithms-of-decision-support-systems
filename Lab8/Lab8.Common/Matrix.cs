namespace Lab8.Common;

public struct Matrix {
    public readonly int Width => _basis.GetLength(1);
    public readonly int Height => _basis.GetLength(0);

    private double[] _baseRows = null!;
    private double[] _rows = null!;
    private double[] _rowPotentials = null!;
    private readonly double[][] rowLayers => [_baseRows, _rows, _rowPotentials];

    private double[] _baseCols = null!;
    private double[] _cols = null!;
    private double[] _colPotentials = null!;
    private readonly double[][] colLayers => [_baseCols, _cols, _colPotentials];

    private readonly Dictionary<char, double[][]> axes => new() {
        { 'y', rowLayers },
        { 'x', colLayers }
    };

    private double[,] _basis;
    private double[,] _costs;
    private double[,] _indirectCosts;
    private readonly double[][,] layers => [_basis, _costs, _indirectCosts];

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

            for (int i = 0; i < _basis.GetLength(0); i++) {
                for (int j = 0; j < _basis.GetLength(1); j++) {
                    maxLength = Math.Max(maxLength, $"{Globals.Round(_basis[i, j])}".Length);
                }
            }

            return maxLength;
        }
    }

    public Matrix(double[,] data) {
        _costs = (double[,])data.Clone();
        var indirectCosts = Enumerable.Repeat(Enumerable.Repeat(double.NaN, data.GetLength(1)).ToArray(), data.GetLength(0)).ToArray();
        _indirectCosts = Convert(indirectCosts);
        _basis = new double[data.GetLength(0), data.GetLength(1)];
    }

    public void Load(double[] rows, double[] cols) {
        if (rows.Length != Height)
            throw GetSizeException(nameof(rows));
        if (cols.Length != Width)
            throw GetSizeException(nameof(cols));

        _rows = (double[])rows.Clone();
        _cols = (double[])cols.Clone();

        _rowPotentials = Enumerable.Repeat(double.NaN, rows.Length).ToArray();
        _colPotentials = Enumerable.Repeat(double.NaN, cols.Length).ToArray();

        _baseRows = (double[])rows.Clone();
        _baseCols = (double[])cols.Clone();
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

    public override string ToString() => ToString(false, 0);

    public string ToString(bool full = false, int layer = 0) {
        string result = string.Empty;
        var extendedTableau = GetExtendedArray(full, layer);

        for (int row = 0; row < extendedTableau.GetLength(0); row++) {
            for (int col = 0; col < extendedTableau.GetLength(1); col++)
                result += extendedTableau[row, col];
            result += "\n";
        }

        return result;
    }

    private string[,] GetExtendedArray(bool full, int layer) {
        if (_basis is null) throw new ArgumentNullException(nameof(_basis));

        int rows = Height + (full ? 2 : 1);
        int cols = Width + (full ? 2 : 1);

        string[,] extendedTable = new string[rows, cols];

        int offset = _baseCols.Max(h => h.ToString().Length) * 4 + 3;
        int leftEdge = _baseRows.Max(h => h.ToString().Length);

        extendedTable[0, 0] = "".PadLeft(leftEdge) + ' ';

        for (int row = 1; row < rows - (full ? 1 : 0); row++) {
            //extendedTable[row, 0] = Rows[row - 1].ToString().PadLeft(leftEdge) + ' ';
            extendedTable[row, 0] = _baseRows[row - 1].ToString().PadLeft(leftEdge) + ' ';
        }

        for (int col = 1; col < cols - (full ? 1 : 0); col++) {
            //extendedTable[0, col] = Cols[col - 1].ToString().PadLeft(offset) + ' ';
            extendedTable[0, col] = _baseCols[col - 1].ToString().PadLeft(offset) + ' ';
        }

        for (int row = 1; row < rows - (full ? 1 : 0); row++) {
            for (int col = 1; col < cols - (full ? 1 : 0); col++) {
                extendedTable[row, col] = $"{Globals.Round(this[layer, row - 1, col - 1])}".PadLeft(offset) + ' ';
            }
        }

        if (full) {
            for (int row = 1; row < rows - 1; row++) {
                extendedTable[row, cols - 1] = _rowPotentials[row - 1].ToString().PadLeft(offset) + ' ';
            }

            extendedTable[rows - 1, 0] = "".PadLeft(leftEdge) + ' ';
            for (int col = 1; col < cols - 1; col++) {
                extendedTable[rows - 1, col] = _colPotentials[col - 1].ToString().PadLeft(offset) + ' ';
            }
        }

        return extendedTable;
    }
}
