namespace CalculatingWork.Core.SimplexAlgorithm.Modules;
internal class ZeroRows : Module {
    internal Tableau Remove(Tableau tableau) {
        Log.WriteLine("\nRemoving zero-rows in the simplex tableau:\n");
        if (tableau.Data is null) return new Tableau();

        while (true) {
            int zeroRow = Array.FindIndex(tableau.Rows, h => h.Contains('0'));
            if (zeroRow < 0) {
                Log.WriteLine("The simplex tableau doesn't have zero-rows.\n");
                return tableau;
            }

            int pivotCol = this.ColumnWithPositiveElementInZeroRow(tableau, zeroRow);
            if (pivotCol < 0) {
                Log.WriteLine("The system of constraints is inconsistent.");
                return new Tableau();
            }

            int pivotRow = this.FindPivotRow(tableau, pivotCol);
            bool swappedZero = tableau.Rows[pivotRow].Contains('0');

            tableau = Designer.LogSolvingElement(tableau, pivotRow, pivotCol, !swappedZero);
            
            if (swappedZero) {
                tableau = this.CropTable(tableau, pivotCol);
                Designer.LogTableau(tableau);
            }
        }
    }

    private int ColumnWithPositiveElementInZeroRow(Tableau tableau, int row) {
        if (tableau.Data is null) throw new ArgumentNullException(nameof(tableau.Data));

        for (int col = 0; col < tableau.Width - 1; col++)
            if (tableau.Data[row, col] > 0) return col;
        return int.MinValue;
    }

    private Tableau CropTable(Tableau tableau, int pivotCol) {
        if (tableau.Data is null) throw new ArgumentNullException(nameof(tableau.Data));

        int rows = tableau.Height;
        int cols = tableau.Width;

        double[,] cropped = new double[rows, cols - 1];
        string[] croppedColHeader = new string[cols - 1];

        for (int row = 0; row < rows; row++) {
            int croppedCol = 0;
            for (int col = 0; col < cols; col++) {
                if (col == pivotCol) continue;

                cropped[row, croppedCol] = tableau.Data[row, col];
                croppedColHeader[croppedCol++] = tableau.Columns[col];
            }
        }

        tableau.Data = cropped;
        tableau.Columns = croppedColHeader;
        return tableau;
    }
}
