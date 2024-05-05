using CalculatingWork.Core.MatrixGame.Models;
using CalculatingWork.Core.SimplexAlgorithm.Models;
using CalculatingWork.Core.SimplexAlgorithm;

namespace CalculatingWork.Core.MatrixGame.Modules;
internal class Strategies {
    private bool _increased = false;

    internal MGResult Pure(Matrix matrix, Pivot pivot) {
        var player1 = new Roots('1', new double[matrix.Height]);
        var player2 = new Roots('2', new double[matrix.Width]);

        player1[pivot.Position.Y] = 1;
        player2[pivot.Position.X] = 1;

        Designer.ShowStrategies("Pure", [player1, player2], pivot.Value);

        return new MGResult(player1, player2, pivot.Value);
    }

    internal MGResult Mixed(Matrix matrix) {
        RemoveNegativeValues(matrix);

        SAResult result = SolveSimpexTableau(matrix);

        var player1 = result.Dual / result.Solution;
        var player2 = result.Straight / result.Solution;
        double price = Math.Round(1 / result.Solution + (this._increased ? -1 : 0), 2);

        Designer.ShowStrategies("Mixed", [player1, player2], price);

        return new MGResult(player1, player2, price);
    }

    private static SAResult SolveSimpexTableau(Matrix matrix) {
        SA sa = new(['r', 'p'], ['q', 't'], true);

        Function func = new Function(Enumerable.Repeat(1d, matrix.Width).ToArray(), 0, 'q');
        Constraint[] constraints = new Constraint[matrix.Height];
        for (int row = 0; row < matrix.Height; row++) {
            double[] constrCoeffs = new double[matrix.Width];
            for (int col = 0; col < matrix.Width; col++) {
                constrCoeffs[col] = matrix[row, col];
            }
            double constrConst = 1;

            constraints[row] = new Constraint(constrCoeffs, constrConst, Relation.LessOrEqual, 'q');
        }

        var result = sa.Run(func, new Constraints(constraints));
        return result;
    }

    private void RemoveNegativeValues(Matrix matrix) {
        double min = matrix.FindMinimum();
        if (min < 0) {
            this._increased = true;

            for (int row = 0; row < matrix.Height; row++) {
                for (int col = 0; col < matrix.Width; col++) {
                    matrix[row, col] += Math.Abs(min);
                }
            }
        }
    }
}
