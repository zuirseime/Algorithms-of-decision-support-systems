using System.Text.RegularExpressions;

namespace Lab4.Core.Input;

public sealed class Function(double[] coefficients, double constant, string? str = null) : Expression(coefficients, constant, str) {
    private const string RE =
        @"(?:(?<coef>[-]?\d*)(?:x(?<var>\d+)))|((?<!x)(?<const>[-]?\d+)(?!x))";

    public static Function Parse(string function) {
        Regex regex = new(RE);
        MatchCollection matches = regex.Matches(function);

        double constant = 0;
        List<double> coefficients = [];

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
            if (match.Groups["const"].Success) {
                constant += double.Parse(match.Groups["const"].Value);
            }
        }

        return new Function([.. coefficients], constant);
    }
}
