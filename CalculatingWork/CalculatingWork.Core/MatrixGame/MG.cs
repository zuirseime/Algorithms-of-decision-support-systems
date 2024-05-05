using CalculatingWork.Core.MatrixGame.Models;
using CalculatingWork.Core.MatrixGame.Modules;

namespace CalculatingWork.Core.MatrixGame;
public class MG {
    private Strategies _strategies = new();

    public MGResult Run(string matrixStr) {
        if (!Matrix.TryParse(matrixStr, out Matrix matrix))
            throw new ArgumentException(nameof(matrixStr));

        return this.Run(matrix);
    }

    public MGResult Run(Matrix matrix) {
        if (Pivot.Find(matrix, out Pivot pivot)) {
            Log.WriteLine("\nPivot point is found.");
            return this._strategies.Pure(matrix, pivot);
        } else {
            Log.WriteLine("\nPivot point is not found.\n");
            return this._strategies.Mixed(matrix);
        }
    }
}
