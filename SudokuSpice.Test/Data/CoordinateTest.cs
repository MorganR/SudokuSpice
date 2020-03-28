using Xunit;

namespace SudokuSpice.Data.Test
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
            var c = new Coordinate(1, 2);
            var other = new Coordinate(1, 2);
            Assert.True(c.Equals(other));
            Assert.True(c == other);
            Assert.False(c != other);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        [InlineData(5, -100)]
        public void EqualsIsFalseForDifferentCoordinates(int otherRow, int otherCol)
        {
            var c = new Coordinate(1, 2);
            var other = new Coordinate(otherRow, otherCol);
            Assert.False(c.Equals(other));
            Assert.False(c == other);
            Assert.True(c != other);
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
