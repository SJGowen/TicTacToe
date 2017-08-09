using System;

namespace TicTacToeGame
{
    // The TicTacToe class enables two players to play TicTacToe
    class TicTacToe
    {
        private static char[] Players = { 'X', 'O' };

        static void Main(string[] args)
        {
            // Stores the locaions each player has moved
            int[] playerPositions = { 0, 0 };
            // Set the starting player to a random player
            var random = new Random();
            var currentPlayer = random.Next(1, 3);
            // Winning player
            var winner = 0;
            string input = null;
            // Display the board and prompt the currentPlayer for his next move
            for (var turn = 1; turn <= 10; turn++)
            {
                if (input != "q") DisplayBoard(playerPositions);

                #region Check for End Game
                if (EndGame(winner, turn, input)) break;
                #endregion Check for End Game

                input = NextMove(playerPositions, currentPlayer);

                winner = DetermineWinner(playerPositions);

                // Switch Players
                currentPlayer = (currentPlayer == 1) ? 2 : 1;
            }
            Console.ReadLine();
        }

        private static bool EndGame(int winner, int turn, string input)
        {
            var endGame = false;
            if (winner > 0)
            {
                Console.WriteLine($"\nPlayer '{Players[winner - 1]}' has won!!!");
                endGame = true;
            }
            else if (turn == 10)
            {
                Console.WriteLine($"\nThe Game was a tie!");
                endGame = true;
            }
            else if (input == "q")
            {
                Console.WriteLine($"\nPlayer, has quit the game");
                endGame = true;
            }
            return endGame;
        }

        private static void DisplayBoard(int[] playerPositions)
        {
            string[] borders =
            {
                "|", "|", "\n---+---+---\n", "|", "|", "\n---+---+---\n", "|", "|", ""
            };
            // Display the current board
            var border = 0; // set the first border (border[0] = "|").
            for (var position = 1; position <= 256; position <<= 1, border++)
            {
                char token = CalculateToken(playerPositions, position);
                Console.Write($" {token} {borders[border]}");
            }
        }

        private static string NextMove(int[] playerPositions, int currentPlayer)
        {
            string input;

            // Repeatedly prompt the player for his next move until a valid move is returned
            bool validMove;
            do
            {
                Console.WriteLine($"\nPlayer '{Players[currentPlayer - 1]}' - Enter move:");
                input = Console.ReadLine();
                validMove = ValidateAndMove(playerPositions, currentPlayer, input);
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

        private static char CalculateToken(int[] playerPositions, int position)
        {
            char token;
            if ((position & playerPositions[0]) == position)
            {
                token = Players[0]; // player 'X' has that position marked.
            }
            else if ((position & playerPositions[1]) == position)
            {
                token = Players[1]; // player 'O' has that position marked.
            }
            else
            {
                token = ' '; // The position is empty
            }
            return token;
        }

        private static bool ValidateAndMove(int[] playerPositions, int currentPlayer, string input)
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
                    if (CalculateToken(playerPositions, position) != ' ')
                    {
                        Console.WriteLine($"ERROR: Square {input} has already been played!");
                    }
                    else
                    {
                        playerPositions[currentPlayer - 1] += position;
                        valid = true;
                    }
                    break;
                case "q":
                    valid = true;
                    break;
                default:
                    // if none of the other case statements
                    // is encountered, then the text is invalid.
                    Console.WriteLine("ERROR: Enter a value from 1-9. Type 'q' to quit");
                    break;
            }
            return valid;
        }
    }
}
