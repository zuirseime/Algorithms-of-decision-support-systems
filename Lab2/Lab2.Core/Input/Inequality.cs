using System.Text.RegularExpressions;

namespace Lab2.Core.Input;

public sealed class Inequality(double[] coefficients, double constant) : Expression(coefficients, constant) {
    private const string RE =
        @"(?:(?<coef>[-]?\d*)(?:x(?<var>\d+)))|((?<rel><=|>=)(?<const>[-]?\d+))";

    /// <summary>
    /// Parses an inequality from the string
    /// </summary>
    /// <param name="function">The inequality string</param>
    /// <returns>The <see cref="Inequality"/> object</returns>
    public static Inequality Parse(string inequality) {
        Regex regex = new(RE);
        MatchCollection matches = regex.Matches(inequality);

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
                leftLessThanRight = match.Groups["rel"].Value.Contains('<');
            }
            if (match.Groups["const"].Success) {
                constant = -double.Parse(match.Groups["const"].Value);
            }
        }

        if (leftLessThanRight) {
            for (int i = 0; i < coefficients.Count; i++) {
                if (coefficients[i] != 0) coefficients[i] *= -1;
            }
            if (constant != 0) constant *= -1;
        }

        return new Inequality([.. coefficients], constant);
    }
}