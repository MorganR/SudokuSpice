namespace SudokuSpice.RuleBased.Rules
{
    /// <summary>
    /// Enforces a rule for a puzzle, such as "all values must be unique within a row." This is
    /// done by tracking possible values for each square specifically as determined by this rule.
    /// These possible values are then enforced along with any other rules by an
    /// <see cref="ISudokuRuleKeeper"/>.
    /// </summary>
    public interface ISudokuRule
    {
        /// <summary>
        /// Gets the possible values for the given coordinate based on this rule.
        /// </summary>
        /// <remarks>
        /// When implementing this method, avoid depending on dynamic information in the current
        /// puzzle being solved, such as the currently set values. This method must return
        /// information that reflects the most recent calls to
        /// <see cref="Update(in Coordinate, int, CoordinateTracker)">Update</see>
        /// or <see cref="Revert(in Coordinate, int, CoordinateTracker)">Revert</see>, as it can
        /// be called before or after modifying the underlying puzzle's data.
        /// </remarks>
        /// <returns>The possible values represented as a <see cref="BitVector"/>.</returns>
        BitVector GetPossibleValues(in Coordinate c);
        /// <summary>
        /// Undoes an update for the given value at the specified coordinate.
        /// <para>
        /// This performs the same internal updates as
        /// <see cref="Revert(in Coordinate, int, CoordinateTracker)"/>, but without passing
        /// affected coordinates back to the caller. Therefore this is more efficient in cases
        /// where the caller already knows all the possible coordinates that could be affected.
        /// </para>
        /// </summary>
        /// <remarks>
        /// This method will always be called before reverting the value on the underlying puzzle.
        /// </remarks>
        /// <param name="c">The coordinate where a value is being unset.</param>
        /// <param name="val">The value being unset.</param>
        void Revert(in Coordinate c, int val);
        /// <summary>
        /// Undoes an update for the given value at the specified coordinate. Tracks affected
        /// coordinates in the given tracker.
        /// </summary>
        /// <remarks>
        /// This method will always be called before reverting the value on the underlying puzzle.
        /// </remarks>
        /// <param name="c">The coordinate where a value is being unset.</param>
        /// <param name="val">The value being unset.</param>
        /// <param name="coordTracker">
        /// The coordinates of unset squares impacted by this change will be tracked in this
        /// tracker.
        /// </param>
        void Revert(in Coordinate c, int val, CoordinateTracker coordTracker);
        /// <summary>
        /// Updates possible values based on setting the given coordinate to the given value.
        /// Tracks affected coordinates in the given tracker.
        /// </summary>
        /// <remarks>
        /// This method will always be called before setting the value on the underlying puzzle.
        /// </remarks>
        /// <param name="c">The coordinate to update.</param>
        /// <param name="val">The value to set <c>c</c> to.</param>
        /// <param name="coordTracker">
        /// The coordinates of unset squares impacted by this change will be tracked in this
        /// tracker.
        /// </param>
        void Update(in Coordinate c, int val, CoordinateTracker coordTracker);
        /// <summary>
        /// Creates a deep copy of this ISudokuRule, with any internal <c>IReadOnlyPuzzle</c>
        /// references updated to the given puzzle.
        /// </summary>
        /// <param name="puzzle">
        /// The new puzzle reference to use. To ensure this rule's internal state is correct, this
        /// puzzle should contain the same data as the current puzzle referenced by this rule.
        /// </param>
        ISudokuRule CopyWithNewReference(IReadOnlyPuzzle puzzle);
    }
}