using System;
using System.Collections.Generic;

namespace SudokuSpice.Data
{
    public class ExactCoverMatrix
    {
        private readonly int[] _allPossibleValues;
        private readonly Square?[][] _matrix;
        private readonly Dictionary<int, int> _valuesToIndices;
        internal ConstraintHeader? FirstHeader;

        public ReadOnlySpan<int> AllPossibleValues => new ReadOnlySpan<int>(_allPossibleValues);

        public IReadOnlyDictionary<int, int> ValuesToIndices => _valuesToIndices;

        public ExactCoverMatrix(IReadOnlyPuzzle puzzle, int[] allPossibleValues)
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

        public Square? GetSquare(in Coordinate c)
        {
            return _matrix[c.Row][c.Column];
        }

        public ReadOnlySpan<Square?> GetSquaresOnRow(int row)
        {
            return new ReadOnlySpan<Square?>(_matrix[row]);
        }

        public List<Square?> GetSquaresOnColumn(int column)
        {
            var squares = new List<Square?>(_matrix.Length);
            for (int row = 0; row < _matrix.Length; row++)
            {
                squares.Add(_matrix[row][column]);
            }
            return squares;
        }

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
