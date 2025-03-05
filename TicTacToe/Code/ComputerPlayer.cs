using System;
using System.Collections.Generic;
using System.Linq;

namespace TicTacToe.Code
{
    public class ComputerPlayer : Player
    {
        public ComputerPlayer(PieceStyle style) : base(style)
        {
        }

        public override (int row, int col) GetMove(GameBoard board)
        {
            // Check for winning move
            var winningMove = GetWinningMove(board, Style);
            if (winningMove.HasValue)
            {
                Console.WriteLine($"Computer (X) winning move: {winningMove.Value}");
                return winningMove.Value;
            }

            // Block opponent's winning move
            var opponentStyle = Style == PieceStyle.X ? PieceStyle.O : PieceStyle.X;
            var blockingMove = GetWinningMove(board, opponentStyle);
            if (blockingMove.HasValue)
            {
                Console.WriteLine($"Computer (X) blocking move: {blockingMove.Value}");
                return blockingMove.Value;
            }

            // Check for potential winning lines
            var potentialMove = GetPotentialWinningMove(board, Style);
            if (potentialMove.HasValue)
            {
                Console.WriteLine($"Computer (X) potential move: {potentialMove.Value}");
                return potentialMove.Value;
            }

            // Use optimal squares (center, corners, edges)
            var optimalMove = GetOptimalMove(board);
            Console.WriteLine($"Computer (X) optimal move: {optimalMove}");
            return optimalMove;
        }

        private Maybe<(int row, int col)> GetWinningMove(GameBoard board, PieceStyle style)
        {
            var winningMove = (from r in Enumerable.Range(0, 3)
                               from c in Enumerable.Range(0, 3)
                               where board.Board[r, c].Style == PieceStyle.Blank
                               select (r, c) into move
                               let simulatedBoard = SimulateMove(board, move, style)
                               let winner = simulatedBoard.GetWinner()
                               where winner.HasValue && winner.Value.WinningStyle == style
                               select move).FirstOrDefault();

            if (winningMove != default)
            {
                Console.WriteLine($"Winning move found for {style}: {winningMove}");
            }

            return winningMove != default
                ? Maybe<(int row, int col)>.Some(winningMove)
                : Maybe<(int row, int col)>.None;
        }

        private GameBoard SimulateMove(GameBoard board, (int row, int col) move, PieceStyle style)
        {
            var newBoard = board.Clone();
            newBoard.Board[move.row, move.col].Style = style;
            return newBoard;
        }

        private Maybe<(int row, int col)> GetPotentialWinningMove(GameBoard board, PieceStyle style) =>
            (from move in GetBlankMoves(board)
             let score = EvaluatePotentialMove(board, move, style)
             orderby score descending
             select move).FirstOrDefault() is (int row, int col) potentialMove
                ? Maybe<(int row, int col)>.Some(potentialMove)
                : Maybe<(int row, int col)>.None;

        private IEnumerable<(int row, int col)> GetBlankMoves(GameBoard board) =>
            from row in Enumerable.Range(0, 3)
            from col in Enumerable.Range(0, 3)
            where board.Board[row, col].Style == PieceStyle.Blank
            select (row, col);

        private int EvaluatePotentialMove(GameBoard board, (int row, int col) move, PieceStyle style)
        {
            var newBoard = SimulateMove(board, move, style);
            return EvaluateBoard(newBoard);
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
            Enumerable.Range(0, 3).Sum(row => EvaluateLine(board.Board[row, 0], board.Board[row, 1], board.Board[row, 2])) +
            Enumerable.Range(0, 3).Sum(col => EvaluateLine(board.Board[0, col], board.Board[1, col], board.Board[2, col])) +
            EvaluateLine(board.Board[0, 0], board.Board[1, 1], board.Board[2, 2]) +
            EvaluateLine(board.Board[0, 2], board.Board[1, 1], board.Board[2, 0]);

        private int EvaluateLine(GamePiece a, GamePiece b, GamePiece c) =>
            new[] { a, b, c }.Sum(piece => piece.Style == Style ? 10 : piece.Style != PieceStyle.Blank ? -10 : 0);
    }
}