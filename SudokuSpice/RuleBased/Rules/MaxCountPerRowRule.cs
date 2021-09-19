using System.Diagnostics;

namespace SudokuSpice.RuleBased.Rules
{
    /// <summary>
    /// Restricts that each row contains a max count of each value, as specified by the
    /// <see cref="IReadOnlyPuzzle.CountPerUniqueValue"/> dictionary.
    /// </summary>
    public class MaxCountPerRowRule : MaxCountRule, IMissingRowValuesTracker
    {
        private IReadOnlyPuzzle? _puzzle;

        public MaxCountPerRowRule() : base() { }

        private MaxCountPerRowRule(MaxCountPerRowRule existing, IReadOnlyPuzzle? puzzle)
            : base(existing)
        {
            _puzzle = puzzle;
        }

        /// <inheritdoc/>
        public override IRule CopyWithNewReference(IReadOnlyPuzzle? puzzle) => new MaxCountPerRowRule(this, puzzle);

        /// <inheritdoc/>
        public override bool TryInit(IReadOnlyPuzzle puzzle, BitVector uniquePossibleValues)
        {
            if (!base.TryInit(puzzle, uniquePossibleValues))
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
            Debug.Assert(_puzzle is not null,
                $"Cannot call {nameof(TrackUnsetCoordinatesOnSameDimension)} when puzzle is null.");
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