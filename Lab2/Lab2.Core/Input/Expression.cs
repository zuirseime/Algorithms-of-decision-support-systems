namespace Lab2.Core.Input;
public abstract class Expression(double[] coefficients, double constant) {
    protected double[] coefficients = coefficients;
    protected double constant = constant;

    public double[] Coefficients => coefficients;
    public double Constant => constant;
}
