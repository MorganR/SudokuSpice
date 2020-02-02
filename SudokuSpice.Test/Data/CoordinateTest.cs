using Xunit;

namespace SudokuSpice
{
    public class CoordinateTest
    {
        [Fact]
        public void RowReturnsRow()
        {
            var coord = new Coordinate(1, 2);
            Assert.Equal(1, coord.Row);
        }

        [Fact]
        public void ColumnReturnsColumn()
        {
            var coord = new Coordinate(1, 2);
            Assert.Equal(2, coord.Column);
        }

        [Fact]
        public void DeconstructReturnsRowThenColumn()
        {
            var coord = new Coordinate(1, 2);
            (int row, int column) = coord;
            Assert.Equal(1, row);
            Assert.Equal(2, column);
        }

        [Fact]
        public void ToStringPrintsRowThenColumn()
        {
            var coord = new Coordinate(1, 2);
            Assert.Equal("(1, 2)", coord.ToString());
        }

        [Fact]
        public void EqualsIsTrueForMatchingCoordinate()
        {
            var obj = new Coordinate(1, 2);
            Assert.True(obj.Equals(new Coordinate(1, 2)));
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        [InlineData(5, -100)]
        public void EqualsIsFalseForDifferentCoordinates(int otherRow, int otherCol)
        {
            var obj = new Coordinate(1, 2);
            Assert.False(obj.Equals(new Coordinate(otherRow, otherCol)));
        }

        [Fact]
        public void EqualsIsFalseForNull()
        {
            var obj = new Coordinate(1, 2);
            Assert.False(obj.Equals(null));
        }

        [Fact]
        public void EqualsIsFalseForTuple()
        {
            var obj = new Coordinate(1, 2);
            Assert.False(obj.Equals((1, 2)));
        }
    }
}
