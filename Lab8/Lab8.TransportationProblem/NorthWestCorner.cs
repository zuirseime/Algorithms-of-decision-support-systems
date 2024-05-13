namespace Lab8.TransportationProblem;
public class NorthWestCorner : TP {
    protected override void FindFeasiblePlan() {
        base.FindFeasiblePlan();
        int r = 0, c = 0;
        LogTable(0, 1);

        while (true) {
            double min = Math.Min(_matrix['y', 1, r], _matrix['x', 1, c]);
            _matrix[0, r, c] = min;
            _matrix['y', 1, r] -= min;
            _matrix['x', 1, c] -= min;

            LogTable(0, 1);

            if (_matrix['y', 1, r] - min >= 0) {
                c++;
                if (c >= _matrix.Width) break;
            } else if (_matrix['x', 1, c] == 0) {
                r++;
                c++;
                if (c >= _matrix.Width & r >= _matrix.Height) break;
            } else {
                r++;
                if (r >= _matrix.Height) break;
            }
        }

        LogTable(0, 0);
        Console.WriteLine($"Preference plan cost: {FindTotalCost()}\n");
    }
}
