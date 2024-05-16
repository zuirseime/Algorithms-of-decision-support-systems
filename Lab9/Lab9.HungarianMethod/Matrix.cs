namespace Lab9.HungarianMethod;
public struct Matrix {
    private double[,] _data;
    private State[,] _mask;


    public readonly int Height => _data.GetLength(0);
    public readonly int Width => _data.GetLength(1);

    public bool[] Rows { get; set; }
    public bool[] Columns { get; set; }

    public double this[int i, int j] {
        get => _data[i, j];
        set => _data[i, j] = value;
    }

    private int Offset {
        get {
            int maxLength = int.MinValue;

            for (int i = 0; i < _data.GetLength(0); i++) {
                for (int j = 0; j < _data.GetLength(1); j++) {
                    var value = Globals.Round(this[i, j]);
                    if ($"{value}".Length > maxLength)
                        maxLength = $"{value}".Length;
                }
            }

            return maxLength;
        }
    }

    public Matrix(double[,] data) {
        _data = data;

        Rows = new bool[_data.GetLength(0)];
        Columns = new bool[_data.GetLength(1)];

        Array.Fill(Rows, false);
        Array.Fill(Columns, false);
    }

    public static Matrix Parse(string input) {
        if (string.IsNullOrEmpty(input))
            throw new FormatException("Incorrect data format.");

        var delimiters = new char[] { ' ', '\t' };

        string[] rows = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        int numRows = rows.Length;
        int numCols = rows[0].Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length;

        double[,] data = new double[numRows, numCols];
        for (int row = 0; row < numRows; row++) {
            string[] elements = rows[row].Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

            if (elements.Length != numCols)
                throw new ArgumentException($"Row {row + 1} contains a differnet number of elements.");

            for (int col = 0; col < numCols; col++) {
                _ = double.TryParse(elements[col], out data[row, col]);
            }
        }

        return new Matrix(data);
    }

    public static bool TryParse(string str, out Matrix matrix) {
        matrix = new Matrix(new double[0, 0]);
        bool valid = false;

        try {
            matrix = Parse(str);
            valid = true;
        } catch (Exception ex) {
            Console.WriteLine(ex);
        }

        return valid;
    }

    public override string ToString() {
        return ToString(false);
    }

    public string ToString(bool state) {
        string result = string.Empty;

        for (int row = 0; row < _data.GetLength(0); row++) {
            for (int col = 0; col < _data.GetLength(1); col++) {
                result += $"{Globals.Round(_data[row, col])}".PadLeft(Offset) + " ";
            }
            result += '\n';
        }

        return result;
    }
}
