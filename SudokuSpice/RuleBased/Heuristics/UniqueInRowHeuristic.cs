using SudokuSpice.RuleBased.Rules;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SudokuSpice.RuleBased.Heuristics
{
    /// <summary>
    /// Checks for any squares that are the unique provider of a given possible value within a row.
    /// Sets the possible values for those squares to just their unique value.
    /// </summary>
    /// <remarks>
    /// For example, if a row had three unset squares with possible values: <c>A: [1, 2]</c>,
    /// <c>B: [1, 2]</c>, and <c>C: [1, 2, 3]</c>, then this would set <c>C</c>'s possible values
    /// to <c>[3]</c>.
    /// </remarks>
    public class UniqueInRowHeuristic : IHeuristic
    {
        private readonly IMissingRowValuesTracker _rowTracker;
        private IReadOnlyPuzzleWithMutablePossibleValues? _puzzle;
        private UniqueInXHelper? _helper;

        /// <summary>
        /// Creates the heuristic.
        /// </summary>
        /// <param name="rowValuesTracker">
        /// Something that tracks the possible values for each row. Rules often do this already,
        /// for example.
        /// </param>
        public UniqueInRowHeuristic(IMissingRowValuesTracker rowValuesTracker)
        {
            _rowTracker = rowValuesTracker;
        }

        private UniqueInRowHeuristic(
            UniqueInRowHeuristic existing,
            IReadOnlyPuzzleWithMutablePossibleValues? puzzle,
            IMissingRowValuesTracker tracker)
        {
            _rowTracker = tracker;
            _puzzle = puzzle;
            if (existing._helper is not null
                && puzzle is not null)
            {
                _helper = existing._helper.CopyWithNewReference(puzzle);
            }
        }

        /// <summary>
        /// Creates a deep copy of this heuristic. Requires <c>rules</c> to contain an
        /// <see cref="IMissingRowValuesTracker"/>.
        /// </summary>
        public IHeuristic CopyWithNewReferences(
            IReadOnlyPuzzleWithMutablePossibleValues? puzzle, IReadOnlyList<ISudokuRule> rules)
        {
            try {
                return new UniqueInRowHeuristic(
                    this, puzzle,
                    (IMissingRowValuesTracker)rules.First(r => r is IMissingRowValuesTracker));
            } catch (InvalidOperationException)
            {
                throw new ArgumentException($"{nameof(rules)} must include an {nameof(IMissingRowValuesTracker)}.");
            }
        }

        /// <inheritdoc/>
        public bool TryInitFor(IReadOnlyPuzzleWithMutablePossibleValues puzzle)
        {
            _puzzle = puzzle;
            _helper = new UniqueInXHelper(puzzle);
            return true;
        }

        /// <inheritdoc/>
        public bool UpdateAll()
        {
            Debug.Assert(
                _puzzle is not null && _helper is not null,
                $"Must initialize heuristic before calling {nameof(UpdateAll)}.");
            int size = _puzzle.Size;
            var possibleValuesToCheck = new BitVector[size];
            var coordinatesToCheck = new Coordinate[size][];
            for (int row = 0; row < size; ++row)
            {
                possibleValuesToCheck[row] = _rowTracker.GetMissingValuesForRow(row);
                coordinatesToCheck[row] = _GetUnsetCoordinatesOn(row).ToArray();
            }
            return _helper.UpdateIfUnique(possibleValuesToCheck, coordinatesToCheck);
        }

        /// <inheritdoc/>
        public void UndoLastUpdate()
        {
            Debug.Assert(
                _helper is not null,
                $"Must initialize heuristic before calling {nameof(UndoLastUpdate)}.");
            _helper.UndoLastUpdate();
        }

        private IEnumerable<Coordinate> _GetUnsetCoordinatesOn(int row)
        {
            int size = _puzzle!.Size;
            for (int col = 0; col < size; ++col)
            {
                if (!_puzzle[row, col].HasValue)
                {
                    yield return new Coordinate(row, col);
                }
            }
        }
    }
}