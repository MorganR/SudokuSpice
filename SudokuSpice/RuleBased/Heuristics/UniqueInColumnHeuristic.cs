using SudokuSpice.RuleBased.Rules;
using System.Collections.Generic;
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
    public class UniqueInColumnHeuristic : UniqueInXHeuristic
    {
        private readonly IMissingColumnValuesTracker _columnTracker;

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
            : base(existing, puzzle)
        {
            _columnTracker = rule;
        }

        /// <summary>
        /// Creates a deep copy of this heuristic. Requires <c>rules</c> to contain an
        /// <see cref="IMissingColumnValuesTracker"/>.
        /// </summary>
        public override ISudokuHeuristic CopyWithNewReferences(
            IReadOnlyPuzzleWithMutablePossibleValues? puzzle,
            IReadOnlyList<ISudokuRule> rules)
        {
            return new UniqueInColumnHeuristic(
                this, puzzle,
                (IMissingColumnValuesTracker)rules.First(r => r is IMissingColumnValuesTracker));
        }

        protected override int GetNumDimensions(IReadOnlyPuzzle puzzle) => puzzle.Size;

        protected override BitVector GetMissingValuesForDimension(int dimension, IReadOnlyPuzzle _) => _columnTracker.GetMissingValuesForColumn(dimension);

        protected override IEnumerable<Coordinate> GetUnsetCoordinatesOnDimension(int dimension, IReadOnlyPuzzle puzzle)
        {
            int size = puzzle.Size;
            for (int row = 0; row < size; ++row)
            {
                if (!puzzle[row, dimension].HasValue)
                {
                    yield return new Coordinate(row, dimension);
                }
            }
        }
    }
}
