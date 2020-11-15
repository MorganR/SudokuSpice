using System.Collections.Generic;
using Xunit;

namespace SudokuSpice.Test
{
    public class SolveStatsTest
    {
        [Theory]
        [MemberData(nameof(EqualSolveStats))]
        public void Equals_WithEqualSolveStats_IsTrue(SolveStats a, SolveStats b)
        {
            Assert.Equal(a, b);
            Assert.True(a.Equals(b));
            Assert.True(a == b);
            Assert.False(a != b);
        }

        public static IEnumerable<object[]> EqualSolveStats()
        {
            yield return new object[]
            {
                new SolveStats(),
                new SolveStats(),
            };
            yield return new object[]
            {
                new SolveStats() {NumSolutionsFound = 1, NumSquaresGuessed = 2, NumTotalGuesses = 3},
                new SolveStats() {NumSolutionsFound = 1, NumSquaresGuessed = 2, NumTotalGuesses = 3},
            };
        }

        [Theory]
        [MemberData(nameof(UnequalSolveStats))]
        public void Equals_WithUnequalSolveStats_IsFalse(SolveStats a, SolveStats b)
        {
            Assert.NotEqual(a, b);
            Assert.False(a.Equals(b));
            Assert.False(a == b);
            Assert.True(a != b);
        }

        public static IEnumerable<object[]> UnequalSolveStats()
        {
            yield return new object[]
            {
                new SolveStats(),
                new SolveStats() { NumSolutionsFound = 1},
            };
            yield return new object[]
            {
                new SolveStats(),
                new SolveStats() { NumSquaresGuessed = 1 },
            };
            yield return new object[]
            {
                new SolveStats(),
                new SolveStats() { NumTotalGuesses = 1 },
            };
            yield return new object[]
            {
                new SolveStats() { NumSolutionsFound = 1, NumSquaresGuessed = 3, NumTotalGuesses = 5 },
                new SolveStats() { NumSolutionsFound = 10, NumSquaresGuessed = 30, NumTotalGuesses = 50 },
            };
        }

        [Theory]
        [MemberData(nameof(SolveStatsAndObjectsForEquality))]
        public void Equals_WithObject_IsCorrect(SolveStats stats, object other, bool isEqual)
        {
            Assert.Equal(isEqual, stats.Equals(other));
        }

        public static IEnumerable<object[]> SolveStatsAndObjectsForEquality()
        {
            yield return new object[]
            {
                new SolveStats(),
                new SolveStats(),
                true,
            };
            yield return new object[]
            {
                new SolveStats(),
                new BitVector(),
                false,
            };
            yield return new object[]
            {
                new SolveStats(),
                null,
                false,
            };
            yield return new object[]
            {
                new SolveStats(),
                new List<SolveStats>(),
                false,
            };
        }

        [Fact]
        public void ToString_IsCorrect()
        {
            var stats = new SolveStats() { NumSolutionsFound = 1, NumSquaresGuessed = 2, NumTotalGuesses = 3 };
            Assert.Equal("NumSolutionsFound: 1, NumSquaresGuessed: 2, NumTotalGuesses: 3", stats.ToString());
        }
    }
}
