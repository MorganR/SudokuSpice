using SudokuSpice.ConstraintBased.Constraints;
using SudokuSpice.Test;
using System;
using System.Collections.Generic;
using Xunit;

namespace SudokuSpice.ConstraintBased.Test
{
    public class PuzzleSolverTest
    {
        [Theory]
        [ClassData(typeof(ValidStandardPuzzles))]
        public void Solve_ValidPuzzle_SolvesPuzzle(int?[,] matrix)
        {
            var puzzle = Puzzle.CopyFrom(matrix);
            var solver = new PuzzleSolver<Puzzle>(
                new List<IConstraint> { new RowUniquenessConstraint(), new ColumnUniquenessConstraint(), new BoxUniquenessConstraint() });
            var solved = solver.Solve(puzzle);
            PuzzleTestUtils.AssertStandardPuzzleSolved(solved);
        }

        [Fact]
        public void Solve_WithMegaPuzzle_Solves()
        {
            var puzzle = new Puzzle(new int?[,] {
                {null, null, null, null, 10,   1,    null, 8,    null, 15,   3,    11,   null, 2,    16,   null},
                {14,   null, 2,    null, null, 4,    3,    null, null, 13,   8,    null, null, 12,   null, null},
                {null, null, null, 12,   null, null, null, 15,   null, null, null, 7,    null, null, 9,    10},
                {1,    10,   15,   null, 6,    null, null, null, null, 14,   null, null, null, null, null, 11},
                {null, 11,   14,   6,    null, null, null, 9,    13,   8,    null, null, null, null, 2,    3},
                {12,   null, null, null, 4,    null, 7,    3,    11,   6,    null, null, 16,   null, 5,    null},
                {13,   16,   null, 2,    null, null, null, 1,    null, null, 5,    null, 10,   9,    null, null},
                {null, 4,    null, null, 13,   null, 2,    null, null, null, 16,   3,    11,   null, null, null},
                {null, null, null, 10,   3,    6,    null, null, null, 9,    null, 12,   null, null, 4,    null},
                {null, null, 12,   15,   null, 9,    null, null, 7,    null, null, null, 1,    null, 3,    14},
                {null, 1,    null, 4,    null, null, 5,    12,   3,    10,   null, 8,    null, null, null, 2},
                {3,    6,    null, null, null, null, 15,   10,   4,    null, null, null, 12,   5,    7,    null},
                {2,    null, null, null, null, null, 4,    null, null, null, null, 15,   null, 16,   11,   9},
                {4,    14,   null, null, 16,   null, null, null, 2,    null, null, null, 6,    null, null, null},
                {null, null, 16,   null, null, 7,    8,    null, null, 4,    10,   null, null, 14,   null, 5},
                {null, 3,    6,    null, 9,    12,   14,   null, 8,    null, 13,   16,   null, null, null, null}
            });
            var solver = new PuzzleSolver<Puzzle>(
                new List<IConstraint> { new RowUniquenessConstraint(), new ColumnUniquenessConstraint(), new BoxUniquenessConstraint(), new DiagonalUniquenessConstraint() });

            var solved = solver.Solve(puzzle);

            PuzzleTestUtils.AssertStandardPuzzleSolved(solved);
        }

        [Theory]
        [ClassData(typeof(ValidStandardPuzzles))]
        public void Solve_ValidPuzzleWithRandomGuesses_SolvesPuzzle(int?[,] matrix)
        {
            var puzzle = new Puzzle(matrix);
            var solver = new PuzzleSolver<Puzzle>(
                new List<IConstraint> { new RowUniquenessConstraint(), new ColumnUniquenessConstraint(), new BoxUniquenessConstraint() });

            var solved = solver.Solve(puzzle);

            PuzzleTestUtils.AssertStandardPuzzleSolved(solved);
        }

        [Theory]
        [ClassData(typeof(InvalidStandardPuzzles))]
        public void Solve_InvalidPuzzle_Throws(int?[,] matrix)
        {
            var puzzle = new Puzzle(matrix);
            var solver = new PuzzleSolver<Puzzle>(
                new List<IConstraint> { new RowUniquenessConstraint(), new ColumnUniquenessConstraint(), new BoxUniquenessConstraint() });
            Assert.Throws<ArgumentException>(() => solver.Solve(puzzle));
        }

