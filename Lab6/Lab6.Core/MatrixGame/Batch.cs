using System.ComponentModel;

namespace Lab6.Core.MatrixGame;
public class Batch(int id, double numberA, string strategyA, double numberB, string strategyB, double gainA, double accumulatedGainA, double averageGainA) {
    public static int Count;

    public int ID { get; set; } = id;
    public double NumberA { get; set; } = numberA;
    public string StrategyA { get; set; } = strategyA;
    public double NumberB { get; set; } = numberB;
    public string StrategyB { get; set; } = strategyB;
    public double GainA { get; set; } = gainA;
    public double AccumulatedGainA { get; set; } = accumulatedGainA;
    public double AverageGainA { get; set; } = averageGainA;

    public Batch(double numberA, string strategyA, double numberB, string strategyB, double gainA, double accumulatedGainA, double averageGainA) 
        : this(Count++, numberA, strategyA, numberB, strategyB, gainA, accumulatedGainA, averageGainA) { }
}
