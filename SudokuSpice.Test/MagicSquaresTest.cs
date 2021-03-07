using System;
using System.Collections.Generic;
using Xunit;

namespace SudokuSpice.Test
{
    public class MagicSquaresTest
    {
        [Theory]
        [InlineData(1, 1)]
        [InlineData(4, 5)]
        [InlineData(9, 15)]
        [InlineData(16, 34)]
        public void ComputeSum_ForStandardValues_Works(int size, int expectedSum)
        {
            var values = new int[size];
            for (int i = 0; i < size; ++i)
            {
                values[i] = i + 1;
            }
            Assert.Equal(expectedSum, MagicSquares.ComputeSum(values));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(8)]
        public void ComputeSum_ForNonSquareSizes_Throws(int size)
        {
            var values = new int[size];
            for (int i = 0; i < size; ++i)
            {
                values[i] = i + 1;
            }
            Assert.Throws<ArgumentException>(() => MagicSquares.ComputeSum(values));
        }

        [Fact]
        public void ComputeSets_ForNonUniqueValues_Throws()
        {
            var values = new int[4];
            values.AsSpan().Fill(3);
            Assert.Throws<ArgumentException>(() => MagicSquares.ComputeSets(values));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(8)]
        public void ComputeSets_ForNonSquareSizes_Throws(int size)
        {
            var values = new int[size];
            for (int i = 0; i < size; ++i)
            {
                values[i] = i + 1;
            }
            Assert.Throws<ArgumentException>(() => MagicSquares.ComputeSets(values));
        }

        [Fact]
        public void ComputeSets_ForValidValues_Succeeds()
        {
            int size = 9;
            var values = new int[size];
            for (int i = 0; i < size; ++i)
            {
                values[i] = i + 1;
            }

            Assert.Equal(
                new HashSet<BitVector> {
                    new BitVector(0b1000100010), // 9 5 1
                    new BitVector(0b0101000010), // 8 6 1
                    new BitVector(0b1000010100), // 9 4 2
                    new BitVector(0b0100100100), // 8 5 2
                    new BitVector(0b0011000100), // 7 6 2
                    new BitVector(0b0100011000), // 8 4 3
                    new BitVector(0b0010101000), // 7 5 3
                    new BitVector(0b0001110000), // 6 5 4
                },
                MagicSquares.ComputeSets(values));
        }

        [Fact]
        public void ComputeSets_WithValidSizeAndVector_Succeeds()
        {
            int size = 9;
            var values = new int[size];
            for (int i = 0; i < size; ++i)
            {
                values[i] = i + 1;
            }

            Assert.Equal(
                new HashSet<BitVector> {
                    new BitVector(0b1000100010), // 9 5 1
                    new BitVector(0b0101000010), // 8 6 1
                    new BitVector(0b1000010100), // 9 4 2
                    new BitVector(0b0100100100), // 8 5 2
                    new BitVector(0b0011000100), // 7 6 2
                    new BitVector(0b0100011000), // 8 4 3
                    new BitVector(0b0010101000), // 7 5 3
                    new BitVector(0b0001110000), // 6 5 4
                },
                MagicSquares.ComputeSets(values, 3, new BitVector(0b1111111110)));
        }

    }
}