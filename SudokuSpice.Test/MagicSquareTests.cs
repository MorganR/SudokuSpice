using Xunit;

namespace SudokuSpice.Test
{
    internal static class MagicSquareTests
    {
        internal static void AssertMagicSquaresSatisfied(
            IReadOnlyPuzzle puzzle, int expectedSum, bool verifyDiagonals)
        {
            int boxSize = Boxes.IntSquareRoot(puzzle.Size);
            var boxes = new Box[puzzle.Size];
            for (int boxIdx = 0; boxIdx < boxes.Length; ++boxIdx)
            {
                boxes[boxIdx] = new Box(Boxes.GetStartingBoxCoordinate(boxIdx, boxSize), boxSize);
            }
            AssertMagicSquaresSatisfied(puzzle, boxes, expectedSum, verifyDiagonals);
        }

        internal static void AssertMagicSquaresSatisfied(
            IReadOnlyPuzzle puzzle, Box[] boxesToCheck, int expectedSum, bool verifyDiagonals)
        {
            foreach (Box box in boxesToCheck)
            {
                int boxSize = box.Size;
                var rowSums = new int[boxSize];
                var colSums = new int[boxSize];
                var startCoord = box.TopLeft;
                var endCoord = new Coordinate(startCoord.Row + boxSize, startCoord.Column + boxSize);
                for (int row = startCoord.Row; row < endCoord.Row; ++row)
                {
                    for (int col = startCoord.Column; col < endCoord.Column; ++col)
                    {
                        var value = puzzle[row, col].Value;
                        rowSums[row - startCoord.Row] += value;
                        colSums[col - startCoord.Column] += value;
                    }
                }
                Assert.All(rowSums, sum => Assert.Equal(expectedSum, sum));
                Assert.All(colSums, sum => Assert.Equal(expectedSum, sum));
                if (verifyDiagonals)
                {
                    int diagSum = 0;
                    for (Coordinate coord = startCoord; coord != endCoord; coord = new Coordinate(coord.Row + 1, coord.Column + 1))
                    {
                        var value = puzzle[in coord].Value;
                        diagSum += value;
                    }
                    Assert.Equal(expectedSum, diagSum);
                    diagSum = 0;
                    var topRightCoord = new Coordinate(startCoord.Row, endCoord.Column - 1);
                    var bottomLeftEndCoord = new Coordinate(endCoord.Row, startCoord.Column - 1);
                    for (Coordinate coord = topRightCoord; coord != bottomLeftEndCoord; coord = new Coordinate(coord.Row + 1, coord.Column - 1))
                    {
                        var value = puzzle[in coord].Value;
                        diagSum += value;
                    }
                    Assert.Equal(expectedSum, diagSum);
                }
            }
        }
    }
}