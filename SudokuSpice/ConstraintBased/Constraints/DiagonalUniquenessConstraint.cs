using System;

namespace SudokuSpice.ConstraintBased.Constraints
{
    /// <summary>
    /// Enforces the constraint that all values in on each diagonal must be unique.
    /// </summary>
    public class DiagonalUniquenessConstraint<TPuzzle> : IConstraint<TPuzzle> where TPuzzle : IReadOnlyPuzzle
    {
        /// <inheritdoc/>
        public void Constrain(TPuzzle puzzle, ExactCoverMatrix<TPuzzle> matrix)
        {
            _ConstrainForwardDiagonal(puzzle, matrix);
            _ConstrainBackwardDiagonal(puzzle, matrix);
        }

        private void _ConstrainForwardDiagonal(TPuzzle puzzle, ExactCoverMatrix<TPuzzle> matrix)
        {
            Span<Coordinate> coordinates = stackalloc Coordinate[puzzle.Size];
            for (int row = 0, col = puzzle.Size - 1;  row < puzzle.Size; row++, col--)
            {
                coordinates[row] = new Coordinate(row, col);
            }
            ConstraintUtil.ImplementUniquenessConstraintForSquares(puzzle, coordinates, matrix);
        }

        private void _ConstrainBackwardDiagonal(TPuzzle puzzle, ExactCoverMatrix<TPuzzle> matrix)
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
