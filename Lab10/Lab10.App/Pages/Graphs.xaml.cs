using Lab10.ProjectSchedule;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Lab10.App.Pages; 
/// <summary>
/// Interaction logic for Schedule.xaml
/// </summary>
public partial class Graphs : Page {
    private List<ProjectTask> _tasks = Globals.Scheduler.Tasks;
    private ProjectTask _current;

    public Graphs() {
        InitializeComponent();

        _tasksCombo.ItemsSource = Enumerable.Range(0, _tasks.Count).Select(t => $"Task {t + 1}").ToList();
    }

    private void CurrentTaskChanged(object sender, SelectionChangedEventArgs e) {
        if (sender is not ComboBox comboBox) return;

        int id = int.Parse(Regex.Match(comboBox.SelectedItem.ToString(), @"\d+").Value);
                
        _current = _tasks.Where(t => t.Id == id).First();
    }

    private void MoveTask(object sender, System.Windows.RoutedEventArgs e) {
        if (sender is not Button button) return;
        if (_current == null) return;

        int direction = int.Parse(button.Tag as string);

        if (_current.Move(direction)) {
            _schedule.Refresh();
            _workload.Refresh();
        }
    }
}
