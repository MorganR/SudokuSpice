using SudokuSpice.ConstraintBased.Constraints;
using System;
using System.Collections.Generic;
using Xunit;

namespace SudokuSpice.ConstraintBased.Test
{
    public class PuzzleGeneratorTest
    {
        [Fact]
        public void Constructor_WithValidArgs_Works()
        {
            var generator = new PuzzleGenerator<Puzzle>(
                () => new Puzzle(9), new List<IConstraint> { new RowUniquenessConstraint() });
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(4, 10)]
        [InlineData(9, 30)]
        public void Generate_CreatesPuzzleWithUniqueSolution(int size, int numToSet)
        {
            var generator = new PuzzleGenerator<Puzzle>(
                () => new Puzzle(size),
                new List<IConstraint> {
                    new RowUniquenessConstraint(),
                    new ColumnUniquenessConstraint(),
                    new BoxUniquenessConstraint()
                });

            Puzzle puzzle = generator.Generate(numToSet, TimeSpan.FromSeconds(60));

            Assert.Equal(size * size - numToSet, puzzle.NumEmptySquares);
            var solver = RuleBased.StandardPuzzles.CreateSolver(puzzle.Size);
            SolveStats stats = solver.GetStatsForAllSolutions(puzzle);
            Assert.Equal(1, stats.NumSolutionsFound);
        }

        [Fact]
        public void Generate_WithShortTimeout_ThrowsTimeoutException()
        {
            var generator = new PuzzleGenerator<Puzzle>(
                () => new Puzzle(9),
                new List<IConstraint> {
                    new RowUniquenessConstraint(),
                    new ColumnUniquenessConstraint(),
                    new BoxUniquenessConstraint()
                });

            Assert.Throws<TimeoutException>(
                () => generator.Generate(17, TimeSpan.FromMilliseconds(1)));
        }
    }
}
