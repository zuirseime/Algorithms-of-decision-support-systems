namespace CalculatingWork.Core.SimplexAlgorithm;
public struct SAResult(Roots straight, Roots dual, double solution) {
    public Roots Straight { get; set; } = straight;
    public Roots Dual { get; set; } = dual;
    public double Solution { get; set; } = solution;

    public static SAResult Default => new(Roots.Empty, Roots.Empty, double.NaN);
}
