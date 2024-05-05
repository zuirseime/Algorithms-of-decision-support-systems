using CalculatingWork.Core.MatrixGame.Models;

namespace CalculatingWork.Core.MatrixGame;
internal static class Designer {
    internal static void ShowStrategies(string type, Roots[] players, double price) {
        Log.WriteLine($"""

            {type} strategies:

            Player 1: {players[0]}
            Player 2: {players[1]}
            Game price: {price}
            """, true);
    }

    internal static void ShowGamePrice(string type, Pivot pivot) {
        Log.WriteLine($"The {type} game price is found: " +
                      $"A[{pivot.Position.Y + 1}, {pivot.Position.X + 1}] = {Globals.Round(pivot.Value)}", true);
    }
}
