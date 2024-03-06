using Lab2.Core;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Lab2.WPF;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {
    Window? logWnd = null!;

    public MainWindow() {
        InitializeComponent();
    }

    private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e) => Application.Current.Shutdown();

    private void Calculate(object sender, RoutedEventArgs e) {
        if (!string.IsNullOrEmpty(constraints.Text) && !string.IsNullOrEmpty(func.Text)) {
            logWnd?.Close();
            logWnd = null;

            Inequality[] constraintsArray = constraints.Text.Trim().Split('\n').Select(relation => Inequality.Parse(relation.Trim())).ToArray();
            Function function = Function.Parse(func.Text);

            int rows = constraintsArray.Length + 1;
            int cols = constraintsArray.Max(c => c.Coefficients.Length + 1);

            double[,] table = new double[rows, cols];

            for (int row = 0; row < rows - 1; row++)
                for (int col = 0; col < cols; col++)
                    table[row, col] = col != cols - 1 ? constraintsArray[row].Coefficients[col] : constraintsArray[row].Constant;

            for (int col = 0; col < cols; col++)
                table[rows - 1, col] = col != cols - 1 ? function.Coefficients[col] : 0;

            SimplexAlgrorithm simplexAlgrorithm = new();
            SimplexAlgrorithmResult result = simplexAlgrorithm.Run(table, constraints.Text, func.Text, max.IsChecked == true);

            roots.Text = result.OptimalSolutionRoots;
            solution.Text = result.OptimalSolution.ToString();

            if (screenLog.IsChecked == true) {
                logWnd = new LogWindow(Log.Instance.ToString());
                logWnd.Show();
            }
            if (fileLog.IsChecked == true) {
                Log.Instance.Save();
                Log.Instance.Show();
            }
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
}