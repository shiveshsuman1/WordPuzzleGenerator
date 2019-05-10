using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Word_Puzzle_Generator
{
    class Program
    {
        private enum _foldernames { WordsBank, Puzzles, PuzzleSolutions }
        private static Dictionary<_foldernames, string> _foldernamesDict =>
            new Dictionary<_foldernames, string>
            {
                { _foldernames.Puzzles, "Puzzles" },
                { _foldernames.WordsBank, "WordsBank" },
                { _foldernames.PuzzleSolutions, "PuzzleSolutions" }
            };
        static void Main(string[] args)
        {
            //List of files to create:
            //WordBank
            //Puzzles
            //PuzzleSolutions

            //WordsBank 

            //Category: <category>
            //list of words

            var pwd = WhereAnIRunning();
            string puzzleFolder = Path.Combine(pwd, _foldernamesDict[_foldernames.Puzzles]);
            string puzzleSolutionFolder = Path.Combine(pwd, _foldernamesDict[_foldernames.PuzzleSolutions]);
            string wordsBankFolder = Path.Combine(pwd, _foldernamesDict[_foldernames.WordsBank]);

            GetOrCreateFolder(puzzleFolder);
            GetOrCreateFolder(puzzleSolutionFolder);
            GetOrCreateFolder(wordsBankFolder);

            var wordCategoriesAndWords = GetWordsAndCategories(wordsBankFolder);
            var categoryIndexAndWords = new Dictionary<int, List<string>>();

            var categoryIndexAndCategory = new Dictionary<int, string>();
            int i = 0;
            if (wordCategoriesAndWords.Count == 0)
            {
                Console.WriteLine($"No words and categories were found!");
            }
            else
            {
                foreach (var categoryAndWords in wordCategoriesAndWords)
                {
                    categoryIndexAndCategory.Add(i, categoryAndWords.Key);
                    categoryIndexAndWords.Add(i, categoryAndWords.Value);
                    i++;
                }
            }

            bool playing = true;

            var maxCategoryIndex = i - 1;

            var selectedCategoryIndex = 0;

            while (playing)
            {
                Console.WriteLine($"Following categories were found: Please select a category index:");

                i = 0;
                foreach (var categoryIndexAndCategoryValue in categoryIndexAndCategory)
                {
                    Console.WriteLine($"[{categoryIndexAndCategoryValue.Key}]  {categoryIndexAndCategoryValue.Value}");
                    i++;
                }

                Console.Write("Your category selection: ");
                var input = Console.ReadLine();

                if (!int.TryParse(input, out selectedCategoryIndex))
                {
                    Console.WriteLine("\nPlease enter a valid category index (number)");
                    continue;
                }

                if (selectedCategoryIndex > maxCategoryIndex)
                {
                    Console.WriteLine($"Please enter a number smaller than {maxCategoryIndex}");
                    continue;
                }

                var selectedCategory = categoryIndexAndCategory[selectedCategoryIndex];

                var wordList = categoryIndexAndWords[selectedCategoryIndex];

                var puzzleMgr = new PuzzleManager();

                //var wordList = WordsDatabase.WordTable[WordsDatabase.Category.BodyParts];

                var puzzle = puzzleMgr.GetEmptyPuzzle(selectedCategory, wordList, 12, 12);

                var puzzleKey = puzzleMgr.GeneratePuzzleKey(puzzle);

                //Save the solution
                DataHandler.DataHandlerUtils.CopyPuzzleGrid(puzzle.PuzzleGrid, puzzle.PuzzleGridSolutionOnly);

                var finishedPuzzle = puzzleMgr.GetFinishedPuzzle(puzzleKey);

                Console.WriteLine("\nPuzzle: \n");
                puzzleMgr.PrintPuzzle(finishedPuzzle);

                Console.WriteLine("Do you want to see the puzzle solution?");
                Console.Write("Your answer: ");
                char userChoice = Console.ReadLine().ToUpper().Trim()[0];

                if (userChoice == 'Y')
                {
                    Console.WriteLine("Puzzle key:\n");
                    puzzleMgr.PrintPuzzleSolution(finishedPuzzle);
                }

                puzzleMgr.SavePuzzle(finishedPuzzle, puzzleFolder);
                puzzleMgr.SavePuzzleSolution(finishedPuzzle, puzzleSolutionFolder);
            }
        }

        private static string WhereAnIRunning()
        {
            var pwd = Directory.GetCurrentDirectory();
            return pwd;
        }

        private static void GetOrCreateFolders(List<string> folderPaths)
        {
            foreach (var path in folderPaths)
            {
                Directory.CreateDirectory(path);
            }
        }

        private static void GetOrCreateFolder(string path)
        {
            Directory.CreateDirectory(path);
        }

        private static Dictionary<string, List<string>> GetWordsAndCategories(string folderPath)
        {
            var files = Directory.GetFiles(folderPath.ToString());

            var categoriesAndWords = new Dictionary<string, List<string>>();

            foreach (var file in files)
            {
                var categoryAndWords = ParseWordBankFile(file);
                categoriesAndWords.Add(categoryAndWords.Category, categoryAndWords.Words);
            }

            return categoriesAndWords;
        }

        private static (string Category, List<string> Words) ParseWordBankFile(string path)
        {
            var Category = default(string);
            var Words = new List<string>();

            bool categoryFound = false;

            StreamReader streamReader = new StreamReader(path);

            while (!categoryFound)
            {
                var line = streamReader.ReadLine();
                {
                    if (streamReader.EndOfStream)
                    {
                        return (Category, Words);
                    }
                }

                if (line.Contains(':'))
                {
                    Category = line.Split(':')[1].Trim().ToUpper();
                    categoryFound = true;
                }
            }

            var endOfFileReached = streamReader.EndOfStream;

            do
            {
                var line = streamReader.ReadLine();
                {
                    if (streamReader.EndOfStream)
                    {
                        return (Category, Words);
                    }
                }

                if (!string.IsNullOrWhiteSpace(line))
                {
                    Words.Add(line.Trim().ToUpper());
                }
               
            } while (!endOfFileReached);


            return (Category, Words);
        }
    }
}
