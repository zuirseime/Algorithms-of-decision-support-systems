namespace CalculatingWork.Core.SimplexAlgorithm.Models;
public class Functions(Function[] data, double[,] rawData) {

    public Function[] Data { get; set; } = data;
    public double[,] RawData => rawData;

    public Function this[int index] {
        get => this.Data[index];
        set => this.Data[index] = value;
    }

    public static Functions Parse(string data) {
        Function[] functions = data.Trim().Split('\n').Select(c => Function.Parse(c.Trim())).ToArray();

        int rows = functions.Length;
        int cols = functions.Max(f => f.Length - 1);

        double[,] coefficents = new double[rows, cols];

        for (int row = 0; row < rows; row++) {
            for (int col = 0; col < cols; col++) {
                coefficents[row, col] = functions[row][col];
            }
        }

        return new Functions(functions, coefficents);
    }
}