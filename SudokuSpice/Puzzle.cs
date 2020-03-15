using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace SudokuSpice
{

    /// <summary>Manages underlying puzzle data.</summary>.
    [SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional")]
    public class Puzzle
    {
        /// <summary>The length of one side of the puzzle.</summary>
        public int Size { get; }
        /// <summary>The length of one side of a mini box within the puzzle.</summary>
        /// <para>A mini box is a square region that must contain each possible value exactly once.</para>
        public int BoxSize { get; }
        /// <summary>The current number of empty/unknown squares in the puzzle.</summary>
        public int NumEmptySquares { get { return _unsetCoordsTracker.NumTracked; } }
        private readonly int?[,] _squares;
        private readonly CoordinateTracker _unsetCoordsTracker;

        public Puzzle(int?[,] puzzleMatrix)
        {
            Size = puzzleMatrix.GetLength(0);
            // Limit to up to 32 possible values (what fits in an int), and ensure that puzzles must
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

            _squares = (int?[,])puzzleMatrix.Clone();
            _unsetCoordsTracker = new CoordinateTracker(Size);
            for (var row = 0; row < Size; row++)
            {
                for (var col = 0; col < Size; col++)
                {
                    if (!_squares[row, col].HasValue)
                    {
                        _unsetCoordsTracker.Add(new Coordinate(row, col));
                    }
                }
            }
        }

        public Puzzle(Puzzle existing)
        {
            Size = existing.Size;
            BoxSize = existing.BoxSize;
            _squares = (int?[,])existing._squares.Clone();
            _unsetCoordsTracker = new CoordinateTracker(existing._unsetCoordsTracker);
        }

        /// <summary>Gets or sets the current value of a given square. A square can be 'unset' by 
        /// setting its value to <c>null</c></summary>
        /// <exception cref="System.InvalidOperationException">Thrown when setting to a non-null 
        /// value, but the specified square already has a value.</exception>
        public int? this[int row, int col]
        {
            get => _squares[row, col];
            set
            {
                if (value.HasValue)
                {
                    _Set(row, col, value.Value);
                }
                else
                {
                    _Unset(row, col);
                }
            }
        }

        [SuppressMessage("Design", "CA1043:Use Integral Or String Argument For Indexers", Justification = "This makes sense with Coordinate, which removes any ambiguity between first and second arguments")]
        public int? this[in Coordinate c]
        {
            get => _squares[c.Row, c.Column];
            set => this[c.Row, c.Column] = value;
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

        /// <summary>Returns the puzzle in a pretty string format, with boxes and rows separated by
        /// pipes and dashes.</summary>
        public override string ToString()
        {
            int maxDigitLength = Size.ToString(NumberFormatInfo.InvariantInfo).Length;
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
                    var numberString =
                        _squares[row, col].HasValue ?
#pragma warning disable CS8629 // Nullable value type may be null.
                        // Protected by the above check.
                        _squares[row, col].Value.ToString(NumberFormatInfo.InvariantInfo) : " ";
#pragma warning restore CS8629 // Nullable value type may be null.
                    int remainingDigits = maxDigitLength - numberString.Length;
                    for (; remainingDigits > 0; remainingDigits--)
                    {
                        strBuild.Append(' ');
                    }
                    strBuild.Append(numberString);
                    if (col % BoxSize == BoxSize - 1)
                    {
                        strBuild.Append('|');
                    }
                    else
                    {
                        strBuild.Append(',');
                    }
                }
                strBuild.Append('\n');
            }
            _AppendBoxDividerRow(strBuild);
            return strBuild.ToString();
        }

        /// <summary>Sets the value of a square.</summary>
        /// <exception cref="System.InvalidOperationException">Thrown when the specified square
        /// already has a value.</exception>
        private void _Set(int row, int col, int val)
        {
            Debug.Assert(!_squares[row, col].HasValue, $"Square ({row}, {col}) already has a value.");
            _squares[row, col] = val;
            _unsetCoordsTracker.Untrack(new Coordinate(row, col));
        }

        /// <summary>Unsets the specified square.</summary>
        private void _Unset(int row, int col)
        {
            Debug.Assert(_squares[row, col].HasValue,
                $"Square ({row}, {col}) doesn't have a value, so can't be unset.");
            _unsetCoordsTracker.Track(new Coordinate(row, col));
            _squares[row, col] = null;
        }

        private void _AppendBoxDividerRow(StringBuilder strBuild)
        {
            int maxDigitLength = Size.ToString(NumberFormatInfo.InvariantInfo).Length;
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
    }
}
