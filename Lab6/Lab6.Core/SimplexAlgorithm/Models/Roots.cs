namespace Lab6.Core.SimplexAlgorithm.Models;
internal struct Roots(char variable, double[] values) {
    public char Variable => variable;
    public double[] Values => values;

    public override string ToString() =>
        this.Variable + " = (" + string.Join("; ", this.Values.Select(v => Math.Round(v, Globals.Round))) + ')';
}
