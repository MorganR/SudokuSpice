using System;
using System.Collections.Generic;

namespace SudokuSpice.Data
{
    /// <summary>
    /// Holds an exact-cover matrix for the current puzzle being solved.
    /// </summary>
    /// <remarks>
    /// The exact cover matrix is organized by <see cref="Square"/>s, which in turn contain
    /// <see cref="PossibleSquareValue"/>s. Each of these represents a row in the exact-cover
    /// matrix. <see cref="SudokuSpice.Constraints.IConstraint">IConstraint</see>s will then add
    /// <see cref="ConstraintHeader"/>s, the columns of the matrix and corresponding
    /// <see cref="SquareLink"/>s.
    /// </remarks>
    /// <seealso cref="https://en.wikipedia.org/wiki/Exact_cover"/>
    public class ExactCoverMatrix
    {
        private readonly int[] _allPossibleValues;
        private readonly Square?[][] _matrix;
        private readonly Dictionary<int, int> _valuesToIndices;
        internal ConstraintHeader? FirstHeader;

        /// <summary>
        /// Contains the possible values for the current puzzle.
        /// </summary>
        public ReadOnlySpan<int> AllPossibleValues => new ReadOnlySpan<int>(_allPossibleValues);

        /// <summary>
        /// Maps possible values for the puzzle to indices in the <see cref="AllPossibleValues"/>
        /// array.
        /// </summary>
        public IReadOnlyDictionary<int, int> ValuesToIndices => _valuesToIndices;

        internal ExactCoverMatrix(IReadOnlyPuzzle puzzle, int[] allPossibleValues)
        {
            _matrix = new Square[puzzle.Size][];
            _allPossibleValues = allPossibleValues;
            _valuesToIndices = new Dictionary<int, int>(allPossibleValues.Length);
            for (int index = 0; index < allPossibleValues.Length; index++)
            {
                _valuesToIndices[allPossibleValues[index]] = index;
            }
            for (int row = 0; row < puzzle.Size; row++)
            {
                var colArray = new Square[puzzle.Size];
                for (int col = 0; col < puzzle.Size; col++)
                {
                    var coord = new Coordinate(row, col);
                    if (!puzzle[in coord].HasValue)
                    {
                        colArray[col] = new Square(coord, allPossibleValues.Length);
                    }
                }
                _matrix[row] = colArray;
            }
        }

        /// <summary>
        /// Gets the square representing the given <see cref="Coordinate"/>. This returns null if
        /// the square's value was preset in the current puzzle being solved.
        /// </summary>
        public Square? GetSquare(in Coordinate c)
        {
            return _matrix[c.Row][c.Column];
        }

        /// <summary>
        /// Gets all the <see cref="Square"/>s on the requested row.
        /// </summary>
        /// <param name="row">A zero-based row index.</param>
        public ReadOnlySpan<Square?> GetSquaresOnRow(int row)
        {
            return new ReadOnlySpan<Square?>(_matrix[row]);
        }

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
            var header = FirstHeader;
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
