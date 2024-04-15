using System.Windows.Controls;

namespace Lab7.Core;
public class HurwitzCriterion : Criterion {
    public override string Run(Matrix matrix) {
        return this.Run(matrix, 1);
    }

    public string Run(Matrix matrix, double factor) {
        Log.WriteLine("\nHurwitzCriterion criterion:\n");
        Log.WriteLine($"Factor y = {factor}\n");

        List<Strategy> minStrategies = GetMinStrategies(matrix);
        List<Strategy> maxStrategies = GetMaxStrategies(matrix);
        List<Strategy> strategies = [];
        
        for (int i = 0; i < matrix.Height; i++) {
            double value = factor * minStrategies[i].Value + (1 - factor) * maxStrategies[i].Value;
            var strategy = new Strategy(i + 1, value);
            strategies.Add(strategy);

            Log.WriteLine($"s{i + 1} = {factor} * {minStrategies[i].Value} + (1 - {factor}) * {maxStrategies[i].Value} = {Math.Round(value, 1)}");
        }

        double maximax = strategies.Max(s => s.Value);
        Log.WriteLine($"\nMaximum element: {Math.Round(maximax, Globals.Round)}\n");

        strategies.RemoveAll(s => s.Value != maximax);

        string optimalStrategies = string.Join(" or ", strategies);
        Log.WriteLine("Optimal strategies: " + optimalStrategies);

        return optimalStrategies;
    }

    private static List<Strategy> GetMinStrategies(Matrix matrix) {
        List<Strategy> strategies = [];

        for (int row = 0; row < matrix.Height; row++) {
            double min = double.MaxValue;
            for (int col = 0; col < matrix.Width; col++) {
                if (matrix[row, col] < min)
                    min = matrix[row, col];
            }
            Log.WriteLine($"Row {row + 1} min: {min}");
            strategies.Add(new Strategy(row + 1, min));
        }

        Log.WriteLine('\b');
        return strategies;
    }

    private static List<Strategy> GetMaxStrategies(Matrix matrix) {
        List<Strategy> strategies = [];

        for (int row = 0; row < matrix.Height; row++) {
            double max = double.MinValue;
            for (int col = 0; col < matrix.Width; col++) {
                if (matrix[row, col] > max)
                    max = matrix[row, col];
            }
            Log.WriteLine($"Row {row + 1} max: {max}");
            strategies.Add(new Strategy(row + 1, max));
        }

        Log.WriteLine('\b');
        return strategies;
    }
}
