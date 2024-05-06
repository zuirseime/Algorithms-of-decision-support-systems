namespace Lab8.TransportationProblem;
public class MinimumItem : TP {
    protected override void FindReferencePlan() {
        base.FindReferencePlan();
        while (true) {
            Console.WriteLine(_matrix);

            if (_matrix['y', 1].All(v => v == 0) && _matrix['x', 1].All(v => v == 0)) break;

            int rMin = 0, cMin = 0;
            double minCost = double.MaxValue;

            for (int r = 0; r < _matrix.Height; r++) {
                for (int c = 0; c < _matrix.Width; c++) {
                    if (_matrix[1, r, c] < minCost && _matrix[0, r, c] == 0 && HaveToFill(r, c)) {
                        rMin = r;
                        cMin = c;
                        minCost = _matrix[1, r, c];
                    }
                }
            }

            double min = Math.Min(_matrix['y', 1, rMin], _matrix['x', 1, cMin]);
            _matrix[0, rMin, cMin] = min;

            if (_matrix['x', 1, cMin] - min >= 0) {
                _matrix['x', 1, cMin] -= min;
            }
            if (_matrix['y', 1, rMin] - min >= 0) {
                _matrix['y', 1, rMin] -= min;
            }
        }

        Console.WriteLine($"Preference plan cost: {FindTotalCost()}\n");
    }

    private bool HaveToFill(int row, int col) {
        return _matrix['y', 1, row] > 0 && _matrix['x', 1, col] > 0;
    }
}
