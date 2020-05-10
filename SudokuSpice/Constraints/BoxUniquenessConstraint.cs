using SudokuSpice.Data;
using System;

namespace SudokuSpice.Constraints
{
    /// <summary>
    /// Enforces the constraint that all values in a box must be unique.
    /// </summary>
    public class BoxUniquenessConstraint : IConstraint
    {
        /// <inheritdoc/>
        public void Constrain(IReadOnlyPuzzle puzzle, ExactCoverMatrix matrix)
        {
            var boxPuzzle = puzzle as IReadOnlyBoxPuzzle;
            if (boxPuzzle is null) {
                throw new ArgumentException(
                    $"puzzle must be of type {nameof(IReadOnlyBoxPuzzle)}. Received type: {puzzle.GetType().Name}.");
            }
            for (int box = 0; box < boxPuzzle.Size; box++)
            {
                _AppendConstraintHeadersInBox(box, boxPuzzle, matrix);
            }
        }

        private static void _AppendConstraintHeadersInBox(
            int box, IReadOnlyBoxPuzzle puzzle, ExactCoverMatrix matrix)
        {
            var startCoord = puzzle.GetStartingBoxCoordinate(box);
            var endCoord = new Coordinate(
                startCoord.Row + puzzle.BoxSize, startCoord.Column + puzzle.BoxSize);
            Span<bool> isConstraintSatisfiedAtIndex =
                stackalloc bool[matrix.AllPossibleValues.Length];
            isConstraintSatisfiedAtIndex.Fill(false);
            for (int row = startCoord.Row; row < endCoord.Row; row++)
            {
                for (int col = startCoord.Column; col < endCoord.Column; col++)
                {
                    var value = puzzle[row, col];
                    if (value.HasValue)
                    {
                        isConstraintSatisfiedAtIndex[matrix.ValuesToIndices[value.Value]] = true;
                    }
                }
            }
            for (int valueIndex = 0; valueIndex < isConstraintSatisfiedAtIndex.Length; valueIndex++)
            {
                if (isConstraintSatisfiedAtIndex[valueIndex])
                {
                    _DropPossibleValuesForValueIndex(
                        in startCoord, in endCoord, valueIndex, matrix);
                    continue;
                }
                _AddCoordinateHeadersForValueIndex(
                    in startCoord, in endCoord, valueIndex, matrix);
            }
        }

        private static void _DropPossibleValuesForValueIndex(
            in Coordinate startCoord,
            in Coordinate endCoord,
            int valueIndex,
            ExactCoverMatrix matrix)
        {
            for (int row = startCoord.Row; row < endCoord.Row; row++)
            {
                for (int col = startCoord.Column; col < endCoord.Column; col++)
                {
                    var square = matrix.GetSquare(new Coordinate(row, col));
                    if (square is null)
                    {
                        continue;
                    }
                    var possibleValue = square.AllPossibleValues[valueIndex];
                    if (possibleValue.State != PossibleSquareState.DROPPED
                        && !possibleValue.TryDrop())
                    {
                        throw new ArgumentException(
                            $"Puzzle violated {nameof(BoxUniquenessConstraint)} for value {matrix.AllPossibleValues[valueIndex]} at ({row}, {col}).");
                    }
                }
            }
        }

        private static void _AddCoordinateHeadersForValueIndex(
            in Coordinate startCoord,
            in Coordinate endCoord,
            int valueIndex,
            ExactCoverMatrix matrix)
        {
            var possibleSquares = new PossibleSquareValue[matrix.AllPossibleValues.Length];
            int numPossibleSquares = 0;
            for (int row = startCoord.Row; row < endCoord.Row; row++)
            {
                for (int col = startCoord.Column; col < endCoord.Column; col++)
                {
                    var square = matrix.GetSquare(new Coordinate(row, col));
                    if (square is null
                        || square.AllPossibleValues[valueIndex].State != PossibleSquareState.UNKNOWN)
                    {
                        continue;
                    }
                    possibleSquares[numPossibleSquares++] = square.AllPossibleValues[valueIndex];
                }
            }
            ConstraintHeader.CreateConnectedHeader(
                matrix,
                new ReadOnlySpan<PossibleSquareValue>(possibleSquares, 0, numPossibleSquares));
        }
    }
}
