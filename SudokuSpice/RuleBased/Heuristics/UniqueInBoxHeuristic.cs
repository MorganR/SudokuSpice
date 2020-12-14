using SudokuSpice.RuleBased.Rules;
using System;
using System.Collections.Generic;
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
    public class UniqueInBoxHeuristic : UniqueInXHeuristic
    {
        private readonly IMissingBoxValuesTracker _boxTracker;
        private int _boxSize;

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
            : base(existing, puzzle)
        {
            _boxTracker = boxTracker;
            _boxSize = existing._boxSize;
        }

        /// <summary>
        /// Creates a deep copy of this heuristic. Requires <c>rules</c> to contain an
        /// <see cref="IMissingBoxValuesTracker"/>.
        /// </summary>
        public override ISudokuHeuristic CopyWithNewReferences(
            IReadOnlyPuzzleWithMutablePossibleValues? puzzle,
            IReadOnlyList<ISudokuRule> rules)
        {
            IMissingBoxValuesTracker? boxTracker;
            try
            {
                boxTracker = (IMissingBoxValuesTracker)rules.First(r => r is IMissingBoxValuesTracker);
            } catch (InvalidOperationException)
            {
                throw new ArgumentException($"{nameof(rules)} must include an {nameof(IMissingBoxValuesTracker)}.");
            }
            return new UniqueInBoxHeuristic(this, puzzle, boxTracker);
        }

        /// <summary>
        /// Tries to initialize this heuristic for solving the given puzzle.
        /// </summary>
        /// <remarks>
        /// In general, it doesn't make sense to want to maintain the previous state if this method
        /// fails. Therefore, it is <em>not</em> guaranteed that the heuristic's state is unchanged
        /// on failure.
        /// </remarks>
        /// <param name="puzzle">
        /// The puzzle to solve. This must implement <see cref="IReadOnlyBoxPuzzle"/>.
        /// </param>
        /// <returns>
        /// False if this heuristic cannot be initialized for the given puzzle, else true.
        /// </returns>
        public override bool TryInitFor(IReadOnlyPuzzleWithMutablePossibleValues puzzle)
        {
            _boxSize = Boxes.CalculateBoxSize(puzzle.Size);
            return base.TryInitFor(puzzle);
        }

        protected override int GetNumDimensions(IReadOnlyPuzzle puzzle) => puzzle.Size;
        protected override BitVector GetMissingValuesForDimension(int dimension, IReadOnlyPuzzle _) => _boxTracker.GetMissingValuesForBox(dimension);
        protected override IEnumerable<Coordinate> GetUnsetCoordinatesOnDimension(int dimension, IReadOnlyPuzzle puzzle)
        {
            return Boxes.YieldUnsetCoordsForBox(dimension, _boxSize, puzzle);
        }
    }
}