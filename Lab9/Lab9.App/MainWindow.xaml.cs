using Lab9.Common;
using Lab9.HungarianMethod;
using Lab9.SimplexAlgorithm;
using System.Windows;

namespace Lab9.App;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();
    }

    private void FindAssignmentMatrix(object sender, RoutedEventArgs e) {
        Log.Initialize();

        if (string.IsNullOrEmpty(_costs.Text)) {
            _assignments.Text = _cost.Text = string.Empty;
            return;
        }

        if (_hungarian.IsChecked == true) {
            HM hm = new();
            (Matrix matrix, double cost) = hm.Run(_costs.Text);

            _assignments.Text = matrix.ToString();
            _cost.Text = cost.ToString();
        } else if (_simplex.IsChecked == true) {
            ProblemBuilder problemBuilder = new();
            problemBuilder.Run(_costs.Text, false);
            SA sa = new(['y'], ['x']);
            (_, var optimalSolution) = sa.Run(problemBuilder.Function, problemBuilder.Constraints);

            _assignments.Text = optimalSolution.ToString();
            _cost.Text = optimalSolution.Value.ToString();
        }
    }
}