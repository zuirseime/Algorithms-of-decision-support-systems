namespace CalculatingWork.Core.SimplexAlgorithm.Modules;
internal class OptimalSolution : Module {
    internal (Tableau, Roots[]) Max(Tableau tableau, bool dual) {
        if (tableau.Data is null) return (new Tableau(), []);
        Log.WriteLine("\nFinding an optimal solution:\n");

        while (true) {
            int pivotCol = FindPivotColumn(tableau, tableau.Height - 1);
            if (pivotCol < 0) {
                Log.WriteLine("The optimal solution has been found:", true);
                return (tableau, Designer.LogRoots(tableau, dual));
            }

            int pivotRow = FindPivotRow(tableau, pivotCol);
            if (pivotRow < 0) {
                Log.WriteLine("Problem is unlimited from above.", true);
                return (new Tableau(), []);
            }

            tableau = Designer.LogSolvingElement(tableau, pivotRow, pivotCol);
        }
    }

    internal (Tableau, Roots[]) Min(Tableau tableau, bool dual) {
        if (tableau.Data is null) return (new Tableau(), []);

        int rows = tableau.Height;
        int cols = tableau.Width;

        (tableau, Roots[] roots) = Max(tableau, dual);
        if (tableau.Data is null) return (new Tableau(), []);

        tableau.Data[rows - 1, cols - 1] *= -1;
        return (tableau, roots);
    }
}