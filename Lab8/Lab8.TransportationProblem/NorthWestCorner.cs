using Lab8.Common;

namespace Lab8.TransportationProblem;
public class NorthWestCorner : TP {
        protected override void FindReferencePlan() {
        int r = 0, c = 0;
        Console.WriteLine(_matrix);

        while (true) {
            double min = Math.Min(_matrix.Rows[r], _matrix.Cols[c]);
            _matrix[r, c] = min;
            _matrix.Rows[r] -= min;
            _matrix.Cols[c] -= min;

            Console.WriteLine(_matrix);

            if (_matrix.Rows[r] - min >= 0) {
                c++;
                if (c >= _matrix.Width) break;
            } else if (_matrix.Cols[c] == 0) {
                r++;
                c++;
                if (c >= _matrix.Width & r >= _matrix.Height) break;
            } else {
                r++;
                if (r >= _matrix.Height) break;
            }
        }
    }
}
