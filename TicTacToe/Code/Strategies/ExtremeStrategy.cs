using System.Collections.Immutable;
using System.Text;

namespace TicTacToe.Code.Strategies;

/// <summary>
/// Extreme difficulty: Computer uses minimax-style lookahead to evaluate move outcomes
/// Considers future game states to make optimal decisions
/// </summary>
public class ExtremeStrategy : IComputerStrategy
{
    private static readonly Position Center = new(1, 1);
    private static readonly ImmutableArray<Position> Corners = [new(0, 0), new(0, 2), new(2, 0), new(2, 2)];
    private static readonly ImmutableArray<Position> MiddleEdges = [new(0, 1), new(1, 0), new(1, 2), new(2, 1)];
    
    private readonly ILogger<ExtremeStrategy>? _logger;
    private const int MaxDepth = 9; // Full game tree for 3x3 board
    private readonly Dictionary<string, int> _memo = new();

    public ExtremeStrategy(ILogger<ExtremeStrategy>? logger = null)
    {
        _logger = logger;
    }

    public Maybe<Position> GetMove(GameBoard board, PieceStyle computerStyle)
    {
        _memo.Clear(); // Clear cache for each new move
        ArgumentNullException.ThrowIfNull(board);

        try
        {
            var blankMoves = BoardUtilities.GetBlankMoves(board).ToList();
            _logger?.LogDebug($"Extreme - Available moves: {blankMoves.Count}");

            if (blankMoves.Count == 0)
            {
                _logger?.LogInformation("Extreme - No moves available");
                return Maybe<Position>.None;
            }

            // Use minimax to evaluate all moves
            var bestMove = FindBestMove(board, blankMoves, computerStyle);
            if (bestMove.HasValue)
            {
                _logger?.LogInformation($"Extreme - Selected optimal move at ({bestMove.Value.Row}, {bestMove.Value.Col})");
                return bestMove;
            }

            // Fallback to first available move
            return Maybe<Position>.Some(blankMoves[0]);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Extreme - An error occurred while getting computer move");
            throw;
        }
    }

    private Maybe<Position> FindBestMove(GameBoard board, List<Position> availableMoves, PieceStyle computerStyle)
    {
        var opponentStyle = computerStyle == PieceStyle.X ? PieceStyle.O : PieceStyle.X;
        var bestScore = int.MinValue;
        var bestMoves = new List<Position>();

        foreach (var move in availableMoves)
        {
            var testBoard = board.Clone();
            testBoard.Board[move.Row, move.Col].Style = computerStyle;

            var score = Minimax(testBoard, 0, false, computerStyle, opponentStyle);
            if (score > bestScore)
            {
                bestScore = score;
                bestMoves.Clear();
                bestMoves.Add(move);
            }
            else if (score == bestScore)
            {
                bestMoves.Add(move);
            }
        }

        if (bestMoves.Count > 0)
            return Maybe<Position>.Some(bestMoves[Random.Shared.Next(bestMoves.Count)]);
        return Maybe<Position>.None;
    }

    private int Minimax(GameBoard board, int depth, bool isMaximizing, PieceStyle computerStyle, PieceStyle opponentStyle)
    {
        string hash = GetBoardHash(board, isMaximizing ? computerStyle : opponentStyle);
        if (_memo.TryGetValue(hash, out int cachedScore))
        {
            _logger?.LogDebug($"Extreme - Cache hit for hash {hash} at depth {depth}: score {cachedScore}");
            return cachedScore;
        }

        var winner = board.GetWinner();
        
        // Terminal states
        if (winner.HasValue)
        {
            var score = winner.Value.WinningStyle == computerStyle ? 10 - depth : depth - 10;
            _logger?.LogDebug($"Extreme - Terminal state at depth {depth}: {winner.Value.WinningStyle} wins, score: {score}");
            _memo[hash] = score;
            return score;
        }

        if (board.ItsADraw())
        {
            _logger?.LogDebug($"Extreme - Draw at depth {depth}");
            _memo[hash] = 0;
            return 0;
        }

        if (depth >= MaxDepth)
        {
            _logger?.LogDebug($"Extreme - Max depth reached at {depth}");
            _memo[hash] = 0;
            return 0;
        }

        var blankMoves = BoardUtilities.GetBlankMoves(board).ToList();
        if (blankMoves.Count == 0)
        {
            _memo[hash] = 0;
            return 0;
        }

        int result;
        if (isMaximizing)
        {
            var maxScore = int.MinValue;
            foreach (var move in blankMoves)
            {
                var testBoard = board.Clone();
                testBoard.Board[move.Row, move.Col].Style = computerStyle;
                var score = Minimax(testBoard, depth + 1, false, computerStyle, opponentStyle);
                maxScore = Math.Max(score, maxScore);
            }
            result = maxScore;
        }
        else
        {
            var minScore = int.MaxValue;
            foreach (var move in blankMoves)
            {
                var testBoard = board.Clone();
                testBoard.Board[move.Row, move.Col].Style = opponentStyle;
                var score = Minimax(testBoard, depth + 1, true, computerStyle, opponentStyle);
                minScore = Math.Min(score, minScore);
            }
            result = minScore;
        }
        _memo[hash] = result;
        return result;
    }

    private string GetBoardHash(GameBoard board, PieceStyle currentPlayer)
    {
        var sb = new StringBuilder();
        for (int row = 0; row < Constants.BoardSize; row++)
            for (int col = 0; col < Constants.BoardSize; col++)
                sb.Append((char)board.Board[row, col].Style);
        sb.Append((char)currentPlayer);
        return sb.ToString();
    }
}
