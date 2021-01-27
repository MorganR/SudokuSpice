using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SudokuSpice.ConstraintBased
{
    /// <summary>
    /// Represents a row in the <see cref="ExactCoverMatrix"/>. This represents a possible value
    /// for a given <see cref="Square"/>.
    /// </summary>
    public class PossibleSquareValue : IPossibility<PossibleSquareValue, Requirement>
    {
        private Link<PossibleSquareValue, Requirement>? _detachedParentLinkThatCausedDrop;
        
        /// <summary>
        /// Gets the index of the possible value that this represents. This index corresponds with
        /// <see cref="ExactCoverMatrix.AllPossibleValues"/>.
        /// </summary>
        public int ValueIndex { get; }
        /// <summary>
        /// Gets the square that this is a possible value for.
        /// </summary>
        public Square Square { get; }
        /// <summary>
        /// Gets the state of this row.
        /// </summary>
        public PossibilityState State { get; private set; }
        [DisallowNull]
        internal Link<PossibleSquareValue, Requirement>? FirstLink { get; private set; }

        internal PossibleSquareValue(Square square, int valueIndex)
        {
            ValueIndex = valueIndex;
            Square = square;
            State = PossibilityState.UNKNOWN;
        }

        void IPossibility<PossibleSquareValue, Requirement>.Append(Link<PossibleSquareValue, Requirement> link) 
        {
            if (FirstLink is null)
            {
                FirstLink = link;
            } else
            {
                FirstLink.PrependToPossibility(link);
            }
        }

        internal bool TrySelect()
        {
            Debug.Assert(
                State == PossibilityState.UNKNOWN,
                $"{nameof(PossibleSquareValue)} at {Square.Coordinate} with value {ValueIndex} was selected while in state {State}.");
            Debug.Assert(
                FirstLink != null,
                $"{nameof(PossibleSquareValue)} at {Square.Coordinate} with value {ValueIndex} was selected while {nameof(FirstLink)} was null.");
            if (!Links.TryUpdateOnPossibility(
                FirstLink,
                link => link.Objective.TrySelectPossibility(link),
                link => link.Objective.DeselectPossibility(link)))
            {
                return false;
            }
            State = PossibilityState.SELECTED;
            return true;
        }

        internal void Deselect()
        {
            Debug.Assert(
                State == PossibilityState.SELECTED,
                $"{nameof(PossibleSquareValue)} at {Square.Coordinate} with value {ValueIndex} was deselected while in state {State}.");
            Debug.Assert(
                FirstLink != null,
                $"{nameof(PossibleSquareValue)} at {Square.Coordinate} with value {ValueIndex} was deselected while {nameof(FirstLink)} was null.");
            State = PossibilityState.UNKNOWN;
            Links.RevertOnPossibility(FirstLink, link => link.Objective.DeselectPossibility(link));
        }

        internal bool TryDrop()
        {
            Debug.Assert(
                State == PossibilityState.UNKNOWN,
                $"{nameof(PossibleSquareValue)} at {Square.Coordinate} with value {ValueIndex} was cleared while in state {State}.");
            if (Square.NumPossibleValues == 1)
            {
                return false;
            }
            if (FirstLink != null)
            {
                if (!Links.TryUpdateOnPossibility(
                    FirstLink,
                    link => link.Objective.TryDropPossibility(link),
                    link => link.Objective.ReattachPossibility(link)))
                {
                    return false;
                }
            }
            Square.NumPossibleValues--;
            State = PossibilityState.DROPPED;
            return true;
        }

        internal void Return()
        {
            Debug.Assert(
                State == PossibilityState.DROPPED,
                $"{nameof(PossibleSquareValue)} at {Square.Coordinate} with value {ValueIndex} was returned while in state {State}.");
            State = PossibilityState.UNKNOWN;
            Square.NumPossibleValues++;
            if (FirstLink is not null)
            {
                Links.RevertOnPossibility(FirstLink, link => link.Objective.ReattachPossibility(link));
            }
        }

        internal bool TryDetachRequirement(Link<PossibleSquareValue, Requirement> thisToRequirement)
        {
            if (FirstLink == thisToRequirement)
            {
                FirstLink = thisToRequirement.NextOnPossibility;
                if (FirstLink == thisToRequirement)
                {
                    FirstLink = null;
                    _detachedParentLinkThatCausedDrop = thisToRequirement;
                    if (!TryDrop())
                    {
                        _detachedParentLinkThatCausedDrop = null;
                        FirstLink = thisToRequirement;
                        return false;
                    }
                } else if (thisToRequirement.Objective.NonOptionalParents.Any(
                    nonOptional => !FirstLink.GetLinksOnPossibility().Any(
                        toRequirement => toRequirement.Objective.NonOptionalParents.Contains(nonOptional))))
                {
                    _detachedParentLinkThatCausedDrop = thisToRequirement;
                    if (!TryDrop())
                    {
                        _detachedParentLinkThatCausedDrop = null;
                        return false;
                    }
                }
            }
            thisToRequirement.PopFromPossibility();
            return true;
        }

        internal bool ReattachRequirement(Link<PossibleSquareValue, Requirement> thisToRequirement)
        {
            thisToRequirement.ReinsertToPossibility();
            if (FirstLink is null)
            {
                Return();
                _detachedParentLinkThatCausedDrop = null;
                FirstLink = thisToRequirement;
            } else if (_detachedParentLinkThatCausedDrop == thisToRequirement)
            {
                Return();
                _detachedParentLinkThatCausedDrop = null;
            }
            return true;
        }

        internal int GetMinUnselectedCountFromRequirements()
        {
            Debug.Assert(
                FirstLink != null,
                $"Called {nameof(GetMinUnselectedCountFromRequirements)} on {nameof(PossibleSquareValue)} at {Square.Coordinate} with value {ValueIndex} while {nameof(FirstLink)} was null.");
            int minCount = FirstLink.Objective.CountUnselected;
            Link<PossibleSquareValue, Requirement> link = FirstLink.NextOnPossibility;
            while (link != FirstLink)
            {
                int count = link.Objective.CountUnselected;
                if (count < minCount)
                {
                    minCount = count;
                }
                link = link.NextOnPossibility;
            }
            return minCount;
        }
    }
}