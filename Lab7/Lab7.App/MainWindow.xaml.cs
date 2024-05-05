using Lab7.Core;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lab7.App;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {
    private int[] criterions = null!;
    private LogWindow logWindow = null!;

    public MainWindow() {
        InitializeComponent();
    }

    private void FindOptimalStrategies(object sender, RoutedEventArgs e) {
        this.logWindow?.Close();

        Log.Initiate();
        Log.Clear();

        if (!Matrix.TryParse(this.utilityMatrix.Text, out Matrix matrix)) return;

        criterions = new int[matrix.Height];

        this.wald.Text = this.RecalculateCriterions(new WaldСriterion().Run(matrix));
        this.maximax.Text = this.RecalculateCriterions(new MaximaxCriterion().Run(matrix));
        this.hurwitz.Text = this.RecalculateCriterions(new HurwitzCriterion().Run(matrix, this.factor.Value));
        this.savage.Text = this.RecalculateCriterions(new SavageCriterion().Run(matrix));
        _ = Probabilities.TryParse(this.probabilities.Text, out Probabilities probabilities);
        this.bayes.Text = this.RecalculateCriterions(new BayesCriterion().Run(matrix, probabilities));
        this.laplace.Text = this.RecalculateCriterions(new LaplaceCriterion().Run(matrix));

        this.common.Text = "A" + (criterions.ToList().IndexOf(criterions.Max()) + 1);

        logWindow = new LogWindow(Log.GetLog(true));
        logWindow.Show();
    }

    private string RecalculateCriterions(string data) {
        string pattern = @"(\d+)";
        var matches = Regex.Matches(data, pattern);

        foreach (var match in matches.Cast<Match>()) {
            int index = int.Parse(match.Value) - 1;
            this.criterions[index]++;
        }

        return data;
    }

    private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e) => Application.Current.Shutdown();
}