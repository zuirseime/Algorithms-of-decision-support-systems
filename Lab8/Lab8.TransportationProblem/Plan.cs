using Lab8.Common;

namespace Lab8.TransportationProblem;
public abstract class Plan {
    protected Matrix _matrix;

    internal double Solution { 
        get {
            double sum = 0;
            for (int r = 0; r < _matrix.Height; r++) {
                for (int c = 0; c < _matrix.Width; c++) {
                    sum += _matrix[0, r, c] * _matrix[1, r, c];
                }
            }
            return sum;
        }
    }
    internal Matrix Matrix => _matrix;

    internal virtual void Find(Matrix matrix) => _matrix = matrix;

    protected void SaveMatrix() => _matrix = (Matrix)_matrix.Clone();

    protected void LogTable(int contentLayer, int headerLayer) {
        Log.WriteLine(_matrix.ToString(contentLayer, headerLayer));
    }
}
