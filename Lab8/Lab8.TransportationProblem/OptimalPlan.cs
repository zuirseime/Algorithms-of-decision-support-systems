using Lab8.Common;
using System.Drawing;

namespace Lab8.TransportationProblem;
public class OptimalPlan : Plan {
    internal override void Find(Matrix matrix) {
        base.Find(matrix);
        Log.WriteLine("Finding an optimal plan:\n");

        while (true) {
            GetPotentials();
            GetIndirectCosts();

            if (IsOptimal(out Point coord)) {
                Log.WriteLine("The optimal condition is satisfied.\n" +
                                  "The optimal transportation plan was found:\n");
                LogTable(0, 0);
                Log.WriteLine($"The optimal plan cost: {Solution}\n");
                break;
            }

            Log.WriteLine("The optimal condition is not satisfied.\n" +
                         $"Detected a bad cell: [{coord.Y}, {coord.X}]\n");
            _matrix[0, coord.Y, coord.X] = double.Epsilon;
            LoopBuilder loopBuilder = new(coord, _matrix[0]);
            var loop = loopBuilder.ToList();
            RecalculatePlan(loop);
        }

        SaveMatrix();
    }

    private Queue<Point> _increasedCells = new();
    private void GetPotentials() {
        Log.WriteLine("Finding potentials:\n");
        _matrix.FillPotentialsWithNaN();
        IncreaseBadCells();

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

        DecreaseBadCells();
        LogTable(1, 2);
    }

    private void IncreaseBadCells() {
        for (int row = 0; row < _matrix.Height; row++) {
            for (int col = 0; col < _matrix.Width; col++) {
                Point cell = new(col, row);
                if (CheckHorizontal(cell) || CheckVertical(cell)) continue;

                Point cellToIncrease = IncreasingDirection(cell);
                _matrix[0, cellToIncrease.Y, cellToIncrease.X] += double.Epsilon;
                _increasedCells.Enqueue(cellToIncrease);
            }
        }
    }

    private void DecreaseBadCells() {
        if (_increasedCells.Count == 0) return;

        for (Point cell = _increasedCells.Dequeue(); 
            _increasedCells.Count > 0; 
            cell = _increasedCells.Dequeue()) {
            _matrix[0, cell.Y, cell.X] -= double.Epsilon;
        }
    }

    private bool CheckVertical(Point cell) {
        for (int row = 0; row < _matrix.Height; row++) {
            if (row == cell.Y || _matrix[0, row, cell.X] == 0) continue;
            return true;
        }
        return false;
    }

    private bool CheckHorizontal(Point cell) {
        for (int col = 0; col < _matrix.Width; col++) {
            if (col == cell.X || _matrix[0, cell.Y, col] == 0) continue;
            return true;
        }
        return false;
    }

    private Point IncreasingDirection(Point cell) {
        Point result = cell;
        if (cell.Y == 0) result.Y++;
        else if (cell.Y == _matrix.Height - 1) result.Y--;
        return result;
    }

    private void GetIndirectCosts() {
        Log.WriteLine("Finding indirect costs:\n");
        _matrix.FillIndirectCosts();

        for (int row = 0; row < _matrix.Height; row++) {
            for (int col = 0; col < _matrix.Width; col++) {
                if (_matrix[0, row, col] == 0)
                    _matrix[2, row, col] = _matrix['x', 2, col] + _matrix['y', 2, row];
            }
        }

        LogTable(2, 2);
    }

    private bool IsOptimal(out Point coord) {
        coord = new Point(int.MinValue, int.MinValue);
        double maxDelta = double.MinValue;

        for (int r = 0; r < _matrix.Height; r++) {
            for (int c = 0; c < _matrix.Width; c++) {
                if (_matrix[0, r, c] != 0) continue;

                double delta = _matrix[2, r, c] - _matrix[1, r, c];
                if (delta >= maxDelta) {
                    maxDelta = delta;
                    coord = new Point(c, r);
                }
            }
        }

        return maxDelta <= 0;
    }

    private void RecalculatePlan(List<Point> loop) {
        double min = double.MaxValue;

        for (int i = 0; i < loop.Count; i++) {
            if (i % 2 != 0)
                min = Math.Min(min, _matrix[0, loop[i].Y, loop[i].X]);
        }
        Log.WriteLine($"A λ value is {min}");

        for (int i = 0; i < loop.Count; i++) {
            if (i % 2 != 0)
                _matrix[0, loop[i].Y, loop[i].X] -= min;
            else
                _matrix[0, loop[i].Y, loop[i].X] += min;
        }

        Log.WriteLine($"A new transportation plan:\n");
        LogTable(0, 0);
    }
}
