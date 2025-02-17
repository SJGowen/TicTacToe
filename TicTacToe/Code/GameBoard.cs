using System.Numerics;

namespace TicTacToe.Code;

public class GameBoard
{
    public GamePiece[,] Board { get; private set; }

    public PieceStyle CurrentStyle { get; private set; } = PieceStyle.X;

    public bool GameComplete => GetWinner().HasValue || ItsADraw();

    public GameBoard()
    {
        Board = new GamePiece[3, 3];
        Reset();
    }

    public void PieceClicked(int row, int col)
    {
        if (GameComplete) { return; }

        GamePiece clickedSpace = Board[row, col];

        if (clickedSpace.Style == PieceStyle.Blank)
        {
            clickedSpace.Style = CurrentStyle;

            if (!GameComplete)
            {
                SwitchTurns();
            }
        }
    }

    public string GetGameCompleteMessage()
    {
        var winningPlay = GetWinner();
        return winningPlay.HasValue ? $"{winningPlay.Value.WinningStyle} Wins!" : "It's a Draw!";
    }

    public bool IsGamePieceAWinningPiece(int row, int col)
    {
        var winningPlay = GetWinner();
        return winningPlay.HasValue && winningPlay.Value.WinningMoves?.Contains($"{row},{col}") == true;
    }

    private bool ItsADraw() => !Board.Cast<GamePiece>().Any(piece => piece.Style == PieceStyle.Blank);

    private Maybe<WinningPlay> GetWinner()
    {
        for (int i = 0; i < 3; i++)
        {
            if (Board[i, 0].Style == CurrentStyle && Board[i, 1].Style == CurrentStyle && Board[i, 2].Style == CurrentStyle)
            {
                return Maybe<WinningPlay>.Some(new WinningPlay
                {
                    WinningStyle = CurrentStyle,
                    WinningMoves = [$"{i},0", $"{i},1", $"{i},2"]
                });
            }
            if (Board[0, i].Style == CurrentStyle && Board[1, i].Style == CurrentStyle && Board[2, i].Style == CurrentStyle)
            {
                return Maybe<WinningPlay>.Some(new WinningPlay
                {
                    WinningStyle = CurrentStyle,
                    WinningMoves = [$"0,{i}", $"1,{i}", $"2,{i}"]
                });
            }
        }

        if (Board[0, 0].Style == CurrentStyle && Board[1, 1].Style == CurrentStyle && Board[2, 2].Style == CurrentStyle)
        {
            return Maybe<WinningPlay>.Some(new WinningPlay
            {
                WinningStyle = CurrentStyle,
                WinningMoves = ["0,0", "1,1", "2,2"]
            });
        }

        if (Board[0, 2].Style == CurrentStyle && Board[1, 1].Style == CurrentStyle && Board[2, 0].Style == CurrentStyle)
        {
            return Maybe<WinningPlay>.Some(new WinningPlay
            {
                WinningStyle = CurrentStyle,
                WinningMoves = ["0,2", "1,1", "2,0"]
            });
        }

        return Maybe<WinningPlay>.None;
    }

    private void SwitchTurns()
    {
        CurrentStyle = CurrentStyle == PieceStyle.X ? PieceStyle.O : PieceStyle.X;
    }

    public void Reset()
    {
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                Board[row, col] = new GamePiece { Style = PieceStyle.Blank };
            }
        }
    }
}