using CalculatingWork.Core.SimplexAlgorithm.Models;
using CalculatingWork.Core.SimplexAlgorithm.Modules;

namespace CalculatingWork.Core.SimplexAlgorithm;
public class SA {
    internal static char[] RowVars { get; set; } = null!;
    internal static char[] ColVars { get; set; } = null!;

    private Tableau _tableau;
    private SAResult _result = SAResult.Default;
    private bool _dual = false;

    private ZeroRows _zeroRows = new();
    private BasicFeasibleSolution _basicFeasibleSolution = new();
    private OptimalSolution _optimalSolution = new();

    public SA(char[] rowVariables, char[] columnVariables, bool dual = false) {
        SA.RowVars = rowVariables;
        SA.ColVars = columnVariables;
        this._dual = dual;
    }

    public SAResult Run(Function func, Constraints constraints) {
        bool max = func.Max;

        Designer.StraightProblemDefinition(func, constraints);

        this.GenerateTableau(func, ((Constraints)constraints.Clone()).Data);
        if (!max) Designer.MinToMax(this._tableau, (Function)func.Clone());

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
            rowHeaders[row] = (constraints[row].Relation == Relation.Equal ? "0"
                                : string.Join(", ", SA.RowVars.Reverse().Select(v => $"{v}{y}")));

            for (int col = 0; col < cols; col++) {
                tableau[row, col] = col == cols - 1 || col < constraints[row].Order ? constraints[row][col] : 0;
            }

            if (constraints[row].Relation != Relation.Equal) y++;
        }

        rowHeaders[^1] = $"{(this._dual ? "1, " : "")}Z";

        for (int col = 0; col < cols; col++) {
            tableau[rows - 1, col] = col != cols - 1 ? func.Coefficients[col] : 0;
            
            if (col < cols - 1)
                colHeaders[col] = string.Join(", ", SA.ColVars.Reverse().Select(v => $"{v}{col + 1}"));
        }

        colHeaders[^1] = $"{(this._dual ? "W, " : "")}1";

        this._tableau = new Tableau(tableau, rowHeaders, colHeaders, SA.RowVars, SA.ColVars, constraints.Max(c => c.Order));
        this._tableau.FixHeaders();
    }

    private void Execute(bool max) {
        if (this._tableau.Data is null) return;

        this._tableau.InvertSigns(max);
        Designer.ShowInputTableau(this._tableau, max, this._dual);

        if (this._tableau.Rows.Any(h => h.Contains('0')))
            this._tableau = this._zeroRows.Remove(this._tableau);

        (this._tableau, _) = this._basicFeasibleSolution.Find(this._tableau, this._dual);

        (this._tableau, Roots[] roots) = max ? this._optimalSolution.Max(this._tableau, this._dual)
                                            : this._optimalSolution.Min(this._tableau, this._dual);

        if (this._tableau.Data is null) {
            this._result = SAResult.Default;
            return;
        }

        this._result.Straight = roots[0];
        if (this._dual) this._result.Dual = roots[1];

        this._result.Solution = this._tableau.Data![this._tableau.Height - 1, this._tableau.Width - 1];

        Designer.ShowSolution(this._result.Solution, max, this._dual);
    }
}