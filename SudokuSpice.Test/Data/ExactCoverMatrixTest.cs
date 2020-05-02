using System.Linq;
using Xunit;

namespace SudokuSpice.Data.Test {
    public class ExactCoverMatrixTest
    {
        [Fact]
        public void GetSquare_ReturnsSquareWithCorrectCoordinates()
        {
            ExactCoverMatrix matrix = new ExactCoverMatrix(4, new int[] { 1, 2, 3, 4 });

            Assert.Equal(new Coordinate(0, 0), matrix.GetSquare(new Coordinate(0, 0)).Coordinate);
            Assert.Equal(new Coordinate(1, 2), matrix.GetSquare(new Coordinate(1, 2)).Coordinate);
            Assert.Equal(new Coordinate(3, 3), matrix.GetSquare(new Coordinate(3, 3)).Coordinate);
        }

        [Fact]
        public void GetSquare_ReturnsSquareWithExpectedPossibleValues()
        {
            var expectedPossibleValues = new int[] { 0, 2, 4, 5 };
            ExactCoverMatrix matrix = new ExactCoverMatrix(4, expectedPossibleValues);

            var square = matrix.GetSquare(new Coordinate(0, 0));
            Assert.Equal(expectedPossibleValues.Length, square.NumPossibleValues);
            Assert.Equal(square.AllPossibleValues.ToArray(), square.GetStillPossibleValues());
            Assert.Equal(expectedPossibleValues, square.AllPossibleValues.ToArray().Select(pv => pv.Value).ToArray());
            Assert.Equal(4, square.GetPossibleValue(4).Value);
        }

        [Fact]
        public void GetSquaresOnRow_ReturnsExpectedSquares()
        {
            ExactCoverMatrix matrix = new ExactCoverMatrix(4, new int[] { 1, 2, 3, 4 });

            int row = 1;
            var squares = matrix.GetSquaresOnRow(row);
            Assert.Equal(4, squares.Length);
            Assert.Equal(new Coordinate(row, 0), squares[0].Coordinate);
            Assert.Equal(new Coordinate(row, 1), squares[1].Coordinate);
            Assert.Equal(new Coordinate(row, 2), squares[2].Coordinate);
            Assert.Equal(new Coordinate(row, 3), squares[3].Coordinate);
        }

        [Fact]
        public void GetSquaresOnColumn_ReturnsExpectedSquares()
        {
            ExactCoverMatrix matrix = new ExactCoverMatrix(4, new int[] { 1, 2, 3, 4 });

            int column = 1;
            var squares = matrix.GetSquaresOnColumn(column);
            Assert.Equal(4, squares.Count);
            Assert.Equal(new Coordinate(0, column), squares[0].Coordinate);
            Assert.Equal(new Coordinate(1, column), squares[1].Coordinate);
            Assert.Equal(new Coordinate(2, column), squares[2].Coordinate);
            Assert.Equal(new Coordinate(3, column), squares[3].Coordinate);
        }
    }
}
