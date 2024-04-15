namespace Lab7.Core;
public class MaximaxCriterion : Criterion {
    public override string Run(Matrix matrix) {
        Log.WriteLine("\nMaximax criterion:\n");
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

        double maximax = strategies.Max(s => s.Value);
        Log.WriteLine($"\nMaximum element: {maximax}\n");

        strategies.RemoveAll(s => s.Value != maximax);

        string optimalStrategies = string.Join(" or ", strategies);
        Log.WriteLine("Optimal strategies: " + optimalStrategies);

        return optimalStrategies;
    }
}
