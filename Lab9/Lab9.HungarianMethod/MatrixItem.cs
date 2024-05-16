namespace Lab9.HungarianMethod;

public enum State {
    Zero = 0,
    None = 32,
    Vertical = 179,
    Horizontal = 196,
    Overlaped= 197
}

public struct MatrixItem(double value, State state = State.None) : IComparable {
    public double Value = value;
    public State State = state;

    public readonly int CompareTo(object? obj) => Value.CompareTo(((MatrixItem)obj).Value);

    public override readonly string ToString() => ToString(false);
    public readonly string ToString(bool state) => $"{(state ? (char)State : Globals.Round(Value))}";
}
