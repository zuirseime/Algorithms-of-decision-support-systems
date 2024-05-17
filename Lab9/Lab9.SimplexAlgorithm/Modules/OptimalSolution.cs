using Lab9.Common;
using Lab9.SimplexAlgorithm.Models;

namespace Lab9.SimplexAlgorithm.Modules;
public class OptimalSolution : Solution {
    internal Tableau Max(Tableau tableau) {
        if (tableau.Data is null) return new Tableau();
        Log.WriteLine("\nFinding an optimal solution:\n");

        while (true) {
            int pivotCol = FindPivotColumn(tableau, tableau.Height - 1);
            if (pivotCol < 0) {
                Log.WriteLine("The optimal solution has been found:", true);
                Roots = Designer.LogRoots(tableau);
                _tableau = (Tableau)tableau.Clone();
                return tableau;
            }

            int pivotRow = FindPivotRow(tableau, pivotCol);
            if (pivotRow < 0) {
                Log.WriteLine("Problem is unlimited from above.", true);
                return new Tableau();
            }

            tableau = Designer.LogSolvingElement(tableau, pivotRow, pivotCol);
        }
    }

    internal Tableau Min(Tableau tableau) {
        if (tableau.Data is null) return new Tableau();

        _tableau = Max(tableau);
        if (_tableau.Data is null) return new Tableau();

        return _tableau;
    }
}