using System.ComponentModel;

namespace Lab10.App;
public class TaskInterface() : INotifyPropertyChanged, IEditableObject {
    private static int counter;

    public event PropertyChangedEventHandler? PropertyChanged;

    private string _previous = null!;
    private string _duration = null!;
    private string _workers = null!;

    private Dictionary<string, object> BackUp() {
        Dictionary<string, object> dict = [];
        var properties = GetType().GetProperties();

        foreach (var property in properties) {
            if (property.CanWrite)
                dict.Add(property.Name, property.GetValue(this)!);
        }

        return dict;
    }

    private Dictionary<string, object>? _backup;

    public int Id { get; set; } = ++counter;
    public string Previous {
        get => _previous; 
        set { 
            _previous = value;
            NotifyPropertyChanged(nameof(Previous));
        }
    }
    public string Duration {
        get => _duration;
        set {
            _duration = value;
            NotifyPropertyChanged(nameof(Duration));
        }
    }
    public string Workers {
        get => _workers;
        set {
            _workers = value;
            NotifyPropertyChanged(nameof(Workers));
        }
    }

    public TaskInterface(string previous, string duration, string workers) : this() {
        Previous = previous;
        Duration = duration;
        Workers = workers;
    }

    public string[] GetArray() => [Id.ToString(), Previous, Duration, Workers];
    public static void Invalidate(IEnumerable<TaskInterface> tasks) => counter = tasks.Count();

    public void NotifyPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

    public void BeginEdit() {
        _backup = BackUp();
    }

    public void CancelEdit() {
        if (_backup == null) return;

        foreach (var item in _backup) {
            var properties = GetType().GetProperties();
            var property = properties.FirstOrDefault(p => p.Name == item.Key);

            property?.SetValue(this, item.Value);
        }
    }

    public void EndEdit() {
        if (_backup != null) {
            _backup.Clear();
            _backup = null;
        }
    }
}
