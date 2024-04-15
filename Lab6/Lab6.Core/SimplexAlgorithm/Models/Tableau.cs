namespace Lab6.Core.SimplexAlgorithm.Models;
internal struct Tableau(double[,] data, string[] rows, string[] columns, char[] rowVars, char[] colVars, int order) {
    internal char[] RowVars { get; private set; } = rowVars;
    internal char[] ColVars { get; private set; } = colVars;

    internal string[] Rows { get; set; } = rows;
    internal string[] Columns { get; set; } = columns;

    internal double[,]? Data { get; set; } = data;

    internal readonly int Offset => this.ColVars.Length * 4 + 3;
    internal readonly int Order => order;
    internal readonly int Width => this.Data is null ? 0 : this.Data.GetLength(1);
    internal readonly int Height => this.Data is null ? 0 : this.Data.GetLength(0);

    internal double this[int row, int col] {
        get => this.Data is not null && row < this.Height && col < this.Width ? this.Data[row, col] : double.NaN;
        set { if (this.Data is not null && row < this.Height && col < this.Width) Data[row, col] = value; }
    }

    internal Tableau(double[,] data, string[] rows, string[] columns, int order)
        : this(data, rows, columns, ['y'], ['x'], order) { }

    internal void Transpose() {
        if (this.Data is null) throw new ArgumentNullException(nameof(this.Data));

        var transposed = new double[this.Width, this.Height];

        for (int row = 0; row < this.Height; row++) {
            for (int col = 0; col < this.Width; col++) {
                transposed[col, row] = this.Data[row, col];
            }
        }
        this.Data = transposed;
        (this.RowVars, this.ColVars) = (this.ColVars, this.RowVars);
    }

    internal void InvertSigns() {
        if (Data is null) throw new ArgumentNullException(nameof(Data));

        for (int row = 0; row < this.Height; row++)
            for (int col = 0; col < this.Width; col++)
                Data[row, col] *= -1;
    }

    internal static Tableau Transpose(Tableau tableau) {
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

    internal static Tableau InvertSigns(Tableau tableau) {
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
                extendedTable[row, col] = $"{Math.Round(Data[row - 1, col - 1], Globals.Round)}".PadLeft(Offset) + ' ';
            }
        }

        return extendedTable;
    }

    internal void FixHeaders() {
        for (int i = 0; i < this.Columns.Length - 1; i++)
            if (!this.Columns[i].Contains('-'))
                this.Columns[i] = this.Columns[i].Replace($"{this.ColVars[0]}", $"-{this.ColVars[0]}");

        for (int i = 0; i < this.Rows.Length - 1; i++)
            this.Rows[i] = this.Rows[i].Replace("-", "");
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
}