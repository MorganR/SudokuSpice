using System;
using System.Diagnostics;

namespace SudokuSpice.RuleBased.Rules
{
    /// <summary>
    /// Restricts that each 'box' contains all unique values.
    /// </summary>
    public class BoxUniquenessRule : UniquenessRule, IMissingBoxValuesTracker
    {
        private IReadOnlyBoxPuzzle? _puzzle;

        public BoxUniquenessRule() : base() { }

        private BoxUniquenessRule(BoxUniquenessRule existing, IReadOnlyBoxPuzzle? puzzle)
            : base(existing)
        {
            _puzzle = puzzle;
        }

        /// <inheritdoc/>
        public override ISudokuRule CopyWithNewReference(IReadOnlyPuzzle? puzzle)
        {
            if (puzzle is not IReadOnlyBoxPuzzle boxPuzzle)
            {
                throw new ArgumentException($"An {nameof(IReadOnlyBoxPuzzle)} is required to copy {nameof(BoxUniquenessRule)}.");
            }
            return new BoxUniquenessRule(this, boxPuzzle);
        }

        /// <summary>
        /// Tries to initialize this rule to prepare to solve the given puzzle.
        /// </summary>
        /// <param name="puzzle">
        /// The puzzle to be solved. Must implement <see cref="IReadOnlyBoxPuzzle"/>, else this
        /// fails and returns false.
        /// </param>
        /// <returns>
        /// False if the puzzle violates this rule and initialization fails, else true.
        /// </returns>
        public override bool TryInit(IReadOnlyPuzzle puzzle)
        {
            if (puzzle is not IReadOnlyBoxPuzzle boxPuzzle)
            {
                return false;
            }
            _puzzle = boxPuzzle;
            if (!base.TryInit(puzzle))
            {
                _puzzle = null;
                return false;
            }
            return true;
        }

        /// <inheritdoc/>
        public BitVector GetMissingValuesForBox(int box) => GetPossibleValues(box);

        /// <inheritdoc/>
        protected override int GetNumDimensions(IReadOnlyPuzzle puzzle) => puzzle.Size;
        /// <inheritdoc/>
        protected override int GetDimension(in Coordinate c) => _puzzle!.GetBoxIndex(c.Row, c.Column);
        /// <inheritdoc/>
        protected override void TrackUnsetCoordinatesOnSameDimension(int dimension, in Coordinate source, CoordinateTracker tracker)
        {
            Debug.Assert(_puzzle is not null, $"Can't call {nameof(Update)} with a null puzzle.");
            foreach (Coordinate unsetCoord in _puzzle.YieldUnsetCoordsForBox(dimension))
            {
                if (source.Column == unsetCoord.Column && source.Row == unsetCoord.Row)
                {
                    continue;
                }
                tracker.AddOrTrackIfUntracked(unsetCoord);
            }
        }
    }
}