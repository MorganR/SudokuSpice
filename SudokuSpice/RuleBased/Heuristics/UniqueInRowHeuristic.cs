using SudokuSpice.RuleBased.Rules;
using System.Collections.Generic;
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
    public class UniqueInRowHeuristic : UniqueInXHeuristic
    {
        private readonly IMissingRowValuesTracker _rowTracker;

        /// <summary>
        /// Creates the heuristic.
        /// </summary>
        /// <param name="rowValuesTracker">
        /// Something that tracks the possible values for each row. Rules often do this already,
        /// for example.
        /// </param>
        public UniqueInRowHeuristic(IMissingRowValuesTracker rowValuesTracker) : base()
        {
            _rowTracker = rowValuesTracker;
        }

        private UniqueInRowHeuristic(
            UniqueInRowHeuristic existing,
            IReadOnlyPuzzleWithMutablePossibleValues? puzzle,
            IMissingRowValuesTracker rule)
            : base(existing, puzzle)
        {
            _rowTracker = rule;
        }

        /// <summary>
        /// Creates a deep copy of this heuristic. Requires <c>rules</c> to contain an
        /// <see cref="IMissingRowValuesTracker"/>.
        /// </summary>
        public override ISudokuHeuristic CopyWithNewReferences(
            IReadOnlyPuzzleWithMutablePossibleValues? puzzle, IReadOnlyList<ISudokuRule> rules)
        {
            return new UniqueInRowHeuristic(
                this, puzzle,
                (IMissingRowValuesTracker)rules.First(r => r is IMissingRowValuesTracker));
        }

        protected override int GetNumDimensions(IReadOnlyPuzzle puzzle) => puzzle.Size;
        protected override BitVector GetMissingValuesForDimension(int row, IReadOnlyPuzzle _) => _rowTracker.GetMissingValuesForRow(row);
        protected override IEnumerable<Coordinate> GetUnsetCoordinatesOnDimension(int row, IReadOnlyPuzzle puzzle)
        {
            int size = puzzle.Size;
            for (int col = 0; col < size; ++col)
            {
                if (!puzzle[row, col].HasValue)
                {
                    yield return new Coordinate(row, col);
                }
            }
        }
    }
}