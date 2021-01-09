namespace SudokuSpice.RuleBased
{
    /// <summary>
    /// Provides read-only access to a puzzle's data and associated possible values.
    /// </summary>
    public interface IReadOnlyPuzzleWithPossibleValues : IReadOnlyPuzzle, IReadOnlyPossibleValues
    {
    }
}