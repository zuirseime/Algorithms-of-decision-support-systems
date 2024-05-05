using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace CalculatingWork.Core.MatrixGame.Models;
internal struct Pivot(Point position, double value) {
    internal Point Position { get; set; } = position;
    internal double Value { get; set; } = value;

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Pivot pivot && Equals(pivot);

    public bool Equals(Pivot other) => this.Position.Equals(other.Position) && this.Value.Equals(other.Value);

    public override int GetHashCode() => HashCode.Combine(Position, Value);

    internal static bool Find(Matrix matrix, out Pivot result) {
        result = new Pivot(Point.Empty, double.NaN);

        var maxmin = Pivot.MaxMin(matrix);
        var minmax = Pivot.MinMax(matrix);

        if (!maxmin.Equals(minmax)) return false;

        result = maxmin;
        return true;
    }

    private static Pivot MaxMin(Matrix matrix) {
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

        var value = mins.MaxBy(m => m.Value);
        Designer.ShowGamePrice("minimum", value);
        return value;
    }

    private static Pivot MinMax(Matrix matrix) {
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

        var value = maxes.MinBy(m => m.Value);
        Designer.ShowGamePrice("maximum", value);
        return value;
    }
}