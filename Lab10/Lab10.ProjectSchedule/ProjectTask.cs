namespace Lab10.ProjectSchedule;

public class ProjectTask(List<ProjectTask> previous, int duration, int workers) {
    public struct Segment(int start, int finish) {
        public int Start { get; set; } = start;
        public int Finish { get; set; } = finish;
    }

    private static int counter;

    public int Id { get; set; } = ++counter;
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
        Early = new Segment(max, max + Duration);
        Console.WriteLine(ToString(1, 4));
    }
    public void SetLate() {
        int min = Next.Count > 0 ? Next.Min(i => i.Late.Start) : Early.Finish;
        Late = new Segment(min - Duration, min);

        Reserve = Late.Finish - Early.Finish;
        Console.WriteLine(ToString(4, 7));
        Critical = Reserve == 0;
    }

    public static void Invalidate(List<ProjectTask> tasks) => counter = tasks.Count;

    private string[] Data => [
        $"    Workers: {Workers}",
        $"    Duration: {Duration}",
        $"    Early start: {Early.Start}",
        $"    Early finish: {Early.Finish}",
        $"    Late start: {Late.Start}",
        $"    Late finish: {Late.Finish}",
        $"    Time reserve: {Reserve}",
    ];

    public override string ToString() {
        return ToString(0, Data.Length);
    }

    private string ToString(int start, int end) {
        string result = $"{(Critical ? "(C) " : "")}Task {Id}:\n";
        result += string.Join("\n", Data.Take(new Range(start, end)));
        return result;
    }
}