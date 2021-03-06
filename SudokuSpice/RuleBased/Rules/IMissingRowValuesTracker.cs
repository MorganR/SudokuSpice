﻿namespace SudokuSpice.RuleBased.Rules
{
    /// <summary>
    /// Tracks all the required unset values for a given row in the Puzzle.
    /// </summary>
    public interface IMissingRowValuesTracker : IRule
    {
        /// <summary>Returns all the values that still need to be set in the given row.</summary>
        BitVector GetMissingValuesForRow(int row);
    }
}