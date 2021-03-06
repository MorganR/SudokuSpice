using Xunit;

namespace SudokuSpice.Test
{
    public sealed class ValidStandardPuzzles : TheoryData<int?[][]>
    {
        public ValidStandardPuzzles()
        {
            Add(new int?[][]
                {
                    new int?[] {   1, null, null,    2},
                    new int?[] {null, null,    1, null},
                    new int?[] {null,    1, null, null},
                    new int?[] {   3, null,    4, null}
                });
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
                });
            Add(new int?[][]
                {
                    new int?[] {null,    2, null,    6, null,    8, null, null, null},
                    new int?[] {   5,    8, null, null, null,    9,    7, null, null},
                    new int?[] {null, null, null, null,    4, null, null, null, null},
                    new int?[] {   3,    7, null, null, null, null,    5, null, null},
                    new int?[] {   6, null, null, null, null, null, null, null,    4},
                    new int?[] {null, null,    8, null, null, null, null,    1,    3},
                    new int?[] {null, null, null, null,    2, null, null, null, null},
                    new int?[] {null, null,    9,    8, null, null, null,    3,    6},
                    new int?[] {null, null, null,    3, null,    6, null,    9, null},
                });
            Add(new int?[][]
                {
                    new int?[] {   1, null, null, null,    2,    6, null, null, null},
                    new int?[] {   7, null,    6, null, null,    5, null, null, null},
                    new int?[] {null, null,    5,    8,    1, null, null, null, null},
                    new int?[] {null,    5, null, null,    8, null,    1, null, null},
                    new int?[] {null,    2, null, null, null, null, null,    8, null},
                    new int?[] {null, null,    1, null,    6, null, null,    3, null},
                    new int?[] {null, null, null, null,    5,    8,    4, null, null},
                    new int?[] {null, null, null,    6, null, null,    3, null,    9},
                    new int?[] {null, null, null,    2,    4, null, null, null,    5}
                });
            Add(new int?[][]
                {
                    new int?[] {null, null,    6, null,    1, null,    9, null, null},
                    new int?[] {   7, null, null,    3, null, null, null,    6,    5},
                    new int?[] {null, null, null, null,    7, null,    4, null,    8},
                    new int?[] {   6, null, null, null, null,    1, null, null, null},
                    new int?[] {null, null,    2, null, null, null,    5, null, null},
                    new int?[] {null, null, null,    2, null, null, null, null,    9},
                    new int?[] {   2, null,    8, null,    4, null, null, null, null},
                    new int?[] {   1,    3, null, null, null,    7, null, null,    6},
                    new int?[] {null, null,    4, null,    8, null,    1, null, null}
                });
            Add(new int?[][]
                {
                    new int?[] {null, null, null,    6, null, null,    4, null, null},
                    new int?[] {   7, null, null, null, null,    3,    6, null, null},
                    new int?[] {null, null, null, null,    9,    1, null,    8, null},
                    new int?[] {null, null, null, null, null, null, null, null, null},
                    new int?[] {null,    5, null,    1,    8, null, null, null,    3},
                    new int?[] {null, null, null,    3, null,    6, null,    4,    5},
                    new int?[] {null,    4, null,    2, null, null, null,    6, null},
                    new int?[] {   9, null,    3, null, null, null, null, null, null},
                    new int?[] {null,    2, null, null, null, null,    1, null, null}
                });
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
                });
        }
    }
}