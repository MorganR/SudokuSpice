using System;
using System.Collections.Generic;

namespace SudokuSpice.Data
{
    public class ExactCoverMatrix
    {
        private readonly Square[][] _matrix;
        private readonly List<ConstraintHeader> _constraintHeaders;

        public IReadOnlyList<int> AllPossibleValues { get; }
        public IReadOnlyList<ConstraintHeader> ConstraintHeaders => _constraintHeaders;

        public ExactCoverMatrix(int puzzleSize, IReadOnlyList<int> allPossibleValues)
        {
            _matrix = new Square[puzzleSize][];
            AllPossibleValues = allPossibleValues;
            var valuesToIndices = new Dictionary<int, int>(allPossibleValues.Count);
            for (int i = 0; i < allPossibleValues.Count; i++)
            {
                valuesToIndices[allPossibleValues[i]] = i;
            }
            for (int row = 0; row < puzzleSize; row++)
            {
                var colArray = new Square[puzzleSize];
                for (int col = 0; col < puzzleSize; col++)
                {
                    colArray[col] = new Square(new Coordinate(row, col), valuesToIndices);
                }
                _matrix[row] = colArray;
            }
            _constraintHeaders = new List<ConstraintHeader>();
        }

        public void AddConstraintHeader(ConstraintHeader constraint)
        {
            _constraintHeaders.Add(constraint);
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
    }
}
