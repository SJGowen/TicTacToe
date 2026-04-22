using LanguageExt;

namespace TicTacToe.Code.Strategies;

/// <summary>
/// Easy difficulty: Computer makes random moves from available positions
/// </summary>
public class EasyStrategy(ILogger<EasyStrategy>? logger = null) : ComputerStrategyBase
{
    protected override Option<Position> ChooseMove(
        GameBoard board, List<Position> blankMoves, PieceStyle computerStyle)
    {
        logger?.LogDebug("Easy - Available moves: {Count}", blankMoves.Count);
        var move = blankMoves[Random.Shared.Next(blankMoves.Count)];
        logger?.LogInformation("Easy - Selected random move at ({Row}, {Col})", move.Row, move.Col);
        return Option.Some(move);
    }
}
