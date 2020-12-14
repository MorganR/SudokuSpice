using System;

namespace SudokuSpice.ConstraintBased.Constraints
{
    /// <summary>
    /// Enforces the constraint that all values in a box must be unique.
    /// </summary>
    public class BoxUniquenessConstraint : IConstraint
    {
        /// <inheritdoc/>
        public void Constrain(IReadOnlyPuzzle puzzle, ExactCoverMatrix matrix)
        {
            for (int box = 0; box < puzzle.Size; box++)
            {
                _AppendConstraintHeadersInBox(box, (IReadOnlyBoxPuzzle)puzzle, matrix);
            }
        }

        private static void _AppendConstraintHeadersInBox(
            int box, IReadOnlyBoxPuzzle puzzle, ExactCoverMatrix matrix)
        {
            Coordinate startCoord = puzzle.GetStartingBoxCoordinate(box);
            var endCoord = new Coordinate(
                startCoord.Row + puzzle.BoxSize, startCoord.Column + puzzle.BoxSize);
            Span<Coordinate> boxCoordinates = stackalloc Coordinate[puzzle.Size];
            int i = 0;
            for (int row = startCoord.Row; row < endCoord.Row; row++)
            {
                for (int col = startCoord.Column; col < endCoord.Column; col++)
                {
                    boxCoordinates[i++] = new Coordinate(row, col);
                }
            }
            ConstraintUtil.ImplementUniquenessConstraintForSquares(puzzle, boxCoordinates, matrix);
        }
    }
}