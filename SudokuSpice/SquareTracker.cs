﻿using SudokuSpice.Data;
using SudokuSpice.Heuristics;
using SudokuSpice.Rules;
using System.Collections.Generic;
using System.Diagnostics;

namespace SudokuSpice
{
    /// <summary>
    /// Tracks and sets Sudoku squares and their possible values.
    /// </summary>
    public class SquareTracker : ISquareTracker
    {
        private readonly Puzzle _puzzle;
        private readonly PossibleValues _possibleValues;
        private readonly ISudokuRuleKeeper _ruleKeeper;
        private readonly ISudokuHeuristic? _heuristic;
        private readonly Stack<Coordinate> _setCoords;
        private readonly Stack<Coordinate> _coordsThatUsedHeuristics;

        /// <summary>
        /// Constructs a square tracker with a <see cref="StandardRuleKeeper"/> and a
        /// <see cref="StandardHeuristic"/>. Provided as a shortcut for standard Sudoku puzzles.
        /// </summary>
        /// <param name="puzzle">The puzzle to track.</param>
        public SquareTracker(Puzzle puzzle)
        {
            _puzzle = puzzle;
            _possibleValues = new PossibleValues(puzzle);
            _ruleKeeper = new StandardRuleKeeper(puzzle, _possibleValues);
            _heuristic = new StandardHeuristic(
                puzzle, _possibleValues, (IMissingRowValuesTracker)_ruleKeeper,
                (IMissingColumnValuesTracker)_ruleKeeper, (IMissingBoxValuesTracker)_ruleKeeper);
            _setCoords = new Stack<Coordinate>(puzzle.NumEmptySquares);
            _coordsThatUsedHeuristics = new Stack<Coordinate>(puzzle.NumEmptySquares);
        }

        /// <summary>
        /// Constructs a square tracker to track the given puzzle using the given possible values,
        /// rule keeper, and heuristic.
        /// </summary>
        /// <param name="puzzle">The puzzle to track.</param>
        /// <param name="possibleValues">A possible values tracker for the given puzzle.</param>
        /// <param name="ruleKeeper">The rule keeper to satisfy when modifying this puzzle.</param>
        /// <param name="heuristic">
        /// A heuristic to use to solve this puzzle efficiently. Can be set to null to skip using
        /// heuristics.
        /// <para>
        /// Note that only one heuristic can be provided. To use multiple heuristics, create a
        /// wrapper heuristic like <see cref="StandardHeuristic"/>.
        /// </para>
        /// </param>
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

        private SquareTracker(SquareTracker existing)
        {
            _puzzle = new Puzzle(existing._puzzle);
            _possibleValues = new PossibleValues(existing._possibleValues);
            _ruleKeeper = existing._ruleKeeper.CopyWithNewReferences(_puzzle, _possibleValues);
            _heuristic = existing._heuristic?.CopyWithNewReferences(
                _puzzle, _possibleValues, _ruleKeeper.GetRules());
            _setCoords = new Stack<Coordinate>(existing._setCoords);
            _coordsThatUsedHeuristics = new Stack<Coordinate>(existing._coordsThatUsedHeuristics);
        }

        /// <inheritdoc/>
        public ISquareTracker DeepCopy()
        {
            return new SquareTracker(this);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public List<int> GetPossibleValues(in Coordinate c)
        {
            return _possibleValues[in c].GetSetBits();
        }

        /// <inheritdoc/>
        public int GetNumEmptySquares() => _puzzle.NumEmptySquares;

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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