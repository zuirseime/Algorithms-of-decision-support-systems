using Lab5.Core.Output;
using Lab5.Core.SimplexAlgorithm;
using Lab5.Core.SimplexAlgorithm.Input;
using Lab5.Core.SimplexAlgorithm.Models;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lab5.App;
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

            SAResult result = new SA(['Z', 'W'], ['y', 'u'], ['x', 'v']).Run(Function.Parse(func.Text), constraintsArray, max.IsChecked == true);
            roots.Text = result.Roots;
            solution.Text = result.Solution.ToString();

            ShowLog();
        }
    }

    private void ShowLog() {
        if (log.IsChecked == true) {
            logWnd = new LogWindow(Log.GetLog(screenLog.IsChecked == true));
            logWnd.Show();

            if (fileLog.IsChecked == true) {
                Log.Save();
                Log.Show();
            }
        }
    }

    private void OnLogActivationChanged(object sender, RoutedEventArgs e) {
        if (log.IsChecked != null && screenLog is not null && fileLog is not null) {
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