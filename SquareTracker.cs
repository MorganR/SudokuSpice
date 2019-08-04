using System;
using System.Collections.Generic;
using System.Linq;

namespace MorganRoff.Sudoku
{
    public class SquareTracker : ISquareTracker
    {
        private readonly Puzzle _puzzle;
        private readonly IReadOnlyList<IRestrict> _restricts;
        private readonly IList<Coordinate> _modifiedCoords;

        public SquareTracker(Puzzle puzzle, IReadOnlyList<IRestrict> restricts = null)
        {
            _puzzle = puzzle;
            _restricts = (restricts == null || restricts.Count == 0)
                ? RestrictUtils.CreateStandardRestricts(_puzzle)
                : restricts;
            _modifiedCoords = new List<Coordinate>(_puzzle.Size * _restricts.Count);
            RestrictUtils.RestrictAllPossibleValues(_puzzle, _restricts);
        }

        public Coordinate GetBestCoordinateToGuess()
        {
            if (_puzzle.NumEmptySquares == 0)
            {
                throw new InvalidOperationException("No unset squares left to guess!");
            }
            int minNumPossibles = _puzzle.Size + 1;
            Coordinate bestCoord = new Coordinate(0, 0);
            foreach (var c in _puzzle.GetUnsetCoords())
            {
                int numPossibles =
                    _puzzle.GetPossibleValues(c.Row, c.Column)
                    .CountSetBits();
                if (numPossibles == 1)
                {
                    return c;
                }
                if (numPossibles < minNumPossibles)
                {
                    bestCoord = c;
                    minNumPossibles = numPossibles;
                }
            }
            return bestCoord;
        }

        public IEnumerable<int> GetPossibleValues(in Coordinate c)
        {
            return _puzzle.GetPossibleValues(c.Row, c.Column)
                .GetSetBits().Select(b => b + 1);
        }

        public int GetNumEmptySquares()
        {
            return _puzzle.NumEmptySquares;
        }

        public bool TrySet(in Coordinate coord, int possibleValue)
        {
            _puzzle.Set(coord.Row, coord.Column, possibleValue);
            for (var restrictIdx = 0; restrictIdx < _restricts.Count; restrictIdx++)
            {
                _modifiedCoords.Clear();
                _restricts[restrictIdx].Update(in coord, possibleValue, _modifiedCoords);
                foreach (var c in _modifiedCoords)
                {
                    _puzzle.SetPossibleValues(c.Row, c.Column,
                        _restricts[restrictIdx].GetPossibleValues(in c)
                        & _puzzle.GetPossibleValues(c.Row, c.Column));
                    if (_puzzle.GetPossibleValues(c.Row, c.Column) == 0)
                    {
                        _RevertRestricts(in coord, possibleValue, restrictIdx + 1);
                        _puzzle.Unset(coord.Row, coord.Column);
                        return false;
                    }
                }
            }
            return true;
        }

        public void Unset(in Coordinate coord)
        {
            var val = _puzzle.Get(coord.Row, coord.Column).Value;
            _RevertRestricts(in coord, val, _restricts.Count);
            _puzzle.Unset(coord.Row, coord.Column);
        }

        private void _RevertRestricts(in Coordinate coord, int possibleValue, int numRestrictsToUndo)
        {
            _modifiedCoords.Clear();
            for (var restrictIdx = numRestrictsToUndo - 1; restrictIdx >= 0; restrictIdx--)
            {
                _restricts[restrictIdx].Revert(in coord, possibleValue, _modifiedCoords);
            }
            foreach (var modifiedCoord in _modifiedCoords)
            {
                _puzzle.SetPossibleValues(modifiedCoord.Row, modifiedCoord.Column,
                    _restricts.Aggregate(
                        -1,
                        (agg, r) => agg &= r.GetPossibleValues(in modifiedCoord)));
            }
        }
    }
}
