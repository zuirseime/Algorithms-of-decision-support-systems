using Lab9.Common;
using Lab9.SimplexAlgorithm.Models;
using Lab9.SimplexAlgorithm.Modules;
using System.Text.RegularExpressions;

namespace Lab9.SimplexAlgorithm;
internal static class Designer {
    internal static void ProblemDefinition(Function func, Constraints constraints) {
        Log.WriteLine($"Straight problem definition:\nZ = {func}\n" +
                      $"with constraints:\n{string.Join('\n', constraints.Data.AsEnumerable())}", true);
    }

    internal static void MinToMax(Function function) {
        Log.WriteLine("\nGoing to the problem of maximizing the objective function Z':");

        function.Invert();
        Log.WriteLine($"Z' = {function}");
    }

    internal static void ShowInputTableau(Tableau tableau) {
        Log.WriteLine("\nInput simplex tableau:");
        LogTableau(tableau);
    }

    internal static void LogTableau(Tableau tableau) => Log.WriteLine(tableau);

    internal static void ShowSolution(double value, bool max) {
        Log.WriteLine($"\n{(max ? "Max" : "Min")} (Z) = {Globals.Round(value)}\n", true);
    }

    internal static Roots LogRoots(Tableau tableau) => LogStraightRoots(tableau);

    internal static Roots LogStraightRoots(Tableau tableau) {
        int cols = tableau.Width - 1;
        double[] roots = new double[tableau.Order];

        for (int row = 0; row < tableau.Rows.Length; row++)
            if (tableau.Rows[row].Contains($"{tableau.ColVars[0]}")) {
                int coefficient = GetCoefficient(tableau.Rows[row], $"{tableau.ColVars[0]}");
                roots[coefficient - 1] = tableau[row, cols];
            }

        Roots xs = new Roots(SA.ColVars[0], roots);
        Log.WriteLine(xs.ToString(true), true);
        return xs;
    }

    private static int GetCoefficient(string source, string variable) =>
        int.Parse(Regex.Match(source, @$"((?<={variable})\d+)").Value);

    internal static Tableau LogSolvingElement(Tableau tableau, int row, int col, bool logging = true) {
        Log.WriteLine($"The solving element position: [{tableau.Rows[row]}; {tableau.Columns[col]}]");
        var newTableau = JordanExclusions.Modified(tableau, row, col);

        if (row != tableau.Height - 1 && col != tableau.Width - 1)
            (tableau.Columns[col], tableau.Rows[row]) = (tableau.Rows[row], tableau.Columns[col]);
        tableau.FixHeaders();

        if (logging) LogTableau(tableau);
        return newTableau;
    }
}
