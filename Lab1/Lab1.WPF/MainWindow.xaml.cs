using System.Windows;
using Lab1.Core;

namespace Lab1.WPF;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {
    private static Random random = new();

    private int[] valueLimits = [-9, 10];

    public MainWindow() {
        InitializeComponent();
    }

    private void GenerateMatrices(object sender, RoutedEventArgs e) {
        int cols = (int)colCount.Value;
        int rows = (int)rowCount.Value;

        if (generateA.IsChecked == true) {
            double[,] matrix = new double[rows, cols];

            for (int row = 0; row < rows; row++) {
                for (int col = 0; col < cols; col++) {
                    matrix[row, col] = random.Next(valueLimits[0], valueLimits[1]);
                }
            }

            aText.Text = $"{new Matrix() { Data = matrix }}";
        }

        if (generateB.IsChecked == true) {
            double[,] matrix = new double[rows, 1];

            for (int row = 0; row < rows; row++) {
                matrix[row, 0] = random.Next(valueLimits[0], valueLimits[1]);
            }

            bText.Text = $"{new Matrix() { Data = matrix }}";
        }
    }

    private void CalculateMatrixRank(object sender, RoutedEventArgs e) {
        rank.Text = Matrix.TryParse(aText.Text, out Matrix matrix) ? matrix.Rank().ToString() : "NaN";
    }

    private void CalculateLinearSystem(object sender, RoutedEventArgs e) {
        if (Matrix.TryParse(aText.Text, out Matrix matrixA) && Matrix.TryParse(bText.Text, out Matrix matrixB)) {
            string protocolStr = string.Empty;
            Matrix result = Matrix.SolveLinearSystem(matrixA.Data, matrixB.Data, ref protocolStr);

            resultMatrix.Text = result.ToString();
            protocol.Text = protocolStr;
        }
    }

    private void CalculateInverseMatrix(object sender, RoutedEventArgs e) {
        string protocolStr = string.Empty;
        inverseMatrix.Text = Matrix.TryParse(aText.Text, out Matrix matrix) ? matrix.Invert(ref protocolStr).ToString() : string.Empty;
        protocol.Text = protocolStr;
    }

    private void OnEnterMatrix(object sender, System.Windows.Controls.TextChangedEventArgs e) {
        protocol.Text = string.Empty;
        resultMatrix.Text = string.Empty;
        inverseMatrix.Text = string.Empty;
        rank.Text = string.Empty;
    }
}