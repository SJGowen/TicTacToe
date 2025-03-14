namespace TicTacToe.Code;

public class ComputerPlayer(PieceStyle style, ILogger<ComputerPlayer>? logger = null) : Player(style)
{
    private static readonly Position Center = new(1, 1);
    private static readonly Position[] Corners = [new(0, 0), new(0, 2), new(2, 0), new(2, 2)];
    private static readonly Position[] Edges = [new(0, 1), new(1, 0), new(1, 2), new(2, 1)];

    public override Maybe<Position> GetMove(GameBoard board)
    {
        ArgumentNullException.ThrowIfNull(board);

        try
        {
            var blankMoves = GetBlankMoves(board).ToList();
            logger?.LogDebug("Available moves: {Count}", blankMoves.Count);
    
            // If there are no moves available, return None
            if (blankMoves.Count == 0) 
            {
                logger?.LogInformation("No moves available");
                return Maybe<Position>.None;
            }
    
            // Check for winning move
            var winningMove = FindWinningMove(board, blankMoves, Style);
            if (winningMove.HasValue) 
            {
                logger?.LogInformation("Found winning move at position ({Row}, {Col})", 
                    winningMove.Value.Row, winningMove.Value.Col);
                return winningMove;
            }
    
            // Block opponent's winning move
            var opponentStyle = Style == PieceStyle.X ? PieceStyle.O : PieceStyle.X;
            var blockingMove = FindWinningMove(board, blankMoves, opponentStyle);
            if (blockingMove.HasValue) 
            {
                logger?.LogInformation("Found blocking move at position ({Row}, {Col})", 
                    blockingMove.Value.Row, blockingMove.Value.Col);
                return blockingMove;
            }
    
            var optimalMove = GetOptimalMove(board, blankMoves);
            logger?.LogInformation("Selected optimal move at position ({Row}, {Col})", 
                optimalMove.Value.Row, optimalMove.Value.Col);
            return optimalMove;
    
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "An error occurred while getting computer move");
            throw;
        }    
    }

    private Maybe<Position> FindWinningMove(GameBoard board, List<Position> moves, PieceStyle style)
    {
        logger?.LogDebug("Searching for winning move for {Style}", style);
        foreach (var move in moves)
        {
            if (IsWinningMove(board, move, style))
            {
                logger?.LogDebug("Found winning move for {Style} at position ({Row}, {Col})", 
                    style, move.Row, move.Col);
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
            var randomEdge = GetRandomMove(Edges);
            if (randomEdge.HasValue)
            {
                logger?.LogDebug("Taking edge position ({Row}, {Col}) to stop opponent getting three corners",
                    randomEdge.Value.Row, randomEdge.Value.Col);
                return randomEdge;
            }
        }
        return Maybe<Position>.None;
    }

    private Maybe<Position> CornerMove(GameBoard board)
    {
        var availableCorners = Corners.Where(corner =>
            board.Board[corner.Row, corner.Col].Style == PieceStyle.Blank).ToArray();
        logger?.LogDebug("Available corner positions: {Count}", availableCorners.Length);

        if (availableCorners.Length > 0)
        {
            var randomCorner = GetRandomMove(availableCorners);
            if (randomCorner.HasValue)
            {
                logger?.LogDebug("Taking corner position ({Row}, {Col}) as optimal move",
                    randomCorner.Value.Row, randomCorner.Value.Col);
                return randomCorner;
            }
        }
        return Maybe<Position>.None;
    }

    private Maybe<Position> EdgeMove(GameBoard board)
    {
        var availableEdges = Edges.Where(edge =>
            board.Board[edge.Row, edge.Col].Style == PieceStyle.Blank).ToArray();
        logger?.LogDebug("Available edge positions: {Count}", availableEdges.Length);

        if (availableEdges.Length > 0)
        {
            var randomEdge = GetRandomMove(availableEdges);
            if (randomEdge.HasValue)
            {
                logger?.LogDebug("Taking edge position ({Row}, {Col}) as optimal move",
                    randomEdge.Value.Row, randomEdge.Value.Col);
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

    private static Maybe<Position> GetRandomMove(Position[] squares)
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