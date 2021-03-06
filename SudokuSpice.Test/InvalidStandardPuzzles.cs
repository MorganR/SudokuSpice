using Xunit;

namespace SudokuSpice.Test
{
    public sealed class InvalidStandardPuzzles : TheoryData<int?[][]>
    {
        public InvalidStandardPuzzles()
        {
            // Duplicate in row.
            Add(new int?[][] {
                    new int?[] {    1, null,    1, null},
                    new int?[] {    2, null, null, null},
                    new int?[] {    3, null, null, null},
                    new int?[] {    4, null, null, null},
                });
            // Duplicate in column.
            Add(new int?[][] {
                    new int?[] {    1,    2,    3,    4},
                    new int?[] { null, null, null, null},
                    new int?[] {    3, null, null, null},
                    new int?[] {    1, null, null, null},
                });
            // Duplicate in box.
            Add(new int?[][] {
                    new int?[] {    1,    2,    3,    4},
                    new int?[] {    2, null, null, null},
                    new int?[] {    3, null, null, null},
                    new int?[] {    4, null, null, null},
                });
            // Unsolvable.
            Add(new int?[][] {
                    new int?[] {    1, null,    3,    4},
                    new int?[] { null, null,    1,    2},
                    new int?[] {    3, null,    2, null},
                    new int?[] {    4, null, null, null},
                });
        }
    }
}