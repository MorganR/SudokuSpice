using System.Diagnostics;

namespace SudokuSpice.Data
{
    public class SquareLink
    {
        internal readonly PossibleSquareValue PossibleSquare;
        internal readonly ConstraintHeader Constraint;
        internal SquareLink Left;
        internal SquareLink Right;
        internal SquareLink Up;
        internal SquareLink Down;

        public SquareLink(PossibleSquareValue possibleSquare, ConstraintHeader constraint)
        {
            PossibleSquare = possibleSquare;
            if (possibleSquare.FirstLink is null)
            {
                possibleSquare.FirstLink = this;
                Right = Left = this;
            }
            else
            {
                Right = possibleSquare.FirstLink;
                Left = Right.Left;
                Right.Left = this;
                Left.Right = this;
            }
            Constraint = constraint;
            if (constraint.FirstLink is null)
            {
                constraint.FirstLink = this;
                Down = Up = this;
            }
            else
            {
                Down = constraint.FirstLink;
                Up = Down.Up;
                Down.Up = this;
                Up.Down = this;
            }
            Constraint.Count++;
        }

        internal bool TryRemoveFromConstraint()
        {
            // If the constraint is already satisfied then we can skip this.
            if (Constraint.IsSatisfied)
            {
                return true;
            }
            if (Constraint.Count == 1)
            {
                return false;
            }
            Down.Up = Up;
            Up.Down = Down;
            Constraint.Count--;
            if (Constraint.FirstLink == this)
            {
                Constraint.FirstLink = Down;
            }
            return true;
        }

        internal void ReturnToConstraint()
        {
            // If the constraint is satisfied then we can skip this since this link was never removed.
            if (Constraint.IsSatisfied)
            {
                return;
            }
            Down.Up = this;
            Up.Down = this;
            Constraint.Count++;
        }

        internal bool TrySatisfyConstraint()
        {
            Debug.Assert(!Constraint.IsSatisfied, $"Constraint was already satisfied when selecting square {PossibleSquare.Square.Coordinate}, value: {PossibleSquare.ValueIndex}.");
            Constraint.IsSatisfied = true;
            var link = Down;
            while (link != this)
            {
                if (!link.PossibleSquare.TryDrop())
                {
                    link = link.Up;
                    while (link != this)
                    {
                        link.PossibleSquare.Return();
                        link = link.Up;
                    }
                    Constraint.IsSatisfied = false;
                    return false;
                }
                link = link.Down;
            }
            return true;
        }

        internal void UnsatisfyConstraint()
        {
            Debug.Assert(Constraint.IsSatisfied, $"Constraint was not satisfied when deselecting square {PossibleSquare.Square.Coordinate}, value: {PossibleSquare.ValueIndex}.");
            Debug.Assert(Constraint.GetLinks().Contains(this), $"Constraint was missing possible square {PossibleSquare.Square.Coordinate}, value: {PossibleSquare.ValueIndex} when unsatisfying constraint.");
            var link = Up;
            while (link != this)
            {
                link.PossibleSquare.Return();
                link = link.Up;
            }
            Constraint.IsSatisfied = false;
        }
    }
}
