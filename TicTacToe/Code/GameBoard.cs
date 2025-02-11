using System.Numerics;

namespace TicTacToe.Code;

public class GameBoard
{
    public GamePiece[,] Board { get; private set; }

    public PieceStyle CurrentStyle { get; private set; } = PieceStyle.X;

    public bool GameComplete => GetWinner() != null || IsADraw();

    public GameBoard()
    {
        Board = new GamePiece[3, 3];
        Reset();
    }

    public void PieceClicked(int col, int row)
    {
        if (GameComplete) { return; }

        GamePiece clickedSpace = Board[col, row];

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
        return winningPlay != null ? $"{winningPlay.WinningStyle} Wins!" : "It's a Draw!";
    }

    public bool IsGamePieceAWinningPiece(int col, int row)
    {
        var winningPlay = GetWinner();
        return winningPlay?.WinningMoves?.Contains($"{col},{row}") ?? false;
    }

    private bool IsADraw() => !Board.Cast<GamePiece>().Any(piece => piece.Style == PieceStyle.Blank);

    private WinningPlay? GetWinner()
    {
        for (int i = 0; i < 3; i++)
        {
            if (Board[i, 0].Style == CurrentStyle && Board[i, 1].Style == CurrentStyle && Board[i, 2].Style == CurrentStyle)
            {
                return new WinningPlay
                {
                    WinningStyle = CurrentStyle,
                    WinningMoves = [$"{i},0", $"{i},1", $"{i},2"]
                };
            }
            if (Board[0, i].Style == CurrentStyle && Board[1, i].Style == CurrentStyle && Board[2, i].Style == CurrentStyle)
            {
                return new WinningPlay
                {
                    WinningStyle = CurrentStyle,
                    WinningMoves = [$"0,{i}", $"1,{i}", $"2,{i}"]
                };
            }
        }

        if (Board[0, 0].Style == CurrentStyle && Board[1, 1].Style == CurrentStyle && Board[2, 2].Style == CurrentStyle)
        {
            return new WinningPlay
            {
                WinningStyle = CurrentStyle,
                WinningMoves = ["0,0", "1,1", "2,2"]
            };
        }

        if (Board[0, 2].Style == CurrentStyle && Board[1, 1].Style == CurrentStyle && Board[2, 0].Style == CurrentStyle)
        {
            return new WinningPlay
            {
                WinningStyle = CurrentStyle,
                WinningMoves = ["0,2", "1,1", "2,0"]
            };
        }

        return null;
    }

    private void SwitchTurns()
    {
        CurrentStyle = CurrentStyle == PieceStyle.X ? PieceStyle.O : PieceStyle.X;
    }

    public void Reset()
    {
        for (int col = 0; col < 3; col++)
        {
            for (int row = 0; row < 3; row++)
            {
                Board[col, row] = new GamePiece { Style = PieceStyle.Blank };
            }
        }

        CurrentStyle = PieceStyle.X;
    }
}