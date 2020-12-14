namespace SudokuSpice.RuleBased
{
    public interface IReadOnlyPuzzleWithMutablePossibleValues : IReadOnlyPuzzle
    {
        public void SetPossibleValues(in Coordinate c, BitVector possibleValues);

        public void IntersectPossibleValues(in Coordinate c, BitVector possibleValues);

        public void ResetPossibleValues(in Coordinate c);
    }
}