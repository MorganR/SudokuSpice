using System;

namespace SudokuSpice.ConstraintBased.Constraints
{
    /// <summary>
    /// Enforces the constraint that all values in a row must be unique.
    /// </summary>
    // TODO: Move these to public tests
    public class RowUniquenessConstraint : IConstraint
    {
        /// <inheritdoc/>
        public bool TryConstrain(IReadOnlyPuzzle puzzle, ExactCoverMatrix matrix)
        {
            Span<bool> isConstraintSatisfiedAtIndex =
                   stackalloc bool[matrix.AllPossibleValues.Length];
            for (int row = 0; row < puzzle.Size; row++)
            {
                ReadOnlySpan<Possibility?[]?> rowSquares = matrix.GetPossibilitiesOnRow(row);
                isConstraintSatisfiedAtIndex.Clear();
                for (int col = 0; col < puzzle.Size; col++)
                {
                    int? puzzleValue = puzzle[row, col];
                    if (puzzleValue.HasValue)
                    {
                        isConstraintSatisfiedAtIndex[matrix.ValuesToIndices[puzzleValue.Value]] = true;
                    }
                }
                for (int possibilityIndex = 0; possibilityIndex < isConstraintSatisfiedAtIndex.Length; possibilityIndex++)
                {
                    if (isConstraintSatisfiedAtIndex[possibilityIndex])
                    {
                        if (!ConstraintUtil.TryDropPossibilitiesAtIndex(rowSquares, possibilityIndex))
                        {
                            return false;
                        }
                        continue;
                    }
                    if (!ConstraintUtil.TryAddObjectiveForPossibilityIndex(
                        rowSquares, possibilityIndex, matrix, requiredCount: 1, objective: out _))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}