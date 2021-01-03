using System.Diagnostics;

namespace SudokuSpice.RuleBased.Rules
{
    /// <summary>
    /// Restricts that each column contains all unique values.
    /// </summary>
    public class ColumnUniquenessRule : UniquenessRule, IMissingColumnValuesTracker
    {
        private IReadOnlyPuzzle? _puzzle;

        public ColumnUniquenessRule() : base() { }

        private ColumnUniquenessRule(ColumnUniquenessRule existing, IReadOnlyPuzzle? puzzle)
            : base(existing)
        {
            _puzzle = puzzle;
        }

        /// <inheritdoc/>
        public override IRule CopyWithNewReference(IReadOnlyPuzzle? puzzle) =>
            new ColumnUniquenessRule(this, puzzle);

        /// <inheritdoc/>
        public override bool TryInit(IReadOnlyPuzzle puzzle, BitVector allPossibleValues)
        {
            if (!base.TryInit(puzzle, allPossibleValues))
            {
                return false;
            }
            _puzzle = puzzle;
            return true;
        }

        /// <inheritdoc/>
        public BitVector GetMissingValuesForColumn(int col) => GetPossibleValues(col);

        /// <inheritdoc/>
        protected override int GetNumDimensions(IReadOnlyPuzzle puzzle) => puzzle.Size;
        /// <inheritdoc/>
        protected override int GetDimension(in Coordinate c) => c.Column;
        /// <inheritdoc/>
        protected override void TrackUnsetCoordinatesOnSameDimension(
            int dimension, in Coordinate source, CoordinateTracker tracker)
        {
            Debug.Assert(_puzzle is not null, $"Cannot call {nameof(TrackUnsetCoordinatesOnSameDimension)} when puzzle is null.");
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