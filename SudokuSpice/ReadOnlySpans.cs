using System;

namespace SudokuSpice
{
    internal static class ReadOnlySpans
    {
        internal static T FindMax<T>(this ReadOnlySpan<T> span) where T : IComparable<T>
        {
            T max = span[0];
            for (int i = 1; i < span.Length; ++i)
            {
                if (span[i].CompareTo(max) > 0)
                {
                    max = span[i];
                }
            }
            return max;
        }
    }
}