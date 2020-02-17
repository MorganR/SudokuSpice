using System;

namespace SudokuSpice
{
    public readonly struct Coordinate : IEquatable<Coordinate>
    {
        public readonly int Row { get; }
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
            return this.Equals(in other);
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
