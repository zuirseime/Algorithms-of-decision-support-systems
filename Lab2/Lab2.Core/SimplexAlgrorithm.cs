namespace Lab2.Core;
public static class SimplexAlgrorithm {
    private static int round = 3;
    private static string[] Xs = null!, Ys = null!;
    private static List<string> log = null!;
    private static int offset = 7;

    public static int Round {
        get => round;
        set {
            if (value >= 0)
                round = value;
        }
    }
    public static List<string> Log => log;

    public static double[]? FindRoots(double[,]? table, string inequalities, string zFunc, bool max = true) {
        log = [];
        log.Add($"Problem definition:\nZ = {zFunc} -> {(max ? "max" : "min")}");
        log.Add($"with constraints:\n{inequalities}");

        table = max ? FindMaxOptimalSolution(table) : FindMinOptimalSolution(table);
        if (table is null) return null;

        double[] roots = new double[Xs.Length + 1];
        for (int row = 0; row < Ys.Length; row++) {
            if (Ys[row].Contains('x')) {
                int index = Ys[row].IndexOf('x');
                _ = int.TryParse(Ys[row][(index + 1)..], out int col);
                roots[col - 1] = table[row, table.GetLength(1) - 1];
            }
        }

        roots[Xs.Length] = table[table.GetLength(0) - 1, table.GetLength(1) - 1];
        log.Add($"{(max ? "Max" : "Min")} (Z) = {Math.Round(roots[Xs.Length], Round)}");
        return roots;
    }

    private static void ModifiedJordanExclusions(ref double[,] matrix, int row, int col) {
        double[,] modMatrix = (double[,])matrix.Clone();

        modMatrix[row, col] = 1;
        for (int i = 0; i < matrix.GetLength(0); i++) {
            if (i != row)
                modMatrix[i, col] *= -1;

            for (int j = 0; j < matrix.GetLength(1); j++) {
                if (i != row && j != col)
                    modMatrix[i, j] = matrix[i, j] * matrix[row, col] -
                                      matrix[i, col] * matrix[row, j];

                modMatrix[i, j] /= matrix[row, col];
            }
        }

        (Xs[col], Ys[row]) = (Ys[row], Xs[col]);
        CheckSigns();
        LogTable(matrix);

        matrix = (double[,])modMatrix.Clone();
    }

    private static double[,]? FindBasicFeasibleSolution(double[,] table) {
        InvertSigns(ref table);
        LogTable(table);
        log.Add("Finding a basic feasible solution:");

        while (true) {
            int negativeRow = RowWithNegativeElementInUnitColumn(table);
            if (negativeRow < 0) {
                log.Add("A basic feasible solution has been found:");
                LogRoots(table);
                return table;
            }

            int pivotCol = FindPivotColumn_BFS(table, negativeRow);
            if (pivotCol < 0) {
                log.Add("The system of constraints is inconsistent.");
                LogTable(table);
                return null;
            }

            int pivotRow = FindPivotRow_BFS(table, pivotCol);
            if (pivotRow < 0) {
                log.Add("The system of constraints is incompatible.");
                LogTable(table);
                return null;
            }

            LogSolvingElement(ref table, pivotCol, pivotRow);
        }
    }

    private static double[,]? FindMaxOptimalSolution(double[,]? table) {
        if (table is null) return null;

        {
            Ys = new string[table.GetLength(0) - 1];
            Xs = new string[table.GetLength(1) - 1];

            for (int row = 0; row < table.GetLength(0) - 1; row++)
                Ys[row] = $"y{row + 1}";

            for (int col = 0; col < table.GetLength(1) - 1; col++)
                Xs[col] = $"x{col + 1}";

            CheckSigns();
            table = FindBasicFeasibleSolution(table);
            if (table is null) return null;
        }

        log.Add("\nFinding an optimal solution:");
        while (true) {
            int pivotCol = FindPivotColumn_OS(table);
            if (pivotCol < 0) {
                log.Add("An optimal solution has been found:");
                LogRoots(table);
                return table;
            }

            int pivotRow = FindPivotRow_OS(table, pivotCol);
            if (pivotRow < 0) {
                log.Add("Problem is unlimited from above.");
                LogTable(table);
                return null;
            }

            LogSolvingElement(ref table, pivotCol, pivotRow);
        }

    }

