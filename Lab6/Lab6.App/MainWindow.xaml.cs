using Lab6.Core.MatrixGame;
using Lab6.Core.Output;
using System.Windows;

namespace Lab6.App;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {
    private MGResult? result;
    private HeadedMatrix hMatrix;
    private LogWindow? logWnd = null!;

    private static Random random = new Random();

    public MainWindow() {
        InitializeComponent();
    }

    private void FindSolution(object sender, RoutedEventArgs e) {
        Log.Initiate();

        string matrix = this.matrix.Text;

        if (Matrix.TryParse(matrix, out Matrix temp)) {
            string[] cols = new string[temp.Width];
            string[] rows = new string[temp.Height];

            for (int i = 0; i < cols.Length; i++)
                cols[i] = "Y" + (i + 1);

            for (int i = 0; i < rows.Length; i++)
                rows[i] = "X" + (i + 1);

            this.hMatrix = new HeadedMatrix(temp, cols, rows);
        }


        try {
            logWnd?.Close();

            MG mg = new();
            this.result = mg.Run(matrix);

            this.player1.Text = this.result.Player1.ToString();
            this.player2.Text = this.result.Player2.ToString();
            this.price.Text = this.result.Price.ToString();

            logWnd = new LogWindow(Log.GetLog(true));
            logWnd.Show();
        } catch (Exception ex) {
            MessageBox.Show(ex.StackTrace, ex.Message);
        }
    }

    private void SimulateGame(object sender, RoutedEventArgs e) {
        if (this.result == null) return;

        int num = Convert.ToInt32(Math.Round(this.batches.Value));

        List<Batch> batches = [];
        List<double> gains = [];

        for (int i = 0; i < num; i++) {
            double aRand = Math.Round(random.NextDouble(), 3);
            double bRand = Math.Round(random.NextDouble(), 3);

            double[] aThresholds = new double[this.result.Player1.Length];
            for (int a = 0; a < this.result.Player1.Length; a++) {
                aThresholds[a] = this.result.Player1[a] + (a > 0 ? aThresholds[a - 1] : 0);
            }
            double[] bThresholds = new double[this.result.Player2.Length];
            for (int b = 0; b < this.result.Player2.Length; b++) {
                bThresholds[b] = this.result.Player2[b] + (b > 0 ? bThresholds[b - 1] : 0);
            }

            string aStrategy = "X" + this.GetStrategy(aRand, aThresholds);
            string bStrategy = "Y" + this.GetStrategy(bRand, bThresholds);

            int row = this.hMatrix.Rows.ToList().IndexOf(aStrategy);
            int col = this.hMatrix.Columns.ToList().IndexOf(bStrategy);
            var gain = this.hMatrix[row, col] + i * 0;
            gains.Add(gain);

            double accumualtedGain = gains.Sum();
            double avarageGain = Math.Round(gains.Average(), 2);

            batches.Add(new(i + 1, aRand, aStrategy, bRand, bStrategy, gain, accumualtedGain, avarageGain));
        }

        Simulation sim = new(batches);
        sim.Show();
    }

    private int GetStrategy(double value, double[] thresholds) {
        for (int i = 0; i < thresholds.Length; i++)
            if (value <= thresholds[i]) return i + 1;

        return thresholds.Length;
    }

    private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e) => Application.Current.Shutdown();
}