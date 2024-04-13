using Lab5.Core.SimplexAlgorithm.Models;

namespace Lab5.Core.SimplexAlgorithm.Modules;
internal class Module {
    protected int FindPivotRow(Tableau tableau, int col) {
        if (tableau.Data is null) throw new ArgumentNullException(nameof(tableau.Data));

        double min = double.MaxValue;
        int desiredRow = int.MinValue;

        for (int row = 0; row < tableau.Height - 1; row++) {
            if (tableau.Data[row, tableau.Width - 1] == 0 && tableau.Data[row, col] < 0)
                continue;

            if (tableau.Data[row, col] != 0) {
                double ratio = tableau.Data[row, tableau.Width - 1] / tableau.Data[row, col];
                if (ratio >= 0 && ratio < min) {
                    min = ratio;
                    desiredRow = row;
                }
            }
        }

        return desiredRow;
    }
}
