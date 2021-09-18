using SudokuSpice.RuleBased.Heuristics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SudokuSpice.RuleBased
{
    /// <summary>
    /// Tracks and sets Sudoku squares and their possible values.
    /// </summary>
    internal class SquareTracker<TPuzzle> where TPuzzle : class, IPuzzleWithPossibleValues<TPuzzle>
    {
        private readonly TPuzzle _puzzle;
        private readonly IRuleKeeper _ruleKeeper;
        private readonly IHeuristic? _heuristic;
        private readonly Stack<Coordinate> _setCoords;
        private readonly Stack<Coordinate>? _coordsThatUsedHeuristics;

        internal IReadOnlyPuzzleWithPossibleValues Puzzle => _puzzle;

        private SquareTracker(
            TPuzzle puzzle,
            IRuleKeeper ruleKeeper,
            IHeuristic? heuristic = null)
        {
            _puzzle = puzzle;
            _ruleKeeper = ruleKeeper;
            _heuristic = heuristic;
            _setCoords = new Stack<Coordinate>(puzzle.NumEmptySquares);
            _coordsThatUsedHeuristics = _heuristic is null ? null : new Stack<Coordinate>(puzzle.NumEmptySquares);
        }

        /// <summary>
        /// Creates a deep copy of this tracker in its current state.
        /// </summary>
        internal SquareTracker(SquareTracker<TPuzzle> existing)
        {
            _puzzle = existing._puzzle.DeepCopy();
            _setCoords = new Stack<Coordinate>(existing._setCoords);
            if (existing._coordsThatUsedHeuristics is not null)
            {
                _coordsThatUsedHeuristics = new Stack<Coordinate>(existing._coordsThatUsedHeuristics!);
            }
            _ruleKeeper = existing._ruleKeeper.CopyWithNewReferences(_puzzle);
            _heuristic = existing._heuristic?.CopyWithNewReferences(
                _puzzle, _ruleKeeper.GetRules());
        }

        /// <summary>
        /// Tries to construct and initialize a tracker for the given puzzle, rules, and heuristics.
        /// </summary>
        /// <param name="puzzle">The puzzle to solve.</param>
        /// <param name="ruleKeeper">The rule-keeper to satisfy.</param>
        /// <param name="heuristic">An optional heuristic to user.</param>
        /// <param name="tracker">
        /// An <c>out</c> param where the tracker will be created. Set to null if initialization
        /// fails (i.e. this method returns false).
        /// </param>
        /// <returns>
        /// False if initialization fails, for example if the puzzle violates a rule, else true.
        /// </returns>
        internal static bool TryInit(
            TPuzzle puzzle,
            IRuleKeeper ruleKeeper,
            IHeuristic? heuristic,
            out SquareTracker<TPuzzle>? tracker)
        {
            if (!ruleKeeper.TryInit(puzzle)
                || (!heuristic?.TryInitFor(puzzle) ?? false))
            {
                tracker = null;
                return false;
            }
            tracker = new SquareTracker<TPuzzle>(puzzle, ruleKeeper, heuristic);
            return true;
        }

        /// <summary>
        /// Gets the coordinate for the next square to fill in.
        /// </summary>
        /// <returns>The coordinate for the unset square with the least possible values.</returns>
        internal Coordinate GetBestCoordinateToGuess()
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
            Debug.Assert(_coordsThatUsedHeuristics is not null,
                $"{nameof(_coordsThatUsedHeuristics)} must not be null if {nameof(_heuristic)} is non-null.");
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

        /// <summary>
        /// Populates a provided Span with the possible values at the given coordinate, and returns
        /// the number of possible values that were populated.
        ///
        /// If there are fewer possible values than the length of the
        /// <paramref name="possibleValues"/> span, the remaining values in the span are left
        /// unchanged.
        /// </summary>
        /// <param name="c">The coordinate of the square to retrieve possible values for.</param>
        /// <param name="possibleValues">
        /// A span to populate with the possible values. Should be at least as large as the size of
        /// the puzzle.
        /// </param>
        /// <returns>
        /// The number of possible values that were set in the <paramref name="possibleValues"/> span.
        /// </returns>

        internal int PopulatePossibleValues(in Coordinate c, Span<int> possibleValues) =>
            _puzzle.GetPossibleValues(in c).PopulateSetBits(possibleValues);

        /// <summary>
        /// Tries to set the square at the given coordinate to the given possible value. This also 
        /// modifies its internal data as needed to maintain track of the square's values. If the
        /// value can't be set, this undoes all internal changes as if this method was not called.
        /// </summary>
        /// <param name="c">The coordinate of the square to set.</param>
        /// <param name="value">The value to set the square to.</param>
        /// <returns>True if the set succeeded.</returns>
        internal bool TrySet(in Coordinate coord, int value)
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

        /// <summary>
        /// Unsets the most recently set square.
        /// </summary>
        internal void UnsetLast()
        {
            Coordinate lastCoord = _setCoords.Pop();
            // If this is null, then we want to throw because this method is being misused.
            int value = _puzzle[in lastCoord]!.Value;
            _puzzle[in lastCoord] = null;
            _ruleKeeper.Unset(in lastCoord, value);
            if (_coordsThatUsedHeuristics?.Count > 0
                && _coordsThatUsedHeuristics.Peek().Equals(lastCoord))
            {
                _coordsThatUsedHeuristics.Pop();
                // Protected by _coordsThatUsedHeuristics.
                _heuristic!.UndoLastUpdate();
            }
        }

        private (Coordinate coord, int numPossibles) _GetCoordinateWithFewestPossibleValues()
        {
            int minNumPossibles = _puzzle.Size + 1;
            var bestCoord = new Coordinate(0, 0);
            foreach (Coordinate c in _puzzle.GetUnsetCoords())
            {
                int numPossibles = _puzzle.GetPossibleValues(in c).ComputeCount();
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