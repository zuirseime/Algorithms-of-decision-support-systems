using Lab6.Core.Output;
using System.Text.RegularExpressions;

namespace Lab6.Core.SimplexAlgorithm.Models;
public sealed class Function(double[] coefficients, double constant, char variable = 'x')
    : Expression(coefficients, constant, variable) {
    private const string PATTERN = @"(?:(?<coef>[-]?\d*)(?:(?<var>[A-Za-z])(?<index>\d+)))|((?<!x)(?<const>[-]?\d+)(?!x))";

    public Function() : this([], double.NaN) { }

    public static Function Parse(string text) {
        Regex regex = new(PATTERN);
        MatchCollection matches = regex.Matches(text);

        List<double> coefficients = [];
        double constant = 0;
        List<char> variables = [];

        foreach (Match match in matches.Cast<Match>()) {
            if (match.Groups["var"] is Group varGroup && varGroup.Success) {
                char variable = char.Parse(varGroup.Value);
                if (variables.Count != 0 && variables[^1] != variable)
                    throw new ArgumentException($"Variable names should be the same.\n{text}");
                variables.Add(variable);
            }

            if (match.Groups["coef"] is Group coefGroup && coefGroup.Success) {
                string coefString = coefGroup.Value;
                double coef = string.IsNullOrEmpty(coefString) ? 1
                            : string.Equals(coefString, "-") ? -1 : double.Parse(coefString);

                int index = int.Parse(match.Groups["index"].Value) - 1;
                while (index > coefficients.Count) coefficients.Add(0);
                coefficients.Add(coef);
            }

            if (match.Groups["const"] is Group constGroup && constGroup.Success)
                constant += double.Parse(constGroup.Value);
        }

        return new Function([.. coefficients], constant, variables[^1]);
    }

    public static bool TryParse(string text, out Function function) {
        function = new Function();

        try {
            function = Parse(text);
            return true;
        } catch (Exception ex) {
            Log.WriteLine(ex);
            return false;
        }
    }

    public override string ToString() {
        string result = string.Empty;

        bool started = false;
        for (int i = 0; i < Length; i++) {
            if (this[i] == 0) continue;

            if (i < Order) {
                result += (started && this[i] > 0 ? "+" : "") + this[i] switch {
                    1 => $"{Variable}{i + 1}",
                    -1 => $"-{Variable}{i + 1}",
                    _ => $"{this[i]}{Variable}{i + 1}"
                };
                if (!started) started = true;
            } else {
                result += (started && this[i] > 0 ? "+" : "") + (this[i] != 0 ? $"{this[i]}" : "");
            }
        }

        return result;
    }
}
