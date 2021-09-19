using System.Diagnostics;

namespace SudokuSpice.RuleBased.Rules
{
    /// <summary>
    /// Restricts that each column contains a max count of each value, as specified by the
    /// <see cref="IReadOnlyPuzzle.CountPerUniqueValue"/> dictionary.
    /// </summary>
    public class MaxCountPerColumnRule : MaxCountRule, IMissingColumnValuesTracker
    {
        private IReadOnlyPuzzle? _puzzle;

        public MaxCountPerColumnRule() : base() { }

        private MaxCountPerColumnRule(MaxCountPerColumnRule existing, IReadOnlyPuzzle? puzzle)
            : base(existing)
        {
            _puzzle = puzzle;
        }

        /// <inheritdoc/>
        public override IRule CopyWithNewReference(IReadOnlyPuzzle? puzzle) =>
            new MaxCountPerColumnRule(this, puzzle);

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
        public BitVector GetMissingValuesForColumn(int column) => GetPossibleValues(column);

        /// <inheritdoc/>
        protected override int GetNumDimensions(IReadOnlyPuzzle puzzle) => puzzle.Size;
        /// <inheritdoc/>
        protected override int GetDimension(in Coordinate c) => c.Column;
        /// <inheritdoc/>
        protected override void TrackUnsetCoordinatesOnSameDimension(
            int dimension, in Coordinate source, CoordinateTracker tracker)
        {
            Debug.Assert(_puzzle is not null,
                $"Cannot call {nameof(TrackUnsetCoordinatesOnSameDimension)} when puzzle is null.");
            for (int row = 0; row < _puzzle.Size; row++)
            {
                if (row != source.Row && !_puzzle[row, dimension].HasValue)
                {
                    tracker.AddOrTrackIfUntracked(new Coordinate(row, dimension));
                }
            }
        }
    }
}