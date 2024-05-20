using Lab10.ProjectSchedule;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;

namespace Lab10.App.Pages; 
/// <summary>
/// Interaction logic for Workload.xaml
/// </summary>
public partial class Workload : Page {
    private List<ProjectTask> _tasks = Globals.Scheduler.Tasks;
    private Dictionary<int, int> _dailyWorkers = new Dictionary<int, int>();

    public Workload() {
        InitializeComponent();

        if (_tasks.Count == 0) return;

        RecalculateChart();
    }

    public void RecalculateChart() {
        foreach (var task in _tasks) {
            for (int day = task.Early.Start + 1; day <= task.Early.Finish; day++) {
                if (_dailyWorkers.ContainsKey(day))
                    _dailyWorkers[day] += task.Workers;
                else
                    _dailyWorkers[day] = task.Workers;
            }
        }

        ColumnSeries series = new() {
            ItemsSource = _dailyWorkers,
            DependentValuePath = "Value",
            IndependentValuePath = "Key"
        };
        _chart.Series.Add(series);
    }
}
