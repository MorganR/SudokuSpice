using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSpice.ConstraintBased
{
    /// <summary>
    /// Holds an exact-cover matrix for the current puzzle being solved.
    /// </summary>
    /// <remarks>
    /// The exact cover matrix is organized by <see cref="Box"/>s, which in turn contain
    /// <see cref="PossibleSquareValue"/>s. Each of these represents a row in the exact-cover
    /// matrix. <see cref="Constraints.IConstraint"/>s will then add
    /// <see cref="Requirement"/>s, the columns of the matrix and corresponding links.
    /// </remarks>
    /// <seealso href="https://en.wikipedia.org/wiki/Exact_cover"/>
    public class ExactCoverMatrix
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

        /// <summary>
        /// Constructs an empty ExactCoverMatrix for solving the given puzzle.
        ///
        /// This matrix is essentially just a single column of row headers until
        /// <see cref="Requirement"/>s are attached. Requirements are necessary to define the
        /// relationships between squares and their possible values.
        ///
        /// Row headers are only created for unset squares in the puzzle.
        /// </summary>
        /// <param name="puzzle">The puzzle to be solved.</param>
        private ExactCoverMatrix(IReadOnlyPuzzle puzzle)
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

        private ExactCoverMatrix(ExactCoverMatrix other)
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

        public static ExactCoverMatrix Create(IReadOnlyPuzzle puzzle)
        {
            var matrix = new ExactCoverMatrix(puzzle);
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

        internal ExactCoverMatrix CopyUnknowns()
        {
            var copy = new ExactCoverMatrix(this);
            foreach (var requiredObjective in _unsatisfiedObjectives)
            {
                var possibilitiesForObjective = _CopyUnknownPossibilities(requiredObjective, copy).ToArray();
                Objective.CreateFullyConnected(
                    copy,
                    possibilitiesForObjective, requiredObjective.CountToSatisfy);
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
        internal ReadOnlySpan<Possibility?[]?> GetPossibilitiesOnRow(int row) =>
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

        private IEnumerable<IPossibility> _CopyUnknownPossibilities(IObjective objective, ExactCoverMatrix puzzleCopy)
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
                    yield return OptionalObjective.CreateWithPossibilities(copiedPossibilities, optionalObjective.TotalCountToSatisfy);
                } else
                {
                    throw new ArgumentException($"Possibilities must be one of {nameof(Possibility)} and {nameof(OptionalObjective)} in order to copy objectives. Received possibility with type: {possibility.GetType().Name}");
                }
            }
        }
    }
}