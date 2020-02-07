using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;

namespace SudokuSpice
{
    public struct BitVector : IEquatable<BitVector>
    {
        private static readonly int[] _masks = _CreateMasks();

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

        private int _data;

        /// <summary>
        /// Gets the data stored in this bit vector, as an int.
        /// </summary>
        public int Data { get => _data; }

        /// <summary>
        /// Constructs a bit vector with the given data.
        /// </summary>
        /// <param name="data">The data to use for this bit vector.</param>
        public BitVector(int data)
        {
            _data = data;
        }

        /// <summary>
        /// Creates a bit vector with only the first <c>size</c> bits set to true.
        /// </summary>
        /// <param name="size">The number of bits to set.</param>
        public static BitVector CreateWithSize(int size)
        {
            return new BitVector((1 << size) - 1);
        }

        /// <summary>
        /// Creates a bit vector that is the intersect of the given vectors.
        /// </summary>
        /// <param name="a">One bit vector.</param>
        /// <param name="b">The other bit vector.</param>
        /// <returns> A new <c>BitVector</c> that is the intersect of the given vectors.</returns>
        public static BitVector FindIntersect(BitVector a, BitVector b)
        {
            return new BitVector(a._data & b._data);
        }

        /// <summary>
        /// Creates a bit vector that is the union of the given vectors.
        /// </summary>
        /// <param name="a">One bit vector.</param>
        /// <param name="b">The other bit vector.</param>
        /// <returns> A new <c>BitVector</c> that is the union of the given vectors.</returns>
        public static BitVector FindUnion(BitVector a, BitVector b)
        {
            return new BitVector(a._data | b._data);
        }

        /// <summary>
        /// Unsets the given bit index in the given vector. Leaves other bits unchanged.
        /// </summary>
        /// <param name="bit">The zero-based index of the bit to unset.</param>
        public void UnsetBit(int bit)
        {
            _data &= ~_masks[bit];
        }

        /// <summary>
        /// Sets the given bit index in the given vector. Leaves other bits unchanged.
        /// </summary>
        /// <param name="bit">The zero-based index of the bit to set.</param>
        public void SetBit(int bit)
        {
            _data |= _masks[bit];
        }

        /// <summary>
        /// Checks if this vector is empty (i.e. no bits are set).
        /// </summary>
        /// <returns>True if empty.</returns>
        public readonly bool IsEmpty()
        {
            return _data == 0;
        }

        /// <summary>
        /// Checks if the bit is true at the given index.
        /// </summary>
        /// <param name="bit">The zero-based index of the bit to check.</param>
        /// <returns>True if set.</returns>
        public readonly bool IsBitSet(int bit)
        {
            return Convert.ToBoolean(_data & _masks[bit]);
        }

        /// <summary>
        /// Counts the number of bits that are set.
        /// </summary>
        /// <returns>The number of bits that are set.</returns>
        public readonly int CountSetBits()
        {
            if (Popcnt.IsSupported)
            {
                return (int)Popcnt.PopCount((uint)_data);
            }
            int count = 0;
            for (int i = 0; i < 32; i++)
            {
                if (IsBitSet(i))
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Gets an enumerable of the bits set in this bit vector.
        /// </summary>
        /// <param name="maxBitCount">The max number of bits that could be set. Bits are only
        ///     checked in the range <c>[0, maxBitCount)</c>. Defaults to 32.</param>
        /// <returns>An enumerable of the bits that are set.</returns>
        public readonly IEnumerable<int> GetSetBits(int maxBitCount = 32)
        {
            for (int i = 0; i < maxBitCount; i++)
            {
                if ((_data & _masks[i]) != 0)
                {
                    yield return i;
                }
            }
        }

        public bool Equals(BitVector other) => _data == other._data;

        public override bool Equals(Object obj)
        {
            if (obj is BitVector bv)
            {
                return Equals(bv);
            }
            return false;
        }

        public override int GetHashCode() => _data;

        public override string ToString() => Convert.ToString(_data, 2);

        public static bool operator ==(BitVector a, BitVector b) => a._data == b._data;
        public static bool operator !=(BitVector a, BitVector b) => a._data != b._data;
    }
}
