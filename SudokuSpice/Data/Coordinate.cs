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

        public override bool Equals(object obj)
        {
            if (obj is Coordinate other)
            {
                return Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Row + (Column << 10);
        }

        public bool Equals(Coordinate other)
        {
            return Row == other.Row && Column == other.Column;
        }

        public static bool operator ==(Coordinate left, Coordinate right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Coordinate left, Coordinate right)
        {
            return !(left == right);
        }
    }
}
