using System.Collections.Immutable;

namespace TicTacToe.Code;

public class ComputerPlayer(PieceStyle style, ILogger<ComputerPlayer>? logger = null) : Player(style)
{
    private static readonly Position Center = new(1, 1);
    private static readonly ImmutableArray<Position> Corners = [new(0, 0), new(0, 2), new(2, 0), new(2, 2)];
    private static readonly ImmutableArray<Position> MiddleEdges = [new(0, 1), new(1, 0), new(1, 2), new(2, 1)];

    /// <summary>
    /// Gets the computer's next move based on the current board state
    /// </summary>
    public override Maybe<Position> GetMove(GameBoard board)
    {
        ArgumentNullException.ThrowIfNull(board);

        try
        {
            var blankMoves = GetBlankMoves(board).ToList();
            logger?.LogDebug($"Available moves: {blankMoves.Count}");

            var move = TryGetMove(board, blankMoves);
            logger?.LogInformation($"Selected move at position ({move.Value.Row}, {move.Value.Col})");
            return move;
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "An error occurred while getting computer move");
            throw;
        }
    }

    private Maybe<Position> TryGetMove(GameBoard board, List<Position> blankMoves)
    {
        if (blankMoves.Count == 0)
        {
            logger?.LogInformation("No moves available");
            return Maybe<Position>.None;
        }

        // Try winning move
        var winningMove = FindWinningMove(board, blankMoves, Style);
        if (winningMove.HasValue) return winningMove;

        // Try blocking move
        var opponentStyle = Style == PieceStyle.X ? PieceStyle.O : PieceStyle.X;
        var blockingMove = FindWinningMove(board, blankMoves, opponentStyle);
        if (blockingMove.HasValue) return blockingMove;

        // Try optimal move
        return GetOptimalMove(board, blankMoves);
    }

    private Maybe<Position> FindWinningMove(GameBoard board, List<Position> moves, PieceStyle style)
    {
        logger?.LogDebug($"Searching for winning move for {style}");
        foreach (var move in moves)
        {
            if (IsWinningMove(board, move, style))
            {
                logger?.LogDebug($"Found winning move for {style} at position ({move.Row}, {move.Col})");
                return Maybe<Position>.Some(move);
            }
        }
        logger?.LogDebug("No winning move found for {Style}", style);
        return Maybe<Position>.None;
    }

    private Maybe<Position> GetOptimalMove(GameBoard board, IEnumerable<Position> moves)
    {
        var movesList = moves as ICollection<Position> ?? moves.ToList();
        if (movesList.Count == 0)
        {
            logger?.LogDebug("No moves available for optimal move selection");
            return Maybe<Position>.None;
        }

        // Try each strategy in order
        var move = CenterMove(board);
        if (move.HasValue) return move;

        move = BlockThreeCorners(board, movesList);
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
            logger?.LogDebug("Taking center position as optimal move");
            return Maybe<Position>.Some(Center);
        }
        return Maybe<Position>.None;
    }

    private Maybe<Position> BlockThreeCorners(GameBoard board, ICollection<Position> moves)
    {
        if (moves.Count != 6 || board.Board[Center.Row, Center.Col].Style != Style)
            return Maybe<Position>.None;

        var opponentStyle = Style == PieceStyle.X ? PieceStyle.O : PieceStyle.X;
        if ((board.Board[0, 0].Style == opponentStyle && board.Board[2, 2].Style == opponentStyle) ||
            (board.Board[0, 2].Style == opponentStyle && board.Board[2, 0].Style == opponentStyle))
        {
            var randomEdge = GetRandomMove(MiddleEdges);
            if (randomEdge.HasValue)
            {
                logger?.LogDebug($"Taking edge position ({randomEdge.Value.Row}, {randomEdge.Value.Col}) " +
                    "to stop opponent getting three corners");
                return randomEdge;
            }
        }
        return Maybe<Position>.None;
    }

    private Maybe<Position> CornerMove(GameBoard board)
    {
        var availableCorners = Corners.Where(corner =>
            board.Board[corner.Row, corner.Col].Style == PieceStyle.Blank).ToArray();
        logger?.LogDebug($"Available corner positions: {availableCorners.Length}");

        if (availableCorners.Length > 0)
        {
            var randomCorner = GetRandomMove([.. availableCorners]);
            if (randomCorner.HasValue)
            {
                logger?.LogDebug($"Taking corner position ({randomCorner.Value.Row}, {randomCorner.Value.Col}) as optimal move");
                return randomCorner;
            }
        }
        return Maybe<Position>.None;
    }

    private Maybe<Position> EdgeMove(GameBoard board)
    {
        var availableEdges = MiddleEdges.Where(edge =>
            board.Board[edge.Row, edge.Col].Style == PieceStyle.Blank);
        logger?.LogDebug($"Available edge positions: {availableEdges.Count()}");

        if (availableEdges.Count() > 0)
        {
            var randomEdge = GetRandomMove([.. availableEdges]);
            if (randomEdge.HasValue)
            {
                logger?.LogDebug($"Taking edge position ({randomEdge.Value.Row}, {randomEdge.Value.Col}) as optimal move");
                return randomEdge;
            }
        }
        return Maybe<Position>.None;
    }

    private static bool IsWinningMove(GameBoard board, Position move, PieceStyle style)
    {
        var newBoard = board.Clone();
        newBoard.Board[move.Row, move.Col].Style = style;
        var winner = newBoard.GetWinner();
        return winner.HasValue && winner.Value.WinningStyle == style;
    }

    private static Maybe<Position> GetRandomMove(ImmutableArray<Position> squares)
    {
        return squares.Length > 0 
            ? Maybe<Position>.Some(squares[Random.Shared.Next(squares.Length)])
            : Maybe<Position>.None;
    }

    private static IEnumerable<Position> GetBlankMoves(GameBoard board) =>
        from row in Enumerable.Range(0, Constants.BoardSize)
        from col in Enumerable.Range(0, Constants.BoardSize)
        where board.Board[row, col].Style == PieceStyle.Blank
        select new Position(row, col);
}