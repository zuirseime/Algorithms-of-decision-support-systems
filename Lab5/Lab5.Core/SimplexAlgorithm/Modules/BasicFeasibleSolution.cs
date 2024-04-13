using Lab5.Core.SimplexAlgorithm.Models;

namespace Lab5.Core.SimplexAlgorithm.Modules;
internal class BasicFeasibleSolution : Module {
    internal (Tableau, string) Find(Tableau tableau) {
        if (tableau.Data is null) return (new Tableau(), string.Empty);

        Log.WriteLine("\nFinding a basic feasible solution:\n");

        while (true) {
            int negativeRow = RowWithNegativeElementInUnitColumn(tableau);
            if (negativeRow < 0) {
                Log.WriteLine("A basic feasible solution has been found:");
                //_result.Roots = Designer.LogRoots(tableau);
                return (tableau, Designer.LogRoots(tableau));
            }

            int pivotCol = FindPivotColumn(tableau, negativeRow);
            if (pivotCol < 0) {
                Log.WriteLine("The system of constraints is inconsistent.");
                return (new Tableau(), string.Empty);
            }

            int pivotRow = FindPivotRow(tableau, pivotCol);
            if (pivotRow < 0) {
                Log.WriteLine("The system of constraints is incompatible.");
                return (new Tableau(), string.Empty);
            }

            tableau = Designer.LogSolvingElement(tableau, pivotRow, pivotCol);
        }
    }

    private int RowWithNegativeElementInUnitColumn(Tableau tableau) {
        if (tableau.Data is null) throw new ArgumentNullException(nameof(tableau.Data));

        for (int row = 0; row < tableau.Height - 1; row++)
            if (tableau.Data[row, tableau.Width - 1] < 0) return row;
        return int.MinValue;
    }

    private int FindPivotColumn(Tableau tableau, int row) {
        if (tableau.Data is null) throw new ArgumentNullException(nameof(tableau.Data));

        for (int col = 0; col < tableau.Height - 1; col++)
            if (tableau.Data[row, col] < 0) return col;
        return int.MinValue;
    }
}
