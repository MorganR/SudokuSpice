using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SudokuSpice
{
    public static class MagicSquares
    {
        public static int ComputeSum(ReadOnlySpan<int> values)
        {
            int boxSize = Boxes.CalculateBoxSize(values.Length);
            return _ComputeSum(values, boxSize);
        }

        public static HashSet<BitVector> ComputeSets(ReadOnlySpan<int> values)
        {
            int boxSize = Boxes.CalculateBoxSize(values.Length);
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

        public static HashSet<BitVector> ComputeSets(ReadOnlySpan<int> values, int boxSize, BitVector allPossibleValues)
        {
            Debug.Assert(Boxes.TryCalculateBoxSize(values.Length, out int expectedBoxSize)
                && expectedBoxSize == boxSize,
                $"Expected box size of {expectedBoxSize} for given size, but received {boxSize}.");
            Debug.Assert(allPossibleValues.ComputeCount() == values.Length,
                $"Expected {nameof(allPossibleValues)} to have the same length as {nameof(values)}.");
            int magicSum = _ComputeSum(values, boxSize);
            var sets = new HashSet<BitVector>();
            for (int i = 0; i < values.Length; ++i)
            {
                int possibleValue = values[i];
                var set = new BitVector();
                set.SetBit(possibleValue);
                BitVector possibleValues = allPossibleValues;
                possibleValues.UnsetBit(possibleValue);
                sets = new (sets.Union(_ComputeMagicSetsForRemainder(boxSize - 1, magicSum - possibleValue, possibleValues, set)));
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
            foreach (var possibleValue in possibleValues.GetSetBits())
            {
                if (remainder - possibleValue >= 0)
                {
                    BitVector set = partialSet;
                    set.SetBit(possibleValue);
                    BitVector reducedPossibleValues = possibleValues;
                    reducedPossibleValues.UnsetBit(possibleValue);
                    result = new (result.Union(_ComputeMagicSetsForRemainder(remainingSize - 1, remainder - possibleValue, reducedPossibleValues, set)));
                }
            }
            return result;
        }

    }
}
