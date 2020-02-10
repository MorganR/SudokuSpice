using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSpice
{
    public class FlexibleSquareTracker : ISquareTracker
    {
        private readonly Puzzle _puzzle;
        private readonly IReadOnlyList<ISudokuRestrict> _restricts;
        private readonly IReadOnlyList<ISudokuHeuristic> _heuristics;
        private readonly IList<Coordinate> _affectedCoords;
        private readonly Stack<Coordinate> _coordsThatUsedHeuristics;

        public FlexibleSquareTracker(
            Puzzle puzzle,
            IReadOnlyList<ISudokuRestrict> restricts = null,
            IReadOnlyList<ISudokuHeuristic> heuristics = null)
        {
            _puzzle = puzzle;
            _restricts = (restricts == null || restricts.Count == 0)
                ? new List<ISudokuRestrict> { new StandardRestrict(puzzle) }
                : restricts;
            _heuristics = heuristics ?? new List<ISudokuHeuristic>();
            _affectedCoords = new List<Coordinate>(_puzzle.Size * _restricts.Count);
            _coordsThatUsedHeuristics = new Stack<Coordinate>(_puzzle.NumEmptySquares);
            RestrictUtils.RestrictAllUnsetPossibleValues(_puzzle, _restricts);
        }

        public Coordinate GetBestCoordinateToGuess()
        {
            if (_puzzle.NumEmptySquares == 0)
            {
                throw new InvalidOperationException("No unset squares left to guess!");
            }
            Coordinate bestCoord;
            int numPossibles;
            (bestCoord, numPossibles) = _GetCoordinateWithFewestPossibleValues();
            if (numPossibles == 1 || _heuristics.Count == 0)
            {
                return bestCoord;
            }
            foreach (var heuristic in _heuristics)
            {
                heuristic.UpdateAll();
            }
            (bestCoord, _) = _GetCoordinateWithFewestPossibleValues();
            _coordsThatUsedHeuristics.Push(bestCoord);
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

        public bool TrySet(in Coordinate coord, int value)
        {
            _puzzle[in coord] = value;
            BitVector updatedPossibles;
            for (var restrictIdx = 0; restrictIdx < _restricts.Count; restrictIdx++)
            {
                _affectedCoords.Clear();
                _restricts[restrictIdx].Update(in coord, value, _affectedCoords);
                foreach (var c in _affectedCoords)
                {
                    updatedPossibles = BitVector.FindIntersect(
                            _restricts[restrictIdx].GetPossibleValues(in c),
                            _puzzle.GetPossibleValues(c.Row, c.Column));
                    if (updatedPossibles.IsEmpty())
                    {
                        _RevertRestricts(in coord, value, restrictIdx + 1);
                        _puzzle[in coord] = null;
                        return false;
                    }
                    _puzzle.SetPossibleValues(c.Row, c.Column, updatedPossibles);
                }
            }
            return true;
        }

        public void Unset(in Coordinate coord)
        {
            var val = _puzzle[in coord].Value;
            if (_coordsThatUsedHeuristics.Count > 0
                && _coordsThatUsedHeuristics.Peek().Equals(coord))
            {
                _coordsThatUsedHeuristics.Pop();
                for (var restrictIdx = _restricts.Count - 1; restrictIdx >= 0; restrictIdx--)
                {
                    _restricts[restrictIdx].Revert(in coord, val, _affectedCoords);
                }
                RestrictUtils.RestrictAllUnsetPossibleValues(_puzzle, _restricts);
            } else
            {
                _RevertRestricts(in coord, val, _restricts.Count);
            }
            _puzzle[in coord] = null;
        }

        private (Coordinate coord, int numPossibles) _GetCoordinateWithFewestPossibleValues()
        {
            int minNumPossibles = _puzzle.Size + 1;
            Coordinate bestCoord = new Coordinate(0, 0);
            foreach (var c in _puzzle.GetUnsetCoords())
            {
                int numPossibles = _puzzle.GetPossibleValues(c.Row, c.Column).Count;
                if (numPossibles == 1)
                {
                    return (c, 1);
                }
                if (numPossibles < minNumPossibles)
                {
                    bestCoord = c;
                    minNumPossibles = numPossibles;
                }
            }
            return (bestCoord, minNumPossibles);
        }

        private void _RevertRestricts(
            in Coordinate coord, int valueToRevert, int numRestrictsToUndo)
        {
            _affectedCoords.Clear();
            for (var restrictIdx = numRestrictsToUndo - 1; restrictIdx >= 0; restrictIdx--)
            {
                _restricts[restrictIdx].Revert(in coord, valueToRevert, _affectedCoords);
            }
            foreach (var modifiedCoord in _affectedCoords)
            {
                _puzzle.SetPossibleValues(modifiedCoord.Row, modifiedCoord.Column,
                    _restricts.Aggregate(
                        new BitVector(uint.MaxValue),
                        (agg, r) => BitVector.FindIntersect(
                            agg, r.GetPossibleValues(in modifiedCoord))));
            }
        }
    }
}
