namespace SudokuSpice.RuleBased
{
    // TODO
    public interface IPuzzleWithPossibleValues<T> : IPuzzle<T>, IReadOnlyPuzzleWithMutablePossibleValues where T : class, IPuzzle<T>
    {
    }
}