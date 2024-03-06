using System.Windows;

namespace Lab2.WPF;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application {
    protected override void OnStartup(StartupEventArgs e) {
        base.OnStartup(e);

        var args = ParseArguments(e.Args);
        List<string?> constraints = [];
        for (int i = 0; args.ContainsKey($"c{i}"); i++) {
            constraints.Add(args[$"c{i}"]);
        }
        string[] constArray = [.. constraints];

        MainWindow window = new(args["f"]!, constArray);
        window.Show();
    }

    private Dictionary<string, string?> ParseArguments(string[] args) {
        Dictionary<string, string?> arguments = [];

        for (int i = 0; i < args.Length; i++) {
            if (args[i].StartsWith('-')) {
                string key = args[i][1..].Trim();
                string? value = i + 1 < args.Length ? args[i + 1] : null;
                arguments[key] = value;
                i++;
            }
        }

        return arguments;
    }
}

