namespace CalculatingWork.Core.SimplexAlgorithm.Models;
public class Constraints(Constraint[] data) : ICloneable {
    public Constraint[] Data { get; set; } = data;

    public Constraint this[int index] {
        get => this.Data[index];
        set => this.Data[index] = value;
    }

    public static Constraints Parse(string data) {
        Constraint[] constraints = data.Trim().Split('\n').Select(c => Constraint.Parse(c.Trim())).ToArray();

        return new Constraints(constraints);
    }

    public object Clone() {
        Constraint[] cloned = this.Data.Select(c => (Constraint)c.Clone()).ToArray();

        return new Constraints(cloned);
    }
}
