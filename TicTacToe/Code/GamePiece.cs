namespace TicTacToe.Code;

public class GamePiece
{
    public PieceStyle Style { get; set; }

    public GamePiece()
    {
        Style = PieceStyle.Blank;
    }

    public GamePiece(PieceStyle style)
    {
        Style = style;
    }
}