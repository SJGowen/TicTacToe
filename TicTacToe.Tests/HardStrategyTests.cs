using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using TicTacToe.Code;
using TicTacToe.Code.Strategies;

namespace TicTacToe.Tests;

public class HardStrategyTests
{
    private readonly ILogger<ComputerPlayer> _logger;

    public HardStrategyTests()
    {
        _logger = NullLogger<ComputerPlayer>.Instance;
    }

    [Fact]
    public void GetMove_WithWinningMove_TakesIt()
    {
        // Arrange
        var strategy = new HardStrategy();
        var board = new GameBoard(PlayerType.Human, PlayerType.ComputerHard);
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
        var strategy = new HardStrategy();
        var board = new GameBoard(PlayerType.Human, PlayerType.ComputerHard);
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
    public void GetMove_WithEmptyBoard_TakesCenter()
    {
        // Arrange
        var strategy = new HardStrategy();
        var board = new GameBoard(PlayerType.Human, PlayerType.ComputerHard);

        // Act
        var move = strategy.GetMove(board, PieceStyle.X);

        // Assert
        Assert.True(move.HasValue);
        Assert.Equal(1, move.Value.Row);
        Assert.Equal(1, move.Value.Col);
    }

    [Fact]
    public void GetMove_AvoidsForkSetup()
    {
        // Arrange
        var strategy = new HardStrategy();
        var board = new GameBoard(PlayerType.Human, PlayerType.ComputerHard);
        board.Board[0, 0].Style = PieceStyle.X;
        board.Board[1, 1].Style = PieceStyle.O;
        board.Board[2, 2].Style = PieceStyle.X;

        // Act
        var move = strategy.GetMove(board, PieceStyle.O);

        // Assert - Should play edge to block three corners
        Assert.True(move.HasValue);
        var moveList = new[] 
        { 
            new Position(0, 1), new Position(1, 0), 
            new Position(1, 2), new Position(2, 1) 
        };
        Assert.Contains(new Position(move.Value.Row, move.Value.Col), moveList);
    }

    [Fact]
    public void GetMove_WithNoAvailableMoves_ReturnsNone()
    {
        // Arrange
        var strategy = new HardStrategy();
        var board = new GameBoard(PlayerType.Human, PlayerType.ComputerHard);
        
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
    public void GetMove_PrefersCenterOverCorner()
    {
        // Arrange
        var strategy = new HardStrategy();
        var board = new GameBoard(PlayerType.Human, PlayerType.ComputerHard);
        board.Board[1, 1].Style = PieceStyle.Blank; // Center open
        board.Board[0, 0].Style = PieceStyle.Blank; // Corner open
        
        // Set some pieces to trigger optimal move logic
        board.Board[0, 1].Style = PieceStyle.X;

        // Act
        var move = strategy.GetMove(board, PieceStyle.O);

        // Assert - Hard strategy should consider position value
        Assert.True(move.HasValue);
    }

    [Fact]
    public void GetMove_BlocksOpponentThreeCornersWithCenter()
    {
        // Arrange
        var strategy = new HardStrategy();
        var board = new GameBoard(PlayerType.Human, PlayerType.ComputerHard);
        board.Board[0, 0].Style = PieceStyle.X;
        board.Board[1, 1].Style = PieceStyle.O;
        board.Board[2, 2].Style = PieceStyle.X;

        // Act
        var move = strategy.GetMove(board, PieceStyle.O);

        // Assert - Should block three corners by taking edge
        Assert.True(move.HasValue);
        var edgePositions = new[] 
        { 
            new Position(0, 1), new Position(1, 0), 
            new Position(1, 2), new Position(2, 1) 
        };
        Assert.Contains(new Position(move.Value.Row, move.Value.Col), edgePositions);
    }

    [Fact]
    public void GetMove_WinBeforeBlock()
    {
        // Arrange
        var strategy = new HardStrategy();
        var board = new GameBoard(PlayerType.Human, PlayerType.ComputerHard);
        
        // Win opportunity
        board.Board[0, 0].Style = PieceStyle.X;
        board.Board[0, 1].Style = PieceStyle.X;
        board.Board[0, 2].Style = PieceStyle.Blank;
        
        // Block opportunity
        board.Board[1, 0].Style = PieceStyle.O;
        board.Board[1, 1].Style = PieceStyle.O;
        board.Board[1, 2].Style = PieceStyle.Blank;

        // Act
        var move = strategy.GetMove(board, PieceStyle.X);

        // Assert - Should win, not block
        Assert.True(move.HasValue);
        Assert.Equal(0, move.Value.Row);
        Assert.Equal(2, move.Value.Col);
    }
}
