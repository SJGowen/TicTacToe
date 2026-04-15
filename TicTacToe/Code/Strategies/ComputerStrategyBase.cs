namespace TicTacToe.Code.Strategies;

/// <summary>
/// Base class for computer AI strategies, providing shared board utilities
/// and enforcing a consistent entry-point contract for all difficulty levels
/// </summary>
public abstract class ComputerStrategyBase : IComputerStrategy
{
    /// <inheritdoc />
    public Maybe<Position> GetMove(GameBoard board, PieceStyle computerStyle)
    {
        ArgumentNullException.ThrowIfNull(board);
        var blankMoves = BoardUtilities.GetBlankMoves(board).ToList();
        return blankMoves.Count == 0
            ? Maybe<Position>.None
            : ChooseMove(board, blankMoves, computerStyle);
    }

    /// <summary>
    /// Selects a move given a non-empty list of available positions.
    /// Guaranteed: <paramref name="board"/> is non-null and <paramref name="blankMoves"/> is non-empty.
    /// </summary>
    protected abstract Maybe<Position> ChooseMove(
        GameBoard board, List<Position> blankMoves, PieceStyle computerStyle);

    /// <summary>Returns the opponent's piece style.</summary>
    protected static PieceStyle OpponentOf(PieceStyle style) =>
        style == PieceStyle.X ? PieceStyle.O : PieceStyle.X;

    /// <summary>
    /// Returns the first move in <paramref name="moves"/> that immediately wins
    /// for <paramref name="style"/>, or <see cref="Maybe{Position}.None"/>.
    /// </summary>
    protected static Maybe<Position> FindWinningMove(
        GameBoard board, IEnumerable<Position> moves, PieceStyle style)
    {
        foreach (var move in moves)
            if (BoardUtilities.IsWinningMove(board, move, style))
                return Maybe<Position>.Some(move);

        return Maybe<Position>.None;
    }
}
