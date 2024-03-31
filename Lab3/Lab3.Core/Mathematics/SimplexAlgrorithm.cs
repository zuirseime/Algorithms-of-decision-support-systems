using Lab3.Core.Input;
using Lab3.Core.Output;

namespace Lab3.Core.Mathematics;

/// <summary>A class that calculates a goal value using simplex algorithm</summary>
public sealed class SimplexAlgrorithm {
    private static int round = 3;
    private static int offset = 7;
    private static Log log = null!;

    public static int Round {
        get => round;
        set {
            if (value >= 0)
                round = value;
        }
    }

    private int _order;
    private string[] _xs = null!, _ys = null!;
    private SimplexAlgrorithmResult _result;

    public SimplexAlgrorithm() {
        log = Log.Instance;
        _result = new SimplexAlgrorithmResult();
    }

    /// <summary>Runs calculating the value of the goal function</summary>
    /// <param name="inputTable">An input table of constraints + goal function</param>
    /// <param name="constraints">A string of constraints</param>
    /// <param name="zFunc">A string of goal fucntion</param>
    /// <param name="max"><see langword="true"/> if the goal value needs to be maximized, and <see langword="false"/> is the goal needs to be minimized</param>
    /// <returns>The result of calculations: the roots and the goal value</returns>
    public SimplexAlgrorithmResult Run(Function func, Constraint[] constraints, bool max = true) {
        double[,]? table = GenerateTable(func, constraints);

        _order = constraints.Max(c => c.Coefficients.Length);

        log.Clear();
        log.WriteLine($"Problem definition:\nZ = {func} -> {(max ? "max" : "min")}\nwith constraints:\n{string.Join('\n', constraints as IEnumerable<Constraint>)}");
        if (max) log.WriteLine("\nInput simplex table:");

        table = max ? FindMaxOptimalSolution(table) : FindMinOptimalSolution(table);
        if (table is null) return SimplexAlgrorithmResult.Default;

        _result.OptimalSolution = Math.Round(table![table.GetLength(0) - 1, table.GetLength(1) - 1], Round);
        log.WriteLine($"{(max ? "Max" : "Min")} (Z) = {_result.OptimalSolution}");
        return _result;
    }

    /// <summary>Generates a simplex table</summary>
    /// <param name="func">The goal function</param>
    /// <param name="constraints">The constraints of the goal function</param>
    /// <returns>The simplex table</returns>
    private double[,]? GenerateTable(Function func, Constraint[] constraints) {
        int rows = constraints.Length + 1;
        int cols = constraints.Max(c => c.Length);

        _ys = new string[rows];
        _xs = new string[cols];

        double[,] table = new double[rows, cols];

        int y = 1;
        for (int row = 0; row < rows - 1; row++) {
            _ys[row] = constraints[row].Relation != Relation.Equal ? $"y{y++}" : "0";
            for (int col = 0; col < cols; col++) {
                table[row, col] = col == cols - 1 || col < constraints[row].Order() ? constraints[row][col] : 0;
                _xs[col] = col < cols - 1 ? $"-x{col + 1}" : "1";
            }
        }
        _ys[^1] = "Z =";

        for (int col = 0; col < cols; col++)
            table[rows - 1, col] = col != cols - 1 ? func.Coefficients[col] : 0;

        return table;
    }

    /// <summary>Executes modified jordan exclusions</summary>
    /// <param name="table">The simplex table to be modified</param>
    /// <param name="row">The pivot row</param>
    /// <param name="col">The pivot column</param>
    private void ModifiedJordanExclusions(double[,] table, int row, int col, bool log = true) {
        double[,] temp = (double[,])table.Clone();

        table[row, col] = 1;
        for (int i = 0; i < table.GetLength(0); i++) {
            if (i != row)
                table[i, col] *= -1;

            for (int j = 0; j < table.GetLength(1); j++) {
                if (i != row && j != col)
                    table[i, j] = temp[i, j] * temp[row, col]
                                - temp[i, col] * temp[row, j];

                table[i, j] /= temp[row, col];
            }
        }

        if (row != table.GetLength(0) - 1 && col != table.GetLength(1) - 1)
            (_xs[col], _ys[row]) = (_ys[row], _xs[col]);
        FixHeaderSigns();

        if (log) LogTable(table);
    }

