﻿using System;
using System.Numerics;
using System.Runtime.Intrinsics.X86;

namespace SudokuSpice
{
    /// <summary>
    /// A 32-bit vector with simple operations for getting and setting bits.
    /// </summary>
    public struct BitVector : IEquatable<BitVector>
    {
        private static readonly uint[] _masks = _CreateMasks();

        private static uint[] _CreateMasks()
        {
            uint[]? masks = new uint[NumBits];
            masks[0] = 1;
            for (int i = 1; i < NumBits; i++)
            {
                masks[i] = masks[i - 1] << 1;
            }
            return masks;
        }

        /// <summary>
        /// The number of bits (i.e. the capacity) contained by a BitVector.
        /// </summary>
        public const int NumBits = sizeof(uint) << 3;

        /// <summary>
        /// Gets the data stored in this bit vector as an unsigned int.
        /// </summary>
        public uint Data { readonly get; private set; }

        /// <summary>
        /// Checks if this vector is empty (i.e. no bits are set).
        /// </summary>
        /// <returns>True if empty.</returns>
        public readonly bool IsEmpty => Data == 0;

        /// <summary>
        /// Constructs a bit vector with the given data.
        /// </summary>
        /// <param name="data">The data to use for this bit vector.</param>
        public BitVector(uint data)
        {
            Data = data;
        }

        /// <summary>
        /// Creates a bit vector with only the first <c>size</c> bits set to true.
        /// </summary>
        /// <param name="size">The number of bits to set.</param>
        public static BitVector CreateWithSize(int size)
        {
            if (size < 0 || size > NumBits)
            {
                throw new ArgumentOutOfRangeException(nameof(size), size, "Must be between 0 and 32 inclusive.");
            }
            if (Bmi2.IsSupported)
            {
                return new BitVector(Bmi2.ZeroHighBits(uint.MaxValue, (uint)size));
            }
            return new BitVector((1u << size) - 1);
        }

        /// <summary>
        /// Creates a bit vector that is the intersect of the given vectors.
        /// </summary>
        /// <param name="a">One bit vector.</param>
        /// <param name="b">The other bit vector.</param>
        /// <returns> A new <c>BitVector</c> that is the intersect of the given vectors.</returns>
        public static BitVector FindIntersect(BitVector a, BitVector b) => new BitVector(a.Data & b.Data);

        /// <summary>
        /// Creates a bit vector that is the union of the given vectors.
        /// </summary>
        /// <param name="a">One bit vector.</param>
        /// <param name="b">The other bit vector.</param>
        /// <returns> A new <c>BitVector</c> that is the union of the given vectors.</returns>
        public static BitVector FindUnion(BitVector a, BitVector b) => new BitVector(a.Data | b.Data);

        /// <summary>
        /// Creates a bit vector that is the difference of the given vectors.
        /// </summary>
        /// <param name="a">One bit vector.</param>
        /// <param name="b">The other bit vector.</param>
        /// <returns> A new <c>BitVector</c> that is the union of the given vectors.</returns>
        public static BitVector FindDifference(BitVector a, BitVector b) => new BitVector(a.Data ^ b.Data);

        /// <summary>
        /// Unsets the given bit index in the given vector. Leaves other bits unchanged.
        /// </summary>
        /// <param name="bit">The zero-based index of the bit to unset.</param>
        public void UnsetBit(int bit) => Data &= ~_masks[bit];

        /// <summary>
        /// Sets the given bit index in the given vector. Leaves other bits unchanged.
        /// </summary>
        /// <param name="bit">The zero-based index of the bit to set.</param>
        public void SetBit(int bit) => Data |= _masks[bit];

        /// <summary>
        /// Gets the count of bits that are set.
        /// </summary>
        /// <remarks>
        /// This relies on the <see cref="Popcnt"/> hardware intrinsic to be efficient. If this
        /// operation is not available on your hardware, then this function falls back to a
        /// less efficient software-based approach.
        /// </remarks>
        public readonly int ComputeCount() => BitOperations.PopCount(Data);

        /// <summary>
        /// Checks if the bit is true at the given index.
        /// </summary>
        /// <param name="bit">The zero-based index of the bit to check.</param>
        /// <returns>True if set.</returns>
        public readonly bool IsBitSet(int bit) => Convert.ToBoolean(Data & _masks[bit]);

        /// <summary>
        /// Computes the last bit that is set, or -1 if none are set.
        /// </summary>
        public int ComputeLastSetBit() => NumBits - 1 - BitOperations.LeadingZeroCount(Data);

        /// <summary>
        /// Populates a provided Span with the indices of set bits, and returns the number of set 
        /// bits it populated.
        ///
        /// This method will return if it reaches the end of the provided
        /// <paramref name="setIndices"/> span, so if <paramref name="setIndices"/> has size N,
        /// then this will only populate the indices for the first N set bits, and will return N.
        /// If there are less setBits than N, say M, the values of <paramref name="setIndices"/>
        /// from setIndices[M] to setIndices[N-1] are left unchanged.
        /// </summary>
        /// <remarks>
        /// This operation is slightly more efficient on average when <see cref="Popcnt"/> is
        /// supported. Worst case performance is roughly the same.
        /// </remarks>
        /// <param name="setIndices">
        /// A span to fill with the indices of set bits. All values will be written to 
        /// </param>
        /// <returns>
        /// The number of set bits that have been populated into <paramref name="setIndices"/>
        /// </returns>
        public readonly int PopulateSetBits(Span<int> setIndices)
        {
            int numRecordedSetBits = 0;
            int maxSetBits = setIndices.Length;
            if (Popcnt.IsSupported)
            {
                int numSetBits = ComputeCount();
                maxSetBits = Math.Min(numSetBits, maxSetBits);
            }
            for (int i = 0;
                i < _masks.Length && numRecordedSetBits < maxSetBits;
                ++i)
            {
                if ((Data & _masks[i]) != 0)
                {
                    setIndices[numRecordedSetBits++] = i;
                }
            }
            return numRecordedSetBits;
        }

        /// <summary>
        /// Determines if this vector is a subset of the given vector (i.e. all bits set in this
        /// vector are also set in <paramref name="other"/>).
        /// </summary>
        /// <param name="other">The possible superset.</param>
        /// <returns>True if this is equal to or a subset of the given vector.</returns>
        public readonly bool IsSubsetOf(BitVector other)
        {
            var intersect = FindIntersect(this, other);
            return intersect == this;
        }

        public readonly bool Equals(BitVector other) => Data == other.Data;

        public override readonly bool Equals(object? obj)
        {
            if (obj is BitVector bv)
            {
                return Equals(bv);
            }
            return false;
        }

        public override readonly int GetHashCode() => (int)Data;

        /// <summary>
        /// Returns this bitvector as a binary-formatted string (eg. 1011).
        /// </summary>
        public override readonly string ToString() => Convert.ToString(Data, 2);

        public static bool operator ==(BitVector a, BitVector b) => a.Data == b.Data;
        public static bool operator !=(BitVector a, BitVector b) => a.Data != b.Data;
    }
}