using Lab9.Common;

namespace Lab9.HungarianMethod;

public enum State {
    None = '\0',
    Zero = '0',
    Overlaped= '+',
    Horizontal = '-',
    Vertical = '|',
    Erased = 'E',
    Picked = 'P'
}

public struct MatrixItem(double value, State state = State.None) : IComparable {
    public double Value = value;
    public State State = state;

    public static MatrixItem operator -(MatrixItem item1, double item2) => new(item1.Value - item2);
    public static MatrixItem operator +(MatrixItem item1, double item2) => new(item1.Value + item2);

    public readonly int CompareTo(object? obj) => Value.CompareTo(((MatrixItem)obj!).Value);

    public override readonly string ToString() => ToString(false);
    public readonly string ToString(bool state) => $"{((state && State != State.None) ? $"{(char)State}" : Globals.Round(Value))}";
}
