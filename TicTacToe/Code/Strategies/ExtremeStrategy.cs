using System.Collections.Concurrent;
using System.Text;

namespace TicTacToe.Code.Strategies;

/// <summary>
/// Extreme difficulty: Computer uses minimax with memoisation to play optimally
/// </summary>
public class ExtremeStrategy(ILogger<ExtremeStrategy>? logger = null) : ComputerStrategyBase
{
    private const int MaxDepth = 9;
    private readonly ConcurrentDictionary<string, int> _memo = new();

    protected override Maybe<Position> ChooseMove(
        GameBoard board, List<Position> blankMoves, PieceStyle computerStyle)
    {
        _memo.Clear();
        logger?.LogDebug("Extreme - Available moves: {Count}", blankMoves.Count);

        var best = FindBestMove(board, blankMoves, computerStyle);
        if (best.HasValue)
            logger?.LogInformation("Extreme - Optimal move at ({Row}, {Col})", best.Value.Row, best.Value.Col);

        return best.HasValue ? best : Maybe<Position>.Some(blankMoves[0]);
    }

    private Maybe<Position> FindBestMove(
        GameBoard board, List<Position> moves, PieceStyle computerStyle)
    {
        var bestScore = int.MinValue;
        var bestMoves = new List<Position>();

        foreach (var move in moves)
        {
            var testBoard = board.Clone();
            testBoard.Board[move.Row, move.Col].Style = computerStyle;
            var score = Minimax(testBoard, 0, false, computerStyle);

            if (score > bestScore) { bestScore = score; bestMoves.Clear(); }
            if (score >= bestScore) bestMoves.Add(move);
        }

        return bestMoves.Count > 0
            ? Maybe<Position>.Some(bestMoves[Random.Shared.Next(bestMoves.Count)])
            : Maybe<Position>.None;
    }

    private int Minimax(GameBoard board, int depth, bool isMaximizing, PieceStyle computerStyle)
    {
        var hash = GetBoardHash(board, isMaximizing ? computerStyle : OpponentOf(computerStyle));
        if (_memo.TryGetValue(hash, out var cached)) return cached;

        var terminal = EvaluateTerminal(board, depth, computerStyle);
        if (terminal.HasValue)
        {
            _memo.TryAdd(hash, terminal.Value);
            return terminal.Value;
        }

        var result = isMaximizing
            ? MaximizingScore(board, depth, computerStyle)
            : MinimizingScore(board, depth, computerStyle);

        _memo.TryAdd(hash, result);
        return result;
    }

    private static Maybe<int> EvaluateTerminal(GameBoard board, int depth, PieceStyle computerStyle)
    {
        var winner = board.GetWinner();
        if (winner.HasValue)
            return Maybe<int>.Some(winner.Value.WinningStyle == computerStyle ? 10 - depth : depth - 10);

        if (board.ItsADraw() || depth >= MaxDepth || !BoardUtilities.GetBlankMoves(board).Any())
            return Maybe<int>.Some(0);

        return Maybe<int>.None;
    }

    private int MaximizingScore(GameBoard board, int depth, PieceStyle computerStyle)
    {
        var max = int.MinValue;
        foreach (var move in BoardUtilities.GetBlankMoves(board))
        {
            var test = board.Clone();
            test.Board[move.Row, move.Col].Style = computerStyle;
            max = Math.Max(max, Minimax(test, depth + 1, false, computerStyle));
        }
        return max;
    }

    private int MinimizingScore(GameBoard board, int depth, PieceStyle computerStyle)
    {
        var min = int.MaxValue;
        var opponent = OpponentOf(computerStyle);
        foreach (var move in BoardUtilities.GetBlankMoves(board))
        {
            var test = board.Clone();
            test.Board[move.Row, move.Col].Style = opponent;
            min = Math.Min(min, Minimax(test, depth + 1, true, computerStyle));
        }
        return min;
    }

    private static string GetBoardHash(GameBoard board, PieceStyle currentPlayer)
    {
        var sb = new StringBuilder(Constants.BoardSize * Constants.BoardSize + 1);
        for (var row = 0; row < Constants.BoardSize; row++)
            for (var col = 0; col < Constants.BoardSize; col++)
                sb.Append((char)board.Board[row, col].Style);
        sb.Append((char)currentPlayer);
        return sb.ToString();
    }
}
