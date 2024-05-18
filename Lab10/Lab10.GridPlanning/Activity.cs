using System.ComponentModel;

namespace Lab10.GridPlanning;

public struct Activity : INotifyPropertyChanged, IEditableObject {
    private static int counter = 0;

    struct ActivityData {
        internal string id;
        internal string links;
        internal string duration;
        internal string workers;
    }

    private ActivityData _data;
    private ActivityData _backup;
    private bool _inTxn = false;

    public static void Invalidate(Activity[] activities) => counter = activities.Length;

    public void BeginEdit() {
        if (!_inTxn) {
            _backup = _data;
            _inTxn = true;
        }
    }

    public void CancelEdit() {
        if (_inTxn) {
            _data = _backup;
            _inTxn = false;
        }
    }

    public void EndEdit() {
        if (_inTxn) {
            _backup = new ActivityData();
            _inTxn = false;
        }
    }

    public Activity(string previous, string duration, string workers) {
        _data.id = (++counter).ToString();
        _data.links = previous;
        _data.duration = duration;
        _data.workers = workers;
    }

    public readonly int Id => int.Parse(_data.id);
    public Links Previous {
        get => new(_data.links);
        set {
            _data.links = value.ToString();
            OnActivityChanged(nameof(Previous));
        }
    }
    public int Duration {
        get => int.Parse(_data.duration);
        set {
            _data.duration = value.ToString();
            OnActivityChanged(nameof(Duration));
        }
    }
    public int Workers {
        get => int.Parse(_data.workers);
        set {
            _data.workers = value.ToString();
            OnActivityChanged(nameof(Workers));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnActivityChanged(string property) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }
}

