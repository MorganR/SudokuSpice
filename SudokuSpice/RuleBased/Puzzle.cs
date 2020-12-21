using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace SudokuSpice.RuleBased
{
    /// <summary>Manages underlying puzzle data.</summary>.
    public class Puzzle : IPuzzle
    {
        private readonly int?[,] _squares;
        private readonly CoordinateTracker _unsetCoordsTracker;
        private readonly PossibleValues _possibleValues;

        /// <inheritdoc cref="IReadOnlyPuzzle"/>
        public int Size { get; }
        /// <inheritdoc cref="IReadOnlyPuzzle"/>
        public int NumSquares { get; }
        /// <inheritdoc cref="IReadOnlyPuzzle"/>
        public int NumEmptySquares => _unsetCoordsTracker.NumTracked;
        /// <inheritdoc cref="IReadOnlyPuzzle"/>
        public int NumSetSquares => NumSquares - NumEmptySquares;
        /// <inheritdoc cref="IReadOnlyPuzzle"/>
        public BitVector AllPossibleValues => _possibleValues.AllPossible;

        /// <summary>
        /// Constructs a new puzzle of the given side length.
        /// </summary>
        /// <param name="size">
        /// The side-length for this Sudoku puzzle. Must be in the inclusive range [1, 31].
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown if size is not the square of a whole number, or is outside the range [1, 31].
        /// </exception>
        public Puzzle(int size)
        {
            if (size < 1 || size > 31)
            {
                throw new ArgumentException("Puzzle size must be in the range [1, 31].");
            }
            Size = size;
            NumSquares = size * size;
            _squares = new int?[size, size];
            _unsetCoordsTracker = new CoordinateTracker(size);
            for (int row = 0; row < Size; row++)
            {
                for (int col = 0; col < Size; col++)
                {
                    if (!_squares[row, col].HasValue)
                    {
                        _unsetCoordsTracker.Add(new Coordinate(row, col));
                    }
                }
            }
            var possibleValues = BitVector.CreateWithSize(Size + 1);
            possibleValues.UnsetBit(0);
            _possibleValues = new PossibleValues(Size, possibleValues);
        }

        /// <summary>
        /// Constructs a new puzzle backed by the given array.
        ///
        /// The puzzle is backed directly by this array (i.e. modifying the array
        /// modifies the puzzle, and vice-versa). If this is not what you want, see
        /// <see cref="Puzzle.CopyFrom(int?[,])"/>.
        /// </summary>
        /// <param name="puzzleMatrix">
        /// The data for this Sudoku puzzle. Preset squares should be set, and unset squares should
        /// be null. The puzzle maintains a reference to this array.
        /// </param>
        public Puzzle(int?[,] puzzleMatrix)
        {
            NumSquares = puzzleMatrix.Length;
            Size = puzzleMatrix.GetLength(0);
            if (Size != puzzleMatrix.GetLength(1))
            {
                throw new ArgumentException("Puzzle must be square.");
            }

            _squares = puzzleMatrix;
            _unsetCoordsTracker = new CoordinateTracker(Size);
            for (int row = 0; row < Size; row++)
            {
                for (int col = 0; col < Size; col++)
                {
                    if (!_squares[row, col].HasValue)
                    {
                        _unsetCoordsTracker.Add(new Coordinate(row, col));
                    }
                }
            }
            var possibleValues = BitVector.CreateWithSize(Size + 1);
            possibleValues.UnsetBit(0);
            _possibleValues = new PossibleValues(Size, possibleValues);
        }

        /// <summary>
        /// A deep copy constructor for an existing <c>Puzzle</c>.
        /// </summary>
        public Puzzle(Puzzle existing)
        {
            Size = existing.Size;
            NumSquares = existing.NumSquares;
            _squares = (int?[,])existing._squares.Clone();
            _unsetCoordsTracker = new CoordinateTracker(existing._unsetCoordsTracker);
            _possibleValues = new PossibleValues(existing._possibleValues);
        }

        /// <summary>Creates a new <c>Puzzle</c> with a copy of the given matrix.</summary>
        public static Puzzle CopyFrom(int?[,] matrix)
        {
            return new Puzzle((int?[,])matrix.Clone());
        }

        /// <inheritdoc cref="IPuzzle"/>
        public int? this[int row, int col]
        {
            get => _squares[row, col];
            set {
                if (value.HasValue)
                {
                    _Set(row, col, value.Value);
                } else
                {
                    _Unset(row, col);
                }
            }
        }

        /// <inheritdoc cref="IPuzzle"/>
        public IPuzzle DeepCopy() => new Puzzle(this);

        /// <inheritdoc cref="IPuzzle"/>
        [SuppressMessage("Design", "CA1043:Use Integral Or String Argument For Indexers", Justification = "This makes sense with Coordinate, which removes any ambiguity between first and second arguments")]
        public int? this[in Coordinate c]
        {
            get => _squares[c.Row, c.Column];
            set => this[c.Row, c.Column] = value;
        }


        /// <summary>Gets a span of coordinates for all the unset squares.</summary>
        public ReadOnlySpan<Coordinate> GetUnsetCoords() => _unsetCoordsTracker.GetTrackedCoords();

        /// <summary>
        /// Returns the puzzle in a pretty string format, with boxes and rows separated by pipes
        /// and dashes.
        /// </summary>
        public override string ToString()
        {
            int maxDigitLength = Size.ToString(NumberFormatInfo.InvariantInfo).Length;
            var strBuild = new StringBuilder();
            int boxSize = Boxes.CalculateBoxSize(Size);
            for (int row = 0; row < Size; row++)
            {
                if (row % boxSize == 0)
                {
                    _AppendBoxDividerRow(strBuild);
                }
                strBuild.Append('|');
                for (int col = 0; col < Size; col++)
                {
                    string? numberString =
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
                    if (col % boxSize == boxSize - 1)
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

        private void _Set(int row, int col, int val)
        {
            Debug.Assert(!_squares[row, col].HasValue, $"Square ({row}, {col}) already has a value.");
            _squares[row, col] = val;
            _unsetCoordsTracker.Untrack(new Coordinate(row, col));
        }

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
            int boxSize = Boxes.CalculateBoxSize(Size);
            for (int col = 0; col < Size; col++)
            {
                if (col % boxSize == 0)
                {
                    strBuild.Append('+');
                } else
                {
                    strBuild.Append('-');
                }
                for (int numCharsToAppend = maxDigitLength; numCharsToAppend > 0; numCharsToAppend--)
                {
                    strBuild.Append('-');
                }
            }
            strBuild.Append("+\n");
        }

        public void IntersectPossibleValues(in Coordinate c, BitVector possibleValues) =>
            _possibleValues.Intersect(in c, possibleValues);
        public void ResetPossibleValues(in Coordinate c) => _possibleValues.Reset(in c);
        public BitVector GetPossibleValues(in Coordinate c) => _possibleValues[in c];
        public void SetPossibleValues(in Coordinate c, BitVector possibleValues) => _possibleValues[in c] = possibleValues;
    }
}