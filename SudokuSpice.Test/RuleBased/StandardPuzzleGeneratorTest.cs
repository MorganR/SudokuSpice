﻿using System;
using Xunit;

namespace SudokuSpice.RuleBased.Test
{
    public class StandardPuzzleGeneratorTest
    {
        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(3)]
        [InlineData(30)]
        [InlineData(36)]
        [InlineData(400)]
        public void Generate_WithInvalidPuzzleSize_Throws(int size)
        {
            var generator = new StandardPuzzleGenerator();
            Assert.Throws<ArgumentException>(() => generator.Generate(size, 1, TimeSpan.FromSeconds(60)));
        }

        [Theory]
        [InlineData(1, -1)]
        [InlineData(1, 2)]
        [InlineData(4, 3)]
        [InlineData(4, 17)]
        [InlineData(9, 16)]
        [InlineData(9, 82)]
        [InlineData(16, 40)]
        [InlineData(16, 266)]
        [InlineData(25, 100)]
        [InlineData(25, 626)]
        public void Generate_WithInvalidNumToSet_Throws(int size, int numToSet)
        {
            var generator = new StandardPuzzleGenerator();

            Assert.Throws<ArgumentOutOfRangeException>(
                () => generator.Generate(size, numToSet, TimeSpan.FromSeconds(60)));
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(4, 10)]
        [InlineData(9, 30)]
        public void Generate_CreatesPuzzleWithUniqueSolution(int size, int numToSet)
        {
            var generator = new StandardPuzzleGenerator();

            PuzzleWithPossibleValues puzzle = generator.Generate(size, numToSet, TimeSpan.FromSeconds(60));

            Assert.Equal(size * size - numToSet, puzzle.NumEmptySquares);
            var solver = StandardPuzzles.CreateSolver();
            SolveStats stats = solver.ComputeStatsForAllSolutions(puzzle);
            Assert.Equal(1, stats.NumSolutionsFound);
        }

        [Fact]
        public void Generate_WithShortTimeout_ThrowsTimeoutException()
        {
            var generator = new StandardPuzzleGenerator();

            Assert.Throws<TimeoutException>(
                () => generator.Generate(16, 150, TimeSpan.FromMilliseconds(1)));
        }
    }
}