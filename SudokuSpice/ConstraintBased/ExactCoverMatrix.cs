using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SudokuSpice.ConstraintBased
{
    /// <summary>
    /// Holds an exact-cover matrix for the current puzzle being solved.
    /// </summary>
    /// <remarks>
    /// The exact cover matrix is organized by <see cref="Square"/>s, which in turn contain
    /// <see cref="PossibleSquareValue"/>s. Each of these represents a row in the exact-cover
    /// matrix. <see cref="Constraints.IConstraint"/>s will then add
    /// <see cref="Requirement"/>s, the columns of the matrix and corresponding links.
    /// </remarks>
    /// <seealso href="https://en.wikipedia.org/wiki/Exact_cover"/>
    public class ExactCoverMatrix
    {
        private readonly int[] _allPossibleValues;
        private readonly Square?[][] _matrix;
        internal Requirement? FirstRequirement;

        /// <summary>
        /// Contains the possible values for the current puzzle.
        /// </summary>
        public ReadOnlySpan<int> AllPossibleValues => new ReadOnlySpan<int>(_allPossibleValues);

        /// <summary>
        /// Maps possible values for the puzzle to indices in the <see cref="AllPossibleValues"/>
        /// array.
        /// </summary>
        public IReadOnlyDictionary<int, int> ValuesToIndices { get; private set; }

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
        public ExactCoverMatrix(IReadOnlyPuzzle puzzle)
        {
            _matrix = new Square[puzzle.Size][];
            _allPossibleValues = puzzle.AllPossibleValuesSpan.ToArray();
            var valuesToIndices = new Dictionary<int, int>(_allPossibleValues.Length);
            for (int index = 0; index < _allPossibleValues.Length; index++)
            {
                valuesToIndices[_allPossibleValues[index]] = index;
            }
            ValuesToIndices = valuesToIndices;
            for (int row = 0; row < puzzle.Size; row++)
            {
                var colArray = new Square[puzzle.Size];
                for (int col = 0; col < puzzle.Size; col++)
                {
                    var coord = new Coordinate(row, col);
                    if (!puzzle[in coord].HasValue)
                    {
                        colArray[col] = new Square(coord, _allPossibleValues.Length);
                    }
                }
                _matrix[row] = colArray;
            }
        }

        private ExactCoverMatrix(ExactCoverMatrix other)
        {
            _matrix = new Square[other._matrix.Length][];
            _allPossibleValues = other.AllPossibleValues.ToArray();
            ValuesToIndices = other.ValuesToIndices;
        }

        internal ExactCoverMatrix CopyUnknowns()
        {
            Debug.Assert(
                FirstRequirement != null,
                $"Cannot copy a matrix that still has a null {nameof(FirstRequirement)}.");
            var copy = new ExactCoverMatrix(this);
            for (int row = 0; row < copy._matrix.Length; row++)
            {
                Square?[] colArray = _matrix[row];
                var copyColArray = new Square[colArray.Length];
                for (int col = 0; col < copyColArray.Length; col++)
                {
                    Square? square = colArray[col];
                    if (square is null
                        || square.IsSet)
                    {
                        continue;
                    }
                    copyColArray[col] = square.CopyWithPossibleValues();
                }
                copy._matrix[row] = copyColArray;
            }
            copy.FirstRequirement = FirstRequirement.CopyToMatrix(copy);
            Requirement copiedRequirement = copy.FirstRequirement;
            for (Requirement nextRequirement = FirstRequirement.Next; nextRequirement != FirstRequirement; nextRequirement = nextRequirement.Next)
            {
                copiedRequirement.Append(nextRequirement.CopyToMatrix(copy));
                copiedRequirement = copiedRequirement.Next;
            }
            return copy;
        }

        /// <summary>
        /// Gets the square representing the given <see cref="Coordinate"/>. This returns null if
        /// the square's value was preset in the current puzzle being solved.
        /// </summary>
        public Square? GetSquare(in Coordinate c) => _matrix[c.Row][c.Column];

        /// <summary>
        /// Gets all the <see cref="Square"/>s on the requested row.
        /// </summary>
        /// <param name="row">A zero-based row index.</param>
        public ReadOnlySpan<Square?> GetSquaresOnRow(int row) => new ReadOnlySpan<Square?>(_matrix[row]);

        /// <summary>
        /// Gets all the <see cref="Square"/>s on the requested column.
        /// </summary>
        /// <param name="column">A zero-based column index.</param>
        public List<Square?> GetSquaresOnColumn(int column)
        {
            var squares = new List<Square?>(_matrix.Length);
            for (int row = 0; row < _matrix.Length; row++)
            {
                squares.Add(_matrix[row][column]);
            }
            return squares;
        }

        /// <summary>
        /// Gets all the currently unsatisfied <see cref="Requirement"/>s.
        /// </summary>
        public IEnumerable<Requirement> GetUnsatisfiedRequirements()
        {
            if (FirstRequirement == null)
            {
                yield break;
            }
            Requirement requirement = FirstRequirement;
            do
            {
                yield return requirement;
                requirement = requirement.Next;
            } while (requirement != FirstRequirement);
        }

        internal void Attach(Requirement requirement)
        {
            if (FirstRequirement is null)
            {
                FirstRequirement = requirement;
            } else
            {
                FirstRequirement.Prepend(requirement);
            }
        }
    }
}