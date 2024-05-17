using Lab9.Common;

namespace Lab9.SimplexAlgorithm;
internal class Program {
    public static void Main(string[] args) {
        Log.Initialize();

        string matrix = """
            2 10 9 7
            15 4 14 8
            13 14 16 11
            4 15 13 19
            """;

        ProblemBuilder problemBuilder = new();
        problemBuilder.Run(matrix, false);
        SA sa = new(['y'], ['x']);
        (var data1, var data2) = sa.Run(problemBuilder.Function, problemBuilder.Constraints);

        Console.WriteLine(data2);
    }
}
