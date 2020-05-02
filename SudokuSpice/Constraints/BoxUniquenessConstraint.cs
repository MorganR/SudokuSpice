using SudokuSpice.Data;
using System;

namespace SudokuSpice.Constraints
{
    public class BoxUniquenessConstraint : IConstraint
    {
        public void Constrain(IReadOnlyPuzzle puzzle, ExactCoverMatrix matrix)
        {
            var boxPuzzle = puzzle as IReadOnlyBoxPuzzle;
            if (boxPuzzle is null) {
                throw new ArgumentException(
                    $"puzzle must be of type {nameof(IReadOnlyBoxPuzzle)}. Received type: {puzzle.GetType().Name}.");
            }
            for (int box = 0; box < boxPuzzle.Size; box++)
            {
                AppendConstraintHeadersInBox(box, boxPuzzle, matrix);
            }
        }

        private static void AppendConstraintHeadersInBox(
            int box, IReadOnlyBoxPuzzle puzzle, ExactCoverMatrix matrix)
        {
            var startCoord = puzzle.GetStartingBoxCoordinate(box);
            var endCoord = new Coordinate(startCoord.Row + puzzle.BoxSize, startCoord.Column + puzzle.BoxSize);
            for (int valueIdx = 0; valueIdx < matrix.AllPossibleValues.Count; valueIdx++)
            {
                var possibleSquares = new PossibleSquareValue[puzzle.Size];
                int i = 0;
                for (int row = startCoord.Row; row < endCoord.Row; row++)
                {
                    for (int col = startCoord.Column; col < endCoord.Column; col++)
                    {
                        possibleSquares[i++] = matrix.GetSquare(new Coordinate(row, col)).AllPossibleValues[valueIdx];
                    }
                }
                matrix.AddConstraintHeader(ConstraintHeader.CreateConnectedHeader(possibleSquares));
            }

        }
    }
}
