using Lab8.SimplexAlgorithm.Models;

namespace Lab8.SimplexAlgorithm;
public struct SAResult(Roots straight, Roots dual, double solution) {
    public Roots Straight { get; set; } = straight;
    public double Solution { get; set; } = solution;

    public static SAResult Default => new(Roots.Empty, Roots.Empty, double.NaN);
}
