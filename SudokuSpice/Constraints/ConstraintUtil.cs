using SudokuSpice.Data;
using System;

namespace SudokuSpice.Constraints
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
        /// <exception cref="ArgumentException">
        /// Thrown if the puzzle violates uniquness for the given coordinates.
        /// </exception>
        public static void ImplementUniquenessConstraintForSquares<TPuzzle>(
            TPuzzle puzzle,
            ReadOnlySpan<Coordinate> squareCoordinates,
            ExactCoverMatrix<TPuzzle> matrix) where TPuzzle : IReadOnlyPuzzle
        {
            Span<bool> isConstraintSatisfiedAtIndex =
                    stackalloc bool[matrix.AllPossibleValues.Length];
            CheckForSetValues(puzzle, matrix, squareCoordinates, isConstraintSatisfiedAtIndex);
            var squares = new Square<TPuzzle>?[squareCoordinates.Length];
            for (int i = 0; i < squares.Length; i++)
            {
                squares[i] = matrix.GetSquare(in squareCoordinates[i]);
            }
            for (int valueIndex = 0; valueIndex < isConstraintSatisfiedAtIndex.Length; valueIndex++)
            {
                if (isConstraintSatisfiedAtIndex[valueIndex])
                {
                    DropPossibleSquaresForValueIndex(squares, valueIndex, matrix);
                    continue;
                }
                AddConstraintHeadersForValueIndex(squares, valueIndex, matrix);
            }
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
        public static void CheckForSetValues<TPuzzle>(
            TPuzzle puzzle,
            ExactCoverMatrix<TPuzzle> matrix,
            ReadOnlySpan<Coordinate> squareCoordinates,
            Span<bool> isValueIndexPresentInSquares) where TPuzzle : IReadOnlyPuzzle
        {
            isValueIndexPresentInSquares.Fill(false);
            for (int i = 0; i < squareCoordinates.Length; i++)
            {
                var puzzleValue = puzzle[squareCoordinates[i]];
                if (puzzleValue.HasValue)
                {
                    isValueIndexPresentInSquares[matrix.ValuesToIndices[puzzleValue.Value]] = true;
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
        /// <param name="matrix">The matrix for the puzzle currently being solved.</param>
        /// <exception cref="ArgumentException">
        /// Thrown if the puzzle violates uniquness for the given coordinates.
        /// </exception>
        public static void DropPossibleSquaresForValueIndex<TPuzzle>(
            ReadOnlySpan<Square<TPuzzle>?> squares, int valueIndex, ExactCoverMatrix<TPuzzle> matrix)
            where TPuzzle : IReadOnlyPuzzle
        {
            for (int i = 0; i < squares.Length; i++)
            {
                var square = squares[i];
                if (square is null)
                {
                    continue;
                }
                var possibleValue = square.GetPossibleValue(valueIndex);
                if (possibleValue is null)
                {
                    continue;
                }
                if (possibleValue.State != PossibleSquareState.DROPPED && !possibleValue.TryDrop())
                {
                    throw new ArgumentException(
                        $"Puzzle violated constraints for value {matrix.AllPossibleValues[valueIndex]} at square {square.Coordinate}.");
                }
            }
        }

        /// <summary>
        /// Add a <see cref="ConstraintHeader"/> connecting all the
        /// <see cref="PossibleSquareValue"/>s at the given <paramref name="valueIndex"/> on the
        /// given <paramref name="squares"/>. Skips null squares, null possible values, and any
        /// possible values in a known state (i.e. dropped or selected).
        /// </summary>
        /// <param name="squares">The squares to add possible square values from.</param>
        /// <param name="valueIndex">
        /// The value index of the possible value within the squares.
        /// </param>
        /// <param name="matrix">The matrix for the current puzzle being solved.</param>
        public static void AddConstraintHeadersForValueIndex<TPuzzle>(
            ReadOnlySpan<Square<TPuzzle>?> squares, int valueIndex, ExactCoverMatrix<TPuzzle> matrix)
            where TPuzzle : IReadOnlyPuzzle
        {
            var possibleSquares = new PossibleSquareValue<TPuzzle>[squares.Length];
            int numPossibleSquares = 0;
            for (int i = 0; i < squares.Length; i++)
            {
                var square = squares[i];
                if (square is null)
                {
                    continue;
                }
                var possibleSquare = square.GetPossibleValue(valueIndex);
                if (possibleSquare is null
                    || possibleSquare.State != PossibleSquareState.UNKNOWN)
                {
                    continue;
                }
                possibleSquares[numPossibleSquares++] = possibleSquare;
            }
            ConstraintHeader<TPuzzle>.CreateConnectedHeader(
                matrix,
                new ReadOnlySpan<PossibleSquareValue<TPuzzle>>(possibleSquares, 0, numPossibleSquares));
        }
    }
}
