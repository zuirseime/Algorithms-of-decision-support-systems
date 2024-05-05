namespace CalculatingWork.Core.MatrixGame;
public class MGResult(Roots player1, Roots player2, double price) {
    public Roots Player1 { get; set; } = player1;
    public Roots Player2 { get; set; } = player2;
    public double Price { get; set; } = price;
}
