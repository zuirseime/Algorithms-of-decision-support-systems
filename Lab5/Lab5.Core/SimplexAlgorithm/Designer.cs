using Lab5.Core.SimplexAlgorithm.Models;
using Lab5.Core.SimplexAlgorithm.Modules;

namespace Lab5.Core.SimplexAlgorithm;
internal static class Designer {

    internal static void LogTable(Tableau tableau) => Log.WriteLine(tableau.ToString());

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
                _ = int.TryParse(Regex.Match(tableau.Rows[row], @"((?<=x)\d+)").Value, out int coefficient);
                roots[coefficient - 1] = tableau[row, cols];
            }

        for (int col = 0; col < tableau.Order; col++)
            result += $"{Math.Round(roots[col], tableau.Round)}{(col != tableau.Order - 1 ? "; " : " )")}";

        Log.WriteLine(result);
        return result[4..];
    }

    internal static string LogDualRoots(Tableau tableau) {
        int rows = tableau.Height - 1;
        double[] roots = new double[rows];
        string result = "U = ( ";

        for (int col = 0; col < tableau.Columns.Length; col++)
            if (tableau.Columns[col].Contains('u')) {
                _ = int.TryParse(Regex.Match(tableau.Columns[col], @"((?<=u)\d+)").Value, out int coefficient);
                roots[coefficient - 1] = tableau[rows, col];
            }

        for (int col = 0; col < rows; col++)
            result += $"{Math.Round(roots[col], tableau.Round)}{(col != rows - 1 ? "; " : " )")}";

        Log.WriteLine(result);
        return result[4..];
    }

    internal static Tableau LogSolvingElement(Tableau tableau, int row, int col, bool logging = true) {
        Log.WriteLine($"The solving element position: [{tableau.Rows[row]}; {tableau.Columns[col]}]");
        var newTableau = JordanExclusions.Modified(tableau, row, col);

        if (row != tableau.Height - 1 && col != tableau.Width - 1)
            (tableau.Columns[col], tableau.Rows[row]) = (tableau.Rows[row], tableau.Columns[col]);
        tableau.FixHeaders();

        if (logging) Designer.LogTable(tableau);
        return newTableau;
    }
}
