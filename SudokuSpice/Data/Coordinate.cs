using System;

namespace SudokuSpice.Data
{
    /// <summary>
    /// Uniquely identifies the location of a square in a Sudoku puzzle.
    /// </summary>
    /// <remarks>
    /// Note that this struct is <see langword="readonly"/>, so can be efficiently referenced using
    /// <see langword="in"/>. This may provide minor performance enhancement on some architectures.
    /// </remarks>
    public readonly struct Coordinate : IEquatable<Coordinate>
    {
        /// <summary>
        /// The zero-based row in the puzzle, starting at the top.
        /// </summary>
        public readonly int Row { get; }
        /// <summary>
        /// The zero-based column in the puzzle, starting on the left.
        /// </summary>
        public readonly int Column { get; }

        public Coordinate(int row, int col)
        {
            Row = row;
            Column = col;
        }

        public void Deconstruct(out int row, out int col) => (row, col) = (Row, Column);

        public override string ToString()
        {
            return $"({Row}, {Column})";
        }

        public override bool Equals(object? obj)
        {
            if (obj is Coordinate other)
            {
                return Equals(in other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Row + (Column << 16);
        }

        public bool Equals(Coordinate other)
        {
            return Equals(in other);
        }

        public bool Equals(in Coordinate other)
        {
            return Row == other.Row && Column == other.Column;
        }

        public static bool operator ==(in Coordinate left, in Coordinate right)
        {
            return left.Equals(in right);
        }

        public static bool operator !=(in Coordinate left, in Coordinate right)
        {
            return !(left == right);
        }
    }
}
