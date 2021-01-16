using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace SudokuSpice.ConstraintBased
{
    /// <summary>
    /// Represents a row in the <see cref="ExactCoverMatrix"/>. This represents a possible value
    /// for a given <see cref="Square"/>.
    /// </summary>
    public class Possibility : IPossibility<Possibility, Requirement>
    {
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
        internal Link<Possibility, Requirement>? FirstLink { get; private set; }

        internal Possibility(Square square, int valueIndex)
        {
            ValueIndex = valueIndex;
            Square = square;
            State = PossibilityState.UNKNOWN;
        }

        void IPossibility<Possibility, Requirement>.Append(Link<Possibility, Requirement> link) 
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
                $"{nameof(Possibility)} at {Square.Coordinate} with value {ValueIndex} was selected while in state {State}.");
            Debug.Assert(
                FirstLink != null,
                $"{nameof(Possibility)} at {Square.Coordinate} with value {ValueIndex} was selected while {nameof(FirstLink)} was null.");
            if (!Links.TryUpdateOnPossibility(
                FirstLink,
                link => link.Objective.TrySelect(link),
                link => link.Objective.Deselect(link)))
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
                $"{nameof(Possibility)} at {Square.Coordinate} with value {ValueIndex} was deselected while in state {State}.");
            Debug.Assert(
                FirstLink != null,
                $"{nameof(Possibility)} at {Square.Coordinate} with value {ValueIndex} was deselected while {nameof(FirstLink)} was null.");
            State = PossibilityState.UNKNOWN;
            Links.RevertOnPossibility(FirstLink, link => link.Objective.Deselect(link));
        }

        internal bool TryDrop()
        {
            Debug.Assert(
                State == PossibilityState.UNKNOWN,
                $"{nameof(Possibility)} at {Square.Coordinate} with value {ValueIndex} was cleared while in state {State}.");
            if (Square.NumPossibleValues == 1)
            {
                return false;
            }
            if (FirstLink != null)
            {
                if (!Links.TryUpdateOnPossibility(
                    FirstLink,
                    link => link.Objective.TryDetach(link),
                    link => link.Objective.Reattach(link)))
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
                $"{nameof(Possibility)} at {Square.Coordinate} with value {ValueIndex} was returned while in state {State}.");
            Debug.Assert(
                FirstLink != null,
                $"{nameof(Possibility)} at {Square.Coordinate} with value {ValueIndex} was returned while {nameof(FirstLink)} was null.");
            State = PossibilityState.UNKNOWN;
            Square.NumPossibleValues++;
            Links.RevertOnPossibility(FirstLink, link => link.Objective.Reattach(link));
        }

        internal int GetMinUnselectedCountFromRequirements()
        {
            Debug.Assert(
                FirstLink != null,
                $"Called {nameof(GetMinUnselectedCountFromRequirements)} on {nameof(Possibility)} at {Square.Coordinate} with value {ValueIndex} while {nameof(FirstLink)} was null.");
            int minCount = FirstLink.Objective.CountUnselected;
            Link<Possibility, Requirement> link = FirstLink.NextOnPossibility;
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