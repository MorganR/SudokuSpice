using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

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
    public sealed class ExactCoverGraph
    {
        private readonly int[] _allPossibleValues;
        private readonly Possibility?[]?[][] _possibilities;
        private Objective? _firstUnsatisfiedObjectiveWithConcretePossibilities;
        private Objective? _firstUnsatisfiedObjectiveWithoutConcretePossibilities;

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
            _possibilities = new Possibility[size][][];
            for (int rowIndex = 0; rowIndex < size; ++rowIndex)
            {
                _possibilities[rowIndex] = new Possibility[size][];
            }
            _allPossibleValues = puzzle.AllPossibleValuesSpan.ToArray();
            var valuesToIndices = new Dictionary<int, int>(_allPossibleValues.Length);
            for (int index = 0; index < _allPossibleValues.Length; index++)
            {
                valuesToIndices[_allPossibleValues[index]] = index;
            }
            ValuesToIndices = valuesToIndices;
        }

        private ExactCoverGraph(ExactCoverGraph other)
        {
            int length = other._possibilities.Length;
            _possibilities = new Possibility[length][][];
            for (int rowIndex = 0; rowIndex < length; ++rowIndex)
            {
                _possibilities[rowIndex] = new Possibility[length][];
            }
            _allPossibleValues = other.AllPossibleValues.ToArray();
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
            var graph = new ExactCoverGraph(puzzle);
            int size = puzzle.Size;
            int numValues = graph._allPossibleValues.Length;
            for (int rowIndex = 0; rowIndex < size; rowIndex++)
            {
                var possibilitiesRow = graph._possibilities[rowIndex];
                for (int columnIndex = 0; columnIndex < size; columnIndex++)
                {
                    var coord = new Coordinate(rowIndex, columnIndex);
                    if (!puzzle[in coord].HasValue)
                    {
                        var possibilitiesForSquare = _CreatePossibilitiesForSquare(in coord, numValues);
                        possibilitiesRow[columnIndex] = possibilitiesForSquare;
                        // Enforce that all squares need to have a value.
                        Objective.CreateFullyConnected(graph, new ReadOnlySpan<Possibility>(possibilitiesForSquare), 1);
                    }
                }
            }
            return graph;
        }

        internal ExactCoverGraph CopyUnknowns()
        {
            var copy = new ExactCoverGraph(this);
            foreach (var requiredObjective in _firstUnsatisfiedObjectiveWithConcretePossibilities!.GetConnectedObjectives())
            {
                var possibilitiesForObjective = _CopyUnknownPossibilities(requiredObjective, copy).ToArray();
                Objective.CreateFullyConnected(
                    copy,
                    possibilitiesForObjective, requiredObjective.TotalCountToSatisfy);
            }
            if (_firstUnsatisfiedObjectiveWithoutConcretePossibilities is not null)
            {
                foreach (var requiredObjective in _firstUnsatisfiedObjectiveWithoutConcretePossibilities.GetConnectedObjectives())
                {
                    var possibilitiesForObjective = _CopyUnknownPossibilities(requiredObjective, copy).ToArray();
                    Objective.CreateFullyConnected(
                        copy,
                        possibilitiesForObjective, requiredObjective.TotalCountToSatisfy);
                }
            }
            return copy;
        }

        /// <summary>
        /// Gets the possibilities at the given <see cref="Coordinate"/>. This returns null if
        /// the square's value was preset in the current puzzle being solved.
        /// </summary>
        public Possibility?[]? GetAllPossibilitiesAt(in Coordinate c) => _possibilities[c.Row][c.Column];

        /// <summary>
        /// Gets all the currently unsatisfied <see cref="Objective"/>s that have at least one
        /// concrete <see cref="Possibility"/> as a direct possibility.
        /// </summary>
        public IEnumerable<Objective> GetUnsatisfiedRequiredObjectivesWithConcretePossibilities()
        {
            return _firstUnsatisfiedObjectiveWithConcretePossibilities!.GetConnectedObjectives();
        }

        /// <summary>
        /// Gets all the currently unsatisfied <see cref="Objective"/>s.
        /// </summary>
        public IEnumerable<Objective> GetUnsatisfiedRequiredObjectives()
        {
            var withConcretePossibilities = _firstUnsatisfiedObjectiveWithConcretePossibilities!.GetConnectedObjectives();
            if (_firstUnsatisfiedObjectiveWithoutConcretePossibilities is null)
            {
                return withConcretePossibilities;
            }
            return withConcretePossibilities.Concat(
                _firstUnsatisfiedObjectiveWithoutConcretePossibilities.GetConnectedObjectives());
        }

        /// <summary>
        /// Gets all the possiblities, grouped by column, on the requested row.
        ///
        /// Indexing the result looks like:
        ///
        /// <c>
        /// var row = graph.GetSquaresOnRow(rowIndex);
        /// var possibility = row[columnIndex][valueIndex];
        /// </c>
        /// </summary>
        /// <param name="row">A zero-based row index.</param>
        public ReadOnlySpan<Possibility?[]?> GetPossibilitiesOnRow(int row) =>
            new ReadOnlySpan<Possibility?[]?>(_possibilities[row]);

        [MethodImpl(
            MethodImplOptions.AggressiveInlining |
            MethodImplOptions.AggressiveOptimization)]
        internal void AttachObjective(Objective objective)
        {
            if (objective.AtLeastOnePossibilityIsConcrete)
            {
                if (_firstUnsatisfiedObjectiveWithConcretePossibilities is null)
                {
                    _firstUnsatisfiedObjectiveWithConcretePossibilities = objective;
                } else
                {
                    objective.PrependToGraphBefore(_firstUnsatisfiedObjectiveWithConcretePossibilities);
                }
            } else
            {
                if (_firstUnsatisfiedObjectiveWithoutConcretePossibilities is null)
                {
                    _firstUnsatisfiedObjectiveWithoutConcretePossibilities = objective;
                } else
                {
                    objective.PrependToGraphBefore(_firstUnsatisfiedObjectiveWithoutConcretePossibilities);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void DetachObjective(Objective toDetach)
        {
            toDetach.PopFromGraph();
            if (toDetach.AtLeastOnePossibilityIsConcrete)
            {
                if (_firstUnsatisfiedObjectiveWithConcretePossibilities == toDetach)
                {
                    _firstUnsatisfiedObjectiveWithConcretePossibilities = toDetach.NextObjectiveInGraph;
                }
            } else
            {
                if (_firstUnsatisfiedObjectiveWithoutConcretePossibilities == toDetach)
                {
                    _firstUnsatisfiedObjectiveWithoutConcretePossibilities = toDetach.NextObjectiveInGraph;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void ReattachObjective(Objective toReattach)
        {
            toReattach.ReinsertToGraph();
        }

        private IEnumerable<IPossibility> _CopyUnknownPossibilities(IObjective objective, ExactCoverGraph puzzleCopy)
        {
            var length = _possibilities.Length;
            foreach (var possibility in objective.GetUnknownDirectPossibilities())
            {
                if (possibility is Possibility concretePossibility)
                {
                    if (concretePossibility.State != NodeState.UNKNOWN)
                    {
                        continue;
                    }

                    var coord = concretePossibility.Coordinate;
                    var copiedRow = puzzleCopy._possibilities[coord.Row];
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

        [MethodImpl(
            MethodImplOptions.AggressiveInlining |
            MethodImplOptions.AggressiveOptimization)]
        private static Possibility[] _CreatePossibilitiesForSquare(in Coordinate location, int numToCreate)
        {
            var possibilities = new Possibility[numToCreate];
            for (int i = 0; i < possibilities.Length; ++i)
            {
                possibilities[i] = new Possibility(location, i);
            }
            return possibilities;
        }
    }
}