    private static double[,]? FindMinOptimalSolution(double[,]? table) {
        if (table is null) return null;

        log.Add("Going to the problem of maximizing the objective function Z':");
        string func = string.Empty;
        for (int col = 0; col < table.GetLength(1) - 1; col++) {
            table[table.GetLength(0) - 1, col] *= -1;
            func += $"{table[table.GetLength(0) - 1, col]}*X[{col + 1}]{(col != table.GetLength(1) - 2 ? func += " + " : "")}";
        }
        log.Add($"Z' = {func} -> max");
        log.Add("Input simplex table:");

        table = FindMaxOptimalSolution(table);
        if (table is null) return null;

        table[table.GetLength(0) - 1, table.GetLength(1) - 1] *= -1;
        return table;
    }

    private static double[,] LogSolvingElement(ref double[,] table, int pivotCol, int pivotRow) {
        log.Add($"""
                The solving row: {Ys[pivotRow]}
                The solving column: {Xs[pivotCol]}
                """);
        ModifiedJordanExclusions(ref table, pivotRow, pivotCol);
        return table;
    }

    #region Basic Feasible Solution steps
    private static int RowWithNegativeElementInUnitColumn(double[,] table) {
        int desiredRow = int.MinValue;

        for (int row = 0; row < table.GetLength(0); row++) {
            if (table[row, table.GetLength(1) - 1] < 0) {
                desiredRow = row;
                break;
            }
        }
        return desiredRow;
    }

    private static int FindPivotColumn_BFS(double[,] table, int row) {
        int desiredCol = int.MinValue;

        for (int col = 0; col < table.GetLength(1); col++) {
            if (table[row, col] < 0) {
                desiredCol = col;
                break;
            }
        }
        return desiredCol;
    }

    private static int FindPivotRow_BFS(double[,] table, int col) {
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

    #region Optimal Solution steps
    private static int FindPivotColumn_OS(double[,] table) {
        int desiredCol = int.MinValue;
        for (int col = 0; col < table.GetLength(1) - 1; col++) {
            if (table[table.GetLength(0) - 1, col] < 0) {
                desiredCol = col;
                break;
            }
        }
        return desiredCol;
    }

    private static int FindPivotRow_OS(double[,] table, int col) {
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

    private static void CheckSigns() {
        for (int i = 0; i < Xs.Length; i++) {
            Xs[i] = !Xs[i].StartsWith('-') ? '-' + Xs[i] : Xs[i];
        }

        for (int i = 0; i < Ys.Length; i++) {
            Ys[i] = Ys[i].StartsWith('-') ? Ys[i][1..] : Ys[i];
        }
    }

    private static void InvertSigns(ref double[,] table) {
        for (int row = 0; row < table.GetLength(0); row++)
            for (int col = 0; col < table.GetLength(1); col++)
                table[row, col] *= -1;
    }


    #region Output
    private static void LogTable(double[,] table) {
        string result = "\n.";

        result += "".PadLeft(offset - 2);
        for (int col = 0; col < table.GetLength(1) - 1; col++) {
            result += Xs[col].PadLeft(offset) + " ";
        }
        result += "1".PadLeft(offset);

        for (int row = 0; row < table.GetLength(0); row++) {
            result += (row == table.GetLength(0) - 1 ? "\n Z = " : $"\n{Ys[row]} = ").PadRight(offset);

            for (int col = 0; col < table.GetLength(1); col++) {
                result += $"{Math.Round(table[row, col], round)}".PadLeft(offset) + " ";
            }
        }
        result += "\n";

        log.Add(result);
    }

    private static void LogRoots(double[,] table) {
        int colCount = table.GetLength(1) - 1;
        double[] roots = new double[colCount];
        string result = "X = ( ";

        for (int yi = 0; yi < Ys.Length; yi++)
            if (Ys[yi].Contains('x')) {
                int index = Ys[yi].IndexOf('x');
                _ = int.TryParse(Ys[yi][(index + 1)..], out int coefficient);
                roots[coefficient - 1] = table[yi, colCount];
            }

        for (int col = 0; col < colCount; col++) {
            result += $"{Math.Round(roots[col], Round)}{(col != colCount - 1 ? "; " : " )")}";
        }

        log.Add(result);
    }

    private static int GetLeftOffset(double[,] table, int col) {
        int maxLength = int.MinValue;

        for (int i = 0; i < table.GetLength(0); i++) {
            if ($"{Math.Round(table[i, col], Round)}".Length > maxLength) {
                maxLength = $"{Math.Round(table[i, col], Round)}".Length;
            }
        }

        return maxLength;
    }

    private static int GetLeftOffset(string[] array) {
        int maxLength = int.MinValue;

        for (int i = 0; i < array.Length - 1; i++) {
            if (array[i].Length > maxLength) {
                maxLength = array[i].Length;
            }
        }

        return maxLength;
    }
    #endregion
}
