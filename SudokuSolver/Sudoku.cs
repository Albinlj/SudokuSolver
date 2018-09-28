using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Console = System.Console;

namespace SudokuSolver {
    class Sudoku {
        public int[,] Grid { get; }
        public List<Tuple<int, int>> EmptyCoords { get; } = new List<Tuple<int, int>>();

        public Sudoku(string inputString) {
            this.Grid = TransformStringToArray(inputString);
        }

        private int[,] TransformStringToArray(string input) {
            int[,] newBoard = new int[9, 9];
            for (int row = 0; row < 9; row++) {
                for (int col = 0; col < 9; col++) {
                    int val;
                    if (int.TryParse(input[col + (row * 9)].ToString(), out val)) {
                        newBoard[row, col] = val;
                    }
                    else {
                        Console.WriteLine("Aee denna e kass");
                    }
                }
            }

            return newBoard;
        }

        //Looks through the board too find coordinates of the empty squares
        public void FindEmptyCoordinates() {
            EmptyCoords.Clear();
            for (int y = 0; y < 9; y++) {
                for (int x = 0; x < 9; x++) {
                    if (Grid[y, x] == 0) {
                        EmptyCoords.Add(new Tuple<int, int>(y, x));
                    }
                }
            }
        }


        public int[,] SolveByElimination() {
            int loopCount = 0;
            bool filledAnyCell;
            do {
                filledAnyCell = false;
                for (int y = 0; y < 9; y++) {
                    for (int x = 0; x < 9; x++) {
                        if (Grid[y, x] != 0) continue;
                        int newCellValue = FindOnlyPossibleNumber(y, x);
                        if (newCellValue == 0) continue;
                        Grid[y, x] = newCellValue;
                        filledAnyCell = true;
                    }
                }

                loopCount++;
            } while (filledAnyCell);

            Console.WriteLine($"Made {loopCount} loops, this is the result:");
            PrintGrid();
            return Grid;

            int FindOnlyPossibleNumber(int cellY, int cellX) {
                bool[] eliminatedNumbers = new bool[9];

                for (int x = 0; x < 9; x++) {
                    int cellVal = Grid[cellY, x];
                    if (cellVal != 0) {
                        eliminatedNumbers[cellVal - 1] = true;
                    }
                }

                for (int y = 0; y < 9; y++) {
                    int cellVal = Grid[y, cellX];
                    if (cellVal != 0) {
                        eliminatedNumbers[cellVal - 1] = true;
                    }
                }

                int blockX = cellX / 3;
                int blockY = cellY / 3;

                for (int y = blockY * 3; y < (blockY * 3) + 3; y++) {
                    for (int x = blockX * 3; x < (blockX * 3) + 3; x++) {
                        int cellVal = Grid[y, x];
                        if (cellVal != 0) {
                            eliminatedNumbers[cellVal - 1] = true;
                        }
                    }
                }

                int trueCount = 0;

                foreach (bool b in eliminatedNumbers) {
                    if (b == true) {
                        trueCount++;
                    }
                }

                if (trueCount == 8) {
                    for (int i = 0; i < 9; i++) {
                        if (eliminatedNumbers[i] == false) {
                            return i + 1;
                        }
                    }
                }

                return 0;
            }
        }

        public List<int[,]> SolveRecursively() {
            List<int[,]> solutions = new List<int[,]>();
            int tryCount = 0;
            int depth = 0;
            FindEmptyCoordinates();

            //This is where the solving begins. We try 1-9 for the first square.
            for (int i = 9; i >= 1; i--) {
                MakeAttempt(EmptyCoords[depth].Item1, EmptyCoords[depth].Item2, i);
            }
            //And the solving is done! We have looped through 1-9 in the first square,
            //and recursively every possible combination for the following squares.
            //We return a list of all the found solutions. This will be empty if it is unsolvable.
            return solutions;

            //Nested method - this is the one that gets called recursively.
            void MakeAttempt(int y, int x, int num) {
                tryCount++;

                // Checks row and column simultaneously to see if they contain the number currently being guessed. 

                for (int i = 0; i < 9; i++) {
                    if (Grid[i, x] == num || Grid[y, i] == num) {
                        // If it already exists, the number guessed in the current square must be wrong! 
                        // We return, and end up where this method was called before.
                        return;
                    }
                }
                // Check Block in the same way
                int blockX = x / 3;
                int blockY = y / 3;
                for (int qx = 0; qx < 3; qx++) {
                    for (int qy = 0; qy < 3; qy++) {
                        if (Grid[blockY * 3 + qy, blockX * 3 + qx] == num) {
                            return;
                        }
                    }
                }

                //The number currently being guessed was not found in the same row/col/block! 
                Grid[y, x] = num;
                depth++;

                //See if the sudoku is solved by checking if the current "calldepth" is the same as the amount of empty squares.
                if (depth == EmptyCoords.Count) {
                    Console.WriteLine($"Found solution recursively in {tryCount} tries.");
                    solutions.Add(Grid);
                    PrintGrid();
                }
                //If it is not solved, continue by guessing a number for the next empty square.
                else {
                    for (int i = 9; i >= 1; i--) {
                        MakeAttempt(EmptyCoords[depth].Item1, EmptyCoords[depth].Item2, i);
                        //THIS is the place referred to below and above!
                    }
                }

                //If we've reached this point, the next square could not contain 1-9.
                //Therefore - the number currently in this square must be wrong! Erase it.
                //When we reach the end of this method, we will go back to the previous "Layer" of recusion call, and try another number for this square.
                depth--;
                Grid[y, x] = 0;
            }
        }

        public void PrintGrid() {
            for (int y = 0; y < 9; y++) {
                if (y % 3 == 0) {
                    Console.WriteLine("+-------+-------+-------+");
                }

                for (int x = 0; x < 9; x++) {
                    if (x % 3 == 0) {
                        Console.Write("| ");
                    }

                    if (Grid[y, x] == 0) {
                        Console.Write("  ");
                    }
                    else {
                        Console.Write(Grid[y, x] + " ");
                    }
                }

                Console.Write("|\n");
            }

            Console.WriteLine("+-------+-------+-------+\n");
        }
    }
}
