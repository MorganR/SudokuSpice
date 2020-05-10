using SudokuSpice.Data;
using System;
using System.Collections.Generic;

namespace SudokuSpice.Constraints
{
    /// <summary>
    /// Enforces the constraint that all values in a column must be unique.
    /// </summary>
    public class ColumnUniquenessConstraint : IConstraint
    {
        /// <inheritdoc/>
        public void Constrain(IReadOnlyPuzzle puzzle, ExactCoverMatrix matrix)
        {
            for (int column = 0; column < puzzle.Size; column++)
            {
                Span<bool> isConstraintSatisfiedAtIndex =
                    stackalloc bool[matrix.AllPossibleValues.Length];
                isConstraintSatisfiedAtIndex.Fill(false);
                for (int row = 0; row < puzzle.Size; row++)
                {
                    var value = puzzle[row, column];
                    if (value.HasValue)
                    {
                        isConstraintSatisfiedAtIndex[matrix.ValuesToIndices[value.Value]] = true;
                    }
                }
                var columnSquares = matrix.GetSquaresOnColumn(column);
                for (int valueIndex = 0; valueIndex < isConstraintSatisfiedAtIndex.Length; valueIndex++)
                {
                    if (isConstraintSatisfiedAtIndex[valueIndex])
                    {
                        _DropPossibleSquaresForValueIndex(columnSquares, valueIndex, matrix);
                        continue;
                    }
                    _AddConstraintHeadersForValueIndex(columnSquares, valueIndex, matrix);
                }
            }
        }

        private static void _DropPossibleSquaresForValueIndex(
            IReadOnlyList<Square?> columnSquares, int valueIndex, ExactCoverMatrix matrix)
        {
            for (int row = 0; row < columnSquares.Count; row++)
            {
                var square = columnSquares[row];
                if (square is null)
                {
                    continue;
                }
                var possibleValue = square.AllPossibleValues[valueIndex];
                if (possibleValue.State != PossibleSquareState.DROPPED && !possibleValue.TryDrop())
                {
                    throw new ArgumentException(
                        $"Puzzle violated {nameof(ColumnUniquenessConstraint)} for value {matrix.AllPossibleValues[valueIndex]} on column {square.Coordinate.Column}.");
                }
            }
        }

        private static void _AddConstraintHeadersForValueIndex(
            IReadOnlyList<Square?> columnSquares, int valueIndex, ExactCoverMatrix matrix)
        {
            var possibleSquares = new PossibleSquareValue[columnSquares.Count];
            int numPossibleSquares = 0;
            for (int row = 0; row < columnSquares.Count; row++)
            {
                var square = columnSquares[row];
                if (square is null || square.AllPossibleValues[valueIndex].State != PossibleSquareState.UNKNOWN)
                {
                    continue;
                }
                possibleSquares[numPossibleSquares++] = square.AllPossibleValues[valueIndex];
            }
            ConstraintHeader.CreateConnectedHeader(
                matrix, new ReadOnlySpan<PossibleSquareValue>(possibleSquares, 0, numPossibleSquares));
        }
    }
}
