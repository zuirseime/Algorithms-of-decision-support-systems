namespace Lab7.Core;
public class WaldСriterion : Criterion {
    public override string Run(Matrix matrix) {
        Log.WriteLine("\nWald criterion:\n");
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

        double max = strategies.Max(s => s.Value);
        Log.WriteLine($"\nMaximum element: {max}\n");

        strategies.RemoveAll(s => s.Value != max);

        string optimalStrategies = string.Join(" or ", strategies);
        Log.WriteLine("Optimal strategies: " + optimalStrategies);

        return optimalStrategies;
    }
}
