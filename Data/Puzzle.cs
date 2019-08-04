using System;
using System.Collections.Generic;
using System.Text;

namespace MorganRoff.Sudoku
{
    /// <summary>Manages underlying puzzle data.</summary>
    public class Puzzle
    {
        /// <summary>The length of one side of the puzzle.</summary>
        public readonly int Size;
        /// <summary>The length of one side of a mini box within the puzzle.</summary>
        /// <para>A mini box is a square region that must contain each possible value exactly once.</para>
        public readonly int BoxSize;
        /// <summary>The current number of empty/unknown squares in the puzzle.</summary>
        public int NumEmptySquares { get; private set; }
        private readonly int?[,] _squares;
        private readonly int[,] _possibleSquareValues;
        private readonly CoordinateTracker _unsetCoordsTracker;

        public Puzzle(int?[,] puzzleMatrix)
        {
            Size = puzzleMatrix.GetLength(0);
            // Limit to up to 32 possible values (what fits in an int), including that puzzles must
            // be square.
            if (Size > 25)
            {
                throw new ArgumentException("Max puzzle size is 25.");
            }
            if (Size != puzzleMatrix.GetLength(1))
            {
                throw new ArgumentException("Puzzle must be square.");
            }
            BoxSize = (int)Math.Sqrt(Size);
            if (BoxSize * BoxSize != Size)
            {
                throw new ArgumentException("Puzzle dimensions must be the square of a whole number.");
            }

            _possibleSquareValues = new int[Size, Size];
            _squares = (int?[,])puzzleMatrix.Clone();
            _unsetCoordsTracker = new CoordinateTracker(Size);
            for (var row = 0; row < Size; row++)
            {
                for (var col = 0; col < Size; col++)
                {
                    if (!_squares[row, col].HasValue)
                    {
                        NumEmptySquares++;
                        _possibleSquareValues[row, col] = BitVectorUtils.CreateWithSize(Size);
                        _unsetCoordsTracker.Add(new Coordinate(row, col));
                    } else
                    {
                        BitVectorUtils.SetBit(_squares[row, col].Value - 1, ref _possibleSquareValues[row, col]);
                    }
                }
            }
        }

        /// <summary>Gets the current value of a given square.</summary>
        public int? Get(int row, int col) => _squares[row, col];

        /// <summary>Sets the value of a square.</summary>
        /// <exception cref="System.InvalidOperationException">Thrown when the specified square
        /// already has a value.</exception>
        public void Set(int row, int col, int val)
        {
            if (_squares[row, col].HasValue)
            {
                throw new InvalidOperationException($"Square ({row}, {col}) already has a value.");
            }
            if (!_possibleSquareValues[row, col].IsBitSet(val - 1))
            {
                throw new InvalidOperationException($"Can't set square ({row}, {col}) to value {val}.");
            }

            _squares[row, col] = val;
            _unsetCoordsTracker.Untrack(new Coordinate(row, col));
            NumEmptySquares--;
        }

        /// <summary>Unsets the specified square.</summary>
        public void Unset(int row, int col)
        {
            _unsetCoordsTracker.Track(new Coordinate(row, col));
            _squares[row, col] = null;
            NumEmptySquares++;
        }

        /// <summary>
        /// Gets the possible values for a given square as a bit vector.
        /// </summary>
        /// <returns>The possible values for this coordinate, represented as a bit vector.</returns>
        public int GetPossibleValues(int row, int col) => _possibleSquareValues[row, col];

        /// <summary>
        /// Sets the possible values for a given square as a bit vector.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="vector"></param>
        public void SetPossibleValues(int row, int col, int vector)
        {
            if (_squares[row, col].HasValue
                && !vector.IsBitSet(_squares[row, col].Value - 1))
            {
                throw new ArgumentException(
                    $"Must include the currently set value ({_squares[row, col].Value}) in the possible values for square ({row}, {col}).");
            }
            _possibleSquareValues[row, col] = vector;
        }

        /// <summary>Returns the index of the box that the given coordinates are in.</summary>
        public int GetBoxIndex(int row, int col) => (row / BoxSize) * BoxSize + col / BoxSize;

        /// <summary>Returns the top-left coordinate for the given box.</summary>
        public Coordinate GetStartingBoxCoordinate(int box)
        {
            return new Coordinate((box / BoxSize) * BoxSize, (box % BoxSize) * BoxSize);
        }

        /// <summary>Gets a span of <c>Coordinate</c>s for all the unset squares.</summary>
        public ReadOnlySpan<Coordinate> GetUnsetCoords()
        {
            return _unsetCoordsTracker.GetTrackedCoords();
        }

        /// <summary>Yields an enumerable of <c>Coordinate</c>s for all the unset squares in the given row.</summary>
        public IEnumerable<Coordinate> YieldUnsetCoordsForRow(int row)
        {
            for (var col = 0; col < Size; col++)
            {
                if (_squares[row, col].HasValue)
                {
                    continue;
                }
                yield return new Coordinate(row, col);
            }
        }

        /// <summary>Yields an enumerable of <c>Coordinate</c>s for all the unset squares in the given column.</summary>
        public IEnumerable<Coordinate> YieldUnsetCoordsForColumn(int col)
        {
            for (var row = 0; row < Size; row++)
            {
                if (_squares[row, col].HasValue)
                {
                    continue;
                }
                yield return new Coordinate(row, col);
            }
        }

        /// <summary>Yields an enumerable of <c>Coordinate</c>s for all the unset squares in the given box.</summary>
        public IEnumerable<Coordinate> YieldUnsetCoordsForBox(int box)
        {
            (var startRow, var startCol) = GetStartingBoxCoordinate(box);
            var endRow = startRow + BoxSize;
            var endCol = startCol + BoxSize;
            for (var row = startRow; row < endRow; row++)
            {
                for (var col = startCol; col < endCol; col++)
                {
                    if (_squares[row, col].HasValue)
                    {
                        continue;
                    }
                    yield return new Coordinate(row, col);
                }
            }
        }

        /// <summary>Returns the puzzle in a pretty string format, with boxes and rows separated by '#'.</summary>
        public override string ToString()
        {
            int maxDigitLength = Size.ToString().Length;
            StringBuilder strBuild = new StringBuilder();
            for (int row = 0; row < Size; row++)
            {
                if (row % BoxSize == 0)
                {
                    _AppendBoxDividerRow(strBuild);
                }
                strBuild.Append('|');
                for (int col = 0; col < Size; col++)
                {
                    var numberString = _squares[row, col].HasValue ? _squares[row, col].Value.ToString() : " ";
                    int remainingDigits = maxDigitLength - numberString.Length;
                    for (; remainingDigits > 0; remainingDigits--)
                    {
                        strBuild.Append(' ');
                    }
                    strBuild.Append(numberString);
                    if (col % BoxSize == BoxSize - 1)
                    {
                        strBuild.Append('|');
                    } else
                    {
                        strBuild.Append(',');
                    }
                }
                strBuild.Append('\n');
            }
            _AppendBoxDividerRow(strBuild);
            return strBuild.ToString();
        }

        #region ToString

        private void _AppendBoxDividerRow(StringBuilder strBuild)
        {
            int maxDigitLength = Size.ToString().Length;
            for (int col = 0; col < Size; col++)
            {
                if (col % BoxSize == 0)
                {
                    strBuild.Append("+");
                }
                else
                {
                    strBuild.Append("-");
                }
                for (int numCharsToAppend = maxDigitLength; numCharsToAppend > 0; numCharsToAppend--)
                {
                    strBuild.Append("-");
                }
            }
            strBuild.Append("+\n");
        }

        #endregion
    }
}
