using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using TicTacToe.Code;
using TicTacToe.Code.Strategies;

namespace TicTacToe.Tests;

/// <summary>
/// Tests the ComputerPlayer class and its integration with different strategies
/// Individual strategy behavior is tested in separate test classes
/// </summary>
public class ComputerPlayerTests
{
    private readonly ILogger<ComputerPlayer> _logger;

    public ComputerPlayerTests()
    {
        _logger = NullLogger<ComputerPlayer>.Instance;
    }

    [Fact]
    public void ComputerPlayer_UsesProvidedStrategy()
    {
        // Arrange
        var strategy = new HardStrategy();
        var computerPlayer = new ComputerPlayer(PieceStyle.X, strategy, _logger);
        var board = new GameBoard(PlayerType.Human, PlayerType.ComputerHard);

        board.Board[0, 0].Style = PieceStyle.X;
        board.Board[0, 1].Style = PieceStyle.X;
        board.Board[0, 2].Style = PieceStyle.Blank;

        // Act
        var move = computerPlayer.GetMove(board);

        // Assert - Should delegate to strategy
        Assert.True(move.IsSome);
        move.IfSome(m =>
        {
            Assert.Equal(0, m.Row);
            Assert.Equal(2, m.Col);
        });
    }

    [Fact]
    public void ComputerPlayer_WorksWithEasyStrategy()
    {
        // Arrange
        var strategy = new EasyStrategy();
        var computerPlayer = new ComputerPlayer(PieceStyle.X, strategy);
        var board = new GameBoard(PlayerType.Human, PlayerType.ComputerEasy);

        // Act
        var move = computerPlayer.GetMove(board);

        // Assert - Should return a valid move
        Assert.True(move.IsSome);
    }

    [Fact]
    public void ComputerPlayer_WorksWithMediumStrategy()
    {
        // Arrange
        var strategy = new MediumStrategy();
        var computerPlayer = new ComputerPlayer(PieceStyle.O, strategy);
        var board = new GameBoard(PlayerType.Human, PlayerType.ComputerMedium);

        board.Board[0, 0].Style = PieceStyle.X;
        board.Board[0, 1].Style = PieceStyle.X;
        board.Board[0, 2].Style = PieceStyle.Blank;

        // Act
        var move = computerPlayer.GetMove(board);

        // Assert - Should find the tactical move
        Assert.True(move.IsSome);
    }

    [Fact]
    public void ComputerPlayer_WorksWithExtremeStrategy()
    {
        // Arrange
        var strategy = new ExtremeStrategy();
        var computerPlayer = new ComputerPlayer(PieceStyle.X, strategy);
        var board = new GameBoard(PlayerType.Human, PlayerType.ComputerExtreme);

        // Act
        var move = computerPlayer.GetMove(board);

        // Assert - Should return optimal move via minimax
        Assert.True(move.IsSome);
    }

    [Fact]
    public void ComputerPlayer_UsesStrategyConsistently()
    {
        // Arrange
        var strategy = new HardStrategy();
        var computerPlayer = new ComputerPlayer(PieceStyle.X, strategy, _logger);
        var board = new GameBoard(PlayerType.Human, PlayerType.ComputerHard);
        
        board.Board[0, 0].Style = PieceStyle.X;
        board.Board[0, 1].Style = PieceStyle.X;
        board.Board[0, 2].Style = PieceStyle.Blank;

        // Act
        var moveFromPlayer = computerPlayer.GetMove(board);

        // Assert
        Assert.True(moveFromPlayer.IsSome);
        moveFromPlayer.IfSome(m =>
        {
            Assert.Equal(0, m.Row);
            Assert.Equal(2, m.Col);
        });
    }

    [Fact]
    public void ComputerPlayer_GetMove_ReturnsNone_WhenBoardFull()
    {
        // Arrange
        var strategy = new HardStrategy();
        var computerPlayer = new ComputerPlayer(PieceStyle.X, strategy);
        var board = new GameBoard(PlayerType.Human, PlayerType.ComputerHard);

        // Fill board
        for (int row = 0; row < Constants.BoardSize; row++)
        {
            for (int col = 0; col < Constants.BoardSize; col++)
            {
                board.Board[row, col].Style = col % 2 == 0 ? PieceStyle.X : PieceStyle.O;
            }
        }

        // Act
        var move = computerPlayer.GetMove(board);

        // Assert
        Assert.False(move.IsSome);
    }

    [Fact]
    public void ComputerPlayer_PreservesStyle()
    {
        // Arrange
        var strategy = new HardStrategy();
        var computerPlayer = new ComputerPlayer(PieceStyle.O, strategy);

        // Assert
        Assert.Equal(PieceStyle.O, computerPlayer.Style);
    }

    [Fact]
    public void ComputerPlayer_CallsStrategyGetMove()
    {
        // Arrange
        var strategy = new MediumStrategy();
        var computerPlayer = new ComputerPlayer(PieceStyle.X, strategy);
        var board = new GameBoard(PlayerType.Human, PlayerType.ComputerMedium);
        
        board.Board[0, 0].Style = PieceStyle.X;
        board.Board[0, 1].Style = PieceStyle.X;
        board.Board[0, 2].Style = PieceStyle.Blank;

        // Act - Call GetMove on ComputerPlayer which should delegate to strategy
        var move = computerPlayer.GetMove(board);

        // Assert - Should find the winning move
        Assert.True(move.IsSome);
        move.IfSome(m =>
        {
            Assert.Equal(0, m.Row);
            Assert.Equal(2, m.Col);
        });
    }
}
