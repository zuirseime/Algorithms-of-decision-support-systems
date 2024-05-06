namespace Lab8.TransportationProblem;
internal class Program {
    public static void Main(string[] args) {
        TP nwc = new NorthWestCorner();

        string matrix = """
            6 3 2
            2 1 5
            3 4 1
            """;

        string po = "30 20 50";
        string pn = "10 65 25";

        nwc.Run(matrix, pn, po);
    }
}
