using Lab9.Common;

namespace Lab9.SimplexAlgorithm.Models;
public struct Tableau(double[,] data, string[] rows, string[] columns, char[] rowVars, char[] colVars, int order) : ICloneable {
    public char[] RowVars { get; private set; } = rowVars;
    public char[] ColVars { get; private set; } = colVars;

    public string[] Rows { get; set; } = rows;
    public string[] Columns { get; set; } = columns;

    public double[,]? Data { get; set; } = data;

    public readonly int Offset => ColVars.Length + 3;
    public readonly int Order => order;
    public readonly int Width => Data is null ? 0 : Data.GetLength(1);
    public readonly int Height => Data is null ? 0 : Data.GetLength(0);

    public double this[int row, int col] {
        get => Data is not null && row < Height && col < Width ? Data[row, col] : double.NaN;
        set { if (Data is not null && row < Height && col < Width) Data[row, col] = value; }
    }

    public Tableau(double[,] data, string[] rows, string[] columns, int order)
        : this(data, rows, columns, ['y'], ['x'], order) { }

    public void Transpose() {
        if (Data is null) throw new ArgumentNullException(nameof(Data));

        var transposed = new double[Width, Height];

        for (int row = 0; row < Height; row++) {
            for (int col = 0; col < Width; col++) {
                transposed[col, row] = Data[row, col];
            }
        }
        Data = transposed;
        (RowVars, ColVars) = (ColVars, RowVars);
    }

    public void InvertSigns(bool max) {
        if (Data is null) throw new ArgumentNullException(nameof(Data));

        for (int row = 0; row < (max ? Height : Height - 1); row++)
            for (int col = 0; col < Width; col++)
                Data[row, col] *= -1;
    }

    public static Tableau Transpose(Tableau tableau) {
        if (tableau.Data is null) throw new ArgumentNullException(nameof(tableau.Data));

        var transposed = new double[tableau.Width, tableau.Height];

        for (int row = 0; row < tableau.Height; row++) {
            for (int col = 0; col < tableau.Width; col++) {
                transposed[col, row] = tableau.Data[row, col];
            }
        }

        char[] rowVars = tableau.ColVars;
        char[] colVars = tableau.RowVars;

        return new(transposed, tableau.Columns, tableau.Rows, rowVars, colVars, tableau.Height - 1);
    }

    public static Tableau InvertSigns(Tableau tableau) {
        if (tableau.Data is null) throw new ArgumentNullException(nameof(Data));

        for (int row = 0; row < tableau.Height; row++)
            for (int col = 0; col < tableau.Width; col++)
                tableau[row, col] *= -1;

        return tableau;
    }

    private string[,] ExtendedTableau() {
        if (Data is null) throw new ArgumentNullException(nameof(Data));

        string[,] extendedTable = new string[Height + 1, Width + 1];

        int rows = extendedTable.GetLength(0);
        int cols = extendedTable.GetLength(1);

        int leftEdge = Rows.Max(h => h.Length);

        extendedTable[0, 0] = "".PadLeft(leftEdge) + ' ';

        for (int row = 1; row < rows; row++) {
            extendedTable[row, 0] = Rows[row - 1].PadLeft(leftEdge) + ' ';
        }

        for (int col = 1; col < cols; col++) {
            extendedTable[0, col] = Columns[col - 1].PadLeft(Offset) + ' ';
        }

        for (int row = 1; row < rows; row++) {
            for (int col = 1; col < cols; col++) {
                if (Globals.Round(Data[row - 1, col - 1]) != 0) {
                    extendedTable[row, col] = $"{Globals.Round(Data[row - 1, col - 1])}".PadLeft(Offset) + ' ';
                } else extendedTable[row, col] = "0".PadLeft(Offset) + ' ';
            }
        }

        return extendedTable;
    }

    public void FixHeaders() {
        for (int i = 0; i < Columns.Length - 1; i++)
            if (!Columns[i].Contains('-'))
                Columns[i] = Columns[i].Replace($"{ColVars[0]}", $"-{ColVars[0]}");

        for (int i = 0; i < Rows.Length - 1; i++)
            Rows[i] = Rows[i].Replace("-", "");
    }

    public override string ToString() {
        string result = "\n";
        var extendedTableau = ExtendedTableau();

        for (int row = 0; row < extendedTableau.GetLength(0); row++) {
            for (int col = 0; col < extendedTableau.GetLength(1); col++)
                result += extendedTableau[row, col];
            result += "\n";
        }

        return result;
    }

    public object Clone() {
        return new Tableau((double[,])Data!.Clone(), (string[])Rows.Clone(), (string[])Columns.Clone(), Order);
    }
}