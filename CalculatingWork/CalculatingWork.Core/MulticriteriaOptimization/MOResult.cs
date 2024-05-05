namespace CalculatingWork.Core.MulticriteriaOptimization;

public struct MOResult(Array2D goalCoefficients, Array2D optimalVectors, Array2D unoptimalSolutions, Array2D matrixGame, Roots weights, Roots compromiseSolution) {
    public Array2D GoalCoefficients { get; set; } = goalCoefficients;
    public Array2D OptimalVectors { get; set; } = optimalVectors;
    public Array2D UnoptimalSolutions { get; set; } = unoptimalSolutions;
    public Array2D MatrixGame { get; set; } = matrixGame;
    public Roots Weights { get; set; } = weights;
    public Roots CompromiseSolution { get; set; } = compromiseSolution;

    public static MOResult Default => new(new Array2D(), new Array2D(), new Array2D(), new Array2D(), Roots.Empty, Roots.Empty);
}
