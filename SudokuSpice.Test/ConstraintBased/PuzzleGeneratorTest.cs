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
                size => new Puzzle(size),
                new PuzzleSolver<Puzzle>(new IConstraint[] { new RowUniquenessConstraint() }));
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(4, 10)]
        [InlineData(9, 30)]
        public void Generate_CreatesPuzzleWithUniqueSolution(int size, int numToSet)
        {
            var generator = new PuzzleGenerator<Puzzle>(
                size => new Puzzle(size),
                new PuzzleSolver<Puzzle>(
                    new IConstraint[] {
                        new RowUniquenessConstraint(),
                        new ColumnUniquenessConstraint(),
                        new BoxUniquenessConstraint()
                    }));

            Puzzle puzzle = generator.Generate(size, numToSet, TimeSpan.FromSeconds(60));

            Assert.Equal(size * size - numToSet, puzzle.NumEmptySquares);
            var solver = RuleBased.StandardPuzzles.CreateSolver();
            SolveStats stats = solver.ComputeStatsForAllSolutions(new RuleBased.PuzzleWithPossibleValues(puzzle));
            Assert.Equal(1, stats.NumSolutionsFound);
        }

        [Fact]
        public void Generate_WithShortTimeout_ThrowsTimeoutException()
        {
            var generator = new PuzzleGenerator<Puzzle>(
                size => new Puzzle(size),
                new PuzzleSolver<Puzzle>(
                    new IConstraint[] {
                        new RowUniquenessConstraint(),
                        new ColumnUniquenessConstraint(),
                        new BoxUniquenessConstraint()
                    }));

            Assert.Throws<TimeoutException>(
                () => generator.Generate(16, 150, TimeSpan.FromMilliseconds(1)));
        }
    }
}