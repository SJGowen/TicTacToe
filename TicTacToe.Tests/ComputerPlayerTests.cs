using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using TicTacToe.Code;

namespace TicTacToe.Tests;

public class ComputerPlayerTests
{
    private readonly ILogger<ComputerPlayer> _logger;

    public ComputerPlayerTests()
    {
        _logger = NullLogger<ComputerPlayer>.Instance;
    }

    [Fact]
    public void TestGetWinningMove()
    {
        var board = new GameBoard(PlayerType.Human, PlayerType.Computer, _logger);
        var computerPlayer = new ComputerPlayer(PieceStyle.X);

        // Set up a board state where the computer can win
        board.Board[0, 0].Style = PieceStyle.X;
        board.Board[0, 1].Style = PieceStyle.X;
        board.Board[0, 2].Style = PieceStyle.Blank;

        var moveResult = computerPlayer.GetMove(board);
        Assert.True(moveResult.HasValue);
        Assert.Equal(0, moveResult.Value.Row);
        Assert.Equal(2, moveResult.Value.Col);
    }

    [Fact]
    public void TestBlockOpponentWinningMove()
    {
        var board = new GameBoard(PlayerType.Human, PlayerType.Computer, _logger);
        var computerPlayer = new ComputerPlayer(PieceStyle.X);

        // Set up a board state where the opponent can win
        board.Board[0, 0].Style = PieceStyle.O;
        board.Board[0, 1].Style = PieceStyle.O;
        board.Board[0, 2].Style = PieceStyle.Blank;

        var moveResult = computerPlayer.GetMove(board);
        Assert.True(moveResult.HasValue);
        Assert.Equal(0, moveResult.Value.Row);
        Assert.Equal(2, moveResult.Value.Col);
    }

    [Fact]
    public void TestBlockOpponentThreeCorners()
    {
        var board = new GameBoard(PlayerType.Human, PlayerType.Computer, _logger);
        var computerPlayer = new ComputerPlayer(PieceStyle.O);

        // Set up a board state where the opponent can win
        board.Board[0, 0].Style = PieceStyle.X;
        board.Board[1, 1].Style = PieceStyle.O;
        board.Board[2, 2].Style = PieceStyle.X;

        var moveResult = computerPlayer.GetMove(board);
        Assert.True(moveResult.HasValue);
        // The move is to one of the MiddleEdges
        Assert.Contains(new Position(moveResult.Value.Row, moveResult.Value.Col),
            new[] { new Position(0, 1), new Position(1, 0), new Position(1, 2), new Position(2, 1) });
    }

    [Fact]
    public void TestBlockOpponentWinningMoveLogged()
    {
        var board = new GameBoard(PlayerType.Human, PlayerType.Computer, _logger);
        var computerPlayer = new ComputerPlayer(PieceStyle.O);

        // Set up initial board state
        board.Board[1, 1].Style = PieceStyle.X;  // Center
        board.Board[0, 2].Style = PieceStyle.O;  // Top right
        board.Board[2, 2].Style = PieceStyle.X;  // Bottom right

        Debug.WriteLine("=== Test Started ===");
        Debug.WriteLine("\nInitial board state:");
        DebugPrintBoard(board);

        var moveResult = computerPlayer.GetMove(board);
        Assert.True(moveResult.HasValue);
        Debug.WriteLine($"\nO's selected move: ({moveResult.Value.Row}, {moveResult.Value.Col})");
        Debug.WriteLine("\n=== Test Completed Successfully ===");

        Assert.Equal(0, moveResult.Value.Row);
        Assert.Equal(0, moveResult.Value.Col);
    }

    [Fact]
    public void TestOptimalMove()
    {
        var board = new GameBoard(PlayerType.Human, PlayerType.Computer, _logger);
        var computerPlayer = new ComputerPlayer(PieceStyle.X);

        // Set up an empty board
        var moveResult = computerPlayer.GetMove(board);
        Assert.True(moveResult.HasValue);
        Assert.Equal(1, moveResult.Value.Row);
        Assert.Equal(1, moveResult.Value.Col);
    }

    [Fact]
    public void GetMove_WhenOnlyOneMove_ReturnsValidMove()
    {
        // Arrange
        var board = new GameBoard(PlayerType.Human, PlayerType.Computer, _logger);
        var computerPlayer = new ComputerPlayer(PieceStyle.X);

        // Set up initial board state
        board.Board[0, 0].Style = PieceStyle.O;
        board.Board[0, 1].Style = PieceStyle.X;
        board.Board[0, 2].Style = PieceStyle.O;
        board.Board[1, 0].Style = PieceStyle.O;
        board.Board[1, 1].Style = PieceStyle.X;
        board.Board[1, 2].Style = PieceStyle.Blank;
        board.Board[2, 0].Style = PieceStyle.X;
        board.Board[2, 1].Style = PieceStyle.O;
        board.Board[2, 2].Style = PieceStyle.X;
                
        // Act
        var moveResult = computerPlayer.GetMove(board);
        
        // Assert
        Assert.True(moveResult.HasValue);
        Assert.Equal(1, moveResult.Value.Row);
        Assert.Equal(2, moveResult.Value.Col);
    }

    private static void DebugPrintBoard(GameBoard board)
    {
        Debug.WriteLine("-------------");
        for (int row = 0; row < Constants.BoardSize; row++)
        {
            Debug.Write("| ");
            for (int col = 0; col < Constants.BoardSize; col++)
            {
                Debug.Write($"{board.Board[row, col].Style,3}");
                Debug.Write(" | ");
            }
            Debug.WriteLine("");
            Debug.WriteLine("-------------");
        }
    }
}
