namespace TicTacToe.Code;

public class GameBoard
{
    private readonly ILogger<ComputerPlayer> _logger;
    public GamePiece[,] Board { get; private set; }
    public PieceStyle CurrentStyle { get; private set; } = PieceStyle.X;
    public bool GameComplete => GetWinner().HasValue || ItsADraw();
    
    private Player _playerX;
    private Player _playerO;
    private Player CurrentPlayer => CurrentStyle == PieceStyle.X ? _playerX : _playerO;

    public GameBoard(PlayerType playerXType, PlayerType playerOType, ILogger<ComputerPlayer> logger)
    {
        _logger = logger;
        Board = new GamePiece[Constants.BoardSize, Constants.BoardSize];

        for (int row = 0; row < Constants.BoardSize; row++)
        {
            for (int col = 0; col < Constants.BoardSize; col++)
            {
                Board[row, col] = new GamePiece { Style = PieceStyle.Blank };
            }
        }

        _playerX = CreatePlayer(playerXType, PieceStyle.X);
        _playerO = CreatePlayer(playerOType, PieceStyle.O);
    }

    private Player CreatePlayer(PlayerType playerType, PieceStyle style)
    {
        return playerType switch
        {
            PlayerType.Human => new HumanPlayer(style),
            PlayerType.Computer => new ComputerPlayer(style, _logger),
            _ => throw new ArgumentException("Invalid player type")
        };
    }

    public GameBoard Clone()
    {
        var clone = new GameBoard(PlayerType.Human, PlayerType.Human, _logger)
        {
            CurrentStyle = this.CurrentStyle
        };

        for (int row = 0; row < Constants.BoardSize; row++)
        {
            for (int col = 0; col < Constants.BoardSize; col++)
            {
                clone.Board[row, col] = new GamePiece { Style = this.Board[row, col].Style };
            }
        }

        return clone;
    }

    public async Task PieceClicked(Position position)
    {
        if (GameComplete || Board[position.Row, position.Col].Style != PieceStyle.Blank)
            return;

        await Task.Run(() =>
        {
            Board[position.Row, position.Col].Style = CurrentPlayer.Style;
            SwitchPlayers();
        });
    }

    public async Task MakeComputerMoveIfNeededAsync()
    {
        if (CurrentPlayer is ComputerPlayer computerPlayer)
        {
            var move = await Task.Run(() => computerPlayer.GetMove(this));
            if (move.HasValue)
            {
                Board[move.Value.Row, move.Value.Col].Style = computerPlayer.Style;
                SwitchPlayers();
            }
        }
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
        for (int i = 0; i < Constants.BoardSize; i++)
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

    private void SwitchPlayers()
    {
        CurrentStyle = CurrentStyle == PieceStyle.X ? PieceStyle.O : PieceStyle.X;
    }
}
