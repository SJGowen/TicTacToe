using System;

namespace TicTacToeGame
{
    // The TicTacToe class enables two players to play TicTacToe
    class TicTacToe
    {
        static void Main(string[] args)
        {
            // Stores the locaions each player has moved
            int[] playerPositions = { 0, 0 };
            // Players are 'X' and '0'
            char[] players = { 'X', 'O' };
            // Set the starting player to player 'X'
            var currentPlayer = 1;
            string input = null;
            // Display the board and prompt the currentPlayer for his next move
            do
            {
                DisplayBoard(players, playerPositions);

                input = NextMove(players, playerPositions, currentPlayer);

                currentPlayer = (currentPlayer == 1) ? 2 : 1; // Switch Players

            } while (!GameOver(players, playerPositions, input));
            Console.ReadLine();
        }

        private static bool GameOver(char[] players, int[] playerPositions, string input)
        {
            var gameOver = false;
            var winner = DetermineWinner(playerPositions);
            if (winner > 0)
            {
                DisplayBoard(players, playerPositions);
                Console.Write($"\nPlayer '{players[winner - 1]}' has won!!!");
                gameOver = true;
            }
            else if (playerPositions[0] + playerPositions[1] == 511)
            { // All possible squares have been filled
                DisplayBoard(players, playerPositions);
                Console.Write($"\nThe Game was a tie!");
                gameOver = true;
            }
            else if (input.ToLower() == "q")
            {
                Console.Write($"\nPlayer, has quit the game");
                gameOver = true;
            }
            return gameOver;
        }

        private static string NextMove(char[] players, int[] playerPositions, int currentPlayer)
        {
            string input;

            // Repeatedly prompt the player for his next move until a valid move is returned
            bool validMove;
            do
            {
                Console.Write($"\nPlayer '{players[currentPlayer - 1]}' - Enter move: ");
                input = Console.ReadLine();
                validMove = ValidateAndMove(players, playerPositions, currentPlayer, input);
            } while (!validMove);

            return input;
        }

        private static int DetermineWinner(int[] playerPositions)
        {
            var winner = 0;

            int[] winningMasks = { 7, 56, 448, 73, 146, 292, 84, 273 };

            foreach (int mask in winningMasks)
            {
                if ((mask & playerPositions[0]) == mask)
                {
                    winner = 1;
                    break;
                }
                if ((mask & playerPositions[1]) == mask)
                {
                    winner = 2;
                    break;
                }
            }
            return winner;
        }

        private static void DisplayBoard(char[] players, int[] playerPositions)
        {
            string[] borders =
            {
                "|", "|", "\n---+---+---\n", "|", "|", "\n---+---+---\n", "|", "|", ""
            };
            // Display the current board
            var border = 0; // set the first border (border[0] = "|").
            for (var position = 1; position <= 256; position <<= 1, border++)
            {
                char token = CalculateToken(players, playerPositions, position);
                Console.Write($" {token} {borders[border]}");
            }
        }

        private static char CalculateToken(char[] players, int[] playerPositions, int position)
        {
            char token;
            if ((position & playerPositions[0]) == position)
            {
                token = players[0]; // player 'X' has that position marked.
            }
            else if ((position & playerPositions[1]) == position)
            {
                token = players[1]; // player 'O' has that position marked.
            }
            else
            {
                token = ' '; // The position is empty
            }
            return token;
        }

        private static bool ValidateAndMove(char[] players, int[] playerPositions, int currentPlayer, string input)
        {
            var valid = false;
            switch (input)
            {
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                    var shifter = int.Parse(input) - 1; // The number of places to shift over a bit
                    var position = 1 << shifter; // The bit which is to be set
                    // Could use a string to record the input, and only if input not in string then input is valid
                    // but we already have similar functionality called from DisplayBoard i.e. CalculateToken
                    if (CalculateToken(players, playerPositions, position) != ' ')
                    {
                        Console.Write($"\nERROR: Square {input} has already been played!");
                    }
                    else
                    {
                        playerPositions[currentPlayer - 1] += position;
                        valid = true;
                    }
                    break;
                case "q":
                case "Q":
                    valid = true;
                    break;
                default:
                    // if none of the other case statements
                    // is encountered, then the text is invalid.
                    Console.Write("\nERROR: Enter a value from 1-9. Type 'q' to quit");
                    break;
            }
            return valid;
        }
    }
}
