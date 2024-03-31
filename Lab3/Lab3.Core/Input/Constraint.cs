using System.Text.RegularExpressions;

namespace Lab3.Core.Input;

public enum Relation {
    None = -1,
    Equal = 0,
    LessThanOrEqual = 1,
    GreaterThanOrEqual = 2,
}

public sealed class Constraint(double[] coefficients, double constant, Relation relation, string str) : Expression(coefficients, constant, str) {
    private const string RE =
        @"(?:(?<coef>[-]?\d*)(?:x(?<var>\d+)))|((?<rel><=|>=|=)(?<const>[-]?\d+))";

    private readonly Relation _relation = relation;
    public Relation Relation => _relation;

    /// <summary>
    /// Parses an inequality from the string
    /// </summary>
    /// <param name="text">The constraint string</param>
    /// <returns>The <see cref="Constraint"/> object</returns>
    public static Constraint Parse(string text) {
        Regex regex = new(RE);
        MatchCollection matches = regex.Matches(text);

        double constant = 0;
        List<double> coefficients = [];
        Relation relation = Relation.None;

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
                relation = match.Groups["rel"].Value switch {
                    "<=" => Relation.LessThanOrEqual,
                    ">=" => Relation.GreaterThanOrEqual,
                    "=" => Relation.Equal,
                    _ => Relation.None
                };
            }
            if (match.Groups["const"].Success) {
                constant = -double.Parse(match.Groups["const"].Value);
            }
        }

        switch (relation) {
            case Relation.None:
                throw new ArgumentException(nameof(relation));
            case Relation.LessThanOrEqual:
            case Relation.Equal: {
                for (int i = 0; i < coefficients.Count; i++) if (coefficients[i] != 0) coefficients[i] *= -1;
                if (constant != 0) constant *= -1;
                break;
            }
        }

        return new Constraint([.. coefficients], constant, relation, text);
    }
}