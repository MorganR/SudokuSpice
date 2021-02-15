using System.Diagnostics;

namespace SudokuSpice.RuleBased.Rules
{
    /// <summary>
    /// Restricts that each 'box' contains all unique values.
    /// </summary>
    public class BoxUniquenessRule : UniquenessRule, IMissingBoxValuesTracker
    {
        private int _boxSize;
        private IReadOnlyPuzzle? _puzzle;

        public BoxUniquenessRule() : base() { }

        private BoxUniquenessRule(BoxUniquenessRule existing, IReadOnlyPuzzle? puzzle)
            : base(existing)
        {
            _boxSize = existing._boxSize;
            _puzzle = puzzle;
        }

        /// <inheritdoc/>
        public override IRule CopyWithNewReference(IReadOnlyPuzzle? puzzle)
        {
            return new BoxUniquenessRule(this, puzzle);
        }

        /// <inheritdoc/>
        public override bool TryInit(IReadOnlyPuzzle puzzle, BitVector allPossibleValues)
        {
            _boxSize = Boxes.IntSquareRoot(puzzle.Size);
            _puzzle = puzzle;
            if (!base.TryInit(puzzle, allPossibleValues))
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
        protected override int GetDimension(in Coordinate c) => Boxes.CalculateBoxIndex(in c, _boxSize);
        /// <inheritdoc/>
        protected override void TrackUnsetCoordinatesOnSameDimension(int dimension, in Coordinate source, CoordinateTracker tracker)
        {
            Debug.Assert(_puzzle is not null, $"Can't call {nameof(Update)} with a null puzzle.");
            foreach (Coordinate unsetCoord in Boxes.YieldUnsetCoordsForBox(dimension, _boxSize, _puzzle!))
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