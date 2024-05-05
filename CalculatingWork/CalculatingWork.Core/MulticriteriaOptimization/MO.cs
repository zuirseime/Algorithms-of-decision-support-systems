using CalculatingWork.Core.MatrixGame;
using CalculatingWork.Core.MatrixGame.Models;
using CalculatingWork.Core.SimplexAlgorithm;
using CalculatingWork.Core.SimplexAlgorithm.Models;

namespace CalculatingWork.Core.MulticriteriaOptimization;
public class MO {
    private MOResult _result = MOResult.Default;
    private Functions _functions = null!;
    private Constraints _constraints = null!;

    public MOResult Run(string functions, string constraints) {
        this._functions = Functions.Parse(functions);
        this._constraints = Constraints.Parse(constraints);

        this.FindOptimalVectors();
        this.GetGoalFunctionCoefficients();
        var matrix = this.FindUnoptimalSolutions();
        matrix = this.BuildMatrixGame(matrix);
        this.SolveMatrixGame(matrix);
        this.FindCompromiseSolution();

        return this._result;
    }

    private void FindOptimalVectors() {
        List<double[]> optimalVectors = [];

        Log.WriteLine("Search for optimal vectors:\n");
        foreach (Function func in this._functions.Data) {
            SA sa = new(['y'], ['x']);
            SAResult result = sa.Run(func, this._constraints);

            optimalVectors.Add(result.Straight.Values);
        }

        var optimalVecs = new Array2D([.. optimalVectors], "X*", "x");
        Log.WriteLine($"\nk={optimalVectors.Count} optimal vectors were obtained:");
        Log.WriteLine(optimalVecs.GetTableau(), true);

        this._result.OptimalVectors = optimalVecs;
    }

    private void GetGoalFunctionCoefficients() {
        var coefficients = new Array2D(this._functions.RawData, "C", "x");
        Log.WriteLine($"\nThe matrix of the coefficients of the goal function:");
        Log.WriteLine(coefficients.GetTableau(), true);

        this._result.GoalCoefficients = coefficients;
    }

    private Array2D FindUnoptimalSolutions() {
        double[][] xs = this._result.OptimalVectors.Values;
        double[][] cs = this._result.GoalCoefficients.Values;

        int k = xs.Length;
        double[][] q = new double[k][];
        
        for (int i = 0; i < k; i++) {
            q[i] = new double[k];
            for (int j = 0; j < k; j++) {
                q[i][j] = Math.Abs((this.GetValue(cs[j], xs[i]) - this.GetValue(cs[j], xs[j])) 
                                    / this.GetValue(cs[j], xs[j]));
            }
        }

        Array2D matrix = new(q, "\0", "\0");

        Log.WriteLine($"Search for unoptimal solutions:\n{matrix}");

        var result = (Array2D)matrix.Clone();
        this._result.UnoptimalSolutions = matrix;

        return result;
    }

    private double GetValue(double[] c, double[] x) {
        double result = 0;

        for (int i = 0; i < x.Length; i++)
            result += x[i] * c[i];

        return result;
    }

    private Array2D BuildMatrixGame(Array2D matrix) {
        double max = double.MinValue;
        for (int row = 0; row < matrix.Height; row++) {
            for (int col = 0; col < matrix.Width; col++) {
                if (matrix[row, col] > max)
                    max = matrix[row, col];
            }
        }

        Log.WriteLine($"max = {Globals.Round(max)}\n");

        for (int row = 0; row < matrix.Height; row++) {
            for (int col = 0; col < matrix.Width; col++) {
                matrix[row, col] = max - matrix[row, col];
            }
        }

        Log.WriteLine($"{matrix}", true);
        this._result.MatrixGame = matrix;

        return (Array2D)matrix.Clone();
    }

    private void SolveMatrixGame(Array2D matrix) {
        Log.WriteLine($"Search for matrix game solutions:\nA:\n{matrix}", true);

        Matrix matrix2d = new(Array2D.Convert(matrix.Values));

        var result = new MG().Run(matrix2d);
        this._result.Weights = result.Player1;
        Log.WriteLine($"\nSolutions weight coefficients: {result.Player1}", true);
    }

    private void FindCompromiseSolution() {
        double[][] functions = this._result.OptimalVectors.Values;
        double[] weights = this._result.Weights.Values;

        int rows = functions.Length;
        int cols = functions.Max(f => f.Length);
        double[] values = new double[cols];

        for (int col = 0; col < cols; col++) {
            double value = 0;
            for (int row = 0; row < rows; row++) {
                value += functions[row][col] * weights[row];
            }
            values[col] = value;
        }

        var solution = new Roots("X*1", values);
        this._result.CompromiseSolution = solution;
        Log.WriteLine($"\nCompromise solution:\n{solution.ToString(true)}", true);
    }
}
