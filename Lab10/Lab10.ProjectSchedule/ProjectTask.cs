namespace Lab10.ProjectSchedule;

public class ProjectTask(int id, List<ProjectTask> previous, int duration, int workers) {
    private Segment _early;

    public int Id { get; set; } = id;
    public List<ProjectTask> Previous { get; set; } = previous;
    public List<ProjectTask> Next { get; set; } = [];
    public Segment Early { get; set; }
    public Segment Late { get; set; }
    public int Duration { get; set; } = duration;
    public int Reserve { get; set; }
    public bool Critical { get; set; } = false;
    public int Workers { get; set; } = workers;

    public void SetEarly() {
        int max = Previous.Count > 0 ? Previous.Max(i => i.Early.Finish) : 0;
        _early = Early = new Segment(max, max + Duration);
        Console.WriteLine(ToString(1, 4));
    }
    public void SetLate() {
        int min = Next.Count > 0 ? Next.Min(i => i.Late.Start) : Early.Finish;
        Late = new Segment(min - Duration, min);

        Reserve = Late.Finish - Early.Finish;
        Console.WriteLine(ToString(4, 7));
        Critical = Reserve == 0;
    }

    public Segment GetBaseEarly() => _early;

    private string[] Data => [
        $"    Workers: {Workers}",
        $"    Duration: {Duration}",
        $"    Early start: {Early.Start}",
        $"    Early finish: {Early.Finish}",
        $"    Late start: {Late.Start}",
        $"    Late finish: {Late.Finish}",
        $"    Time reserve: {Reserve}",
    ];

    public bool Move(int value) {
        Segment newEarly = new(Early.Start + value, Early.Finish + value);
        if (newEarly.Start >= _early.Start && newEarly.Finish <= Late.Finish) {
            Early = newEarly;
            return true;
        }
        return false;
    }

    public override string ToString() {
        return ToString(0, Data.Length);
    }

    public string ToString(int start, int end) {
        string result = $"{(Critical ? "(C) " : "")}Task {Id}:\n";
        result += string.Join("\n", Data.Take(new Range(start, end)));
        return result;
    }
}