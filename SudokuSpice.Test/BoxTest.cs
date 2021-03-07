using System;
using Xunit;

namespace SudokuSpice.Test
{
    public class BoxTest
    {
        [Fact]
        public void Equals_WithEqualBoxes_IsTrue()
        {
            var box = new Box(new Coordinate(1, 4), 3);

            Assert.Equal(box, new Box(new Coordinate(1, 4), 3));
        }

        [Theory]
        [InlineData(0, 0, 1)]
        [InlineData(0, 1, 3)]
        [InlineData(1, 0, 3)]
        [InlineData(1, 4, 5)]
        public void Equals_WithUnequalBoxes_IsFalse(int row, int column, int size)
        {
            var box = new Box(new Coordinate(0, 0), 3);

            Assert.NotEqual(box, new Box(new Coordinate(row, column), size));
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(1, 3)]
        [InlineData(2, 2)]
        [InlineData(2, 3)]
        public void Contains_WithContainedCoordinates_IsTrue(int row, int column)
        {
            var box = new Box(new Coordinate(1, 2), 2);

            Assert.True(box.Contains(new Coordinate(row, column)));
        }

        [Theory]
        [InlineData(0, 2)]
        [InlineData(1, 1)]
        [InlineData(2, 4)]
        [InlineData(3, 2)]
        [InlineData(3, 3)]
        [InlineData(3, 4)]
        public void Contains_WithExternalCoordiantes_IsFalse(int row, int column)
        {
            var box = new Box(new Coordinate(1, 2), 2);

            Assert.False(box.Contains(new Coordinate(row, column)));
        }

        [Fact]
        public void Contains_WithSizeOne_OnlyIncludesTopLeft()
        {
            var box = new Box(new Coordinate(1, 1), 1);

            Assert.True(box.Contains(new Coordinate(1, 1)));
            Assert.False(box.Contains(new Coordinate(0, 1)));
            Assert.False(box.Contains(new Coordinate(1, 0)));
            Assert.False(box.Contains(new Coordinate(0, 0)));
            Assert.False(box.Contains(new Coordinate(1, 2)));
            Assert.False(box.Contains(new Coordinate(2, 1)));
            Assert.False(box.Contains(new Coordinate(2, 2)));
        }

        [Fact]
        public void Constructor_WithSizeZero_Throws()
        {
            Assert.Throws<ArgumentException>(
                () => new Box(new Coordinate(1, 1), 0));
        }
    }
}