using Lab8.Common;

namespace Lab8.TransportationProblem;
internal class Program {
    public static void Main(string[] args) {
        Log.Initialize();
        TP nwc = new();

        string matrix = """
            3 1 2 2
            1 5 3 5
            5 8 6 3
            """;

        string suppliers = "20 45 5";
        string customers = "15 20 10 30";

        nwc.Run(matrix, customers, suppliers);
    }
}
