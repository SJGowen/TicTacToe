using TicTacToe.Code;

namespace TicTacToe.Code;

public class GameState
{
    private readonly ILogger<ComputerPlayer> _logger;
    public GameBoard Board { get; private set; }
    public bool GameStarted { get; set; }
    public string ButtonText => GameStarted ? "Stop Game" : "Start Game";
    public string GameCompleteMessage { get; private set; } = string.Empty;
    public string SelectPlayers { get; private set; } = "Select Players";
    public Maybe<WinningPlay> WinningPlay { get; private set; } = Maybe<WinningPlay>.None;
    public ComputerPlayer ComputerPlayer { get; }

    public GameState(ILogger<ComputerPlayer> logger)
    {
        _logger = logger;
        ComputerPlayer = new ComputerPlayer(PieceStyle.O, _logger);
        Board = new GameBoard(PlayerType.Human, PlayerType.Human, _logger);
    }

    public void StartGame(PlayerType playerXType, PlayerType playerOType)
    {
        Board = new GameBoard(playerXType, playerOType, _logger);
        GameStarted = true;
        WinningPlay = Maybe<WinningPlay>.None;
    }

    public void StopGame()
    {
        GameStarted = false;
    }

    public void ResetGame(PlayerType playerXType, PlayerType playerOType)
    {
        Board = new GameBoard(playerXType, playerOType, _logger);
        GameStarted = false;
        GameCompleteMessage = string.Empty;
        WinningPlay = Maybe<WinningPlay>.None;
    }

    public void UpdateGameCompleteMessage()
    {
        WinningPlay = Board.GetWinner();
        GameCompleteMessage = WinningPlay.HasValue 
            ? $"{WinningPlay.Value.WinningStyle} Wins!" 
            : "It's a Draw!";
    }
}