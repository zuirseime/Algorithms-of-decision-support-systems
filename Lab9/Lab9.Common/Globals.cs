using System.Drawing;

namespace Lab9.Common;
public class Globals {
    private static int round => 2;

    public static Size MatrixSize { get; set; }

    public static double Round(double value) => Math.Round(value, round);
}
