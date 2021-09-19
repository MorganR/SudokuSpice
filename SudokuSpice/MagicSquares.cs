using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SudokuSpice
{
    /// <summary>
    /// Utility methods for working with
    /// <a href="https://en.wikipedia.org/wiki/Magic_square">magic squares</a>, i.e. the sums of the
    /// values in each of their rows, columns, and (optionally) their diagonals add up to the same
    /// value.
    ///
    /// These all require the possible values in the magic square to be unique.
    /// </summary>
    public static class MagicSquares
    {
        /// <summary>
        /// Computes the "sum" that each row/column/diagonal must add up to.
        /// </summary>
        /// <param name="values">
        /// All the possible values for the magic square. Each value must be unique.
        /// </param>.
        /// <exception cref="ArgumentException">
        /// Thrown if no sum is possible.
        /// </exception>
        public static int ComputeSum(ReadOnlySpan<int> values)
        {
            int boxSize = Boxes.IntSquareRoot(values.Length);
            return _ComputeSum(values, boxSize);
        }

        /// <summary>
        /// Computes each unique set of values of length SquareRoot(values.Length) that can be
        /// formed from the given values.
        /// </summary>
        /// <param name="values">
        /// All the possible values for the magic square. Each value must be unique.
        /// </param>
        public static HashSet<BitVector> ComputeSets(ReadOnlySpan<int> values)
        {
            int boxSize = Boxes.IntSquareRoot(values.Length);
            BitVector allPossibleValues = new BitVector();
            for (int i = 0; i < values.Length; ++i)
            {
                if (allPossibleValues.IsBitSet(values[i]))
                {
                    throw new ArgumentException("Values must be unique.");
                }
                allPossibleValues.SetBit(values[i]);
            }
            return ComputeSets(values, boxSize, allPossibleValues);
        }

        /// <summary>
        /// Computes each unique set of values of length <paramref name="boxSize"/> that can be
        /// formed from the given values.
        /// </summary>
        /// <param name="values">
        /// All the possible values for the magic square. Each value must be unique. Values must
        /// have length `boxSize*boxSize`.
        /// </param>
        /// <param name="allPossibleValues">
        /// The 
        /// </param>
        public static HashSet<BitVector> ComputeSets(ReadOnlySpan<int> values, int boxSize, BitVector allPossibleValues)
        {
            if (values.Length != boxSize * boxSize)
            {
                throw new ArgumentException(
                    $"{nameof(boxSize)} ({boxSize}) must be the exact square root of the length of {nameof(values)} ({values.Length}).");
            }
            if (allPossibleValues.ComputeCount() != values.Length)
            {
                throw new ArgumentException($"Expected {nameof(allPossibleValues)} to have the same length as {nameof(values)}.");
            }
                
            int magicSum = _ComputeSum(values, boxSize);
            var sets = new HashSet<BitVector>();
            for (int i = 0; i < values.Length; ++i)
            {
                int possibleValue = values[i];
                var set = new BitVector();
                set.SetBit(possibleValue);
                BitVector possibleValues = allPossibleValues;
                possibleValues.UnsetBit(possibleValue);
                sets = new(sets.Union(_ComputeMagicSetsForRemainder(boxSize - 1, magicSum - possibleValue, possibleValues, set)));
            }
            return sets;
        }

        private static int _ComputeSum(ReadOnlySpan<int> values, int boxSize)
        {
            int sum = 0;
            for (int i = 0; i < values.Length; ++i)
            {
                sum += values[i];
            }
            int result = Math.DivRem(sum, boxSize, out int remainder);
            if (remainder == 0)
            {
                return result;
            }
            throw new ArgumentException(
                "To satisy a magic square, the sum of the given values must be evenly divisible by the side-length of the square.");
        }

        private static HashSet<BitVector> _ComputeMagicSetsForRemainder(int remainingSize, int remainder, BitVector possibleValues, BitVector partialSet)
        {
            var result = new HashSet<BitVector>();
            if (remainingSize == 0)
            {
                if (remainder == 0)
                {
                    result.Add(partialSet);
                }
                return result;
            }
            Span<int> setBits = stackalloc int[BitVector.NumBits];
            int numSetBits = possibleValues.PopulateSetBits(setBits);
            foreach (var possibleValue in setBits.Slice(0, numSetBits))
            {
                if (remainder - possibleValue >= 0)
                {
                    BitVector set = partialSet;
                    set.SetBit(possibleValue);
                    BitVector reducedPossibleValues = possibleValues;
                    reducedPossibleValues.UnsetBit(possibleValue);
                    result = new(result.Union(_ComputeMagicSetsForRemainder(remainingSize - 1, remainder - possibleValue, reducedPossibleValues, set)));
                }
            }
            return result;
        }
    }
}