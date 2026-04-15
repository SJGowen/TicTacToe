namespace TicTacToe.Code.Strategies;

/// <summary>
/// Medium difficulty: Computer prioritizes winning and blocking, then picks a random move
/// </summary>
public class MediumStrategy(ILogger<MediumStrategy>? logger = null) : ComputerStrategyBase
{
    protected override Maybe<Position> ChooseMove(
        GameBoard board, List<Position> blankMoves, PieceStyle computerStyle)
    {
        logger?.LogDebug("Medium - Available moves: {Count}", blankMoves.Count);

        var winningMove = FindWinningMove(board, blankMoves, computerStyle);
        if (winningMove.HasValue)
        {
            logger?.LogInformation("Medium - Winning move at ({Row}, {Col})", winningMove.Value.Row, winningMove.Value.Col);
            return winningMove;
        }

        var blockingMove = FindWinningMove(board, blankMoves, OpponentOf(computerStyle));
        if (blockingMove.HasValue)
        {
            logger?.LogInformation("Medium - Blocking move at ({Row}, {Col})", blockingMove.Value.Row, blockingMove.Value.Col);
            return blockingMove;
        }

        var randomMove = blankMoves[Random.Shared.Next(blankMoves.Count)];
        logger?.LogInformation("Medium - Random move at ({Row}, {Col})", randomMove.Row, randomMove.Col);
        return Maybe<Position>.Some(randomMove);
    }
}
