using System;
using Xunit;

namespace SudokuSpice.RuleBased.Test
{
    public class PuzzleGeneratorTest
    {
        [Fact]
        public void Constructor_WithValidArgs_Works()
        {
            var generator = new PuzzleGenerator(StandardPuzzles.CreateSolver());
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(4, 10)]
        [InlineData(9, 30)]
        public void Generate_CreatesPuzzleWithUniqueSolution(int size, int numToSet)
        {
            var generator = new PuzzleGenerator(StandardPuzzles.CreateSolver());

            int?[,] puzzle = generator.Generate(size, numToSet, TimeSpan.FromSeconds(60));

            int numSet = 0;
            for (int row = 0; row < size; ++row)
            {
                for (int col = 0; col < size; ++col)
                {
                    if (puzzle[row, col].HasValue)
                    {
                        ++numSet;
                    }
                }
            }
            Assert.Equal(numToSet, numSet);
            PuzzleSolver solver = StandardPuzzles.CreateSolver();
            SolveStats stats = solver.GetStatsForAllSolutions(puzzle);
            Assert.Equal(1, stats.NumSolutionsFound);
        }

        [Fact]
        public void Generate_WithShortTimeout_ThrowsTimeoutException()
        {
            var generator = new PuzzleGenerator(StandardPuzzles.CreateSolver());

            Assert.Throws<TimeoutException>(
                () => generator.Generate(25, 185, TimeSpan.FromMilliseconds(1)));
        }
    }
}