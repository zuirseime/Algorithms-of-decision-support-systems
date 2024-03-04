using System.Text.RegularExpressions;

namespace Lab2.Core;

public class Inequality(double[] coefficients, double constant) {
    private const string RE =
        @"(?:(?<coef>[-]?\d*)(?:x(?<var>\d+)))|((?<rel><|>|<=|>=)(?<const>[-]?\d+))";

    private double[] coefficients = coefficients;
    private double constant = constant;

    public double[] Coefficients => coefficients;
    public double Constant => constant;

    public static Inequality Parse(string equation) {
        Regex regex = new(RE);
        MatchCollection matches = regex.Matches(equation);

        double constant = 0;
        List<double> coefficients = [];

        bool leftLessThanRight = false;
        foreach (Match match in matches.Cast<Match>()) {
            if (match.Groups["coef"].Success) {
                string coefficientString = match.Groups["coef"].Value;
                double coefficient = string.IsNullOrEmpty(coefficientString)
                                     ? 1 : string.Equals(coefficientString, "-")
                                     ? -1 : double.Parse(coefficientString);

                int variableIndex = int.Parse(match.Groups["var"].Value) - 1;
                while (variableIndex > coefficients.Count)
                    coefficients.Add(0);
                coefficients.Add(coefficient);
            }
            if (match.Groups["rel"].Success) {
                leftLessThanRight = match.Groups["rel"].Value == "<=" || match.Groups["rel"].Value == "<";
            }
            if (match.Groups["const"].Success) {
                constant = double.Parse(match.Groups["const"].Value);
            }
        }

        if (leftLessThanRight) {
            for (int i = 0; i < coefficients.Count; i++) {
                coefficients[i] *= -1;
            }
            constant *= -1;
        }

        return new Inequality([..coefficients], constant);
    }
}