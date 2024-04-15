using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Lab7.Core;

public class Probabilities(double[] values) {
    public double[] Values { get; set; } = values;

    public double this[int index] {
        get => index >= 0 && index < this.Values.Length ? this.Values[index] : double.NaN;
        set {
            if (index >= 0 && index < this.Values.Length) this.Values[index] = value;
        }
    }

    public static Probabilities Parse(string text) {
        string pattern = @"(\d+(,\d+)*)";
        var matches = Regex.Matches(text, pattern);

        List<double> values = new List<double>();
        foreach (Match match in matches.Cast<Match>()) {
            values.Add(double.Parse(match.Value));
        }

        return new Probabilities(values.ToArray());
    }

    public static bool TryParse(string text, out Probabilities probabilities) {
        probabilities = new Probabilities(Array.Empty<double>());

        try {
            probabilities = Probabilities.Parse(text);
            return true;
        } catch (Exception ex) {
            Debug.WriteLine(ex);
            return false;
        }
    }

    public override string ToString() {
        string result = string.Empty;

        for (int i = 0; i < this.Values.Length; i++)
            result += $"p{i + 1} = {this.Values[i]}; ";

        return result;
    }
}