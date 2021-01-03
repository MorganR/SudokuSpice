using System;

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
        /// adds <see cref="ConstraintHeader"/>s and links to enforce this constraint for the ones
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
            var squares = new Square?[squareCoordinates.Length];
            for (int i = 0; i < squares.Length; i++)
            {
                squares[i] = matrix.GetSquare(in squareCoordinates[i]);
            }
            for (int valueIndex = 0; valueIndex < isConstraintSatisfiedAtIndex.Length; valueIndex++)
            {
                if (isConstraintSatisfiedAtIndex[valueIndex])
                {
                    if (!TryDropPossibleSquaresForValueIndex(squares, valueIndex))
                    {
                        return false;
                    }
                    continue;
                }
                if (!TryAddConstraintHeadersForValueIndex(squares, valueIndex, matrix))
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
        /// Drops <see cref="PossibleSquareValue"/>s with the given <paramref name="valueIndex"/>
        /// from the given set of <paramref name="squares"/>. Null squares and possible values are
        /// ignored.
        /// </summary>
        /// <param name="squares">The squares to drop, if not null.</param>
        /// <param name="valueIndex">
        /// The value index of the possible values within the squares.
        /// </param>
        /// <returns>
        /// True if all the <see cref="PossibleSquareValue"/>s were dropped safely (eg. without
        /// resulting in an empty <see cref="ConstraintHeader"/> without any possible square
        /// values, or a <see cref="Square"/> with no more possible values), else false.
        /// </returns>
        public static bool TryDropPossibleSquaresForValueIndex(
            ReadOnlySpan<Square?> squares, int valueIndex)
        {
            for (int i = 0; i < squares.Length; i++)
            {
                Square? square = squares[i];
                if (square is null)
                {
                    continue;
                }
                PossibleSquareValue? possibleValue = square.GetPossibleValue(valueIndex);
                if (possibleValue is null)
                {
                    continue;
                }
                if (possibleValue.State != PossibleValueState.DROPPED && !possibleValue.TryDrop())
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Add a <see cref="ConstraintHeader"/> connecting all the
        /// <see cref="PossibleSquareValue"/>s at the given <paramref name="valueIndex"/> on the
        /// given <paramref name="squares"/>. Skips null squares, null possible values, and any
        /// possible values in a known state (i.e. dropped or selected).
        /// </summary>
        /// <param name="squares">
        /// The squares to add <see cref="PossibleSquareValue"/>s from.
        /// </param>
        /// <param name="valueIndex">
        /// The value index of the possible value within the squares.
        /// </param>
        /// <param name="matrix">The matrix for the current puzzle being solved.</param>
        /// <returns>
        /// False if the header could not be added, for example because none of the corresponding
        /// <see cref="PossibleSquareValue"/>s were still possible and the constraint would have
        /// been empty. Else returns true.
        /// </returns>
        public static bool TryAddConstraintHeadersForValueIndex(
            ReadOnlySpan<Square?> squares, int valueIndex, ExactCoverMatrix matrix)
        {
            var possibleSquareValues = new PossibleSquareValue[squares.Length];
            int numPossibleSquares = 0;
            for (int i = 0; i < squares.Length; i++)
            {
                Square? square = squares[i];
                if (square is null)
                {
                    continue;
                }
                PossibleSquareValue? possibleValue = square.GetPossibleValue(valueIndex);
                if (possibleValue is null
                    || possibleValue.State != PossibleValueState.UNKNOWN)
                {
                    continue;
                }
                possibleSquareValues[numPossibleSquares++] = possibleValue;
            }
            if (numPossibleSquares == 0)
            {
                return false;
            }
            ConstraintHeader.CreateConnectedHeader(
                matrix,
                new ReadOnlySpan<PossibleSquareValue>(possibleSquareValues, 0, numPossibleSquares));
            return true;
        }
    }
}