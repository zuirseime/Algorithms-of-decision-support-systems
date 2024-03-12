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

            Inequality[] constraintsArray = constraints.Text.Trim().Split('\n').Select(relation => Inequality.Parse(relation.Trim())).ToArray();
            double[,] table = GenerateTable(constraintsArray, Function.Parse(func.Text), constraintsArray.Length + 1, constraintsArray.Max(c => c.Coefficients.Length + 1));

            SimplexAlgrorithmResult result = GetSimplexResult(table);
            roots.Text = result.OptimalSolutionRoots;
            solution.Text = result.OptimalSolution.ToString();

            ShowLog();
        }
    }

    private static double[,] GenerateTable(Inequality[] constraintsArray, Function function, int rows, int cols) {
        double[,] table = new double[rows, cols];

        for (int row = 0; row < rows - 1; row++)
            for (int col = 0; col < cols; col++)
                table[row, col] = col != cols - 1 ? constraintsArray[row].Coefficients[col] : constraintsArray[row].Constant;

        for (int col = 0; col < cols; col++)
            table[rows - 1, col] = col != cols - 1 ? function.Coefficients[col] : 0;
        return table;
    }

    private SimplexAlgrorithmResult GetSimplexResult(double[,] table) => new SimplexAlgrorithm().Run(table, constraints.Text, func.Text, max.IsChecked == true);

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