    /// <summary>Finds a basic feasible solution</summary>
    /// <param name="table">The simplex table</param>
    /// <returns>A modified simplex table or <see langword="null"/> if the system of constrains is inconsistent or incompatible</returns>
    private double[,]? FindBasicFeasibleSolution(double[,]? table) {
        if (table is null) return null;

        InvertItemSigns(table);
        LogTable(table);

        if (_ys.Any(y => y == "0"))
            table = RemoveZeroRows(table);

        log.WriteLine("Finding a basic feasible solution:\n");
        while (true) {
            int negativeRow = RowWithNegativeElementInUnitColumn(table);
            if (negativeRow < 0) {
                log.WriteLine("A basic feasible solution has been found:");
                _result.BasicFeasibleSolutionRoots = LogRoots(table);
                return table;
            }

            int pivotCol = FindPivotColumn(table, negativeRow);
            if (pivotCol < 0) {
                log.WriteLine("The system of constraints is inconsistent.");
                LogTable(table);
                return null;
            }

            int pivotRow = FindPivotRow(table, pivotCol);
            if (pivotRow < 0) {
                log.WriteLine("The system of constraints is incompatible.");
                LogTable(table);
                return null;
            }

            LogSolvingElement(table, pivotRow, pivotCol);
        }
    }

    /// <summary>Finds a maximized optimal solution</summary>
    /// <param name="table">The simplex table</param>
    /// <returns>A modified simplex table or <see langword="null"/> if input table is <see langword="null"/> or a problem is unlimited from above</returns>
    private double[,]? FindMaxOptimalSolution(double[,]? table) {
        table = FindBasicFeasibleSolution(table);
        if (table is null) return null;

        log.WriteLine("\nFinding an optimal solution:\n");
        while (true) {
            int pivotCol = FindPivotColumn(table, table.GetLength(0) - 1);
            if (pivotCol < 0) {
                log.WriteLine("An optimal solution has been found:");
                _result.OptimalSolutionRoots = LogRoots(table);
                return table;
            }

            int pivotRow = FindPivotRow(table, pivotCol);
            if (pivotRow < 0) {
                log.WriteLine("Problem is unlimited from above.");
                LogTable(table);
                return null;
            }

            LogSolvingElement(table, pivotRow, pivotCol);
        }
    }

    /// <summary>Finds a maximized optimal solution</summary>
    /// <param name="table">The simplex table</param>
    /// <returns>A modified simplex table or <see langword="null"/> if input table is <see langword="null"/> or a problem is unlimited from above</returns>
    private double[,]? FindMinOptimalSolution(double[,]? table) {
        if (table is null) return null;

        log.WriteLine("\nGoing to the problem of maximizing the objective function Z':");
        string func = string.Empty;
        for (int col = 0; col < table.GetLength(1) - 1; col++) {
            table[table.GetLength(0) - 1, col] *= -1;
            double temp = table[table.GetLength(0) - 1, col];
            func += $"{(temp >= 0 ? temp : -temp)}*X[{col + 1}]{(col != table.GetLength(1) - 2 ? temp >= 0 ? " + " : " - " : "")}";
        }
        log.WriteLine($"Z' = {func} -> max");
        log.WriteLine("Input simplex table:");

        table = FindMaxOptimalSolution(table);
        if (table is null) return null;

        table[table.GetLength(0) - 1, table.GetLength(1) - 1] *= -1;
        return table;
    }

