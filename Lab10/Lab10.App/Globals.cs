using Lab10.ProjectSchedule;
using System.Collections.ObjectModel;

namespace Lab10.App; 
public static class Globals {
    public static Scheduler Scheduler { get; set; } = new();
    public static ObservableCollection<TaskInterface> Tasks { get; set; } = [];
}
