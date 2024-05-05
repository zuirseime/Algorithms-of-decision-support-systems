namespace CalculatingWork.Core.SimplexAlgorithm.Models;
public class Expression(double[] coefficients, double constant, char variable = 'x') {
    public char Variable => variable;
    public double[] Coefficients => coefficients;
    public double Constant { get; set; } = constant;

    public int Order => Coefficients.Length;
    public int Length => Order + 1;

    public double this[int index] {
        get => index < Order ? Coefficients[index] : Constant;
        set {
            if (index < Order) Coefficients[index] = value;
            else Constant = value;
        }
    }
}