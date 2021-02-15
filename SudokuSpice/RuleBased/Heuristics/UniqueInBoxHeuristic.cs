using SudokuSpice.RuleBased.Rules;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SudokuSpice.RuleBased.Heuristics
{
    /// <summary>
    /// Checks for any squares that are the unique provider of a given possible value within a box.
    /// Sets the possible values for those squares to just their unique value.
    ///
    /// For example, if a box had three unset squares with possible values: <c>A: [1, 2]</c>,
    /// <c>B: [1, 2]</c>, and <c>C: [1, 2, 3]</c>, then this would set <c>C</c>'s possible values
    /// to <c>[3]</c>.
    /// </summary>
    public class UniqueInBoxHeuristic : IHeuristic
    {
        private readonly IMissingBoxValuesTracker _boxTracker;
        private int _boxSize;
        private IReadOnlyPuzzleWithMutablePossibleValues? _puzzle;
        private UniqueInXHelper? _helper;

        /// <summary>
        /// Creates the heuristic.
        /// </summary>
        /// <param name="boxValuesTracker">
        /// Something that tracks the possible values for each box. Rules often do this already,
        /// for example.
        /// </param>
        public UniqueInBoxHeuristic(IMissingBoxValuesTracker boxValuesTracker) : base()
        {
            _boxTracker = boxValuesTracker;
        }

        private UniqueInBoxHeuristic(
            UniqueInBoxHeuristic existing,
            IReadOnlyPuzzleWithMutablePossibleValues? puzzle,
            IMissingBoxValuesTracker boxTracker)
        {
            _boxTracker = boxTracker;
            _boxSize = existing._boxSize;
            _puzzle = puzzle;
            if (puzzle is not null && existing._helper is not null)
            {
                _helper = existing._helper.CopyWithNewReference(puzzle);
            }
        }

        /// <summary>
        /// Creates a deep copy of this heuristic. Requires <c>rules</c> to contain an
        /// <see cref="IMissingBoxValuesTracker"/>.
        /// </summary>
        public IHeuristic CopyWithNewReferences(
            IReadOnlyPuzzleWithMutablePossibleValues? puzzle,
            IReadOnlyList<IRule> rules)
        {
            try
            {
                return new UniqueInBoxHeuristic(
                    this, puzzle, (IMissingBoxValuesTracker)rules.First(r => r is IMissingBoxValuesTracker));
            } catch (InvalidOperationException)
            {
                throw new ArgumentException($"{nameof(rules)} must include an {nameof(IMissingBoxValuesTracker)}.");
            }
        }

        /// <inheritdoc/>
        public bool TryInitFor(IReadOnlyPuzzleWithMutablePossibleValues puzzle)
        {
            _boxSize = Boxes.IntSquareRoot(puzzle.Size);
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
            for (int boxIdx = 0; boxIdx < size; ++boxIdx)
            {
                possibleValuesToCheck[boxIdx] = _boxTracker.GetMissingValuesForBox(boxIdx);
                coordinatesToCheck[boxIdx] = Boxes.YieldUnsetCoordsForBox(boxIdx, _boxSize, _puzzle).ToArray();
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
    }
}