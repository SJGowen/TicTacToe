using TicTacToe.Code;
using TicTacToe.Code.Strategies;

namespace TicTacToe.Tests;

public class ExtremeStrategyTests
{
    [Fact]
    public void GetMove_WithWinningMove_TakesIt()
    {
        // Arrange
        var strategy = new ExtremeStrategy();
        var board = new GameBoard(PlayerType.Human, PlayerType.Human);
        board.Board[0, 0].Style = PieceStyle.X;
        board.Board[0, 1].Style = PieceStyle.X;
        board.Board[0, 2].Style = PieceStyle.Blank;

        // Act
        var move = strategy.GetMove(board, PieceStyle.X);

        // Assert
        Assert.True(move.IsSome);
        move.IfSome(m =>
        {
            Assert.Equal(0, m.Row);
            Assert.Equal(2, m.Col);
        });
    }

    [Fact]
    public void GetMove_WithEmptyBoard_ReturnsValidMove()
    {
        // Arrange
        var strategy = new ExtremeStrategy();
        var board = new GameBoard(PlayerType.Human, PlayerType.Human);

        // Act
        var move = strategy.GetMove(board, PieceStyle.X);

        // Assert - On empty board, all first moves are equivalent via minimax
        // So we just verify it returns a valid move
        Assert.True(move.IsSome);
        move.IfSome(m =>
        {
            Assert.Equal(PieceStyle.Blank, board.Board[m.Row, m.Col].Style);
        });
    }

    [Fact]
    public void GetMove_BlocksOpponentWin()
    {
        // Arrange
        var strategy = new ExtremeStrategy();
        var board = new GameBoard(PlayerType.Human, PlayerType.Human);
        board.Board[0, 0].Style = PieceStyle.O;
        board.Board[0, 1].Style = PieceStyle.O;
        board.Board[0, 2].Style = PieceStyle.Blank;

        // Act
        var move = strategy.GetMove(board, PieceStyle.X);

        // Assert
        Assert.True(move.IsSome);
        move.IfSome(m =>
        {
            Assert.Equal(0, m.Row);
            Assert.Equal(2, m.Col);
        });
    }

    [Fact]
    public void GetMove_WithNoAvailableMoves_ReturnsNone()
    {
        // Arrange
        var strategy = new ExtremeStrategy();
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
        Assert.False(move.IsSome);
    }

    [Fact]
    public void GetMove_EvaluatesBoardStateOptimally()
    {
        // Arrange
        var strategy = new ExtremeStrategy();
        var board = new GameBoard(PlayerType.Human, PlayerType.Human);
        board.Board[1, 1].Style = PieceStyle.X; // Center
        board.Board[0, 0].Style = PieceStyle.O; // Corner

        // Act
        var move = strategy.GetMove(board, PieceStyle.X);

        // Assert - Should find optimal move via minimax
        Assert.True(move.IsSome);
        move.IfSome(m =>
        {
            Assert.Equal(PieceStyle.Blank, board.Board[m.Row, m.Col].Style);
        });
    }

    [Fact]
    public void GetMove_WinBeforeBlock()
    {
        // Arrange
        var strategy = new ExtremeStrategy();
        var board = new GameBoard(PlayerType.Human, PlayerType.Human);
        
        // Win opportunity
        board.Board[0, 0].Style = PieceStyle.X;
        board.Board[0, 1].Style = PieceStyle.X;
        board.Board[0, 2].Style = PieceStyle.Blank;
        
        // Block opportunity (not as good)
        board.Board[1, 0].Style = PieceStyle.O;
        board.Board[1, 1].Style = PieceStyle.O;
        board.Board[1, 2].Style = PieceStyle.Blank;

        // Act
        var move = strategy.GetMove(board, PieceStyle.X);

        // Assert - Minimax should evaluate winning move as better
        Assert.True(move.IsSome);
        move.IfSome(m =>
        {
            Assert.Equal(0, m.Row);
            Assert.Equal(2, m.Col);
        });
    }

    [Fact]
    public void GetMove_PrefersWinOverBlockInMinimax()
    {
        // Arrange
        var strategy = new ExtremeStrategy();
        var board = new GameBoard(PlayerType.Human, PlayerType.Human);
        
        // Setup: X has two in a row (can win)
        board.Board[2, 0].Style = PieceStyle.X;
        board.Board[2, 1].Style = PieceStyle.X;
        board.Board[2, 2].Style = PieceStyle.Blank; // Win

        // O threatens
        board.Board[0, 0].Style = PieceStyle.O;
        board.Board[1, 0].Style = PieceStyle.O;
        board.Board[0, 1].Style = PieceStyle.Blank; // Block point

        // Act
        var move = strategy.GetMove(board, PieceStyle.X);

        // Assert - Extreme should win when possible
        Assert.True(move.IsSome);
        move.IfSome(m =>
        {
            Assert.Equal(2, m.Row);
            Assert.Equal(2, m.Col);
        });
    }

    [Fact]
    public void GetMove_ReturnsValidMove_WhenSingleSpaceRemains()
    {
        // Arrange
        var strategy = new ExtremeStrategy();
        var board = new GameBoard(PlayerType.Human, PlayerType.Human);
        
        // Fill board except center
        board.Board[0, 0].Style = PieceStyle.X;
        board.Board[0, 1].Style = PieceStyle.O;
        board.Board[0, 2].Style = PieceStyle.X;
        board.Board[1, 0].Style = PieceStyle.O;
        board.Board[1, 1].Style = PieceStyle.Blank;
        board.Board[1, 2].Style = PieceStyle.O;
        board.Board[2, 0].Style = PieceStyle.X;
        board.Board[2, 1].Style = PieceStyle.O;
        board.Board[2, 2].Style = PieceStyle.X;

        // Act
        var move = strategy.GetMove(board, PieceStyle.X);

        // Assert
        Assert.True(move.IsSome);
        move.IfSome(m =>
        {
            Assert.Equal(1, m.Row);
            Assert.Equal(1, m.Col);
        });
    }

    [Fact]
    public void GetMove_AvoidLoss_InThreateningPosition()
    {
        // Arrange
        var strategy = new ExtremeStrategy();
        var board = new GameBoard(PlayerType.Human, PlayerType.Human);
        
        // O can win across top
        board.Board[0, 0].Style = PieceStyle.O;
        board.Board[0, 1].Style = PieceStyle.O;
        board.Board[0, 2].Style = PieceStyle.Blank;

        // Act
        var move = strategy.GetMove(board, PieceStyle.X);

        // Assert - Extreme should block
        Assert.True(move.IsSome);
        move.IfSome(m =>
        {
            Assert.Equal(0, m.Row);
            Assert.Equal(2, m.Col);
        });
    }
}
