using CalculatingWork.Core;
using CalculatingWork.Core.MulticriteriaOptimization;
using System.Windows;
using System.Windows.Controls;

namespace CalculatingWork.App;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {
    private double _baseHeight;
    private double _fullHeight;

    private List<Example> _examples = [
        new Example("2x1+2x2+x3+x4+x5 max\nx1-3x2+5x3-x4-2x5 min\nx1-4x2+5x3+9x4-2x5 max",
            "-x1+2x2-x3+2x4+x5=6\nx1+4x2+3x3+2x4+x5=9\nx1+2x2+2x4-x5=2"),
        new Example("x1-8x2+x3+4x4 max\n-x1+3x2+5x3+x4 min\n3x1+x2+x3-x4 max",
            "x1-x2+x3+x4<=2\nx1+x2+x3-x4<=2\n-x1+x2+x3+x4<=2\nx1+x2-x3+x4<=2"),
    ];

    public MainWindow() {
        InitializeComponent();
        Log.Initialize();

        this._fullHeight = (this._baseHeight = this.Height) + (this.log.MaxLines + 1) * (this.FontSize + 2) + this.log.Margin.Top - 1;
    }

    private void FindSolution(object sender, RoutedEventArgs e) {
        Log.Clear();

        if (string.IsNullOrEmpty(this.goals.Text) || string.IsNullOrEmpty(this.constraints.Text)) {
            MessageBox.Show("Goals or Constraints are empty");
            return;
        }

        MOResult result = new MO().Run(this.goals.Text, this.constraints.Text);

        this.optimalVectors.Text = result.OptimalVectors.ToString();
        this.coefficients.Text = result.GoalCoefficients.ToString();
        this.unoptimalSolutions.Text = result.UnoptimalSolutions.ToString();
        this.matrixGame.Text = result.MatrixGame.ToString();
        this.weights.Text = result.Weights.ToString();
        this.compromiseSolution.Text = result.CompromiseSolution.ToString();

        this.log.Visibility = Visibility.Visible;
        this.log.Text = Log.GetLog(true);
        
        if (this.Height != this._fullHeight)
            this.SetCurrentValue(HeightProperty, this._fullHeight);
    }

    private void MenuItem_Click(object sender, RoutedEventArgs e) {
        if (this.Height != this._baseHeight)
            this.SetCurrentValue(HeightProperty, this._baseHeight);

        if (sender is MenuItem menuItem) {
            int id = int.Parse(menuItem.Uid);

            this.goals.Text = this._examples[id].Functions;
            this.constraints.Text = this._examples[id].Constraints;

            this.optimalVectors.Text = string.Empty;
            this.coefficients.Text = string.Empty;
            this.unoptimalSolutions.Text = string.Empty;
            this.matrixGame.Text = string.Empty;
            this.weights.Text = string.Empty;
            this.compromiseSolution.Text = string.Empty;
            this.log.Text = string.Empty;
        }
    }
}