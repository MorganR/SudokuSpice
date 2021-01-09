namespace SudokuSpice.RuleBased
{
    /// <summary>
    /// Holds a puzzle's data and corresponding possible values.
    /// </summary>
    /// <typeparam name="T">Should be the concrete type implementing this interface.</typeparam>
    public interface IPuzzleWithPossibleValues<T> : IPuzzle<T>, IReadOnlyPuzzleWithMutablePossibleValues where T : class, IPuzzle<T>
    {
    }
}