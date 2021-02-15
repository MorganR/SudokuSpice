using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSpice.ConstraintBased
{
    /// <summary>
    /// Holds an exact-cover graph for the current puzzle being solved.
    /// 
    /// This is an extension of the
    /// <a href="https://en.wikipedia.org/wiki/Exact_cover">exact-cover matrix</a> concept. Rows of
    /// the exact-cover matrix, i.e. a specific location and possible value for that location, are
    /// represented by <see cref="Possibility"/> objects. These are linked together by
    /// <see cref="IObjective"/> objects, which represent the columns of an exact-cover matrix.
    /// 
    /// For example, the <see cref="Constraints.RowUniquenessConstraint"/> uses a required 
    /// <see cref="Objective"/> to link all the <see cref="Possibility"/> objects on a single row
    /// that have the same possible value. This way, when one of these possibilities is selected,
    /// then the others are all dropped.
    ///
    /// To extend the concept into a larger graph, this also uses <see cref="OptionalObjective"/>s
    /// to create subgroups over <see cref="Possibility"/> objects and/or other
    /// <see cref="OptionalObjective"/> objects. This allows for much more complex constraints,
    /// such as the <see cref="Constraints.MagicSquaresConstraint"/>.
    /// </summary>
    /// <remarks>
    /// The ExactCoverGraph adds <see cref="Possibility"/> objects for all the unset coordinates
    /// in a puzzle on creation, as well as <see cref="Objective"/> objects that group all the
    /// possible values for each location. These effectively implements the constraint: "Each
    /// coordinate in the puzzle must have one and only one value."
    /// </remarks>
    /// <seealso href="https://en.wikipedia.org/wiki/Exact_cover"/>
    public class ExactCoverGraph
    {
        private readonly int[] _allPossibleValues;
        private readonly Possibility?[]?[][] _matrix;
        private readonly LinkedList<Objective> _unsatisfiedObjectives;

        /// <summary>
        /// Contains the possible values for the current puzzle.
        /// </summary>
        public ReadOnlySpan<int> AllPossibleValues => new ReadOnlySpan<int>(_allPossibleValues);

        /// <summary>
        /// Maps possible values for the puzzle to indices in the <see cref="AllPossibleValues"/>
        /// array.
        /// </summary>
        public IReadOnlyDictionary<int, int> ValuesToIndices { get; }

        private ExactCoverGraph(IReadOnlyPuzzle puzzle)
        {
            int size = puzzle.Size;
            _matrix = new Possibility[size][][];
            for (int rowIndex = 0; rowIndex < size; ++rowIndex)
            {
                _matrix[rowIndex] = new Possibility[size][];
            }
            _allPossibleValues = puzzle.AllPossibleValuesSpan.ToArray();
            _unsatisfiedObjectives = new LinkedList<Objective>();
            var valuesToIndices = new Dictionary<int, int>(_allPossibleValues.Length);
            for (int index = 0; index < _allPossibleValues.Length; index++)
            {
                valuesToIndices[_allPossibleValues[index]] = index;
            }
            ValuesToIndices = valuesToIndices;
        }

        private ExactCoverGraph(ExactCoverGraph other)
        {
            int length = other._matrix.Length;
            _matrix = new Possibility[length][][];
            for (int rowIndex = 0; rowIndex < length; ++rowIndex)
            {
                _matrix[rowIndex] = new Possibility[length][];
            }
            _allPossibleValues = other.AllPossibleValues.ToArray();
            _unsatisfiedObjectives = new LinkedList<Objective>();
            ValuesToIndices = other.ValuesToIndices;
        }

        /// <summary>
        /// Creates an exact-cover graph for solving the given puzzle.
        ///
        /// This adds <see cref="Possibility"/> objects for all the unset coordinates in a puzzle on
        /// creation, as well as <see cref="Objective"/> objects that group all the possible values
        /// for each location. These effectively implements the constraint: "Each square in the
        /// puzzle must have one and only one value."
        /// </summary>
        /// <param name="puzzle">The puzzle to solve.</param>
        public static ExactCoverGraph Create(IReadOnlyPuzzle puzzle)
        {
            var matrix = new ExactCoverGraph(puzzle);
            int size = puzzle.Size;
            for (int rowIndex = 0; rowIndex < size; rowIndex++)
            {
                var possibilitiesRow = matrix._matrix[rowIndex];
                for (int columnIndex = 0; columnIndex < size; columnIndex++)
                {
                    var coord = new Coordinate(rowIndex, columnIndex);
                    if (!puzzle[in coord].HasValue)
                    {
                        var possibilitiesForSquare = matrix._allPossibleValues.Select((_, index) => new Possibility(coord, index)).ToArray();
                        possibilitiesRow[columnIndex] = possibilitiesForSquare;
                        // Enforce that all squares need to have a value.
                        Objective.CreateFullyConnected(matrix, possibilitiesForSquare, 1);
                    }
                }
            }
            return matrix;
        }

        internal ExactCoverGraph CopyUnknowns()
        {
            var copy = new ExactCoverGraph(this);
            foreach (var requiredObjective in _unsatisfiedObjectives)
            {
                var possibilitiesForObjective = _CopyUnknownPossibilities(requiredObjective, copy).ToArray();
                Objective.CreateFullyConnected(
                    copy,
                    possibilitiesForObjective, requiredObjective.TotalCountToSatisfy);
            }
            return copy;
        }

        /// <summary>
        /// Gets the possibilities at the given <see cref="Coordinate"/>. This returns null if
        /// the square's value was preset in the current puzzle being solved.
        /// </summary>
        public Possibility?[]? GetAllPossibilitiesAt(in Coordinate c) => _matrix[c.Row][c.Column];

        /// <summary>
        /// Gets all the currently unsatisfied <see cref="Objective"/>s.
        /// </summary>
        public IEnumerable<Objective> GetUnsatisfiedRequiredObjectives() => _unsatisfiedObjectives;

        /// <summary>
        /// Gets all the possiblities, grouped by column, on the requested row.
        ///
        /// Indexing the result looks like:
        ///
        /// <c>
        /// var row = matrix.GetSquaresOnRow(rowIndex);
        /// var possibility = row[columnIndex][valueIndex];
        /// </c>
        /// </summary>
        /// <param name="row">A zero-based row index.</param>
        public ReadOnlySpan<Possibility?[]?> GetPossibilitiesOnRow(int row) =>
            new ReadOnlySpan<Possibility?[]?>(_matrix[row]);

        internal LinkedListNode<Objective> AttachObjective(Objective objective)
        {
            return _unsatisfiedObjectives.AddLast(objective);
        }

        internal void DetachObjective(LinkedListNode<Objective> node)
        {
            _unsatisfiedObjectives.Remove(node);
        }

        internal void ReattachObjective(LinkedListNode<Objective> node)
        {
            _unsatisfiedObjectives.AddLast(node);
        }

        private IEnumerable<IPossibility> _CopyUnknownPossibilities(IObjective objective, ExactCoverGraph puzzleCopy)
        {
            var length = _matrix.Length;
            foreach (var possibility in objective.GetUnknownDirectPossibilities())
            {
                if (possibility is Possibility concretePossibility)
                {
                    if (concretePossibility.State != NodeState.UNKNOWN)
                    {
                        continue;
                    }

                    var coord = concretePossibility.Coordinate;
                    var copiedRow = puzzleCopy._matrix[coord.Row];
                    var copiedSquare = copiedRow[coord.Column];
                    if (copiedSquare is null)
                    {
                        copiedSquare = new Possibility[_allPossibleValues.Length];
                        copiedRow[coord.Column] = copiedSquare;
                    }
                    
                    var copiedPossibility = copiedSquare[concretePossibility.Index];
                    if (copiedPossibility is null)
                    {
                        copiedPossibility = new Possibility(coord, concretePossibility.Index);
                        copiedSquare[concretePossibility.Index] = copiedPossibility;
                    }
                    yield return copiedPossibility;
                } else if (possibility is OptionalObjective optionalObjective)
                {
                    if (optionalObjective.State != NodeState.UNKNOWN)
                    {
                        continue;
                    }
                    var copiedPossibilities = _CopyUnknownPossibilities(optionalObjective, puzzleCopy).ToArray();
                    yield return OptionalObjective.CreateWithPossibilities(copiedPossibilities, optionalObjective.RemainingCountToSatisfy);
                } else
                {
                    throw new ArgumentException($"Possibilities must be one of {nameof(Possibility)} and {nameof(OptionalObjective)} in order to copy objectives. Received possibility with type: {possibility.GetType().Name}");
                }
            }
        }
    }
}