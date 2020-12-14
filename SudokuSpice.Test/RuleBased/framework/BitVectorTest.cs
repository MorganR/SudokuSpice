using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SudokuSpice.RuleBased.Test
{
    public class BitVectorTest
    {
        [Theory]
        [InlineData(0)]
        [InlineData(uint.MaxValue)]
        [InlineData(0b10101)]
        public void Constructor_Succeeds(uint data)
        {
            var vector = new BitVector(data);
            Assert.Equal(data, vector.Data);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(2, 0b11)]
        [InlineData(8, 0b1111_1111)]
        public void CreateWithSize_ValidValues_Succeeds(int size, uint data)
        {
            var vector = BitVector.CreateWithSize(size);
            Assert.Equal(data, vector.Data);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(33)]
        public void CreateWithSize_InvalidValues_ThrowsOutOfRange(int size) => Assert.Throws<ArgumentOutOfRangeException>("size", () => BitVector.CreateWithSize(size));

        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(0, uint.MaxValue, 0)]
        [InlineData(0b1101_1001, 0b0100_0111, 0b0100_0001)]
        public void FindIntersect_IsCorrect(uint dataA, uint dataB, uint intersectData)
        {
            var vectorA = new BitVector(dataA);
            var vectorB = new BitVector(dataB);
            Assert.Equal(new BitVector(intersectData), BitVector.FindIntersect(vectorA, vectorB));
        }

        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(0, uint.MaxValue, uint.MaxValue)]
        [InlineData(0b1101_1001, 0b0100_0111, 0b1101_1111)]
        public void FindUnion_IsCorrect(uint dataA, uint dataB, uint unionData)
        {
            var vectorA = new BitVector(dataA);
            var vectorB = new BitVector(dataB);
            Assert.Equal(new BitVector(unionData), BitVector.FindUnion(vectorA, vectorB));
        }

        [Fact]
        public void SetBit_Succeeds()
        {
            var vector = new BitVector(0b1001);
            vector.SetBit(0);
            Assert.Equal<uint>(0b1001, vector.Data);
            vector.SetBit(1);
            Assert.Equal<uint>(0b1011, vector.Data);
        }

        [Fact]
        public void UnsetBit_Succeeds()
        {
            var vector = new BitVector(0b1001);
            vector.UnsetBit(1);
            Assert.Equal<uint>(0b1001, vector.Data);
            vector.UnsetBit(3);
            Assert.Equal<uint>(0b0001, vector.Data);
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
        [InlineData(uint.MaxValue, 32)]
        public void Count_OnConstruction_IsCorrect(uint data, int numBits)
        {
            var vector = new BitVector(data);
            Assert.Equal(numBits, vector.Count);
        }

        [Fact]
        public void Count_AfterMutations_IsCorrect()
        {
            var vector = new BitVector(0b1011);

            // Mutate (set)
            vector.SetBit(6);
            vector.SetBit(8);
            Assert.Equal(5, vector.Count);

            // No-op (set)
            vector.SetBit(8);
            Assert.Equal(5, vector.Count);

            // Mutate (unset)
            vector.UnsetBit(0);
            Assert.Equal(4, vector.Count);

            // No-op (unset)
            vector.UnsetBit(0);
            Assert.Equal(4, vector.Count);
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(0b100, false)]
        [InlineData(0b0011_1101, false)]
        [InlineData(uint.MaxValue, false)]
        public void IsEmpty_Succeeds(uint data, bool isEmpty)
        {
            var vector = new BitVector(data);
            Assert.Equal(isEmpty, vector.IsEmpty());
        }

        [Theory]
        [InlineData(0, new int[] { })]
        [InlineData(0b100, new int[] { 2 })]
        [InlineData(0b0011_1101, new int[] { 0, 2, 3, 4, 5 })]
        public void GetSetBits_Succeeds(uint data, int[] setBits)
        {
            var vector = new BitVector(data);
            Assert.Equal(setBits, vector.GetSetBits());
        }

        [Theory]
        [InlineData(0, new int[] { })]
        [InlineData(0b100, new int[] { 2 })]
        [InlineData(0b0011_1101, new int[] { 0, 2, 3, 4, 5 })]
        public void PopulateSetBits_Succeeds(uint data, int[] expectedSetBits)
        {
            var vector = new BitVector(data);
            Span<int> setBits = stackalloc int[32];

            var numSetBits = vector.PopulateSetBits(setBits);

            Assert.Equal(expectedSetBits.Length, numSetBits);
            Assert.Equal(expectedSetBits, setBits[0..numSetBits].ToArray());
        }

        [Fact]
        public void PopulateSetBits_WithSpanLongerThanBitVector_Succeeds()
        {
            var vector = new BitVector(uint.MaxValue);
            Span<int> setBits = stackalloc int[64];

            var numSetBits = vector.PopulateSetBits(setBits);

            Assert.Equal(32, numSetBits);
            Assert.Equal(Enumerable.Range(0, 32), setBits[0..numSetBits].ToArray());
        }

        [Fact]
        public void PopulateSetBits_WithSpanShorterThanNumSet_Succeeds()
        {
            var vector = new BitVector(uint.MaxValue);
            Span<int> setBits = stackalloc int[5];

            var numSetBits = vector.PopulateSetBits(setBits);

            Assert.Equal(5, numSetBits);
            Assert.Equal(Enumerable.Range(0, 5), setBits[0..numSetBits].ToArray());
        }

        [Theory]
        [InlineData(0, 0, true)]
        [InlineData(0b1101, 0b1101, true)]
        [InlineData(0b0011_1101, 0b1111_1111, false)]
        [InlineData(0b1111, 0, false)]
        [InlineData(0, 0b1111, false)]
        public void Equals_WithBitVector_IsCorrect(uint dataA, uint dataB, bool isEqual)
        {
            var vectorA = new BitVector(dataA);
            var vectorB = new BitVector(dataB);
            Assert.Equal(isEqual, vectorA.Equals(vectorB));
            Assert.Equal(isEqual, vectorA == vectorB);
            Assert.NotEqual(isEqual, vectorA != vectorB);
        }

        [Theory]
        [InlineData(0, 0, false)]
        [InlineData(0, 1, false)]
        [InlineData(5, 5.0, false)]
        [InlineData(0, null, false)]
        public void Equals_WithObject_IsCorrect(uint data, object other, bool isEqual)
        {
            var vector = new BitVector(data);
            Assert.Equal(isEqual, vector.Equals(other));
        }

        [Fact]
        public void Equals_OtherClass_IsFalse()
        {
            var vector = new BitVector(5);
            Assert.False(vector.Equals(new List<int>()));
        }

        [Theory]
        [InlineData(0, "0")]
        [InlineData(0b1011, "1011")]
        [InlineData(0b1011_0001, "10110001")]
        public void ToString_ReturnsExpected(uint data, string result)
        {
            var vector = new BitVector(data);
            Assert.Equal(result, vector.ToString());
        }
    }
}