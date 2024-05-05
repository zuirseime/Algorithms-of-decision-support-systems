namespace CalculatingWork.Core.MulticriteriaOptimization;

public struct Array2D(string rowVar, string colVar) : ICloneable {
    public double[][] Values { get; set; }
    public readonly int Width => Values.Max(r => r.Length);
    public readonly int Height => Values.GetLength(0);

    private string _rowVar = rowVar;
    private string _colVar = colVar;

    public double this[int i, int j] {
        get => this.Values[i][j];
        set => this.Values[i][j] = value;
    }

    public Array2D(double[][] values, string rowVar, string colVar) : this(rowVar, colVar) {
        this.Values = values;
    }

    public Array2D(double[,] values, string rowVar, string colVar) : this(rowVar, colVar) {
        this.Values = Array2D.Convert(values);
    }

    public override string ToString() {
        string result = string.Empty;
        if (this.Values is null || (this.Width == 0 || this.Height == 0))
            return result;

        var tableau = GetArray();

        for (int row = 0; row < tableau.GetLength(0); row++) {
            for (int col = 0; col < tableau.GetLength(1); col++)
                result += tableau[row, col];
            result += "\n";
        }

        return result;
    }

    private string[,] GetArray() {
        string[,] strArray = new string[this.Height, this.Width];

        int offset = this._colVar.Length * 4 + 3;

        for (int row = 0; row < this.Height; row++) {
            for (int col = 0; col < this.Width; col++) {
                strArray[row, col] = $"{Globals.Round(this.Values[row][col])}".PadLeft(offset) + ' ';
            }
        }

        return strArray;
    }

    public string GetTableau() {
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
        if (this.Values is null) throw new ArgumentNullException(nameof(this.Values));

        string[,] extendedTable = new string[this.Height + 1, this.Width + 1];

        int offset = this._colVar.Length * 4 + 3;

        int rows = extendedTable.GetLength(0);
        int cols = extendedTable.GetLength(1);

        string[] rHeaders = new string[this.Height];
        for (int row = 0; row < this.Height; row++)
            rHeaders[row] = $"{this._rowVar}{row + 1}";

        string[] cHeaders = new string[this.Width];
        for (int col = 0; col < this.Width; col++)
            cHeaders[col] = $"{this._colVar}{col + 1}";

        int leftEdge = rHeaders.Max(h => h.Length);

        extendedTable[0, 0] = "".PadLeft(leftEdge) + ' ';

        for (int row = 1; row < rows; row++) {
            extendedTable[row, 0] = rHeaders[row - 1].PadLeft(leftEdge) + ' ';
        }

        for (int col = 1; col < cols; col++) {
            extendedTable[0, col] = cHeaders[col - 1].PadLeft(offset) + ' ';
        }

        for (int row = 1; row < rows; row++) {
            for (int col = 1; col < cols; col++) {
                extendedTable[row, col] = $"{Globals.Round(this.Values[row - 1][col - 1])}".PadLeft(offset) + ' ';
            }
        }

        return extendedTable;
    }

    public static double[,] Convert(double[][] data) {
        int rows = data.GetLength(0);
        int cols = data.Max(r => r.Length);

        double[,] temp = new double[rows, cols];
        for (int row = 0; row < rows; row++) {
            for (int col = 0; col < cols; col++)
                temp[row, col] = data[row][col];
        }

        return temp;
    }

    public static double[][] Convert(double[,] data) {
        int rows = data.GetLength(0);
        int cols = data.GetLength(1);

        double[][] temp = new double[rows][];
        for (int row = 0; row < rows; row++) {
            temp[row] = new double[cols];
            for (int col = 0; col < cols; col++)
                temp[row][col] = data[row, col];
        }

        return temp;
    }

    public readonly object Clone() {
        var cloned = this.Values.Select(s => s.ToArray()).ToArray();

        var clonedRowVar = (string)this._rowVar.Clone();
        var clonedColVar = (string)this._colVar.Clone();

        return new Array2D(cloned, clonedRowVar, clonedColVar);
    }
}
