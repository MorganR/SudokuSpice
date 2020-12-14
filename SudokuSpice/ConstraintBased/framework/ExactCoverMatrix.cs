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
    /// <see cref="PossibleValue"/>s. Each of these represents a row in the exact-cover
    /// matrix. <see cref="Constraints.IConstraint"/>s will then add
    /// <see cref="ConstraintHeader"/>s, the columns of the matrix and corresponding links.
    /// </remarks>
    /// <seealso href="https://en.wikipedia.org/wiki/Exact_cover"/>
    public class ExactCoverMatrix
    {
        private readonly int[] _allPossibleValues;
        private readonly Square?[][] _matrix;
        internal ConstraintHeader? FirstHeader;

        /// <summary>
        /// Contains the possible values for the current puzzle.
        /// </summary>
        public ReadOnlySpan<int> AllPossibleValues => new ReadOnlySpan<int>(_allPossibleValues);

        /// <summary>
        /// Maps possible values for the puzzle to indices in the <see cref="AllPossibleValues"/>
        /// array.
        /// </summary>
        public IReadOnlyDictionary<int, int> ValuesToIndices { get; private set; }

        internal ExactCoverMatrix(IPuzzle puzzle)
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
                FirstHeader != null,
                $"Cannot copy a matrix that still has a null {nameof(FirstHeader)}.");
            var copy = new ExactCoverMatrix(this);
            for (int row = 0; row < copy._matrix.Length; row++)
            {
                Square?[]? colArray = _matrix[row];
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
            copy.FirstHeader = FirstHeader.CopyToMatrix(copy);
            ConstraintHeader? copiedHeader = copy.FirstHeader;
            for (ConstraintHeader? nextHeader = FirstHeader.NextHeader; nextHeader != FirstHeader; nextHeader = nextHeader.NextHeader)
            {
                copiedHeader.NextHeader = nextHeader.CopyToMatrix(copy);
                copiedHeader.NextHeader.PreviousHeader = copiedHeader;
                copiedHeader = copiedHeader.NextHeader;
            }
            copiedHeader.NextHeader = copy.FirstHeader;
            copy.FirstHeader.PreviousHeader = copiedHeader;
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
        /// Gets all the currently unsatisfied <see cref="ConstraintHeader"/>s.
        /// </summary>
        public IEnumerable<ConstraintHeader> GetUnsatisfiedConstraintHeaders()
        {
            if (FirstHeader == null)
            {
                yield break;
            }
            ConstraintHeader? header = FirstHeader;
            do
            {
                yield return header;
                header = header.NextHeader;
            } while (header != FirstHeader);
        }

        internal void Attach(ConstraintHeader header)
        {
            if (FirstHeader is null)
            {
                FirstHeader = header;
            } else
            {
                header.NextHeader = FirstHeader;
                header.PreviousHeader = FirstHeader.PreviousHeader;
                FirstHeader.PreviousHeader = header;
                header.PreviousHeader.NextHeader = header;
            }
        }
    }
}