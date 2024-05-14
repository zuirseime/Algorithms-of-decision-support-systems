using Lab8.Common;

namespace Lab8.TransportationProblem;

public class TP {
    private Matrix _matrix;

    private FeasiblePlan _feasiblePlan = null!;
    private OptimalPlan _optimalPlan = null!;

    public (FeasiblePlan, OptimalPlan) Run(string matrix, string customers, string suppliers) {
        return Run(Matrix.Parse(matrix), StringToArray(customers), StringToArray(suppliers));
    }

    public (FeasiblePlan, OptimalPlan) Run(Matrix matrix, double[] customers, double[] suppliers) {
        _matrix = matrix;
        _optimalPlan = new OptimalPlan();
        _feasiblePlan = new FeasiblePlan();

        try {
            _matrix.Load(suppliers, customers);
        } catch {

        }

        _feasiblePlan.Find(_matrix);
        _optimalPlan.Find(_matrix);

        return (_feasiblePlan, _optimalPlan);
    }

    protected static double[] StringToArray(string text) 
        => text.Split().Select(double.Parse).ToArray();

    protected void LogTable(int contentLayer, int headerLayer) {
        Console.WriteLine(_matrix.ToString(contentLayer, headerLayer));
    }
}