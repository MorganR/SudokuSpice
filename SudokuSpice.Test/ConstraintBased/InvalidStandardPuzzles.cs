using Xunit;

namespace SudokuSpice.ConstraintBased.Test
{
    public sealed class InvalidStandardPuzzles : TheoryData<Puzzle>
    {
        public InvalidStandardPuzzles()
        {
            // Duplicate in row.
            Add(new Puzzle(
                    new int?[,] {
                        {    1, null,    1, null},
                        {    2, null, null, null},
                        {    3, null, null, null},
                        {    4, null, null, null},
                    }));
            // Duplicate in column.
            Add(new Puzzle(
                    new int?[,] {
                        {    1,    2,    3,    4},
                        { null, null, null, null},
                        {    3, null, null, null},
                        {    1, null, null, null},
                    }));
            // Duplicate in box.
            Add(new Puzzle(
                    new int?[,] {
                        {    1,    2,    3,    4},
                        {    2, null, null, null},
                        {    3, null, null, null},
                        {    4, null, null, null},
                    }));
            // Unsolvable.
            Add(new Puzzle(
                    new int?[,] {
                        {    1, null,    3,    4},
                        { null, null,    1,    2},
                        {    3, null,    2, null},
                        {    4, null, null, null},
                    }));
        }
    }
}
