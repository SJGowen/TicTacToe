namespace TicTacToe.Code;

public class ComputerPlayer : Player
{
    private const int MaxDepth = 10;

    public ComputerPlayer(PieceStyle style) : base(style) { }

    public override (int row, int col) GetMove(GameBoard board)
    {
        return MiniMax(board, Style, int.MinValue, int.MaxValue, 0).Move;
    }

    private (int Score, (int row, int col) Move) MiniMax(GameBoard board, PieceStyle currentStyle, int alpha, int beta, int depth)
    {
        // Check if the game is complete and return the score
        if (board.GameComplete)
        {
            var winner = board.GetWinner();
            if (winner.HasValue)
            {
                if (winner.Value.WinningStyle == Style)
                    return (1000 - depth, (-1, -1)); // Win
                else
                    return (-1000 + depth, (-1, -1)); // Loss
            }
            else
            {
                return (0, (-1, -1)); // Draw
            }
        }

        // Limit the depth of the search
        if (depth >= MaxDepth)
        {
            return (EvaluateBoard(board), (-1, -1));
        }

        var bestScore = currentStyle == Style ? int.MinValue : int.MaxValue;
        var bestMove = (-1, -1);

        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                if (board.Board[row, col].Style == PieceStyle.Blank)
                {
                    // Create a copy of the board and make the move
                    var newBoard = board.Clone();
                    newBoard.Board[row, col].Style = currentStyle;

                    // Recursively call MiniMax with alpha-beta pruning
                    var score = MiniMax(newBoard, currentStyle == PieceStyle.X ? PieceStyle.O : PieceStyle.X, alpha, beta, depth + 1).Score;

                    // Update the best score and move
                    if (currentStyle == Style)
                    {
                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestMove = (row, col);
                        }
                        alpha = Math.Max(alpha, bestScore);
                    }
                    else
                    {
                        if (score < bestScore)
                        {
                            bestScore = score;
                            bestMove = (row, col);
                        }
                        beta = Math.Min(beta, bestScore);
                    }

                    // Alpha-beta pruning
                    if (beta <= alpha)
                    {
                        break;
                    }
                }
            }
        }

        return (bestScore, bestMove);
    }

    private int EvaluateBoard(GameBoard board)
    {
        int score = 0;

        // Evaluate rows
        for (int row = 0; row < 3; row++)
        {
            score += EvaluateLine(board.Board[row, 0], board.Board[row, 1], board.Board[row, 2]);
        }

        // Evaluate columns
        for (int col = 0; col < 3; col++)
        {
            score += EvaluateLine(board.Board[0, col], board.Board[1, col], board.Board[2, col]);
        }

        // Evaluate diagonals
        score += EvaluateLine(board.Board[0, 0], board.Board[1, 1], board.Board[2, 2]);
        score += EvaluateLine(board.Board[0, 2], board.Board[1, 1], board.Board[2, 0]);

        return score;
    }

    private int EvaluateLine(GamePiece a, GamePiece b, GamePiece c)
    {
        int score = 0;

        // Evaluate the line based on the number of Xs and Os
        if (a.Style == Style) score += 10;
        if (b.Style == Style) score += 10;
        if (c.Style == Style) score += 10;

        if (a.Style != Style && a.Style != PieceStyle.Blank) score -= 10;
        if (b.Style != Style && b.Style != PieceStyle.Blank) score -= 10;
        if (c.Style != Style && c.Style != PieceStyle.Blank) score -= 10;

        return score;
    }
}
