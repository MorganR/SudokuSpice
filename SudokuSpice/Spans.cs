using System;
using System.Runtime.CompilerServices;

namespace SudokuSpice
{
    /// <summary>
    /// Utilities for working with <see cref="System.Span{T}"/> objects.
    /// </summary>
    public static class Spans
    {
        /// <summary>
        /// Pops a random value from the span between 0 (inclusive) and end (exclusive).
        /// 
        /// Modifies the Span to swap the returned value to the end of the given range.
        /// </summary>
        /// <typeparam name="T">The type of data in the Span.</typeparam>
        /// <param name="end">The end of the range to pop from, exclusive.</param>
        /// <param name="rand">The random instance to use.</param>
        /// <param name="data">The span to pop from.</param>
        /// <returns></returns>
        public static ref T PopRandom<T>(Random rand, Span<T> data)
        {
            int indexToPop = rand.Next(0, data.Length);
            _SwapToEnd(indexToPop, data);
            return ref data[data.Length - 1];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void _SwapToEnd<T>(int i, Span<T> data)
        {
            T tmp = data[i];
            data[i] = data[data.Length - 1];
            data[data.Length - 1] = tmp;
        }
    }
}
