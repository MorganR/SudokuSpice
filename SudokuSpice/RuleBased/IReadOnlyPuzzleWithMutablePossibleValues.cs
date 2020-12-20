namespace SudokuSpice.RuleBased
{
    public interface IReadOnlyPuzzleWithMutablePossibleValues : IReadOnlyPuzzle
    {
        void SetPossibleValues(in Coordinate c, BitVector possibleValues);

        void IntersectPossibleValues(in Coordinate c, BitVector possibleValues);

        void ResetPossibleValues(in Coordinate c);
    }
}