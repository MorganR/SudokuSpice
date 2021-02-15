using System;

namespace SudokuSpice.ConstraintBased.Constraints
{
    /// <summary>
    /// Enforces the constraint that all values in on each diagonal must be unique.
    /// </summary>
    public class DiagonalUniquenessConstraint : IConstraint
    {
        /// <inheritdoc/>
        public bool TryConstrain(IReadOnlyPuzzle puzzle, ExactCoverGraph matrix)
        {
            return _TryConstrainForwardDiagonal(puzzle, matrix)
                && _TryConstrainBackwardDiagonal(puzzle, matrix);
        }

        private static bool _TryConstrainForwardDiagonal(IReadOnlyPuzzle puzzle, ExactCoverGraph matrix)
        {
            Span<Coordinate> coordinates = stackalloc Coordinate[puzzle.Size];
            for (int row = 0, col = puzzle.Size - 1; row < puzzle.Size; row++, col--)
            {
                coordinates[row] = new Coordinate(row, col);
            }
            return ConstraintUtil.TryImplementUniquenessConstraintForSquares(puzzle, coordinates, matrix);
        }

        private static bool _TryConstrainBackwardDiagonal(IReadOnlyPuzzle puzzle, ExactCoverGraph matrix)
        {
            Span<Coordinate> Coordinates = stackalloc Coordinate[puzzle.Size];
            for (int row = 0, col = 0; row < puzzle.Size; row++, col++)
            {
                Coordinates[row] = new Coordinate(row, col);
            }
            return ConstraintUtil.TryImplementUniquenessConstraintForSquares(puzzle, Coordinates, matrix);
        }
    }
}