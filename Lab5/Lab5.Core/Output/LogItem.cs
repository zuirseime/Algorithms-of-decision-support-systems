namespace Lab5.Core.Output;
internal struct LogItem(string? data, bool important) {
    public string? Data { get; set; } = data;
    public bool Important { get; set; } = important;

    public override string ToString() => $"{this.Data}";
}
