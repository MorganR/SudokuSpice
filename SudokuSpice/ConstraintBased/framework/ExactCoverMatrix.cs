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
    /// matrix. <see cref="SudokuSpice.ConstraintBased.Constraints.IConstraint">IConstraint</see>s will then add
    /// <see cref="ConstraintHeader"/>s, the columns of the matrix and corresponding links.
    /// </remarks>
    /// <seealso href="https://en.wikipedia.org/wiki/Exact_cover"/>
    public class ExactCoverMatrix<TPuzzle> where TPuzzle : IReadOnlyPuzzle
    {
        private readonly int[] _allPossibleValues;
        private readonly Square<TPuzzle>?[][] _matrix;
        internal ConstraintHeader<TPuzzle>? FirstHeader;

        /// <summary>
        /// Contains the possible values for the current puzzle.
        /// </summary>
        public ReadOnlySpan<int> AllPossibleValues => new ReadOnlySpan<int>(_allPossibleValues);

        /// <summary>
        /// Maps possible values for the puzzle to indices in the <see cref="AllPossibleValues"/>
        /// array.
        /// </summary>
        public IReadOnlyDictionary<int, int> ValuesToIndices { get; private set; }

        internal ExactCoverMatrix(TPuzzle puzzle)
        {
            _matrix = new Square<TPuzzle>[puzzle.Size][];
            _allPossibleValues = puzzle.AllPossibleValues.ToArray();
            var valuesToIndices = new Dictionary<int, int>(_allPossibleValues.Length);
            for (int index = 0; index < _allPossibleValues.Length; index++)
            {
                valuesToIndices[_allPossibleValues[index]] = index;
            }
            ValuesToIndices = valuesToIndices;
            for (int row = 0; row < puzzle.Size; row++)
            {
                var colArray = new Square<TPuzzle>[puzzle.Size];
                for (int col = 0; col < puzzle.Size; col++)
                {
                    var coord = new Coordinate(row, col);
                    if (!puzzle[in coord].HasValue)
                    {
                        colArray[col] = new Square<TPuzzle>(coord, _allPossibleValues.Length);
                    }
                }
                _matrix[row] = colArray;
            }
        }

        private ExactCoverMatrix(ExactCoverMatrix<TPuzzle> other)
        {
            _matrix = new Square<TPuzzle>[other._matrix.Length][];
            _allPossibleValues = other.AllPossibleValues.ToArray();
            ValuesToIndices = other.ValuesToIndices;
        }

        internal ExactCoverMatrix<TPuzzle> CopyUnknowns()
        {
            Debug.Assert(
                FirstHeader != null, 
                $"Cannot copy a matrix that still has a null {nameof(FirstHeader)}.");
            var copy = new ExactCoverMatrix<TPuzzle>(this);
            for (int row = 0; row < copy._matrix.Length; row++)
            {
                var colArray = _matrix[row];
                var copyColArray = new Square<TPuzzle>[colArray.Length];
                for (int col = 0; col < copyColArray.Length; col++)
                {
                    var square = colArray[col];
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
            var copiedHeader = copy.FirstHeader;
            for (var nextHeader = FirstHeader.NextHeader; nextHeader != FirstHeader; nextHeader = nextHeader.NextHeader)
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
        public Square<TPuzzle>? GetSquare(in Coordinate c)
        {
            return _matrix[c.Row][c.Column];
        }

        /// <summary>
        /// Gets all the <see cref="Square"/>s on the requested row.
        /// </summary>
        /// <param name="row">A zero-based row index.</param>
        public ReadOnlySpan<Square<TPuzzle>?> GetSquaresOnRow(int row)
        {
            return new ReadOnlySpan<Square<TPuzzle>?>(_matrix[row]);
        }

        /// <summary>
        /// Gets all the <see cref="Square"/>s on the requested column.
        /// </summary>
        /// <param name="column">A zero-based column index.</param>
        public List<Square<TPuzzle>?> GetSquaresOnColumn(int column)
        {
            var squares = new List<Square<TPuzzle>?>(_matrix.Length);
            for (int row = 0; row < _matrix.Length; row++)
            {
                squares.Add(_matrix[row][column]);
            }
            return squares;
        }

        /// <summary>
        /// Gets all the currently unsatisfied <see cref="ConstraintHeader"/>s.
        /// </summary>
        public IEnumerable<ConstraintHeader<TPuzzle>> GetUnsatisfiedConstraintHeaders()
        {
            if (FirstHeader == null)
            {
                yield break;
            }
            var header = FirstHeader;
            do
            {
                yield return header;
                header = header.NextHeader;
            } while (header != FirstHeader);
        }

        internal void Attach(ConstraintHeader<TPuzzle> header)
        {
            if (FirstHeader is null)
            {
                FirstHeader = header;
            }
            else
            {
                header.NextHeader = FirstHeader;
                header.PreviousHeader = FirstHeader.PreviousHeader;
                FirstHeader.PreviousHeader = header;
                header.PreviousHeader.NextHeader = header;
            }
        }
    }
}
