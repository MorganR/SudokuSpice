namespace SudokuSpice.RuleBased
{
    // TODO
    public interface IPossibleValues : IReadOnlyPossibleValues
    {
        /// <summary>
        /// Sets the possible values for a square.
        /// </summary>
        void SetPossibleValues(in Coordinate c, BitVector possibleValues);

        /// <summary>
        /// Modifies the possible values for a square to be the intersect of the current possible
        /// values and the given <paramref name="possibleValues"/>.
        /// </summary>
        void IntersectPossibleValues(in Coordinate c, BitVector possibleValues);

        /// <summary>
        /// Resets the possible values at the given location to be all possible values for this
        /// puzzle.
        /// </summary>
        void ResetPossibleValues(in Coordinate c);
    }
}