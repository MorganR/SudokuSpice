using System;

namespace SudokuSpice.ConstraintBased.Constraints
{
    /// <summary>
    /// Enforces the constraint that all values in a box must be unique.
    /// </summary>
    public class BoxUniquenessConstraint : IConstraint
    {
        /// <inheritdoc/>
        public bool TryConstrain(IReadOnlyPuzzle puzzle, ExactCoverGraph matrix)
        {
            if (!Boxes.TryIntSquareRoot(puzzle.Size, out int boxSize))
            {
                return false;
            }
            for (int box = 0; box < puzzle.Size; box++)
            {
                if (!_TryAppendRequirementsInBox(box, boxSize, puzzle, matrix))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool _TryAppendRequirementsInBox(
            int box, int boxSize, IReadOnlyPuzzle puzzle, ExactCoverGraph matrix)
        {
            Coordinate startCoord = Boxes.GetStartingBoxCoordinate(box, boxSize);
            var endCoord = new Coordinate(
                startCoord.Row + boxSize, startCoord.Column + boxSize);
            Span<Coordinate> boxCoordinates = stackalloc Coordinate[puzzle.Size];
            int i = 0;
            for (int row = startCoord.Row; row < endCoord.Row; row++)
            {
                for (int col = startCoord.Column; col < endCoord.Column; col++)
                {
                    boxCoordinates[i++] = new Coordinate(row, col);
                }
            }
            return ConstraintUtil.TryImplementUniquenessConstraintForSquares(puzzle, boxCoordinates, matrix);
        }
    }
}