using LanguageExt;
using System.Collections.Immutable;

namespace TicTacToe.Code.Strategies;

/// <summary>
/// Hard difficulty: Computer uses advanced strategic play with optimal position prioritization
/// </summary>
public class HardStrategy(ILogger<HardStrategy>? logger = null) : ComputerStrategyBase
{
    private static readonly Position Center = new(1, 1);
    private static readonly ImmutableArray<Position> Corners = [new Position(0, 0), new Position(0, 2), new Position(2, 0), new Position(2, 2)];
    private static readonly ImmutableArray<Position> MiddleEdges  = [new Position(0, 1), new Position(1, 0), new Position(1, 2), new Position(2, 1)];

    protected override Option<Position> ChooseMove(
        GameBoard board, List<Position> blankMoves, PieceStyle computerStyle)
    {
        logger?.LogDebug("Hard - Available moves: {Count}", blankMoves.Count);

        var move = FindWinningMove(board, blankMoves, computerStyle);
        if (move.IsSome) { logger?.LogInformation("Hard - Selected move at ({Row}, {Col})", move.Match(p => p.Row, () => -1), move.Match(p => p.Col, () => -1)); return move; }

        var block = FindWinningMove(board, blankMoves, OpponentOf(computerStyle));
        if (block.IsSome) { logger?.LogInformation("Hard - Selected move at ({Row}, {Col})", block.Match(p => p.Row, () => -1), block.Match(p => p.Col, () => -1)); return block; }

        var optimal = GetOptimalMove(board, blankMoves, computerStyle);
        logger?.LogInformation("Hard - Selected move at ({Row}, {Col})", optimal.Match(p => p.Row, () => -1), optimal.Match(p => p.Col, () => -1));
        return optimal;
    }

    private Option<Position> GetOptimalMove(
        GameBoard board, ICollection<Position> moves, PieceStyle computerStyle)
    {
        var center = CenterMove(board);
        if (center.IsSome) return center;

        var block = BlockThreeCorners(board, moves, computerStyle);
        if (block.IsSome) return block;

        var corner = CornerMove(board);
        if (corner.IsSome) return corner;

        var edge = EdgeMove(board);
        return edge;
    }

    private Option<Position> CenterMove(GameBoard board)
    {
        if (board.Board[Center.Row, Center.Col].Style != PieceStyle.Blank) return Option<Position>.None;
        logger?.LogDebug("Hard - Taking center");
        return Option.Some(Center);
    }

    private Option<Position> BlockThreeCorners(
        GameBoard board, ICollection<Position> moves, PieceStyle computerStyle)
    {
        if (moves.Count != 6 || board.Board[Center.Row, Center.Col].Style != computerStyle)
            return Option<Position>.None;

        var opp = OpponentOf(computerStyle);
        var diagonalThreat =
            (board.Board[0, 0].Style == opp && board.Board[2, 2].Style == opp) ||
            (board.Board[0, 2].Style == opp && board.Board[2, 0].Style == opp);

        if (!diagonalThreat) return Option<Position>.None;

        var edge = BoardUtilities.GetRandomMove(MiddleEdges);
        edge.IfSome(e => logger?.LogDebug("Hard - Blocking three corners via edge ({Row}, {Col})", e.Row, e.Col));
        return edge;
    }

    private Option<Position> CornerMove(GameBoard board)
    {
        var available = Corners.Where(c => board.Board[c.Row, c.Col].Style == PieceStyle.Blank).ToImmutableArray();
        if (available.IsEmpty) return Option<Position>.None;

        var corner = BoardUtilities.GetRandomMove(available);
        corner.IfSome(c => logger?.LogDebug("Hard - Taking corner ({Row}, {Col})", c.Row, c.Col));
        return corner;
    }

    private Option<Position> EdgeMove(GameBoard board)
    {
        var available = MiddleEdges.Where(e => board.Board[e.Row, e.Col].Style == PieceStyle.Blank).ToImmutableArray();
        if (available.IsEmpty) return Option<Position>.None;

        var edge = BoardUtilities.GetRandomMove(available);
        edge.IfSome(e => logger?.LogDebug("Hard - Taking edge ({Row}, {Col})", e.Row, e.Col));
        return edge;
    }
}
