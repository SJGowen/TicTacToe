using LanguageExt;

namespace TicTacToe.Code.Strategies;

/// <summary>
/// Base class for computer AI strategies, providing shared board utilities
/// and enforcing a consistent entry-point contract for all difficulty levels
/// </summary>
public abstract class ComputerStrategyBase : IComputerStrategy
{
    /// <inheritdoc />
    public Option<Position> GetMove(GameBoard board, PieceStyle computerStyle)
    {
        ArgumentNullException.ThrowIfNull(board);
        var blankMoves = BoardUtilities.GetBlankMoves(board).ToList();
        return blankMoves.Count == 0
            ? Option<Position>.None
            : ChooseMove(board, blankMoves, computerStyle);
    }

    /// <summary>
    /// Selects a move given a non-empty list of available positions.
    /// Guaranteed: <paramref name="board"/> is non-null and <paramref name="blankMoves"/> is non-empty.
    /// </summary>
    protected abstract Option<Position> ChooseMove(
        GameBoard board, List<Position> blankMoves, PieceStyle computerStyle);

    /// <summary>Returns the opponent's piece style.</summary>
    protected static PieceStyle OpponentOf(PieceStyle style) =>
        style == PieceStyle.X ? PieceStyle.O : PieceStyle.X;

    /// <summary>
    /// Returns the first move in <paramref name="moves"/> that immediately wins
    /// for <paramref name="style"/>, or <see cref="Option{Position}.None"/>.
    /// </summary>
    protected static Option<Position> FindWinningMove(
        GameBoard board, IEnumerable<Position> moves, PieceStyle style)
    {
        foreach (var move in moves)
            if (BoardUtilities.IsWinningMove(board, move, style))
                return Option.Some(move);

        return Option<Position>.None;
    }
}
