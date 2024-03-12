using System.Windows;

namespace Lab3;
/// <summary>
/// Interaction logic for Log.xaml
/// </summary>
public partial class LogWindow : Window {
    public LogWindow() => InitializeComponent();
    public LogWindow(string log) : this() => output.Text = log;
}
