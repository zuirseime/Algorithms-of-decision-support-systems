using CalculatingWork.Core.SimplexAlgorithm.Models;
using CalculatingWork.Core.SimplexAlgorithm.Modules;
using System.Text.RegularExpressions;

namespace CalculatingWork.Core.SimplexAlgorithm;
internal static class Designer {
    internal static void StraightProblemDefinition(Function func, Constraints constraints) {
        Log.WriteLine($"Straight problem definition:\nZ = {func}\n" +
                      $"with constraints:\n{string.Join('\n', constraints.Data.AsEnumerable())}", true);
    }

    internal static void DualProblemDefinition(Tableau tableau, bool max) {
        int rows = tableau.Height;
        int cols = tableau.Width;

        double[] funcCoeffs = new double[cols - 1];
        for (int col = 0; col < cols - 1; col++) {
            funcCoeffs[col] = tableau[rows - 1, col] * (!max ? -1 : 1);
        }
        double funcConst = tableau[rows - 1, cols - 1] * (!max ? -1 : 1);
        Function func = new(funcCoeffs, funcConst, tableau.ColVars[1], !max);

        Constraint[] constraints = new Constraint[rows - 1];
        for (int row = 0; row < rows - 1; row++) {
            double[] constrCoeffs = new double[cols - 1];
            for (int col = 0; col < cols - 1; col++) {
                constrCoeffs[col] = tableau[row, col];
            }
            double constrConst = -tableau[row, cols - 1];

            constraints[row] = new Constraint(constrCoeffs, constrConst, Relation.GreaterOrEqual, tableau.ColVars[1]);
        }

        Log.WriteLine($"\nDual problem definition:\nW = {func}\n" +
                      $"with constraints:\n{string.Join('\n', constraints.AsEnumerable())}", true);

        if (tableau.Columns.Any(c => c.Contains('0'))) {
            Log.WriteLine("Free variables: " + string.Join(", ", Enumerable.Range(0, cols)
                .Where(col => tableau.Columns[col].Contains('0'))
                .Select(col => $"{tableau.ColVars[1]}{col + 1}")), true);
        }
    }

    internal static void MinToMax(Tableau tableau, Function function) {
        Log.WriteLine("\nGoing to the problem of maximizing the objective function Z':");

        function.Invert();
        Log.WriteLine($"Z' = {function}");
    }

    internal static void ShowInputTableau(Tableau tableau, bool max, bool dual) {
        Log.WriteLine("\nInput simplex tableau:");
        Designer.LogTableau(tableau);

        if (dual) Designer.DualProblemDefinition(Tableau.Transpose(tableau), max);
    }

    internal static void LogTableau(Tableau tableau) => Log.WriteLine(tableau);

    internal static void ShowSolution(double value, bool max, bool dual) {
        Log.WriteLine($"\n{(max ? "Max" : "Min")} (Z) = {(dual ? $"{(!max ? "Max" : "Min")} (W) = " : "")}{Globals.Round(value)}\n", true);
    }

    internal static Roots[] LogRoots(Tableau tableau, bool isDual) {
        Roots dual = Roots.Empty;

        Roots straight = Designer.LogStraightRoots(tableau);
        if (isDual) dual = Designer.LogDualRoots(tableau);

        return [straight, dual];
    }

    internal static Roots LogStraightRoots(Tableau tableau) {
        int cols = tableau.Width - 1;
        double[] roots = new double[tableau.Order];

        for (int row = 0; row < tableau.Rows.Length; row++)
            if (tableau.Rows[row].Contains($"{tableau.ColVars[0]}")) {
                int coefficient = Designer.GetCoefficient(tableau.Rows[row], $"{tableau.ColVars[0]}");
                roots[coefficient - 1] = tableau[row, cols];
            }

        Roots xs = new Roots(SA.ColVars[0], roots);
        Log.WriteLine(xs.ToString(true), true);
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

        Roots us = new Roots(SA.ColVars[1], roots);
        Log.WriteLine(us.ToString(true), true);
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
