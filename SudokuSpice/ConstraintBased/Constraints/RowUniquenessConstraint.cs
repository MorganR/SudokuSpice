using System;

namespace SudokuSpice.ConstraintBased.Constraints
{
    /// <summary>
    /// Enforces the constraint that all values in a row must be unique.
    /// </summary>
    public class RowUniquenessConstraint : IConstraint
    {
        /// <inheritdoc/>
        public bool TryConstrain(IReadOnlyPuzzle puzzle, ExactCoverGraph graph)
        {
            Span<bool> isConstraintSatisfiedAtIndex =
                   stackalloc bool[graph.AllPossibleValues.Length];
            for (int row = 0; row < puzzle.Size; row++)
            {
                ReadOnlySpan<Possibility?[]?> rowSquares = graph.GetPossibilitiesOnRow(row);
                isConstraintSatisfiedAtIndex.Clear();
                for (int col = 0; col < puzzle.Size; col++)
                {
                    int? puzzleValue = puzzle[row, col];
                    if (puzzleValue.HasValue)
                    {
                        int valueIndex = graph.ValuesToIndices[puzzleValue.Value];
                        if (isConstraintSatisfiedAtIndex[valueIndex])
                        {
                            return false;
                        }
                        isConstraintSatisfiedAtIndex[valueIndex] = true;
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
                        rowSquares, possibilityIndex, graph, requiredCount: 1, objective: out _))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}