using Lab10.GridPlanning;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Lab10.App;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {
    private static string PAGES = "Pages";

    private Activity[] _activities = null!;

    public MainWindow() {
        InitializeComponent();
        Navigate("Home.xaml");
    }

    private void ChangePage(object sender, RoutedEventArgs e) {
        if (sender is Button button) {
            Navigate(button.Tag.ToString()!);
        }
    }

    private void Navigate(string path) {
        _frame.Navigate(new(Path.Combine(PAGES, path), UriKind.Relative));
    }
}