    /// <summary>Removes zero-rows from a simplex table</summary>
    /// <param name="table">The simplex table</param>
    /// <returns>A modified simplex table or <see langword="null"/> if input table is <see langword="null"/> or a problem is unlimited from above</returns>
    private double[,]? RemoveZeroRows(double[,]? table) {
        if (table is null) return null;

        log.WriteLine("\nRemoving zero-rows in the simplex table:\n");
        while (true) {
            int zeroRow = Array.FindIndex(_ys, y => y.Contains('0'));
            if (zeroRow < 0) {
                log.WriteLine("The simplex table doesn't have zero-rows.\n");
                zeroRow = int.MinValue;
                return table;
            }

            int pivotCol = ColumnWithPositiveElementInZeroRow(table, zeroRow);
            if (pivotCol < 0) {
                log.WriteLine("The system of constraints is inconsistent.");
                LogTable(table);
                return null;
            }

            int pivotRow = FindPivotRow(table, pivotCol);
            LogSolvingElement(table, pivotRow, pivotCol, pivotRow != zeroRow);

            if (pivotRow == zeroRow) {
                table = CropTable(table, pivotCol);
                LogTable(table);
            }
        }
    }

    #region Zero-rows removing steps
    /// <summary>Finds an index of the column that has a positiv element in the zero-row of a simplex table</summary>
    /// <param name="table">The simplex table</param>
    /// <param name="row">The zero-row index</param>
    /// <returns>The column index</returns>
    private static int ColumnWithPositiveElementInZeroRow(double[,] table, int row) {
        for (int col = 0; col < table.GetLength(1) - 1; col++) {
            if (table[row, col] > 0) {
                return col;
            }
        }
        return int.MinValue;
    }

    /// <summary>Crops a simplex table</summary>
    /// <param name="table">The simplex table</param>
    /// <param name="pivotCol">The column to be removed</param>
    /// <returns>The cropped simplex table</returns>
    private double[,] CropTable(double[,] table, int pivotCol) {
        double[,] cropped = new double[table.GetLength(0), table.GetLength(1) - 1];
        string[] croppedColHeader = new string[_xs.Length - 1];

        for (int row = 0; row < table.GetLength(0); row++) {
            int croppedCol = 0;
            for (int col = 0; col < table.GetLength(1); col++) {
                if (col == pivotCol) continue;

                cropped[row, croppedCol] = table[row, col];
                croppedColHeader[croppedCol++] = _xs[col];
            }
        }

        _xs = (string[])croppedColHeader.Clone();
        return cropped;
    }
    #endregion

    #region Basic Feasible and Optimal Solutions steps
    /// <summary>Finds an index of the row that has a negative element in the unit column of a simplex table</summary>
    /// <param name="table">The simplex table</param>
    /// <returns>The index of the row</returns>
    private static int RowWithNegativeElementInUnitColumn(double[,] table) {
        for (int row = 0; row < table.GetLength(0) - 1; row++) {
            if (table[row, table.GetLength(1) - 1] < 0) {
                return row;
            }
        }
        return int.MinValue;
    }

    /// <summary>Finds an index of the pivot column by the row</summary>
    /// <param name="table">The simplex table</param>
    /// <param name="row">The row</param>
    /// <returns>The index of the column</returns>
    private static int FindPivotColumn(double[,] table, int row) {
        for (int col = 0; col < table.GetLength(1) - 1; col++) {
            if (table[row, col] < 0) {
                return col;
            }
        }
        return int.MinValue;
    }

    /// <summary>Finds an index of the pivot row by the column</summary>
    /// <param name="table">The simplex table</param>
    /// <param name="col">The column</param>
    /// <returns>The index of the row</returns>
    private static int FindPivotRow(double[,] table, int col) {
        double min = double.MaxValue;
        int desiredRow = int.MinValue;

        for (int row = 0; row < table.GetLength(0) - 1; row++) {
            if (table[row, table.GetLength(1) - 1] == 0 && table[row, col] < 0)
                continue;

            if (table[row, col] != 0) {
                double ratio = table[row, table.GetLength(1) - 1] / table[row, col];
                if (ratio >= 0 && ratio < min) {
                    min = ratio;
                    desiredRow = row;
                }
            }
        }

        return desiredRow;
    }
    #endregion

    /// <summary>Fixes the row and column headers</summary>
    private void FixHeaderSigns() {
        for (int i = 0; i < _xs.Length - 1; i++)
            _xs[i] = !_xs[i].StartsWith('-') ? '-' + _xs[i] : _xs[i];

        for (int i = 0; i < _ys.Length - 1; i++)
            _ys[i] = _ys[i].StartsWith('-') ? _ys[i][1..] : _ys[i];
    }

