namespace Lab10.ProjectSchedule;
public class Scheduler {
    public List<ProjectTask> Tasks { get; private set; } = [];
    private string _way = string.Empty;
    private int _days;

    public (string, string) Run(string[][] tasks) {
        FillTaskList(tasks);

        CalculateEarlyDates();
        CalculateLateDates();

        Console.WriteLine($"""
                            {'\n'}Calculated parameters of the grid schedule:{'\n'}
                            {string.Join('\n', Tasks)}{'\n'}
                            Critical way: {_way}
                            """);

        return (_way, _days.ToString());
    }

    private void CalculateEarlyDates() {
        Console.WriteLine("Early dates:\n");
        Tasks.ForEach(task => task.SetEarly());

        Tasks.ForEach(task => task.Previous.ForEach(prev => prev.Next.Add(task)));
        var ends = Tasks.Where(task => task.Next.Count == 0).ToList();
        if (ends.Count > 1) {
            Segment segment = ends.Max(end => end.Early);
            segment.Start = segment.Finish;
            ProjectTask task = new(Tasks.Count, [], 0, 0) { Early = segment };
            Tasks.Add(task);
            ends.ForEach(end => end.Next.Add(task));
        }
        Console.WriteLine($"\nProject duration: {_days = Tasks[^1].Early.Finish}\n");
    }

    private void CalculateLateDates() {
        Console.WriteLine("Late dates:\n");
        Tasks.Reverse<ProjectTask>().ToList().ForEach(t => t.SetLate());

        _way = string.Join('-', Tasks.Where(t => t.Critical).Select(i => i.Id));
    }

    int zeroCount;
    private void FillTaskList(string[][] tasks) {
        zeroCount = tasks.Where(t => t.Contains("0")).Count();
        if (zeroCount > 1)
            Tasks.Insert(0, new ProjectTask(0, [], 0, 0) { Id = 0 });

        foreach (var (task, args) in GetEnumerable(tasks)) {
            if (args.Length != 4)
                throw new ArgumentException($"Task #{Array.FindIndex(tasks, t => t == task)} has incorrect number of arguments");

            int[] prevs = args[1].Split(',').Select(int.Parse).ToArray();
            _ = int.TryParse(args[0], out int index);
            _ = int.TryParse(args[2], out int duration);
            _ = int.TryParse(args[3], out int workers);

            List<ProjectTask> previous = [];
            if (zeroCount > 1) {
                foreach (var id in prevs) {
                    previous.Add(Tasks[id]);
                }
            } else if (!prevs.Contains(0)) {
                foreach (var id in prevs) {
                    previous.Add(Tasks[id - 1]);
                }
            }

            ProjectTask projectTask = new(index, previous, duration, workers);
            Tasks.Add(projectTask);
        }
    }

    private static IEnumerable<(string[] task, string[] args)> GetEnumerable(string[][] tasks)
        => from task in tasks let args = task.Select(p => p.Trim()).ToArray() select (task, args);

    private void ConnectPrevious(int[] previous, ProjectTask projectTask) {
        foreach (var prev in from item in previous let prev = Tasks[item - 1] select prev) {
            prev.Next.Add(projectTask);
            projectTask.Previous.Add(prev);
        }
    }
}
