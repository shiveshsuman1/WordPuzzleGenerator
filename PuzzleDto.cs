using System;
namespace Word_Puzzle_Generator
{
    [DataContract]
    public class PuzzleDto
    {
        public char[,] PuzzleGrid;

        public char[,] PuzzleGridSolutionOnly;

        public WordsDatabase.Category PuzzleCategory;
    }
}
