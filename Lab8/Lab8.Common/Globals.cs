namespace Lab8.Common;
public class Globals {
    private static int round => 2;
    public static double Round(double value) => Math.Round(value, Globals.round);
}
