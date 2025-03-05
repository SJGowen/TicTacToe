using System.Numerics;

namespace TicTacToe.Code;

public class GameBoard
{
    public GamePiece[,] Board { get; private set; }
    public PieceStyle CurrentStyle { get; private set; } = PieceStyle.X;
    public bool GameComplete => GetWinner().HasValue || ItsADraw();

    private Player _playerX;
    private Player _playerO;

    public GameBoard(PlayerType playerXType, PlayerType playerOType)
    {
        Board = new GamePiece[3, 3];

        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                Board[row, col] = new GamePiece { Style = PieceStyle.Blank };
            }
        }

        _playerX = CreatePlayer(playerXType, PieceStyle.X);
        _playerO = CreatePlayer(playerOType, PieceStyle.O);
    }

    public GameBoard Clone()
    {
        var clone = new GameBoard(PlayerType.Human, PlayerType.Human)
        {
            CurrentStyle = this.CurrentStyle
        };

        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                clone.Board[row, col] = new GamePiece { Style = this.Board[row, col].Style };
            }
        }

        return clone;
    }

    private Player CreatePlayer(PlayerType playerType, PieceStyle style)
    {
        return playerType switch
        {
            PlayerType.Human => new HumanPlayer(style),
            PlayerType.Computer => new ComputerPlayer(style),
            _ => throw new ArgumentException("Invalid player type")
        };
    }

    public async Task PieceClicked(int row, int col)
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

        await Task.CompletedTask;
    }

    public async Task MakeComputerMoveIfNeededAsync()
    {
        var currentPlayer = CurrentStyle == PieceStyle.X ? _playerX : _playerO;
        if (currentPlayer is ComputerPlayer)
        {
            int row, col;
            (row, col) = currentPlayer.GetMove(this);

            if (row != -1 && col != -1)
            {
                await PieceClicked(row, col);
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

    public Maybe<WinningPlay> GetWinner()
    {
        // Check rows and columns
        for (int i = 0; i < 3; i++)
        {
            if (Board[i, 0].Style != PieceStyle.Blank && Board[i, 0].Style == Board[i, 1].Style && Board[i, 1].Style == Board[i, 2].Style)
            {
                return Maybe<WinningPlay>.Some(new WinningPlay
                {
                    WinningStyle = Board[i, 0].Style,
                    WinningMoves = [$"{i},0", $"{i},1", $"{i},2"]
                });
            }
            if (Board[0, i].Style != PieceStyle.Blank && Board[0, i].Style == Board[1, i].Style && Board[1, i].Style == Board[2, i].Style)
            {
                return Maybe<WinningPlay>.Some(new WinningPlay
                {
                    WinningStyle = Board[0, i].Style,
                    WinningMoves = [$"0,{i}", $"1,{i}", $"2,{i}"]
                });
            }
        }

        // Check diagonals
        if (Board[0, 0].Style != PieceStyle.Blank && Board[0, 0].Style == Board[1, 1].Style && Board[1, 1].Style == Board[2, 2].Style)
        {
            return Maybe<WinningPlay>.Some(new WinningPlay
            {
                WinningStyle = Board[0, 0].Style,
                WinningMoves = ["0,0", "1,1", "2,2"]
            });
        }
        if (Board[0, 2].Style != PieceStyle.Blank && Board[0, 2].Style == Board[1, 1].Style && Board[1, 1].Style == Board[2, 0].Style)
        {
            return Maybe<WinningPlay>.Some(new WinningPlay
            {
                WinningStyle = Board[0, 2].Style,
                WinningMoves = ["0,2", "1,1", "2,0"]
            });
        }

        return Maybe<WinningPlay>.None;
    }

    private void SwitchTurns()
    {
        CurrentStyle = CurrentStyle == PieceStyle.X ? PieceStyle.O : PieceStyle.X;
    }

    public async Task Reset()
    {
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                Board[row, col] = new GamePiece { Style = PieceStyle.Blank };
            }
        }

        await Task.CompletedTask;
    }
}
