using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SudokuSpice
{
    public class StandardSquareTracker : ISquareTracker
    {
        private readonly Puzzle _puzzle;
        private readonly StandardRestrict _restrict;
        private readonly IReadOnlyList<ISudokuHeuristic> _heuristics;
        private readonly IList<Coordinate> _affectedCoords;
        private readonly Stack<Coordinate> _coordsThatUsedHeuristics;

        public StandardSquareTracker(
            Puzzle puzzle,
            StandardRestrict restrict,
            IReadOnlyList<ISudokuHeuristic> heuristics = null)
        {
            _puzzle = puzzle;
            _restrict = restrict;
            _heuristics = heuristics ?? new List<ISudokuHeuristic>();
            _affectedCoords = new List<Coordinate>(puzzle.NumEmptySquares);
            _coordsThatUsedHeuristics = new Stack<Coordinate>(puzzle.NumEmptySquares);
            BitVector updatedPossibles;
            foreach (var c in _puzzle.GetUnsetCoords())
            {
                updatedPossibles = _restrict.GetPossibleValues(in c);
                if (updatedPossibles.IsEmpty())
                {
                    throw new ArgumentException(
                        "Puzzle could not be solved with the given preset values.");
                }
                _puzzle.SetPossibleValues(c.Row, c.Column, updatedPossibles);
            }
        }

        public Coordinate GetBestCoordinateToGuess()
        {
            Debug.Assert(_puzzle.NumEmptySquares > 0, "No unset squares left to guess!");
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

        public int GetNumEmptySquares() => _puzzle.NumEmptySquares;

        public bool TrySet(in Coordinate coord, int value)
        {
            _puzzle[in coord] = value;
            _affectedCoords.Clear();
            _restrict.Update(in coord, value, _affectedCoords);
            BitVector updatedPossibles;
            for (int i = 0; i < _affectedCoords.Count; i++)
            {
                var c = _affectedCoords[i];
                updatedPossibles = BitVector.FindIntersect(
                    _puzzle.GetPossibleValues(c.Row, c.Column),
                    _restrict.GetPossibleValues(in c));
                if (updatedPossibles.IsEmpty())
                {
                    i--;
                    _restrict.Revert(in coord, value, new List<Coordinate>(_affectedCoords.Count));
                    for (; i >= 0; i--)
                    {
                        var affectedCoord = _affectedCoords[i];
                        _puzzle.SetPossibleValues(affectedCoord.Row, affectedCoord.Column,
                            _restrict.GetPossibleValues(in affectedCoord));
                    }
                    _puzzle[in coord] = null;
                    return false;
                }
                _puzzle.SetPossibleValues(c.Row, c.Column, updatedPossibles);
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
                // No need to clear modifiedCoords since the value isn't used here.
                _restrict.Revert(in coord, val, _affectedCoords);
                foreach (var c in _puzzle.GetUnsetCoords())
                {
                    _puzzle.SetPossibleValues(c.Row, c.Column,
                        _restrict.GetPossibleValues(in c));
                }
            }
            else
            {
                _RevertRestrict(in coord, val);
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

        private void _RevertRestrict(in Coordinate coord, int valueToRevert)
        {
            _affectedCoords.Clear();
            _restrict.Revert(in coord, valueToRevert, _affectedCoords);
            foreach (var affectedCoord in _affectedCoords)
            {
                _puzzle.SetPossibleValues(affectedCoord.Row, affectedCoord.Column,
                    _restrict.GetPossibleValues(in affectedCoord));
            }
        }
    }
}
