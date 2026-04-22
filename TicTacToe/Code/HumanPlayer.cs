using LanguageExt;

namespace TicTacToe.Code;

public class HumanPlayer(PieceStyle style) : Player(style)
{
    public override Option<Position> GetMove(GameBoard board)
    {
        // Implementation depends on your UI interaction
        return Option<Position>.None;
    }
}