using System;
using Xunit;

namespace SudokuSpice.RuleBased.Test
{
    public class PuzzleGeneratorTest
    {
        [Fact]
        public void Constructor_WithValidArgs_Works()
        {
            var generator = new PuzzleGenerator<PuzzleWithPossibleValues>(
                size => new PuzzleWithPossibleValues(size), StandardPuzzles.CreateSolver());
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(4, 10)]
        [InlineData(9, 30)]
        public void Generate_CreatesPuzzleWithUniqueSolution(int size, int numToSet)
        {
            var generator = new PuzzleGenerator<PuzzleWithPossibleValues>(
                size => new PuzzleWithPossibleValues(size), StandardPuzzles.CreateSolver());

            PuzzleWithPossibleValues puzzle = generator.Generate(size, numToSet, TimeSpan.FromSeconds(60));

            Assert.Equal(size * size - numToSet, puzzle.NumEmptySquares);
            var solver = new ConstraintBased.PuzzleSolver<PuzzleWithPossibleValues>(
                new ConstraintBased.Constraints.IConstraint[] {
                    new ConstraintBased.Constraints.RowUniquenessConstraint(),
                    new ConstraintBased.Constraints.ColumnUniquenessConstraint(),
                    new ConstraintBased.Constraints.BoxUniquenessConstraint(),
                });
            SolveStats stats = solver.ComputeStatsForAllSolutions(puzzle);
            Assert.Equal(1, stats.NumSolutionsFound);
        }

        [Fact]
        public void Generate_WithShortTimeout_ThrowsTimeoutException()
        {
            var generator = new PuzzleGenerator<PuzzleWithPossibleValues>(
                size => new PuzzleWithPossibleValues(size), StandardPuzzles.CreateSolver());

            Assert.Throws<TimeoutException>(
                () => generator.Generate(16, 150, TimeSpan.FromMilliseconds(1)));
        }
    }
}