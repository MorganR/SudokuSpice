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
        /// This drops any <see cref="Possibility"/>s that are no longer possible, and adds
        /// <see cref="Objective"/>s to enforce this constraint for the ones that are still
        /// possible.
        /// </remarks>
        /// <param name="puzzle">The puzzle being solved.</param>
        /// <param name="squareCoordinates">
        /// The coordinates that must contain unique values.
        /// </param>
        /// <param name="graph">The exact-cover graph for the current puzzle.</param>
        /// <returns>
        /// False if the puzzle violates uniquness for the given coordinates, else true.
        /// </returns>
        public static bool TryImplementUniquenessConstraintForSquares(
            IReadOnlyPuzzle puzzle,
            ReadOnlySpan<Coordinate> squareCoordinates,
            ExactCoverGraph graph)
        {
            Span<bool> isConstraintSatisfiedAtIndex =
                    stackalloc bool[graph.AllPossibleValues.Length];
            if (!TryCheckForSetValues(puzzle, graph, squareCoordinates, isConstraintSatisfiedAtIndex))
            {
                return false;
            }
            Possibility?[]?[] squares = new Possibility[squareCoordinates.Length][];
            for (int i = 0; i < squares.Length; i++)
            {
                squares[i] = graph.GetAllPossibilitiesAt(in squareCoordinates[i]);
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
                if (!TryAddObjectiveForPossibilityIndex(squares, possibilityIndex, graph, requiredCount: 1, objective: out _))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Tries to fill the <paramref name="isValueIndexPresentInSquares"/> span according to
        /// which value indices are already set in the given list of
        /// <paramref name="squareCoordinates"/>. Each index is true if that value is already set.
        ///
        /// Results are only complete if this returns true. If it returns false, no guarantees are
        /// made as to the state of <paramref name="isValueIndexPresentInSquares"/>.
        /// </summary>
        /// <param name="puzzle">The current puzzle being solved.</param>
        /// <param name="graph">The graph for the puzzle being solved.</param>
        /// <param name="squareCoordinates">The coordinates to check.</param>
        /// <param name="isValueIndexPresentInSquares">
        /// An array that will be updated to indicate which values are set.
        /// </param>
        /// <returns>False if a value is duplicated in the given coordinates.</returns>
        public static bool TryCheckForSetValues(
            IReadOnlyPuzzle puzzle,
            ExactCoverGraph graph,
            ReadOnlySpan<Coordinate> squareCoordinates,
            Span<bool> isValueIndexPresentInSquares)
        {
            isValueIndexPresentInSquares.Clear();
            foreach (Coordinate coordinate in squareCoordinates)
            {
                int? square = puzzle[in coordinate];
                if (square.HasValue)
                {
                    int valueIndex = graph.ValuesToIndices[square.Value];
                    if (isValueIndexPresentInSquares[valueIndex])
                    {
                        // Duplicate values are not allowed.
                        return false;
                    }
                    isValueIndexPresentInSquares[valueIndex] = true;
                }
            }
            return true;
        }

        /// <summary>
        /// Drops <see cref="Possibility"/>s with the given <paramref name="possibilityIndex"/>
        /// from the given set of <paramref name="squares"/>. Null squares and possible values are
        /// ignored.
        ///
        /// If this returns false, there is no guarantee as to the state of the given
        /// <paramref name="squares"/>. Some may have been dropped.
        /// </summary>
        /// <param name="squares">The squares to drop, if not null.</param>
        /// <param name="possibilityIndex">
        /// The index of the possibility within the squares.
        /// </param>
        /// <returns>
        /// True if all the <paramref name="squares"/> were dropped safely (eg. without
        /// resulting in any <see cref="Objective"/> that can no longer be satisfied).
        /// </returns>
        public static bool TryDropPossibilitiesAtIndex(
            ReadOnlySpan<Possibility?[]?> squares, int possibilityIndex)
        {
            for (int i = 0; i < squares.Length; i++)
            {
                Possibility?[]? square = squares[i];
                if (square is null)
                {
                    continue;
                }
                Possibility? possibleValue = square[possibilityIndex];
                if (possibleValue is null)
                {
                    continue;
                }
                if (!possibleValue.TryDrop())
                {
                    return false;
                }
                square[possibilityIndex] = null;
            }
            return true;
        }

        /// <summary>
        /// Add an <see cref="OptionalObjective"/> connecting all the
        /// <see cref="Possibility"/>s at the given <paramref name="possibilityIndex"/> on the
        /// given <paramref name="squares"/>. Skips null squares, null possibilities, and any
        /// possibilities in a known state (i.e. dropped or selected).
        /// </summary>
        /// <param name="squares">
        /// The squares to add <see cref="Possibility"/>s from.
        /// </param>
        /// <param name="possibilityIndex">
        /// The value index of the possible value within the squares.
        /// </param>
        /// <param name="graph">The graph for the current puzzle being solved.</param>
        /// <param name="requiredCount">
        /// The number of possibilities required to satisfy the new <see cref="OptionalObjective"/>.
        /// </param>
        /// <param name="objective">The new optional objective, if successful, else null.</param>
        /// <returns>
        /// False if the objective could not be added, for example because not enough
        /// <see cref="Possibility"/> objects were still possible to satisfy it, else true.
        /// </returns>
        public static bool TryAddOptionalObjectiveForPossibilityIndex(
            ReadOnlySpan<Possibility?[]?> squares, int possibilityIndex, ExactCoverGraph graph, int requiredCount, out OptionalObjective? objective)
        {
            var possibilities = new Possibility[squares.Length];
            int numPossibilities = _RetrieveUnknownPossibilities(squares, possibilityIndex, graph, possibilities);
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
        /// Add an <see cref="Objective"/> connecting all the <see cref="Possibility"/>s at the
        /// given <paramref name="possibilityIndex"/> on the given <paramref name="squares"/>.
        /// Skips null squares, null possibilities, and any possibilities in a known state (i.e.
        /// dropped or selected).
        /// </summary>
        /// <param name="squares">
        /// The squares to add <see cref="Possibility"/>s from.
        /// </param>
        /// <param name="possibilityIndex">
        /// The value index of the possible value within the squares.
        /// </param>
        /// <param name="graph">The graph for the current puzzle being solved.</param>
        /// <param name="requiredCount">
        /// The number of possibilities required to satisfy the new <see cref="Objective"/>.
        /// </param>
        /// <param name="objective">The new objective, if successful, else null.</param>
        /// <returns>
        /// False if the objective could not be added, for example because not enough
        /// <see cref="Possibility"/> objects were still possible to satisfy it, else true.
        /// </returns>
        public static bool TryAddObjectiveForPossibilityIndex(
            ReadOnlySpan<Possibility?[]?> squares, int possibilityIndex, ExactCoverGraph graph, int requiredCount, out Objective? objective)
        {
            var possibilities = new Possibility[squares.Length];
            int numPossibilities = _RetrieveUnknownPossibilities(squares, possibilityIndex, graph, possibilities);
            if (numPossibilities == 0)
            {
                objective = null;
                return false;
            }
            objective = Objective.CreateFullyConnected(
                graph,
                possibilities[0..numPossibilities],
                countToSatisfy: requiredCount);
            return true;
        }

        private static int _RetrieveUnknownPossibilities(
            ReadOnlySpan<Possibility?[]?> squares, int possibilityIndex, ExactCoverGraph graph,
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