using Lab6.Core.Output;
using Lab6.Core.SimplexAlgorithm;
using Lab6.Core.SimplexAlgorithm.Models;
using System.Drawing;

namespace Lab6.Core.MatrixGame;

public class MG {
    private bool _inceased = false;

    public static Player Player1;
    public static Player Player2;

    public MGResult Run(string matrixStr) {
        if (!Matrix.TryParse(matrixStr, out Matrix matrix))
            throw new ArgumentException(nameof(matrixStr));

        if (this.FindPivotPoint(matrix, out Pivot pivot)) {
            Log.WriteLine("Pivot point is found.\nPure strategies:\n");
            return this.FindPureStrategies(matrix, pivot);
        } else {
            Log.WriteLine("Pivot point is not found.\n");
            return SolveBySimplexAlgorithm(matrix);
        }
    }

    private MGResult FindPureStrategies(Matrix data, Pivot pivot) {
        MG.Player1 = new Player(new double[data.Height]);
        MG.Player2 = new Player(new double[data.Width]);

        MG.Player1[pivot.Position.Y] = 1;
        MG.Player2[pivot.Position.X] = 1;

        Log.WriteLine("Player 1: " + MG.Player1);
        Log.WriteLine("Player 2: " + MG.Player2);
        Log.WriteLine("Game price: " + pivot.Value);

        return new MGResult(MG.Player1, MG.Player2, pivot.Value);
    }

    private MGResult SolveBySimplexAlgorithm(Matrix matrix) {
        SA sa = new(['Z', 'W'], ['r', 'p'], ['q', 't']);

        double min = matrix.FindMinimum();
        if (min < 0) {
            this._inceased = true;

            for (int row = 0; row < matrix.Height; row++) {
                for (int col = 0; col < matrix.Width; col++) {
                    matrix[row, col] += Math.Abs(min);
                }
            }
        }

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

        var result = sa.Run(func, constraints, true);

        var player1 = MG.Player1 / result.Solution;
        var player2 = MG.Player2 / result.Solution;
        double price = Math.Round(1 / result.Solution + (this._inceased ? -1 : 0), 2);

        Log.WriteLine("\nMixed strategies:\n");
        Log.WriteLine("Player 1: " + player1);
        Log.WriteLine("Player 2: " + player2);
        Log.WriteLine("Game price: " + price);

        return new MGResult(player1, player2, price);
    }

    private bool FindPivotPoint(Matrix matrix, out Pivot pivot) {
        pivot = new Pivot(Point.Empty, double.NaN);

        Pivot maxmin = this.MaxMin(matrix);
        Pivot minmax = this.MinMax(matrix);

        if (!maxmin.Equals(minmax)) return false;

        pivot = maxmin;
        return true;
    }

    private Pivot MaxMin(Matrix matrix) {
        Pivot[] mins = new Pivot[matrix.Height];

        for (int row = 0; row < matrix.Height; row++) {
            double min = matrix[row, 0];
            Point minPos = new(0, row);

            for (int col = 1; col < matrix.Width; col++) {
                if (matrix[row, col] < min) {
                    min = matrix[row, col];
                    minPos = new Point(col, row);
                }
            }
            mins[row] = new Pivot(minPos, min);
        }

        return mins.MaxBy(m => m.Value);
    }

    private Pivot MinMax(Matrix matrix) {
        Pivot[] maxes = new Pivot[matrix.Width];

        for (int col = 0; col < matrix.Width; col++) {
            double max = matrix[0, col];
            Point maxPos = new(col, 0);

            for (int row = 1; row < matrix.Height; row++) {
                if (matrix[row, col] > max) {
                    max = matrix[row, col];
                    maxPos = new Point(col, row);
                }
            }
            maxes[col] = new Pivot(maxPos, max);
        }

        return maxes.MinBy(m => m.Value);
    }
}
