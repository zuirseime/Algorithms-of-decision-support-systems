using Lab10.ProjectSchedule;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Media;

namespace Lab10.App.Pages;
/// <summary>
/// Interaction logic for Schedule.xaml
/// </summary>
public partial class Schedule : Page {
    private List<ProjectTask> _tasks = Globals.Scheduler.Tasks;

    public Schedule() {
        InitializeComponent();
        RecalculateChart();
    }

    public void RecalculateChart() {
        List<Point> dataPoints = [];
        foreach (var task in _tasks) {
            LineSeries solidLines = new() {
                Title = $"{(task.Critical ? "(C) " : " ")}Task {task.Id} (Early), {task.Workers} workers",
                ItemsSource = new[] {
                    new KeyValuePair<int, int>(task.Early.Start, task.Id),
                    new KeyValuePair<int, int>(task.Early.Finish, task.Id),
                },
                DependentValuePath = "Value",
                IndependentValuePath = "Key",
                PolylineStyle = (Style)Resources["SolidLineStyle"],
            };
            _chart.Series.Add(solidLines);

            if (task.Critical) continue;

            LineSeries dashedLines = new() {
                Title = $"Task {task.Id} (Late), {task.Workers} workers",
                ItemsSource = new[] {
                    new KeyValuePair<int, int>(task.GetBaseEarly().Start, task.Id),
                    new KeyValuePair<int, int>(task.Late.Finish, task.Id),
                },
                DependentValuePath = "Value",
                IndependentValuePath = "Key",
                PolylineStyle = (Style)Resources["DashedLineStyle"],
            };
            _chart.Series.Add(dashedLines);
        }
    }
}