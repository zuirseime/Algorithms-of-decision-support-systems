using Lab6.Core.Output;
using Lab6.Core.SimplexAlgorithm.Models;
using Lab6.Core.SimplexAlgorithm.Modules;

namespace Lab6.Core.SimplexAlgorithm;

public class SA(char[] functions, char[] rowVariables, char[] columnVariables) {
    private char[] _functions = functions;

    private Tableau _tableau;
    private SAResult _result = SAResult.Default;

    private BasicFeasibleSolution _basicFeasibleSolution = new();
    private OptimalSolution _optimalSolution = new();

    public SAResult Run(Function func, Constraint[] constraints, bool max = true) {
        Log.Clear();
        Designer.StraightProblemDefinition(func, constraints, max);

        this.GenerateTableau(func, constraints);
        if (!max) Designer.MinToMax(this._tableau);

        this.Execute(max);

        return this._result;
    }

    private void GenerateTableau(Function func, Constraint[] constraints) {
        int rows = constraints.Length + 1;
        int cols = constraints.Max(c => c.Length);

        string[] colHeaders = new string[cols];
        string[] rowHeaders = new string[rows];

        int order = constraints.Max(c => c.Order);

        double[,] tableau = new double[rows, cols];

        Array.ForEach(constraints, c => {
            if (c.Relation != Relation.GreaterOrEqual) c.Invert();
        });

        int y = 1;
        for (int row = 0; row < rows - 1; row++) {
            rowHeaders[row] = "(" + (constraints[row].Relation == Relation.Equal ? "0"
                                  : string.Join(", ", rowVariables.Reverse().Select(v => $"{v}{y}"))) + ")";

            for (int col = 0; col < cols; col++) {
                tableau[row, col] = col == cols - 1 || col < constraints[row].Order ? constraints[row][col] : 0;
            }

            if (constraints[row].Relation != Relation.Equal) y++;
        }

        rowHeaders[^1] = $"(1, {this._functions[0]})";

        for (int col = 0; col < cols; col++) {
            tableau[rows - 1, col] = col != cols - 1 ? func.Coefficients[col] : 0;
            colHeaders[col] = "(" + (col >= cols - 1 ? $"{this._functions[1]}, 1"
                                  : string.Join(", ", columnVariables.Reverse().Select(v => $"{v}{col + 1}"))) + ")";
        }

        this._tableau = new Tableau(tableau, rowHeaders, colHeaders, rowVariables, columnVariables, constraints.Max(c => c.Order));
        this._tableau.FixHeaders();
    }

    private void Execute(bool max) {
        if (this._tableau.Data is null) return;

        this._tableau.InvertSigns();
        Designer.ShowInputTableaus(this._tableau, max);

        (this._tableau, this._result.Roots) = this._basicFeasibleSolution.Find(this._tableau);

        (this._tableau, this._result.Roots) = max ? this._optimalSolution.Max(this._tableau)
                                            : this._optimalSolution.Min(this._tableau);

        if (this._tableau.Data is null) {
            this._result = SAResult.Default;
            return;
        }

        this._result.Solution = Math.Round(
            this._tableau.Data![this._tableau.Height - 1, this._tableau.Width - 1],
            Globals.Round - 1
        );

        Designer.ShowSolution(this._result.Solution, max);
    }
}