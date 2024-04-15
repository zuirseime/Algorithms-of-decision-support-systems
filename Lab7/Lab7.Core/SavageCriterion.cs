namespace Lab7.Core;
public class SavageCriterion : Criterion {
    public override string Run(Matrix matrix) {
        Log.WriteLine("\nSavage criterion:\n");
        List<Strategy> strategies = [];

        Log.WriteLine("Risk matrix:\n");
        var riskMatrix = this.GetRiskMatrix(matrix);
        Log.WriteLine(riskMatrix);

        for (int row = 0; row < riskMatrix.Height; row++) {
            double max = double.MinValue;
            for (int col = 0; col < riskMatrix.Width; col++) {
                if (riskMatrix[row, col] > max)
                    max = riskMatrix[row, col];
            }
            Log.WriteLine($"Row {row + 1} max: {max}");
            strategies.Add(new Strategy(row + 1, max));
        }

        double min = strategies.Min(s => s.Value);
        Log.WriteLine($"\nMinimum element: {min}\n");

        strategies.RemoveAll(s => s.Value != min);

        string optimalStrategies = string.Join(" or ", strategies);
        Log.WriteLine("Optimal strategies: " + optimalStrategies);

        return optimalStrategies;
    }

    private Matrix GetRiskMatrix(Matrix matrix) {
        Matrix temp = new Matrix((double[,])matrix.Data.Clone());

        for (int col = 0; col < temp.Width; col++) {
            double[] column = new double[temp.Height];
            for (int row = 0; row < temp.Height; row++) {
                column[row] = temp[row, col];
            }

            double max = column.Max();
            for (int row = 0; row < temp.Height; row++) {
                temp[row, col] = max - temp[row, col];
            }
        }

        return temp;
    }
}
