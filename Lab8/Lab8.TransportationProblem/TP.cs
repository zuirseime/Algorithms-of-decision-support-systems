using Lab8.Common;
using System.Drawing;

namespace Lab8.TransportationProblem;

public abstract class TP {
    protected Matrix _matrix;
    protected TPResult _result;

    public TPResult Run(string matrix, string inputs, string outputs) {
        return Run(Matrix.Parse(matrix), StringToArray(inputs), StringToArray(outputs));
    }

    public TPResult Run(Matrix matrix, double[] inputs, double[] outputs) {
        _matrix = matrix;

        try {
            _matrix.Load(outputs, inputs);
        } catch {

        }

        FindFeasiblePlan();
        FindOptimalPlan();

        return _result;
    }

    protected virtual void FindFeasiblePlan() {
        Console.WriteLine("Finding feasible plan:\n");
    }

    private void FindOptimalPlan() {
        Console.WriteLine("Finding optimal plan:\n");

        while (true) {
            FindPotentials();
            FindIndirectCosts();

            if (IsOptimal(out Point coord)) {
                Console.WriteLine("The optimal condition is satisfied.\n" +
                                  "The optimal transportation plan was found:\n");
                LogTable(0, 0);
                break;
            }

            Console.WriteLine("The optimal condition is not satisfied.\n" +
                             $"Detected a bad cell: [{coord.Y}, {coord.X}]\n");
            _matrix[0, coord.Y, coord.X] = double.Epsilon;
            var loop = SearchLoop(coord);
            RecalculatePlan(loop);
        }
    }

    private void FindPotentials() {
        Console.WriteLine("Finding potentials:\n");
        _matrix.FillPotentialsWithNaN(_matrix.Height, _matrix.Width);

        _matrix['y', 2, 0] = 0;
        int r = 0, c = 0;
        while (true) {
            if (_matrix[0, r, c] != 0) {
                if (double.IsNaN(_matrix['x', 2, c]) && !double.IsNaN(_matrix['y', 2, r])) {
                    _matrix['x', 2, c] = _matrix[1, r, c] - _matrix['y', 2, r];
                }
                if (double.IsNaN(_matrix['y', 2, r]) && !double.IsNaN(_matrix['x', 2, c])) {
                    _matrix['y', 2, r] = _matrix[1, r, c] - _matrix['x', 2, c];
                }
            }

            c++;
            if (c >= _matrix.Width) {
                c = 0;
                r++;
            }

            if (r >= _matrix.Height) {
                if (_matrix['x', 2].Contains(double.NaN) || _matrix['y', 2].Contains(double.NaN)) {
                    r = c = 0;
                } else break;
            }
        }

        LogTable(1, 2);
    }

    private void FindIndirectCosts() {
        Console.WriteLine("Finding indirect costs:\n");
        _matrix.FillIndirectCosts(_matrix.Height, _matrix.Width);
        for (int r = 0; r < _matrix.Height; r++) {
            for (int c = 0; c < _matrix.Width; c++) {
                if (_matrix[0, r, c] == 0) {
                    _matrix[2, r, c] = _matrix['x', 2, c] + _matrix['y', 2, r];
                }
            }
        }

        LogTable(2, 2);
    }

    private bool IsOptimal(out Point coords) {
        coords = new Point(int.MinValue, int.MinValue);
        double maxDelta = double.MinValue;

        for (int r = 0; r < _matrix.Height; r++) {
            for (int c = 0; c < _matrix.Width; c++) {
                if (_matrix[0, r, c] != 0) continue;

                double delta = _matrix[2, r, c] - _matrix[1, r, c];
                if (delta > maxDelta) {
                    maxDelta = delta;
                    coords = new Point(c, r);
                }
            }
        }

        return maxDelta <= 0;
    }

    private List<Point> SearchLoop(Point coord) {
        List<Point> loop = [coord];

        var current = coord;
        bool row = true;
        do {
            current = Next(current, row);
            if (current == coord) break;
            loop.Add(current);
            row = !row;
        } while (current != coord);

        return loop;
    }

    private Point Next(Point current, bool row) {
        Point next;
        int index;

        if (row) {
            index = MoveVertical(current.X, 0, r => r < current.Y, r => ++r);
            if (index < 0) index = MoveVertical(current.X, _matrix.Height - 1, r => r > current.Y, r => --r);
            
            next = new Point(current.X, index);
        } else {
            index = MoveHorizontal(current.Y, 0, c => c < current.X, c => ++c);
            if (index < 0) index = MoveHorizontal(current.Y, _matrix.Width - 1, c => c > current.X, col => --col);
            
            next = new Point(index, current.Y);
        }

        return next;
    }

    private int MoveVertical(int c, int start, Func<int, bool> end, Func<int, int> action) {
        for (int row = start; end(row); row = action(row)) {
            if (_matrix[0, row, c] != 0) return row;
        }
        return int.MinValue;
    }

    private int MoveHorizontal(int r, int start, Func<int, bool> end, Func<int, int> action) {
        for (int col = start; end(col); col = action(col)) {
            if (_matrix[0, r, col] != 0) return col;
        }
        return int.MinValue;
    }

    private void RecalculatePlan(List<Point> loop) {
        double min = double.MaxValue;

        for (int i = 0; i < loop.Count; i++) {
            if (i % 2 != 0)
                min = Math.Min(min, _matrix[0, loop[i].Y, loop[i].X]);
        }
        Console.WriteLine($"A λ value is {min}");

        for (int i = 0; i < loop.Count; i++) {
            if (i % 2 != 0)
                _matrix[0, loop[i].Y, loop[i].X] -= min;
            else
                _matrix[0, loop[i].Y, loop[i].X] += min;
        }

        Console.WriteLine($"A new transportation plan:\n");
        LogTable(0, 0);
    }

    protected double FindTotalCost() {
        double sum = 0;
        for (int r = 0; r < _matrix.Height; r++) {
            for (int c = 0; c < _matrix.Width; c++) {
                sum += _matrix[0, r, c] * _matrix[1, r, c];
            }
        }
        return sum;
    }

    protected static double[] StringToArray(string text) 
        => text.Split().Select(double.Parse).ToArray();

    protected void LogTable(int contentLayer, int headerLayer) {
        Console.WriteLine(_matrix.ToString(contentLayer, headerLayer));
    }
}

