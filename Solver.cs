using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace Sudoku
{
    public class Solver
    {
        private readonly ISquareTracker _tracker;

        public Solver(ISquareTracker tracker)
        {
            _tracker = tracker;
        }

        public void Solve()
        {
            if (!_TrySolve())
            {
                throw new ApplicationException($"Failed to solve the puzzle with {_tracker.GetNumEmptySquares()} empty squares remaining.");
            }
        }

        private bool _TrySolve()
        {
            if (_tracker.GetNumEmptySquares() == 0)
            {
                return true;
            }
            var idx = _tracker.GetBestIndexToGuess();
            foreach (var possibleValue in _tracker.GetPossibleValues(idx))
            {
                if (_tracker.TrySet(idx, possibleValue))
                {
                    if (_TrySolve())
                    {
                        return true;
                    }
                    _tracker.Unset(idx);
                }
            }
            return false;
        }
    }
}
