using SudokuSpice.ConstraintBased.Constraints;
using System.Collections.Generic;

namespace SudokuSpice.ConstraintBased
{
    /// <summary>Provides utilities for interacting with standard Sudoku puzzles.</summary>
    public static class StandardPuzzles
    {
        /// <summary>Creates an efficient solver for solving standard Sudoku puzzles.</summary>
        public static PuzzleSolver<Puzzle> CreateSolver()
        {
            return new PuzzleSolver<Puzzle>(
                new IConstraint[] {
                    new RowUniquenessConstraint(),
                    new ColumnUniquenessConstraint(),
                    new BoxUniquenessConstraint(),
                });
        }
    }
}