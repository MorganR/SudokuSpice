using System;

namespace SudokuSpice.ConstraintBased.Constraints
{
    /// <summary>
    /// Enforces the constraint that all values in on each diagonal must be unique.
    /// </summary>
    public class DiagonalUniquenessConstraint : IConstraint
    {
        /// <inheritdoc/>
        public void Constrain(IReadOnlyPuzzle puzzle, ExactCoverMatrix matrix)
        {
            _ConstrainForwardDiagonal(puzzle, matrix);
            _ConstrainBackwardDiagonal(puzzle, matrix);
        }

        private static void _ConstrainForwardDiagonal(IReadOnlyPuzzle puzzle, ExactCoverMatrix matrix)
        {
            Span<Coordinate> coordinates = stackalloc Coordinate[puzzle.Size];
            for (int row = 0, col = puzzle.Size - 1; row < puzzle.Size; row++, col--)
            {
                coordinates[row] = new Coordinate(row, col);
            }
            ConstraintUtil.ImplementUniquenessConstraintForSquares(puzzle, coordinates, matrix);
        }

        private static void _ConstrainBackwardDiagonal(IReadOnlyPuzzle puzzle, ExactCoverMatrix matrix)
        {
            Span<Coordinate> Coordinates = stackalloc Coordinate[puzzle.Size];
            for (int row = 0, col = 0; row < puzzle.Size; row++, col++)
            {
                Coordinates[row] = new Coordinate(row, col);
            }
            ConstraintUtil.ImplementUniquenessConstraintForSquares(puzzle, Coordinates, matrix);
        }
    }
}