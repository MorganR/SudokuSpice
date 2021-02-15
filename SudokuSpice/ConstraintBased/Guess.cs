using System.Collections.Generic;

namespace SudokuSpice.ConstraintBased
{
    internal readonly struct Guess
    {
        internal readonly Coordinate Coordinate { get; }
        internal readonly int PossibilityIndex { get; }

        internal Guess(Coordinate c, int index)
        {
            Coordinate = c;
            PossibilityIndex = index;
        }

        internal IEnumerable<Guess> Yield() { yield return this; }
    }
}
