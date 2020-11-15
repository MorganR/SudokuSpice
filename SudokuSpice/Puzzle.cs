using SudokuSpice.Data;
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
    public class Puzzle : IPuzzle, IReadOnlyBoxPuzzle
    {
        private readonly int?[,] _squares;
        private readonly CoordinateTracker _unsetCoordsTracker;
        private readonly int[] _allPossibleValues;

        /// <summary>The length of one side of the puzzle.</summary>
        public int Size { get; }
        /// <summary>The length of one side of a mini box within the puzzle.</summary>
        /// <para>A mini box is a square region that must contain each possible value exactly once.</para>
        public int BoxSize { get; }
        /// <summary>The total number of squares in the puzzle.</summary>
        public int NumSquares { get; }
        /// <summary>The current number of empty/unknown squares in the puzzle.</summary>
        public int NumEmptySquares { get { return _unsetCoordsTracker.NumTracked; } }
        /// <summary>The number of set/known squares in the puzzle.</summary>
        public int NumSetSquares { get { return NumSquares - NumEmptySquares; } }
        public ReadOnlySpan<int> AllPossibleValues => _allPossibleValues;

        /// <summary>
        /// Constructs a new puzzle of the given side length.
        /// </summary>
        /// <param name="size">
        /// The side-length for this Sudoku puzzle. Must be a square of a whole number in the range [1, 25].
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown if size is not the square of a whole number, or is outside the range [1, 25].
        /// </exception>
        public Puzzle(int size)
        {
            Size = size;
            switch (size)
            {
                case 1:
                    NumSquares = 1;
                    BoxSize = 1;
                    break;
                case 4:
                    NumSquares = 4 * 4;
                    BoxSize = 2;
                    break;
                case 9:
                    NumSquares = 9 * 9;
                    BoxSize = 3;
                    break;
                case 16:
                    NumSquares = 16 * 16;
                    BoxSize = 4;
                    break;
                case 25:
                    NumSquares = 25 * 25;
                    BoxSize = 5;
                    break;
                default:
                    throw new ArgumentException("Size must be one of [1, 4, 9, 16, 25].");
            }
            _squares = new int?[size, size];
            _unsetCoordsTracker = new CoordinateTracker(size);
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
            _allPossibleValues = new int[size];
            for (int i = 0; i < size; i++)
            {
                _allPossibleValues[i] = i + 1;
            }
        }

        /// <summary>
        /// Constructs a new puzzle whose data matches the given array.
        /// </summary>
        /// <param name="puzzleMatrix">
        /// The data for this Sudoku puzzle. Preset squares should be set, and unset squares should
        /// be null. A copy of this data is stored in this <c>Puzzle</c>.
        /// </param>
        public Puzzle(int?[,] puzzleMatrix)
        {
            NumSquares = puzzleMatrix.Length;
            Size = puzzleMatrix.GetLength(0);
            if (Size != puzzleMatrix.GetLength(1))
            {
                throw new ArgumentException("Puzzle must be square.");
            }
            BoxSize = Size switch
            {
                1 => 1,
                4 => 2,
                9 => 3,
                16 => 4,
                25 => 5,
                _ => throw new ArgumentException("Size must be one of [1, 4, 9, 16, 25]."),
            };
            
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
            _allPossibleValues = new int[Size];
            for (int i = 0; i < Size; i++)
            {
                _allPossibleValues[i] = i + 1;
            }
        }

        /// <summary>
        /// A copy constructor for an existing <c>Puzzle</c>.
        /// </summary>
        public Puzzle(Puzzle existing)
        {
            Size = existing.Size;
            BoxSize = existing.BoxSize;
            NumSquares = existing.NumSquares;
            _squares = (int?[,])existing._squares.Clone();
            _unsetCoordsTracker = new CoordinateTracker(existing._unsetCoordsTracker);
            _allPossibleValues = existing.AllPossibleValues.ToArray();
        }

        /// <inheritdoc cref="IPuzzle"/>
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

        /// <inheritdoc cref="IPuzzle"/>
        public IPuzzle DeepCopy()
        {
            return new Puzzle(this);
        }

        /// <inheritdoc cref="IPuzzle"/>
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

        /// <summary>Gets a span of coordinates for all the unset squares.</summary>
        public ReadOnlySpan<Coordinate> GetUnsetCoords()
        {
            return _unsetCoordsTracker.GetTrackedCoords();
        }

        /// <summary>
        /// Yields an enumerable of coordinates for all the unset squares in the given box.
        /// </summary>
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

        /// <summary>
        /// Returns the puzzle in a pretty string format, with boxes and rows separated by pipes
        /// and dashes.
        /// </summary>
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
