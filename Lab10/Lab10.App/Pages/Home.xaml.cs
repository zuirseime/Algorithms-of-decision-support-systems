using Lab10.GridPlanning;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Lab10.App.Pages;
/// <summary>
/// Interaction logic for Home.xaml
/// </summary>
public partial class Home : Page {
    private ObservableCollection<Activity> _activities { get; set; } = [];

    public Home() {
        InitializeComponent();
        _data.ItemsSource = _activities;
    }

    private void ActivityCountChanged(object sender, EventArgs e) {
        int count = (int)Math.Round(_activityNumber.Value);

        Activity.Invalidate([.. _activities]);

        if (count > _activities.Count) {
            _activities.Add(new("0", "0", "0"));
        } else if (count < _activities.Count) {
            _activities.RemoveAt(_activities.Count - 1);
        }

        _data.ItemsSource = _activities;
    }
}

public class PageEventArgs(ObservableCollection<Activity> activities) : EventArgs {
    public ObservableCollection<Activity> Activities { get; private set; } = activities;
}
