using LanguageExt;

namespace TicTacToe.Code;

public abstract class Player(PieceStyle style)
{
    public PieceStyle Style { get; } = style;

    public abstract Option<Position> GetMove(GameBoard board);
}
