using Lab8.Common;
using Lab8.SimplexAlgorithm;
using Lab8.TransportationProblem;
using System.Windows;

namespace Lab8.App;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();
    }

    private void FindOptimalPlan(object sender, RoutedEventArgs e) {
        Log.Initialize();

        if (string.IsNullOrEmpty(_matrix.Text) || string.IsNullOrEmpty(_orders.Text) || string.IsNullOrEmpty(_supplies.Text)) return;

        if (_corner.IsChecked == true) {
            TP tp = new();
            (var feasiblePlan, var optimaPlan) = tp.Run(_matrix.Text, _orders.Text, _supplies.Text);

            _feasiblePlan.Text = feasiblePlan.Matrix.ToString(full: false);
            _optimalPlan.Text = optimaPlan.Matrix.ToString(full: false);
            _feasibleCost.Text = feasiblePlan.Solution.ToString();
            _optimalCost.Text = optimaPlan.Solution.ToString();
        } else if (_simplexMethod.IsChecked == true) {
            ProblemBuilder builder = new();
            builder.Run(_matrix.Text, _supplies.Text, _orders.Text, false);
            SA sa = new(['y'], ['x']);
            (var feasiblePlan, var optimaPlan) = sa.Run(builder.Function, builder.Constraints);

            _feasiblePlan.Text = feasiblePlan.ToString();
            _optimalPlan.Text = optimaPlan.ToString();
            _feasibleCost.Text = feasiblePlan.Value.ToString();
            _optimalCost.Text = optimaPlan.Value.ToString();

        }
    }

    private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e) => Application.Current.Shutdown();
}