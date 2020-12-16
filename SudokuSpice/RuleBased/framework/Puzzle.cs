using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace SudokuSpice.RuleBased
{
    /// <summary>Manages underlying puzzle data.</summary>.
    [SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional")]
    public class Puzzle : IReadOnlyPuzzleWithMutablePossibleValues
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
        /// Constructs a new puzzle backed by the given matrix.
        /// </summary>
        /// <param name="puzzleMatrix">
        /// The data for this Sudoku puzzle. Preset squares should be set, and unset squares should
        /// be null. Changes to this <c>Puzzle</c> will be reflected in this matrix.
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
        /// A copy constructor for an existing <c>Puzzle</c>.
        /// </summary>
        public Puzzle(Puzzle existing)
        {
            Size = existing.Size;
            NumSquares = existing.NumSquares;
            _squares = (int?[,])existing._squares.Clone();
            _unsetCoordsTracker = new CoordinateTracker(existing._unsetCoordsTracker);
            _possibleValues = new PossibleValues(existing._possibleValues);
        }

        public static Puzzle CopyFrom(int?[,] matrix)
        {
            return new Puzzle((int?[,])matrix.Clone());
        }

        /// <summary>
        /// Gets or sets the current value of a given square. A square can be 'unset' by setting
        /// its value to <c>null</c>.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the value of the given square, like <see cref="this[int, int]"/>, but
        /// using a <see cref="Coordinate"/> instead of <see langword="int"/> accessors.
        /// </summary>
        /// <param name="c">The location of the square to get/set the value of.</param>
        /// <returns>The value of the square at <paramref name="c"/></returns>
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