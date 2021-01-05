using Xunit;

namespace SudokuSpice.Test
{
    public sealed class ValidStandardPuzzles : TheoryData<int?[,]>
    {
        public ValidStandardPuzzles()
        {
            Add(new int?[,]
                {
                    {   1, null, null,    2},
                    {null, null,    1, null},
                    {null,    1, null, null},
                    {   3, null,    4, null}
                });
            Add(new int?[,]
                {
                    {   4, null,    2, null, null,    1,    8,    7,    6},
                    {   3, null,    8, null, null,    5, null,    9,    4},
                    {   6, null,    9,    4, null,    8,    3, null,    5},
                    {null,    3,    1, null,    6, null, null, null, null},
                    {   2,    4,    5,    9, null,    7,    1,    6,    3},
                    {   9, null,    7,    2, null,    3,    5,    4,    8},
                    {null,    9, null,    8, null,    2, null, null, null},
                    {   1,    8,    3, null,    4,    9,    6,    5,    2},
                    {   5,    2,    4,    1,    3,    6,    9, null,    7}
                });
            Add(new int?[,]
                {
                    {null,    2, null,    6, null,    8, null, null, null},
                    {   5,    8, null, null, null,    9,    7, null, null},
                    {null, null, null, null,    4, null, null, null, null},
                    {   3,    7, null, null, null, null,    5, null, null},
                    {   6, null, null, null, null, null, null, null,    4},
                    {null, null,    8, null, null, null, null,    1,    3},
                    {null, null, null, null,    2, null, null, null, null},
                    {null, null,    9,    8, null, null, null,    3,    6},
                    {null, null, null,    3, null,    6, null,    9, null},
                });
            Add(new int?[,]
                {
                    {   1, null, null, null,    2,    6, null, null, null},
                    {   7, null,    6, null, null,    5, null, null, null},
                    {null, null,    5,    8,    1, null, null, null, null},
                    {null,    5, null, null,    8, null,    1, null, null},
                    {null,    2, null, null, null, null, null,    8, null},
                    {null, null,    1, null,    6, null, null,    3, null},
                    {null, null, null, null,    5,    8,    4, null, null},
                    {null, null, null,    6, null, null,    3, null,    9},
                    {null, null, null,    2,    4, null, null, null,    5}
                });
            Add(new int?[,]
                {
                    {null, null,    6, null,    1, null,    9, null, null},
                    {   7, null, null,    3, null, null, null,    6,    5},
                    {null, null, null, null,    7, null,    4, null,    8},
                    {   6, null, null, null, null,    1, null, null, null},
                    {null, null,    2, null, null, null,    5, null, null},
                    {null, null, null,    2, null, null, null, null,    9},
                    {   2, null,    8, null,    4, null, null, null, null},
                    {   1,    3, null, null, null,    7, null, null,    6},
                    {null, null,    4, null,    8, null,    1, null, null}
                });
            Add(new int?[,]
                {
                    {null, null, null,    6, null, null,    4, null, null},
                    {   7, null, null, null, null,    3,    6, null, null},
                    {null, null, null, null,    9,    1, null,    8, null},
                    {null, null, null, null, null, null, null, null, null},
                    {null,    5, null,    1,    8, null, null, null,    3},
                    {null, null, null,    3, null,    6, null,    4,    5},
                    {null,    4, null,    2, null, null, null,    6, null},
                    {   9, null,    3, null, null, null, null, null, null},
                    {null,    2, null, null, null, null,    1, null, null}
                });
            Add(new int?[,]
                {
                    {null,    2, null, null, null, null, null, null, null},
                    {null, null, null,    6, null, null, null, null,    3},
                    {null,    7,    4, null,    8, null, null, null, null},
                    {null, null, null, null, null,    3, null, null,    2},
                    {null,    8, null, null,    4, null, null,    1, null},
                    {   6, null, null,    5, null, null, null, null, null},
                    {null, null, null, null,    1, null,    7,    8, null},
                    {   5, null, null, null, null,    9, null, null, null},
                    {null, null, null, null, null, null, null,    4, null}
                });
        }
    }
}