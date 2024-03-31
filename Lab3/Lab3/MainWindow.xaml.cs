using Lab3.Core.Input;
using Lab3.Core.Mathematics;
using Lab3.Core.Output;
using System.Windows;

namespace Lab3;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {
    Window? logWnd = null!;

    public MainWindow() => InitializeComponent();

    public MainWindow(string? func, string[] constraints) : this() {
        this.func.Text = func;
        this.constraints.Text = string.Join('\n', constraints);
    }

    private void Calculate(object sender, RoutedEventArgs e) {
        if (!string.IsNullOrEmpty(constraints.Text) && !string.IsNullOrEmpty(func.Text)) {
            logWnd?.Close();

            Constraint[] constraintsArray = constraints.Text.Trim().Split('\n').Select(relation => Constraint.Parse(relation.Trim())).ToArray();

            SimplexAlgrorithmResult result = new SimplexAlgrorithm().Run(Function.Parse(func.Text), constraintsArray, max.IsChecked == true);
            roots.Text = result.OptimalSolutionRoots;
            solution.Text = result.OptimalSolution.ToString();

            ShowLog();
        }
    }

    private void ShowLog() {
        if (screenLog.IsChecked == true) {
            logWnd = new LogWindow(Log.Instance.ToString());
            logWnd.Show();
        }
        if (fileLog.IsChecked == true) {
            Log.Instance.Save();
            Log.Instance.Show();
        }
    }

    private void OnLogActivationChanged(object sender, RoutedEventArgs e) {
        if (log.IsChecked != null) {
            if (log.IsChecked == true)
                screenLog.IsEnabled = fileLog.IsEnabled = true;
            else {
                screenLog.IsChecked = fileLog.IsChecked = false;
                screenLog.IsEnabled = fileLog.IsEnabled = false;
            }
        }
    }

    private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e) => Application.Current.Shutdown();
}