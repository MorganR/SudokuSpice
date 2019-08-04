using System;
using System.Collections.Generic;
using System.Linq;

namespace MorganRoff.Sudoku
{
    public class RestrictUtils
    {
        public static IReadOnlyList<IRestrict> CreateStandardRestricts(Puzzle puzzle)
        {
            return new List<IRestrict> {
                new RowRestrict(puzzle),
                new ColumnRestrict(puzzle),
                new BoxRestrict(puzzle)
            };
        }

        public static void RestrictAllPossibleValues(Puzzle puzzle, IReadOnlyList<IRestrict> restricts)
        {
            foreach (var c in puzzle.GetUnsetCoords())
            {
                puzzle.SetPossibleValues(
                    c.Row, c.Column,
                    restricts.Aggregate(
                        -1,
                        (agg, r) => agg &= r.GetPossibleValues(in c)));
                if (puzzle.GetPossibleValues(c.Row, c.Column) == 0)
                {
                    throw new ArgumentException(
                        "Puzzle could not be solved with the given preset values.");
                }
            }
        }
    }
}
