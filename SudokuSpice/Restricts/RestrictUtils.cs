using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSpice
{
    public static class RestrictUtils
    {
        public static void RestrictAllUnsetPossibleValues(Puzzle puzzle, IReadOnlyList<ISudokuRestrict> restricts)
        {
            foreach (var c in puzzle.GetUnsetCoords())
            {
                puzzle.SetPossibleValues(
                    c.Row, c.Column,
                    restricts.Aggregate(
                        new BitVector(-1),
                        (agg, r) => BitVector.FindIntersect(agg, r.GetPossibleValues(in c))));
                if (puzzle.GetPossibleValues(c.Row, c.Column).IsEmpty())
                {
                    throw new ArgumentException(
                        "Puzzle could not be solved with the given preset values.");
                }
            }
        }
    }
}
