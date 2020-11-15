using System;

namespace SudokuSpice.ConstraintBased.Constraints
{
    /// <summary>
    /// Enforces the constraint that all values in a row must be unique.
    /// </summary>
    public class RowUniquenessConstraint<TPuzzle> : IConstraint<TPuzzle> where TPuzzle : IReadOnlyPuzzle 
    {
        /// <inheritdoc/>
        public void Constrain(TPuzzle puzzle, ExactCoverMatrix<TPuzzle> matrix)
        {
            Span<bool> isConstraintSatisfiedAtIndex =
                   stackalloc bool[matrix.AllPossibleValues.Length];
            for (int row = 0; row < puzzle.Size; row++)
            {
                var rowSquares = matrix.GetSquaresOnRow(row);
                isConstraintSatisfiedAtIndex.Clear();
                for (int col = 0; col < puzzle.Size; col++)
                {
                    var puzzleValue = puzzle[row, col];
                    if (puzzleValue.HasValue)
                    {
                        isConstraintSatisfiedAtIndex[matrix.ValuesToIndices[puzzleValue.Value]] = true;
                    }
                }
                for (int valueIndex = 0; valueIndex < isConstraintSatisfiedAtIndex.Length; valueIndex++)
                {
                    if (isConstraintSatisfiedAtIndex[valueIndex])
                    {
                        ConstraintUtil.DropPossibleSquaresForValueIndex(rowSquares, valueIndex, matrix);
                        continue;
                    }
                    ConstraintUtil.AddConstraintHeadersForValueIndex(rowSquares, valueIndex, matrix);
                }
            }
        }
    }
}
