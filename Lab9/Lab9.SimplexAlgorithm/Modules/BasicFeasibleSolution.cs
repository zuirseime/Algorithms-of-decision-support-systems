using Lab9.Common;
using Lab9.SimplexAlgorithm.Models;

namespace Lab9.SimplexAlgorithm.Modules;
public class BasicFeasibleSolution : Solution {
    internal Tableau Find(Tableau tableau) {
        if (tableau.Data is null) return new Tableau();

        Log.WriteLine("Finding a basic feasible solution:");

        while (true) {
            int negativeRow = RowWithNegativeElementInUnitColumn(tableau);
            if (negativeRow < 0) {
                Log.WriteLine("\nThe basic feasible solution has been found:", true);
                Roots = Designer.LogRoots(tableau);
                _tableau = (Tableau)tableau.Clone();
                return tableau;
            }

            int pivotCol = FindPivotColumn(tableau, negativeRow);
            if (pivotCol < 0) {
                Log.WriteLine("The system of constraints is inconsistent.", true);
                return new Tableau();
            }

            int pivotRow = FindPivotRow(tableau, pivotCol);
            if (pivotRow < 0) {
                Log.WriteLine("The system of constraints is incompatible.", true);
                return new Tableau();
            }

            tableau = Designer.LogSolvingElement(tableau, pivotRow, pivotCol);
        }
    }

    private int RowWithNegativeElementInUnitColumn(Tableau tableau) {
        if (tableau.Data is null)
            throw new ArgumentNullException("Could not find a row with negative element in the unit column because the tableau is empty.");

        for (int row = 0; row < tableau.Height - 1; row++)
            if (tableau.Data[row, tableau.Width - 1] < 0) return row;
        return int.MinValue;
    }
}