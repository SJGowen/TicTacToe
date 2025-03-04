namespace TicTacToe.Code;

public class HumanPlayer : Player
{
    public HumanPlayer(PieceStyle style) : base(style) { }

    public override (int row, int col) GetMove(GameBoard board)
    {
        // Human move logic will be handled by UI, so return (-1, -1) as a placeholder
        return (-1, -1);
    }
}