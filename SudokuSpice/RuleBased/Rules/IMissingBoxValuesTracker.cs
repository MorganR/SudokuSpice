namespace SudokuSpice.RuleBased.Rules
{
    /// <summary>
    /// Tracks all the required unset values for a given box in the Puzzle.
    /// </summary>
    public interface IMissingBoxValuesTracker : IRule
    {
        /// <summary>Returns all the values that still need to be set in the given box.</summary>
        BitVector GetMissingValuesForBox(int box);
    }
}