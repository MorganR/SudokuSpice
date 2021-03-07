using System;
using Xunit;

namespace SudokuSpice.Test
{
    public class SpansTest
    {
        [Fact]
        public void RandomPop_WithEmptySpan_Throws()
        {
            int[] array = new int[0];

            Assert.Throws<IndexOutOfRangeException>(
                () => Spans.PopRandom(new Random(), new Span<int>(array)));
        }

        [Fact]
        public void RandomPop_WithOneValue_ReturnsValueAndLeavesUnchanged()
        {
            Span<int> span = stackalloc int[1];
            span[0] = 5;

            Assert.Equal(5, Spans.PopRandom(new Random(), span));
            Assert.Equal(5, span[0]);
        }

        [Fact]
        public void RandomPop_WithManyValues_ReturnsValueAndSwapsValueToEnd()
        {
            Span<int> span = stackalloc int[5] { 0, 1, 2, 3, 4 };

            Assert.Equal(1, Spans.PopRandom(new Random(1234), span));
            Assert.Equal(new int[5] { 0, 4, 2, 3, 1 }, span.ToArray());
        }

        [Fact]
        public void RandomPop_WithStructAndRef_AvoidsCopyDuringReturn()
        {
            Span<BitVector> span = stackalloc BitVector[5] {
                BitVector.CreateWithSize(0),
                BitVector.CreateWithSize(1),
                BitVector.CreateWithSize(2),
                BitVector.CreateWithSize(3),
                BitVector.CreateWithSize(4),
            };

            ref var byReference = ref Spans.PopRandom(new Random(1234), span);
            byReference.SetBit(5);

            Assert.Equal(byReference, span[4]);
        }

        [Fact]
        public void RandomPop_WithStructAndNoRef_CopiesValueDuringReturn()
        {
            Span<BitVector> span = stackalloc BitVector[5] {
                BitVector.CreateWithSize(0),
                BitVector.CreateWithSize(1),
                BitVector.CreateWithSize(2),
                BitVector.CreateWithSize(3),
                BitVector.CreateWithSize(4),
            };

            var byValue = Spans.PopRandom(new Random(1234), span);
            byValue.SetBit(5);

            Assert.NotEqual(byValue, span[4]);
            Assert.Equal(BitVector.CreateWithSize(1), span[4]);
        }

        [Fact]
        public void RandomPop_WithClassAndNoRef_ReturnsOriginalReference()
        {
            var puzzle0 = new Puzzle(1);
            var puzzle1 = new Puzzle(4);
            var puzzle2 = new Puzzle(9);
            var array = new Puzzle[] {
                puzzle0,
                puzzle1,
                puzzle2,
            };

            var shouldBe1 = Spans.PopRandom(new Random(1234), new Span<Puzzle>(array));
            Assert.Same(puzzle1, shouldBe1);
            Assert.Same(shouldBe1, array[array.Length - 1]);
            Assert.Same(puzzle2, array[1]);
        }
    }
}