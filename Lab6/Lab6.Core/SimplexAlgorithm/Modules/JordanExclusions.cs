using Lab6.Core.SimplexAlgorithm.Models;

namespace Lab6.Core.SimplexAlgorithm.Modules;
internal static class JordanExclusions {
    internal static Tableau Modified(Tableau tableau, int row, int col) {
        if (tableau.Data is null) throw new ArgumentNullException(nameof(tableau.Data));

        double[,] temp = (double[,])tableau.Data.Clone();

        tableau.Data[row, col] = 1;
        for (int i = 0; i < tableau.Height; i++) {
            if (i != row)
                tableau.Data[i, col] *= -1;

            for (int j = 0; j < tableau.Width; j++) {
                if (i != row && j != col)
                    tableau.Data[i, j] = temp[i, j] * temp[row, col] - temp[i, col] * temp[row, j];

                tableau.Data[i, j] /= temp[row, col];
            }
        }

        return tableau;
    }
}