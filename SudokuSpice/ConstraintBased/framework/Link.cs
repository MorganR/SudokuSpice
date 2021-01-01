﻿namespace SudokuSpice.ConstraintBased
{
    internal class Link
    {
        internal readonly PossibleSquareValue PossibleSquareValue;
        internal readonly ConstraintHeader Constraint;
        internal Link Left { get; private set; }
        internal Link Right { get; private set; }
        internal Link Up { get; private set; }
        internal Link Down { get; private set; }

        private Link(PossibleSquareValue possibleValue, ConstraintHeader constraint)
        {
            this.PossibleSquareValue = possibleValue;
            Constraint = constraint;
            Up = Down = Right = Left = this;
        }

        internal static Link CreateConnectedLink(PossibleSquareValue possibleValue, ConstraintHeader header)
        {
            var squareLink = new Link(possibleValue, header);
            possibleValue.Attach(squareLink);
            header.Attach(squareLink);
            return squareLink;
        }

        internal bool TryRemoveFromConstraint()
        {
            // If the constraint is already satisfied then we can skip this.
            if (Constraint.IsSatisfied)
            {
                return true;
            }
            return Constraint.TryDetach(this);
        }

        internal void ReturnToConstraint()
        {
            // If the constraint is satisfied then we can skip this since this link was never removed.
            if (Constraint.IsSatisfied)
            {
                return;
            }
            Constraint.Reattach(this);
        }

        internal bool TrySatisfyConstraint() => Constraint.TrySatisfyFrom(this);

        internal void UnsatisfyConstraint() => Constraint.UnsatisfyFrom(this);

        internal void AppendRight(Link toAppend)
        {
            toAppend.Right = Right;
            toAppend.Left = this;
            Right.Left = toAppend;
            Right = toAppend;
        }

        internal void AppendDown(Link toAppend)
        {
            toAppend.Down = Down;
            toAppend.Up = this;
            Down.Up = toAppend;
            Down = toAppend;
        }

        internal void PrependLeft(Link toPrepend)
        {
            toPrepend.Right = this;
            toPrepend.Left = Left;
            Left.Right = toPrepend;
            Left = toPrepend;
        }

        internal void PrependUp(Link toPrepend)
        {
            toPrepend.Down = this;
            toPrepend.Up = Up;
            Up.Down = toPrepend;
            Up = toPrepend;
        }

        internal void PopVertically()
        {
            Up.Down = Down;
            Down.Up = Up;
        }

        internal void ReinsertVertically()
        {
            Up.Down = this;
            Down.Up = this;
        }
    }
}