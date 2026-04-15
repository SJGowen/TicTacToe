using System.Collections.Immutable;

namespace TicTacToe.Code.Strategies;

/// <summary>
/// Hard difficulty: Computer uses advanced strategic play with optimal position prioritization
/// </summary>
public class HardStrategy(ILogger<HardStrategy>? logger = null) : ComputerStrategyBase
{
    private static readonly Position Center = new(1, 1);
    private static readonly ImmutableArray<Position> Corners      = [new(0, 0), new(0, 2), new(2, 0), new(2, 2)];
    private static readonly ImmutableArray<Position> MiddleEdges  = [new(0, 1), new(1, 0), new(1, 2), new(2, 1)];

    protected override Maybe<Position> ChooseMove(
        GameBoard board, List<Position> blankMoves, PieceStyle computerStyle)
    {
        logger?.LogDebug("Hard - Available moves: {Count}", blankMoves.Count);

        var move = FindWinningMove(board, blankMoves, computerStyle).OrElse(() =>
                   FindWinningMove(board, blankMoves, OpponentOf(computerStyle)).OrElse(() =>
                   GetOptimalMove(board, blankMoves, computerStyle)));

        logger?.LogInformation("Hard - Selected move at ({Row}, {Col})", move.Value.Row, move.Value.Col);
        return move;
    }

    private Maybe<Position> GetOptimalMove(
        GameBoard board, ICollection<Position> moves, PieceStyle computerStyle) =>
            CenterMove(board).OrElse(() =>
            BlockThreeCorners(board, moves, computerStyle).OrElse(() =>
            CornerMove(board).OrElse(() =>
            EdgeMove(board))));

    private Maybe<Position> CenterMove(GameBoard board)
    {
        if (board.Board[Center.Row, Center.Col].Style != PieceStyle.Blank) return Maybe<Position>.None;
        logger?.LogDebug("Hard - Taking center");
        return Maybe<Position>.Some(Center);
    }

    private Maybe<Position> BlockThreeCorners(
        GameBoard board, ICollection<Position> moves, PieceStyle computerStyle)
    {
        if (moves.Count != 6 || board.Board[Center.Row, Center.Col].Style != computerStyle)
            return Maybe<Position>.None;

        var opp = OpponentOf(computerStyle);
        var diagonalThreat =
            (board.Board[0, 0].Style == opp && board.Board[2, 2].Style == opp) ||
            (board.Board[0, 2].Style == opp && board.Board[2, 0].Style == opp);

        if (!diagonalThreat) return Maybe<Position>.None;

        var edge = BoardUtilities.GetRandomMove(MiddleEdges);
        if (edge.HasValue) logger?.LogDebug("Hard - Blocking three corners via edge ({Row}, {Col})", edge.Value.Row, edge.Value.Col);
        return edge;
    }

    private Maybe<Position> CornerMove(GameBoard board)
    {
        var available = Corners.Where(c => board.Board[c.Row, c.Col].Style == PieceStyle.Blank).ToImmutableArray();
        if (available.IsEmpty) return Maybe<Position>.None;

        var corner = BoardUtilities.GetRandomMove(available);
        if (corner.HasValue) logger?.LogDebug("Hard - Taking corner ({Row}, {Col})", corner.Value.Row, corner.Value.Col);
        return corner;
    }

    private Maybe<Position> EdgeMove(GameBoard board)
    {
        var available = MiddleEdges.Where(e => board.Board[e.Row, e.Col].Style == PieceStyle.Blank).ToImmutableArray();
        if (available.IsEmpty) return Maybe<Position>.None;

        var edge = BoardUtilities.GetRandomMove(available);
        if (edge.HasValue) logger?.LogDebug("Hard - Taking edge ({Row}, {Col})", edge.Value.Row, edge.Value.Col);
        return edge;
    }
}
