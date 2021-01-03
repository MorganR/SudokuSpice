using System;

namespace SudokuSpice.ConstraintBased.Constraints
{
    /// <summary>
    /// Enforces the constraint that all values in a column must be unique.
    /// </summary>
    public class ColumnUniquenessConstraint : IConstraint
    {
        /// <inheritdoc/>
        public bool TryConstrain(IReadOnlyPuzzle puzzle, ExactCoverMatrix matrix)
        {
            Span<Coordinate> columnCoordinates = stackalloc Coordinate[puzzle.Size];
            for (int column = 0; column < puzzle.Size; column++)
            {
                for (int row = 0; row < puzzle.Size; row++)
                {
                    columnCoordinates[row] = new Coordinate(row, column);
                }
                if (!ConstraintUtil.TryImplementUniquenessConstraintForSquares(puzzle, columnCoordinates, matrix))
                {
                    return false;
                }
            }
            return true;
        }
    }
}