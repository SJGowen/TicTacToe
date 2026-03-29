using Microsoft.Extensions.Logging.Abstractions;
using TicTacToe.Code;
using TicTacToe.Code.Strategies;

namespace TicTacToe.Tests;

public class MediumStrategyTests
{
    [Fact]
    public void GetMove_WithWinningMove_TakesIt()
    {
        // Arrange
        var strategy = new MediumStrategy();
        var board = new GameBoard(PlayerType.Human, PlayerType.Human);
        board.Board[0, 0].Style = PieceStyle.X;
        board.Board[0, 1].Style = PieceStyle.X;
        board.Board[0, 2].Style = PieceStyle.Blank;

        // Act
        var move = strategy.GetMove(board, PieceStyle.X);

        // Assert
        Assert.True(move.HasValue);
        Assert.Equal(0, move.Value.Row);
        Assert.Equal(2, move.Value.Col);
    }

    [Fact]
    public void GetMove_WithOpponentWinningMove_BlocksIt()
    {
        // Arrange
        var strategy = new MediumStrategy();
        var board = new GameBoard(PlayerType.Human, PlayerType.Human);
        board.Board[0, 0].Style = PieceStyle.O;
        board.Board[0, 1].Style = PieceStyle.O;
        board.Board[0, 2].Style = PieceStyle.Blank;

        // Act
        var move = strategy.GetMove(board, PieceStyle.X);

        // Assert
        Assert.True(move.HasValue);
        Assert.Equal(0, move.Value.Row);
        Assert.Equal(2, move.Value.Col);
    }

    [Fact]
    public void GetMove_WithNoTacticalMoves_ReturnsRandomMove()
    {
        // Arrange
        var strategy = new MediumStrategy();
        var board = new GameBoard(PlayerType.Human, PlayerType.Human);
        // Empty board - no tactical moves

        // Act
        var move = strategy.GetMove(board, PieceStyle.X);

        // Assert
        Assert.True(move.HasValue);
        Assert.Equal(PieceStyle.Blank, board.Board[move.Value.Row, move.Value.Col].Style);
    }

    [Fact]
    public void GetMove_WinningMoveBeforeBlocking()
    {
        // Arrange - both winning and blocking moves available
        var strategy = new MediumStrategy();
        var board = new GameBoard(PlayerType.Human, PlayerType.Human);
        board.Board[0, 0].Style = PieceStyle.X;
        board.Board[0, 1].Style = PieceStyle.X;
        board.Board[0, 2].Style = PieceStyle.Blank; // Winning move

        board.Board[1, 0].Style = PieceStyle.O;
        board.Board[1, 1].Style = PieceStyle.O;
        board.Board[1, 2].Style = PieceStyle.Blank; // Blocking move

        // Act
        var move = strategy.GetMove(board, PieceStyle.X);

        // Assert - Should prioritize winning over blocking
        Assert.True(move.HasValue);
        Assert.Equal(0, move.Value.Row);
        Assert.Equal(2, move.Value.Col);
    }

    [Fact]
    public void GetMove_WithNoAvailableMoves_ReturnsNone()
    {
        // Arrange
        var strategy = new MediumStrategy();
        var board = new GameBoard(PlayerType.Human, PlayerType.Human);
        
        // Fill entire board
        for (int row = 0; row < Constants.BoardSize; row++)
        {
            for (int col = 0; col < Constants.BoardSize; col++)
            {
                board.Board[row, col].Style = col % 2 == 0 ? PieceStyle.X : PieceStyle.O;
            }
        }

        // Act
        var move = strategy.GetMove(board, PieceStyle.X);

        // Assert
        Assert.False(move.HasValue);
    }

    [Fact]
    public void GetMove_PrefersWinOverBlock()
    {
        // Arrange
        var strategy = new MediumStrategy();
        var board = new GameBoard(PlayerType.Human, PlayerType.Human);
        
        // Create scenario where we can win in column or row
        board.Board[2, 0].Style = PieceStyle.X;
        board.Board[2, 1].Style = PieceStyle.X;
        board.Board[2, 2].Style = PieceStyle.Blank; // Win by completing row

        // Act
        var move = strategy.GetMove(board, PieceStyle.X);

        // Assert - Must take the winning move
        Assert.True(move.HasValue);
        Assert.Equal(2, move.Value.Row);
        Assert.Equal(2, move.Value.Col);
    }
}
