namespace Lab10.ProjectSchedule;
public class Scheduler {
    private List<ProjectTask> _tasks = [];

    public (string, int) Run(List<string> tasks) {
        FillTaskList(tasks);

        _tasks.ForEach(task => task.Previous.ForEach(prev => prev.Next.Add(task)));

        _tasks.Reverse<ProjectTask>().ToList().ForEach(t => t.SetLate());
        string way = string.Join('-', _tasks.Where(t => t.Critical).Select(i => i.Id));
        int days = _tasks[^1].Early.Finish;

        return (way, days);
    }

    private void FillTaskList(List<string> tasks) {
        foreach (var (task, args) in GetEnumerable(tasks)) {
            if (args.Length != 4)
                throw new ArgumentException($"Task #{tasks.IndexOf(task) - 1} has incorrect number of arguments");

            int[] prevs = args[1].Split(',').Select(int.Parse).ToArray();
            _ = int.TryParse(args[2], out int duration);
            _ = int.TryParse(args[3], out int workers);

            List<ProjectTask> previous = [];
            if (!prevs.Contains(0)) {
                foreach (var id in prevs) {
                    previous.Add(_tasks[id - 1]);
                }
            }

            ProjectTask projectTask = new(previous, duration, workers);
            projectTask.SetEarly();

            _tasks.Add(projectTask);
        }
    }

    private static IEnumerable<(string task, string[] args)> GetEnumerable(List<string> tasks) 
        => from task in tasks let args = task.Split().Select(p => p.Trim()).ToArray() select (task, args);

    private void ConnectPrevious(int[] previous, ProjectTask projectTask) {
        foreach (var prev in from item in previous let prev = _tasks[item - 1] select prev) {
            prev.Next.Add(projectTask);
            projectTask.Previous.Add(prev);
        }
    }
}
