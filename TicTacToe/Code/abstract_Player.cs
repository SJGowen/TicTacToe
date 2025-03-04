namespace TicTacToe.Code;

public abstract class Player
{
    public PieceStyle Style { get; }

    protected Player(PieceStyle style)
    {
        Style = style;
    }

    public abstract (int row, int col) GetMove(GameBoard board);
}
