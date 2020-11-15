using SudokuSpice.ConstraintBased.Constraints;
using SudokuSpice.RuleBased;
using System;
using System.Collections.Generic;
using Xunit;

namespace SudokuSpice.ConstraintBased.Test
{
    public class ConstraintBasedGeneratorTest
    {
        [Fact]
        public void Constructor_WithValidArgs_Works()
        {
            var generator = new ConstraintBasedGenerator<Puzzle>(
                () => new Puzzle(9), new List<IConstraint<Puzzle>> { new RowUniquenessConstraint<Puzzle>() });
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(4, 10)]
        [InlineData(9, 30)]
        public void Generate_CreatesPuzzleWithUniqueSolution(int size, int numToSet)
        {
            var generator = new ConstraintBasedGenerator<Puzzle>(
                () => new Puzzle(size),
                new List<IConstraint<Puzzle>> {
                    new RowUniquenessConstraint<Puzzle>(),
                    new ColumnUniquenessConstraint<Puzzle>(),
                    new BoxUniquenessConstraint<Puzzle>()
                });

            Puzzle puzzle = generator.Generate(numToSet, TimeSpan.FromSeconds(60));

            Assert.Equal(size * size - numToSet, puzzle.NumEmptySquares);
            var solver = new Solver(puzzle);
            SolveStats stats = solver.GetStatsForAllSolutions();
            Assert.Equal(1, stats.NumSolutionsFound);
        }

        [Fact]
        public void Generate_WithShortTimeout_ThrowsTimeoutException()
        {
            var generator = new ConstraintBasedGenerator<Puzzle>(
                () => new Puzzle(9),
                new List<IConstraint<Puzzle>> {
                    new RowUniquenessConstraint<Puzzle>(),
                    new ColumnUniquenessConstraint<Puzzle>(),
                    new BoxUniquenessConstraint<Puzzle>()
                });

            Assert.Throws<TimeoutException>(
                () => generator.Generate(17, TimeSpan.FromMilliseconds(1)));
        }
    }
}
