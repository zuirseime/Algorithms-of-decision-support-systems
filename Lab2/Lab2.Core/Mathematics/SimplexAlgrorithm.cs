﻿using Lab2.Core.Output;

namespace Lab2.Core.Mathematics;

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

    private string[] _xs = null!, _ys = null!;
    private SimplexAlgrorithmResult _result;

    public SimplexAlgrorithm() {
        log = Log.Instance;
        _result = new SimplexAlgrorithmResult();
    }

    /// <summary>Runs calculating the value of the goal function</summary>
    /// <param name="inputTable">An input table of constraints + goal function</param>
    /// <param name="inequalities">A string of constraints</param>
    /// <param name="zFunc">A string of goal fucntion</param>
    /// <param name="max"><see langword="true"/> if the goal value needs to be maximized, and <see langword="false"/> is the goal needs to be minimized</param>
    /// <exception cref="ArgumentNullException"/>
    /// <returns>The result of calculations: the roots and the goal value</returns>
    public SimplexAlgrorithmResult Run(double[,]? inputTable, string inequalities, string zFunc, bool max = true) {
        ArgumentNullException.ThrowIfNull(inputTable);
        double[,]? table = (double[,])inputTable.Clone();

        log.Clear();
        log.WriteLine($"Problem definition:\nZ = {zFunc.Trim()} -> {(max ? "max" : "min")}\nwith constraints:\n{inequalities.Trim()}");
        if (max) log.WriteLine("\nInput simplex table:");

        PrepareAxes(table);

        table = max ? FindMaxOptimalSolution(table) : FindMinOptimalSolution(table);
        if (table is null) return SimplexAlgrorithmResult.Default;

        _result.OptimalSolution = Math.Round(table![table.GetLength(0) - 1, table.GetLength(1) - 1], Round);
        log.WriteLine($"{(max ? "Max" : "Min")} (Z) = {_result.OptimalSolution}");
        return _result;
    }

    /// <summary>Prepares row and column headers</summary>
    /// <param name="table">The simplex table</param>
    private void PrepareAxes(double[,] table) {
        _ys = new string[table.GetLength(0)];
        _xs = new string[table.GetLength(1)];

        for (int row = 0; row < table.GetLength(0); row++)
            _ys[row] = row < table.GetLength(0) - 1 ? $"y{row + 1}" : "Z";

        for (int col = 0; col < table.GetLength(1); col++)
            _xs[col] = col < table.GetLength(1) - 1 ? $"-x{col + 1}" : "1";
    }

    /// <summary>Executes modified jordan exclusions</summary>
    /// <param name="table">The simplex table to be modified</param>
    /// <param name="row">The pivot row</param>
    /// <param name="col">The pivot column</param>
    private void ModifiedJordanExclusions(ref double[,] table, int row, int col) {
        double[,] modTable = (double[,])table.Clone();

        modTable[row, col] = 1;
        for (int i = 0; i < table.GetLength(0); i++) {
            if (i != row)
                modTable[i, col] *= -1;

            for (int j = 0; j < table.GetLength(1); j++) {
                if (i != row && j != col)
                    modTable[i, j] = table[i, j] * table[row, col]
                                   - table[i, col] * table[row, j];

                modTable[i, j] /= table[row, col];
            }
        }

        if (row != table.GetLength(0) - 1 && col != table.GetLength(1) - 1)
            (_xs[col], _ys[row]) = (_ys[row], _xs[col]);
        FixHeaderSigns();
        LogTable(modTable);

        table = (double[,])modTable.Clone();
    }

    /// <summary>Finds a basic feasible solution</summary>
    /// <param name="table">The simplex table</param>
    /// <returns>A modified simplex table or <see langword="null"/> if the system of constrains is inconsistent or incompatible</returns>
    private double[,]? FindBasicFeasibleSolution(double[,]? table) {
        if (table is null) return null;

        InvertItemSigns(ref table);
        LogTable(table);
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

            LogSolvingElement(ref table, pivotRow, pivotCol);
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

            LogSolvingElement(ref table, pivotRow, pivotCol);
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

    #region Basic Feasible and Optimal Solutions steps
    /// <summary>Finds an index of the row that has a negative element in the unit column of a simplex table</summary>
    /// <param name="table">The simplex table</param>
    /// <returns>The index of the row</returns>
    private static int RowWithNegativeElementInUnitColumn(double[,] table) {
        int desiredRow = int.MinValue;

        for (int row = 0; row < table.GetLength(0) - 1; row++) {
            if (table[row, table.GetLength(1) - 1] < 0) {
                desiredRow = row;
                break;
            }
        }

        return desiredRow;
    }

    /// <summary>Finds an index of the pivot column by the row</summary>
    /// <param name="table">The simplex table</param>
    /// <param name="row">The row</param>
    /// <returns>The index of the column</returns>
    private static int FindPivotColumn(double[,] table, int row) {
        int desiredCol = int.MinValue;

        for (int col = 0; col < table.GetLength(1) - 1; col++) {
            if (table[row, col] < 0) {
                desiredCol = col;
                break;
            }
        }

        return desiredCol;
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
    private static void InvertItemSigns(ref double[,] table) {
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
        double[] roots = new double[colCount];
        string result = "X = ( ";

        for (int yi = 0; yi < _ys.Length; yi++)
            if (_ys[yi].Contains('x')) {
                int index = _ys[yi].IndexOf('x');
                _ = int.TryParse(_ys[yi][(index + 1)..], out int coefficient);
                roots[coefficient - 1] = table[yi, colCount];
            }

        for (int col = 0; col < colCount; col++) {
            result += $"{Math.Round(roots[col], Round)}{(col != colCount - 1 ? "; " : " )")}";
        }

        log.WriteLine(result);
        return result;
    }

    /// <summary>Makes the simplex table solving element look like text</summary>
    /// <param name="table">The simplex table</param>
    /// <param name="row">The solving element row index</param>
    /// <param name="col">The solving elemnt column index</param>
    private void LogSolvingElement(ref double[,] table, int row, int col) {
        log.WriteLine($"The solving row: {_ys[row]}\nThe solving column: {_xs[col]}");
        ModifiedJordanExclusions(ref table, row, col);
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