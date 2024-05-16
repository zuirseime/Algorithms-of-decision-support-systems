using System.Drawing;

namespace Lab8.TransportationProblem;
internal class LoopBuilder {
    private readonly double[,] _matrix;
    private readonly List<Point> _loop = [];
    private Point _start;

    public Point this[int i] {
        get => _loop[i];
        private set => _loop[i] = value;
    }

    public LoopBuilder(Point start, double[,] matrix) {
        _start = start;
        _matrix = matrix;

        _loop.Add(_start);

        var current = _start;
        bool row = true;

        do {
            current = Next(current, row);
            if (current == _start) break;
            _loop.Add(current);
            row = !row;
        } while (current.Y != _start.Y);
    }

    private Point Next(Point current, bool row) {
        Point next;
        int index;

        if (row) {
            index = MoveVertical(current.X, 0, r => r < current.Y, r => ++r);
            if (index < 0)
                index = MoveVertical(
                    current.X,
                    _matrix.GetLength(0) - 1,
                    r => r > current.Y,
                    r => --r
                );

            next = new Point(current.X, index);
        } else {
            index = MoveHorizontal(current.Y, 0, c => c < current.X, c => ++c);
            if (index < 0)
                index = MoveHorizontal(
                        current.Y,
                        _matrix.GetLength(1) - 1,
                        c => c > current.X,
                        col => --col
                    );

            next = new Point(index, current.Y);
        }

        return next;
    }

    private int MoveVertical(int c, int start, Func<int, bool> end, Func<int, int> action) {
        for (int row = start; end(row); row = action(row)) {
            if (_matrix[row, c] != 0) return row;
        }
        return int.MinValue;
    }

    private int MoveHorizontal(int r, int start, Func<int, bool> end, Func<int, int> action) {
        for (int col = start; end(col); col = action(col)) {
            if (_matrix[r, col] != 0) return col;
        }
        return int.MinValue;
    }

    public List<Point> ToList() => _loop;
}
