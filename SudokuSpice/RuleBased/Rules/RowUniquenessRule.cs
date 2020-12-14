using System.Diagnostics;

namespace SudokuSpice.RuleBased.Rules
{
    /// <summary>
    /// Restricts that each row contains all unique values.
    /// </summary>
    public class RowUniquenessRule : UniquenessRule, IMissingRowValuesTracker
    {
        private IReadOnlyPuzzle? _puzzle;

        public RowUniquenessRule() : base() { }

        private RowUniquenessRule(RowUniquenessRule existing, IReadOnlyPuzzle? puzzle)
            : base(existing)
        {
            _puzzle = puzzle;
        }

        /// <inheritdoc/>
        public override ISudokuRule CopyWithNewReference(IReadOnlyPuzzle? puzzle) => new RowUniquenessRule(this, puzzle);

        /// <inheritdoc/>
        public override bool TryInit(IReadOnlyPuzzle puzzle)
        {
            // This should be enforced by the rule keeper.
            if (!base.TryInit(puzzle))
            {
                return false;
            }
            _puzzle = puzzle;
            return true;
        }

        /// <inheritdoc/>
        public BitVector GetMissingValuesForRow(int row) => GetPossibleValues(row);

        /// <inheritdoc/>
        protected override int GetNumDimensions(IReadOnlyPuzzle puzzle) => puzzle.Size;

        /// <inheritdoc/>
        protected override int GetDimension(in Coordinate c) => c.Row;

        /// <inheritdoc/>
        protected override void TrackUnsetCoordinatesOnSameDimension(int dimension, in Coordinate source, CoordinateTracker tracker)
        {
            Debug.Assert(_puzzle is not null, "Cannot call RowUniquenessRule._AddUnsetFromRow when puzzle is null.");
            int size = _puzzle.Size;
            for (int col = 0; col < size; col++)
            {
                if (col != source.Column && !_puzzle[dimension, col].HasValue)
                {
                    tracker.AddOrTrackIfUntracked(new Coordinate(source.Row, col));
                }
            }
        }
    }
}