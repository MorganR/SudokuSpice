using System;

namespace SudokuSpice
{
    /// <summary>
    /// Defines a square region within a puzzle.
    /// </summary>
    public readonly struct Box : IEquatable<Box>
    {
        /// <summary>
        /// The top-left coordinate (inclusive) for this box.
        /// </summary>
        public readonly Coordinate TopLeft { get; }
        /// <summary>
        /// The size (i.e. side-length) of this box.
        /// </summary>
        public readonly int Size { get; }

        /// <summary>
        /// Constructs a box covering the given region. <paramref name="size"/> must be greater
        /// than 0.
        /// </summary>
        public Box(Coordinate topLeft, int size)
        {
            if (size == 0)
            {
                throw new ArgumentException($"{nameof(Box)} must have a size > 0.");
            }
            TopLeft = topLeft;
            Size = size;
        }

        /// <summary>
        /// Returns true if the given coordinate is part of the box.
        /// </summary>
        public readonly bool Contains(in Coordinate coord)
        {
            return coord.Row >= TopLeft.Row
                && coord.Column >= TopLeft.Column
                && coord.Row < TopLeft.Row + Size
                && coord.Column < TopLeft.Column + Size;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Box other)
            {
                return Equals(other);
            }
            return false;
        }

        public bool Equals(Box other)
        {
            return Size == other.Size && TopLeft == other.TopLeft;
        }

        public override int GetHashCode()
        {
            return TopLeft.GetHashCode() + Size;
        }

        public static bool operator ==(Box left, Box right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Box left, Box right)
        {
            return left.Size != right.Size || left.TopLeft != right.TopLeft;
        }
    }
}