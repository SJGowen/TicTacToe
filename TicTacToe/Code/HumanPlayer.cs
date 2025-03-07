namespace TicTacToe.Code;

public class HumanPlayer(PieceStyle style) : Player(style)
{
    public override Maybe<Position> GetMove(GameBoard board)
    {
        // Implementation depends on your UI interaction
        return Maybe<Position>.None;
    }
}