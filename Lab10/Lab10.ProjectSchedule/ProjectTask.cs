namespace Lab10.ProjectSchedule;

public class ProjectTask(int start, int duration) {
    private static int counter;

    public int Id { get; set; } = ++counter;
    public List<ProjectTask> Previous { get; set; } = [];
    public List<ProjectTask> Next { get; set; } = [];
    public Segment Early { get; set; } = new Segment(start, start + duration);
    public Segment Late { get; set; }
    public int Duration { get; set; } = duration;
    public int Reserve { get; set; }
    public bool Critical { get; set; } = false;

    public void SetLate(int finish) => Late = new Segment(finish - Duration, finish);
    public void CalculateReserve() {
        Reserve = Late.Finish - Early.Finish;
        Critical = Reserve == 0;
    }

    public static void Invalidate(List<ProjectTask> tasks) => counter = tasks.Count;
}

public struct Segment(int start, int finish) {
    public int Start { get; set; } = start;
    public int Finish { get; set; } = finish;
}