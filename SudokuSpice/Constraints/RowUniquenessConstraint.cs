using SudokuSpice.Data;
using System;

namespace SudokuSpice.Constraints
{
    public class RowUniquenessConstraint : IConstraint
    {
        public void Constrain(IReadOnlyPuzzle puzzle, ExactCoverMatrix matrix)
        {
            for (int row = 0; row < puzzle.Size; row++)
            {
                Span<bool> isConstraintSatisfiedAtIndex = stackalloc bool[matrix.AllPossibleValues.Length];
                isConstraintSatisfiedAtIndex.Fill(false);
                for (int column = 0; column < puzzle.Size; column++)
                {
                    var value = puzzle[row, column];
                    if (value.HasValue)
                    {
                        isConstraintSatisfiedAtIndex[matrix.ValuesToIndices[value.Value]] = true;
                    }
                }
                var rowSquares = matrix.GetSquaresOnRow(row);
                for (int valueIndex = 0; valueIndex < isConstraintSatisfiedAtIndex.Length; valueIndex++)
                {
                    if (isConstraintSatisfiedAtIndex[valueIndex])
                    {
                        _DropPossibleValuesForValueIndex(rowSquares, valueIndex, matrix);
                        continue;
                    }
                    _AddConstraintHeadersForValueIndex(rowSquares, valueIndex, matrix);
                }
            }
        }

        private static void _DropPossibleValuesForValueIndex(ReadOnlySpan<Square?> rowSquares, int valueIndex, ExactCoverMatrix matrix)
        {
            for (int column = 0; column < rowSquares.Length; column++)
            {
                var square = rowSquares[column];
                if (square is null)
                {
                    continue;
                }
                var possibleValue = square.AllPossibleValues[valueIndex];
                if (possibleValue.State != PossibleSquareState.DROPPED && !possibleValue.TryDrop())
                {
                    throw new ArgumentException($"Puzzle violated {nameof(RowUniquenessConstraint)} for value {matrix.AllPossibleValues[valueIndex]} on row {square.Coordinate.Row}.");
                }
            }
        }

        private static void _AddConstraintHeadersForValueIndex(ReadOnlySpan<Square?> rowSquares, int valueIndex, ExactCoverMatrix matrix)
        {
            var possibleSquares = new PossibleSquareValue[rowSquares.Length];
            int numPossibleSquares = 0;
            for (int column = 0; column < rowSquares.Length; column++)
            {
                var square = rowSquares[column];
                if (square is null || square.AllPossibleValues[valueIndex].State != PossibleSquareState.UNKNOWN)
                {
                    continue;
                }
                possibleSquares[numPossibleSquares++] = square.AllPossibleValues[valueIndex];
            }
            ConstraintHeader.CreateConnectedHeader(matrix, new ReadOnlySpan<PossibleSquareValue>(possibleSquares, 0, numPossibleSquares));
        }
    }
}
