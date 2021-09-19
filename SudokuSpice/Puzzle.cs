using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace SudokuSpice
{
    /// <summary>
    /// Represents a square puzzle of shape size-by-size.
    /// </summary>.
    public class Puzzle : IPuzzle<Puzzle>
    {
        private readonly int?[][] _squares;
        private readonly CoordinateTracker _unsetCoordsTracker;
        private readonly int[] _allPossibleValues;

        /// <inheritdoc/>
        public int Size { get; }
        /// <inheritdoc/>
        public int NumSquares => Size * Size;
        /// <inheritdoc/>
        public int NumEmptySquares => _unsetCoordsTracker.NumTracked;
        /// <inheritdoc/>
        public int NumSetSquares => NumSquares - NumEmptySquares;
        /// <inheritdoc/>
        public ReadOnlySpan<int> AllPossibleValuesSpan => _allPossibleValues;
        /// <inheritdoc/>
        public IReadOnlyDictionary<int, int> CountPerUniqueValue { get; }

        /// <summary>
        /// Constructs a new puzzle of the given side length. Assumes the standard possible values
        /// for each region (i.e. the numbers from [1, size]).
        /// </summary>
        /// <param name="size">
        /// The side-length for this Sudoku puzzle.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown if size is less than 1.
        /// </exception>
        public Puzzle(int size)
        {
            if (size < 1)
            {
                throw new ArgumentException($"{nameof(size)} must be >= 1.");
            }
            Size = size;
            _squares = new int?[size][];
            _unsetCoordsTracker = new CoordinateTracker(size);
            _InitUnsetCoordsTrackerAndSquares(_unsetCoordsTracker, _squares);
            _allPossibleValues = new int[Size];
            var countPerUniqueValue = new Dictionary<int, int>(size);
            _InitStandardPossibleValues(_allPossibleValues, countPerUniqueValue);
            CountPerUniqueValue = countPerUniqueValue;
        }

        /// <summary>
        /// Constructs a new puzzle of the given side length and possible values for each region.
        /// </summary>
        /// <param name="size">
        /// The side-length for this Sudoku puzzle.
        /// </param>
        /// <param name="allPossibleValues">
        /// List the possible values for a given region in the puzzle. A value should be repeated
        /// as many times as it can be used.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown if size is less than 1.
        /// </exception>
        public Puzzle(int size, ReadOnlySpan<int> allPossibleValues)
        {
            if (size < 1)
            {
                throw new ArgumentException($"{nameof(size)} must be >= 1.");
            }
            Size = size;
            _squares = new int?[size][];
            _unsetCoordsTracker = new CoordinateTracker(size);
            _InitUnsetCoordsTrackerAndSquares(_unsetCoordsTracker, _squares);
            _allPossibleValues = allPossibleValues.ToArray();
            var countPerUniqueValue = new Dictionary<int, int>(size);
            _InitCustomPossibleValues(allPossibleValues, countPerUniqueValue);
            CountPerUniqueValue = countPerUniqueValue;
        }

        /// <summary>
        /// Constructs a new puzzle backed by the given array.
        ///
        /// The puzzle is backed directly by this array (i.e. modifying the array modifies the
        /// puzzle, and vice-versa). If this is not what you want, see
        /// <see cref="CopyFrom(int?[,])"/> and <see cref="CopyFrom(int?[][])"/>. Note that all
        /// future modifications should be done through this puzzle object, else this will be in an
        /// incorrect state.
        /// </summary>
        /// <param name="puzzleMatrix">
        /// The data for this Sudoku puzzle. Preset squares should be set, and unset squares should
        /// be null. The puzzle maintains a reference to this array.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown if the given matrix is not square.
        /// </exception>
        public Puzzle(int?[][] puzzleMatrix)
        {
            Size = puzzleMatrix.Length;
            if (Size == 0 || Size != puzzleMatrix[0].Length)
            {
                throw new ArgumentException("Puzzle must be square with non-zero dimensions.");
            }

            _squares = puzzleMatrix;
            _unsetCoordsTracker = new CoordinateTracker(Size);
            _InitUnsetCoordsTracker(_unsetCoordsTracker, _squares);
            _allPossibleValues = new int[Size];
            var countPerUniqueValue = new Dictionary<int, int>(Size);
            _InitStandardPossibleValues(_allPossibleValues, countPerUniqueValue);
            CountPerUniqueValue = countPerUniqueValue;
        }

        /// <summary>
        /// Constructs a new puzzle backed by the given array.
        ///
        /// The puzzle is backed directly by this array (i.e. modifying the array modifies the
        /// puzzle, and vice-versa). If this is not what you want, see
        /// <see cref="CopyFrom(int?[,])"/> and <see cref="CopyFrom(int?[][])"/>. Note that all
        /// future modifications should be done through this puzzle object, else this will be in an
        /// incorrect state.
        /// </summary>
        /// <param name="puzzleMatrix">
        /// The data for this Sudoku puzzle. Preset squares should be set, and unset squares should
        /// be null. The puzzle maintains a reference to this array.
        /// </param>
        /// <param name="allPossibleValues">
        /// List the possible values for a given region in the puzzle. A value should be repeated
        /// as many times as it can be used.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown if the given matrix is not square.
        /// </exception>
        public Puzzle(int?[][] puzzleMatrix, ReadOnlySpan<int> allPossibleValues)
        {
            Size = puzzleMatrix.Length;
            if (Size == 0 || Size != puzzleMatrix[0].Length)
            {
                throw new ArgumentException("Puzzle must be square with non-zero dimensions.");
            }

            _squares = puzzleMatrix;
            _unsetCoordsTracker = new CoordinateTracker(Size);
            _InitUnsetCoordsTracker(_unsetCoordsTracker, _squares);
            _allPossibleValues = allPossibleValues.ToArray();
            var countPerUniqueValue = new Dictionary<int, int>(Size);
            _InitCustomPossibleValues(allPossibleValues, countPerUniqueValue);
            CountPerUniqueValue = countPerUniqueValue;
        }

        private static void _InitUnsetCoordsTrackerAndSquares(CoordinateTracker unsetCoordsTracker, Span<int?[]> squares)
        {
            for (int row = 0; row < squares.Length; row++)
            {
                squares[row] = new int?[squares.Length];
                for (int col = 0; col < squares.Length; col++)
                {
                    unsetCoordsTracker.Add(new Coordinate(row, col));
                }
            }
        }

        private static void _InitUnsetCoordsTracker(CoordinateTracker unsetCoordsTracker, ReadOnlySpan<int?[]> squares)
        {
            for (int row = 0; row < squares.Length; row++)
            {
                var squaresRow = squares[row];
                for (int col = 0; col < squaresRow.Length; col++)
                {
                    if (!squaresRow[col].HasValue)
                    {
                        unsetCoordsTracker.Add(new Coordinate(row, col));
                    }
                }
            }
        }

        private static void _InitStandardPossibleValues(Span<int> allPossibleValues, Dictionary<int, int> countPerUniqueValue)
        {
            for (int i = 0; i < allPossibleValues.Length; ++i)
            {
                allPossibleValues[i] = i + 1;
                countPerUniqueValue[i + 1] = 1;
            }
        }

        private static void _InitCustomPossibleValues(ReadOnlySpan<int> allPossibleValues, Dictionary<int, int> countPerUniqueValue)
        {
            for (int i = 0; i < allPossibleValues.Length; ++i)
            {
                int value = allPossibleValues[i];
                if (countPerUniqueValue.ContainsKey(value))
                {
                    ++countPerUniqueValue[value];
                } else
                {
                    countPerUniqueValue[value] = 1;
                }
            }
        }

        /// <summary>
        /// A copy constructor for an existing <c>Puzzle</c>.
        /// </summary>
        public Puzzle(Puzzle existing)
        {
            Size = existing.Size;
            _squares = new int?[Size][];
            int i = 0;
            foreach (var row in existing._squares)
            {
                _squares[i++] = row.AsSpan().ToArray();
            }
            _unsetCoordsTracker = new CoordinateTracker(existing._unsetCoordsTracker);
            _allPossibleValues = existing._allPossibleValues;
            CountPerUniqueValue = existing.CountPerUniqueValue;
        }

        /// <summary>Creates a new puzzle with a copy of the given matrix.</summary>
        [SuppressMessage("Performance", "CA1814:Prefer jagged arrays over multidimensional", Justification = "Provided to ease migration.")]
        public static Puzzle CopyFrom(int?[,] matrix)
        {
            return new Puzzle(matrix.CopyToJagged2D());
        }

        /// <summary>Creates a new puzzle with a copy of the given matrix.</summary>
        public static Puzzle CopyFrom(int?[][] matrix)
        {
            return new Puzzle(matrix.Copy2D());
        }

        /// <inheritdoc/>
        public Puzzle DeepCopy() => new Puzzle(this);

        /// <inheritdoc/>
        public int? this[int row, int col]
        {
            get => _squares[row][col];
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

        /// <inheritdoc/>
        [SuppressMessage("Design", "CA1043:Use Integral Or String Argument For Indexers", Justification = "This makes sense with Coordinate, which removes any ambiguity between first and second arguments")]
        public int? this[in Coordinate c]
        {
            get => _squares[c.Row][c.Column];
            set => this[c.Row, c.Column] = value;
        }

        /// <summary>Gets a span of coordinates for all the unset squares.</summary>
        public ReadOnlySpan<Coordinate> GetUnsetCoords() => _unsetCoordsTracker.TrackedCoords;

        /// <summary>
        /// Returns the puzzle in a pretty string format, with boxes and rows separated by pipes
        /// and dashes.
        /// </summary>
        public override string ToString() => Puzzles.ToString(this);

        /// <summary>Sets the value of a square.</summary>
        private void _Set(int row, int col, int val)
        {
            Debug.Assert(!_squares[row][col].HasValue, $"Square ({row}, {col}) already has a value.");
            _squares[row][col] = val;
            _unsetCoordsTracker.Untrack(new Coordinate(row, col));
        }

        /// <summary>Unsets the specified square.</summary>
        private void _Unset(int row, int col)
        {
            Debug.Assert(_squares[row][col].HasValue,
                $"Square ({row}, {col}) doesn't have a value, so can't be unset.");
            _unsetCoordsTracker.Track(new Coordinate(row, col));
            _squares[row][col] = null;
        }
    }
}