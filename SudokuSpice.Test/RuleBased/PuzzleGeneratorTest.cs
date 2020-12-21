using System;
using Xunit;

namespace SudokuSpice.RuleBased.Test
{
    public class PuzzleGeneratorTest
    {
        [Fact]
        public void Constructor_WithValidArgs_Works()
        {
            var generator = new PuzzleGenerator<Puzzle>(
                size => new Puzzle(size), StandardPuzzles.CreateSolver());
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(4, 10)]
        [InlineData(9, 30)]
        public void Generate_CreatesPuzzleWithUniqueSolution(int size, int numToSet)
        {
            var generator = new PuzzleGenerator<Puzzle>(
                size => new Puzzle(size), StandardPuzzles.CreateSolver());

            Puzzle puzzle = generator.Generate(size, numToSet, TimeSpan.FromSeconds(60));

            Assert.Equal(size * size - numToSet, puzzle.NumEmptySquares);
            PuzzleSolver solver = StandardPuzzles.CreateSolver();
            SolveStats stats = solver.GetStatsForAllSolutions(puzzle);
            Assert.Equal(1, stats.NumSolutionsFound);
        }

        [Fact]
        public void Generate_WithShortTimeout_ThrowsTimeoutException()
        {
            var generator = new PuzzleGenerator<Puzzle>(
                size => new Puzzle(size), StandardPuzzles.CreateSolver());

            Assert.Throws<TimeoutException>(
                () => generator.Generate(16, 150, TimeSpan.FromMilliseconds(1)));
        }
    }
}