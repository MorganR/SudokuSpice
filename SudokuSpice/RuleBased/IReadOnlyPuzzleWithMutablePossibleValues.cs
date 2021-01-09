namespace SudokuSpice.RuleBased
{
    /// <summary>
    /// Provides read-only access to a puzzle's data with mutable access to the associated possible values.
    /// </summary>
    public interface IReadOnlyPuzzleWithMutablePossibleValues : IReadOnlyPuzzleWithPossibleValues, IPossibleValues
    {
    }
}