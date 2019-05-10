using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Word_Puzzle_Generator
{
    public class PuzzleManager
    {
        public Puzzle GetEmptyPuzzle(string puzzleCategory, List<string> wordsInPuzzleDesired, int sizeX, int sizeY)
        {
            var puzzle = new Puzzle();

            puzzle.PuzzleTime = DateTime.Now;
            puzzle.PuzzleCategory = puzzleCategory;
            puzzle.WordsInThePuzzleFilled = new List<string>();
            puzzle.WordsInThePuzzleDesired = wordsInPuzzleDesired;
            puzzle.PuzzleSolutionSet = new Dictionary<string, PuzzleSolutionWord>();
            puzzle.PuzzleGrid = new char[sizeY, sizeX];
            puzzle.iMax = sizeY;
            puzzle.jMax = sizeX;

            for (int i = 0; i < puzzle.iMax; i++)
            {
                for (int j = 0; j < puzzle.jMax; j++)
                {
                    puzzle.PuzzleGrid[i, j] = ' ';
                }
            }

            return puzzle;
        }

        const int DirectionSeekMax = 1000_000;

        public Puzzle GetFinishedPuzzle(Puzzle puzzleKey)
        {
            puzzleKey.PuzzleGridSolutionOnly = new char[puzzleKey.iMax, puzzleKey.jMax];

            for (int i = 0; i < puzzleKey.iMax; i++)
            {
                for (int j = 0; j < puzzleKey.jMax; j++)
                {
                    puzzleKey.PuzzleGridSolutionOnly[i, j] = puzzleKey.PuzzleGrid[i, j];
                }
            }

            return puzzleKey.PadEmptyCells();
        }

        public Puzzle GeneratePuzzleKey(Puzzle notYetCompletelyFilledPuzzle)
        {
            int iMax = notYetCompletelyFilledPuzzle.iMax;
            int jMax = notYetCompletelyFilledPuzzle.jMax;

            var wordList = notYetCompletelyFilledPuzzle.WordsInThePuzzleDesired;

            var wordsSuccessfullyFilled = new List<string>();

            int iMaxTrialForSameWord = 10000;

            foreach (var word in wordList)
            {
                int iTrialForSameWord = 0;

                bool wordNotFilled = true;

                while ((iTrialForSameWord < iMaxTrialForSameWord) && wordNotFilled)
                {
                    iTrialForSameWord++;

                    //Pick a random location
                    var location = HelpWithRandomPick.GetRandomLocation(iMax, jMax);

                    //Pick a random direction
                    DirectionInGrid direction = new DirectionInGrid();
                    var dirCounter = 0;
                    direction.SetToRandomDirection();

                    while ((dirCounter < 8) && wordNotFilled)
                    {
                        if (IsSpaceEnough(direction, location, word, notYetCompletelyFilledPuzzle))
                        {
                            if (TryFillingWord(direction, location, word, notYetCompletelyFilledPuzzle))
                            {
                                wordNotFilled = false;
                                notYetCompletelyFilledPuzzle.WordsInThePuzzleFilled.Add(word);
                                var puzzleSolutionWord = new PuzzleSolutionWord(word, direction, location);
                                notYetCompletelyFilledPuzzle.PuzzleSolutionSet.Add(word, puzzleSolutionWord);
                                continue; //done with direction switching
                            }
                            else
                            {
                                direction.SetToNextDirection();
                                dirCounter++;
                            }
                        }
                        else
                        {
                            direction.SetToNextDirection();
                            dirCounter++;
                        }

                    } //Keep trying for same word
                }
            }

            return notYetCompletelyFilledPuzzle;
        }

        private bool TryFillingWord(DirectionInGrid direction, LocationOnGrid location, string word, Puzzle notYetCompletelyFilledPuzzle)
        {
            var targetSpaceInGrid = notYetCompletelyFilledPuzzle.GetWordOfLengthInDirectionAtLocation(word.Length, direction, location);

            var okToPutWord = true;

            for (int i = 0; i < word.Length; i++)
            {
                var cWord = word[i];
                var cTarget = targetSpaceInGrid[i];

                if ((cTarget == ' ') || (cTarget == cWord))
                {
                    //okToPutWord remains true;
                }
                else
                {
                    okToPutWord = false;
                    break;
                }
            }

            if (!okToPutWord)
            {
                return false;
            }

            notYetCompletelyFilledPuzzle.PutWordAtLocationInDirection(word, location, direction);

            return true;
        }

        private bool IsSpaceEnough(DirectionInGrid direction, LocationOnGrid location, string word, Puzzle puzzle)
        {
            var finalLoc = location.AfterSoManyStepsInDirection(word.Length, direction);
            return finalLoc.IsValidLocationInPuzzle(puzzle, finalLoc);
        }

        public void PrintPuzzle(Puzzle puzzle)
        {
            for (int i = 0; i < puzzle.iMax; i++)
            {
                for (int j = 0; j < puzzle.jMax; j++)
                {
                    Console.Write(puzzle.PuzzleGrid[i, j]);
                    Console.Write(" ");
                }

                Console.WriteLine();
            }
        }

        public void PrintPuzzleSolution(Puzzle puzzle)
        {
            for (int i = 0; i < puzzle.iMax; i++)
            {
                for (int j = 0; j < puzzle.jMax; j++)
                {
                    Console.Write(puzzle.PuzzleGridSolutionOnly[i, j]);
                    Console.Write(" ");
                }

                Console.WriteLine();
            }
        }

        public void SavePuzzleSolution(Puzzle puzzle, string path)
        {
            var fileName = puzzle.PuzzleCategory + "_" + puzzle.PuzzleTime.ToString().
                Replace('/', '-').Replace(':', '_') + ".txt";
            var sw = new StreamWriter(Path.Combine(path, fileName));

            for (int i = 0; i < puzzle.iMax; i++)
            {
                for (int j = 0; j < puzzle.jMax; j++)
                {
                    sw.Write(puzzle.PuzzleGridSolutionOnly[i, j]);
                    sw.Write(" ");
                }

                sw.WriteLine();
            }

            sw.Flush();
        }

        public void SavePuzzle(Puzzle puzzle, string path)
        {
            var fileName = puzzle.PuzzleCategory + "_" + puzzle.PuzzleTime.ToString().
                Replace('/','-').Replace(':','_') + ".txt";
            var sw = new StreamWriter(Path.Combine(path, fileName));

            for (int i = 0; i < puzzle.iMax; i++)
            {
                for (int j = 0; j < puzzle.jMax; j++)
                {
                    sw.Write(puzzle.PuzzleGrid[i, j]);
                    sw.Write(" ");
                }

                sw.WriteLine();
            }

            sw.Flush();
        }
    }

    public class PuzzleSolutionWord
    {
        public string Word;
        public DirectionInGrid Direction;
        public LocationOnGrid Location;
        public PuzzleSolutionWord(string word, DirectionInGrid direction, LocationOnGrid location)
        {
            Word = word;
            Direction = direction;
            Location = location;
        }
    }

    public class Puzzle
    {
        public DateTime PuzzleTime;

        public char[,] PuzzleGrid;
        public char[,] PuzzleGridSolutionOnly;

        public string PuzzleCategory;

        public Dictionary<string, PuzzleSolutionWord> PuzzleSolutionSet;
        public List<string> WordsInThePuzzleFilled;
        public List<string> WordsInThePuzzleDesired;
        public int iMax;
        public int jMax;

        public char CharAtLocation(LocationOnGrid location)
        {
            return PuzzleGrid[location.i,location.j];
        }

        public void PutCharAtLocation(char c, LocationOnGrid location)
        {
            var existingChar = CharAtLocation(location);

            if ( existingChar == c)
            {
                return;
            }

            if (existingChar != ' ')
            {
                throw new Exception($"Can not add char {c} at location ({location.i},{location.j})");
            }

            PuzzleGrid[location.i, location.j] = c;
        }

        public string GetWordOfLengthInDirectionAtLocation(int wordLength, DirectionInGrid direction, LocationOnGrid location)
        {
            string wordOut = "";

            for (int i = 0; i < wordLength; i++)
            { 
                var c = CharAtLocation(location);
                wordOut += c.ToString();
                location = location.AfterSoManyStepsInDirection(1, direction);
            }

            return wordOut;
        }

        internal void PutWordAtLocationInDirection(string word, LocationOnGrid location, DirectionInGrid direction)
        {
            foreach (char c in word)
            {
                PutCharAtLocation(c, location);
                location = location.AfterSoManyStepsInDirection(1, direction);
            }
        }

        public Puzzle PadEmptyCells()
        {
            for (int i = 0; i < iMax; i++)
            {
                for (int j = 0; j < jMax; j++)
                {
                    if (PuzzleGrid[i,j] == ' ')
                    {
                        PuzzleGrid[i, j] = HelpWithRandomPick.GetRandomChar();
                    }
                }
            }

            return this;
        }
    }

    public class LocationOnGrid
    {
        public int i;
        public int j;

        public LocationOnGrid(int iLoc, int jLoc)
        {
            i = iLoc;
            j = jLoc;
        }

        public LocationOnGrid AfterSoManyStepsInDirection(int nSteps, DirectionInGrid direction)
        {  
            int iLoc = i;
            int jLoc = j;

            int iNext = 0;
            int jNext = 0;

            switch (direction.Direction)
            {
                case 1:
                    iNext = iLoc;
                    jNext = jLoc + nSteps;
                    break;
                case 2:
                    iNext = iLoc - nSteps;
                    jNext = jLoc + nSteps;
                    break;
                case 3:
                    iNext = iLoc - nSteps;
                    jNext = jLoc;
                    break;
                case 4:
                    iNext = iLoc - nSteps;
                    jNext = jLoc - nSteps;
                    break;
                case 5:
                    iNext = iLoc;
                    jNext = jLoc - nSteps;
                    break;
                case 6:
                    iNext = iLoc + nSteps;
                    jNext = jLoc - nSteps;
                    break;
                case 7:
                    iNext = iLoc + nSteps;
                    jNext = jLoc;
                    break;
                case 8:
                    iNext = iLoc + nSteps;
                    jNext = jLoc + nSteps;
                    break;
                default:
                    throw new Exception($"Unknown direction number {direction}");
            }

            var newLocation = new LocationOnGrid(iNext, jNext);
            return newLocation;
        }

        public bool IsValidLocationInPuzzle(Puzzle puzzle, LocationOnGrid location)
        {
            int iMax = puzzle.iMax;
            int jMax = puzzle.jMax;

            if ((location.i < 0) || (location.j < 0))
            {
                return false;
            }

            if ((location.i > iMax) || (location.j > jMax))
            {
                return false;
            }

            return true;

        }
    }

    public class DirectionInGrid
    {
        //In direction 1 only j increases i remains same
        //In direction 2 j increases and i decreases
        //In direction 3 i decreases j remains same
        //In direction 4 j reduces, i reduces
        //In direction 5 j reduces i remains thhe same
        //In direction 6 j reduces i increases
        //In direction 7 j remains same i increases
        //In direction 8 i increaes j increases

        public int Direction;

        public LocationOnGrid GetNextLocationInDirection(LocationOnGrid location)
        {
            int iLoc = location.i;
            int jLoc = location.j;

            int iNext;
            int jNext;

            switch (Direction)
            {
                case 1:
                    iNext = iLoc;
                    jNext = jLoc++;
                    break;
                case 2:
                    iNext = iLoc--;
                    jNext = jLoc++;
                    break;
                case 3:
                    iNext = iLoc--;
                    jNext = jLoc;
                    break;
                case 4:
                    iNext = iLoc--;
                    jNext = jLoc--;
                    break;
                case 5:
                    iNext = iLoc;
                    jNext = jLoc--;
                    break;
                case 6:
                    iNext = iLoc++;
                    jNext = jLoc--;
                    break;
                case 7:
                    iNext = iLoc++;
                    jNext = jLoc;
                    break;
                case 8:
                    iNext = iLoc++;
                    jNext = jLoc++;
                    break;
                default:
                    throw new Exception($"Unknown direction number {Direction}");
            }

            return new LocationOnGrid(iNext, jNext);
        }

        public void SetToRandomDirection()
        {
            Direction = HelpWithRandomPick.GetRandomDirection();
        }

        public void SetToNextDirection()
        {
            var nextNo = Direction + 1;

            if (nextNo > 8)
            {
                nextNo = 1;
            }

            Direction = nextNo;
        }
    }

    public static class HelpWithRandomPick
    {
        public static readonly Random Rng = new Random(DateTime.UtcNow.Second);
        public static char GetRandomChar()
        {
            var randInt = Rng.Next(1,27);
            return CharMap[randInt];           
        }

        public static int GetRandomDirection()
        {
            var randInt = Rng.Next(1, 9);
            return randInt;
        }

        public static LocationOnGrid GetRandomLocation(int iMax, int jMax)
        {
            var randi = Rng.Next(0, iMax);
            var randj = Rng.Next(0, jMax);
            return new LocationOnGrid(randi, randj);
        }

        private static readonly Dictionary<int, char> CharMap = new Dictionary<int, char>
        {
            {1, 'A' },
            {2, 'B' },
            {3, 'C' },
            {4, 'D' },
            {5, 'E' },
            {6, 'F' },
            {7, 'G' },
            {8, 'H' },
            {9, 'I' },
            {10, 'J' },
            {11, 'K' },
            {12, 'L' },
            {13, 'M' },
            {14, 'N' },
            {15, 'O' },
            {16, 'P' },
            {17, 'Q' },
            {18, 'R' },
            {19, 'S' },
            {20, 'T' },
            {21, 'U' },
            {22, 'V' },
            {23, 'W' },
            {24, 'X' },
            {25, 'Y' },
            {26, 'Z' },
        };
    }

}
