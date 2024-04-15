using System.Windows;

namespace Lab6.App;
/// <summary>
/// Interaction logic for LogWindow.xaml
/// </summary>
public partial class LogWindow : Window {
    public LogWindow() => InitializeComponent();
    public LogWindow(string log) : this() => output.Text = log;
}
