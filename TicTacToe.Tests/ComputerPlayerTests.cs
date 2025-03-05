using System;
using TicTacToe.Code;
using Bunit;

namespace TicTacToe.Tests;

public class ComputerPlayerTests
{
    [Fact]
    public void TestGetWinningMove()
    {
        var board = new GameBoard(PlayerType.Human, PlayerType.Computer);
        var computerPlayer = new ComputerPlayer(PieceStyle.X);

        // Set up a board state where the computer can win
        board.Board[0, 0].Style = PieceStyle.X;
        board.Board[0, 1].Style = PieceStyle.X;
        board.Board[0, 2].Style = PieceStyle.Blank;

        var move = computerPlayer.GetMove(board);
        Assert.Equal((0, 2), move);
    }

    [Fact]
    public void TestBlockOpponentWinningMove()
    {
        var board = new GameBoard(PlayerType.Human, PlayerType.Computer);
        var computerPlayer = new ComputerPlayer(PieceStyle.X);

        // Set up a board state where the opponent can win
        board.Board[0, 0].Style = PieceStyle.O;
        board.Board[0, 1].Style = PieceStyle.O;
        board.Board[0, 2].Style = PieceStyle.Blank;

        var move = computerPlayer.GetMove(board);
        Assert.Equal((0, 2), move);
    }

    [Fact]
    public void TestOptimalMove()
    {
        var board = new GameBoard(PlayerType.Human, PlayerType.Computer);
        var computerPlayer = new ComputerPlayer(PieceStyle.X);

        // Set up an empty board
        var move = computerPlayer.GetMove(board);
        Assert.Equal((1, 1), move); // Center is the optimal move
    }
}
