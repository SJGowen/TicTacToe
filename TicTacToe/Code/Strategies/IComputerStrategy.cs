namespace TicTacToe.Code.Strategies;

/// <summary>
/// Defines the interface for computer AI strategies at different difficulty levels
/// </summary>
public interface IComputerStrategy
{
    /// <summary>
    /// Gets the next move based on the current board state and the computer's piece style
    /// </summary>
    Maybe<Position> GetMove(GameBoard board, PieceStyle computerStyle);
}
