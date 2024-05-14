using Lab8.Common;

namespace Lab8.SimplexAlgorithm;
internal class Program {
    public static void Main(string[] args) {
        Log.Initialize();

        string matrix = """
            6 3 2
            2 1 5
            3 4 1
            """;

        string suppliers = "30 20 50";
        string customers = "10 65 25";

        ProblemBuilder builder = new();
        builder.Run(matrix, suppliers, customers, false);
        SA sa = new(['y'], ['x']);
        (var data1, var data2) = sa.Run(builder.Function, builder.Constraints);

        Console.WriteLine(data1);
        Console.WriteLine(data2);
    }
}
