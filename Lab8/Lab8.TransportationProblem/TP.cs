using Lab8.Common;

namespace Lab8.TransportationProblem;

public abstract class TP {
    protected Matrix _matrix;
    protected TPResult _result;

    public TPResult Run(string matrix, string inputs, string outputs) {
        try {
            return Run(Matrix.Parse(matrix), StringToArray(inputs), StringToArray(outputs));
        } catch (Exception ex) {
            return _result;
        }
    }

    public TPResult Run(Matrix matrix, double[] inputs, double[] outputs) {
        this._matrix = matrix;

        try {
            this._matrix.Load(outputs, inputs);
        } catch (ArgumentException ex) {

        }

        FindReferencePlan();

        return _result;
    }

    protected virtual void FindReferencePlan() {
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

    private void FindOptimalPlan() {
        throw new NotImplementedException();
    }

    private void FindTotalCost() {
        throw new NotImplementedException();
    }

    protected static double[] StringToArray(string text) 
        => text.Split().Select(double.Parse).ToArray();
}

