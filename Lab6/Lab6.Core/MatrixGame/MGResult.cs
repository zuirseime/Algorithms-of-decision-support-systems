namespace Lab6.Core.MatrixGame;

public class MGResult(Player player1, Player player2, double price) {
    public Player Player1 { get; set; } = player1;
    public Player Player2 { get; set; } = player2;
    public double Price { get; set; } = price;
}
