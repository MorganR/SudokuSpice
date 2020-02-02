using System;

namespace SudokuSpice
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
            var c = _tracker.GetBestCoordinateToGuess();
            foreach (var possibleValue in _tracker.GetPossibleValues(in c))
            {
                if (_tracker.TrySet(in c, possibleValue))
                {
                    if (_TrySolve())
                    {
                        return true;
                    }
                    _tracker.Unset(in c);
                }
            }
            return false;
        }
    }
}
