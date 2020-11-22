﻿using System;
using Xunit;

namespace SudokuSpice.RuleBased.Test
{
    public class PuzzleGeneratorTest
    {
        [Fact]
        public void Constructor_WithValidArgs_Works()
        {
            var generator = new PuzzleGenerator<Puzzle>(
                () => new Puzzle(9), StandardPuzzles.CreateSolver(9));
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(4, 10)]
        [InlineData(9, 30)]
        public void Generate_CreatesPuzzleWithUniqueSolution(int size, int numToSet)
        {
            var generator = new PuzzleGenerator<Puzzle>(
                () => new Puzzle(size), StandardPuzzles.CreateSolver(size));

            Puzzle puzzle = generator.Generate(numToSet, TimeSpan.FromSeconds(60));

            Assert.Equal(size * size - numToSet, puzzle.NumEmptySquares);
            PuzzleSolver solver = StandardPuzzles.CreateSolver(size);
            SolveStats stats = solver.GetStatsForAllSolutions(puzzle);
            Assert.Equal(1, stats.NumSolutionsFound);
        }

        [Fact]
        public void Generate_WithShortTimeout_ThrowsTimeoutException()
        {
            var generator = new PuzzleGenerator<Puzzle>(
                () => new Puzzle(9), StandardPuzzles.CreateSolver(9));

            Assert.Throws<TimeoutException>(
                () => generator.Generate(17, TimeSpan.FromMilliseconds(1)));
        }
    }
}
