using Lab6.Core.MatrixGame;
using Lab6.Core.Output;
using Lab6.Core.SimplexAlgorithm.Models;
using Lab6.Core.SimplexAlgorithm.Modules;
using System.Text.RegularExpressions;

namespace Lab6.Core.SimplexAlgorithm;
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
        Function func = new(funcCoeffs, funcConst, tableau.ColVars[1]);

        Constraint[] constraints = new Constraint[rows - 1];
        for (int row = 0; row < rows - 1; row++) {
            double[] constrCoeffs = new double[cols - 1];
            for (int col = 0; col < cols - 1; col++) {
                constrCoeffs[col] = tableau[row, col];
            }
            double constrConst = -tableau[row, cols - 1];

            constraints[row] = new Constraint(constrCoeffs, constrConst, Relation.GreaterOrEqual, tableau.ColVars[1]);
        }

        Log.WriteLine($"\nDual problem definition:\nW = {func} -> {(max ? "min" : "max")}\n" +
                      $"with constraints:\n{string.Join('\n', constraints.AsEnumerable())}", true);

        if (tableau.Columns.Any(c => c.Contains('0'))) {
            Log.WriteLine("Free variables: " + string.Join(", ", Enumerable.Range(0, cols)
                .Where(col => tableau.Columns[col].Contains('0'))
                .Select(col => $"{tableau.ColVars[1]}{col + 1}")), true);
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
                : -tableau[rows - 1, col])}*{$"{tableau.ColVars[0]}".ToUpper()}[{col + 1}]{(col != cols - 2
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
        Log.WriteLine($"\n{(max ? "Max" : "Min")} (Z) = {(!max ? "Max" : "Min")} (W) = {value}", true);
    }

    internal static string LogRoots(Tableau tableau) {
        Roots straight = Designer.LogStraightRoots(tableau);
        Roots dual = Designer.LogDualRoots(tableau);

        MG.Player1 = new Player(dual.Values);
        MG.Player2 = new Player(straight.Values);

        return straight.ToString();
    }

    internal static Roots LogStraightRoots(Tableau tableau) {
        int cols = tableau.Width - 1;
        double[] roots = new double[tableau.Order];

        for (int row = 0; row < tableau.Rows.Length; row++)
            if (tableau.Rows[row].Contains($"{tableau.ColVars[0]}")) {
                int coefficient = Designer.GetCoefficient(tableau.Rows[row], $"{tableau.ColVars[0]}");
                roots[coefficient - 1] = tableau[row, cols];
            }

        Roots xs = new Roots('X', roots);
        Log.WriteLine(xs, true);
        return xs;
    }

    internal static Roots LogDualRoots(Tableau tableau) {
        int rows = tableau.Height - 1;
        double[] roots = new double[rows];

        for (int col = 0; col < tableau.Columns.Length; col++)
            if (tableau.Columns[col].Contains($"{tableau.RowVars[1]}")) {
                int coefficient = Designer.GetCoefficient(tableau.Columns[col], $"{tableau.RowVars[1]}");
                roots[coefficient - 1] = tableau[rows, col];
            }

        Roots us = new Roots('U', roots);
        Log.WriteLine(us, true);
        return us;
    }

    private static int GetCoefficient(string source, string variable) =>
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