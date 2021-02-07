using System;
using System.Diagnostics;

namespace SudokuSpice.ConstraintBased.Constraints
{
    /// <summary>
    /// Provides utilities for easily implementing constraints.
    /// </summary>
    public static class ConstraintUtil
    {
        /// <summary>
        /// Enforces uniqueness of the values at the given coordinates.
        /// </summary>
        /// <remarks>
        /// This drops any <see cref="PossibleSquareValue"/>s that are no longer possible, and
        /// adds <see cref="Requirement"/>s and links to enforce this constraint for the ones
        /// that are still possible.
        /// </remarks>
        /// <param name="puzzle">The puzzle being solved.</param>
        /// <param name="squareCoordinates">
        /// The coordinates that must contain unique values.
        /// </param>
        /// <param name="matrix">The exact cover matrix for the current puzzle.</param>
        /// <returns>
        /// False if the puzzle violates uniquness for the given coordinates, else true.
        /// </returns>
        public static bool TryImplementUniquenessConstraintForSquares(
            IReadOnlyPuzzle puzzle,
            ReadOnlySpan<Coordinate> squareCoordinates,
            ExactCoverMatrix matrix)
        {
            Span<bool> isConstraintSatisfiedAtIndex =
                    stackalloc bool[matrix.AllPossibleValues.Length];
            CheckForSetValues(puzzle, matrix, squareCoordinates, isConstraintSatisfiedAtIndex);
            Possibility?[]?[] squares = new Possibility[squareCoordinates.Length][];
            for (int i = 0; i < squares.Length; i++)
            {
                squares[i] = matrix.GetAllPossibilitiesAt(in squareCoordinates[i]);
            }
            for (int possibilityIndex = 0; possibilityIndex < isConstraintSatisfiedAtIndex.Length; possibilityIndex++)
            {
                if (isConstraintSatisfiedAtIndex[possibilityIndex])
                {
                    if (!TryDropPossibilitiesAtIndex(squares, possibilityIndex))
                    {
                        return false;
                    }
                    continue;
                }
                if (!TryAddObjectiveForPossibilityIndex(squares, possibilityIndex, matrix, requiredCount: 1, objective: out _))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Fills the <paramref name="isValueIndexPresentInSquares"/> span according to which
        /// value indices are already set in the given list of
        /// <paramref name="squareCoordinates"/>. Each index is true if that value is already set.
        /// </summary>
        /// <param name="puzzle">The current puzzle being solved.</param>
        /// <param name="matrix">The matrix for the puzzle being solved.</param>
        /// <param name="squareCoordinates">The coordinates to check.</param>
        /// <param name="isValueIndexPresentInSquares">
        /// An array that will be updated to indicate which values are set.
        /// </param>
        public static void CheckForSetValues(
            IReadOnlyPuzzle puzzle,
            ExactCoverMatrix matrix,
            ReadOnlySpan<Coordinate> squareCoordinates,
            Span<bool> isValueIndexPresentInSquares)
        {
            isValueIndexPresentInSquares.Clear();
            foreach (Coordinate coordinate in squareCoordinates)
            {
                int? square = puzzle[in coordinate];
                if (square.HasValue)
                {
                    isValueIndexPresentInSquares[matrix.ValuesToIndices[square.Value]] = true;
                }
            }
        }

        /// <summary>
        /// Drops <see cref="Possibility"/>s with the given <paramref name="valueIndex"/>
        /// from the given set of <paramref name="squares"/>. Null squares and possible values are
        /// ignored.
        ///
        /// If this returns false, there is no guarantee as to the state of the given
        /// <paramref name="squares"/>. Some may have been dropped.
        /// </summary>
        /// <param name="squares">The squares to drop, if not null.</param>
        /// <param name="valueIndex">
        /// The value index of the possible values within the squares.
        /// </param>
        /// <returns>
        /// True if all the <paramref name="squares"/> were dropped safely (eg. without
        /// resulting in an empty <see cref="Objective"/> without any possible square
        /// values, else false.
        /// </returns>
        public static bool TryDropPossibilitiesAtIndex(
            ReadOnlySpan<Possibility?[]?> squares, int valueIndex)
        {
            for (int i = 0; i < squares.Length; i++)
            {
                Possibility?[]? square = squares[i];
                if (square is null)
                {
                    continue;
                }
                Possibility? possibleValue = square[valueIndex];
                if (possibleValue is null)
                {
                    continue;
                }
                if (!possibleValue.TryDrop())
                {
                    return false;
                }
                square[valueIndex] = null;
            }
            return true;
        }

        /// <summary>
        /// Add an <see cref="IObjective"/> connecting all the
        /// <see cref="PossibleSquareValue"/>s at the given <paramref name="possibilityIndex"/> on the
        /// given <paramref name="squares"/>. Skips null squares, null possible values, and any
        /// possible values in a known state (i.e. dropped or selected).
        /// </summary>
        /// <param name="squares">
        /// The squares to add <see cref="PossibleSquareValue"/>s from.
        /// </param>
        /// <param name="possibilityIndex">
        /// The value index of the possible value within the squares.
        /// </param>
        /// <param name="matrix">The matrix for the current puzzle being solved.</param>
        /// <param name="requiredCount">
        /// The number of possible square values required to satisfy the new
        /// <see cref="Requirement"/>.
        /// </param>
        /// <param name="isOptional">
        /// Whether or not the new <see cref="Requirement"/> is optional.
        /// </param>
        /// <param name="requirement">The new requirement, if successful, else null.</param>
        /// <returns>
        /// False if the requirement could not be added, for example because none of the
        /// corresponding <see cref="PossibleSquareValue"/>s were still possible and the
        /// requirement would have been empty. Else returns true.
        /// </returns>
        public static bool TryAddOptionalObjectiveForPossibilityIndex(
            ReadOnlySpan<Possibility?[]?> squares, int possibilityIndex, ExactCoverMatrix matrix, int requiredCount, out OptionalObjective? objective)
        {
            var possibilities = new Possibility[squares.Length];
            int numPossibilities = _RetrieveUnknownPossibilities(squares, possibilityIndex, matrix, possibilities);
            if (numPossibilities == 0)
            {
                objective = null;
                return false;
            }
            objective = OptionalObjective.CreateWithPossibilities(
                possibilities[0..numPossibilities],
                countToSatisfy: requiredCount);
            return true;
        }

        /// <summary>
        /// Add an <see cref="IObjective"/> connecting all the
        /// <see cref="PossibleSquareValue"/>s at the given <paramref name="possibilityIndex"/> on the
        /// given <paramref name="squares"/>. Skips null squares, null possible values, and any
        /// possible values in a known state (i.e. dropped or selected).
        /// </summary>
        /// <param name="squares">
        /// The squares to add <see cref="PossibleSquareValue"/>s from.
        /// </param>
        /// <param name="possibilityIndex">
        /// The value index of the possible value within the squares.
        /// </param>
        /// <param name="matrix">The matrix for the current puzzle being solved.</param>
        /// <param name="requiredCount">
        /// The number of possible square values required to satisfy the new
        /// <see cref="Requirement"/>.
        /// </param>
        /// <param name="isOptional">
        /// Whether or not the new <see cref="Requirement"/> is optional.
        /// </param>
        /// <param name="requirement">The new requirement, if successful, else null.</param>
        /// <returns>
        /// False if the requirement could not be added, for example because none of the
        /// corresponding <see cref="PossibleSquareValue"/>s were still possible and the
        /// requirement would have been empty. Else returns true.
        /// </returns>
        public static bool TryAddObjectiveForPossibilityIndex(
            ReadOnlySpan<Possibility?[]?> squares, int possibilityIndex, ExactCoverMatrix matrix, int requiredCount, out Objective? objective)
        {
            var possibilities = new Possibility[squares.Length];
            int numPossibilities = _RetrieveUnknownPossibilities(squares, possibilityIndex, matrix, possibilities);
            if (numPossibilities == 0)
            {
                objective = null;
                return false;
            }
            objective = Objective.CreateFullyConnected(
                matrix,
                possibilities[0..numPossibilities],
                countToSatisfy: requiredCount);
            return true;
        }

        private static int _RetrieveUnknownPossibilities(
            ReadOnlySpan<Possibility?[]?> squares, int possibilityIndex, ExactCoverMatrix matrix,
            Span<Possibility?> unknownPossibilities)
        {
            Debug.Assert(unknownPossibilities.Length == squares.Length);
            int numPossibilities = 0;
            for (int i = 0; i < squares.Length; i++)
            {
                Possibility?[]? square = squares[i];
                if (square is null)
                {
                    continue;
                }
                Possibility? possibility = square[possibilityIndex];
                if (possibility is null
                    || possibility.State != NodeState.UNKNOWN)
                {
                    continue;
                }
                unknownPossibilities[numPossibilities++] = possibility;
            }
            return numPossibilities;
        }
    }
}