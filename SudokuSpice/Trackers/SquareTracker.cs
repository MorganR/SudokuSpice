using SudokuSpice.Rules;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SudokuSpice
{
    public class SquareTracker : ISquareTracker
    {
        private readonly Puzzle _puzzle;
        private readonly PossibleValues _possibleValues;
        private readonly ISudokuRuleKeeper _ruleKeeper;
        private readonly ISudokuHeuristic? _heuristic;
        private readonly Stack<Coordinate> _setCoords;
        private readonly Stack<Coordinate> _coordsThatUsedHeuristics;

        public SquareTracker(Puzzle puzzle)
        {
            _puzzle = puzzle;
            _possibleValues = new PossibleValues(puzzle);
            _ruleKeeper = new StandardRuleKeeper(puzzle, _possibleValues);
            _heuristic = new StandardHeuristic(
                puzzle, _possibleValues, (StandardRuleKeeper)_ruleKeeper,
                (StandardRuleKeeper)_ruleKeeper, (StandardRuleKeeper)_ruleKeeper);
            _setCoords = new Stack<Coordinate>(puzzle.NumEmptySquares);
            _coordsThatUsedHeuristics = new Stack<Coordinate>(puzzle.NumEmptySquares);
        }

        public SquareTracker(
            Puzzle puzzle,
            PossibleValues possibleValues,
            ISudokuRuleKeeper ruleKeeper,
            ISudokuHeuristic? heuristic = null)
        {
            _puzzle = puzzle;
            _possibleValues = possibleValues;
            _ruleKeeper = ruleKeeper;
            _heuristic = heuristic;
            _setCoords = new Stack<Coordinate>(puzzle.NumEmptySquares);
            _coordsThatUsedHeuristics = new Stack<Coordinate>(puzzle.NumEmptySquares);
        }

        public Coordinate GetBestCoordinateToGuess()
        {
            Debug.Assert(_puzzle.NumEmptySquares > 0, "No unset squares left to guess!");
            Coordinate bestCoord;
            int numPossibles;
            (bestCoord, numPossibles) = _GetCoordinateWithFewestPossibleValues();
            if (numPossibles == 1 || _heuristic is null)
            {
                return bestCoord;
            }

            // Try heuristics if there isn't a definite square value.
            if (_setCoords.Count > 0)
            {
                _coordsThatUsedHeuristics.Push(_setCoords.Peek());
            }
            if (!_heuristic.UpdateAll())
            {
                return bestCoord;
            }
            (bestCoord, _) = _GetCoordinateWithFewestPossibleValues();
            return bestCoord;
        }

        public IEnumerable<int> GetPossibleValues(in Coordinate c)
        {
            return _possibleValues[in c]
                .GetSetBits().Select(b => b + 1);
        }

        public int GetNumEmptySquares() => _puzzle.NumEmptySquares;

        public bool TrySet(in Coordinate coord, int value)
        {
            bool isValid = _ruleKeeper.TrySet(in coord, value);
            if (!isValid)
            {
                return false;
            }
            _puzzle[in coord] = value;
            _setCoords.Push(coord);
            return true;
        }

        public void UnsetLast()
        {
            var lastCoord = _setCoords.Pop();
#pragma warning disable CS8629 // Nullable value type may be null.
            // If this is null, then we want to throw because this method is being misused.
            var value = _puzzle[in lastCoord].Value;
#pragma warning restore CS8629 // Nullable value type may be null.
            _puzzle[in lastCoord] = null;
            if (_coordsThatUsedHeuristics.Count > 0
                && _coordsThatUsedHeuristics.Peek().Equals(lastCoord))
            {
                _coordsThatUsedHeuristics.Pop();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                // Protected by _coordsThatUsedHeuristics.
                _heuristic.UndoLastUpdate();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }
            _ruleKeeper.Unset(in lastCoord, value);
        }

        private (Coordinate coord, int numPossibles) _GetCoordinateWithFewestPossibleValues()
        {
            int minNumPossibles = _puzzle.Size + 1;
            Coordinate bestCoord = new Coordinate(0, 0);
            foreach (var c in _puzzle.GetUnsetCoords())
            {
                int numPossibles = _possibleValues[in c].Count;
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
    }
}
