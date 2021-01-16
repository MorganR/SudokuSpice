using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SudokuSpice.ConstraintBased
{
    /// <summary>
    /// Represents a column from an exact-cover matrix.
    /// 
    /// Columns can require multiple rows to be satisfied, and can be optional. See
    /// <see cref="CreateFullyConnected(ExactCoverMatrix, ReadOnlySpan{Possibility}, int, bool)"/>
    /// for more details.
    /// </summary>
    /// <seealso href="https://en.wikipedia.org/wiki/Exact_cover">Exact cover (Wikipedia)</seealso>
    public class Requirement
    {
        private readonly bool _isOptional;
        private readonly int _requiredCount;

        private int _selectedCount;
        private int _count;

        internal ExactCoverMatrix Matrix { get; private set; }
        [DisallowNull]
        internal Link? FirstLink { get; private set; }
        internal int CountUnselected => _count - _selectedCount;
        internal bool AreAllLinksRequired => _count == _requiredCount;
        internal bool AreAllLinksSelected => _selectedCount == _requiredCount;
        internal Requirement Next { get; set; }
        internal Requirement Previous { get; set; }

        // Visible for testing
        internal Requirement(bool isOptional, int requiredCount, ExactCoverMatrix matrix)
        {
            _isOptional = isOptional;
            _requiredCount = requiredCount;
            Matrix = matrix;
            Next = Previous = this;
        }

        internal Requirement CopyToMatrix(ExactCoverMatrix matrix)
        {
            Debug.Assert(FirstLink != null, $"Can't copy a requirement with a null {nameof(FirstLink)}.");
            Debug.Assert(!AreAllLinksSelected, $"Can't copy a requirement that's already satisfied.");
            var copy = new Requirement(_isOptional, _requiredCount, matrix);
            foreach (Link? link in GetLinks())
            {
                Square? square = matrix.GetSquare(link.PossibleSquareValue.Square.Coordinate);
                Debug.Assert(square != null,
                    $"Tried to copy a square link for a null square at {link.PossibleSquareValue.Square.Coordinate}.");
                Possibility? possibleValue = square.GetPossibleValue(link.PossibleSquareValue.ValueIndex);
                Debug.Assert(possibleValue != null, "Tried to link requirement to null possible square value.");
                _ = Link.CreateConnectedLink(possibleValue, copy);
            }
            return copy;
        }

        /// <summary>
        /// Creates a fully connected requirement that can be satisfied by exactly
        /// <paramref name="requiredCount"/> <paramref name="possibleSquares"/>. Adds and attaches
        /// necessary links to connect the matrix.
        /// </summary>
        /// <param name="matrix">That matrix that this requirement should be attached to.</param>
        /// <param name="possibleSquares">
        /// The possible square values that would satisfy this requirement.
        /// </param>
        /// <param name="requiredCount">
        /// The number of <paramref name="possibleSquares"/> required to satisfy this constraint.
        /// </param>
        /// <param name="isOptional">
        /// Whether or not this constraint is optional. If a constraint is optional, then it behaves
        /// as follows:
        ///
        ///  * If the required number of links are selected: any remaining possible square values
        ///    are dropped.
        ///  * If the constraint becomes impossible, no changes are made to the related possible
        ///    square values; they will gradually be selected or dropped as necessary.
        /// </param>
        /// <returns>The newly constructed requirement.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown if called with fewer <paramref name="possibleSquares"/> than the
        /// <paramref name="requiredCount"/> (even if the requirement is optional).
        /// </exception>
        public static Requirement CreateFullyConnected(
            ExactCoverMatrix matrix, ReadOnlySpan<Possibility> possibleSquares, int requiredCount = 1, bool isOptional = false)
        {
            if (possibleSquares.Length < requiredCount)
            {
                throw new ArgumentException(
                    $"Must provide at least {requiredCount} {nameof(Possibility)}s when creating a {nameof(Requirement)}.");
            }
            var requirement = new Requirement(isOptional, requiredCount, matrix);
            matrix.Attach(requirement);
            foreach (Possibility? possibleSquare in possibleSquares)
            {
                _ = Link.CreateConnectedLink(possibleSquare, requirement);
            }
            return requirement;
        }

        internal bool TrySelect(Link sourceLink)
        {
            Debug.Assert(!AreAllLinksSelected, $"Constraint was already satisfied when selecting square {sourceLink.PossibleSquareValue.Square.Coordinate}, value: {sourceLink.PossibleSquareValue.ValueIndex}.");
            Debug.Assert(FirstLink != null, $"Tried to satisfy constraint via square {sourceLink.PossibleSquareValue.Square.Coordinate}, value: {sourceLink.PossibleSquareValue.ValueIndex} but {nameof(FirstLink)} was null.");
            Debug.Assert(GetLinks().Contains(sourceLink), $"Constraint was missing possible square {sourceLink.PossibleSquareValue.Square.Coordinate}, value: {sourceLink.PossibleSquareValue.ValueIndex} when satisfying constraint.");
            ++_selectedCount;
            if (!AreAllLinksSelected)
            {
                // Drop it from the linked list so we don't consider it a possible value anymore.
                // Note that the count does not change, because this link is still part of the
                // constraint; it's just hidden from the linked list of uncertain possible square
                // values.
                if (FirstLink == sourceLink)
                {
                    FirstLink = sourceLink.Down;
                }
                sourceLink.PopVertically();
                return true;
            }
            Link link = sourceLink.Down;
            while (link != sourceLink)
            {
                if (!link.PossibleSquareValue.TryDrop())
                {
                    link = link.Up;
                    while (link != sourceLink)
                    {
                        link.PossibleSquareValue.Return();
                        link = link.Up;
                    }
                    --_selectedCount;
                    return false;
                }
                link = link.Down;
            }
            Next.Previous = Previous;
            Previous.Next = Next;
            if (Matrix.FirstRequirement == this)
            {
                Matrix.FirstRequirement = Next;
            }
            return true;
        }

        internal void Deselect(Link sourceLink)
        {
            if (!AreAllLinksSelected)
            {
                // In this case, this link was removed but no other links were modified. Simply
                // reinsert the link.
                sourceLink.ReinsertVertically();
                --_selectedCount;
                return;
            }
            // In this case, other links were dropped but this link was not removed. Return dropped
            // links and matrix connections only.
            Link link = sourceLink.Up;
            while (link != sourceLink)
            {
                link.PossibleSquareValue.Return();
                link = link.Up;
            }
            --_selectedCount;
            Next.Previous = this;
            Previous.Next = this;
        }

        internal void Append(Requirement toAppend)
        {
            toAppend.Next = Next;
            toAppend.Previous = this;
            Next.Previous = toAppend;
            Next = toAppend;
        }

        internal void Prepend(Requirement toPrepend)
        {
            toPrepend.Next = this;
            toPrepend.Previous = Previous;
            Previous.Next = toPrepend;
            Previous = toPrepend;
        }

        internal bool TryDetach(Link link)
        {
            Debug.Assert(
                GetLinks().Contains(link),
                $"Can't remove missing possible square {link.PossibleSquareValue.Square.Coordinate}, value: {link.PossibleSquareValue.ValueIndex} from constraint.");
            if (AreAllLinksRequired && !_isOptional)
            {
                return false;
            }
            if (FirstLink == link)
            {
                FirstLink = link.Down;
            }
            link.PopVertically();
            --_count;
            return true;
        }

        internal void Attach(Link link)
        {
            Debug.Assert(!GetLinks().Contains(link), $"Constraint already contained possible square value at {link.PossibleSquareValue.Square.Coordinate}, value: {link.PossibleSquareValue.ValueIndex} when calling {nameof(Attach)}.");
            if (FirstLink is null)
            {
                FirstLink = link;
            } else
            {
                FirstLink.PrependUp(link);
            }
            ++_count;
        }

        internal void Reattach(Link link)
        {
            Debug.Assert(!GetLinks().Contains(link), $"Constraint already contained possible square value at {link.PossibleSquareValue.Square.Coordinate}, value: {link.PossibleSquareValue.ValueIndex} when calling {nameof(Reattach)}.");
            link.ReinsertVertically();
            ++_count;
        }

        internal IEnumerable<Link> GetLinks()
        {
            if (FirstLink == null)
            {
                yield break;
            }
            Link? link = FirstLink;
            do
            {
                yield return link;
                link = link.Down;
            } while (link != FirstLink);
        }
    }
}