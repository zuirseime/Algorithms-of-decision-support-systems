using Lab5.Core.SimplexAlgorithm.Input;
using Lab5.Core.SimplexAlgorithm.Models;
using Lab5.Core.SimplexAlgorithm.Modules;

namespace Lab5.Core.SimplexAlgorithm;

public class SA(char[] functions, char[] rowVariables, char[] columnVariables) {
    private char[] _functions = functions;

    private Tableau _tableau;
    private SAResult _result = SAResult.Default;

    private ZeroRows _zeroRows = new();
    private BasicFeasibleSolution _basicFeasibleSolution = new();
    private OptimalSolution _optimalSolution = new();

    public SAResult Run(Function func, Constraint[] constraints, bool max = true) {
        Log.Clear();
        Log.WriteLine($"Straight problem definition:\nZ = {func} -> {(max ? "max" : "min")}\n" +
                      $"with constraints:\n{string.Join('\n', constraints.AsEnumerable())}");

        this.GenerateTableau(func, constraints);

        if (!max) {
            Log.WriteLine("\nGoing to the problem of maximizing the objective function Z':");

            int rows = this._tableau.Height;
            int cols = this._tableau.Width;

            string funcStr = string.Empty;
            for (int col = 0; col < cols - 1; col++) {
                this._tableau[rows - 1, col] = this._tableau[rows - 1, col] *= -1;
                funcStr += $"{(this._tableau[rows - 1, col] >= 0 
                    ? this._tableau[rows - 1, col] 
                    : -this._tableau[rows - 1, col])}*X[{col + 1}]{(col != cols - 2 
                    ? this._tableau[rows - 1, col] >= 0 ? " + " 
                    : " - " : "")}";
            }
            Log.WriteLine($"Z' = {funcStr} -> max");
        }
        Log.WriteLine("\nInput simplex tableau:");

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

        this._tableau = new Tableau(tableau, rowHeaders, colHeaders, rowVariables, columnVariables);
        this._tableau.FixHeaders();
    }

    private void DualProblemDefinition(bool max) {
        Tableau dualTableau = Tableau.Transpose(this._tableau);

        int rows = dualTableau.Height;
        int cols = dualTableau.Width;

        double[] funcCoeffs = new double[cols - 1];
        for (int col = 0; col < cols - 1; col++) {
            funcCoeffs[col] = -dualTableau[rows - 1, col];
        }
        double funcConst = -dualTableau[rows - 1, cols - 1];
        Function func = new(funcCoeffs, funcConst, 'u');

        Constraint[] constraints = new Constraint[rows - 1];
        for (int row = 0; row < rows - 1; row++) {
            double[] constrCoeffs = new double[cols - 1];
            for (int col = 0; col < cols - 1; col++) {
                constrCoeffs[col] = dualTableau[row, col];
            }
            double constrConst = dualTableau[row, cols - 1];

            constraints[row] = new Constraint(constrCoeffs, constrConst, Relation.GreaterOrEqual, 'u');
        }

        Log.WriteLine($"\nDual problem definition:\nW = {func} -> {(max ? "min" : "max")}\n" +
                      $"with constraints:\n{string.Join('\n', constraints.AsEnumerable())}");

        if (dualTableau.Columns.Any(c => c.Contains('0'))) {
            Log.WriteLine("Free variables: " + string.Join(", ", Enumerable.Range(0, cols)
                .Where(col => dualTableau.Columns[col].Contains('0'))
                .Select(col => $"u{col + 1}")));
        }
    }

    private void Execute(bool max) {
        if (this._tableau.Data is null) return;
        this._tableau.InvertSigns();
        Designer.LogTable(this._tableau);

        this.DualProblemDefinition(max);

        if (this._tableau.Rows.Any(h => h.Contains('0')))
            this._tableau = this._zeroRows.Remove(this._tableau);

        (this._tableau, this._result.Roots) = this._basicFeasibleSolution.Find(this._tableau);

        (this._tableau, this._result.Roots) = max ? this._optimalSolution.Max(this._tableau)
                                            : this._optimalSolution.Min(this._tableau);

        if (this._tableau.Data is null) {
            this._result = SAResult.Default;
            return;
        }

        this._result.Solution = Math.Round(
            this._tableau.Data![this._tableau.Height - 1, this._tableau.Width - 1],
            this._tableau.Round - 1
        );
        Log.WriteLine($"\n{(max ? "Max" : "Min")} (Z) = {this._result.Solution}" +
                      $"\n{(!max ? "Max" : "Min")} (W) = {this._result.Solution}");
    }
}