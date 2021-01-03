using Xunit;

namespace SudokuSpice.Test
{
    public sealed class InvalidStandardPuzzles : TheoryData<int?[,]>
    {
        public InvalidStandardPuzzles()
        {
            // Duplicate in row.
            Add(new int?[,] {
                    {    1, null,    1, null},
                    {    2, null, null, null},
                    {    3, null, null, null},
                    {    4, null, null, null},
                });
            // Duplicate in column.
            Add(new int?[,] {
                    {    1,    2,    3,    4},
                    { null, null, null, null},
                    {    3, null, null, null},
                    {    1, null, null, null},
                });
            // Duplicate in box.
            Add(new int?[,] {
                    {    1,    2,    3,    4},
                    {    2, null, null, null},
                    {    3, null, null, null},
                    {    4, null, null, null},
                });
            // Unsolvable.
            Add(new int?[,] {
                    {    1, null,    3,    4},
                    { null, null,    1,    2},
                    {    3, null,    2, null},
                    {    4, null, null, null},
                });
        }
    }
}
