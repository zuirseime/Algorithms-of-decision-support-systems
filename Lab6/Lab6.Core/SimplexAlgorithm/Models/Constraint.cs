using Lab6.Core.Output;
using System.Text.RegularExpressions;

namespace Lab6.Core.SimplexAlgorithm.Models;
public class Constraint(double[] coefficients, double constant, Relation relation, char variable = 'x')
    : Expression(coefficients, constant, variable) {
    private const string PATTERN = @"(?:(?<coef>[-]?\d*)(?:(?<var>[A-Za-z])(?<index>\d+)))|(?<rel><=|>=|=)|(?<const>[-]?\d+)";

    public Relation Relation { get; private set; } = relation;

    public Constraint() : this([], double.NaN, Relation.None) { }

    public void Invert() {
        for (int i = 0; i < Length; i++) {
            this[i] *= -1;
        }

        Relation = Relation switch {
            Relation.GreaterOrEqual => Relation.LessOrEqual,
            Relation.LessOrEqual => Relation.GreaterOrEqual,
            _ => Relation
        };
    }

    public static Constraint Parse(string text) {
        Regex regex = new(PATTERN);
        MatchCollection matches = regex.Matches(text);

        List<double> coefficients = [];
        double constant = 0;
        List<char> variables = [];
        Relation relation = Relation.None;

        int relPos = int.MaxValue;

        foreach (Match match in matches.Cast<Match>()) {
            if (match.Groups["var"] is Group varGroup && varGroup.Success) {
                char variable = char.Parse(varGroup.Value);
                if (variables.Count != 0 && variables[^1] != variable)
                    throw new ArgumentException("Variable names should be the same.");
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

            if (match.Groups["rel"] is Group relGroup && relGroup.Success) {
                relation = match.Groups["rel"].Value switch {
                    "<=" => Relation.LessOrEqual,
                    ">=" => Relation.GreaterOrEqual,
                    "=" => Relation.Equal,
                    _ => Relation.None
                };
                relPos = relGroup.Index;
            }

            if (match.Groups["const"] is Group constGroup && constGroup.Success) {
                double value = double.Parse(constGroup.Value);

                if (constGroup.Index < relPos) constant -= value;
                else constant += value;
            }
        }

        return new Constraint([.. coefficients], constant, relation, variables[^1]);
    }

    public static bool TryParse(string text, out Constraint constraint) {
        constraint = new Constraint();

        try {
            constraint = Parse(text);
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
                    0 => "",
                    1 => $"{Variable}{i + 1}",
                    -1 => $"-{Variable}{i + 1}",
                    _ => $"{this[i]}{Variable}{i + 1}"
                };
                if (!started) started = true;
            }
        }

        result += Relation switch {
            Relation.LessOrEqual => "<=",
            Relation.GreaterOrEqual => ">=",
            Relation.Equal => "=",
            _ => "?"
        } + $"{this[^1]}";

        return result;
    }
}
