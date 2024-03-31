namespace Lab3.Core.Input;
public abstract class Expression(double[] coefficients, double constant, string str) {
    private readonly string _string = str;

    protected double[] _coefficients = coefficients;
    protected double _constant = constant;

    public double[] Coefficients => _coefficients;
    public double Constant => _constant;
    public int Length => Coefficients.Length + 1;

    public double this[int key] {
        get => key < _coefficients.Length ? _coefficients[key] : _constant;
        set {
            if (key < _coefficients.Length) {
                _coefficients[key] = value;
            } else
                _constant = value;
        }
    }

    public int Order() => Coefficients.Length;

    public override string ToString() => _string;
}
