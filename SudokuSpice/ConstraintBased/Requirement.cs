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
    /// <see cref="CreateFullyConnected(ExactCoverMatrix, ReadOnlySpan{PossibleSquareValue}, int, bool)"/>
    /// for more details.
    /// </summary>
    /// <seealso href="https://en.wikipedia.org/wiki/Exact_cover">Exact cover (Wikipedia)</seealso>
    public class Requirement :
        IObjective<Requirement, PossibleSquareValue>,
        IPossibility<Requirement, RequirementGroup>
    {
        private readonly bool _isOptional;
        private readonly int _requiredCount;

        private int _selectedCount;
        private int _count;

        internal ExactCoverMatrix Matrix { get; private set; }
        [DisallowNull]
        internal Link<PossibleSquareValue, Requirement>? FirstPossibilityLink { get; private set; }
        internal Link<Requirement, RequirementGroup>? FirstGroupLink { get; private set; }
        internal int CountUnselected => _count - _selectedCount;
        internal bool AreAllPossibilitiesRequired => _count == _requiredCount;
        internal bool AreAllRequiredPossibilitiesSelected => _selectedCount == _requiredCount;
        internal bool IsOptional => _isOptional;
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
            Debug.Assert(FirstPossibilityLink != null, $"Can't copy a requirement with a null {nameof(FirstPossibilityLink)}.");
            Debug.Assert(!AreAllRequiredPossibilitiesSelected, $"Can't copy a requirement that's already satisfied.");
            var copy = new Requirement(_isOptional, _requiredCount, matrix);
            foreach (Link<PossibleSquareValue, Requirement>? link in GetPossibleValueLinks())
            {
                Square? square = matrix.GetSquare(link.Possibility.Square.Coordinate);
                Debug.Assert(square != null,
                    $"Tried to copy a square link for a null square at {link.Possibility.Square.Coordinate}.");
                PossibleSquareValue? possibleValue = square.GetPossibleValue(link.Possibility.ValueIndex);
                Debug.Assert(possibleValue != null, "Tried to link requirement to null possible square value.");
                _ = Link<PossibleSquareValue, Requirement>.CreateConnectedLink(possibleValue, copy);
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
            ExactCoverMatrix matrix, ReadOnlySpan<PossibleSquareValue> possibleSquares, int requiredCount = 1, bool isOptional = false)
        {
            if (possibleSquares.Length < requiredCount)
            {
                throw new ArgumentException(
                    $"Must provide at least {requiredCount} {nameof(PossibleSquareValue)}s when creating a {nameof(Requirement)}.");
            }
            var requirement = new Requirement(isOptional, requiredCount, matrix);
            matrix.Attach(requirement);
            foreach (PossibleSquareValue? possibleSquare in possibleSquares)
            {
                _ = Link<PossibleSquareValue, Requirement>.CreateConnectedLink(possibleSquare, requirement);
            }
            return requirement;
        }

        void IPossibility<Requirement, RequirementGroup>.Append(Link<Requirement, RequirementGroup> link)
        {
            if (FirstGroupLink is null)
            {
                FirstGroupLink = link;
                return;
            }
            FirstGroupLink.PrependToPossibility(link);
        }

        void IObjective<Requirement, PossibleSquareValue>.Append(Link<PossibleSquareValue, Requirement> link)
        {
            Debug.Assert(!GetPossibleValueLinks().Contains(link), $"Constraint already contained possible square value at {link.Possibility.Square.Coordinate}, value: {link.Possibility.ValueIndex} when calling {nameof(Append)}.");
            if (FirstPossibilityLink is null)
            {
                FirstPossibilityLink = link;
            } else
            {
                FirstPossibilityLink.PrependToObjective(link);
            }
            ++_count;
        }

        internal bool TrySelectPossibility(Link<PossibleSquareValue, Requirement> possibleValueToRequirement)
        {
            Debug.Assert(!AreAllRequiredPossibilitiesSelected, $"Constraint was already satisfied when selecting square {possibleValueToRequirement.Possibility.Square.Coordinate}, value: {possibleValueToRequirement.Possibility.ValueIndex}.");
            Debug.Assert(FirstPossibilityLink != null, $"Tried to satisfy constraint via square {possibleValueToRequirement.Possibility.Square.Coordinate}, value: {possibleValueToRequirement.Possibility.ValueIndex} but {nameof(FirstPossibilityLink)} was null.");
            Debug.Assert(GetPossibleValueLinks().Contains(possibleValueToRequirement), $"Constraint was missing possible square {possibleValueToRequirement.Possibility.Square.Coordinate}, value: {possibleValueToRequirement.Possibility.ValueIndex} when satisfying constraint.");
            ++_selectedCount;
            if (!AreAllRequiredPossibilitiesSelected)
            {
                // Drop it from the linked list so we don't consider it a possible value anymore.
                // Note that the count does not change, because this link is still part of the
                // constraint; it's just hidden from the linked list of uncertain possible square
                // values.
                if (FirstPossibilityLink == possibleValueToRequirement)
                {
                    FirstPossibilityLink = possibleValueToRequirement.NextOnObjective;
                }
                possibleValueToRequirement.PopFromObjective();
                return true;
            }
            // TODO: Only need to detach other links from objectives, excluding the sourcelink.
            if (!Links.TryUpdateOthersOnObjective(
                possibleValueToRequirement,
                toDrop => toDrop.Possibility.TryDrop(),
                toReturn => toReturn.Possibility.Return()))
            {
                --_selectedCount;
                return false;
            }
            // TODO: Probably do this check first
            if (FirstGroupLink is not null)
            {
                if (!Links.TryUpdateOnPossibility(
                    FirstGroupLink,
                    link => link.Objective.TrySelectPossibility(link),
                    link =>
                    {
                        link.Objective.DeselectPossibility(link);
                    }))
                {
                    Links.RevertOthersOnObjective(
                        possibleValueToRequirement,
                        toReturn => toReturn.Possibility.Return());
                    return false;
                }
            }
            _PopFromMatrix();
            return true;
        }

        internal void DeselectPossibility(Link<PossibleSquareValue, Requirement> sourceLink)
        {
            if (!AreAllRequiredPossibilitiesSelected)
            {
                // In this case, this link was removed but no other links were modified. Simply
                // reinsert the link.
                sourceLink.ReinsertToObjective();
                --_selectedCount;
                return;
            }
            // In this case, other links were dropped but this link was not removed. Return dropped
            // links and matrix connections only.
            Links.RevertOthersOnObjective(
                sourceLink,
                toReturn => toReturn.Possibility.Return());
            if (FirstGroupLink is not null)
            {
                Links.RevertOnPossibility(
                    FirstGroupLink, link => link.Objective.DeselectPossibility(link));
            }
            --_selectedCount;
            _ReinsertToMatrix();
        }

        internal bool TryDropPossibility(Link<PossibleSquareValue, Requirement> link)
        {
            Debug.Assert(
                GetPossibleValueLinks().Contains(link),
                $"Can't remove missing possible square {link.Possibility.Square.Coordinate}, value: {link.Possibility.ValueIndex} from requirement.");
            Debug.Assert(
                link.Possibility.State != PossibilityState.SELECTED,
                "Can't detach an already selected possibility.");
            // If the requirement is already satisfied then we can skip this.
            if (AreAllRequiredPossibilitiesSelected)
            {
                return true;
            }
            if (AreAllPossibilitiesRequired)
            {
                if (!_isOptional)
                {
                    return false;
                }
                if (FirstGroupLink is not null)
                {
                    if (!Links.TryUpdateOnPossibility(
                        FirstGroupLink,
                        link => link.Objective.TryDropPossibility(link),
                        link => link.Objective.ReturnPossibility(link)))
                    {
                        return false;
                    }
                }
                _PopFromMatrix();
            }
            if (FirstPossibilityLink == link)
            {
                FirstPossibilityLink = link.NextOnObjective;
                if (FirstPossibilityLink == link)
                {
                    FirstPossibilityLink = null;
                }
            }
            link.PopFromObjective();
            --_count;
            return true;
        }

        internal void ReattachPossibility(Link<PossibleSquareValue, Requirement> link)
        {
            // If the constraint is satisfied then we can skip this since this link was never removed.
            if (AreAllRequiredPossibilitiesSelected)
            {
                return;
            }
            Debug.Assert(!GetPossibleValueLinks().Contains(link), $"{nameof(Requirement)} already contained possible square value at {link.Possibility.Square.Coordinate}, value: {link.Possibility.ValueIndex} when calling {nameof(ReattachPossibility)}.");
            Debug.Assert(
                link.Possibility.State != PossibilityState.SELECTED,
                "Shouldn't need to reattach a selected possibility.");
            link.ReinsertToObjective();
            if (FirstPossibilityLink is null)
            {
                FirstPossibilityLink = link;
            }
            ++_count;
            if (AreAllPossibilitiesRequired)
            {
                _ReinsertToMatrix();
                if (FirstGroupLink is not null)
                {
                    Links.RevertOnPossibility(
                        FirstGroupLink,
                        toReturn =>toReturn.Objective.ReturnPossibility(toReturn));
                }
            }
        }

        internal bool TryDrop(Link<Requirement, RequirementGroup> thisToParent)
        {
            Debug.Assert(
                FirstPossibilityLink is not null,
                $"{nameof(TryDrop)} called with null {nameof(FirstPossibilityLink)}.");
            Debug.Assert(
                FirstGroupLink is not null,
                $"{nameof(TryDrop)} called with null {nameof(FirstGroupLink)}.");
            Debug.Assert(
                FirstGroupLink.GetLinksOnPossibility().Contains(thisToParent),
                "Cannot drop a requirement from a disconnected link.");
            if (!_isOptional)
            {
                return false;
            }
            // Drop this possibility from the other groups
            if (!Links.TryUpdateOthersOnPossibility(
                thisToParent,
                toDrop => toDrop.Objective.TryDropPossibility(toDrop),
                toReturn => toReturn.Objective.ReturnPossibility(toReturn)))
            {
                return false;
            }
            Links.TryUpdateOnObjective(
                FirstPossibilityLink,
                toDetachFrom => toDetachFrom.Possibility.TryDetachRequirement(toDetachFrom),
                toReattachTo => toReattachTo.Possibility.ReattachRequirement(toReattachTo));
            _PopFromMatrix();
            return true;
        }

        internal void Return(Link<Requirement, RequirementGroup> sourceLink)
        {
            Debug.Assert(
                FirstPossibilityLink is not null,
                $"{nameof(Return)} called with null {nameof(FirstPossibilityLink)}.");
            _ReinsertToMatrix();
            Links.RevertOnObjective(
                FirstPossibilityLink,
                toReattachTo => toReattachTo.Possibility.ReattachRequirement(toReattachTo));
            Links.RevertOthersOnPossibility(
                sourceLink,
                toReturnTo => toReturnTo.Objective.ReturnPossibility(toReturnTo));
        }

        internal void DetachGroup(Link<Requirement, RequirementGroup> link)
        {
            Debug.Assert(
                FirstGroupLink is not null,
                $"Cannot call {nameof(DetachGroup)} for unattached link.");
            if (FirstGroupLink == link)
            {
                FirstGroupLink = FirstGroupLink.NextOnPossibility;
                if (FirstGroupLink == link)
                {
                    FirstGroupLink = null;
                }
            }
            link.PopFromPossibility();
        }

        internal void ReattachGroup(Link<Requirement, RequirementGroup> link)
        {
            link.ReinsertToPossibility();
            if (FirstGroupLink is null)
            {
                FirstGroupLink = link;
            }
        }

        internal IEnumerable<Link<PossibleSquareValue, Requirement>> GetPossibleValueLinks()
        {
            if (FirstPossibilityLink == null)
            {
                return Enumerable.Empty<Link<PossibleSquareValue, Requirement>>();
            }
            return FirstPossibilityLink.GetLinksOnObjective();
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

        private void _PopFromMatrix()
        {
            Next.Previous = Previous;
            Previous.Next = Next;
            if (Matrix.FirstRequirement == this)
            {
                Matrix.FirstRequirement = Next;
                if (Matrix.FirstRequirement == this)
                {
                    Matrix.FirstRequirement = null;
                }
            }
        }

        private void _ReinsertToMatrix()
        {
            Next.Previous = this;
            Previous.Next = this;
            if (Matrix.FirstRequirement is null)
            {
                Matrix.FirstRequirement = this;
            }
        }
    }
}