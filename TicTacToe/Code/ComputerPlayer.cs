namespace TicTacToe.Code
{
    public class ComputerPlayer : Player
    {
        public ComputerPlayer(PieceStyle style) : base(style)
        {
        }

        public override (int row, int col) GetMove(GameBoard board)
        {
            var blankMoves = GetBlankMoves(board).ToList();

            // Check for winning move
            var winningMove = blankMoves.FirstOrDefault(move => IsWinningMove(board, move, Style));
            if (winningMove != default) return winningMove;

            // Block opponent's winning move
            var opponentStyle = Style == PieceStyle.X ? PieceStyle.O : PieceStyle.X;
            var blockingMove = blankMoves.FirstOrDefault(move => IsWinningMove(board, move, opponentStyle));
            if (blockingMove != default) return blockingMove;

            // Check for potential winning lines
            var potentialMove = GetPotentialWinningMove(board, Style);
            if (potentialMove.HasValue) return potentialMove.Value;

            // Use optimal squares (center, corners, edges)
            return GetOptimalMove(board);
        }

        private bool IsWinningMove(GameBoard board, (int row, int col) move, PieceStyle style)
        {
            board.Board[move.row, move.col].Style = style;
            var winner = board.GetWinner();
            board.Board[move.row, move.col].Style = PieceStyle.Blank;
            return winner.HasValue && winner.Value.WinningStyle == style;
        }

        private Maybe<(int row, int col)> GetPotentialWinningMove(GameBoard board, PieceStyle style) =>
            (from move in GetBlankMoves(board)
             let score = EvaluatePotentialMove(board, move, style)
             orderby score descending
             select move).FirstOrDefault() is (int row, int col) potentialMove
                ? Maybe<(int row, int col)>.Some(potentialMove)
                : Maybe<(int row, int col)>.None;

        private IEnumerable<(int row, int col)> GetBlankMoves(GameBoard board) =>
            from row in Enumerable.Range(0, Constants.BoardSize)
            from col in Enumerable.Range(0, Constants.BoardSize)
            where board.Board[row, col].Style == PieceStyle.Blank
            select (row, col);

        private int EvaluatePotentialMove(GameBoard board, (int row, int col) move, PieceStyle style)
        {
            // Apply the move
            board.Board[move.row, move.col].Style = style;

            // Evaluate the board
            int score = EvaluateBoard(board);

            // Revert the move
            board.Board[move.row, move.col].Style = PieceStyle.Blank;

            return score;
        }

        private (int row, int col) GetOptimalMove(GameBoard board)
        {
            var optimalMoves = new List<(int row, int col)>
                {
                    (1, 1), // Center
                    (0, 0), (0, 2), (2, 0), (2, 2), // Corners
                    (0, 1), (1, 0), (1, 2), (2, 1) // Edges
                };

            return optimalMoves.FirstOrDefault(move => board.Board[move.row, move.col].Style == PieceStyle.Blank);
        }

        private int EvaluateBoard(GameBoard board) =>
            Enumerable.Range(0, Constants.BoardSize).Sum(row => EvaluateLine(board.Board[row, 0], board.Board[row, 1], board.Board[row, 2])) +
            Enumerable.Range(0, Constants.BoardSize).Sum(col => EvaluateLine(board.Board[0, col], board.Board[1, col], board.Board[2, col])) +
            EvaluateLine(board.Board[0, 0], board.Board[1, 1], board.Board[2, 2]) +
            EvaluateLine(board.Board[0, 2], board.Board[1, 1], board.Board[2, 0]);

        private int EvaluateLine(GamePiece a, GamePiece b, GamePiece c)
        {
            int score = 0;
            score += GetPieceScore(a);
            score += GetPieceScore(b);
            score += GetPieceScore(c);
            return score;
        }

        private int GetPieceScore(GamePiece piece)
        {
            if (piece.Style == Style) return 10;
            if (piece.Style != PieceStyle.Blank) return -10;
            return 0;
        }
    }
}