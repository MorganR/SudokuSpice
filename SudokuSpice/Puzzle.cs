using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace SudokuSpice
{
    /// <summary>
    /// Represents a puzzle with the following assumptions:
    ///
    /// * Puzzles must be square, of shape size-by-size.
    /// * The possible values for any square in the puzzle are the numbers from 1 to size,
    ///   inclusive.
    /// </summary>.
    public class Puzzle : IPuzzle<Puzzle>
    {
        private readonly int?[][] _squares;
        private readonly CoordinateTracker _unsetCoordsTracker;
        private readonly int[] _allPossibleValues;

        /// <inheritdoc/>
        public int Size { get; }
        /// <inheritdoc/>
        public int NumSquares { get; }
        /// <inheritdoc/>
        public int NumEmptySquares => _unsetCoordsTracker.NumTracked;
        /// <inheritdoc/>
        public int NumSetSquares => NumSquares - NumEmptySquares;
        /// <inheritdoc/>
        public ReadOnlySpan<int> AllPossibleValuesSpan => _allPossibleValues;

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
            if (size < 1)
            {
                throw new ArgumentException($"{nameof(size)} must be >= 1.");
            }
            Size = size;
            NumSquares = size * size;
            _squares = new int?[size][];
            _unsetCoordsTracker = new CoordinateTracker(size);
            for (int row = 0; row < Size; row++)
            {
                _squares[row] = new int?[size];
                for (int col = 0; col < Size; col++)
                {
                    _unsetCoordsTracker.Add(new Coordinate(row, col));
                }
            }
            _allPossibleValues = new int[Size];
            for (int i = 0; i < Size; ++i)
            {
                _allPossibleValues[i] = i + 1;
            }
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
        public Puzzle(int?[][] puzzleMatrix)
        {
            NumSquares = puzzleMatrix.Length;
            Size = puzzleMatrix.Length;
            if (Size == 0 || Size != puzzleMatrix[0].Length)
            {
                throw new ArgumentException("Puzzle must be square with non-zero dimensions.");
            }

            _squares = puzzleMatrix;
            _unsetCoordsTracker = new CoordinateTracker(Size);
            for (int row = 0; row < _squares.Length; row++)
            {
                var squaresRow = _squares[row];
                for (int col = 0; col < squaresRow.Length; col++)
                {
                    if (!squaresRow[col].HasValue)
                    {
                        _unsetCoordsTracker.Add(new Coordinate(row, col));
                    }
                }
            }
            _allPossibleValues = new int[Size];
            for (int i = 0; i < Size; ++i)
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
            NumSquares = existing.NumSquares;
            _squares = new int?[Size][];
            int i = 0;
            foreach (var row in existing._squares)
            {
                _squares[i++] = row.AsSpan().ToArray();
            }
            _unsetCoordsTracker = new CoordinateTracker(existing._unsetCoordsTracker);
            _allPossibleValues = existing._allPossibleValues;
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