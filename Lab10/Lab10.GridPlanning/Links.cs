namespace Lab10.GridPlanning;
public struct Links(string data) {
    public int[] Data { get; set; } = data.Split([",", ", "], StringSplitOptions.TrimEntries)
                                          .Select(int.Parse).ToArray();

    public override string ToString() {
        return string.Join(", ", Data);
    }
}
