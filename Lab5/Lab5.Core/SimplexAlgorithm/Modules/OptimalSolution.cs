using Lab5.Core.SimplexAlgorithm.Models;

namespace Lab5.Core.SimplexAlgorithm.Modules;
internal class OptimalSolution : Module {
    internal (Tableau, string) Max(Tableau tableau) {
        if (tableau.Data is null) return (new Tableau(), string.Empty);
        Log.WriteLine("\nFinding an optimal solution:\n");

        while (true) {
            int pivotCol = FindPivotColumn(tableau, tableau.Height - 1);
            if (pivotCol < 0) {
                Log.WriteLine("The optimal solution has been found:");
                return (tableau, Designer.LogRoots(tableau));
            }

            int pivotRow = FindPivotRow(tableau, pivotCol);
            if (pivotRow < 0) {
                Log.WriteLine("Problem is unlimited from above.");
                return (new Tableau(), string.Empty);
            }

            tableau = Designer.LogSolvingElement(tableau, pivotRow, pivotCol);
        }
    }

    internal (Tableau, string) Min(Tableau tableau) {
        if (tableau.Data is null) return (new Tableau(), string.Empty);

        int rows = tableau.Height;
        int cols = tableau.Width;

        (tableau, string roots) = Max(tableau);
        if (tableau.Data is null) return (new Tableau(), string.Empty);

        tableau.Data[rows - 1, cols - 1] *= -1;
        return (tableau, roots);
    }

    private int FindPivotColumn(Tableau tableau, int row) {
        if (tableau.Data is null) throw new ArgumentNullException(nameof(tableau.Data));

        for (int col = 0; col < tableau.Width - 1; col++) {
            if (Math.Round(tableau[row, col], 2) == 0) continue;
            if (tableau[row, col] < 0) return col;
        }
        return int.MinValue;
    }
}
