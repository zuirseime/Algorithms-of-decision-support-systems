using Lab9.Common;

namespace Lab9.HungarianMethod;
public static class Program {
    public static void Main(string[] args) {
        Log.Initialize();
        string matrix = """
            2 10 9 7
            15 4 14 8
            13 14 16 11
            4 15 13 19
            """;

        HM hm = new();
        hm.Run(matrix);
    }
}
