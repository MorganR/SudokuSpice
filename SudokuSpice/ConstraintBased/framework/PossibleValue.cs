using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace SudokuSpice.ConstraintBased
{
    /// <summary>
    /// Represents a row in the <see cref="ExactCoverMatrix{TPuzzle}"/>. This represents a possible value
    /// for a given <see cref="Square{TPuzzle}"/>.
    /// </summary>
    public class PossibleValue<TPuzzle> where TPuzzle : IReadOnlyPuzzle
    {
        /// <summary>
        /// Gets the index of the possible value that this represents. This index corresponds with
        /// <see cref="ExactCoverMatrix{TPuzzle}.AllPossibleValues"/>.
        /// </summary>
        public int ValueIndex { get; }
        /// <summary>
        /// Gets the square that this is a possible value for.
        /// </summary>
        public Square<TPuzzle> Square { get; }
        /// <summary>
        /// Gets the state of this row.
        /// </summary>
        public PossibleSquareState State { get; private set; }
        [DisallowNull]
        internal Link<TPuzzle>? FirstLink { get; private set; }

        internal PossibleValue(Square<TPuzzle> square, int valueIndex)
        {
            ValueIndex = valueIndex;
            Square = square;
            State = PossibleSquareState.UNKNOWN;
        }

        internal void Attach(Link<TPuzzle> link)
        {
            if (FirstLink is null)
            {
                FirstLink = link;
            } else
            {
                link.Right = FirstLink;
                link.Left = link.Right.Left;
                link.Right.Left = link;
                link.Left.Right = link;
            }
        }

        internal bool TrySelect()
        {
            Debug.Assert(State == PossibleSquareState.UNKNOWN, $"PossibleSquare at {Square.Coordinate} with value {ValueIndex} was selected while in state {State}.");
            Debug.Assert(FirstLink != null, $"PossibleSquare at {Square.Coordinate} with value {ValueIndex} was selected while FirstLink was null.");
            if (!_TryUpdateLinks(link => link.TrySatisfyConstraint(), link => link.UnsatisfyConstraint()))
            {
                return false;
            }
            State = PossibleSquareState.SELECTED;
            return true;
        }

        internal void Deselect()
        {
            Debug.Assert(State == PossibleSquareState.SELECTED, $"PossibleSquare at {Square.Coordinate} with value {ValueIndex} was deselected while in state {State}.");
            Debug.Assert(FirstLink != null, $"PossibleSquare at {Square.Coordinate} with value {ValueIndex} was deselected while FirstLink was null.");
            State = PossibleSquareState.UNKNOWN;
            _RevertLinks(link => link.UnsatisfyConstraint());
        }

        internal bool TryDrop()
        {
            Debug.Assert(State == PossibleSquareState.UNKNOWN, $"PossibleSquare at {Square.Coordinate} with value {ValueIndex} was cleared while in state {State}.");
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
            State = PossibleSquareState.DROPPED;
            return true;
        }

        internal void Return()
        {
            Debug.Assert(State == PossibleSquareState.DROPPED, $"PossibleSquare at {Square.Coordinate} with value {ValueIndex} was returned while in state {State}.");
            Debug.Assert(FirstLink != null, $"PossibleSquare at {Square.Coordinate} with value {ValueIndex} was returned while FirstLink was null.");
            State = PossibleSquareState.UNKNOWN;
            Square.NumPossibleValues++;
            _RevertLinks(link => link.ReturnToConstraint());
        }

        internal int GetMinConstraintCount()
        {
            Debug.Assert(FirstLink != null, $"Called {nameof(GetMinConstraintCount)} on PossibleSquare at {Square.Coordinate} with value {ValueIndex} while FirstLink was null.");
            int minCount = FirstLink.Constraint.Count;
            Link<TPuzzle>? link = FirstLink.Right;
            while (link != FirstLink)
            {
                if (link.Constraint.Count < minCount)
                {
                    minCount = link.Constraint.Count;
                }
                link = link.Right;
            }
            return minCount;
        }

        private bool _TryUpdateLinks(Func<Link<TPuzzle>, bool> tryFn, Action<Link<TPuzzle>> undoFn)
        {
            Debug.Assert(FirstLink != null, $"PossibleSquare at {Square.Coordinate} with value {ValueIndex} called {nameof(_TryUpdateLinks)} while FirstLink was null.");
            Link<TPuzzle>? link = FirstLink;
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

        private void _RevertLinks(Action<Link<TPuzzle>> fn)
        {
            Debug.Assert(FirstLink != null, $"PossibleSquare at {Square.Coordinate} with value {ValueIndex} called {nameof(_RevertLinks)} while FirstLink was null.");
            Link<TPuzzle>? lastLink = FirstLink.Left;
            Link<TPuzzle>? link = lastLink;
            do
            {
                fn(link);
                link = link.Left;
            } while (link != lastLink);
        }
    }
}
