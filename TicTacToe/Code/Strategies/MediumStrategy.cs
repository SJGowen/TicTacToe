using System.Collections.Immutable;

namespace TicTacToe.Code.Strategies;

/// <summary>
/// Medium difficulty: Computer prioritizes winning and blocking, then picks random moves
/// </summary>
public class MediumStrategy : IComputerStrategy
{
    private readonly ILogger<MediumStrategy>? _logger;

    public MediumStrategy(ILogger<MediumStrategy>? logger = null)
    {
        _logger = logger;
    }

    public Maybe<Position> GetMove(GameBoard board, PieceStyle computerStyle)
    {
        ArgumentNullException.ThrowIfNull(board);

        var blankMoves = BoardUtilities.GetBlankMoves(board).ToList();
        _logger?.LogDebug($"Medium - Available moves: {blankMoves.Count}");

        if (blankMoves.Count == 0)
        {
            _logger?.LogInformation("Medium - No moves available");
            return Maybe<Position>.None;
        }

        // Try winning move
        var winningMove = FindWinningMove(board, blankMoves, computerStyle);
        if (winningMove.HasValue)
        {
            _logger?.LogInformation($"Medium - Found winning move at ({winningMove.Value.Row}, {winningMove.Value.Col})");
            return winningMove;
        }

        // Try blocking opponent's winning move
        var opponentStyle = computerStyle == PieceStyle.X ? PieceStyle.O : PieceStyle.X;
        var blockingMove = FindWinningMove(board, blankMoves, opponentStyle);
        if (blockingMove.HasValue)
        {
            _logger?.LogInformation($"Medium - Found blocking move at ({blockingMove.Value.Row}, {blockingMove.Value.Col})");
            return blockingMove;
        }

        // Random move
        var randomMove = blankMoves[Random.Shared.Next(blankMoves.Count)];
        _logger?.LogInformation($"Medium - Selected random move at ({randomMove.Row}, {randomMove.Col})");
        return Maybe<Position>.Some(randomMove);
    }

    private Maybe<Position> FindWinningMove(GameBoard board, List<Position> moves, PieceStyle style)
    {
        _logger?.LogDebug($"Medium - Searching for winning move for {style}");
        foreach (var move in moves)
        {
            if (BoardUtilities.IsWinningMove(board, move, style))
            {
                _logger?.LogDebug($"Medium - Found winning move for {style} at ({move.Row}, {move.Col})");
                return Maybe<Position>.Some(move);
            }
        }
        return Maybe<Position>.None;
    }
}
