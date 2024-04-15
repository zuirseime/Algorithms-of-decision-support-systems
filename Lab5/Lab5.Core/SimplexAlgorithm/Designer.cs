using Lab5.Core.SimplexAlgorithm.Input;
using Lab5.Core.SimplexAlgorithm.Models;
using Lab5.Core.SimplexAlgorithm.Modules;
using System;

namespace Lab5.Core.SimplexAlgorithm;
internal static class Designer {
    internal static void StraightProblemDefinition(Function func, Constraint[] constraints, bool max) {
        Log.WriteLine($"Straight problem definition:\nZ = {func} -> {(max ? "max" : "min")}\n" +
                      $"with constraints:\n{string.Join('\n', constraints.AsEnumerable())}", true);
    }

    internal static void DualProblemDefinition(Tableau tableau, bool max) {
        int rows = tableau.Height;
        int cols = tableau.Width;

        double[] funcCoeffs = new double[cols - 1];
        for (int col = 0; col < cols - 1; col++) {
            funcCoeffs[col] = tableau[rows - 1, col] * (!max ? -1 : 1);
        }
        double funcConst = tableau[rows - 1, cols - 1] * (!max ? -1 : 1);
        Function func = new(funcCoeffs, funcConst, 'u');

        Constraint[] constraints = new Constraint[rows - 1];
        for (int row = 0; row < rows - 1; row++) {
            double[] constrCoeffs = new double[cols - 1];
            for (int col = 0; col < cols - 1; col++) {
                constrCoeffs[col] = tableau[row, col];
            }
            double constrConst = -tableau[row, cols - 1];

            constraints[row] = new Constraint(constrCoeffs, constrConst, Relation.GreaterOrEqual, 'u');
        }

        Log.WriteLine($"\nDual problem definition:\nW = {func} -> {(max ? "min" : "max")}\n" +
                      $"with constraints:\n{string.Join('\n', constraints.AsEnumerable())}", true);

        if (tableau.Columns.Any(c => c.Contains('0'))) {
            Log.WriteLine("Free variables: " + string.Join(", ", Enumerable.Range(0, cols)
                .Where(col => tableau.Columns[col].Contains('0'))
                .Select(col => $"u{col + 1}")), true);
        }
    }

    internal static void MinToMax(Tableau tableau) {
        Log.WriteLine("\nGoing to the problem of maximizing the objective function Z':");

        int rows = tableau.Height;
        int cols = tableau.Width;

        string funcStr = string.Empty;
        for (int col = 0; col < cols - 1; col++) {
            tableau[rows - 1, col] = tableau[rows - 1, col] *= -1;
            funcStr += $"{(tableau[rows - 1, col] >= 0
                ? tableau[rows - 1, col]
                : -tableau[rows - 1, col])}*X[{col + 1}]{(col != cols - 2
                ? tableau[rows - 1, col] >= 0 ? " + "
                : " - " : "")}";
        }
        Log.WriteLine($"Z' = {funcStr} -> max");
    }

    internal static void ShowInputTableaus(Tableau tableau, bool max) {
        Log.WriteLine("\nInput simplex tableau:");
        Designer.LogTableau(tableau);

        Designer.DualProblemDefinition(Tableau.Transpose(tableau), max);
    }

    internal static void LogTableau(Tableau tableau) => Log.WriteLine(tableau);

    internal static void ShowSolution(double value, bool max) {
        Log.WriteLine($"\n{(max ? "Max" : "Min")} (Z) = {value}" +
                      $"\n{(!max ? "Max" : "Min")} (W) = {value}", true);
    }

    internal static string LogRoots(Tableau tableau) {
        string straight = Designer.LogStraightRoots(tableau);
        string dual = Designer.LogDualRoots(tableau);
        return straight;
    }

    internal static string LogStraightRoots(Tableau tableau) {
        int cols = tableau.Width - 1;
        double[] roots = new double[tableau.Order];
        string result = "X = ( ";

        for (int row = 0; row < tableau.Rows.Length; row++)
            if (tableau.Rows[row].Contains('x')) {
                int coefficient = Designer.GetCoefficient(tableau.Rows[row], 'x');
                roots[coefficient - 1] = tableau[row, cols];
            }

        for (int col = 0; col < tableau.Order; col++)
            result += $"{Math.Round(roots[col], tableau.Round)}{(col != tableau.Order - 1 ? "; " : " )")}";

        Log.WriteLine(result, true);
        return result[4..];
    }

    internal static string LogDualRoots(Tableau tableau) {
        int rows = tableau.Height - 1;
        double[] roots = new double[rows];
        string result = "U = ( ";

        for (int col = 0; col < tableau.Columns.Length; col++)
            if (tableau.Columns[col].Contains('u')) {
                int coefficient = Designer.GetCoefficient(tableau.Columns[col], 'u');
                roots[coefficient - 1] = tableau[rows, col];
            }

        for (int col = 0; col < rows; col++)
            result += $"{Math.Round(roots[col], tableau.Round)}{(col != rows - 1 ? "; " : " )")}";

        Log.WriteLine(result, true);
        return result[4..];
    }

    private static int GetCoefficient(string source, char variable) =>
        int.Parse(Regex.Match(source, @$"((?<={variable})\d+)").Value);

    internal static Tableau LogSolvingElement(Tableau tableau, int row, int col, bool logging = true) {
        Log.WriteLine($"The solving element position: [{tableau.Rows[row]}; {tableau.Columns[col]}]");
        var newTableau = JordanExclusions.Modified(tableau, row, col);

        if (row != tableau.Height - 1 && col != tableau.Width - 1)
            (tableau.Columns[col], tableau.Rows[row]) = (tableau.Rows[row], tableau.Columns[col]);
        tableau.FixHeaders();

        if (logging) Designer.LogTableau(tableau);
        return newTableau;
    }
}
