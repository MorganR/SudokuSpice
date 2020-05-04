using System;
using System.Collections.Generic;

namespace SudokuSpice.Data
{
    public class ExactCoverMatrix
    {
        private readonly Square[][] _matrix;
        private readonly int[] _allPossibleValues;
        internal ConstraintHeader? FirstHeader;

        public ReadOnlySpan<int> AllPossibleValues => new ReadOnlySpan<int>(_allPossibleValues);

        public ExactCoverMatrix(int puzzleSize, int[] allPossibleValues)
        {
            _matrix = new Square[puzzleSize][];
            _allPossibleValues = allPossibleValues;
            for (int row = 0; row < puzzleSize; row++)
            {
                var colArray = new Square[puzzleSize];
                for (int col = 0; col < puzzleSize; col++)
                {
                    colArray[col] = new Square(new Coordinate(row, col), allPossibleValues.Length);
                }
                _matrix[row] = colArray;
            }
        }

        public Square GetSquare(in Coordinate c)
        {
            return _matrix[c.Row][c.Column];
        }

        public ReadOnlySpan<Square> GetSquaresOnRow(int row)
        {
            return new ReadOnlySpan<Square>(_matrix[row]);
        }

        public List<Square> GetSquaresOnColumn(int column)
        {
            var squares = new List<Square>(_matrix.Length);
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
    }
}
