using System;
using System.Collections.Generic;
using System.Linq;

namespace Sudoku
{
    public class SquareTracker : ISquareTracker
    {
        private readonly Puzzle _puzzle;
        private readonly Coordinate[] _trackedCoords;
        private readonly IReadOnlyList<IRestrict> _restricts;
        private readonly IList<Coordinate> _modifiedCoords;
        private int _unsetCoordsEnd;
        private int _setCoordsEnd;

        public SquareTracker(Puzzle puzzle, IReadOnlyList<IRestrict> restricts = null)
        {
            _puzzle = puzzle;
            if (restricts == null || restricts.Count == 0)
            {
                _restricts = _CreateStandardRestricts(puzzle);
            }
            else
            {
                _restricts = restricts;
            }
            _modifiedCoords = new List<Coordinate>(puzzle.Size * _restricts.Count);
            _trackedCoords = new Coordinate[_puzzle.NumEmptySquares];
            _unsetCoordsEnd = _puzzle.NumEmptySquares;
            _setCoordsEnd = _puzzle.NumEmptySquares;

            int i = 0;
            foreach (var c in _puzzle.YieldUnsetCoords())
            {
                _trackedCoords[i++] = c;
                _puzzle.SetPossibleValues(
                    c.Row, c.Column,
                    _restricts.Aggregate(
                        -1,
                        (agg, r) => agg &= r.GetPossibleValues(in c)));
                if (_puzzle.GetPossibleValues(c.Row, c.Column) == 0)
                {
                    throw new ArgumentException(
                        "Puzzle could not be solved with the given preset values.");
                }
            }
        }

        public int GetBestIndexToGuess()
        {
            if (_unsetCoordsEnd == 0)
            {
                throw new InvalidOperationException("No unset squares left to guess!");
            }
            int minNumPossibles = _puzzle.Size + 1;
            int bestCoordIdx = -1;
            for (int i = 0; i < _unsetCoordsEnd; i++)
            {
                int numPossibles =
                    _puzzle.GetPossibleValues(_trackedCoords[i].Row, _trackedCoords[i].Column)
                    .CountSetBits();
                if (numPossibles == 1)
                {
                    return i;
                }
                if (numPossibles < minNumPossibles)
                {
                    bestCoordIdx = i;
                    minNumPossibles = numPossibles;
                }
            }
            return bestCoordIdx;
        }

        public IEnumerable<int> GetPossibleValues(int idx)
        {
            return _puzzle.GetPossibleValues(_trackedCoords[idx].Row, _trackedCoords[idx].Column)
                .GetSetBits().Select(b => b + 1);
        }

        public int GetNumEmptySquares()
        {
            return _puzzle.NumEmptySquares;
        }

        public bool TrySet(int idx, int possibleValue)
        {
            var coord = _trackedCoords[idx];
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
            _SwapToSet(idx);
            return true;
        }

        public void Unset(int unsetIdx)
        {
            if (_unsetCoordsEnd == _setCoordsEnd)
            {
                throw new InvalidOperationException("Cannot unset a square when none have been set.");
            }
            _ReturnLastSetToUnsetAt(unsetIdx);
            var coord = _trackedCoords[unsetIdx];
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

        private void _SwapToSet(int idx)
        {
            if (idx >= _unsetCoordsEnd)
            {
                throw new ArgumentOutOfRangeException(nameof(idx),
                    $"Must be less than the number of unset values ({_unsetCoordsEnd})");
            }
            var tmp = _trackedCoords[idx];
            _trackedCoords[idx] = _trackedCoords[_unsetCoordsEnd - 1];
            _trackedCoords[_unsetCoordsEnd - 1] = tmp;
            _unsetCoordsEnd--;
        }

        private void _ReturnLastSetToUnsetAt(int idx)
        {
            if (idx > _unsetCoordsEnd)
            {
                throw new ArgumentOutOfRangeException(nameof(idx),
                    $"Must be less than or equal to the number of unset values ({_unsetCoordsEnd})");
            }
            var tmp = _trackedCoords[_unsetCoordsEnd];
            _trackedCoords[_unsetCoordsEnd] = _trackedCoords[idx];
            _trackedCoords[idx] = tmp;
            _unsetCoordsEnd++;
        }

        private static IReadOnlyList<IRestrict> _CreateStandardRestricts(Puzzle puzzle)
        {
            return new List<IRestrict>
            {
                new RowRestrict(puzzle),
                new ColumnRestrict(puzzle),
                new BoxRestrict(puzzle)
            };
        }
    }
}
