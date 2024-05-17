using Lab9.Common;

namespace Lab9.HungarianMethod;
public struct Matrix : ICloneable {
    public readonly int Height => Data.GetLength(0);
    public readonly int Width => Data.GetLength(1);

    public MatrixItem[,] Data { get; set; }

    public MatrixItem this[int i, int j] {
        get => Data[i, j];
        set => Data[i, j] = value;
    }

    private int Offset { get; set; }

    public Matrix(MatrixItem[,] data) {
        Data = data;
        SetOffset();
    }

    private void SetOffset() {
        int maxLength = int.MinValue;

        for (int i = 0; i < Data.GetLength(0); i++) {
            for (int j = 0; j < Data.GetLength(1); j++) {
                var value = Globals.Round(this[i, j].Value);
                if ($"{value}".Length > maxLength)
                    maxLength = $"{value}".Length;
            }
        }

        Offset = maxLength;
    }

    public void RestoreStates() {
        for (int row = 0; row < Height; row++) {
            for (int col = 0; col < Width; col++) {
                Data[row, col].State = State.None;
            }
        }
    }

    public static Matrix Parse(string input) {
        if (string.IsNullOrEmpty(input))
            throw new FormatException("Incorrect data format.");

        var delimiters = new char[] { ' ', '\t' };

        string[] rows = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        int numRows = rows.Length;
        int numCols = rows[0].Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length;

        MatrixItem[,] data = new MatrixItem[numRows, numCols];
        for (int row = 0; row < numRows; row++) {
            string[] elements = rows[row].Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

            if (elements.Length != numCols)
                throw new ArgumentException($"Row {row + 1} contains a differnet number of elements.");

            for (int col = 0; col < numCols; col++) {
                _ = double.TryParse(elements[col], out data[row, col].Value);
                data[row, col].State = State.None;
            }
        }

        return new Matrix(data);
    }

    public static bool TryParse(string str, out Matrix matrix) {
        matrix = new Matrix(new MatrixItem[0, 0]);
        bool valid = false;

        try {
            matrix = Parse(str);
            valid = true;
        } catch (Exception ex) {
            Log.WriteLine(ex);
        }

        return valid;
    }

    public override string ToString() {
        return ToString(false);
    }

    public string ToString(bool state) {
        string result = string.Empty;

        for (int row = 0; row < Data.GetLength(0); row++) {
            for (int col = 0; col < Data.GetLength(1); col++) {
                result += $"{Data[row, col].ToString(state)}".PadLeft(Offset) + " ";
            }
            result += '\n';
        }

        return result;
    }

    public object Clone() => new Matrix((MatrixItem[,])Data.Clone());
}
