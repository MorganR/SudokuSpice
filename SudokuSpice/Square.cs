namespace SudokuSpice
{
    public readonly struct Square
    {
        public readonly Coordinate TopLeft { get; }
        public readonly int Size { get; }

        public Square(Coordinate topLeft, int size)
        {
            TopLeft = topLeft;
            Size = size;
        }

        public readonly bool Contains(in Coordinate coord)
        {
            return coord.Row >= TopLeft.Row
                && coord.Column >= TopLeft.Column
                && coord.Row < TopLeft.Row + Size
                && coord.Column < TopLeft.Column + Size;
        }
    }
}
