using Lab8.Common;

namespace Lab8.SimplexAlgorithm.Models;
public struct Roots(string variable, double[] values) {
    public string Variable { get; set; } = variable;
    public double[] Values { get; set; } = values;
    public readonly int Length => Values.Length;

    public double this[int index] {
        get => index >= 0 && index < Values.Length ? Values[index] : double.NaN;
        internal set {
            if (index >= 0 && index < Values.Length) {
                Values[index] = value;
            }
        }
    }

    public Roots(char variable, double[] values) : this(variable.ToString(), values) { }

    public static Roots Empty => new('\0', []);

    public static Roots operator /(Roots roots, double value) {
        for (int row = 0; row < roots.Length; row++) {
            roots[row] = Math.Round(roots[row] / value, 2);
        }

        return roots;
    }

    public override readonly string ToString() =>
        "(" + string.Join("; ", Values.Select(Globals.Round)) + ')';

    public readonly string ToString(bool fullForm) => Variable + " = " + ToString();
}