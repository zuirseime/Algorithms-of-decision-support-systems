namespace Lab8.Common;

public struct Matrix {
    private double[] _baseRows;
    private double[] _baseCols;

    public double[] Rows { get; private set; } 
    public double[] Cols { get; private set; }
    public double[,] Data { get; set; }
    public double[,] Costs { get; private set; }

    public readonly int Width => Data.GetLength(1);
    public readonly int Height => Data.GetLength(0);

    private int Offset {
        get {
            int maxLength = int.MinValue;

            for (int i = 0; i < Data.GetLength(0); i++) {
                for (int j = 0; j < Data.GetLength(1); j++) {
                    maxLength = Math.Max(maxLength, $"{Globals.Round(Data[i, j])}".Length);
                }
            }

            return maxLength;
        }
    }

    public double this[int i, int j] {
        get => Data[i, j];
        set => Data[i, j] = value;
    }

    public Matrix(double[,] data) {
        Costs = (double[,])data.Clone();
        Data = new double[data.GetLength(0), data.GetLength(1)];
    }

    public void Load(double[] rows, double[] columns) {
        if (rows.Length != Height)
            throw GetSizeException(nameof(rows));
        if (columns.Length != Width)
            throw GetSizeException(nameof(columns));

        Rows = (double[])rows.Clone();
        Cols = (double[])columns.Clone();
        _baseRows = (double[])rows.Clone();
        _baseCols = (double[])columns.Clone();
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

    public override string ToString() {
        string result = "\n";
        var extendedTableau = GetExtendedArray();

        for (int row = 0; row < extendedTableau.GetLength(0); row++) {
            for (int col = 0; col < extendedTableau.GetLength(1); col++)
                result += extendedTableau[row, col];
            result += "\n";
        }

        return result;
    }

    private readonly string[,] GetExtendedArray() {
        if (this.Data is null) throw new ArgumentNullException(nameof(this.Data));

        string[,] extendedTable = new string[this.Height + 1, this.Width + 1];

        int offset = this._baseCols.Max(h => h.ToString().Length) * 4 + 3;

        int rows = extendedTable.GetLength(0);
        int cols = extendedTable.GetLength(1);


        int leftEdge = _baseRows.Max(h => h.ToString().Length);

        extendedTable[0, 0] = "".PadLeft(leftEdge) + ' ';

        for (int row = 1; row < rows; row++) {
            extendedTable[row, 0] = Rows[row - 1].ToString().PadLeft(leftEdge) + ' ';
            //extendedTable[row, 0] = _baseRows[row - 1].ToString().PadLeft(leftEdge) + ' ';
        }

        for (int col = 1; col < cols; col++) {
            extendedTable[0, col] = Cols[col - 1].ToString().PadLeft(offset) + ' ';
            //extendedTable[0, col] = _baseCols[col - 1].ToString().PadLeft(offset) + ' ';
        }

        for (int row = 1; row < rows; row++) {
            for (int col = 1; col < cols; col++) {
                extendedTable[row, col] = $"{Globals.Round(this.Data[row - 1, col - 1])}".PadLeft(offset) + ' ';
            }
        }

        return extendedTable;
    }
}
