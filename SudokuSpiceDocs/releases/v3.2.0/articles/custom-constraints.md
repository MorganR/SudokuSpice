# Custom Constraints

Let's continue the [custom rules example](custom-rules.md), where we want to solve a puzzle that also enforces
that the diagonals contain all unique values. In this case, however, we'll solve this by adding a
new constraint: the
[`DiagonalUniquenessConstraint`](xref:SudokuSpice.ConstraintBased.Constraints.DiagonalUniquenessConstraint).

If you haven't yet read the [constraints summary](framework.md#important-concepts), read that first!

## Creating a constraint

A single constraint object can be used to solve multiple puzzles. Where possible, a constraint
should be flexible to multiple puzzle sizes.

### The `TryConstrain` method

The `IConstraint.TryConstrain` operation adds new
[`Objective`s](xref:SudokuSpice.ConstraintBased.Objective) to the given
[`ExactCoverGraph`](xref:SudokuSpice.ConstraintBased.ExactCoverGraph). It must also drop any
[`Possibility`s](xref:SudokuSpice.ConstraintBased.Possibility) that are now impossible based on
applying this constraint to the puzzle's preset values.

It should return false if the given puzzle cannot satisfy this constraint, else true.

#### Define your objectives

The first thing we need to identify is what an objective looks like for this constraint. We could
state this constraint as follows:

> Each forward diagonal, and each backward diagonal, needs to contain all possible values.

We can implement this by adding an objective for each possible value on each diagonal.

#### Determine possible coordinates

The possible coordinates here are those on the forward diagonal, and those on the backward diagonal.
We can find all the coordinates on the forward diagonal as follows:

```csharp
public bool TryConstrain(TPuzzle puzzle, ExactCoverGraph graph)
{
    Span<Coordinate> forwardDiagonalCoordinates = stackalloc Coordinate[puzzle.Size];
    for (int row = 0, col = puzzle.Size - 1;  row < puzzle.Size; row++, col--)
    {
        forwardDiagonalCoordinates[row] = new Coordinate(row, col);
    }

    // TODO: Get backward diagonal coordinates.
}
```

#### Identify possible values

We need to identify which values are actually possible on each diagonal based on the preset
values. Within the constraint-based solver, we map a puzzle's possible values to zero-based indices
based on the order they are returned when we first call `puzzle.AllPossibleValues`. This mapping is
handled by the `ExactCoverGraph`.

```csharp
public bool TryConstrain(TPuzzle puzzle, ExactCoverGraph graph)
{
    ...

    Span<bool> isConstraintSatisfiedAtIndex =
            stackalloc bool[graph.AllPossibleValues.Length];
    // Zero the array since stackalloc does not guarantee this.
    isConstraintSatisfiedAtIndex.Clear();
    for (int i = 0; i < forwardDiagonalCoordinates.Length; i++)
    {
        var puzzleValue = puzzle[forwardDiagonalCoordinates[i]];
        if (puzzleValue.HasValue)
        {
            var possibilityIndex = graph.ValuesToIndices[puzzleValue.Value];
            if (isConstraintSatisfiedAtIndex[possibilityIndex])
            {
                // This value is duplicated on the diagonal. This is not allowed.
                return false;
            }
            isConstraintSatisfiedAtIndex[possibilityIndex] = true;
        }
    }

    // TODO: Check this for backward diagonal coordinates.
}
```

#### Add objectives and drop impossible possibilities

Now we can iterate through each possible value on each diagonal, drop possibilities that are no
longer possible, and add constraint headers for the rest.

```csharp
public bool TryConstrain(TPuzzle puzzle, ExactCoverGraph graph)
{
    ...

	Possibility?[]?[] squares = new Possibility[forwardDiagonalCoordinates.Length][];
    for (int i = 0; i < squares.Length; i++)
    {
		squares[i] = graph.GetAllPossibilitiesAt(in squareCoordinates[i]);
    }
    for (
        int possibilityIndex = 0;
        possibilityIndex < isConstraintSatisfiedAtIndex.Length;
        possibilityIndex++)
    {
        if (isConstraintSatisfiedAtIndex[possibilityIndex])
        {
			if (!ConstraintUtil.TryDropPossibilitiesAtIndex(squares, possibilityIndex))
			{
				return false;
			}
			continue;
        }
        // Each possible value should only be present once on the diagonal, so set
        // requiredCount to 1. The objective will be linked into the graph, so discard the out
        // parameter.
		if (!ConstraintUtil.TryAddObjectiveForPossibilityIndex(
            squares, possibilityIndex, graph, requiredCount: 1, objective: out _))
		{
			return false;
		}
    }

    // TODO: Add objectives and drop possibilities for the backward diagonal.

    ...

    return true;
}
```

Note that here we made use of the
[`ConstraintUtil`](xref:SudokuSpice.ConstraintBased.Constraints.ConstraintUtil) to easily
add objectives and drop possibilities. This provides a few useful functions for implementing
constraints. In fact, we could have replaced all of this work for handling the forward diagonal
with the following:

```csharp
Span<Coordinate> coordinates = stackalloc Coordinate[puzzle.Size];
for (int row = 0, col = puzzle.Size - 1;  row < puzzle.Size; row++, col--)
{
    coordinates[row] = new Coordinate(row, col);
}
return ConstraintUtil.TryImplementUniquenessConstraintForSquares(puzzle, coordinates, graph);
```
