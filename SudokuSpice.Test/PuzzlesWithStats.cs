using Xunit;

namespace SudokuSpice.Test
{
    public sealed class PuzzlesWithStats : TheoryData<int?[][], SolveStats>
    {
        public PuzzlesWithStats()
        {
            Add(new int?[][]
                {
                    new int?[] {   1, null, null,    2},
                    new int?[] {null, null,    1, null},
                    new int?[] {null,    1, null, null},
                    new int?[] {   3, null,    4, null}
                },
                new SolveStats() { NumSolutionsFound = 1 });
            Add(new int?[][]
                {
                    new int?[] {   4, null,    2, null, null,    1,    8,    7,    6},
                    new int?[] {   3, null,    8, null, null,    5, null,    9,    4},
                    new int?[] {   6, null,    9,    4, null,    8,    3, null,    5},
                    new int?[] {null,    3,    1, null,    6, null, null, null, null},
                    new int?[] {   2,    4,    5,    9, null,    7,    1,    6,    3},
                    new int?[] {   9, null,    7,    2, null,    3,    5,    4,    8},
                    new int?[] {null,    9, null,    8, null,    2, null, null, null},
                    new int?[] {   1,    8,    3, null,    4,    9,    6,    5,    2},
                    new int?[] {   5,    2,    4,    1,    3,    6,    9, null,    7}
                },
                new SolveStats() { NumSolutionsFound = 1 });
            Add(new int?[][]
                {
                    new int?[] {null,    2, null, null, null, null, null, null, null},
                    new int?[] {null, null, null,    6, null, null, null, null,    3},
                    new int?[] {null,    7,    4, null,    8, null, null, null, null},
                    new int?[] {null, null, null, null, null,    3, null, null,    2},
                    new int?[] {null,    8, null, null,    4, null, null,    1, null},
                    new int?[] {   6, null, null,    5, null, null, null, null, null},
                    new int?[] {null, null, null, null,    1, null,    7,    8, null},
                    new int?[] {   5, null, null, null, null,    9, null, null, null},
                    new int?[] {null, null, null, null, null, null, null,    4, null}
                },
                new SolveStats() { NumSolutionsFound = 1, NumSquaresGuessed = 10, NumTotalGuesses = 22 });
            Add(new int?[][]
                {
                    new int?[] {   1, null, null, null},
                    new int?[] {null, null,    1, null},
                    new int?[] {null,    1, null, null},
                    new int?[] {   3, null,    4, null}
                },
                // Solutions:
                // +---+---+    +---+---+ +---+---+ +---+---+
                // |1 x|x x|    |1 4|3 2| |1 3|2 4| |1 4|2 3|
                // |x x|1 x|    |2 3|1 4| |2 4|1 3| |2 3|1 4|
                // +---+---+ => +---+---+ +---+---+ +---+---+
                // |x 1|x x|    |4 1|2 3| |4 1|3 2| |4 1|3 2|
                // |3 x|4 x|    |3 2|4 1| |3 2|4 1| |3 2|4 1|
                // +---+---+    +---+---+ +---+---+ +---+---+
                new SolveStats() { NumSolutionsFound = 3, NumSquaresGuessed = 2, NumTotalGuesses = 4 });
        }
    }
}