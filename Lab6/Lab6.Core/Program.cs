using Lab6.Core.MatrixGame;
using Lab6.Core.Output;

namespace Lab6.Core;
internal class Program {
    public static void Main(string[] args) {
        double[,] a1 = {
            { 4, -2, -3 },
            { -1, 1, -2 },
            { -1, -1, -4 }
        };

        double[,] a2 = {
            { 5, 11 },
            { 8, 2 }
        };

        double[,] a3 = {
            { 15, 5 },
            { 6, 2 },
            { 7, 13 },
            { 3, 11 },
        };

        string funcStr = "x2-3x3+2x5";
        string constrStr = """
                           -x1+3x3-2x4+x5<=3
                           x1+3x2-x3-x4+x5<=2
                           x1-x2+x4+x5<=3
                           """;

        string matrix = """
            2 -1 3 3
            -1 2 2 7
            1 1 1 2
            """;

        Log.Initiate();

        MG game = new();
        game.Run(matrix);

        //Function function = Function.Parse(funcStr);
        //Constraint[] constraints = constrStr.Trim().Split('\n').Select(relation => Constraint.Parse(relation.Trim())).ToArray();

        //var result = new SA(['Z', 'W'], ['y', 'u'], ['x', 'v']).Run(function, constraints, true);
    }
}
