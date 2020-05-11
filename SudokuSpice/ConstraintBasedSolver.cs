using SudokuSpice.Constraints;
using SudokuSpice.Data;
using System;
using System.Collections.Generic;

namespace SudokuSpice
{
    /// <summary>
    /// Solves <see cref="IPuzzle"/>s using an <see cref="ExactCoverMatrix"/>.
    /// </summary>
    /// <remarks>
    /// This class is thread-safe.
    /// </remarks>
    public class ConstraintBasedSolver
    {
        private readonly IReadOnlyList<IConstraint> _constraints;

        /// <summary>
        /// Creates a solver that can solve <see cref="IPuzzle"/>s using the given
        /// <see cref="IConstraint"/>s. The same solver can be reused for multiple puzzles.
        /// </summary>
        /// <param name="constraints">The constraints to satisfy when solving puzzles.</param>
        public ConstraintBasedSolver(IReadOnlyList<IConstraint> constraints)
        {
            _constraints = constraints;
        }

        /// <summary>
        /// Solves the given <paramref name="puzzle"/>. This modifies the puzzle's data.
        /// </summary>
        /// <remarks>
        /// It is safe to call this method from different threads on the same solver object,
        /// although note that the given <paramref name="puzzle"/> is modified.
        /// </remarks>
        /// <param name="puzzle">The puzzle to solve.</param>
        /// <param name="possibleValues">
        /// The possible values for this puzzle. These must be unique.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown if the puzzle cannot be solved with this solver's constraints, or if the
        /// possible values are not unique.
        /// </exception>
        public void Solve(IPuzzle puzzle)
        {
            if (!_AreValuesUnique(puzzle.AllPossibleValues)) {
                throw new ArgumentException(
                    $"{nameof(puzzle.AllPossibleValues)} must all be unique. Received values: {puzzle.AllPossibleValues.ToString()}.");
            }
            var matrix = new ExactCoverMatrix(puzzle);
            foreach (var constraint in _constraints)
            {
                constraint.Constrain(puzzle, matrix);
            }
            if (!_TrySolve(puzzle, new ConstraintBasedTracker(puzzle, matrix)))
            {
                throw new ArgumentException($"Failed to solve the given puzzle.");
            }
        }

        private static bool _AreValuesUnique(ReadOnlySpan<int> values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                for (int j = i - 1; j >= 0; j--)
                {
                    if (values[j] == values[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool _TrySolve(IPuzzle puzzle, ConstraintBasedTracker tracker)
        {
            if (puzzle.NumEmptySquares == 0)
            {
                return true;
            }
            (var c, var possibleValues) = tracker.GetBestGuess();
            foreach (var possibleValue in possibleValues)
            {
                if (tracker.TrySet(in c, possibleValue))
                {
                    if (_TrySolve(puzzle, tracker))
                    {
                        return true;
                    }
                    tracker.UnsetLast();
                }
            }
            return false;
        }
    }
}
