using System.Collections.Immutable;

namespace TicTacToe.Code.Strategies;

/// <summary>
/// Easy difficulty: Computer makes random moves from available positions
/// </summary>
public class EasyStrategy : IComputerStrategy
{
    private readonly ILogger<EasyStrategy>? _logger;

    public EasyStrategy(ILogger<EasyStrategy>? logger = null)
    {
        _logger = logger;
    }

    public Maybe<Position> GetMove(GameBoard board, PieceStyle computerStyle)
    {
        ArgumentNullException.ThrowIfNull(board);

        var blankMoves = GetBlankMoves(board).ToList();
        _logger?.LogDebug($"Easy - Available moves: {blankMoves.Count}");

        if (blankMoves.Count == 0)
        {
            _logger?.LogInformation("Easy - No moves available");
            return Maybe<Position>.None;
        }

        var move = blankMoves[Random.Shared.Next(blankMoves.Count)];
        _logger?.LogInformation($"Easy - Selected random move at ({move.Row}, {move.Col})");
        return Maybe<Position>.Some(move);
    }

    private static IEnumerable<Position> GetBlankMoves(GameBoard board) =>
        from row in Enumerable.Range(0, Constants.BoardSize)
        from col in Enumerable.Range(0, Constants.BoardSize)
        where board.Board[row, col].Style == PieceStyle.Blank
        select new Position(row, col);
}
