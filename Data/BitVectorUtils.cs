using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;

namespace MorganRoff.Sudoku
{
    public static class BitVectorUtils
    {
        private static readonly int[] _masks = _CreateMasks();

        /// <summary>
        /// Creates a bit vector with only the first <c>size</c> bits set to true.
        /// </summary>
        /// <param name="size">The number of bits to set.</param>
        /// <param name="vector">The output vector.</param>
        public static int CreateWithSize(int size)
        {
            return (1 << size) - 1;
        }

        /// <summary>
        /// Unsets the given bit index in the given vector. Leaves other bits unchanged.
        /// </summary>
        /// <param name="bit">The zero-based index of the bit to unset.</param>
        /// <param name="vector">The vector in which to unset the bit.</param>
        public static void UnsetBit(int bit, ref int vector)
        {
            vector &= ~_masks[bit];
        }

        /// <summary>
        /// Sets the given bit index in the given vector. Leaves other bits unchanged.
        /// </summary>
        /// <param name="bit">The zero-based index of the bit to set.</param>
        /// <param name="vector">The vector in which to set the bit.</param>
        public static void SetBit(int bit, ref int vector)
        {
            vector |= _masks[bit];
        }

        /// <summary>
        /// Checks if the bit is true at the given index.
        /// </summary>
        /// <param name="vector">The vector to check.</param>
        /// <param name="bit">The zero-based index of the bit to check.</param>
        /// <returns></returns>
        public static bool IsBitSet(this int vector, int bit)
        {
            return Convert.ToBoolean(vector & _masks[bit]);
        }

        /// <summary>
        /// Counts the number of bits that are set in the given bit vector.
        /// </summary>
        /// <param name="vector">The vector to count the bits from.</param>
        /// <returns>The number of bits that are set.</returns>
        public static int CountSetBits(this int vector)
        {
            if (Popcnt.IsSupported)
            {
                return (int)Popcnt.PopCount((uint)vector);
            }
            int count = 0;
            for (int i = 0; i < 32; i++)
            {
                if (vector.IsBitSet(i))
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Gets an enumerable of the bits set in this bit vector.
        /// </summary>
        /// <param name="vector">The vector to get the bits from.</param>
        /// <param name="maxBitCount">The max number of bits that could be set. Bits are only
        ///     checked in the range <c>[0, maxBitCount)</c>.</param>
        /// <returns>An enumerable of the bits that are set.</returns>
        public static IEnumerable<int> GetSetBits(this int vector, int maxBitCount = 32)
        {
            for (int i = 0; i < maxBitCount; i++)
            {
                if ((vector & _masks[i]) != 0)
                {
                    yield return i;
                }
            }
        }

        private static int[] _CreateMasks()
        {
            var masks = new int[32];
            masks[0] = 1;
            for (int i = 1; i < 32; i++)
            {
                masks[i] = masks[i-1] << 1;
            }
            return masks;
        }
    }
}
