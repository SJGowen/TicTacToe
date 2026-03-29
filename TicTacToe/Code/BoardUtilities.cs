using System.Collections.Immutable;

namespace TicTacToe.Code;

/// <summary>
/// Utility methods for board operations
/// </summary>
public static class BoardUtilities
{
    /// <summary>
    /// Gets all blank (empty) positions on the game board
    /// </summary>
    public static IEnumerable<Position> GetBlankMoves(GameBoard board)
    {
        return from row in Enumerable.Range(0, Constants.BoardSize)
               from col in Enumerable.Range(0, Constants.BoardSize)
               where board.Board[row, col].Style == PieceStyle.Blank
               select new Position(row, col);
    }

    /// <summary>
    /// Checks if placing a piece at the given position would result in a win
    /// </summary>
    public static bool IsWinningMove(GameBoard board, Position move, PieceStyle style)
    {
        var newBoard = board.Clone();
        newBoard.Board[move.Row, move.Col].Style = style;
        var winner = newBoard.GetWinner();
        return winner.HasValue && winner.Value.WinningStyle == style;
    }

    /// <summary>
    /// Selects a random position from the given array of positions
    /// </summary>
    public static Maybe<Position> GetRandomMove(ImmutableArray<Position> squares)
    {
        return squares.Length > 0
            ? Maybe<Position>.Some(squares[Random.Shared.Next(squares.Length)])
            : Maybe<Position>.None;
    }
}
