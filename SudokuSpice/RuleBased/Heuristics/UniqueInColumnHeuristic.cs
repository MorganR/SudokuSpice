using SudokuSpice.RuleBased.Rules;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SudokuSpice.RuleBased.Heuristics
{
    /// <summary>
    /// Checks for any squares that are the unique provider of a given possible value within a
    /// column. Sets the possible values for those squares to just their unique value.
    /// </summary>
    /// <remarks>
    /// For example, if a column had three unset squares with possible values: <c>A: [1, 2]</c>,
    /// <c>B: [1, 2]</c>, and <c>C: [1, 2, 3]</c>, then this would set <c>C</c>'s possible values
    /// to <c>[3]</c>.
    /// </remarks>
    public class UniqueInColumnHeuristic : IHeuristic
    {
        private readonly IMissingColumnValuesTracker _columnTracker;
        private IReadOnlyPuzzleWithMutablePossibleValues? _puzzle;
        private UniqueInXHelper? _helper;

        /// <summary>
        /// Creates the heuristic.
        /// </summary>
        /// <param name="columnValuesTracker">
        /// Something that tracks the possible values for each column. Rules often do this already,
        /// for example.
        /// </param>
        /// 
        public UniqueInColumnHeuristic(IMissingColumnValuesTracker columnValuesTracker) : base()
        {
            _columnTracker = columnValuesTracker;
        }

        private UniqueInColumnHeuristic(
            UniqueInColumnHeuristic existing,
            IReadOnlyPuzzleWithMutablePossibleValues? puzzle,
            IMissingColumnValuesTracker rule)
        {
            _columnTracker = rule;
            _puzzle = puzzle;
            if (existing._helper is not null
                && puzzle is not null)
            {
                _helper = existing._helper.CopyWithNewReference(puzzle);
            }
        }

        /// <summary>
        /// Creates a deep copy of this heuristic. Requires <c>rules</c> to contain an
        /// <see cref="IMissingColumnValuesTracker"/>.
        /// </summary>
        public IHeuristic CopyWithNewReferences(
            IReadOnlyPuzzleWithMutablePossibleValues? puzzle,
            IReadOnlyList<IRule> rules)
        {
            try
            {
                return new UniqueInColumnHeuristic(
                    this, puzzle,
                    (IMissingColumnValuesTracker)rules.First(r => r is IMissingColumnValuesTracker));
            } catch (InvalidOperationException)
            {
                throw new ArgumentException($"{nameof(rules)} must include an {nameof(IMissingColumnValuesTracker)}.");
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
            for (int column = 0; column < size; ++column)
            {
                possibleValuesToCheck[column] = _columnTracker.GetMissingValuesForColumn(column);
                coordinatesToCheck[column] = _GetUnsetCoordinatesOn(column).ToArray();
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

        private IEnumerable<Coordinate> _GetUnsetCoordinatesOn(int column)
        {
            int size = _puzzle!.Size;
            for (int row = 0; row < size; ++row)
            {
                if (!_puzzle[row, column].HasValue)
                {
                    yield return new Coordinate(row, column);
                }
            }
        }
    }
}