using Xunit;

namespace SudokuSpice
{
    public class RestrictUtilsTest
    {
        [Fact]
        public void CreateStandardRestricts_CreatesExpectedRestricts()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                {           3,            2,            4,            1}
            });
            var restricts = RestrictUtils.CreateStandardRestricts(puzzle);

            Assert.IsType<RowRestrict>(restricts[0]);
            Assert.IsType<ColumnRestrict>(restricts[1]);
            Assert.IsType<BoxRestrict>(restricts[2]);
        }

        [Fact]
        public void RestrictAllPossibleValues_Succeeds()
        {
            var puzzle = new Puzzle(new int?[,] {
                {           1, null /* 4 */, null /* 3 */,            2},
                {null /* 2 */, null /* 3 */,            1, null /* 4 */},
                {null /* 4 */, null /* 1 */, null /* 2 */, null /* 3 */},
                {           3,            2,            4,            1}
            });
            var restricts = RestrictUtils.CreateStandardRestricts(puzzle);

            RestrictUtils.RestrictAllUnsetPossibleValues(puzzle, restricts);

            Assert.Equal(0b1100, puzzle.GetPossibleValues(0, 1));
            Assert.Equal(0b0100, puzzle.GetPossibleValues(0, 2));
            Assert.Equal(0b1010, puzzle.GetPossibleValues(1, 0));
            Assert.Equal(0b1100, puzzle.GetPossibleValues(1, 1));
            Assert.Equal(0b1100, puzzle.GetPossibleValues(1, 3));
            Assert.Equal(0b1000, puzzle.GetPossibleValues(2, 0));
            Assert.Equal(0b1001, puzzle.GetPossibleValues(2, 1));
            Assert.Equal(0b0110, puzzle.GetPossibleValues(2, 2));
            Assert.Equal(0b0100, puzzle.GetPossibleValues(2, 3));
        }
    }
}
