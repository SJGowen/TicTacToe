using System.Collections.Immutable;

namespace TicTacToe.Code.Strategies;

/// <summary>
/// Hard difficulty: Computer uses advanced strategic play with optimal position prioritization
/// This is the original advanced AI strategy
/// </summary>
public class HardStrategy : IComputerStrategy
{
    private static readonly Position Center = new(1, 1);
    private static readonly ImmutableArray<Position> Corners = [new(0, 0), new(0, 2), new(2, 0), new(2, 2)];
    private static readonly ImmutableArray<Position> MiddleEdges = [new(0, 1), new(1, 0), new(1, 2), new(2, 1)];
    
    private readonly ILogger<HardStrategy>? _logger;

    public HardStrategy(ILogger<HardStrategy>? logger = null)
    {
        _logger = logger;
    }

    public Maybe<Position> GetMove(GameBoard board, PieceStyle computerStyle)
    {
        ArgumentNullException.ThrowIfNull(board);

        try
        {
            var blankMoves = BoardUtilities.GetBlankMoves(board).ToList();
            _logger?.LogDebug($"Hard - Available moves: {blankMoves.Count}");

            var move = TryGetMove(board, blankMoves, computerStyle);
            _logger?.LogInformation($"Hard - Selected move at ({move.Value.Row}, {move.Value.Col})");
            return move;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Hard - An error occurred while getting computer move");
            throw;
        }
    }

    private Maybe<Position> TryGetMove(GameBoard board, List<Position> blankMoves, PieceStyle computerStyle)
    {
        if (blankMoves.Count == 0)
        {
            _logger?.LogInformation("Hard - No moves available");
            return Maybe<Position>.None;
        }

        // Try winning move
        var winningMove = FindWinningMove(board, blankMoves, computerStyle);
        if (winningMove.HasValue) return winningMove;

        // Try blocking move
        var opponentStyle = computerStyle == PieceStyle.X ? PieceStyle.O : PieceStyle.X;
        var blockingMove = FindWinningMove(board, blankMoves, opponentStyle);
        if (blockingMove.HasValue) return blockingMove;

        // Try optimal move
        return GetOptimalMove(board, blankMoves, computerStyle);
    }

    private Maybe<Position> FindWinningMove(GameBoard board, List<Position> moves, PieceStyle style)
    {
        _logger?.LogDebug($"Hard - Searching for winning move for {style}");
        foreach (var move in moves)
        {
            if (BoardUtilities.IsWinningMove(board, move, style))
            {
                _logger?.LogDebug($"Hard - Found winning move for {style} at ({move.Row}, {move.Col})");
                return Maybe<Position>.Some(move);
            }
        }
        _logger?.LogDebug("Hard - No winning move found for {Style}", style);
        return Maybe<Position>.None;
    }

    private Maybe<Position> GetOptimalMove(GameBoard board, IEnumerable<Position> moves, PieceStyle computerStyle)
    {
        var movesList = moves as ICollection<Position> ?? moves.ToList();
        if (movesList.Count == 0)
        {
            _logger?.LogDebug("Hard - No moves available for optimal move selection");
            return Maybe<Position>.None;
        }

        // Try each strategy in order
        var move = CenterMove(board);
        if (move.HasValue) return move;

        move = BlockThreeCorners(board, movesList, computerStyle);
        if (move.HasValue) return move;

        move = CornerMove(board);
        if (move.HasValue) return move;

        move = EdgeMove(board);
        if (move.HasValue) return move;

        return Maybe<Position>.None;
    }

    private Maybe<Position> CenterMove(GameBoard board)
    {
        if (board.Board[Center.Row, Center.Col].Style == PieceStyle.Blank)
        {
            _logger?.LogDebug("Hard - Taking center position as optimal move");
            return Maybe<Position>.Some(Center);
        }
        return Maybe<Position>.None;
    }

    private Maybe<Position> BlockThreeCorners(GameBoard board, ICollection<Position> moves, PieceStyle computerStyle)
    {
        if (moves.Count != 6 || board.Board[Center.Row, Center.Col].Style != computerStyle)
            return Maybe<Position>.None;

        var opponentStyle = computerStyle == PieceStyle.X ? PieceStyle.O : PieceStyle.X;
        if ((board.Board[0, 0].Style == opponentStyle && board.Board[2, 2].Style == opponentStyle) ||
            (board.Board[0, 2].Style == opponentStyle && board.Board[2, 0].Style == opponentStyle))
        {
            var randomEdge = BoardUtilities.GetRandomMove(MiddleEdges);
            if (randomEdge.HasValue)
            {
                _logger?.LogDebug($"Hard - Taking edge ({randomEdge.Value.Row}, {randomEdge.Value.Col}) to block three corners");
                return randomEdge;
            }
        }
        return Maybe<Position>.None;
    }

    private Maybe<Position> CornerMove(GameBoard board)
    {
        var availableCorners = Corners.Where(corner =>
            board.Board[corner.Row, corner.Col].Style == PieceStyle.Blank).ToArray();
        _logger?.LogDebug($"Hard - Available corner positions: {availableCorners.Length}");

        if (availableCorners.Length > 0)
        {
            var randomCorner = BoardUtilities.GetRandomMove([.. availableCorners]);
            if (randomCorner.HasValue)
            {
                _logger?.LogDebug($"Hard - Taking corner ({randomCorner.Value.Row}, {randomCorner.Value.Col})");
                return randomCorner;
            }
        }
        return Maybe<Position>.None;
    }

    private Maybe<Position> EdgeMove(GameBoard board)
    {
        var availableEdges = MiddleEdges.Where(edge =>
            board.Board[edge.Row, edge.Col].Style == PieceStyle.Blank);
        _logger?.LogDebug($"Hard - Available edge positions: {availableEdges.Count()}");

        if (availableEdges.Count() > 0)
        {
            var randomEdge = BoardUtilities.GetRandomMove([.. availableEdges]);
            if (randomEdge.HasValue)
            {
                _logger?.LogDebug($"Hard - Taking edge ({randomEdge.Value.Row}, {randomEdge.Value.Col})");
                return randomEdge;
            }
        }
        return Maybe<Position>.None;
    }
}
