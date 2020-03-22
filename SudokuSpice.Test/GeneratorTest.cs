using System;
using Xunit;

namespace SudokuSpice.Test
{
    public class GeneratorTest
    {
        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(3)]
        [InlineData(30)]
        [InlineData(36)]
        [InlineData(400)]
        public void Constructor_WithInvalidArgs_Throws(int size)
        {
            Assert.Throws<ArgumentException>(() => new Generator(size));
        }

        [Theory]
        [InlineData(1, -1)]
        [InlineData(1, 2)]
        [InlineData(4, 0)]
        [InlineData(4, 5)]
        [InlineData(9, 16)]
        [InlineData(9, 82)]
        [InlineData(16, 40)]
        [InlineData(16, 266)]
        [InlineData(25, 100)]
        [InlineData(25, 626)]
        public void Generate_WithInvalidArgs_Throws(int size, int numToSet)
        {
            var generator = new Generator(size);

            Assert.Throws<ArgumentException>(() => generator.Generate(numToSet));
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(4, 2)]
        [InlineData(9, 30)]
        public void Generate_CreatesPuzzleWithUniqueSolution(int size, int numToSet)
        {
            var generator = new Generator(size);
            
            var puzzle = generator.Generate(numToSet);

            var solver = new Solver(puzzle);
            var stats = solver.GetStatsForAllSolutions();
            Assert.Equal(1, stats.NumSolutionsFound);
        }
    }
}
