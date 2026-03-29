using TicTacToe.Code.Strategies;

namespace TicTacToe.Code;

public class ComputerPlayer(PieceStyle style, IComputerStrategy strategy, ILogger<ComputerPlayer>? logger = null) : Player(style)
{
    private readonly ILogger<ComputerPlayer>? _logger = logger;
    private readonly IComputerStrategy _strategy = strategy;

    /// <summary>
    /// Gets the computer's next move based on the current board state and the configured strategy
    /// </summary>
    public override Maybe<Position> GetMove(GameBoard board)
    {
        ArgumentNullException.ThrowIfNull(board);
        return _strategy.GetMove(board, Style);
    }
}