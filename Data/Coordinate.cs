namespace MorganRoff.Sudoku
{
    public readonly struct Coordinate
    {
        public readonly int Row;
        public readonly int Column;

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
    }
}
