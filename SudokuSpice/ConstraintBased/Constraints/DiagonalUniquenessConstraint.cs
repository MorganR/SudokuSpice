using System;

namespace SudokuSpice.ConstraintBased.Constraints
{
    /// <summary>
    /// Enforces the constraint that all values in on each diagonal must be unique.
    /// </summary>
    public class DiagonalUniquenessConstraint : IConstraint
    {
        /// <inheritdoc/>
        public bool TryConstrain(IReadOnlyPuzzle puzzle, ExactCoverGraph graph)
        {
            return _TryConstrainForwardDiagonal(puzzle, graph)
                && _TryConstrainBackwardDiagonal(puzzle, graph);
        }

        private static bool _TryConstrainForwardDiagonal(IReadOnlyPuzzle puzzle, ExactCoverGraph graph)
        {
            Span<Coordinate> coordinates = stackalloc Coordinate[puzzle.Size];
            for (int row = 0, col = puzzle.Size - 1; row < puzzle.Size; row++, col--)
            {
                coordinates[row] = new Coordinate(row, col);
            }
            return ConstraintUtil.TryImplementUniquenessConstraintForSquares(puzzle, coordinates, graph);
        }

        private static bool _TryConstrainBackwardDiagonal(IReadOnlyPuzzle puzzle, ExactCoverGraph graph)
        {
            Span<Coordinate> Coordinates = stackalloc Coordinate[puzzle.Size];
            for (int row = 0, col = 0; row < puzzle.Size; row++, col++)
            {
                Coordinates[row] = new Coordinate(row, col);
            }
            return ConstraintUtil.TryImplementUniquenessConstraintForSquares(puzzle, Coordinates, graph);
        }
    }
}