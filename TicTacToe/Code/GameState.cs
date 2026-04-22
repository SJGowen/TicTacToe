using LanguageExt;
using TicTacToe.Code;
using TicTacToe.Code.Strategies;

namespace TicTacToe.Code;

public class GameState
{
    private readonly ILogger<ComputerPlayer>? _logger;
    public GameBoard Board { get; private set; }
    public bool GameStarted { get; set; }
    public string ButtonText => GameStarted ? "Stop Game" : "Start Game";
    public string GameCompleteMessage { get; private set; } = string.Empty;
    public string SelectPlayers { get; private set; } = "Select Players";
    public Option<WinningPlay> WinningPlay { get; private set; } = Option<WinningPlay>.None;
    public ComputerPlayer ComputerPlayer { get; }

    public GameState(ILogger<ComputerPlayer>? logger = null)
    {
        _logger = logger;
        ComputerPlayer = new ComputerPlayer(PieceStyle.O, new HardStrategy(), _logger);
        Board = new GameBoard(PlayerType.Human, PlayerType.Human, _logger);
    }

    public void StartGame(PlayerType playerXType, PlayerType playerOType)
    {
        Board = new GameBoard(playerXType, playerOType, _logger);
        GameStarted = true;
        WinningPlay = Option<WinningPlay>.None;
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
        WinningPlay = Option<WinningPlay>.None;
    }

    public void UpdateGameCompleteMessage()
    {
        WinningPlay = Board.GetWinner();
        GameCompleteMessage = WinningPlay.IsSome 
            ? $"{WinningPlay.Match(w => w.WinningStyle, () => PieceStyle.Blank)} Wins!" 
            : "It's a Draw!";
    }
}