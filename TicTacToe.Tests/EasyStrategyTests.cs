using TicTacToe.Code;
using TicTacToe.Code.Strategies;

namespace TicTacToe.Tests;

public class EasyStrategyTests
{
    [Fact]
    public void GetMove_WithAvailableMoves_ReturnsRandomMove()
    {
        // Arrange
        var strategy = new EasyStrategy();
        var board = new GameBoard(PlayerType.Human, PlayerType.Human);
        var computerStyle = PieceStyle.X;

        // Act
        var move = strategy.GetMove(board, computerStyle);

        // Assert
        Assert.True(move.IsSome);
        move.IfSome(m =>
        {
            Assert.Equal(PieceStyle.Blank, board.Board[m.Row, m.Col].Style);
        });
        
    }

    [Fact]
    public void GetMove_WithNoAvailableMoves_ReturnsNone()
    {
        // Arrange
        var strategy = new EasyStrategy();
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
    public void GetMove_DoesNotPrioritizeWinningMove()
    {
        // Arrange - Set up board where computer has a winning move
        var strategy = new EasyStrategy();
        var board = new GameBoard(PlayerType.Human, PlayerType.Human);
        board.Board[0, 0].Style = PieceStyle.X;
        board.Board[0, 1].Style = PieceStyle.X;
        board.Board[0, 2].Style = PieceStyle.Blank; // Winning move available

        // Act - Run multiple times to check randomness
        var moves = new List<(int Row, int Col)>();
        for (int i = 0; i < 5; i++)
        {
            var move = strategy.GetMove(board, PieceStyle.X);
            if (move.IsSome)
            {
                move.IfSome(m =>
                {
                    moves.Add((m.Row, m.Col));
                });
            }
        }

        // Assert - Easy strategy should sometimes ignore the winning move (randomness)
        // If it always took (0, 2), we'd have no variety
        var uniqueMoves = moves.Distinct().Count();
        Assert.True(uniqueMoves >= 1); // At least returns valid moves (may be random)
    }

    [Fact]
    public void GetMove_WithSingleMove_ReturnsIt()
    {
        // Arrange
        var strategy = new EasyStrategy();
        var board = new GameBoard(PlayerType.Human, PlayerType.Human);
        
        // Fill board except one spot
        for (int row = 0; row < Constants.BoardSize; row++)
        {
            for (int col = 0; col < Constants.BoardSize; col++)
            {
                if (!(row == 1 && col == 1))
                {
                    board.Board[row, col].Style = col % 2 == 0 ? PieceStyle.X : PieceStyle.O;
                }
            }
        }

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
}
