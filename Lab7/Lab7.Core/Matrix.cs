using System.Diagnostics;

namespace Lab7.Core;
public class Matrix(double[,] data) {
    public int Width => this.Data.GetLength(1);
    public int Height => this.Data.GetLength(0);
    public double[,] Data { get; set; } = data;

    public double this[int row, int col] {
        get => row < this.Height && col < this.Width ? this.Data[row, col] : double.NaN;
        set { if (row < this.Height && col < this.Width) Data[row, col] = value; }
    }

    private int Offset {
        get {
            int maxLength = int.MinValue;

            for (int i = 0; i < Data.GetLength(0); i++) {
                for (int j = 0; j < Data.GetLength(1); j++) {
                    if ($"{Math.Round(Data[i, j], Globals.Round)}".Length > maxLength) {
                        maxLength = $"{Math.Round(Data[i, j], Globals.Round)}".Length;
                    }
                }
            }

            return maxLength;
        }
    }

    internal double FindMinimum() {
        double min = double.MaxValue;

        for (int row = 0; row < this.Height; row++) {
            for (int col = 0; col < this.Width; col++) {
                if (this.Data[row, col] < min)
                    min = this.Data[row, col];
            }
        }

        return min;
    }

    public static Matrix Parse(string str) {
        if (string.IsNullOrEmpty(str))
            throw new FormatException("Incorrect data format.");

        var delimiters = new char[] { ' ', '\t' };

        string[] rows = str.Split('\n', StringSplitOptions.RemoveEmptyEntries);

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
        } catch (FormatException ex) {
            Debug.WriteLine(ex.Message);
        } catch (ArgumentException ex) {
            Debug.WriteLine(ex.Message);
        }

        return valid;
    }

    public override string ToString() {
        string result = string.Empty;

        for (int row = 0; row < Data.GetLength(0); row++) {
            for (int col = 0; col < Data.GetLength(1); col++) {
                result += $"{Math.Round(Data[row, col], Globals.Round)}".PadLeft(Offset) + " ";
            }
            result += '\n';
        }

        return result;
    }
}
