﻿namespace SudokuSpice.RuleBased.Rules
{
    /// <summary>
    /// Tracks all the required unset values for a given column in the Puzzle.
    /// </summary>
    public interface IMissingColumnValuesTracker : IRule
    {
        /// <summary>
        /// Returns all the values that still need to be set in the given column.
        /// </summary>
        BitVector GetMissingValuesForColumn(int column);
    }
}