using System.Windows.Controls;

namespace Lab10.App.Pages;
/// <summary>
/// Interaction logic for BasePage.xaml
/// </summary>
public partial class BasePage : Page {
    public TaskInterface[] Tasks { get; set; } = null!;

    public BasePage() {
        InitializeComponent();
    }
}
