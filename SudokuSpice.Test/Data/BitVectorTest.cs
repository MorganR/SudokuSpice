using Xunit;

namespace SudokuSpice
{
    public class BitVectorTest
    {
        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(2, 0b11)]
        [InlineData(8, 0b1111_1111)]
        public void CreateWithSize_Succeeds(int size, int data)
        {
            var vector = BitVector.CreateWithSize(size);
            Assert.Equal(data, (int) vector);
        }

        [Fact]
        public void SetBit_Succeeds()
        {
            var vector = new BitVector(0b1001);
            vector.SetBit(0);
            Assert.Equal(0b1001, (int) vector);
            vector.SetBit(1);
            Assert.Equal(0b1011, (int) vector);
        }

        [Fact]
        public void UnsetBit_Succeeds()
        {
            var vector = new BitVector(0b1001);
            vector.UnsetBit(1);
            Assert.Equal(0b1001, (int) vector);
            vector.UnsetBit(3);
            Assert.Equal(0b0001, (int) vector);
        }

        [Fact]
        public void IsBitSet_Succeeds()
        {
            var vector = new BitVector(0b1001_1100);
            Assert.False(vector.IsBitSet(0));
            Assert.False(vector.IsBitSet(1));
            Assert.False(vector.IsBitSet(5));
            Assert.False(vector.IsBitSet(6));
            Assert.True(vector.IsBitSet(2));
            Assert.True(vector.IsBitSet(3));
            Assert.True(vector.IsBitSet(4));
            Assert.True(vector.IsBitSet(7));
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(0b100, 1)]
        [InlineData(0b0011_1101, 5)]
        [InlineData(-1, 32)]
        public void CountSetBits_Succeeds(int data, int numBits)
        {
            var vector = new BitVector(data); 
            Assert.Equal(numBits, vector.CountSetBits());
        }

        [Theory]
        [InlineData(0, new int[] { })]
        [InlineData(0b100, new int[] { 2 })]
        [InlineData(0b0011_1101, new int[] { 0, 2, 3, 4, 5 })]
        public void GetSetBits_Succeeds(int data, int[] setBits)
        {
            var vector = new BitVector(data); 
            Assert.Equal(setBits, vector.GetSetBits());
        }

        [Theory]
        [InlineData(0b0000, 4, new int[] { })]
        [InlineData(0b0100, 4, new int[] { 2 })]
        [InlineData(0b0011_1101, 4, new int[] { 0, 2, 3 })]
        [InlineData(0b1111, 0, new int[] { })]
        public void GetSetBits_WithMaxBitCount_LimitsToFirstBits(int data, int maxBitCount, int[] setBits)
        {
            var vector = new BitVector(data); 
            Assert.Equal(setBits, vector.GetSetBits(maxBitCount));
        }
    }
}