        [Theory]
        [ClassData(typeof(InvalidStandardPuzzles))]
        public void Solve_InvalidPuzzleWithRandomGuesses_Throws(int?[,] matrix)
        {
            var puzzle = new Puzzle(matrix);
            var solver = new PuzzleSolver<Puzzle>(
                new List<IConstraint> { new RowUniquenessConstraint(), new ColumnUniquenessConstraint(), new BoxUniquenessConstraint() });
            Assert.Throws<ArgumentException>(() => solver.Solve(puzzle, randomizeGuesses: true));
        }

        [Theory]
        [ClassData(typeof(ValidStandardPuzzles))]
        public void TrySolve_ValidPuzzle_SolvesPuzzle(int?[,] matrix)
        {
            var puzzle = new Puzzle(matrix);
            var solver = new PuzzleSolver<Puzzle>(
                new List<IConstraint> { new RowUniquenessConstraint(), new ColumnUniquenessConstraint(), new BoxUniquenessConstraint() });
            Assert.True(solver.TrySolve(puzzle));
            PuzzleTestUtils.AssertStandardPuzzleSolved(puzzle);
        }

        [Theory]
        [ClassData(typeof(ValidStandardPuzzles))]
        public void TrySolve_ValidPuzzleWithRandomGuesses_SolvesPuzzle(int?[,] matrix)
        {
            var puzzle = new Puzzle(matrix);
            var solver = new PuzzleSolver<Puzzle>(
                new List<IConstraint> { new RowUniquenessConstraint(), new ColumnUniquenessConstraint(), new BoxUniquenessConstraint() });
            Assert.True(solver.TrySolve(puzzle, randomizeGuesses: true));
            PuzzleTestUtils.AssertStandardPuzzleSolved(puzzle);
        }

        [Theory]
        [ClassData(typeof(InvalidStandardPuzzles))]
        public void TrySolve_InvalidPuzzle_ReturnsFalse(int?[,] matrix)
        {
            var puzzle = new Puzzle(matrix);
            var solver = new PuzzleSolver<Puzzle>(
                new List<IConstraint> { new RowUniquenessConstraint(), new ColumnUniquenessConstraint(), new BoxUniquenessConstraint() });
            Assert.False(solver.TrySolve(puzzle));
        }

        [Theory]
        [ClassData(typeof(InvalidStandardPuzzles))]
        public void TrySolve_InvalidPuzzleWithRandomGuesses_ReturnsFalse(int?[,] matrix)
        {
            var puzzle = new Puzzle(matrix);
            var solver = new PuzzleSolver<Puzzle>(
                new List<IConstraint> { new RowUniquenessConstraint(), new ColumnUniquenessConstraint(), new BoxUniquenessConstraint() });
            Assert.False(solver.TrySolve(puzzle, randomizeGuesses: true));
        }

        [Theory]
        [ClassData(typeof(PuzzlesWithStats))]
        public void ComputeStatsForAllSolutions_ReturnsExpectedNumSolutions(int?[,] matrix, SolveStats expectedStats)
        {
            var solver = new PuzzleSolver<Puzzle>(
                new List<IConstraint> { new RowUniquenessConstraint(), new ColumnUniquenessConstraint(), new BoxUniquenessConstraint() });
            Assert.Equal(
                expectedStats.NumSolutionsFound,
                solver.ComputeStatsForAllSolutions(new Puzzle(matrix)).NumSolutionsFound);
        }

        [Theory]
        [ClassData(typeof(InvalidStandardPuzzles))]
        public void ComputeStatsForAllSolutions_WithInvalidPuzzles_ReturnsNoSolutions(int?[,] matrix)
        {
            var puzzle = new Puzzle(matrix);
            var solver = new PuzzleSolver<Puzzle>(
                new List<IConstraint> { new RowUniquenessConstraint(), new ColumnUniquenessConstraint(), new BoxUniquenessConstraint() });
            Assert.Equal(0, solver.ComputeStatsForAllSolutions(puzzle).NumSolutionsFound);
        }
    }
}