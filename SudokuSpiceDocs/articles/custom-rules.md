# Custom Rules

Let's say we want to solve a puzzle that also enforces that the diagonals contain all unique
values. In this case, we can use the standard [`Puzzle`](xref:SudokuSpice.Puzzle) to store the
data, but we need to add a custom rule. In this example, we'll go through the steps for
implementing and using the
[`DiagonalUniquenessRule`](xref:SudokuSpice.RuleBased.Rules.DiagonalUniquenessRule).

## Creating a rule

### Constructor

The new rule needs to extend the [`ISudokuRule`](xref:SudokuSpice.RuleBased.Rules.ISudokuRule), and we'll
need to have some way of tracking the set of values that are available in each diagonal. For this
we'll use the [`BitVector`](xref:SudokuSpice.BitVector) struct, which provides an efficient
set-like struct using the bits of a `uint`.

```csharp
public class DiagonalUniquenessRule : ISudokuRule
{
    private readonly IReadOnlyPuzzle _puzzle;
    private readonly BitVector _allUnset;
    private BitVector _unsetBackwardDiag;
    private BitVector _unsetForwardDiag;

    public DiagonalUniquenessRule(IReadOnlyPuzzle puzzle, BitVector allUniqueValues)
    {
        Debug.Assert(puzzle.Size == allUniqueValues.Count,
            $"Can't enforce box uniqueness for mismatched puzzle size {puzzle.Size} and number of unique values {allUniqueValues.Count}");
        _puzzle = puzzle;
        _allUnset = _unsetForwardDiag = _unsetBackwardDiag = allUniqueValues;
        // Iterate through the backward diagonal (like a backslash '\')
        for (int row = 0, col = 0; row < puzzle.Size; row++, col++)
        {
            var val = puzzle[row, col];
            if (val.HasValue)
            {
                if (!_unsetBackwardDiag.IsBitSet(val.Value))
                {
                    throw new ArgumentException(
                            $"Puzzle does not satisfy diagonal uniqueness rule at ({row}, {col}).");
                    }
                    _unsetBackwardDiag.UnsetBit(val.Value);
                }
            }
        }
        
        // TODO: Validate the forward diagonal, and update _unsetForwardDiag accordingly.
    }
}
```

### GetPossibleValues

Now we need to implement the `ISudokuRule.GetPossibleValues` operation, which provides the possible
values for any square according to this rule. You might be wondering, what should we return if the
given coordinate is not on one of the diagonals? In that case, we should return *all* possible
values! We definitely don't want to return no possible values, because that would make any puzzle
with unset squares off the diagonal impossible.

```csharp
public BitVector GetPossibleValues(in Coordinate c)
{
    if (_IsOnBackwardDiag(in c))
    {
        return _unsetBackwardDiag;
    } else if (_IsOnForwardDiag(in c))
    {
        return _unsetForwardDiag;
    } else
    {
        return _allUnset;
    }
}

private static bool _IsOnBackwardDiag(in Coordinate c)
{
    return c.Row == c.Column;
}

private bool _IsOnForwardDiag(in Coordinate c)
{
    return c.Column == _puzzle.Size - c.Row - 1;
}
```

### Update

Great, now we need to provide a way for the rule keeper to update this rule. When the rule keeper
wants to set a square's value, it will call `ISudokuRule.Update` with the location and new value.
It will also include a [`CoordinateTracker`](xref:SudokuSpice.CoordinateTracker), with which
this rule needs to track any coordinates whose possible values have changed.

```csharp
public void Update(in Coordinate c, int val, CoordinateTracker coordTracker)
{
    if (_IsOnBackwardDiag(in c))
    {
        // Remove this value from the list of possible values.
        _unsetBackwardDiag.UnsetBit(val);
        _AddUnsetFromBackwardDiag(in c, coordTracker);
    }
    // TODO: Handle an update that's on the forward diagonal.
}

private void _AddUnsetFromBackwardDiag(in Coordinate c, CoordinateTracker coordTracker)
{
    // Iterate along the backward diagonal, tracking the coordinates of any unset squares.
    for (int row = 0, col = 0; row < _puzzle.Size; row++, col++)
    {
        // Make sure to skip the square that is currently being updated! Its value is still unset.
        if ((row == c.Row && col == c.Column) || _puzzle[row, col].HasValue)
        {
            continue;
        }
        coordTracker.AddOrTrackIfUntracked(new Coordinate(row, col));
    }
}
```

### Revert

Ok, now what if that update needs to be reverted, for example if the solver made a wrong guess?
Let's implement the `ISudokuRule.Revert` methods. These should both perform roughly the same
changes: reverting the changes made during the `Update` method. However, in one we can skip
tracking the affected square's in the `CoordinateTracker`.

```csharp
public void Revert(in Coordinate c, int val)
{
    if (_IsOnBackwardDiag(in c))
    {
        _unsetBackwardDiag.SetBit(val);
    }
    // TODO: Handle the case when the square is on the forward diagonal
}

public void Revert(in Coordinate c, int val, CoordinateTracker coordTracker)
{
    if (_IsOnBackwardDiag(in c))
    {
        _unsetBackwardDiag.SetBit(val);
        _AddUnsetFromBackwardDiag(in c, coordTracker);
    }
    // TODO: Handle the case when the square is on the forward diagonal
}
```

### CopyWithNewReference

Lastly, we must implement the `ISudokuRule.CopyWithNewReference` method to provide a deep copy for
the
[`Solver.GetStatsForAllSolutions`](xref:SudokuSpice.RuleBased.Solver#SudokuSpice_RuleBased_Solver_GetStatsForAllSolutions)
method and for the [`PuzzleGenerator`](xref:SudokuSpice.RuleBased.PuzzleGenerator`1).

```csharp
public ISudokuRule CopyWithNewReference(IReadOnlyPuzzle puzzle)
{
    return new DiagonalUniquenessRule(this, puzzle);
}

private DiagonalUniquenessRule(DiagonalUniquenessRule existing, IReadOnlyPuzzle puzzle)
{
    _puzzle = puzzle;
    // BitVectors are structs, so they are copied on assignment.
    _unsetBackwardDiag = existing._unsetBackwardDiag;
    _unsetForwardDiag = existing._unsetForwardDiag;
    _allUnset = existing._allUnset;
}
```

## Using the new rule

Now let's assume we have a `puzzle` already that we want to solve with this rule. We can solve it
as follows:

```csharp
var possibleValues = new PossibleValues(puzzle);
var solver = new Solver(
    puzzle,
    possibleValues,
    new DynamicRuleKeeper(
        puzzle,
        possibleValues,
        new List<ISudokuRule>()
        {
            new StandardRules(puzzle, possibleValues.AllPossible),
            new DiagonalUniquenessRule(puzzle, possibleValues.AllPossible),
		}));
solver.Solve();
```

Similarly, we can generate new puzzles that follow this rule as below:

```csharp
var generator = new PuzzleGenerator<Puzzle>(
    () => new Puzzle(/*size=*/16),
    puzzle => {
        var possibleValues = new PossibleValues(puzzle);
        return new Solver(
            puzzle,
            possibleValues,
            new DynamicRuleKeeper(
                puzzle,
                possibleValues,
                new List<ISudokuRule>()
                {
                    new StandardRules(puzzle, possibleValues.AllPossible),
                    new DiagonalUniquenessRule(puzzle, possibleValues.AllPossible),
		        }));
    });
```

Remember to include heuristics for the fastest performance!