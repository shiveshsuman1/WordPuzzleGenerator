using System;
using System.Runtime;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json.Serialization;

namespace Word_Puzzle_Generator
{
    [DataContract]
    public class PuzzleDto
    {
        [DataMember]
        public char[,] PuzzleGrid;

        [DataMember]
        public char[,] PuzzleGridSolutionOnly;

        [DataMember]
        public WordsDatabase.Category PuzzleCategory;
    }
}
