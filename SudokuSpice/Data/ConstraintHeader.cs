using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace SudokuSpice.Data
{
    public class ConstraintHeader
    {
        internal bool IsSatisfied = false;
        [DisallowNull]
        internal SquareLink? FirstLink;
        internal int Count = 0;
        
        internal ConstraintHeader() {}

        public static ConstraintHeader CreateConnectedHeader(IEnumerable<PossibleSquareValue> possibleSquares)
        {
            var header = new ConstraintHeader();
            foreach (var possibleSquare in possibleSquares)
            {
                var squareLink = new SquareLink(possibleSquare, header);
            }
            if (header.Count == 0)
            {
                throw new ArgumentException(
                    $"Must provide at least one {nameof(PossibleSquareValue)} when connecting a {nameof(ConstraintHeader)}.");
            }
            return header;
        }

        internal List<SquareLink> GetLinks()
        {
            Debug.Assert(FirstLink != null, $"{nameof(FirstLink)} was null when calling {nameof(GetLinks)}.");
            var links = new List<SquareLink>(Count);
            var link = FirstLink;
            do
            {
                links.Add(link);
                link = link.Down;
            } while (link != FirstLink);
            return links;
        }
    }
}
