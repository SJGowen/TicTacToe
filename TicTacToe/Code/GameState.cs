namespace TicTacToe.Code
{
    public class GameState
    {
        public GameBoard Board { get; private set; }
        public bool GameStarted { get; set; }
        public string ButtonText => GameStarted ? "Stop Game" : "Start Game";
        public string GameCompleteMessage { get; private set; } = string.Empty;

        public GameState()
        {
            Board = new GameBoard(PlayerType.Human, PlayerType.Human);
        }

        public void StartGame(PlayerType playerXType, PlayerType playerOType)
        {
            Board = new GameBoard(playerXType, playerOType);
            GameStarted = true;
        }

        public void StopGame()
        {
            GameStarted = false;
        }

        public void ResetGame(PlayerType playerXType, PlayerType playerOType)
        {
            Board = new GameBoard(playerXType, playerOType);
            GameStarted = false;
            GameCompleteMessage = string.Empty;
        }

        public void UpdateGameCompleteMessage()
        {
            var winningPlay = Board.GetWinner();
            GameCompleteMessage = winningPlay.HasValue ? $"{winningPlay.Value.WinningStyle} Wins!" : "It's a Draw!";
        }
    }
}
