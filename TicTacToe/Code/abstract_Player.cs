namespace TicTacToe.Code;

public abstract class Player
{
    public PieceStyle Style { get; }

    protected Player(PieceStyle style)
    {
        Style = style;
    }

    public abstract Maybe<Position> GetMove(GameBoard board);
}
