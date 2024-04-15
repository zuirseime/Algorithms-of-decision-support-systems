﻿namespace Lab7.Core;
public class LaplaceCriterion : Criterion {
    public override string Run(Matrix matrix) {
        Log.WriteLine("\nLaplace criterion:\n");

        List<Strategy> strategies = this.GetStrategies(matrix, new Probabilities(Enumerable.Repeat(1d / matrix.Width, matrix.Width).ToArray()));

        double maximax = strategies.Max(s => s.Value);
        Log.WriteLine($"\nMaximum element: {maximax}\n");

        strategies.RemoveAll(s => s.Value != maximax);

        string optimalStrategies = string.Join(" or ", strategies);
        Log.WriteLine("Optimal strategies: " + optimalStrategies);

        return optimalStrategies;
    }

    private List<Strategy> GetStrategies(Matrix matrix, Probabilities probabilities) {
        List<Strategy> strategies = [];

        for (int row = 0; row < matrix.Height; row++) {
            double value = 0;
            string text = $"s{row + 1} = ";
            for (int col = 0; col < matrix.Width; col++) {
                value += matrix[row, col] * probabilities[col];
                text += $"{matrix[row, col]} * {probabilities[col]}";
            }
            Log.WriteLine($"{text} = {Math.Round(value, Globals.Round)}");
            strategies.Add(new Strategy(row + 1, value));
        }

        return strategies;
    }
}
