namespace SudokuSpice
{
    public readonly struct Box
    {
        public readonly Coordinate TopLeft { get; }
        public readonly int Size { get; }

        public Box(Coordinate topLeft, int size)
        {
            TopLeft = topLeft;
            Size = size;
        }
    }
}