    /// <summary>Inverts signs of all the items in the simplex table</summary>
    /// <param name="table">The simplex table</param>
    private static void InvertItemSigns(double[,] table) {
        for (int row = 0; row < table.GetLength(0); row++)
            for (int col = 0; col < table.GetLength(1) - 1; col++)
                table[row, col] *= -1;
    }


    #region Output
    /// <summary>Makes the simplex table look like text</summary>
    /// <param name="table">The simplex table</param>
    private void LogTable(double[,] table) {
        string result = "\n";
        string[,] extendedTable = GetExtendedTable(table, _xs, _ys);

        for (int row = 0; row < extendedTable.GetLength(0); row++) {
            for (int col = 0; col < extendedTable.GetLength(1); col++) {
                result += extendedTable[row, col];
            }
            result += '\n';
        }

        log.WriteLine(result);
    }

    /// <summary>Gives the simplex table in its extended form with row and column headers</summary>
    /// <param name="content">The inner content of the simplex table.</param>
    /// <param name="columnHeaders">The column headers</param>
    /// <param name="rowHeaders">The row headers</param>
    /// <returns>The extended simplex table</returns>
    private static string[,] GetExtendedTable(double[,] content, string[] columnHeaders, string[] rowHeaders) {
        string[,] extendedTable = new string[content.GetLength(0) + 1, content.GetLength(1) + 1];

        extendedTable[0, 0] = "".PadLeft(offset) + ' ';

        for (int row = 1; row < extendedTable.GetLength(0); row++) {
            extendedTable[row, 0] = rowHeaders[row - 1].PadLeft(offset) + ' ';
        }

        for (int col = 1; col < extendedTable.GetLength(1); col++) {
            extendedTable[0, col] = columnHeaders[col - 1].PadLeft(offset) + ' ';
        }

        for (int row = 1; row < extendedTable.GetLength(0); row++) {
            for (int col = 1; col < extendedTable.GetLength(1); col++) {
                extendedTable[row, col] = $"{Math.Round(content[row - 1, col - 1], Round)}".PadLeft(offset) + ' ';
            }
        }

        return extendedTable;
    }

    /// <summary>Makes the goal function optimal roots look like text</summary>
    /// <param name="table">The simplex table</param>
    /// <returns>The roots in text form</returns>
    private string LogRoots(double[,] table) {
        int colCount = table.GetLength(1) - 1;
        double[] roots = new double[_order];
        string result = "X = ( ";

        for (int yi = 0; yi < _ys.Length; yi++)
            if (_ys[yi].Contains('x')) {
                int index = _ys[yi].IndexOf('x');
                _ = int.TryParse(_ys[yi][(index + 1)..], out int coefficient);
                roots[coefficient - 1] = table[yi, colCount];
            }

        for (int col = 0; col < _order; col++) {
            result += $"{Math.Round(roots[col], Round)}{(col != _order - 1 ? "; " : " )")}";
        }

        log.WriteLine(result);
        return result;
    }

    /// <summary>Makes the simplex table solving element look like text</summary>
    /// <param name="table">The simplex table</param>
    /// <param name="row">The solving element row index</param>
    /// <param name="col">The solving elemnt column index</param>
    private void LogSolvingElement(double[,] table, int row, int col, bool logging = true) {
        log.WriteLine($"The solving row: {_ys[row]}\nThe solving column: {_xs[col]}");
        ModifiedJordanExclusions(table, row, col, logging);
    }
    #endregion
}
/// <summary>
/// A structure of the simplex algorithm result
/// </summary>
public struct SimplexAlgrorithmResult {
    public string BasicFeasibleSolutionRoots { get; set; }
    public string OptimalSolutionRoots { get; set; }
    public double OptimalSolution { get; set; }

    public static SimplexAlgrorithmResult Default => new() {
        BasicFeasibleSolutionRoots = string.Empty,
        OptimalSolutionRoots = string.Empty,
        OptimalSolution = double.NaN
    };
}