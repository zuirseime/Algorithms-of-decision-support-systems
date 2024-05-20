namespace Lab10.ProjectSchedule;

public struct Segment(int start, int finish) : IComparable {
    public int Start { get; set; } = start;
    public int Finish { get; set; } = finish;

    public void Move(int value) {
        Start += value;
        Finish += value;
    }

    public int CompareTo(object? obj) {
        return Finish.CompareTo(((Segment)obj).Finish);
    }
}
