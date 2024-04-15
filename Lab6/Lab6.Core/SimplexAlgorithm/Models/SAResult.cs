namespace Lab6.Core.SimplexAlgorithm.Models;
public struct SAResult(string roots, double solution) {
    public string Roots { get; set; } = roots;
    public double Solution { get; set; } = solution;

    public static SAResult Default => new(string.Empty, double.NaN);
}
