namespace Lab6.Core.MatrixGame;

public struct Player(double[] values) {
    private double[] _values = values;

    public readonly int Length => this._values.Length;

    public double this[int index] {
        get => index >= 0 && index < this._values.Length ? _values[index] : double.NaN;
        internal set {
            if (index >= 0 && index < this._values.Length) {
                _values[index] = value;
            }
        }
    }

    public static Player operator /(Player player, double value) {
        for (int row = 0; row < player.Length; row++) {
            player[row] = Math.Round(player[row] / value, 2);
        }

        return player;
    }

    public override string ToString() => '(' + string.Join("; ", this._values.Select(v => Math.Round(v, Globals.Round))) + ')';
}