using SudokuSpice.Constraints;
using SudokuSpice.Data;
using System;
using System.Collections.Generic;

namespace SudokuSpice
{
    public class ConstraintBasedSolver
    {
        private readonly IReadOnlyList<IConstraint> _constraints;

        public ConstraintBasedSolver(IReadOnlyList<IConstraint> constraints)
        {
            _constraints = constraints;
        }

        public void Solve(IPuzzle puzzle, int[] possibleValues)
        {
            var matrix = new ExactCoverMatrix(puzzle, (int[])possibleValues.Clone());
            foreach (var constraint in _constraints)
            {
                constraint.Constrain(puzzle, matrix);
            }
            if (!_TrySolve(puzzle, matrix, new ConstraintBasedTracker(puzzle, matrix)))
            {
                throw new ArgumentException($"Failed to solve the given puzzle.");
            }
        }

        private static bool _TrySolve(IPuzzle puzzle, ExactCoverMatrix matrix, ConstraintBasedTracker tracker)
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
                    if (_TrySolve(puzzle, matrix, tracker))
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
