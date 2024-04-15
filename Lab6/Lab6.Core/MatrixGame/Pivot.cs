using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace Lab6.Core.MatrixGame;

internal struct Pivot(Point position, double value) {
    private double _value = value;

    public Point Position { get; set; } = position;
    public double Value {
        get => Math.Round(_value, Globals.Round);
        set => _value = Math.Round(value, Globals.Round);
    }

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Pivot pivot && Equals(pivot);

    public bool Equals(Pivot other) => this.Position.Equals(other.Position) && this.Value.Equals(other.Value);

    public override int GetHashCode() {
        return HashCode.Combine(Position, Value);
    }
}
