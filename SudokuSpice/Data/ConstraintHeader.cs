using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SudokuSpice.Data
{
    public class ConstraintHeader
    {
        internal ExactCoverMatrix Matrix;
        internal ConstraintHeader NextHeader;
        internal ConstraintHeader PreviousHeader;
        [DisallowNull]
        internal SquareLink? FirstLink;
        internal int Count = 0;

        internal bool IsSatisfied { get; private set; }

        internal ConstraintHeader(ExactCoverMatrix matrix) {
            Matrix = matrix;
            if (matrix.FirstHeader is null)
            {
                matrix.FirstHeader = this;
                NextHeader = this;
                PreviousHeader = this;
            } else
            {
                NextHeader = matrix.FirstHeader;
                PreviousHeader = NextHeader.PreviousHeader;
                NextHeader.PreviousHeader = this;
                PreviousHeader.NextHeader = this;
            }
            Count = 0;
        }

        public static ConstraintHeader CreateConnectedHeader(ExactCoverMatrix matrix, IEnumerable<PossibleSquareValue> possibleSquares)
        {
            var header = new ConstraintHeader(matrix);
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

        internal bool TrySatisfyFrom(SquareLink sourceLink)
        {
            Debug.Assert(!IsSatisfied, $"Constraint was already satisfied when selecting square {sourceLink.PossibleSquare.Square.Coordinate}, value: {sourceLink.PossibleSquare.ValueIndex}.");
            Debug.Assert(FirstLink != null, $"Tried to satisfy constraint via square {sourceLink.PossibleSquare.Square.Coordinate}, value: {sourceLink.PossibleSquare.ValueIndex} but {nameof(FirstLink)} was null.");
            Debug.Assert(GetLinks().Contains(sourceLink), $"Constraint was missing possible square {sourceLink.PossibleSquare.Square.Coordinate}, value: {sourceLink.PossibleSquare.ValueIndex} when satisfying constraint.");
            IsSatisfied = true;
            var link = sourceLink.Down;
            while (link != sourceLink)
            {
                if (!link.PossibleSquare.TryDrop())
                {
                    link = link.Up;
                    while (link != sourceLink)
                    {
                        link.PossibleSquare.Return();
                        link = link.Up;
                    }
                    IsSatisfied = false;
                    return false;
                }
                link = link.Down;
            }
            NextHeader.PreviousHeader = PreviousHeader;
            PreviousHeader.NextHeader = NextHeader;
            if (Matrix.FirstHeader == this)
            {
                Matrix.FirstHeader = NextHeader;
            }
            return true;
        }

        internal void UnsatisfyFrom(SquareLink sourceLink)
        {
            Debug.Assert(IsSatisfied, $"Constraint was not satisfied when deselecting square {sourceLink.PossibleSquare.Square.Coordinate}, value: {sourceLink.PossibleSquare.ValueIndex}.");
            Debug.Assert(GetLinks().Contains(sourceLink), $"Constraint was missing possible square {sourceLink.PossibleSquare.Square.Coordinate}, value: {sourceLink.PossibleSquare.ValueIndex} when unsatisfying constraint.");
            var link = sourceLink.Up;
            while (link != sourceLink)
            {
                link.PossibleSquare.Return();
                link = link.Up;
            }
            IsSatisfied = false;
            NextHeader.PreviousHeader = this;
            PreviousHeader.NextHeader = this;
        }

        internal IEnumerable<SquareLink> GetLinks()
        {
            if (FirstLink == null)
            {
                yield break;
            }
            var link = FirstLink;
            do
            {
                yield return link;
                link = link.Down;
            } while (link != FirstLink);
        }
    }
}
