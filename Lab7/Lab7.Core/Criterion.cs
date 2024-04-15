namespace Lab7.Core;
public abstract class Criterion {
    protected struct Strategy(int position, double value) {
        public string Position { get; set; } = "A" + position;
        public double Value { get; set; } = value;

        public override string ToString() => Position;
    }

    public abstract string Run(Matrix matrix);
}
