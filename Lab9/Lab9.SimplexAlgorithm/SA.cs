
using Lab9.SimplexAlgorithm.Models;
using Lab9.SimplexAlgorithm.Modules;

namespace Lab9.SimplexAlgorithm;

public class SA {
    internal static char[] RowVars { get; set; } = null!;
    internal static char[] ColVars { get; set; } = null!;

    private Tableau _tableau;

    private ZeroRows _zeroRows = new();
    private BasicFeasibleSolution _basicFeasibleSolution = new();
    private OptimalSolution _optimalSolution = new();

    public SA(char[] rowVariables, char[] columnVariables) {
        RowVars = rowVariables;
        ColVars = columnVariables;
    }

    public (BasicFeasibleSolution, OptimalSolution) Run(Function func, Constraints constraints) {
        bool max = func.Max;

        Designer.ProblemDefinition(func, constraints);

        GenerateTableau(func, ((Constraints)constraints.Clone()).Data);
        if (!max) Designer.MinToMax((Function)func.Clone());

        Execute(max);

        return (_basicFeasibleSolution, _optimalSolution);
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
                                : string.Join(", ", RowVars.Reverse().Select(v => $"{v}{y}")));

            for (int col = 0; col < cols; col++) {
                tableau[row, col] = col == cols - 1 || col < constraints[row].Order ? constraints[row][col] : 0;
            }

            if (constraints[row].Relation != Relation.Equal) y++;
        }

        rowHeaders[^1] = "Z";

        for (int col = 0; col < cols; col++) {
            tableau[rows - 1, col] = col != cols - 1 ? func.Coefficients[col] : 0;

            if (col < cols - 1)
                colHeaders[col] = string.Join(", ", ColVars.Reverse().Select(v => $"{v}{col + 1}"));
        }

        colHeaders[^1] = "1";

        _tableau = new Tableau(tableau, rowHeaders, colHeaders, RowVars, ColVars, constraints.Max(c => c.Order));
        _tableau.FixHeaders();
    }

    private void Execute(bool max) {
        if (_tableau.Data is null) return;

        _tableau.InvertSigns(max);
        Designer.ShowInputTableau(_tableau);

        if (_tableau.Rows.Any(h => h == "0"))
            _tableau = _zeroRows.Remove(_tableau);

        _tableau = _basicFeasibleSolution.Find(_tableau);
        Solution.IsMax = max;
        Designer.ShowSolution(_basicFeasibleSolution.Value, max);

        _tableau = max ? _optimalSolution.Max(_tableau) : _optimalSolution.Min(_tableau);
        Designer.ShowSolution(_optimalSolution.Value, max);
    }
}