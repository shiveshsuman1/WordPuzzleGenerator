using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Word_Puzzle_Generator
{
   public static class WordsDatabase
    {
        public enum Category { BodyParts, FarmAnimals };

        public static Dictionary<Category, List<string>> WordTable;

        static WordsDatabase()
        {
            WordTable = new Dictionary<Category, List<string>>();
            var entry1 = new List<string>()
            { "EAR", "EYE", "NOSE", "MOUTH", "TEETH", "KNEES", "TOES" };

            WordTable.Add(Category.BodyParts, entry1);
        }
    }
}
