using Lab5.Core.SimplexAlgorithm;
using Lab5.Core.SimplexAlgorithm.Input;

namespace Lab5.Core;
internal class Program {
    public static void Main(string[] args) {
        string funcStr = "-x4-2x5";
        string constrStr = """
                           2x1+2x2+x3+2x4+x5<=4 
                           2x1+x2+3x4+2x5<=4 
                           3x1+x3+2x4+5x5<=9
                           """;

        Log.Initiate();

        Function function = Function.Parse(funcStr);
        Constraint[] constraints = constrStr.Trim().Split('\n').Select(relation => Constraint.Parse(relation.Trim())).ToArray();

        var result = new SA(['Z', 'W'], ['y', 'u'], ['x', 'v']).Run(function, constraints, false);
    }
}
