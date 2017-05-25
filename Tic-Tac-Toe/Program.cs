using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tester
{
    class Program
    {
        static void Main()  // entry point to program
        {       
            /* Stores locations each player has moved (or placed their symbol into)
             *   playerPositions stores an integer for each player representing in which cells their symbols reside
             *   (e.g. if you have symbols in cells 2 and 9, your position integer is: 2^2 + 2^9)
            */
            int[] playerPositions = { 1024, 1024 };

            //select play number
            System.Console.WriteLine("Please select the player you'll want to be (1 or 2): ");
            int Player = int.Parse(System.Console.ReadLine());
            while (Player != 2 && Player != 1)
            {
                System.Console.WriteLine("Please select the player you'll want to be (1 or 2): ");
                Player = int.Parse(System.Console.ReadLine());
            }
            // initialize current player to 1
            int currentPlayer = 1;

            // Winning Player
            int winner = 0;

            string input = null;

            // Display the board and prompt the current player
            // for her next move
            for (int turn = 1; turn <= 10; turn++)
            {
                DisplayBoard(playerPositions);

                #region Check for End Game
                if (EndGame(winner, turn, input))
                {
                    break;
                }
                #endregion Check for End Game
                if (currentPlayer == Player)
                {
                    input = NextMove(playerPositions, currentPlayer);
                }
                else
                {
                    input = aiMove(playerPositions, turn, Player);
                }
                playerPositions[currentPlayer-1] += (int) (Math.Pow(2, double.Parse(input)-1));//(type) to convert to type
             
                winner = DetermineWinner(playerPositions);

                // Switch players
                currentPlayer = (currentPlayer == 2) ? 1 : 2;
            }

        }  // end Main()


        private static string NextMove(int[] playerPositions, int currentPlayer)
        {
            /*  Repeatedly prompt player for a move until a valid move is entered
             * {Do}
             *      get location-number from player and verify that it is a valid location-choice (i.e. 1-9, empty location)
             *          [***NOTE that the EXE does not implement the above correctly! ***]
             *  {Until ValidMove}
             *  return chosen move-location
             *  
             *  INPUTS: array of current playerPositions; current player ID
             *  OUTPUT: player's chosen move
             */

            string input, result;
            bool validMove, correct;

            System.Console.WriteLine("Please make your next move into valid box using numbers 1 to 9: ");

            input = System.Console.ReadLine();
            validMove = false;
            correct = true;
            
            // Repeatedly prompt player for a move until a valid move is entered
            while (validMove == false)
            {
                if (int.Parse(input) <= 9 && int.Parse(input) >= 1)
                {
                    result = System.Convert.ToString((playerPositions[0] + playerPositions[1]) & (int)(Math.Pow(2, double.Parse(input) - 1)));
                    if (double.Parse(result) != Math.Pow(2, double.Parse(input) - 1)) {
                        validMove = true;
                    }
                    else
                    {
                        System.Console.WriteLine("You entered an invalid number ");
                        correct = false;
                    }
                }
                if (correct == false)
                {
                    System.Console.WriteLine("Please enter a number between 1 and 9" + "\n" + "and in an empty segment: ");
                    input = System.Console.ReadLine();
                    correct = true;
                }              
                                
            }

            return input;
        }  // end NextMove()



        static bool EndGame(int winner, int turn, string input)
        {
            /*  Execute End of Game procedures.  
             *  {set blnEndGame var to false} 
             *  Game will end if:
             *      winner is a player-number  {write a win-message}, 
             *      OR if gameTurn=10 {write a Tie-message}
             *      OR if player's input is 'quit' or the empty string  {write a Quit-message}
             *  {if blnEndGame, ReadKey to keep console window open}
             *  {return blnEndGame var}
             *  INPUTS: winning player ID; Turn Number; current input string
             *  OUTPUT: boolean representing EndOfGame, or not
             */
            bool endGame = false;
            if (winner != 0)
            {
                if (winner == 1)
                {
                    System.Console.WriteLine("The winner is player 1, Nice Job!");
                    endGame = true;
                }
                else
                {
                    System.Console.WriteLine("The winner is player 2, Nice Job!");
                    endGame = true;
                }
            }
            else if (turn == 10)
            {
                System.Console.WriteLine("The game ends in a tie, Goodbye");
                endGame = true;
            }
            else if (input == "quit" || input == "")
            {
                System.Console.WriteLine("The game is over, Goodbye");
                endGame = true;
            }
            else {
                endGame = false;
            }

            return endGame;
        } // end EndGame()

        static int DetermineWinner(int[] playerPositions)
        {
            /* PURPOSE: Determine a Winner, if any
            *  for each possible win-condition:
            *    AND-mask each player's positions with win-mask
            *    if result of AND equals the win-mask, set winner var to that player ID
            *  return winner ID, or 0
            *  
            *  INPUTS: array of current playerPositions
            *  OUTPUT: winning player's ID, or 0
            */
            int winner = 0;
            int[] winningMasks = { 7, 56, 448, 73, 146, 292, 84, 273 };            

            // Determine if there is a winner
            for (int i = 0; i < winningMasks.Length; i++ ) {
                string p1 = (playerPositions[0] & winningMasks[i]).ToString();
                string p2 = (playerPositions[1] & winningMasks[i]).ToString();
                if (p1 == winningMasks[i].ToString())
                {
                    winner = 1;
                }
                else if (p2 == winningMasks[i].ToString())
                {
                    winner = 2;
                }
            }         

            return winner;
        }  // end DetermineWinner()

        private static string aiMove(int[] playerPositions, int turn, int player)// implementation for AI
        {
            int Human = (int)(playerPositions[player-1]) - 1024;//convert to 0 value
            int AI = (player == 2) ? 1 : 2;
            int Comp = (int)(playerPositions[AI-1]) - 1024;//convert to 0 value
            int currentValue = 0;
            bool fail = false;
            int value = 0;
            int checker = (int)(Math.Log(Comp, 2));
            string input = null; Random rnd = new Random(); string output;
            int[] winningMask = { 7, 56, 448, 73, 146, 292, 84, 273 };//values of win mask
            int midBoard = 4;
            int sideBoard = ((int)(Math.Pow(2, (double)(winningMask[6])))+ (int)(Math.Pow(2, (double)(winningMask[6]))))-(midBoard *2);
            List<int> almostWin = new List<int>();
            List<int> difference = new List<int>();
            if (AI == 2) { turn = turn / AI; }
            else
            {
                if (turn != 1) { turn = (turn / 2) + 1; }
            }
            for (int j = 0; j<9; j++)//provide all the values for the almost win mask
            {
                if (j % 3 == 0) { currentValue = j / 3; }
                int valueIncrease = (int)(Math.Pow(2, (double)(j)));
                almostWin.Add((winningMask[currentValue]) - valueIncrease);
                if (j == 0) { almostWin.Add(winningMask[3] - valueIncrease); almostWin.Add(winningMask[7] - valueIncrease); }
                if (j == 1) { almostWin.Add(winningMask[4] - valueIncrease); }
                if (j == 2) { almostWin.Add(winningMask[5] - valueIncrease); almostWin.Add(winningMask[6] - valueIncrease); }
                if (j == 3) { almostWin.Add(winningMask[3] - valueIncrease); }
                if (j == 4) { almostWin.Add(winningMask[4] - valueIncrease); almostWin.Add(winningMask[6] - valueIncrease); almostWin.Add(winningMask[7] - valueIncrease); }
                if (j == 5) { almostWin.Add(winningMask[5] - valueIncrease); }
                if (j == 6) { almostWin.Add(winningMask[3] - valueIncrease); almostWin.Add(winningMask[6] - valueIncrease); }
                if (j == 7) { almostWin.Add(winningMask[4] - valueIncrease); }
                if (j == 8) { almostWin.Add(winningMask[5] - valueIncrease); almostWin.Add(winningMask[7] - valueIncrease); }
            }
            for (int i = 0; i < 9; i++)
            {
                difference.Add((int)(Math.Pow(2, (double)(i))));
            }
            int sumaWin = almostWin.Sum();//sum of all almost win values
            
            if (turn == 1)
            {
                output = System.Convert.ToString((Human) & (int)(Math.Pow(2, midBoard)));
                if (int.Parse(output) != (int) Math.Pow(2, midBoard)) { input = midBoard.ToString(); }
                else {
                    value = rnd.Next(0, 8);
                    do { value = rnd.Next(0, 8); }
                    while (value % 2 != 0 || value == midBoard);
                    input = value.ToString();
                }
            }
            if (turn == 2)
            {
                midBoard = (int)Math.Pow(2, midBoard);
                if ((Comp & midBoard) == midBoard)
                {
                    foreach (int val in almostWin)
                    {
                        if ( val == Human)//check to see if comp has almost won
                        {
                            for (int k = 0; k < winningMask.Length; k++)
                            {
                                foreach (int item in difference)
                                {
                                    value = (int)(Math.Log(item, 2));
                                    if ((winningMask[k] - Human) == item && (item & Comp) != item) { input = value.ToString(); } //check to see if difference in human or item in mask

                                }

                            }

                        }
                    }
                    if (input == null) { fail = true; }//terminal statement to randomize spot if neither human nor player has almost won
                    if (fail == true)
                    {
                        value = rnd.Next(0, 8);
                        int test = (int)(Math.Pow(2, value));
                        int result = Human & (int)(Math.Pow(2, value));
                        do { value = rnd.Next(0, 8); result = (Human) & (int)(Math.Pow(2, value)); test = (int)(Math.Pow(2, value)); }
                        while (value % 2 != 0 || test == midBoard || result == test);
                        input = value.ToString();
                    }
                    
                }
                else
                {
                    foreach (int val in almostWin)
                    {
                        if (val == Human)//check to see if comp has almost won
                        {
                            for (int k = 0; k < winningMask.Length; k++)
                            {
                                foreach (int item in difference)
                                {
                                    value = (int)(Math.Log(item, 2));//careful
                                    if ((winningMask[k] - Human) == item && (item & Comp) != item) { input = value.ToString(); } //check to see if difference in human or item in mask

                                }

                            }

                        }
                    }
                    if (input == null) { fail = true; }//terminal statement to randomize spot if neither human nor player has almost won
                    if (fail == true)
                    {
                        value = rnd.Next(0, 8);
                        int test = (int)(Math.Pow(2, value));
                        int result = (Human) & (int)(Math.Pow(2, value));
                        do { value = rnd.Next(0, 8); result = (Human) & (int)(Math.Pow(2, value)); test = (int)(Math.Pow(2, value)); }
                        while (value % 2 != 0 || (Comp & test) == test || value == (checker + 4) || value == (checker - 4) || result == test);
                        input = value.ToString();
                    }
                    
                }
            }

            if (turn >= 3)
            {
                foreach (int val in almostWin)
                {
                    if ((Human & val) == val )// gotten for all larger values
                    {
                        for (int k = 0; k < winningMask.Length; k++)
                        {
                            foreach (int item in difference)
                            {
                                value = (int)(Math.Log(item, 2));
                                if ((winningMask[k] - val) == item && (item & Comp) != item) { input = value.ToString(); } //check to see if difference in human or item in mask

                            }

                        }

                    }

                }
                foreach (int val in almostWin)
                {
                    if ((Comp & val) == val)//check to see if comp has almost won
                    {
                        for (int k = 0; k < winningMask.Length; k++)
                        {
                            foreach (int item in difference)
                            {
                                value = (int)(Math.Log(item, 2));
                                if ((winningMask[k] - val) == item && (item & Human) != item) { input = value.ToString(); }//check to see if difference in mask or item in human

                            }

                        }

                    }
                }
                
                if (input == null) { fail = true; }//terminal statement to randomize spot if neither human nor player has almost won
                if (fail == true)
                {
                    value = rnd.Next(0, 8);
                    int test = (int)(Math.Pow(2, value));
                    int result = (Human + Comp) & test;
                    do { value = rnd.Next(0, 8); test = (int)(Math.Pow(2, value)); result = (Human + Comp) & test; }
                    while ( result == test );
                    input = value.ToString();
                }
            }
            input = (int.Parse(input) + 1).ToString();
            
            return input;
         }

        static void DisplayBoard(int[] playerPositions)
        {
            Console.WriteLine(Convert.ToString(playerPositions[0], 2));
            Console.WriteLine(Convert.ToString(playerPositions[1], 2));
            // This represents the borders between each cell for one row
            string[] borders = {"|", "|", "\n---+---+---\n", "|", "|",
                                   "\n---+---+---\n", "|", "|", ""};

            // Display the current board
            int border = 0;  // set the first border  (border[0] = "|")
            #if CSHARP2
               System.Console.Clear();
            #endif

            for (int position = 1; position <= 256; position <<= 1, border++)
            {
                char token = CalculateToken(playerPositions, position);
                // write out a cell value and the border that comes after it
                System.Console.Write(" {0} {1}", token, borders[border]);
            }

        }  // end DisplayBoard


        static char CalculateToken(int[] playerPositions, int position)
        {
            /* PURPOSE: When printing the board-state, determine which symbol to print (X or O) 
            *      based on each player's playerPosition value.
            *    Use an AND-mask to determine if a player's symbol is placed in position.
            *    If so, return player's symbol.  Else, return a space.
            *  
            *  INPUTS: array of current playerPositions; current position under consideration
            *  OUTPUT: symbol: X or O or ' ' {space}
            */
            char token;
            string p1 = (playerPositions[0] & position).ToString();
            string p2 = (playerPositions[1] & position).ToString();
            if (p1 == position.ToString()) {
                token = char.Parse("X");
            }
            else if (p2 == position.ToString())
            {
                token = char.Parse("O");
            }
            else {
                token = char.Parse(" ");
            }

            return token;
        }  // end CalculateToken()


    }
}