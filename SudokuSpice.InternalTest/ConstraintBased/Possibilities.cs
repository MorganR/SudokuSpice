namespace SudokuSpice.ConstraintBased.InternalTest
{
    internal static class Possibilities
    {
        internal static Possibility[] CreatePossibilities(Coordinate coordinate, int count)
        {
            var possibilities = new Possibility[count];
            for (--count; count >= 0; --count)
            {
                possibilities[count] = new Possibility(coordinate, 0);
            }
            return possibilities;
        }
    }
}