using Lab10.ProjectSchedule;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Controls;

namespace Lab10.App.Pages;
/// <summary>
/// Interaction logic for Home.xaml
/// </summary>
public partial class Home : Page {
    private ObservableCollection<TaskInterface> _tasks {
        get => Globals.Tasks;
        set => Globals.Tasks = value;
    }
    private Scheduler _scheduler = Globals.Scheduler;

    public ObservableCollection<TaskInterface> Tasks {
        get => _tasks;
        private set => _tasks = value;
    }

    public Home() {
        InitializeComponent();
        IncreaseRows();
    }

    private void TaskCountChanged(object sender, TextChangedEventArgs e) {
        int count = (int)Math.Round(_taskCount.Value);

        TaskInterface.Invalidate(_tasks);

        while (count != _tasks.Count) {
            if (count > _tasks.Count) IncreaseRows();
            else if (count < _tasks.Count) DecreaseRows();
        }

        _table.ItemsSource = _tasks;
    }

    private void IncreaseRows() => _tasks.Add(new TaskInterface());
    private void DecreaseRows() => _tasks.RemoveAt(_tasks.Count - 1);

    private void FindCriticalWay(object sender, System.Windows.RoutedEventArgs e) {
        string[][] tasks = _tasks.Select(t => t.GetArray()).ToArray();
        (_way.Text, _duration.Text) = _scheduler.Run(tasks);
    }

    private void _table_BeginningEdit(object sender, DataGridBeginningEditEventArgs e) {
        var row = e.Row.Item as TaskInterface;
        if (row != null) row.BeginEdit();
    }
}
