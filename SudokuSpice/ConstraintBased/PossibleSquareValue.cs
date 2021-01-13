using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace SudokuSpice.ConstraintBased
{
    /// <summary>
    /// Represents a row in the <see cref="ExactCoverMatrix"/>. This represents a possible value
    /// for a given <see cref="Square"/>.
    /// </summary>
    public class PossibleSquareValue
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
        public PossibleValueState State { get; private set; }
        [DisallowNull]
        internal Link? FirstLink { get; private set; }

        internal PossibleSquareValue(Square square, int valueIndex)
        {
            ValueIndex = valueIndex;
            Square = square;
            State = PossibleValueState.UNKNOWN;
        }

        internal void Attach(Link link)
        {
            if (FirstLink is null)
            {
                FirstLink = link;
            } else
            {
                FirstLink.PrependLeft(link);
            }
        }

        internal bool TrySelect()
        {
            Debug.Assert(
                State == PossibleValueState.UNKNOWN,
                $"{nameof(PossibleSquareValue)} at {Square.Coordinate} with value {ValueIndex} was selected while in state {State}.");
            Debug.Assert(
                FirstLink != null,
                $"{nameof(PossibleSquareValue)} at {Square.Coordinate} with value {ValueIndex} was selected while {nameof(FirstLink)} was null.");
            if (!_TryUpdateLinks(link => link.TrySelectForConstraint(), link => link.DeselectForConstraint()))
            {
                return false;
            }
            State = PossibleValueState.SELECTED;
            return true;
        }

        internal void Deselect()
        {
            Debug.Assert(
                State == PossibleValueState.SELECTED,
                $"{nameof(PossibleSquareValue)} at {Square.Coordinate} with value {ValueIndex} was deselected while in state {State}.");
            Debug.Assert(
                FirstLink != null,
                $"{nameof(PossibleSquareValue)} at {Square.Coordinate} with value {ValueIndex} was deselected while {nameof(FirstLink)} was null.");
            State = PossibleValueState.UNKNOWN;
            _RevertLinks(link => link.DeselectForConstraint());
        }

        internal bool TryDrop()
        {
            Debug.Assert(
                State == PossibleValueState.UNKNOWN,
                $"{nameof(PossibleSquareValue)} at {Square.Coordinate} with value {ValueIndex} was cleared while in state {State}.");
            if (Square.NumPossibleValues == 1)
            {
                return false;
            }
            if (FirstLink != null)
            {
                if (!_TryUpdateLinks(link => link.TryRemoveFromConstraint(), link => link.ReturnToConstraint()))
                {
                    return false;
                }
            }
            Square.NumPossibleValues--;
            State = PossibleValueState.DROPPED;
            return true;
        }

        internal void Return()
        {
            Debug.Assert(
                State == PossibleValueState.DROPPED,
                $"{nameof(PossibleSquareValue)} at {Square.Coordinate} with value {ValueIndex} was returned while in state {State}.");
            Debug.Assert(
                FirstLink != null,
                $"{nameof(PossibleSquareValue)} at {Square.Coordinate} with value {ValueIndex} was returned while {nameof(FirstLink)} was null.");
            State = PossibleValueState.UNKNOWN;
            Square.NumPossibleValues++;
            _RevertLinks(link => link.ReturnToConstraint());
        }

        internal int GetMinConstraintCount()
        {
            Debug.Assert(
                FirstLink != null,
                $"Called {nameof(GetMinConstraintCount)} on {nameof(PossibleSquareValue)} at {Square.Coordinate} with value {ValueIndex} while {nameof(FirstLink)} was null.");
            int minCount = FirstLink.Constraint.CountUnselected;
            Link link = FirstLink.Right;
            while (link != FirstLink)
            {
                int count = link.Constraint.CountUnselected;
                if (count < minCount)
                {
                    minCount = count;
                }
                link = link.Right;
            }
            return minCount;
        }

        private bool _TryUpdateLinks(Func<Link, bool> tryFn, Action<Link> undoFn)
        {
            Debug.Assert(
                FirstLink != null,
                $"{nameof(PossibleSquareValue)} at {Square.Coordinate} with value {ValueIndex} called {nameof(_TryUpdateLinks)} while {nameof(FirstLink)} was null.");
            Link link = FirstLink;
            do
            {
                if (!tryFn(link))
                {
                    while (link != FirstLink)
                    {
                        link = link.Left;
                        undoFn(link);
                    }
                    return false;
                }
                link = link.Right;
            } while (link != FirstLink);
            return true;
        }

        private void _RevertLinks(Action<Link> fn)
        {
            Debug.Assert(
                FirstLink != null,
                $"{nameof(PossibleSquareValue)} at {Square.Coordinate} with value {ValueIndex} called {nameof(_RevertLinks)} while {nameof(FirstLink)} was null.");
            Link lastLink = FirstLink.Left;
            Link link = lastLink;
            do
            {
                fn(link);
                link = link.Left;
            } while (link != lastLink);
        }
    }
}