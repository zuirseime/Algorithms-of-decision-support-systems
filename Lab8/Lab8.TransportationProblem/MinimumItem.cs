namespace Lab8.TransportationProblem;
public class MinimumItem : TP {
    protected override void FindReferencePlan() {
        while (true) {
            Console.WriteLine(_matrix);

            if (_matrix.Rows.All(v => v == 0) && _matrix.Cols.All(v => v == 0)) break;

            int rMin = 0, cMin = 0;
            double minCost = double.MaxValue;

            for (int r = 0; r < _matrix.Height; r++) {
                for (int c = 0; c < _matrix.Width; c++) {
                    if (_matrix.Costs[r, c] < minCost && _matrix[r, c] == 0 && HaveToFill(r, c)) {
                        rMin = r;
                        cMin = c;
                        minCost = _matrix.Costs[r, c];
                    }
                }
            }

            double min = Math.Min(_matrix.Rows[rMin], _matrix.Cols[cMin]);
            _matrix[rMin, cMin] = min;

            if (_matrix.Cols[cMin] - min >= 0) {
                _matrix.Cols[cMin] -= min;
            }
            if (_matrix.Rows[rMin] - min >= 0) {
                _matrix.Rows[rMin] -= min;
            }
        }
    }

    private bool HaveToFill(int row, int col) {
        return _matrix.Rows[row] > 0 && _matrix.Cols[col] > 0;
    }
}
