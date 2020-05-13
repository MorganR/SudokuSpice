using SudokuSpice.Data;

namespace SudokuSpice.Constraints
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

        private void _ConstrainForwardDiagonal(IReadOnlyPuzzle puzzle, ExactCoverMatrix matrix)
        {
            var coordinates = new Coordinate[puzzle.Size];
            for (int row = 0, col = puzzle.Size - 1;  row < puzzle.Size; row++, col--)
            {
                coordinates[row] = new Coordinate(row, col);
            }
            ConstraintUtil.ImplementUniquenessConstraintForSquares(puzzle, coordinates, matrix);
        }

        private void _ConstrainBackwardDiagonal(IReadOnlyPuzzle puzzle, ExactCoverMatrix matrix)
        {
            var oordinates = new Coordinate[puzzle.Size];
            for (int row = 0, col = 0; row < puzzle.Size; row++, col++)
            {
                oordinates[row] = new Coordinate(row, col);
            }
            ConstraintUtil.ImplementUniquenessConstraintForSquares(puzzle, oordinates, matrix);
        }
    }
}
